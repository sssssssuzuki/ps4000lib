// <copyright file="PicoStatus.cs" company="Pico Technology Ltd">
// Copyright(C) 2009-2018 Pico Technology Ltd. See LICENSE file for terms.
// </copyright>

namespace PS4000Lib.Shared
{
    /// <summary>
    /// PICO_INFO Values
    /// </summary>
    public static class PicoInfo
    {
        public const uint PicoDriverVersion = (uint)0x00000000UL;
        public const uint PicoUsbVersion = (uint)0x00000001UL;
        public const uint PicoHardwareVersion = (uint)0x00000002UL;
        public const uint PicoVariantInfo = (uint)0x00000003UL;
        public const uint PicoBatchAndSerial = (uint)0x00000004UL;
        public const uint PicoCalDate = (uint)0x00000005UL;
        public const uint PicoKernelVersion = (uint)0x00000006UL;

        public const uint PicoDigitalHardwareVersion = (uint)0x00000007UL;
        public const uint PicoAnalogueHardwareVersion = (uint)0x00000008UL;

        public const uint PicoFirmwareVersion1 = (uint)0x00000009UL;
        public const uint PicoFirmwareVersion2 = (uint)0x0000000AUL;

        public const uint PicoMacAddress = (uint)0x0000000BUL;

        public const uint PicoShadowCal = (uint)0x0000000CUL;

        public const uint PicoIppVersion = (uint)0x0000000DUL;

        public const uint PicoDriverPath = (uint)0x0000000EUL;
    }

    /// <summary>
    /// This class defines the status codes returned by a Pico device,
    /// a PC Oscilloscope or data logger and is based on the PicoStatus.h header file.
    /// </summary>
    /// <remarks>
    /// In comments, "-API-" is a placeholder for the name of the scope or data logger API.
    /// For example, for the ps5000a API, it stands for "PS5000A" Or "ps5000a".
    /// </remarks>
    public static class StatusCodes
    {
        /// <summary>The PicoScope is functioning correctly.</summary>
        public const uint PicoOk = (uint)0x00000000UL;

        /// <summary>An attempt has been made to open more than -API-_MAX_UNITS.</summary>
        public const uint PicoMaxUnitsOpened = (uint)0x00000001UL;

        /// <summary>Not enough memory could be allocated on the host machine.</summary>
        public const uint PicoMemoryFail = (uint)0x00000002UL;

        /// <summary>No Pico Technology device could be found.</summary>
        public const uint PicoNotFound = (uint)0x00000003UL;

        /// <summary>Unable to download firmware.</summary>
        public const uint PicoFwFail = (uint)0x00000004UL;

        /// <summary>The driver is busy opening a device.</summary>
        public const uint PicoOpenOperationInProgress = (uint)0x00000005UL;

        /// <summary>An unspecified failure occurred.</summary>
        public const uint PicoOperationFailed = (uint)0x00000006UL;

        /// <summary>The PicoScope is not responding to commands from the PC.</summary>
        public const uint PicoNotResponding = (uint)0x00000007UL;

        /// <summary>The configuration information in the PicoScope is corrupt or missing.</summary>
        public const uint PicoConfigFail = (uint)0x00000008UL;

        /// <summary>The picopp.sys file is too old to be used with the device driver.</summary>
        public const uint PicoKernelDriverTooOld = (uint)0x00000009UL;

        /// <summary>The EEPROM has become corrupt, so the device will use a default setting.</summary>
        public const uint PicoEepromCorrupt = (uint)0x0000000AUL;

        /// <summary>The operating system on the PC is not supported by this driver.</summary>
        public const uint PicoOsNotSupported = (uint)0x0000000BUL;

        /// <summary>There is no device with the handle value passed.</summary>
        public const uint PicoInvalidHandle = (uint)0x0000000CUL;

        /// <summary>A parameter value is not valid.</summary>
        public const uint PicoInvalidParameter = (uint)0x0000000DUL;

        /// <summary>The timebase is not supported or is invalid.</summary>
        public const uint PicoInvalidTimebase = (uint)0x0000000EUL;

        /// <summary>The voltage range is not supported or is invalid.</summary>
        public const uint PicoInvalidVoltageRange = (uint)0x0000000FUL;

        /// <summary>The channel number is not valid on this device or no channels have been set.</summary>
        public const uint PicoInvalidChannel = (uint)0x00000010UL;

        /// <summary>The channel set for a trigger is not available on this device.</summary>
        public const uint PicoInvalidTriggerChannel = (uint)0x00000011UL;

        /// <summary>The channel set for a condition is not available on this device.</summary>
        public const uint PicoInvalidConditionChannel = (uint)0x00000012UL;

        /// <summary>The device does not have a signal generator.</summary>
        public const uint PicoNoSignalGenerator = (uint)0x00000013UL;

        /// <summary>Streaming has failed to start or has stopped without user request.</summary>
        public const uint PicoStreamingFailed = (uint)0x00000014UL;

        /// <summary>Block failed to start - a parameter may have been set wrongly.</summary>
        public const uint PicoBlockModeFailed = (uint)0x00000015UL;

        /// <summary>A parameter that was required is NULL.</summary>
        public const uint PicoNullParameter = (uint)0x00000016UL;

        /// <summary>The current functionality is not available while using ETS capture mode.</summary>
        public const uint PicoEtsModeSet = (uint)0x00000017UL;

        /// <summary>No data is available from a run block call.</summary>
        public const uint PicoDataNotAvailable = (uint)0x00000018UL;

        /// <summary>The buffer passed for the information was too small.</summary>
        public const uint PicoStringBufferToSmall = (uint)0x00000019UL;

        /// <summary>ETS is not supported on this device.</summary>
        public const uint PicoEtsNotSupported = (uint)0x0000001AUL;

        /// <summary>The auto trigger time is less than the time it will take to collect the pre-trigger data.</summary>
        public const uint PicoAutoTriggerTimeToShort = (uint)0x0000001BUL;

        /// <summary>The collection of data has stalled as unread data would be overwritten.</summary>
        public const uint PicoBufferStall = (uint)0x0000001CUL;

        /// <summary>Number of samples requested is more than available in the current memory segment.</summary>
        public const uint PicoTooManySamples = (uint)0x0000001DUL;

        /// <summary>Not possible to create number of segments requested.</summary>
        public const uint PicoTooManySegments = (uint)0x0000001EUL;

        /// <summary>
        /// A null pointer has been passed in the trigger function
        /// or one of the parameters is out of range.
        /// </summary>
        public const uint PicoPulseWidthQualifier = (uint)0x0000001FUL;

        /// <summary>One or more of the hold-off parameters are out of range.</summary>
        public const uint PicoDelay = (uint)0x00000020UL;

        /// <summary>One or more of the source details are incorrect.</summary>
        public const uint PicoSourceDetails = (uint)0x00000021UL;

        /// <summary>One or more of the conditions are incorrect.</summary>
        public const uint PicoConditions = (uint)0x00000022UL;

        /// <summary>
        /// The driver's thread is currently in the -API-Ready callback function
        /// and therefore the action cannot be carried out.
        /// </summary>
        public const uint PicoUserCallback = (uint)0x00000023UL;

        /// <summary>
        /// An attempt is being made to get stored data while streaming.
        /// Either stop streaming by calling -API-Stop, or use -API-GetStreamingLatestValues.
        /// </summary>
        public const uint PicoDeviceSampling = (uint)0x00000024UL;

        /// <summary>Data is unavailable because a run has not been completed.</summary>
        public const uint PicoNoSamplesAvailable = (uint)0x00000025UL;

        /// <summary>The memory segment index is out of range.</summary>
        public const uint PicoSegmentOutOfRange = (uint)0x00000026UL;

        /// <summary>The device is busy so data cannot be returned yet.</summary>
        public const uint PicoBusy = (uint)0x00000027UL;

        /// <summary>The start time to get stored data is out of range.</summary>
        public const uint PicoStartIndexInvalid = (uint)0x00000028UL;

        /// <summary>The information number requested is not a valid number.</summary>
        public const uint PicoInvalidInfo = (uint)0x00000029UL;

        /// <summary>
        /// The handle is invalid so no information is available about the device.
        /// Only PICO_DRIVER_VERSION is available
        /// </summary>
        public const uint PicoInfoUnavailable = (uint)0x0000002AUL;

        /// <summary>The sample interval selected for streaming is out of range.</summary>
        public const uint PicoInvalidSampleInterval = (uint)0x0000002BUL;

        /// <summary>ETS is set but no trigger has been set. A trigger setting is required for ETS.</summary>
        public const uint PicoTriggerError = (uint)0x0000002CUL;

        /// <summary>Driver cannot allocate memory.</summary>
        public const uint PicoMemory = (uint)0x0000002DUL;

        /// <summary>Incorrect parameter passed to the signal generator.</summary>
        public const uint PicoSigGenParam = (uint)0x0000002EUL;

        /// <summary>Conflict between the shots and sweeps parameters sent to the signal generator.</summary>
        public const uint PicoShotsSweepsWarning = (uint)0x0000002FUL;

        /// <summary>A software trigger has been sent but the trigger source is not a software trigger.</summary>
        public const uint PicoSiggenTriggerSource = (uint)0x00000030UL;

        /// <summary>
        /// An -API-SetTrigger call has found a conflict
        /// between the trigger source and the AUX output enable.
        /// </summary>
        public const uint PicoAuxOutputConflict = (uint)0x00000031UL;

        /// <summary>ETS mode is being used and AUX is set as an input.</summary>
        public const uint PicoAuxOutputEtsConflict = (uint)0x00000032UL;

        /// <summary>Attempt to set different EXT input thresholds set
        /// for signal generator and oscilloscope trigger.
        /// </summary>
        public const uint PicoWarningExtThresholdConflict = (uint)0x00000033UL;

        /// <summary>An -API-SetTrigger... function has set AUX as an output
        /// and the signal generator is using it as a trigger.
        /// </summary>
        public const uint PicoWarningAuxOutputConflict = (uint)0x00000034UL;

        /// <summary>
        /// The combined peak to peak voltage and the analog offset voltage
        /// exceed the maximum voltage the signal generator can produce.
        /// </summary>
        public const uint PicoSiggenOutputOverVoltage = (uint)0x00000035UL;

        /// <summary>NULL pointer passed as delay parameter.</summary>
        public const uint PicoDelayNull = (uint)0x00000036UL;

        /// <summary>The buffers for overview data have not been set while streaming.</summary>
        public const uint PicoInvalidBuffer = (uint)0x00000037UL;

        /// <summary>The analog offset voltage is out of range.</summary>
        public const uint PicoSiggenOffsetVoltage = (uint)0x00000038UL;

        /// <summary>The analog peak-to-peak voltage is out of range.</summary>
        public const uint PicoSiggenPkToPk = (uint)0x00000039UL;

        /// <summary>A block collection has been canceled.</summary>
        public const uint PicoCancelled = (uint)0x0000003AUL;

        /// <summary>The segment index is not currently being used.</summary>
        public const uint PicoSegmentNotUsed = (uint)0x0000003BUL;

        /// <summary>The wrong GetValues function has been called for the collection mode in use.</summary>
        public const uint PicoInvalidCall = (uint)0x0000003CUL;

        /// <summary></summary>
        public const uint PicoGetValuesInterrupted = (uint)0x0000003DUL;

        /// <summary>The function is not available.</summary>
        public const uint PicoNotUsed = (uint)0x0000003FUL;

        /// <summary>The aggregation ratio requested is out of range.</summary>
        public const uint PicoInvalidSampleRatio = (uint)0x00000040UL;

        /// <summary>Device is in an invalid state.</summary>
        public const uint PicoInvalidState = (uint)0x00000041UL;

        /// <summary>The number of segments allocated is fewer than the number of captures requested.</summary>
        public const uint PicoNotEnoughSegments = (uint)0x00000042UL;

        /// <summary>
        /// A driver function has already been called and not yet finished.
        /// Only one call to the driver can be made at any one time.
        /// </summary>
        public const uint PicoDriverFunction = (uint)0x00000043UL;

        /// <summary>Not used</summary>
        public const uint PicoReserved = (uint)0x00000044UL;

        /// <summary>An invalid coupling type was specified in -API-SetChannel.</summary>
        public const uint PicoInvalidCoupling = (uint)0x00000045UL;

        /// <summary>An attempt was made to get data before a data buffer was defined.</summary>
        public const uint PicoBuffersNotSet = (uint)0x00000046UL;

        /// <summary>The selected down sampling mode (used for data reduction) is not allowed.</summary>
        public const uint PicoRatioModeNotSupported = (uint)0x00000047UL;

        /// <summary>Aggregation was requested in rapid block mode.</summary>
        public const uint PicoRapidNotSupportAggregation = (uint)0x00000048UL;

        /// <summary>An invalid parameter was passed to -API-SetTriggerChannelProperties.</summary>
        public const uint PicoInvalidTriggerProperty = (uint)0x00000049UL;

        /// <summary>The driver was unable to contact the oscilloscope.</summary>
        public const uint PicoInterfaceNotConnected = (uint)0x0000004AUL;

        /// <summary>Resistance-measuring mode is not allowed in conjunction with the specified probe.</summary>
        public const uint PicoResistanceAndProbeNotAllowed = (uint)0x0000004BUL;

        /// <summary>The device was unexpectedly powered down.</summary>
        public const uint PicoPowerFailed = (uint)0x0000004CUL;

        /// <summary>A problem occurred in -API-SetSigGenBuiltIn or -API-SetSigGenArbitrary.</summary>
        public const uint PicoSiggenWaveformSetupFailed = (uint)0x0000004DUL;

        /// <summary>FPGA not successfully set up.</summary>
        public const uint PicoFpgaFail = (uint)0x0000004EUL;

        /// <summary></summary>
        public const uint PicoPowerManager = (uint)0x0000004FUL;

        /// <summary>An impossible analog offset value was specified in -API-SetChannel.</summary>
        public const uint PicoInvalidAnalogueOffset = (uint)0x00000050UL;

        /// <summary>There is an error within the device hardware.</summary>
        public const uint PicoPllLockFailed = (uint)0x00000051UL;

        /// <summary>There is an error within the device hardware.</summary>
        public const uint PicoAnalogBoard = (uint)0x00000052UL;

        /// <summary>Unable to configure the signal generator.</summary>
        public const uint PicoConfigFailAwg = (uint)0x00000053UL;

        /// <summary>The FPGA cannot be initialized, so unit cannot be opened.</summary>
        public const uint PicoInitialiseFpga = (uint)0x00000054UL;

        /// <summary>The frequency for the external clock is not within 15% of the nominal value.</summary>
        public const uint PicoExternalFrequencyInvalid = (uint)0x00000056UL;

        /// <summary>The FPGA could not lock the clock signal.</summary>
        public const uint PicoClockChangeError = (uint)0x00000057UL;

        /// <summary>You are trying to configure the AUX input as both a trigger and a reference clock.</summary>
        public const uint PicoTriggerAndExternalClockClash = (uint)0x00000058UL;

        /// <summary>
        /// You are trying to configure the AUX input as both a pulse width qualifier
        /// and a reference clock.
        /// </summary>
        public const uint PicoPwqAndExternalClockClash = (uint)0x00000059UL;

        /// <summary>The requested scaling file cannot be opened.</summary>
        public const uint PicoUnableToOpenScalingFile = (uint)0x0000005AUL;

        /// <summary>The frequency of the memory is reporting incorrectly.</summary>
        public const uint PicoMemoryClockFrequency = (uint)0x0000005BUL;

        /// <summary>The I2C that is being actioned is not responding to requests.</summary>
        public const uint PicoI2CNotResponding = (uint)0x0000005CUL;

        /// <summary>There are no captures available and therefore no data can be returned.</summary>
        public const uint PicoNoCapturesAvailable = (uint)0x0000005DUL;

        /// <summary>
        /// The number of trigger channels is greater than 4,
        /// except for a PS4824 where 8 channels are allowed for rising/falling/rising_or_falling trigger directions.
        /// </summary>
        public const uint PicoTooManyTriggerChannelsInUse = (uint)0x0000005FUL;

        /// <summary>When more than 4 trigger channels are set on a PS4824 and the direction is out of range.</summary>
        public const uint PicoInvalidTriggerDirection = (uint)0x00000060UL;

        /// <summary>
        /// When more than 4 trigger channels are set
        /// and their trigger condition states are not -API-_CONDITION_TRUE.
        /// </summary>
        public const uint PicoInvalidTriggerStates = (uint)0x00000061UL;

        /// <summary>The capture mode the device is currently running in does not support the current request.</summary>
        public const uint PicoNotUsedInThisCaptureMode = (uint)0x0000005EUL;

        /// <summary></summary>
        public const uint PicoGetDataActive = (uint)0x00000103UL;

        // Codes 104 to 10B are used by the PT104 (USB) when connected via the Network Socket.

        /// <summary>The device is currently connected via the IP Network socket
        /// and thus the call made is not supported.
        /// </summary>
        public const uint PicoIpNetworked = (uint)0x00000104UL;

        /// <summary>An incorrect IP address has been passed to the driver.</summary>
        public const uint PicoInvalidIpAddress = (uint)0x00000105UL;

        /// <summary>The IP socket has failed.</summary>
        public const uint PicoIpSocketFailed = (uint)0x00000106UL;

        /// <summary>The IP socket has timed out.</summary>
        public const uint PicoIpSocketTimeout = (uint)0x00000107UL;

        /// <summary>Failed to apply the requested settings.</summary>
        public const uint PicoSettingsFailed = (uint)0x00000108UL;

        /// <summary>The network connection has failed.</summary>
        public const uint PicoNetworkFailed = (uint)0x00000109UL;

        /// <summary>Unable to load the WS2 DLL.</summary>
        public const uint PicoWs232DllNotLoaded = (uint)0x0000010AUL;

        /// <summary>The specified IP port is invalid.</summary>
        public const uint PicoInvalidIpPort = (uint)0x0000010BUL;

        /// <summary>The type of coupling requested is not supported on the opened device.</summary>
        public const uint PicoCouplingNotSupported = (uint)0x0000010CUL;

        /// <summary>Bandwidth limiting is not supported on the opened device.</summary>
        public const uint PicoBandwidthNotSupported = (uint)0x0000010DUL;

        /// <summary>The value requested for the bandwidth limit is out of range.</summary>
        public const uint PicoInvalidBandwidth = (uint)0x0000010EUL;

        /// <summary>The arbitrary waveform generator is not supported by the opened device.</summary>
        public const uint PicoAwgNotSupported = (uint)0x0000010FUL;

        /// <summary>
        /// Data has been requested with ETS mode set but run block has not been called,
        /// or stop has been called.
        /// </summary>
        public const uint PicoEtsNotRunning = (uint)0x00000110UL;

        /// <summary>White noise output is not supported on the opened device.</summary>
        public const uint PicoSigGenWhiteNoiseNotSupported = (uint)0x00000111UL;

        /// <summary>The wave type requested is not supported by the opened device.</summary>
        public const uint PicoSigGenWavetypeNotSupported = (uint)0x00000112UL;

        /// <summary>The requested digital port number is out of range (MSOs only).</summary>
        public const uint PicoInvalidDigitalPort = (uint)0x00000113UL;

        /// <summary>
        /// The digital channel is not in the range -API-_DIGITAL_CHANNEL0 to -API-_DIGITAL_CHANNEL15,
        /// the digital channels that are supported.
        /// </summary>
        public const uint PicoInvalidDigitalChannel = (uint)0x00000114UL;

        /// <summary>
        /// The digital trigger direction is not a valid trigger direction and should be equal
        /// in value to one of the -API-_DIGITAL_DIRECTION enumerations.
        /// </summary>
        public const uint PicoInvalidDigitalTriggerDirection = (uint)0x00000115UL;

        /// <summary>Signal generator does not generate pseudo-random binary sequence.</summary>
        public const uint PicoSigGenPrbsNotSupported = (uint)0x00000116UL;

        /// <summary>When a digital port is enabled, ETS sample mode is not available for use.</summary>
        public const uint PicoEtsNotAvailableWithLogicChannels = (uint)0x00000117UL;

        public const uint PicoWarningRepeatValue = (uint)0x00000118UL;

        /// <summary>4-channel scopes only: The DC power supply is connected.</summary>
        public const uint PicoPowerSupplyConnected = (uint)0x00000119UL;

        /// <summary>4-channel scopes only: The DC power supply is not connected.</summary>
        public const uint PicoPowerSupplyNotConnected = (uint)0x0000011AUL;

        /// <summary>Incorrect power mode passed for current power source.</summary>
        public const uint PicoPowerSupplyRequestInvalid = (uint)0x0000011BUL;

        /// <summary>The supply voltage from the USB source is too low.</summary>
        public const uint PicoPowerSupplyUnderVoltage = (uint)0x0000011CUL;

        /// <summary>The oscilloscope is in the process of capturing data.</summary>
        public const uint PicoCapturingData = (uint)0x0000011DUL;

        /// <summary>A USB 3.0 device is connected to a non-USB 3.0 port.</summary>
        public const uint PicoUsb30DeviceNonUsb30Port = (uint)0x0000011EUL;

        /// <summary>A function has been called that is not supported by the current device.</summary>
        public const uint PicoNotSupportedByThisDevice = (uint)0x0000011FUL;

        /// <summary>The device resolution is invalid (out of range).</summary>
        public const uint PicoInvalidDeviceResolution = (uint)0x00000120UL;

        /// <summary>
        /// The number of channels that can be enabled is limited in 15 and 16-bit modes.
        /// (Flexible Resolution Oscilloscopes only)
        /// </summary>
        public const uint PicoInvalidNumberChannelsForResolution = (uint)0x00000121UL;

        /// <summary>USB power not sufficient for all requested channels.</summary>
        public const uint PicoChannelDisabledDueToUsbPowered = (uint)0x00000122UL;

        /// <summary>The signal generator does not have a configurable DC offset.</summary>
        public const uint PicoSiggenDCVoltageNotConfigurable = (uint)0x00000123UL;

        /// <summary>An attempt has been made to define pre-trigger delay without first enabling a trigger.</summary>
        public const uint PicoNoTriggerEnabledForTriggerInPreTrig = (uint)0x00000124UL;

        /// <summary>An attempt has been made to define pre-trigger delay without first arming a trigger.</summary>
        public const uint PicoTriggerWithinPreTrigNotArmed = (uint)0x00000125UL;

        /// <summary>Pre-trigger delay and post-trigger delay cannot be used at the same time.</summary>
        public const uint PicoTriggerWithinPreNotAllowedWithDelay = (uint)0x00000126UL;

        /// <summary>The array index points to a nonexistent trigger.</summary>
        public const uint PicoTriggerIndexUnavailable = (uint)0x00000127UL;

        /// <summary></summary>
        public const uint PicoAwgClockFrequency = (uint)0x00000128UL;

        /// <summary>There are more 4 analog channels with a trigger condition set.</summary>
        public const uint PicoTooManyChannelsInUse = (uint)0x00000129UL;

        /// <summary>The condition parameter is a null pointer.</summary>
        public const uint PicoNullConditions = (uint)0x0000012AUL;

        /// <summary>There is more than one condition pertaining to the same channel.</summary>
        public const uint PicoDuplicateConditionSource = (uint)0x0000012BUL;

        /// <summary>The parameter relating to condition information is out of range.</summary>
        public const uint PicoInvalidConditionInfo = (uint)0x0000012CUL;

        /// <summary>Reading the metadata has failed.</summary>
        public const uint PicoSettingsReadFailed = (uint)0x0000012DUL;

        /// <summary>Writing the metadata has failed.</summary>
        public const uint PicoSettingsWriteFailed = (uint)0x0000012EUL;

        /// <summary>A parameter has a value out of the expected range.</summary>
        public const uint PicoArgumentOutOfRange = (uint)0x0000012FUL;

        /// <summary>The driver does not support the hardware variant connected.</summary>
        public const uint PicoHardwareVersionNotSupported = (uint)0x00000130UL;

        /// <summary>The driver does not support the digital hardware variant connected.</summary>
        public const uint PicoDigitalHardwareVersionNotSupported = (uint)0x00000131UL;

        /// <summary>The driver does not support the analog hardware variant connected.</summary>
        public const uint PicoAnalogueHardwareVersionNotSupported = (uint)0x00000132UL;

        /// <summary>Converting a channel's ADC value to resistance has failed.</summary>
        public const uint PicoUnableToConvertToResistance = (uint)0x00000133UL;

        /// <summary>The channel is listed more than once in the function call.</summary>
        public const uint PicoDuplicatedChannel = (uint)0x00000134UL;

        /// <summary>The range cannot have resistance conversion applied.</summary>
        public const uint PicoInvalidResistanceConversion = (uint)0x00000135UL;

        /// <summary>An invalid value is in the max buffer.</summary>
        public const uint PicoInvalidValueInMaxBuffer = (uint)0x00000136UL;

        /// <summary>An invalid value is in the min buffer.</summary>
        public const uint PicoInvalidValueInMinBuffer = (uint)0x00000137UL;

        /// <summary>
        /// When calculating the frequency for phase conversion,
        /// the frequency is greater than that supported by the current variant.
        /// </summary>
        public const uint PicoSiggenFrequencyOutOfRange = (uint)0x00000138UL;

        /// <summary>
        /// The device's EEPROM is corrupt.
        /// Contact Pico Technology support: https://www.picotech.com/tech-support.
        /// </summary>
        public const uint PicoEeprom2Corrupt = (uint)0x00000139UL;

        /// <summary>The EEPROM has failed.</summary>
        public const uint PicoEeprom2Fail = (uint)0x0000013AUL;

        /// <summary>The serial buffer is too small for the required information.</summary>
        public const uint PicoSerialBufferTooSmall = (uint)0x0000013BUL;

        /// <summary>
        /// The signal generator trigger and the external clock have both been set.
        /// This is not allowed.
        /// </summary>
        public const uint PicoSiggenTriggerAndExternalClockClash = (uint)0x0000013CUL;

        /// <summary>
        /// The AUX trigger was enabled and the external clock has been enabled,
        /// so the AUX has been automatically disabled.
        /// </summary>
        public const uint PicoWarningSiggenAuxioTriggerDisabled = (uint)0x0000013DUL;

        /// <summary>
        /// The AUX I/O was set as a scope trigger and is now being set as a signal generator
        /// gating trigger. This is not allowed.
        /// </summary>
        public const uint PicoSiggenGatingAuxioNotAvailable = (uint)0x00000013EUL;

        /// <summary>
        /// The AUX I/O was set by the signal generator as a gating trigger and is now being set
        /// as a scope trigger. This is not allowed.
        /// </summary>
        public const uint PicoSiggenGatingAuxioEnabled = (uint)0x00000013FUL;

        /// <summary>A resource has failed to initialize</summary>
        public const uint PicoResourceError = (uint)0x00000140UL;

        /// <summary>The temperature type is out of range</summary>
        public const uint PicoTemperatureTypeInvalid = (uint)0x000000141UL;

        /// <summary>A requested temperature type is not supported on this device</summary>
        public const uint PicoTemperatureTypeNotSupported = (uint)0x000000142UL;

        /// <summary>A read/write to the device has timed out</summary>
        public const uint PicoTimeout = (uint)0x00000143UL;

        /// <summary>The device cannot be connected correctly</summary>
        public const uint PicoDeviceNotFunctioning = (uint)0x00000144UL;

        /// <summary>The driver has experienced an unknown error and is unable to recover from this error</summary>
        public const uint PicoInternalError = (uint)0x00000145UL;

        /// <summary>Used when opening units via IP and more than multiple units have the same ip address</summary>
        public const uint PicoMultipleDevicesFound = (uint)0x00000146UL;

        /// <summary></summary>
        public const uint PicoWarningNumberOfSegmentsReduced = (uint)0x00000147UL;

        /// <summary>The calibration pin states argument is out of range</summary>
        public const uint PicoCalPinsStates = (uint)0x00000148UL;

        /// <summary>The calibration pin frequency argument is out of range</summary>
        public const uint PicoCalPinsFrequency = (uint)0x00000149UL;

        /// <summary>The calibration pin amplitude argument is out of range</summary>
        public const uint PicoCalPinsAmplitude = (uint)0x0000014AUL;

        /// <summary>The calibration pin wavetype argument is out of range</summary>
        public const uint PicoCalPinsWavetype = (uint)0x0000014BUL;

        /// <summary>The calibration pin offset argument is out of range</summary>
        public const uint PicoCalPinsOffset = (uint)0x0000014CUL;

        /// <summary>The probe's identity has a problem</summary>
        public const uint PicoProbeFault = (uint)0x0000014DUL;

        /// <summary>The probe has not been identified</summary>
        public const uint PicoProbeIdentityUnknown = (uint)0x0000014EUL;

        /// <summary>Enabling the probe would cause the device to exceed the allowable current limit</summary>
        public const uint PicoProbePowerDCPowerSupplyRequired = (uint)0x0000014FUL;

        /// <summary>
        /// The DC power supply is connected; enabling the probe would cause the device to exceed the
        /// allowable current limit.
        /// </summary>
        public const uint PicoProbeNotPoweredWithDCPowerSupply = (uint)0x00000150UL;

        /// <summary>Failed to complete probe configuration</summary>
        public const uint PicoProbeConfigFailure = (uint)0x00000151UL;

        /// <summary>Failed to set the callback function, as currently in current callback function</summary>
        public const uint PicoProbeInteractionCallback = (uint)0x00000152UL;

        /// <summary>The probe has been verified but not know on this driver</summary>
        public const uint PicoUnknownIntelligentProbe = (uint)0x00000153UL;

        /// <summary>The intelligent probe cannot be verified</summary>
        public const uint PicoIntelligentProbeCorrupt = (uint)0x00000154UL;

        /// <summary>
        /// The callback is null, probe collection will only start when
        /// first callback is a none null pointer
        /// </summary>
        public const uint PicoProbeCollectionNotStarted = (uint)0x00000155UL;

        /// <summary>The current drawn by the probe(s) has exceeded the allowed limit</summary>
        public const uint PicoProbePowerConsumptionExceeded = (uint)0x00000156UL;

        /// <summary>
        /// The channel range limits have changed due to connecting or disconnecting a probe
        /// the channel has been enabled
        /// </summary>
        public const uint PicoWarningProbeChannelOutOfSync = (uint)0x00000157UL;

        /// <summary>The time stamp per waveform segment has been reset.</summary>
        public const uint PicoDeviceTimeStampReset = (uint)0x01000000UL;

        /// <summary>An internal error has occurred and a watchdog timer has been called.</summary>
        public const uint PicoWatchdogtimer = (uint)0x10000000UL;

        /// <summary>The picoipp.dll has not been found.</summary>
        public const uint PicoIppNotFound = (uint)0x10000001UL;

        /// <summary>A function in the picoipp.dll does not exist.</summary>
        public const uint PicoIppNoFunction = (uint)0x10000002UL;

        /// <summary>The Pico IPP call has failed.</summary>
        public const uint PicoIppError = (uint)0x10000003UL;

        /// <summary>Shadow calibration is not available on this device.</summary>
        public const uint PicoShadowCalNotAvailable = (uint)0x10000004UL;

        /// <summary>Shadow calibration is currently disabled.</summary>
        public const uint PicoShadowCalDisabled = (uint)0x10000005UL;

        /// <summary>Shadow calibration error has occurred.</summary>
        public const uint PicoShadowCalError = (uint)0x10000006UL;

        /// <summary>The shadow calibration is corrupt.</summary>
        public const uint PicoShadowCalCorrupt = (uint)0x10000007UL;

        /// <summary>The memory onboard the device has overflowed</summary>
        public const uint PicoDeviceMemoryOverflow = (uint)0x10000008UL;

        /// <summary>Reserved</summary>
        public const uint PicoReserved1 = (uint)0x11000000UL;
    }
}
