using PicoPinnedArray;
using PicoStatus;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PS4000Lib
{
    public enum Scale
    {
        mV,
        ADC_Counts,
    }

    struct ChannelSettings
    {
        public bool DCcoupled;
        public Range range;
        public bool enabled;
    }

    public class Pwq
    {
        public PwqConditions[] conditions;
        public short nConditions;
        public ThresholdDirection direction;
        public uint lower;
        public uint upper;
        public PulseWidthType type;

        public Pwq(PwqConditions[] conditions,
                    short nConditions,
                    ThresholdDirection direction,
                    uint lower, uint upper,
                    PulseWidthType type)
        {
            this.conditions = conditions;
            this.nConditions = nConditions;
            this.direction = direction;
            this.lower = lower;
            this.upper = upper;
            this.type = type;
        }
    }

    public class PS4000 : IDisposable
    {
        #region const
        public const int BUFFER_SIZE = 1024;
        public const int MAX_CHANNELS = 4;
        public const int QUAD_SCOPE = 4;
        public const int DUAL_SCOPE = 2;
        readonly IReadOnlyList<ushort> InputRanges = new ushort[] { 10, 20, 50, 100, 200, 500, 1000, 2000, 5000, 10000, 20000, 50000 };
        #endregion

        #region field
        short _handle;

        string _devInfo = "";

        short[][] appBuffers;
        short[][] buffers;

        private bool _ready = false;
        private short _trig = 0;
        private uint _trigAt = 0;
        private int _sampleCount = 0;
        private uint _startIndex = 0;
        private bool _autoStop;
        private ChannelSettings[] _channelSettings;
        private int _channelCount;

        private List<TriggerChannelProperties> _sourceDetails;
        private List<TriggerConditions> _conditions;
        private List<ThresholdDirection> _directions;

        private Range _firstRange;
        private Range _lastRange;
        private uint _timebase;
        private short _oversample;
        private bool _scaleVoltages = true;
        private NativeMethods.ps4000BlockReady _callbackDelegate;
        #endregion

        #region ctor
        public PS4000()
        {
            _channelSettings = new ChannelSettings[MAX_CHANNELS];
            for (int i = 0; i < MAX_CHANNELS; i++)
            {
                _channelSettings[i].enabled = true;
                _channelSettings[i].DCcoupled = true;
                _channelSettings[i].range = Range.Range_5V;
            }

            ResetTriggerProperties();
            ResetTriggerConditions();
            ResetTriggerThresholdDirection();
        }

        private bool _disposed = false;

        ~PS4000()
        {
            this.Dispose(false);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    Close();
                }
                this._disposed = true;
            }
        }
        #endregion

        #region property
        public string Settings
        {
            get
            {
                var res = new StringBuilder();

                var scale = _scaleVoltages ? "mV" : "ADC counts";
                res.Append($"Readings will be scaled in {scale}");

                for (int ch = 0; ch < _channelCount; ch++)
                {
                    var voltage = InputRanges[(int)_channelSettings[ch].range];
                    res.Append($"\nChannel {(char)('A' + ch)} Voltage Range = ");
                    res.Append(voltage < 1000 ? $"{voltage}mV" : $"{voltage / 1000}V");
                }

                return res.ToString();
            }
        }

        public string DeviceInfo
        {
            get
            {
                if (string.IsNullOrEmpty(_devInfo))
                {
                    var variant = 0;
                    var description = new[]{
                           "Driver Version    ",
                           "USB Version       ",
                           "Hardware Version  ",
                           "Variant Info      ",
                           "Serial            ",
                           "Cal Date          ",
                           "Kernel Ver        "
                         };
                    var line = new StringBuilder(80);
                    var result = new StringBuilder();

                    if (_handle >= 0)
                    {
                        for (int i = 0; i < 7; i++)
                        {
                            NativeMethods.GetUnitInfo(_handle, line, 80, out short _, i);

                            if (i == 3)
                            {
                                line.Length = 4;
                                variant = Convert.ToInt16(line.ToString());
                            }

                            if (i != 0) result.Append("\n");
                            result.Append($"{description[i]}: {line}");
                        }

                        switch (variant)
                        {
                            case (int)Model.PS4223:
                                _firstRange = Range.Range_50MV;
                                _lastRange = Range.Range_100V;
                                _channelCount = DUAL_SCOPE;
                                break;
                            case (int)Model.PS4224:
                                _firstRange = Range.Range_50MV;
                                _lastRange = Range.Range_20V;
                                _channelCount = DUAL_SCOPE;
                                break;
                            case (int)Model.PS4423:
                                _firstRange = Range.Range_50MV;
                                _lastRange = Range.Range_100V;
                                _channelCount = QUAD_SCOPE;
                                break;
                            case (int)Model.PS4424:
                                _firstRange = Range.Range_50MV;
                                _lastRange = Range.Range_20V;
                                _channelCount = QUAD_SCOPE;
                                break;
                            case (int)Model.PS4226:
                                _firstRange = Range.Range_50MV;
                                _lastRange = Range.Range_20V;
                                _channelCount = DUAL_SCOPE;
                                break;
                            case (int)Model.PS4227:
                                _firstRange = Range.Range_50MV;
                                _lastRange = Range.Range_20V;
                                _channelCount = DUAL_SCOPE;
                                break;
                            case (int)Model.PS4262:
                                _firstRange = Range.Range_10MV;
                                _lastRange = Range.Range_20V;
                                _channelCount = DUAL_SCOPE;
                                break;
                        }
                    }
                    return result.ToString();
                }
                else
                {
                    return _devInfo;
                }
            }
        }

        public Pwq Pwq { get; set; } = null;
        #endregion

        #region controll
        public void Open()
        {
            var status = NativeMethods.OpenUnit(out short handle);

            if (status != StatusCodes.PICO_OK)
                throw new IOException($"Error. Cannot open device. Code: {status}");
            else
                _handle = handle;

            _devInfo = DeviceInfo;
            SetChannel();

            var timebase = 1u;
            SetTimebase(ref timebase, out int _);
        }

        public void Close()
        {
            NativeMethods.CloseUnit(_handle);
        }
        #endregion

        #region get data
        public string CollectBlockImmediate()
        {
            SetTrigger(null, null, null, null, 0, 0, 0);
            return BlockDataHandler();
        }

        public string CollectBlockTriggered()
        {
            SetTrigger(_sourceDetails.ToArray(), _conditions.ToArray(), _directions.ToArray(), Pwq, 0, 0, 0);
            return BlockDataHandler();
        }

        public short[][][] CollectBlockRapid(ushort numRapidCaptures)
        {
            if (NativeMethods.SetNoOfRapidCaptures(_handle, numRapidCaptures) > 0)
            {
                throw new InvalidOperationException();
            }

            NativeMethods.MemorySegments(_handle, numRapidCaptures, out int maxSamples);

            SetTrigger(null, null, null, null, 0, 0, 0);

            return RapidBlockDataHandler(numRapidCaptures);
        }

        public string CollectStreamingImmediate()
        {
            SetTrigger(null, null, null, null, 0, 0, 0);
            return StreamDataHandler(0);
        }
        public string CollectStreamingTriggered()
        {
            SetTrigger(_sourceDetails.ToArray(), _conditions.ToArray(), _directions.ToArray(), null, 0, 0, 0);
            return StreamDataHandler(100000);
        }

        private short[][][] RapidBlockDataHandler(ushort nRapidCaptures)
        {
            short status;
            int numChannels = _channelCount;
            uint numSamples = BUFFER_SIZE;

            // Run the rapid block capture
            _ready = false;

            _callbackDelegate = BlockCallback;

            NativeMethods.RunBlock(_handle,
                        0,
                        (int)numSamples,
                        _timebase,
                        _oversample,
                        out int timeIndisposed,
                        0,
                        _callbackDelegate,
                        IntPtr.Zero);

            while (!_ready)
            {
                Thread.Sleep(100);
            }

            NativeMethods.Stop(_handle);

            var values = new short[nRapidCaptures][][];
            var pinned = new PinnedArray<short>[nRapidCaptures, numChannels];

            for (ushort segment = 0; segment < nRapidCaptures; segment++)
            {
                values[segment] = new short[numChannels][];
                for (short channel = 0; channel < numChannels; channel++)
                {
                    if (_channelSettings[channel].enabled)
                    {
                        values[segment][channel] = new short[numSamples];
                        pinned[segment, channel] = new PinnedArray<short>(values[segment][channel]);

                        status = NativeMethods.SetDataBuffersRapid(_handle,
                                               (Channel)channel,
                                               values[segment][channel],
                                               (int)numSamples,
                                               segment);
                    }
                    else
                    {
                        status = NativeMethods.SetDataBuffersRapid(_handle,
                                   (Channel)channel,
                                    null,
                                    0,
                                    segment);

                    }
                }
            }

            var overflows = new short[nRapidCaptures];

            status = NativeMethods.GetValuesRapid(_handle, ref numSamples, 0, (ushort)(nRapidCaptures - 1), overflows);

            foreach (PinnedArray<short> p in pinned)
                p?.Dispose();

            return values;
        }
        private string BlockDataHandler()
        {
            var res = string.Empty;

            uint sampleCount = BUFFER_SIZE;
            var minPinned = new PinnedArray<short>[_channelCount];
            var maxPinned = new PinnedArray<short>[_channelCount];

            for (int i = 0; i < _channelCount; i++)
            {
                var minBuffers = new short[sampleCount];
                var maxBuffers = new short[sampleCount];
                minPinned[i] = new PinnedArray<short>(minBuffers);
                maxPinned[i] = new PinnedArray<short>(maxBuffers);
                NativeMethods.SetDataBuffers(_handle, (Channel)i, maxBuffers, minBuffers, (int)sampleCount);
            }

            /*  Verify the currently selected timebase index, and the maximum number of samples per channel with the current settings. */
            int timeInterval;

            while (NativeMethods.GetTimebase(_handle, _timebase, (int)sampleCount, out timeInterval, _oversample, out int maxSamples, 0) != 0)
                _timebase++;

            /* Start it collecting, then wait for completion*/
            _ready = false;
            _callbackDelegate = BlockCallback;

            NativeMethods.RunBlock(_handle, 0, (int)sampleCount, _timebase, _oversample, out int timeIndisposed, 0, _callbackDelegate, IntPtr.Zero);

            while (!_ready)
            {
                Thread.Sleep(100);
            }

            NativeMethods.Stop(_handle);

            if (_ready)
            {
                NativeMethods.GetValues(_handle, 0, ref sampleCount, 1, DownSamplingMode.None, 0, out short overflow);
                res = FormatBlockData(Math.Min(sampleCount, BUFFER_SIZE), timeInterval, minPinned, maxPinned);
            }
            else
                res = "data collection aborted";

            foreach (PinnedArray<short> p in minPinned) p?.Dispose();
            foreach (PinnedArray<short> p in maxPinned) p?.Dispose();

            return res;
        }
        private string StreamDataHandler(uint preTrigger)
        {
            var res = string.Empty;
            uint sampleCount = BUFFER_SIZE * 10; /*  *10 is to make sure buffer large enough */
            appBuffers = new short[_channelCount * 2][];
            buffers = new short[_channelCount * 2][];

            var totalsamples = 0u;
            var triggeredAt = 0u;
            short status;

            var sampleInterval = 1u;

            for (int ch = 0; ch < _channelCount * 2; ch += 2)
            {
                appBuffers[ch] = new short[sampleCount];
                appBuffers[ch + 1] = new short[sampleCount];

                buffers[ch] = new short[sampleCount];
                buffers[ch + 1] = new short[sampleCount];

                status = NativeMethods.SetDataBuffers(_handle, (Channel)(ch / 2), buffers[ch], buffers[ch + 1], (int)sampleCount);
            }

            _autoStop = false;
            status = NativeMethods.RunStreaming(_handle, ref sampleInterval, ReportedTimeUnits.MicroSeconds,
                                        preTrigger, 1000000 - preTrigger, true, 1000, sampleCount);

            // Build File Header
            var sb = new StringBuilder();
            var heading = new[] { "Channel", "Max ADC", "Max mV", "Min ADC", "Min mV" };

            sb.AppendLine("For each of the active channels, results shown are....");
            sb.AppendLine("Maximum Aggregated value ADC Count & mV, Minimum Aggregated value ADC Count & mV");
            sb.AppendLine();

            for (int i = 0; i < _channelCount; i++)
            {
                if (_channelSettings[i].enabled)
                {
                    sb.AppendFormat("{0,10} {1,10} {2,10} {3,10} {4,10}",
                                    heading[0],
                                    heading[1],
                                    heading[2],
                                    heading[3],
                                    heading[4]);
                }
            }
            sb.AppendLine();

            while (!_autoStop)
            {
                /* Poll until data is received. Until then, GetStreamingLatestValues wont call the callback */
                Thread.Sleep(100);
                _ready = false;
                NativeMethods.GetStreamingLatestValues(_handle, StreamingCallback, IntPtr.Zero);

                if (_ready && _sampleCount > 0) /* can be ready and have no data, if autoStop has fired */
                {
                    if (_trig > 0) triggeredAt = totalsamples + _trigAt;
                    totalsamples += (uint)_sampleCount;

                    // Build File Body
                    for (uint i = _startIndex; i < (_startIndex + _sampleCount); i++)
                    {
                        for (int ch = 0; ch < _channelCount * 2; ch += 2)
                        {
                            if (_channelSettings[ch / 2].enabled)
                            {
                                sb.AppendFormat("{0,10} {1,10} {2,10} {3,10} {4,10}",
                                                (char)('A' + (ch / 2)),
                                                appBuffers[ch][i],
                                                ConvertADC2mV(appBuffers[ch][i], (int)_channelSettings[GetChannelIndex(ch / 2)].range),
                                                appBuffers[ch + 1][i],
                                                ConvertADC2mV(appBuffers[ch + 1][i], (int)_channelSettings[GetChannelIndex(ch / 2)].range));
                            }
                        }

                        sb.AppendLine();
                    }
                }
            }

            NativeMethods.Stop(_handle);

            return _autoStop ? sb.ToString() : "data collection aborted";
        }

        #endregion

        #region setting
        public void ResetTriggerProperties()
        {
            _sourceDetails = new List<TriggerChannelProperties>();
        }
        public void SetTriggerProperties(params TriggerChannelProperties[] triggerChannelProperties)
        {
            _sourceDetails = new List<TriggerChannelProperties>(triggerChannelProperties);
        }
        public void AddTriggerProperties(TriggerChannelProperties triggerChannelProperties)
        {
            if (_sourceDetails == null) ResetTriggerProperties();
            _sourceDetails.Add(triggerChannelProperties);
        }
        public void AddTriggerProperties(Channel channel, short triggerVoltage_mV) => AddTriggerProperties(channel, triggerVoltage_mV, ThresholdMode.Level);
        public void AddTriggerProperties(Channel channel, short triggerVoltage_mV, ThresholdMode thresholdMode)
        {
            var vol = ConvertmV2ADC(triggerVoltage_mV, (short)_channelSettings[(int)channel].range);
            AddTriggerProperties(new TriggerChannelProperties(
                                            vol,
                                            256 * 10,
                                            vol,
                                            256 * 10,
                                            channel,
                                            thresholdMode));
        }

        public void ResetTriggerConditions()
        {
            _conditions = new List<TriggerConditions>();
        }
        public void SetTriggerConditions(params TriggerConditions[] triggerConditions)
        {
            _conditions = new List<TriggerConditions>(triggerConditions);
        }
        public void AddTriggerConditions(TriggerConditions triggerConditions)
        {
            if (_conditions == null) ResetTriggerConditions();
            _conditions.Add(triggerConditions);
        }

        public void ResetTriggerThresholdDirection()
        {
            _directions = new List<ThresholdDirection>();
        }
        public void SetTriggerThresholdDirection(params ThresholdDirection[] thresholdDirections)
        {
            _directions = new List<ThresholdDirection>(thresholdDirections);
        }
        public void AddTriggerThresholdDirection(ThresholdDirection thresholdDirections)
        {
            if (_directions == null) ResetTriggerThresholdDirection();
            _directions.Add(thresholdDirections);
        }

        private short SetTrigger(
                            TriggerChannelProperties[] channelProperties,
                            TriggerConditions[] triggerConditions,
                            ThresholdDirection[] directions,
                            Pwq pwq,
                            uint delay,
                            short auxOutputEnabled,
                            int autoTriggerMs)
        {
            var nChannelProperties = (short)(channelProperties == null ? 0 : channelProperties.Length);
            var nTriggerConditions = (short)(triggerConditions == null ? 0 : triggerConditions.Length);

            if (directions == null)
                directions = Enumerable.Repeat(ThresholdDirection.None, 6).ToArray();
            if (pwq == null)
                pwq = new Pwq(null, 0, ThresholdDirection.None, 0, 0, PulseWidthType.None);

            var status = NativeMethods.SetTriggerChannelProperties(_handle, channelProperties, nChannelProperties,
                                                            auxOutputEnabled, autoTriggerMs);
            if (status != StatusCodes.PICO_OK) return status;

            status = NativeMethods.SetTriggerChannelConditions(_handle, triggerConditions, nTriggerConditions);
            if (status != StatusCodes.PICO_OK) return status;

            status = NativeMethods.SetTriggerChannelDirections(_handle,
                                                              directions[(int)Channel.ChannelA],
                                                              directions[(int)Channel.ChannelB],
                                                              directions[(int)Channel.ChannelC],
                                                              directions[(int)Channel.ChannelD],
                                                              directions[(int)Channel.External],
                                                              directions[(int)Channel.Aux]);
            if (status != StatusCodes.PICO_OK) return status;

            status = NativeMethods.SetTriggerDelay(_handle, delay);
            if (status != StatusCodes.PICO_OK) return status;

            status = NativeMethods.SetPulseWidthQualifier(_handle, pwq.conditions,
                                                    pwq.nConditions, pwq.direction,
                                                    pwq.lower, pwq.upper, pwq.type);
            return status;
        }

        public void SetScale(Scale scale)
        {
            switch (scale)
            {
                case Scale.mV: _scaleVoltages = true; break;
                case Scale.ADC_Counts: _scaleVoltages = false; break;
                default: break;
            }
        }

        public void SetVoltage(Channel channel, Range range)
        {
            var ch = (int)channel;
            _channelSettings[ch].range = range;
            _channelSettings[ch].enabled = true;

            SetChannel();
        }

        public void DisableChannel(Channel channel)
        {
            var ch = (int)channel;
            _channelSettings[ch].enabled = false;

            SetChannel();
        }

        public bool SetTimebase(ref uint timebase, out int timeInterval)
        {
            var valid = true;

            while (NativeMethods.GetTimebase(_handle, _timebase, BUFFER_SIZE, out timeInterval, 1, out int maxSamples, 0) != 0)
            {
                valid = false;
                timebase++;
            }

            _timebase = timebase;
            _oversample = 1;

            return valid;
        }

        private void SetChannel()
        {
            for (int i = 0; i < _channelCount; i++)
                NativeMethods.SetChannel(_handle, Channel.ChannelA + i,
                                   (short)(_channelSettings[GetChannelIndex(i)].enabled ? 1 : 0),
                                   (short)(_channelSettings[GetChannelIndex(i)].DCcoupled ? 1 : 0),
                                   _channelSettings[GetChannelIndex(i)].range);
        }
        #endregion

        #region callback
        void StreamingCallback(short handle,
                                int noOfSamples,
                                uint startIndex,
                                short ov,
                                uint triggerAt,
                                short triggered,
                                short autoStop,
                                IntPtr pVoid)
        {
            // used for streaming
            _sampleCount = noOfSamples;
            _startIndex = startIndex;
            _autoStop = autoStop != 0;

            // flag to say done reading data
            _ready = true;

            // flags to show if & where a trigger has occurred
            _trig = triggered;
            _trigAt = triggerAt;

            if (_sampleCount != 0)
            {
                for (int ch = 0; ch < _channelCount * 2; ch += 2)
                {
                    if (_channelSettings[GetChannelIndex(ch / 2)].enabled)
                    {
                        Array.Copy(buffers[ch], _startIndex, appBuffers[ch], _startIndex, _sampleCount); //max
                        Array.Copy(buffers[ch + 1], _startIndex, appBuffers[ch + 1], _startIndex, _sampleCount);//min
                    }
                }
            }
        }
        void BlockCallback(short handle, short status, IntPtr pVoid)
        {
            _ready = true;
        }
        #endregion

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int GetChannelIndex(int i) => (int)Channel.ChannelA + i;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int ConvertADC2mV(int raw, int ch) => (raw * InputRanges[ch]) / NativeMethods.MaxValue;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private short ConvertmV2ADC(short mv, short ch) => (short)((mv * NativeMethods.MaxValue) / InputRanges[ch]);

        private string FormatBlockData(uint sampleCount, int timeInterval, PinnedArray<short>[] minPinned, PinnedArray<short>[] maxPinned)
        {
            var sb = new StringBuilder();

            sb.AppendLine("For each of the active channels, results shown are....");
            sb.AppendLine("Time interval (ns), Maximum Aggregated value ADC Count & mV, Minimum Aggregated value ADC Count & mV");
            sb.AppendLine();

            // Build Header
            var heading = new[] { "Time", "Channel", "Max ADC", "Max mV", "Min ADC", "Min mV" };

            sb.AppendFormat("{0, 10}", heading[0]);
            for (int i = 0; i < _channelCount; i++)
            {
                if (_channelSettings[i].enabled)
                {
                    sb.AppendFormat("{0,10} {1,10} {2,10} {3,10} {4,10}",
                                    heading[1],
                                    heading[2],
                                    heading[3],
                                    heading[4],
                                    heading[5]);
                }
            }
            sb.AppendLine();

            // Build Body
            for (int i = 0; i < sampleCount; i++)
            {
                sb.AppendFormat("{0,10}", (i * timeInterval));

                for (int ch = 0; ch < _channelCount; ch++)
                {
                    if (_channelSettings[ch].enabled)
                    {
                        sb.AppendFormat("{0,10} {1,10} {2,10} {3,10} {4,10}",
                                        (char)('A' + ch),
                                        maxPinned[ch].Target[i],
                                        ConvertADC2mV(maxPinned[ch].Target[i], (int)_channelSettings[GetChannelIndex(ch)].range),
                                        minPinned[ch].Target[i],
                                        ConvertADC2mV(minPinned[ch].Target[i], (int)_channelSettings[GetChannelIndex(ch)].range));
                    }
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }
    }
}
