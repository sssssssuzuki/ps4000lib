/*
 * File: NativeMethods.cs
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
using System.Runtime.InteropServices;
using System.Text;
using Range = PS4000Lib.Enum.Range;

namespace PS4000Lib
{
    #region Driver Structs
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct TriggerChannelProperties
    {
        public short ThresholdMajor;
        public ushort HysteresisMajor;
        public short ThresholdMinor;
        public ushort HysteresisMinor;
        public ChannelType Channel;
        public ThresholdMode ThresholdMode;

        public TriggerChannelProperties(
            short thresholdMajor,
            ushort hysteresisMajor,
            short thresholdMinor,
            ushort hysteresisMinor,
            ChannelType channel,
            ThresholdMode thresholdMode)
        {
            ThresholdMajor = thresholdMajor;
            HysteresisMajor = hysteresisMajor;
            ThresholdMinor = thresholdMinor;
            HysteresisMinor = hysteresisMinor;
            Channel = channel;
            ThresholdMode = thresholdMode;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct TriggerConditions
    {
        public TriggerState ChannelA;
        public TriggerState ChannelB;
        public TriggerState ChannelC;
        public TriggerState ChannelD;
        public TriggerState External;
        public TriggerState Aux;
        public TriggerState Pwq;

        public TriggerConditions(
            TriggerState channelA,
            TriggerState channelB,
            TriggerState channelC,
            TriggerState channelD,
            TriggerState external,
            TriggerState aux,
            TriggerState pwq)
        {
            ChannelA = channelA;
            ChannelB = channelB;
            ChannelC = channelC;
            ChannelD = channelD;
            External = external;
            Aux = aux;
            Pwq = pwq;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct PwqConditions
    {
        public TriggerState ChannelA;
        public TriggerState ChannelB;
        public TriggerState ChannelC;
        public TriggerState ChannelD;
        public TriggerState External;
        public TriggerState Aux;

        public PwqConditions(
            TriggerState channelA,
            TriggerState channelB,
            TriggerState channelC,
            TriggerState channelD,
            TriggerState external,
            TriggerState aux)
        {
            ChannelA = channelA;
            ChannelB = channelB;
            ChannelC = channelC;
            ChannelD = channelD;
            External = external;
            Aux = aux;
        }
    }
    #endregion

    internal class NativeMethods
    {
        #region const
        private const string DriverFilename = "ps4000.dll";
        #endregion

        #region Driver Imports
        #region Callback delegates
        public delegate void PS4000BlockReady(short handle, short status, IntPtr pVoid);
        public delegate void PS4000StreamingReady(
                                                short handle,
                                                int noOfSamples,
                                                uint startIndex,
                                                short ov,
                                                uint triggerAt,
                                                short triggered,
                                                short autoStop,
                                                IntPtr pVoid);
        public delegate void PS4000DataReady(
                                                short handle,
                                                int noOfSamples,
                                                short overflow,
                                                uint triggerAt,
                                                short triggered,
                                                IntPtr pVoid);
        #endregion

        [DllImport(DriverFilename, EntryPoint = "ps4000OpenUnit")] public static extern short OpenUnit(out short handle);
        [DllImport(DriverFilename, EntryPoint = "ps4000CloseUnit")] public static extern short CloseUnit(short handle);
        [DllImport(DriverFilename, EntryPoint = "ps4000RunBlock")]
        public static extern short RunBlock(
                                                short handle,
                                                int noOfPreTriggerSamples,
                                                int noOfPostTriggerSamples,
                                                uint timebase,
                                                short oversample,
                                                out int timeIndisposedMs,
                                                ushort segmentIndex,
                                                PS4000BlockReady lpps4000BlockReady,
                                                IntPtr pVoid);
        [DllImport(DriverFilename, EntryPoint = "ps4000Stop")] public static extern short Stop(short handle);
        [DllImport(DriverFilename, EntryPoint = "ps4000SetChannel")]
        public static extern short SetChannel(
                                                short handle,
                                                ChannelType channel,
                                                short enabled,
                                                short dc,
                                                Range range);
        [DllImport(DriverFilename, EntryPoint = "ps4000SetDataBuffer")]
        public static extern short SetDataBuffer(
                                                short handle,
                                                ChannelType channel,
                                                short[] buffer,
                                                int bufferLth);
        [DllImport(DriverFilename, EntryPoint = "ps4000SetDataBuffers")]
        public static extern short SetDataBuffers(
                                                short handle,
                                                ChannelType channel,
                                                short[] bufferMax,
                                                short[] bufferMin,
                                                int bufferLth);
        [DllImport(DriverFilename, EntryPoint = "ps4000SetDataBufferWithMode")]
        public static extern short SetDataBufferWithMode(
                                                short handle,
                                                ChannelType channel,
                                                short[] buffer,
                                                int bufferLth,
                                                DownSamplingMode mode);
        [DllImport(DriverFilename, EntryPoint = "ps4000SetTriggerChannelDirections")]
        public static extern short SetTriggerChannelDirections(
                                                short handle,
                                                ThresholdDirection channelA,
                                                ThresholdDirection channelB,
                                                ThresholdDirection channelC,
                                                ThresholdDirection channelD,
                                                ThresholdDirection ext,
                                                ThresholdDirection aux);
        [DllImport(DriverFilename, EntryPoint = "ps4000GetTimebase")]
        public static extern short GetTimebase(
                                                short handle,
                                                uint timebase,
                                                int noSamples,
                                                out int timeIntervalNanoseconds,
                                                short oversample,
                                                out int maxSamples,
                                                ushort segmentIndex);
        [DllImport(DriverFilename, EntryPoint = "ps4000GetValues")]
        public static extern short GetValues(
                                                short handle,
                                                uint startIndex,
                                                ref uint noOfSamples,
                                                uint downSampleRatio,
                                                DownSamplingMode downSampleDownSamplingMode,
                                                ushort segmentIndex,
                                                out short overflow);
        [DllImport(DriverFilename, EntryPoint = "ps4000SetPulseWidthQualifier")]
        public static extern short SetPulseWidthQualifier(
                                                short handle,
                                                PwqConditions[] conditions,
                                                short numConditions,
                                                ThresholdDirection direction,
                                                uint lower,
                                                uint upper,
                                                PulseWidthType type);
        [DllImport(DriverFilename, EntryPoint = "ps4000SetTriggerChannelProperties")]
        public static extern short SetTriggerChannelProperties(
                                                short handle,
                                                TriggerChannelProperties[] channelProperties,
                                                short numChannelProperties,
                                                short auxOutputEnable,
                                                int autoTriggerMilliseconds);
        [DllImport(DriverFilename, EntryPoint = "ps4000SetTriggerChannelConditions")]
        public static extern short SetTriggerChannelConditions(
                                                short handle,
                                                TriggerConditions[] conditions,
                                                short numConditions);
        [DllImport(DriverFilename, EntryPoint = "ps4000SetTriggerDelay")] public static extern short SetTriggerDelay(short handle, uint delay);
        [DllImport(DriverFilename, EntryPoint = "ps4000GetUnitInfo", CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        public static extern short GetUnitInfo(short handle, [MarshalAs(UnmanagedType.LPStr)] StringBuilder infoString, short stringLength, out short requiredSize, int info);
        [DllImport(DriverFilename, EntryPoint = "ps4000RunStreaming")]
        public static extern short RunStreaming(
                                                short handle,
                                                ref uint sampleInterval,
                                                ReportedTimeUnits sampleIntervalTimeUnits,
                                                uint maxPreTriggerSamples,
                                                uint maxPostPreTriggerSamples,
                                                bool autoStop,
                                                uint downSamplingRation,
                                                uint overviewBufferSize);
        [DllImport(DriverFilename, EntryPoint = "ps4000GetStreamingLatestValues")]
        public static extern short GetStreamingLatestValues(
                                                short handle,
                                                PS4000StreamingReady lpps4000StreamingReady,
                                                IntPtr pVoid);
        [DllImport(DriverFilename, EntryPoint = "ps4000SetNoOfCaptures")]
        public static extern short SetNoOfRapidCaptures(
                                                short handle,
                                                ushort nWaveforms);
        [DllImport(DriverFilename, EntryPoint = "ps4000MemorySegments")]
        public static extern short MemorySegments(
                                                short handle,
                                                ushort nSegments,
                                                out int nMaxSamples);
        [DllImport(DriverFilename, EntryPoint = "ps4000SetDataBufferBulk")]
        public static extern short SetDataBuffersRapid(
                                                short handle,
                                                ChannelType channel,
                                                short[] buffer,
                                                int bufferLth,
                                                ushort waveform);
        [DllImport(DriverFilename, EntryPoint = "ps4000GetValuesBulk")]
        public static extern short GetValuesRapid(
                                                short handle,
                                                ref uint noOfSamples,
                                                ushort fromSegmentIndex,
                                                ushort toSegmentIndex,
                                                short[] overflows);
        #endregion
    }
}
