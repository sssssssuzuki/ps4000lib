using PicoPinnedArray;
using PicoStatus;
using PS4000Lib.Enum;
using System;
using System.Collections.Generic;
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
        public const int MAX_CHANNELS = 4;
        public const int QUAD_SCOPE = 4;
        public const int DUAL_SCOPE = 2;
        private static readonly IReadOnlyList<ushort> InputRanges = new ushort[] { 10, 20, 50, 100, 200, 500, 1000, 2000, 5000, 10000, 20000, 50000 };
        #endregion

        #region field
        private bool _disposed = false;
        private short _handle;
        private uint _timebase;
        private double _timeInterval;
        private int _maxSamples;
        private int _channelCount;
        private bool _ready;

        private List<TriggerConditions> _conditions;

        private NativeMethods.ps4000BlockReady _callbackDelegate;
        #endregion

        #region property
        public bool IsOpen { get; private set; }

        public Model Model { get; private set; }

        public Channel ChannelA { get; }
        public Channel ChannelB { get; }
        public Channel ChannelC { get; }
        public Channel ChannelD { get; }
        public Channel ChannelExt { get; }
        public Channel ChannelAux { get; }
        public Channel ChannelPwq { get; }

        public Range MinRange { get; private set; }
        public Range MaxRange { get; private set; }

        public int BufferSize { get; set; }

        public double SamplingRateHz
        {
            get => 1_000_000_000.0 / SamplingIntervalNanoSec;
            set => SamplingIntervalNanoSec = 1_000_000_000.0 / value;

        }

        public double SamplingIntervalNanoSec
        {
            get => _timeInterval;
            set
            {
                Timebase = ConvertSamplingInterval2Timebase(value);
                _timeInterval = ConvertTimebase2SamplingInterval(Timebase);
            }
        }
        public uint Timebase
        {
            get => _timebase;
            set
            {
                _timebase = value;
                while (NativeMethods.GetTimebase(_handle, _timebase, BufferSize, out _, OverSample, out _maxSamples, 0) != 0)
                {
                    _timebase++;
                }
            }
        }
        public short OverSample { get; set; }
        public int MaxSamples => _maxSamples;
        public Scale Scale { get; set; }

        public string Settings
        {
            get
            {
                StringBuilder res = new StringBuilder();

                string scale = Scale == Scale.mV ? "mV" : "ADC counts";
                res.Append($"Readings will be scaled in {scale}");

                foreach (Channel ch in EnumerateChannel(false, false))
                {
                    ushort voltage = InputRanges[(int)ch.Range];
                    res.Append($"\nChannel {ch.Name} Voltage Range = ");
                    res.Append(voltage < 1000 ? $"{voltage}mV" : $"{voltage / 1000}V");
                }

                return res.ToString();
            }
        }

        public string DeviceInfo { get; private set; }
        public Pwq Pwq { get; set; } = null;
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

            foreach (Channel ch in EnumerateChannel(false, false))
            {
                ch.SettingUpdate += SetChannel;
            }
        }

        ~PS4000()
        {
            Dispose(false);
        }
        #endregion

        #region control
        public void Open()
        {
            short status = NativeMethods.OpenUnit(out short handle);
            _handle = (status == StatusCodes.PICO_OK) ? handle : throw new PicoException(status);

            OverSample = 1;
            Scale = Scale.mV;
            SetDeviceInfo();

            SetChannel();
            Timebase = 0u;

            IsOpen = true;
        }

        public void Close()
        {
            if (IsOpen)
            {
                foreach (Channel ch in EnumerateChannel(false, false))
                {
                    ch.SettingUpdate -= SetChannel;
                }

                NativeMethods.CloseUnit(_handle);
                IsOpen = false;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

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
        public async Task<BlockData> CollectBlockImmediateAsync()
        {
            return await Task.Run(() =>
            {
                return CollectBlockImmediate();
            });
        }

        public BlockData CollectBlockImmediate()
        {
            short status = SetTrigger(null, null, null, null, 0, 0, 0);
            return status == StatusCodes.PICO_OK ? BlockDataHandler(0) : throw new PicoException(status);
        }

        public async Task<BlockData> CollectBlockTriggeredAsync(int noOfPreTriggerSamples = 0)
        {
            return await Task.Run(() =>
            {
                return CollectBlockTriggered(noOfPreTriggerSamples);
            });
        }

        public BlockData CollectBlockTriggered(int noOfPreTriggerSamples = 0)
        {
            TriggerChannelProperties[] details = GetTriggerChannelProperties();
            TriggerConditions[] conds = _conditions.ToArray();
            ThresholdDirection[] dirs = GetTriggerThresholdDirections();

            short status = SetTrigger(details, conds, dirs, Pwq, 0, 0, 0);
            return status == StatusCodes.PICO_OK ? BlockDataHandler(noOfPreTriggerSamples) : throw new PicoException(status);
        }

        private BlockData BlockDataHandler(int noOfPreTriggerSamples)
        {
            BlockData res;

            uint sampleCount = (uint)BufferSize;
            PinnedArray<short>[] minPinned = new PinnedArray<short>[_channelCount];
            PinnedArray<short>[] maxPinned = new PinnedArray<short>[_channelCount];

            foreach (Channel ch in EnumerateChannel(false, false))
            {
                short[] minBuffers = new short[sampleCount];
                short[] maxBuffers = new short[sampleCount];
                minPinned[ch.ChannelNum] = new PinnedArray<short>(minBuffers);
                maxPinned[ch.ChannelNum] = new PinnedArray<short>(maxBuffers);
                NativeMethods.SetDataBuffers(_handle, ch.Type, maxBuffers, minBuffers, (int)sampleCount);
            }

            /*  Verify the currently selected timebase index, and the maximum number of samples per channel with the current settings. */
            int timeInterval;

            while (NativeMethods.GetTimebase(_handle, _timebase, (int)sampleCount, out timeInterval, OverSample, out int maxSamples, 0) != 0)
            {
                _timebase++;
            }

            /* Start it collecting, then wait for completion*/
            _ready = false;
            _callbackDelegate = BlockCallback;

            NativeMethods.RunBlock(_handle, noOfPreTriggerSamples, (int)sampleCount, _timebase, OverSample, out int timeIndisposed, 0, _callbackDelegate, IntPtr.Zero);

            while (!_ready)
            {
                Thread.Sleep(100);
            }

            NativeMethods.Stop(_handle);

            if (_ready)
            {
                NativeMethods.GetValues(_handle, 0, ref sampleCount, 1, DownSamplingMode.None, 0, out short overflow);
                res = new BlockData(this)
                {
                    SampleCount = Math.Min(sampleCount, (uint)BufferSize),
                    TimeInterval = timeInterval,
                    MinPinned = minPinned,
                    MaxPinned = maxPinned
                };
            }
            else
            {
                res = null;
            }

            return res;
        }
        #endregion

        #region setting
        public void ResetTriggerConditions()
        {
            _conditions = new List<TriggerConditions>();
        }

        public void AddTriggerConditions(TriggerConditions cond)
        {
            if (_conditions == null)
            {
                ResetTriggerConditions();
            }

            _conditions.Add(cond);
        }

        private TriggerChannelProperties[] GetTriggerChannelProperties()
        {
            List<TriggerChannelProperties> props = new List<TriggerChannelProperties>();
            foreach (Channel ch in EnumerateChannel(false, false))
            {
                short vol = ConvertmV2ADC(ch.TriggerVoltageMV / ch.Attenuation, ch.Range);
                props.Add(new TriggerChannelProperties(
                                                vol,
                                                (ushort)(256 * 10 / ch.Attenuation),
                                                vol,
                                                (ushort)(256 * 10 / ch.Attenuation),
                                                ch.Type,
                                                ch.TriggerMode));
            }
            return props.ToArray();
        }

        private ThresholdDirection[] GetTriggerThresholdDirections()
        {
            ThresholdDirection[] dirs = new ThresholdDirection[6];
            foreach (Channel ch in EnumerateChannel(true, false))
            {
                dirs[ch.ChannelNum] = ch.TriggerDirection;
            }

            return dirs;
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
            short nChannelProperties = (short)(channelProperties == null ? 0 : channelProperties.Length);
            short nTriggerConditions = (short)(triggerConditions == null ? 0 : triggerConditions.Length);

            if (directions == null)
            {
                directions = Enumerable.Repeat(ThresholdDirection.None, 6).ToArray();
            }

            if (pwq == null)
            {
                pwq = new Pwq(null, 0, ThresholdDirection.None, 0, 0, PulseWidthType.None);
            }

            short status = NativeMethods.SetTriggerChannelProperties(_handle, channelProperties, nChannelProperties,
                                                            auxOutputEnabled, autoTriggerMs);
            if (status != StatusCodes.PICO_OK)
            {
                return status;
            }

            status = NativeMethods.SetTriggerChannelConditions(_handle, triggerConditions, nTriggerConditions);
            if (status != StatusCodes.PICO_OK)
            {
                return status;
            }

            status = NativeMethods.SetTriggerChannelDirections(_handle,
                                                              directions[ChannelA.ChannelNum],
                                                              directions[ChannelB.ChannelNum],
                                                              directions[ChannelC.ChannelNum],
                                                              directions[ChannelD.ChannelNum],
                                                              directions[ChannelExt.ChannelNum],
                                                              directions[ChannelAux.ChannelNum]);
            if (status != StatusCodes.PICO_OK)
            {
                return status;
            }

            status = NativeMethods.SetTriggerDelay(_handle, delay);
            if (status != StatusCodes.PICO_OK)
            {
                return status;
            }

            status = NativeMethods.SetPulseWidthQualifier(_handle, pwq.conditions,
                                                    pwq.nConditions, pwq.direction,
                                                    pwq.lower, pwq.upper, pwq.type);
            return status;
        }
        #endregion

        #region callback
        private void BlockCallback(short handle, short status, IntPtr pVoid)
        {
            _ready = true;
        }
        #endregion

        #region private methods
        private void SetChannel()
        {
            foreach (Channel ch in EnumerateChannel(false, false))
            {
                NativeMethods.SetChannel(_handle, ch.Type,
                                  (short)(ch.Enabled ? 1 : 0),
                                  (short)(ch.Coupling == CouplingMode.DC ? 1 : 0),
                                  ch.Range);
            }
        }

        private void SetDeviceInfo()
        {
            int variant = 0;
            string[] description = new[]{
                           "Driver Version    ",
                           "USB Version       ",
                           "Hardware Version  ",
                           "Variant Info      ",
                           "Serial            ",
                           "Cal Date          ",
                           "Kernel Ver        "
                         };
            StringBuilder line = new StringBuilder(80);
            StringBuilder result = new StringBuilder();

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

                    if (i != 0)
                    {
                        result.Append("\n");
                    }

                    result.Append($"{description[i]}: {line}");
                }

                switch (variant)
                {
                    case (int)Model.PS4223:
                        Model = Model.PS4223;
                        MinRange = Range.Range_50MV;
                        MaxRange = Range.Range_100V;
                        _channelCount = DUAL_SCOPE;
                        break;
                    case (int)Model.PS4224:
                        Model = Model.PS4224;
                        MinRange = Range.Range_50MV;
                        MaxRange = Range.Range_20V;
                        _channelCount = DUAL_SCOPE;
                        break;
                    case (int)Model.PS4423:
                        Model = Model.PS4423;
                        MinRange = Range.Range_50MV;
                        MaxRange = Range.Range_100V;
                        _channelCount = QUAD_SCOPE;
                        break;
                    case (int)Model.PS4424:
                        Model = Model.PS4424;
                        MinRange = Range.Range_50MV;
                        MaxRange = Range.Range_20V;
                        _channelCount = QUAD_SCOPE;
                        break;
                    case (int)Model.PS4226:
                        Model = Model.PS4226;
                        MinRange = Range.Range_50MV;
                        MaxRange = Range.Range_20V;
                        _channelCount = DUAL_SCOPE;
                        break;
                    case (int)Model.PS4227:
                        Model = Model.PS4227;
                        MinRange = Range.Range_50MV;
                        MaxRange = Range.Range_20V;
                        _channelCount = DUAL_SCOPE;
                        break;
                    case (int)Model.PS4262:
                        Model = Model.PS4262;
                        MinRange = Range.Range_10MV;
                        MaxRange = Range.Range_20V;
                        _channelCount = DUAL_SCOPE;
                        break;
                }
            }
            DeviceInfo = result.ToString();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int ConvertADC2mV(short raw, Range range)
        {
            return (raw * InputRanges[(int)range]) / MaxValue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static short ConvertmV2ADC(int raw, Range range)
        {
            return (short)((raw * MaxValue) / InputRanges[(int)range]);
        }

        private uint ConvertSamplingInterval2Timebase(double samplingIntervalNanoSec)
        {
            switch (Model)
            {
                case Model.PS4223:
                case Model.PS4224:
                case Model.PS4423:
                case Model.PS4424:
                    return samplingIntervalNanoSec <= 50.0 ? (uint)(Math.Log(samplingIntervalNanoSec * 80_000_000 / 1_000_000_000) / Math.Log(2)) : (uint)(samplingIntervalNanoSec * 20_000_000 / 1_000_000_000) + 1u;
                case Model.PS4226:
                case Model.PS4227:
                    return samplingIntervalNanoSec <= 32.0 ? (uint)(Math.Log(samplingIntervalNanoSec / 1_000_000_000 * 250_000_000) / Math.Log(2)) : (uint)(samplingIntervalNanoSec / 1_000_000_000 * 31_250_000) + 2u;
                case Model.PS4262:
                    return (uint)(samplingIntervalNanoSec / 1_000_000_000 * 10_000_000) - 1u;
                default:
                    throw new NotSupportedException(nameof(Model));
            }
        }
        private double ConvertTimebase2SamplingInterval(uint timebase)
        {
            switch (Model)
            {
                case Model.PS4223:
                case Model.PS4224:
                case Model.PS4423:
                case Model.PS4424:
                    return timebase <= 2u ? Math.Pow(2, timebase) * 12.5 : (timebase - 1u) * 50.0;
                case Model.PS4226:
                case Model.PS4227:
                    return timebase <= 3u ? Math.Pow(2, timebase) * 4.0 : (timebase - 2u) * 32.0;
                case Model.PS4262:
                    return (timebase + 1) * 100.0;
                default:
                    throw new NotSupportedException(nameof(Model));
            }
        }

        internal IEnumerable<Channel> EnumerateChannel(bool includeExtAux, bool includePwq)
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
