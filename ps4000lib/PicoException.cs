/*
 * File: PicoException.cs
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

using System;
using PS4000Lib.Shared;

namespace PS4000Lib
{
    public class PicoException : Exception
    {
        public PicoException() { }

        public PicoException(string message) : base(message) { }

        public PicoException(string message, Exception inner) : base(message, inner) { }

        public PicoException(short status) : this((uint)status) { }
        public PicoException(uint status) : base(ToMessage(status)) { }

        public static string ToMessage(uint status)
        {
            var message = status switch
            {
                StatusCodes.PicoMaxUnitsOpened => "An attempt has been made to open more than ps4000_MAX_UNITS.",
                StatusCodes.PicoMemoryFail => "Not enough memory could be allocated on the host machine.",
                StatusCodes.PicoNotFound => "No Pico Technology device could be found.",
                StatusCodes.PicoFwFail => "Unable to download firmware.",
                StatusCodes.PicoOpenOperationInProgress => "The driver is busy opening a device.",
                StatusCodes.PicoOperationFailed => "An unspecified failure occurred.",
                StatusCodes.PicoNotResponding => "The PicoScope is not responding to commands from the PC.",
                StatusCodes.PicoConfigFail => "The configuration information in the PicoScope is corrupt or missing.",
                StatusCodes.PicoKernelDriverTooOld =>
                    "The picopp.sys file is too old to be used with the device driver.",
                StatusCodes.PicoEepromCorrupt =>
                    "The EEPROM has become corrupt, so the device will use a default setting.",
                StatusCodes.PicoOsNotSupported => "The operating system on the PC is not supported by this driver.",
                StatusCodes.PicoInvalidHandle => "There is no device with the handle value passed.",
                StatusCodes.PicoInvalidParameter => "A parameter value is not valid.",
                StatusCodes.PicoInvalidTimebase => "The timebase is not supported or is invalid.",
                StatusCodes.PicoInvalidVoltageRange => "The voltage range is not supported or is invalid.",
                StatusCodes.PicoInvalidChannel =>
                    "The channel number is not valid on this device or no channels have been set.",
                StatusCodes.PicoInvalidTriggerChannel =>
                    "The channel set for a trigger is not available on this device.",
                StatusCodes.PicoInvalidConditionChannel =>
                    "The channel set for a condition is not available on this device.",
                StatusCodes.PicoNoSignalGenerator => "The device does not have a signal generator.",
                StatusCodes.PicoStreamingFailed => "Streaming has failed to start or has stopped without user request.",
                StatusCodes.PicoBlockModeFailed => "Block failed to start - a parameter may have been set wrongly.",
                StatusCodes.PicoNullParameter => "A parameter that was required is NULL.",
                StatusCodes.PicoEtsModeSet =>
                    "The current functionality is not available while using ETS capture mode.",
                StatusCodes.PicoDataNotAvailable => "No data is available from a run block call.",
                StatusCodes.PicoStringBufferToSmall => "The buffer passed for the information was too small.",
                StatusCodes.PicoEtsNotSupported => "ETS is not supported on this device.",
                StatusCodes.PicoAutoTriggerTimeToShort =>
                    "The auto trigger time is less than the time it will take to collect the pre-trigger data.",
                StatusCodes.PicoBufferStall =>
                    "The collection of data has stalled as unread data would be overwritten.",
                StatusCodes.PicoTooManySamples =>
                    "Number of samples requested is more than available in the current memory segment.",
                StatusCodes.PicoTooManySegments => "Not possible to create number of segments requested.",
                StatusCodes.PicoPulseWidthQualifier =>
                    "A null pointer has been passed in the trigger function or one of the parameters is out of range.",
                StatusCodes.PicoDelay => "One or more of the hold-off parameters are out of range.",
                StatusCodes.PicoSourceDetails => "One or more of the source details are incorrect.",
                StatusCodes.PicoConditions => "One or more of the conditions are incorrect.",
                StatusCodes.PicoUserCallback =>
                    "The driver's thread is currently in the ps4000Ready callback function and therefore the action cannot be carried out.",
                StatusCodes.PicoDeviceSampling =>
                    "An attempt is being made to get stored data while streaming. Either stop streaming by calling ps4000Stop, or use ps4000GetStreamingLatestValues.",
                StatusCodes.PicoNoSamplesAvailable => "Data is unavailable because a run has not been completed.",
                StatusCodes.PicoSegmentOutOfRange => "The memory segment index is out of range.",
                StatusCodes.PicoBusy => "The device is busy so data cannot be returned yet.",
                StatusCodes.PicoStartIndexInvalid => "The start time to get stored data is out of range.",
                StatusCodes.PicoInvalidInfo => "The information number requested is not a valid number.",
                StatusCodes.PicoInfoUnavailable =>
                    "The handle is invalid so no information is available about the device. Only PICO_DRIVER_VERSION is available.",
                StatusCodes.PicoInvalidSampleInterval => "The sample interval selected for streaming is out of range.",
                StatusCodes.PicoTriggerError =>
                    "ETS is set but no trigger has been set. A trigger setting is required for ETS.",
                StatusCodes.PicoMemory => "Driver cannot allocate memory.",
                StatusCodes.PicoSigGenParam => "Incorrect parameter passed to the signal generator.",
                StatusCodes.PicoShotsSweepsWarning =>
                    "Conflict between the shots and sweeps parameters sent to the signal generator.",
                StatusCodes.PicoSiggenTriggerSource =>
                    "A software trigger has been sent but the trigger source is not a software trigger.",
                StatusCodes.PicoAuxOutputConflict =>
                    "An ps4000SetTrigger call has found a conflict between the trigger source and the AUX output enable.",
                StatusCodes.PicoAuxOutputEtsConflict => "ETS mode is being used and AUX is set as an input.",
                StatusCodes.PicoWarningExtThresholdConflict =>
                    "Attempt to set different EXT input thresholds set for signal generator and oscilloscope trigger.",
                StatusCodes.PicoWarningAuxOutputConflict =>
                    "An ps4000SetTrigger... function has set AUX as an output and the signal generator is using it as a trigger.",
                StatusCodes.PicoSiggenOutputOverVoltage =>
                    "The combined peak to peak voltage and the analog offset voltage exceed the maximum voltage the signal generator can produce.",
                StatusCodes.PicoDelayNull => "NULL pointer passed as delay parameter.",
                StatusCodes.PicoInvalidBuffer => "The buffers for overview data have not been set while streaming.",
                StatusCodes.PicoSiggenOffsetVoltage => "The analog offset voltage is out of range.",
                StatusCodes.PicoSiggenPkToPk => "The analog peak-to-peak voltage is out of range.",
                StatusCodes.PicoCancelled => "A block collection has been canceled.",
                StatusCodes.PicoSegmentNotUsed => "The segment index is not currently being used.",
                StatusCodes.PicoInvalidCall =>
                    "The wrong GetValues function has been called for the collection mode in use.",
                StatusCodes.PicoNotUsed => "The function is not available.",
                StatusCodes.PicoInvalidSampleRatio => "The aggregation ratio requested is out of range.",
                StatusCodes.PicoInvalidState => "Device is in an invalid state.",
                StatusCodes.PicoNotEnoughSegments =>
                    "The number of segments allocated is fewer than the number of captures requested.",
                StatusCodes.PicoDriverFunction =>
                    "A driver function has already been called and not yet finished. Only one call to the driver can be made at any one time.",
                StatusCodes.PicoReserved => "Not used.",
                StatusCodes.PicoInvalidCoupling => "An invalid coupling type was specified in ps4000SetChannel.",
                StatusCodes.PicoBuffersNotSet => "An attempt was made to get data before a data buffer was defined.",
                StatusCodes.PicoRatioModeNotSupported =>
                    "The selected down sampling mode (used for data reduction) is not allowed.",
                StatusCodes.PicoRapidNotSupportAggregation => "Aggregation was requested in rapid block mode.",
                StatusCodes.PicoInvalidTriggerProperty =>
                    "An invalid parameter was passed to ps4000SetTriggerChannelProperties.",
                StatusCodes.PicoInterfaceNotConnected => "The driver was unable to contact the oscilloscope.",
                StatusCodes.PicoResistanceAndProbeNotAllowed =>
                    "Resistance-measuring mode is not allowed in conjunction with the specified probe.",
                StatusCodes.PicoPowerFailed => "The device was unexpectedly powered down.",
                StatusCodes.PicoSiggenWaveformSetupFailed =>
                    "A problem occurred in ps4000SetSigGenBuiltIn or ps4000SetSigGenArbitrary.",
                StatusCodes.PicoFpgaFail => "FPGA not successfully set up.",
                StatusCodes.PicoInvalidAnalogueOffset =>
                    "An impossible analog offset value was specified in ps4000SetChannel.",
                StatusCodes.PicoPllLockFailed => "There is an error within the device hardware.",
                StatusCodes.PicoAnalogBoard => "There is an error within the device hardware.",
                StatusCodes.PicoConfigFailAwg => "Unable to configure the signal generator.",
                StatusCodes.PicoInitialiseFpga => "The FPGA cannot be initialized, so unit cannot be opened.",
                StatusCodes.PicoExternalFrequencyInvalid =>
                    "The frequency for the external clock is not within 15% of the nominal value.",
                StatusCodes.PicoClockChangeError => "The FPGA could not lock the clock signal.",
                StatusCodes.PicoTriggerAndExternalClockClash =>
                    "You are trying to configure the AUX input as both a trigger and a reference clock.",
                StatusCodes.PicoPwqAndExternalClockClash =>
                    "You are trying to configure the AUX input as both a pulse width qualifier and a reference clock.",
                StatusCodes.PicoUnableToOpenScalingFile => "The requested scaling file cannot be opened.",
                StatusCodes.PicoMemoryClockFrequency => "The frequency of the memory is reporting incorrectly.",
                StatusCodes.PicoI2CNotResponding => "The I2C that is being actioned is not responding to requests.",
                StatusCodes.PicoNoCapturesAvailable =>
                    "There are no captures available and therefore no data can be returned.",
                StatusCodes.PicoTooManyTriggerChannelsInUse =>
                    "The number of trigger channels is greater than 4, except for a PS4824 where 8 channels are allowed for rising/falling/rising_or_falling trigger directions.",
                StatusCodes.PicoInvalidTriggerDirection =>
                    "When more than 4 trigger channels are set on a PS4824 and the direction is out of range.",
                StatusCodes.PicoInvalidTriggerStates =>
                    "When more than 4 trigger channels are set and their trigger condition states are not ps4000_CONDITION_TRUE.",
                StatusCodes.PicoNotUsedInThisCaptureMode =>
                    "The capture mode the device is currently running in does not support the current request.",
                StatusCodes.PicoIpNetworked =>
                    "The device is currently connected via the IP Network socket and thus the call made is not supported.",
                StatusCodes.PicoInvalidIpAddress => "An incorrect IP address has been passed to the driver.",
                StatusCodes.PicoIpSocketFailed => "The IP socket has failed.",
                StatusCodes.PicoIpSocketTimeout => "The IP socket has timed out.",
                StatusCodes.PicoSettingsFailed => "Failed to apply the requested settings.",
                StatusCodes.PicoNetworkFailed => "The network connection has failed.",
                StatusCodes.PicoWs232DllNotLoaded => "Unable to load the WS2 DLL.",
                StatusCodes.PicoInvalidIpPort => "The specified IP port is invalid.",
                StatusCodes.PicoCouplingNotSupported =>
                    "The type of coupling requested is not supported on the opened device.",
                StatusCodes.PicoBandwidthNotSupported => "Bandwidth limiting is not supported on the opened device.",
                StatusCodes.PicoInvalidBandwidth => "The value requested for the bandwidth limit is out of range.",
                StatusCodes.PicoAwgNotSupported =>
                    "The arbitrary waveform generator is not supported by the opened device.",
                StatusCodes.PicoEtsNotRunning =>
                    "Data has been requested with ETS mode set but run block has not been called, or stop has been called.",
                StatusCodes.PicoSigGenWhiteNoiseNotSupported =>
                    "White noise output is not supported on the opened device.",
                StatusCodes.PicoSigGenWavetypeNotSupported =>
                    "The wave type requested is not supported by the opened device.",
                StatusCodes.PicoInvalidDigitalPort => "The requested digital port number is out of range (MSOs only).",
                StatusCodes.PicoInvalidDigitalChannel =>
                    "The digital channel is not in the range ps4000_DIGITAL_CHANNEL0 to ps4000_DIGITAL_CHANNEL15, the digital channels that are supported.",
                StatusCodes.PicoInvalidDigitalTriggerDirection =>
                    "The digital trigger direction is not a valid trigger direction and should be equal in value to one of the ps4000_DIGITAL_DIRECTION enumerations.",
                StatusCodes.PicoSigGenPrbsNotSupported =>
                    "Signal generator does not generate pseudo-random binary sequence.",
                StatusCodes.PicoEtsNotAvailableWithLogicChannels =>
                    "When a digital port is enabled, ETS sample mode is not available for use.",
                StatusCodes.PicoPowerSupplyConnected => "4-channel scopes only: The DC power supply is connected.",
                StatusCodes.PicoPowerSupplyNotConnected =>
                    "4-channel scopes only: The DC power supply is not connected.",
                StatusCodes.PicoPowerSupplyRequestInvalid => "Incorrect power mode passed for current power source.",
                StatusCodes.PicoPowerSupplyUnderVoltage => "The supply voltage from the USB source is too low.",
                StatusCodes.PicoCapturingData => "The oscilloscope is in the process of capturing data.",
                StatusCodes.PicoUsb30DeviceNonUsb30Port => "A USB 3.0 device is connected to a non-USB 3.0 port.",
                StatusCodes.PicoNotSupportedByThisDevice =>
                    "A function has been called that is not supported by the current device.",
                StatusCodes.PicoInvalidDeviceResolution => "The device resolution is invalid (out of range).",
                StatusCodes.PicoInvalidNumberChannelsForResolution =>
                    "The number of channels that can be enabled is limited in 15 and 16-bit modes. (Flexible Resolution Oscilloscopes only)",
                StatusCodes.PicoChannelDisabledDueToUsbPowered =>
                    "USB power not sufficient for all requested channels.",
                StatusCodes.PicoSiggenDCVoltageNotConfigurable =>
                    "The signal generator does not have a configurable DC offset.",
                StatusCodes.PicoNoTriggerEnabledForTriggerInPreTrig =>
                    "An attempt has been made to define pre-trigger delay without first enabling a trigger.",
                StatusCodes.PicoTriggerWithinPreTrigNotArmed =>
                    "An attempt has been made to define pre-trigger delay without first arming a trigger.",
                StatusCodes.PicoTriggerWithinPreNotAllowedWithDelay =>
                    "Pre-trigger delay and post-trigger delay cannot be used at the same time.",
                StatusCodes.PicoTriggerIndexUnavailable => "The array index points to a nonexistent trigger.",
                StatusCodes.PicoTooManyChannelsInUse =>
                    "There are more 4 analog channels with a trigger condition set.",
                StatusCodes.PicoNullConditions => "The condition parameter is a null pointer.",
                StatusCodes.PicoDuplicateConditionSource =>
                    "There is more than one condition pertaining to the same channel.",
                StatusCodes.PicoInvalidConditionInfo =>
                    "The parameter relating to condition information is out of range.",
                StatusCodes.PicoSettingsReadFailed => "Reading the metadata has failed.",
                StatusCodes.PicoSettingsWriteFailed => "Writing the metadata has failed.",
                StatusCodes.PicoArgumentOutOfRange => "A parameter has a value out of the expected range.",
                StatusCodes.PicoHardwareVersionNotSupported =>
                    "The driver does not support the hardware variant connected.",
                StatusCodes.PicoDigitalHardwareVersionNotSupported =>
                    "The driver does not support the digital hardware variant connected.",
                StatusCodes.PicoAnalogueHardwareVersionNotSupported =>
                    "The driver does not support the analog hardware variant connected.",
                StatusCodes.PicoUnableToConvertToResistance =>
                    "Converting a channel's ADC value to resistance has failed.",
                StatusCodes.PicoDuplicatedChannel => "The channel is listed more than once in the function call.",
                StatusCodes.PicoInvalidResistanceConversion => "The range cannot have resistance conversion applied.",
                StatusCodes.PicoInvalidValueInMaxBuffer => "An invalid value is in the max buffer.",
                StatusCodes.PicoInvalidValueInMinBuffer => "An invalid value is in the min buffer.",
                StatusCodes.PicoSiggenFrequencyOutOfRange =>
                    "When calculating the frequency for phase conversion, the frequency is greater than that supported by the current variant.",
                StatusCodes.PicoEeprom2Corrupt =>
                    "The device's EEPROM is corrupt. Contact Pico Technology support: https://www.picotech.com/tech-support.",
                StatusCodes.PicoEeprom2Fail => "The EEPROM has failed.",
                StatusCodes.PicoSerialBufferTooSmall => "The serial buffer is too small for the required information.",
                StatusCodes.PicoSiggenTriggerAndExternalClockClash =>
                    "The signal generator trigger and the external clock have both been set. This is not allowed.",
                StatusCodes.PicoWarningSiggenAuxioTriggerDisabled =>
                    "The AUX trigger was enabled and the external clock has been enabled, so the AUX has been automatically disabled.",
                StatusCodes.PicoSiggenGatingAuxioNotAvailable =>
                    "The AUX I/O was set as a scope trigger and is now being set as a signal generator gating trigger. This is not allowed.",
                StatusCodes.PicoSiggenGatingAuxioEnabled =>
                    "The AUX I/O was set by the signal generator as a gating trigger and is now being set as a scope trigger. This is not allowed.",
                StatusCodes.PicoResourceError => "A resource has failed to initialize.",
                StatusCodes.PicoTemperatureTypeInvalid => "The temperature type is out of range.",
                StatusCodes.PicoTemperatureTypeNotSupported =>
                    "A requested temperature type is not supported on this device.",
                StatusCodes.PicoTimeout => "A read/write to the device has timed out.",
                StatusCodes.PicoDeviceNotFunctioning => "The device cannot be connected correctly.",
                StatusCodes.PicoInternalError =>
                    "The driver has experienced an unknown error and is unable to recover from this error.",
                StatusCodes.PicoMultipleDevicesFound =>
                    "Used when opening units via IP and more than multiple units have the same ip address.",
                StatusCodes.PicoCalPinsStates => "The calibration pin states argument is out of range.",
                StatusCodes.PicoCalPinsFrequency => "The calibration pin frequency argument is out of range.",
                StatusCodes.PicoCalPinsAmplitude => "The calibration pin amplitude argument is out of range.",
                StatusCodes.PicoCalPinsWavetype => "The calibration pin wavetype argument is out of range.",
                StatusCodes.PicoCalPinsOffset => "The calibration pin offset argument is out of range.",
                StatusCodes.PicoProbeFault => "The probe's identity has a problem.",
                StatusCodes.PicoProbeIdentityUnknown => "The probe has not been identified.",
                StatusCodes.PicoProbePowerDCPowerSupplyRequired =>
                    "Enabling the probe would cause the device to exceed the allowable current limit.",
                StatusCodes.PicoProbeNotPoweredWithDCPowerSupply =>
                    "The DC power supply is connected; enabling the probe would cause the device to exceed the allowable current limit.",
                StatusCodes.PicoProbeConfigFailure => "Failed to complete probe configuration.",
                StatusCodes.PicoProbeInteractionCallback =>
                    "Failed to set the callback function, as currently in current callback function.",
                StatusCodes.PicoUnknownIntelligentProbe => "The probe has been verified but not know on this driver.",
                StatusCodes.PicoIntelligentProbeCorrupt => "The intelligent probe cannot be verified.",
                StatusCodes.PicoProbeCollectionNotStarted =>
                    "The callback is null, probe collection will only start when first callback is a none null pointer",
                StatusCodes.PicoProbePowerConsumptionExceeded =>
                    "The current drawn by the probe(s) has exceeded the allowed limit.",
                StatusCodes.PicoWarningProbeChannelOutOfSync =>
                    "The channel range limits have changed due to connecting or disconnecting a probe the channel has been enabled",
                StatusCodes.PicoDeviceTimeStampReset => "The time stamp per waveform segment has been reset.",
                StatusCodes.PicoWatchdogtimer => "An internal error has occurred and a watchdog timer has been called.",
                StatusCodes.PicoIppNotFound => "The picoipp.dll has not been found.",
                StatusCodes.PicoIppNoFunction => "A function in the picoipp.dll does not exist.",
                StatusCodes.PicoIppError => "The Pico IPP call has failed.",
                StatusCodes.PicoShadowCalNotAvailable => "Shadow calibration is not available on this device.",
                StatusCodes.PicoShadowCalDisabled => "Shadow calibration is currently disabled.",
                StatusCodes.PicoShadowCalError => "Shadow calibration error has occurred.",
                StatusCodes.PicoShadowCalCorrupt => "The shadow calibration is corrupt.",
                StatusCodes.PicoDeviceMemoryOverflow => "The memory onboard the device has overflowed.",
                StatusCodes.PicoReserved1 => "Reserved.",
                _ => $"ErrorCode: {status}"
            };

            return message;
        }
    }
}
