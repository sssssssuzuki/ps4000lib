/*
 * File: PS4000.cs
 * Project: ps4000lib
 * Created Date: 19/04/2021
 * Author: Shun Suzuki
 * -----
 * Last Modified: 19/04/2021
 * Modified By: Shun Suzuki (suzuki@hapis.k.u-tokyo.ac.jp)
 * -----
 * Copyright (c) 2021 Hapis Lab. All rights reserved.
 * 
 */

using PS4000Lib.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PS4000Lib.Shared;
using Range = PS4000Lib.Enum.Range;

namespace PS4000Lib
{
    public class Pwq
    {
        public PwqConditions[] Conditions;
        public short NConditions;
        public ThresholdDirection Direction;
        public uint Lower;
        public uint Upper;
        public PulseWidthType Type;

        public Pwq(PwqConditions[] conditions,
                    short nConditions,
                    ThresholdDirection direction,
                    uint lower, uint upper,
                    PulseWidthType type)
        {
            Conditions = conditions;
            NConditions = nConditions;
            Direction = direction;
            Lower = lower;
            Upper = upper;
            Type = type;
        }
    }

    public class PS4000 : IDisposable
    {
        #region const
        public const int MaxValue = 32764;
        public const int MaxChannels = 4;
        public const int QuadScope = 4;
        public const int DualScope = 2;
        private static readonly IReadOnlyList<ushort> InputRanges = new ushort[] { 10, 20, 50, 100, 200, 500, 1000, 2000, 5000, 10000, 20000, 50000 };
        #endregion

        #region field
        private bool _disposed;
        private short _handle;
        private uint _timebase;
        private double _timeInterval;
        private int _maxSamples;
        private int _channelCount;
        private bool _ready;

        private List<TriggerConditions> _conditions;

        private NativeMethods.PS4000BlockReady _callbackDelegate;
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
                var res = new StringBuilder();

                var scale = Scale == Scale.MV ? "mV" : "ADC counts";
                res.Append($"Readings will be scaled in {scale}");

                foreach (var ch in EnumerateChannel(false, false))
                {
                    var voltage = InputRanges[(int)ch.Range];
                    res.Append($"\nChannel {ch.Name} Voltage Range = ");
                    res.Append(voltage < 1000 ? $"{voltage}mV" : $"{voltage / 1000}V");
                }

                return res.ToString();
            }
        }

        public string DeviceInfo { get; private set; }
        public Pwq Pwq { get; set; } = null;

        // Not supported yet
        public uint DownSampleRatio { get; set; } = 1;
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

            foreach (var ch in EnumerateChannel(false, false))
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
            var status = NativeMethods.OpenUnit(out var handle);
            _handle = status == StatusCodes.PicoOk ? handle : throw new PicoException(status);

            OverSample = 1;
            Scale = Scale.MV;
            SetDeviceInfo();

            SetChannel();
            Timebase = 0u;

            IsOpen = true;
        }

        public void Close()
        {
            if (!IsOpen) return;
            foreach (var ch in EnumerateChannel(false, false))
            {
                ch.SettingUpdate -= SetChannel;
            }

            NativeMethods.CloseUnit(_handle);
            IsOpen = false;
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
            return await Task.Run(CollectBlockImmediate);
        }

        public BlockData CollectBlockImmediate()
        {
            var status = SetTrigger(null, null, null, null, 0, 0, 0);
            return status == StatusCodes.PicoOk ? BlockDataHandler(0) : throw new PicoException(status);
        }

        public async Task<BlockData> CollectBlockTriggeredAsync(int noOfPreTriggerSamples = 0)
        {
            return await Task.Run(() => CollectBlockTriggered(noOfPreTriggerSamples));
        }

        public BlockData CollectBlockTriggered(int noOfPreTriggerSamples = 0)
        {
            var details = GetTriggerChannelProperties();
            var conditions = _conditions.ToArray();
            var dirs = GetTriggerThresholdDirections();

            var status = SetTrigger(details, conditions, dirs, Pwq, 0, 0, 0);
            return status == StatusCodes.PicoOk ? BlockDataHandler(noOfPreTriggerSamples) : throw new PicoException(status);
        }

        private BlockData BlockDataHandler(int noOfPreTriggerSamples)
        {
            BlockData res;

            var sampleCount = (uint)BufferSize;
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

            while (NativeMethods.GetTimebase(_handle, _timebase, (int)sampleCount, out timeInterval, OverSample, out _, 0) != 0)
            {
                _timebase++;
            }

            /* Start it collecting, then wait for completion*/
            _ready = false;
            _callbackDelegate = BlockCallback;

            NativeMethods.RunBlock(_handle, noOfPreTriggerSamples, (int)sampleCount, _timebase, OverSample, out _, 0, _callbackDelegate, IntPtr.Zero);

            while (!_ready)
            {
                Thread.Sleep(100);
            }

            NativeMethods.Stop(_handle);

            if (_ready)
            {
                NativeMethods.GetValues(_handle, 0, ref sampleCount, DownSampleRatio, DownSamplingMode.None, 0, out _);
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
            => (from ch in EnumerateChannel(false, false) let vol = ConvertMV2ADC(ch.TriggerVoltageMV / ch.Attenuation, ch.Range) select new TriggerChannelProperties(vol, (ushort)(256 * 10 / ch.Attenuation), vol, (ushort)(256 * 10 / ch.Attenuation), ch.Type, ch.TriggerMode)).ToArray();

        private ThresholdDirection[] GetTriggerThresholdDirections()
        {
            var dirs = new ThresholdDirection[6];
            foreach (var ch in EnumerateChannel(true, false))
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
            var nChannelProperties = (short)(channelProperties?.Length ?? 0);
            var nTriggerConditions = (short)(triggerConditions?.Length ?? 0);

            directions ??= Enumerable.Repeat(ThresholdDirection.None, 6).ToArray();

            pwq ??= new Pwq(null, 0, ThresholdDirection.None, 0, 0, PulseWidthType.None);

            var status = NativeMethods.SetTriggerChannelProperties(_handle, channelProperties, nChannelProperties,
                                                            auxOutputEnabled, autoTriggerMs);
            if (status != StatusCodes.PicoOk)
            {
                return status;
            }

            status = NativeMethods.SetTriggerChannelConditions(_handle, triggerConditions, nTriggerConditions);
            if (status != StatusCodes.PicoOk)
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
            if (status != StatusCodes.PicoOk)
            {
                return status;
            }

            status = NativeMethods.SetTriggerDelay(_handle, delay);
            if (status != StatusCodes.PicoOk)
            {
                return status;
            }

            status = NativeMethods.SetPulseWidthQualifier(_handle, pwq.Conditions,
                                                    pwq.NConditions, pwq.Direction,
                                                    pwq.Lower, pwq.Upper, pwq.Type);
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
                for (var i = 0; i < 7; i++)
                {
                    NativeMethods.GetUnitInfo(_handle, line, 80, out _, i);

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
                        MinRange = Range.Range50MV;
                        MaxRange = Range.Range100V;
                        _channelCount = DualScope;
                        break;
                    case (int)Model.PS4224:
                        Model = Model.PS4224;
                        MinRange = Range.Range50MV;
                        MaxRange = Range.Range20V;
                        _channelCount = DualScope;
                        break;
                    case (int)Model.PS4423:
                        Model = Model.PS4423;
                        MinRange = Range.Range50MV;
                        MaxRange = Range.Range100V;
                        _channelCount = QuadScope;
                        break;
                    case (int)Model.PS4424:
                        Model = Model.PS4424;
                        MinRange = Range.Range50MV;
                        MaxRange = Range.Range20V;
                        _channelCount = QuadScope;
                        break;
                    case (int)Model.PS4226:
                        Model = Model.PS4226;
                        MinRange = Range.Range50MV;
                        MaxRange = Range.Range20V;
                        _channelCount = DualScope;
                        break;
                    case (int)Model.PS4227:
                        Model = Model.PS4227;
                        MinRange = Range.Range50MV;
                        MaxRange = Range.Range20V;
                        _channelCount = DualScope;
                        break;
                    case (int)Model.PS4262:
                        Model = Model.PS4262;
                        MinRange = Range.Range10MV;
                        MaxRange = Range.Range20V;
                        _channelCount = DualScope;
                        break;
                }
            }
            DeviceInfo = result.ToString();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int ConvertADC2MV(short raw, Range range, int attenuation)
        {
            return attenuation * raw * InputRanges[(int)range] / MaxValue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static short ConvertMV2ADC(int raw, Range range)
        {
            return (short)(raw * MaxValue / InputRanges[(int)range]);
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
                case Model.None:
                    throw new NotSupportedException(nameof(Model));
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
                case Model.None:
                    throw new NotSupportedException(nameof(Model));
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
