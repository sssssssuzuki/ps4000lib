using PicoPinnedArray;
using PicoStatus;
using PS4000Lib.Enum;
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
        public const int MaxValue = 32764;
        public const int BUFFER_SIZE = 1024;
        public const int MAX_CHANNELS = 4;
        public const int QUAD_SCOPE = 4;
        public const int DUAL_SCOPE = 2;
        readonly IReadOnlyList<ushort> InputRanges = new ushort[] { 10, 20, 50, 100, 200, 500, 1000, 2000, 5000, 10000, 20000, 50000 };
        #endregion

        #region field
        private bool _disposed = false;
        private short _handle;
        private uint _timebase;
        private int _timeInterval;
        private int _maxSamples;
        private int _channelCount;
        private bool _ready;

        private NativeMethods.ps4000BlockReady _callbackDelegate;
        #endregion

        #region property
        public bool IsOpen { get; private set; }

        public Channel ChannelA { get; }
        public Channel ChannelB { get; }
        public Channel ChannelC { get; }
        public Channel ChannelD { get; }
        public Channel ChannelExt { get; }
        public Channel ChannelAux { get; }
        public Channel ChannelPwq { get; }

        public Range MinRange { get; private set; }
        public Range MaxRange { get; private set; }

        public int TimeInterval => _timeInterval;
        public uint Timebase
        {
            get
            {
                return _timebase;
            }
            set
            {
                while (NativeMethods.GetTimebase(_handle, _timebase, BUFFER_SIZE, out _timeInterval, OverSample, out _maxSamples, 0) != 0)
                    _timebase++;
            }
        }
        public short OverSample { get; set; }
        public int MaxSamples => _maxSamples;
        public Scale Scaling { get; set; }

        public string Settings
        {
            get
            {
                var res = new StringBuilder();

                var scale = Scaling == Scale.mV ? "mV" : "ADC counts";
                res.Append($"Readings will be scaled in {scale}");

                foreach (var ch in EnumerateChannel(false, false))
                {
                    var voltage = InputRanges[(int)ch.Range];
                    res.Append($"\nChannel {ch.Name} Voltage Range = ");
                    res.Append(voltage < 1000 ? $"{voltage}mV" : $"{voltage / 1000}V");
                }

                for (int ch = 0; ch < _channelCount; ch++)
                {

                }

                return res.ToString();
            }
        }

        public string DeviceInfo { get; private set; }
        #endregion

        #region ctor
        public PS4000()
        {
            ChannelA = new Channel(ChannelType.ChannelA, "A");
            ChannelB = new Channel(ChannelType.ChannelB, "B");
            ChannelC = new Channel(ChannelType.ChannelC, "C");
            ChannelD = new Channel(ChannelType.ChannelD, "D");
            ChannelExt = new Channel(ChannelType.External, "Ext");
            ChannelAux = new Channel(ChannelType.Aux, "Aux");
            ChannelPwq = new Channel(ChannelType.None, "Pwq");

            OverSample = 1;
            Scaling = Scale.mV;
            SetDeviceInfo();
        }

        ~PS4000()
        {
            this.Dispose(false);
        }
        #endregion

        #region control
        public void Open()
        {
            var status = NativeMethods.OpenUnit(out short handle);
            _handle = (status == StatusCodes.PICO_OK) ? handle : throw new PicoException(status);
            this.IsOpen = true;
        }
        public void Close()
        {
            if (this.IsOpen)
            {
                NativeMethods.CloseUnit(_handle);
                this.IsOpen = false;
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                // Free any other managed objects here.
                //
            }

            Close();

            _disposed = true;
        }
        #endregion


        #region mesure
        public string CollectBlockImmediate()
        {
            var status = SetTrigger(null, null, null, null, 0, 0, 0);
            return status == StatusCodes.PICO_OK ? BlockDataHandler() : throw new PicoException(status);
        }

        private string BlockDataHandler()
        {
            var res = string.Empty;

            uint sampleCount = BUFFER_SIZE;
            var minPinned = new PinnedArray<short>[_channelCount];
            var maxPinned = new PinnedArray<short>[_channelCount];

            foreach (var ch in EnumerateChannel(false, false))
            {
                var minBuffers = new short[sampleCount];
                var maxBuffers = new short[sampleCount];
                minPinned[ch.ChannelNum] = new PinnedArray<short>(minBuffers);
                maxPinned[ch.ChannelNum] = new PinnedArray<short>(maxBuffers);
                NativeMethods.SetDataBuffers(_handle, ch.Type, maxBuffers, minBuffers, (int)sampleCount);
            }

            /*  Verify the currently selected timebase index, and the maximum number of samples per channel with the current settings. */
            int timeInterval;

            while (NativeMethods.GetTimebase(_handle, _timebase, (int)sampleCount, out timeInterval, OverSample, out int maxSamples, 0) != 0)
                _timebase++;

            /* Start it collecting, then wait for completion*/
            _ready = false;
            _callbackDelegate = BlockCallback;

            NativeMethods.RunBlock(_handle, 0, (int)sampleCount, _timebase, OverSample, out int timeIndisposed, 0, _callbackDelegate, IntPtr.Zero);

            while (!_ready)
                Thread.Sleep(100);

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
        #endregion

        #region setting
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
                                                              directions[ChannelA.ChannelNum],
                                                              directions[ChannelB.ChannelNum],
                                                              directions[ChannelC.ChannelNum],
                                                              directions[ChannelD.ChannelNum],
                                                              directions[ChannelExt.ChannelNum],
                                                              directions[ChannelAux.ChannelNum]);
            if (status != StatusCodes.PICO_OK) return status;

            status = NativeMethods.SetTriggerDelay(_handle, delay);
            if (status != StatusCodes.PICO_OK) return status;

            status = NativeMethods.SetPulseWidthQualifier(_handle, pwq.conditions,
                                                    pwq.nConditions, pwq.direction,
                                                    pwq.lower, pwq.upper, pwq.type);
            return status;
        }
        #endregion

        #region callback
        void BlockCallback(short handle, short status, IntPtr pVoid)
        {
            _ready = true;
        }
        #endregion

        #region private methods
        private void SetChannel()
        {
            foreach (var ch in EnumerateChannel(false, false))
            {
                NativeMethods.SetChannel(_handle, ch.Type,
                                  (short)(ch.Enabled ? 1 : 0),
                                  (short)(ch.Coupling == CouplingMode.DC ? 1 : 0),
                                  ch.Range);
            }
        }

        private void SetDeviceInfo()
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
                        MinRange = Range.Range_50MV;
                        MaxRange = Range.Range_100V;
                        _channelCount = DUAL_SCOPE;
                        break;
                    case (int)Model.PS4224:
                        MinRange = Range.Range_50MV;
                        MaxRange = Range.Range_20V;
                        _channelCount = DUAL_SCOPE;
                        break;
                    case (int)Model.PS4423:
                        MinRange = Range.Range_50MV;
                        MaxRange = Range.Range_100V;
                        _channelCount = QUAD_SCOPE;
                        break;
                    case (int)Model.PS4424:
                        MinRange = Range.Range_50MV;
                        MaxRange = Range.Range_20V;
                        _channelCount = QUAD_SCOPE;
                        break;
                    case (int)Model.PS4226:
                        MinRange = Range.Range_50MV;
                        MaxRange = Range.Range_20V;
                        _channelCount = DUAL_SCOPE;
                        break;
                    case (int)Model.PS4227:
                        MinRange = Range.Range_50MV;
                        MaxRange = Range.Range_20V;
                        _channelCount = DUAL_SCOPE;
                        break;
                    case (int)Model.PS4262:
                        MinRange = Range.Range_10MV;
                        MaxRange = Range.Range_20V;
                        _channelCount = DUAL_SCOPE;
                        break;
                }
            }
            DeviceInfo = result.ToString();
        }

        private string FormatBlockData(uint sampleCount, int timeInterval, PinnedArray<short>[] minPinned, PinnedArray<short>[] maxPinned)
        {
            var sb = new StringBuilder();

            sb.AppendLine("For each of the active channels, results shown are....");
            sb.AppendLine("Time interval (ns), Maximum Aggregated value ADC Count & mV, Minimum Aggregated value ADC Count & mV");
            sb.AppendLine();

            // Build Header
            var heading = new[] { "Time", "Channel", "Max ADC", "Max mV", "Min ADC", "Min mV" };

            sb.AppendFormat("{0, 10}", heading[0]);

            foreach (var ch in EnumerateChannel(false, false))
            {
                if (ch.Enabled)
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

                foreach (var ch in EnumerateChannel(false, false))
                {
                    if (ch.Enabled)
                    {
                        sb.AppendFormat("{0,10} {1,10} {2,10} {3,10} {4,10}",
                                        ch.Name,
                                        maxPinned[ch.ChannelNum].Target[i],
                                        ConvertADC2mV(maxPinned[ch.ChannelNum].Target[i], ch.Range),
                                        minPinned[ch.ChannelNum].Target[i],
                                        ConvertADC2mV(minPinned[ch.ChannelNum].Target[i], ch.Range));
                    }
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int ConvertADC2mV(int raw, Range range) => (raw * InputRanges[(int)range]) / MaxValue;

        private IEnumerable<Channel> EnumerateChannel(bool includeExtAux, bool includePwq)
        {
            yield return ChannelA;
            yield return ChannelB;
            if (_channelCount > 2)
            {
                yield return ChannelC;
                yield return ChannelD;
            }
            if (includeExtAux)
            {
                yield return ChannelExt;
                yield return ChannelAux;
            }
            if (includePwq)
            {
                yield return ChannelPwq;
            }
        }
        #endregion
    }
}
