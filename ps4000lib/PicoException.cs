using PicoStatus;
using System;
using System.Runtime.Serialization;

namespace PS4000Lib
{
    public class PicoException : Exception
    {
        public PicoException() { }

        public PicoException(string message) : base(message) { }

        public PicoException(string message, Exception inner) : base(message, inner) { }

        public PicoException(short status) : this((uint)status) { }
        public PicoException(uint status) : base(ToMessgae(status)) { }

        public static string ToMessgae(uint status)
        {
            string message;
            switch (status)
            {
                case StatusCodes.PICO_MAX_UNITS_OPENED:
                    message = "An attempt has been made to open more than ps4000_MAX_UNITS.";
                    break;
                case StatusCodes.PICO_MEMORY_FAIL:
                    message = "Not enough memory could be allocated on the host machine.";
                    break;
                case StatusCodes.PICO_NOT_FOUND:
                    message = "No Pico Technology device could be found.";
                    break;
                case StatusCodes.PICO_FW_FAIL:
                    message = "Unable to download firmware.";
                    break;
                case StatusCodes.PICO_OPEN_OPERATION_IN_PROGRESS:
                    message = "The driver is busy opening a device.";
                    break;
                case StatusCodes.PICO_OPERATION_FAILED:
                    message = "An unspecified failure occurred.";
                    break;
                case StatusCodes.PICO_NOT_RESPONDING:
                    message = "The PicoScope is not responding to commands from the PC.";
                    break;
                case StatusCodes.PICO_CONFIG_FAIL:
                    message = "The configuration information in the PicoScope is corrupt or missing.";
                    break;
                case StatusCodes.PICO_KERNEL_DRIVER_TOO_OLD:
                    message = "The picopp.sys file is too old to be used with the device driver.";
                    break;
                case StatusCodes.PICO_EEPROM_CORRUPT:
                    message = "The EEPROM has become corrupt, so the device will use a default setting.";
                    break;
                case StatusCodes.PICO_OS_NOT_SUPPORTED:
                    message = "The operating system on the PC is not supported by this driver.";
                    break;
                case StatusCodes.PICO_INVALID_HANDLE:
                    message = "There is no device with the handle value passed.";
                    break;
                case StatusCodes.PICO_INVALID_PARAMETER:
                    message = "A parameter value is not valid.";
                    break;
                case StatusCodes.PICO_INVALID_TIMEBASE:
                    message = "The timebase is not supported or is invalid.";
                    break;
                case StatusCodes.PICO_INVALID_VOLTAGE_RANGE:
                    message = "The voltage range is not supported or is invalid.";
                    break;
                case StatusCodes.PICO_INVALID_CHANNEL:
                    message = "The channel number is not valid on this device or no channels have been set.";
                    break;
                case StatusCodes.PICO_INVALID_TRIGGER_CHANNEL:
                    message = "The channel set for a trigger is not available on this device.";
                    break;
                case StatusCodes.PICO_INVALID_CONDITION_CHANNEL:
                    message = "The channel set for a condition is not available on this device.";
                    break;
                case StatusCodes.PICO_NO_SIGNAL_GENERATOR:
                    message = "The device does not have a signal generator.";
                    break;
                case StatusCodes.PICO_STREAMING_FAILED:
                    message = "Streaming has failed to start or has stopped without user request.";
                    break;
                case StatusCodes.PICO_BLOCK_MODE_FAILED:
                    message = "Block failed to start - a parameter may have been set wrongly.";
                    break;
                case StatusCodes.PICO_NULL_PARAMETER:
                    message = "A parameter that was required is NULL.";
                    break;
                case StatusCodes.PICO_ETS_MODE_SET:
                    message = "The current functionality is not available while using ETS capture mode.";
                    break;
                case StatusCodes.PICO_DATA_NOT_AVAILABLE:
                    message = "No data is available from a run block call.";
                    break;
                case StatusCodes.PICO_STRING_BUFFER_TO_SMALL:
                    message = "The buffer passed for the information was too small.";
                    break;
                case StatusCodes.PICO_ETS_NOT_SUPPORTED:
                    message = "ETS is not supported on this device.";
                    break;
                case StatusCodes.PICO_AUTO_TRIGGER_TIME_TO_SHORT:
                    message = "The auto trigger time is less than the time it will take to collect the pre-trigger data.";
                    break;
                case StatusCodes.PICO_BUFFER_STALL:
                    message = "The collection of data has stalled as unread data would be overwritten.";
                    break;
                case StatusCodes.PICO_TOO_MANY_SAMPLES:
                    message = "Number of samples requested is more than available in the current memory segment.";
                    break;
                case StatusCodes.PICO_TOO_MANY_SEGMENTS:
                    message = "Not possible to create number of segments requested.";
                    break;
                case StatusCodes.PICO_PULSE_WIDTH_QUALIFIER:
                    message = "A null pointer has been passed in the trigger function or one of the parameters is out of range.";
                    break;
                case StatusCodes.PICO_DELAY:
                    message = "One or more of the hold-off parameters are out of range.";
                    break;
                case StatusCodes.PICO_SOURCE_DETAILS:
                    message = "One or more of the source details are incorrect.";
                    break;
                case StatusCodes.PICO_CONDITIONS:
                    message = "One or more of the conditions are incorrect.";
                    break;
                case StatusCodes.PICO_USER_CALLBACK:
                    message = "The driver's thread is currently in the ps4000Ready callback function and therefore the action cannot be carried out.";
                    break;
                case StatusCodes.PICO_DEVICE_SAMPLING:
                    message = "An attempt is being made to get stored data while streaming. Either stop streaming by calling ps4000Stop, or use ps4000GetStreamingLatestValues.";
                    break;
                case StatusCodes.PICO_NO_SAMPLES_AVAILABLE:
                    message = "Data is unavailable because a run has not been completed.";
                    break;
                case StatusCodes.PICO_SEGMENT_OUT_OF_RANGE:
                    message = "The memory segment index is out of range.";
                    break;
                case StatusCodes.PICO_BUSY:
                    message = "The device is busy so data cannot be returned yet.";
                    break;
                case StatusCodes.PICO_STARTINDEX_INVALID:
                    message = "The start time to get stored data is out of range.";
                    break;
                case StatusCodes.PICO_INVALID_INFO:
                    message = "The information number requested is not a valid number.";
                    break;
                case StatusCodes.PICO_INFO_UNAVAILABLE:
                    message = "The handle is invalid so no information is available about the device. Only PICO_DRIVER_VERSION is available.";
                    break;
                case StatusCodes.PICO_INVALID_SAMPLE_INTERVAL:
                    message = "The sample interval selected for streaming is out of range.";
                    break;
                case StatusCodes.PICO_TRIGGER_ERROR:
                    message = "ETS is set but no trigger has been set. A trigger setting is required for ETS.";
                    break;
                case StatusCodes.PICO_MEMORY:
                    message = "Driver cannot allocate memory.";
                    break;
                case StatusCodes.PICO_SIG_GEN_PARAM:
                    message = "Incorrect parameter passed to the signal generator.";
                    break;
                case StatusCodes.PICO_SHOTS_SWEEPS_WARNING:
                    message = "Conflict between the shots and sweeps parameters sent to the signal generator.";
                    break;
                case StatusCodes.PICO_SIGGEN_TRIGGER_SOURCE:
                    message = "A software trigger has been sent but the trigger source is not a software trigger.";
                    break;
                case StatusCodes.PICO_AUX_OUTPUT_CONFLICT:
                    message = "An ps4000SetTrigger call has found a conflict between the trigger source and the AUX output enable.";
                    break;
                case StatusCodes.PICO_AUX_OUTPUT_ETS_CONFLICT:
                    message = "ETS mode is being used and AUX is set as an input.";
                    break;
                case StatusCodes.PICO_WARNING_EXT_THRESHOLD_CONFLICT:
                    message = "Attempt to set different EXT input thresholds set for signal generator and oscilloscope trigger.";
                    break;
                case StatusCodes.PICO_WARNING_AUX_OUTPUT_CONFLICT:
                    message = "An ps4000SetTrigger... function has set AUX as an output and the signal generator is using it as a trigger.";
                    break;
                case StatusCodes.PICO_SIGGEN_OUTPUT_OVER_VOLTAGE:
                    message = "The combined peak to peak voltage and the analog offset voltage exceed the maximum voltage the signal generator can produce.";
                    break;
                case StatusCodes.PICO_DELAY_NULL:
                    message = "NULL pointer passed as delay parameter.";
                    break;
                case StatusCodes.PICO_INVALID_BUFFER:
                    message = "The buffers for overview data have not been set while streaming.";
                    break;
                case StatusCodes.PICO_SIGGEN_OFFSET_VOLTAGE:
                    message = "The analog offset voltage is out of range.";
                    break;
                case StatusCodes.PICO_SIGGEN_PK_TO_PK:
                    message = "The analog peak-to-peak voltage is out of range.";
                    break;
                case StatusCodes.PICO_CANCELLED:
                    message = "A block collection has been canceled.";
                    break;
                case StatusCodes.PICO_SEGMENT_NOT_USED:
                    message = "The segment index is not currently being used.";
                    break;
                case StatusCodes.PICO_INVALID_CALL:
                    message = "The wrong GetValues function has been called for the collection mode in use.";
                    break;
                case StatusCodes.PICO_NOT_USED:
                    message = "The function is not available.";
                    break;
                case StatusCodes.PICO_INVALID_SAMPLERATIO:
                    message = "The aggregation ratio requested is out of range.";
                    break;
                case StatusCodes.PICO_INVALID_STATE:
                    message = "Device is in an invalid state.";
                    break;
                case StatusCodes.PICO_NOT_ENOUGH_SEGMENTS:
                    message = "The number of segments allocated is fewer than the number of captures requested.";
                    break;
                case StatusCodes.PICO_DRIVER_FUNCTION:
                    message = "A driver function has already been called and not yet finished. Only one call to the driver can be made at any one time.";
                    break;
                case StatusCodes.PICO_RESERVED:
                    message = "Not used.";
                    break;
                case StatusCodes.PICO_INVALID_COUPLING:
                    message = "An invalid coupling type was specified in ps4000SetChannel.";
                    break;
                case StatusCodes.PICO_BUFFERS_NOT_SET:
                    message = "An attempt was made to get data before a data buffer was defined.";
                    break;
                case StatusCodes.PICO_RATIO_MODE_NOT_SUPPORTED:
                    message = "The selected down sampling mode (used for data reduction) is not allowed.";
                    break;
                case StatusCodes.PICO_RAPID_NOT_SUPPORT_AGGREGATION:
                    message = "Aggregation was requested in rapid block mode.";
                    break;
                case StatusCodes.PICO_INVALID_TRIGGER_PROPERTY:
                    message = "An invalid parameter was passed to ps4000SetTriggerChannelProperties.";
                    break;
                case StatusCodes.PICO_INTERFACE_NOT_CONNECTED:
                    message = "The driver was unable to contact the oscilloscope.";
                    break;
                case StatusCodes.PICO_RESISTANCE_AND_PROBE_NOT_ALLOWED:
                    message = "Resistance-measuring mode is not allowed in conjunction with the specified probe.";
                    break;
                case StatusCodes.PICO_POWER_FAILED:
                    message = "The device was unexpectedly powered down.";
                    break;
                case StatusCodes.PICO_SIGGEN_WAVEFORM_SETUP_FAILED:
                    message = "A problem occurred in ps4000SetSigGenBuiltIn or ps4000SetSigGenArbitrary.";
                    break;
                case StatusCodes.PICO_FPGA_FAIL:
                    message = "FPGA not successfully set up.";
                    break;
                case StatusCodes.PICO_INVALID_ANALOGUE_OFFSET:
                    message = "An impossible analog offset value was specified in ps4000SetChannel.";
                    break;
                case StatusCodes.PICO_PLL_LOCK_FAILED:
                    message = "There is an error within the device hardware.";
                    break;
                case StatusCodes.PICO_ANALOG_BOARD:
                    message = "There is an error within the device hardware.";
                    break;
                case StatusCodes.PICO_CONFIG_FAIL_AWG:
                    message = "Unable to configure the signal generator.";
                    break;
                case StatusCodes.PICO_INITIALISE_FPGA:
                    message = "The FPGA cannot be initialized, so unit cannot be opened.";
                    break;
                case StatusCodes.PICO_EXTERNAL_FREQUENCY_INVALID:
                    message = "The frequency for the external clock is not within 15% of the nominal value.";
                    break;
                case StatusCodes.PICO_CLOCK_CHANGE_ERROR:
                    message = "The FPGA could not lock the clock signal.";
                    break;
                case StatusCodes.PICO_TRIGGER_AND_EXTERNAL_CLOCK_CLASH:
                    message = "You are trying to configure the AUX input as both a trigger and a reference clock.";
                    break;
                case StatusCodes.PICO_PWQ_AND_EXTERNAL_CLOCK_CLASH:
                    message = "You are trying to configure the AUX input as both a pulse width qualifier and a reference clock.";
                    break;
                case StatusCodes.PICO_UNABLE_TO_OPEN_SCALING_FILE:
                    message = "The requested scaling file cannot be opened.";
                    break;
                case StatusCodes.PICO_MEMORY_CLOCK_FREQUENCY:
                    message = "The frequency of the memory is reporting incorrectly.";
                    break;
                case StatusCodes.PICO_I2C_NOT_RESPONDING:
                    message = "The I2C that is being actioned is not responding to requests.";
                    break;
                case StatusCodes.PICO_NO_CAPTURES_AVAILABLE:
                    message = "There are no captures available and therefore no data can be returned.";
                    break;
                case StatusCodes.PICO_TOO_MANY_TRIGGER_CHANNELS_IN_USE:
                    message = "The number of trigger channels is greater than 4, except for a PS4824 where 8 channels are allowed for rising/falling/rising_or_falling trigger directions.";
                    break;
                case StatusCodes.PICO_INVALID_TRIGGER_DIRECTION:
                    message = "When more than 4 trigger channels are set on a PS4824 and the direction is out of range.";
                    break;
                case StatusCodes.PICO_INVALID_TRIGGER_STATES:
                    message = "When more than 4 trigger channels are set and their trigger condition states are not ps4000_CONDITION_TRUE.";
                    break;
                case StatusCodes.PICO_NOT_USED_IN_THIS_CAPTURE_MODE:
                    message = "The capture mode the device is currently running in does not support the current request.";
                    break;
                case StatusCodes.PICO_IP_NETWORKED:
                    message = "The device is currently connected via the IP Network socket and thus the call made is not supported.";
                    break;
                case StatusCodes.PICO_INVALID_IP_ADDRESS:
                    message = "An incorrect IP address has been passed to the driver.";
                    break;
                case StatusCodes.PICO_IPSOCKET_FAILED:
                    message = "The IP socket has failed.";
                    break;
                case StatusCodes.PICO_IPSOCKET_TIMEDOUT:
                    message = "The IP socket has timed out.";
                    break;
                case StatusCodes.PICO_SETTINGS_FAILED:
                    message = "Failed to apply the requested settings.";
                    break;
                case StatusCodes.PICO_NETWORK_FAILED:
                    message = "The network connection has failed.";
                    break;
                case StatusCodes.PICO_WS2_32_DLL_NOT_LOADED:
                    message = "Unable to load the WS2 DLL.";
                    break;
                case StatusCodes.PICO_INVALID_IP_PORT:
                    message = "The specified IP port is invalid.";
                    break;
                case StatusCodes.PICO_COUPLING_NOT_SUPPORTED:
                    message = "The type of coupling requested is not supported on the opened device.";
                    break;
                case StatusCodes.PICO_BANDWIDTH_NOT_SUPPORTED:
                    message = "Bandwidth limiting is not supported on the opened device.";
                    break;
                case StatusCodes.PICO_INVALID_BANDWIDTH:
                    message = "The value requested for the bandwidth limit is out of range.";
                    break;
                case StatusCodes.PICO_AWG_NOT_SUPPORTED:
                    message = "The arbitrary waveform generator is not supported by the opened device.";
                    break;
                case StatusCodes.PICO_ETS_NOT_RUNNING:
                    message = "Data has been requested with ETS mode set but run block has not been called, or stop has been called.";
                    break;
                case StatusCodes.PICO_SIG_GEN_WHITENOISE_NOT_SUPPORTED:
                    message = "White noise output is not supported on the opened device.";
                    break;
                case StatusCodes.PICO_SIG_GEN_WAVETYPE_NOT_SUPPORTED:
                    message = "The wave type requested is not supported by the opened device.";
                    break;
                case StatusCodes.PICO_INVALID_DIGITAL_PORT:
                    message = "The requested digital port number is out of range (MSOs only).";
                    break;
                case StatusCodes.PICO_INVALID_DIGITAL_CHANNEL:
                    message = "The digital channel is not in the range ps4000_DIGITAL_CHANNEL0 to ps4000_DIGITAL_CHANNEL15, the digital channels that are supported.";
                    break;
                case StatusCodes.PICO_INVALID_DIGITAL_TRIGGER_DIRECTION:
                    message = "The digital trigger direction is not a valid trigger direction and should be equal in value to one of the ps4000_DIGITAL_DIRECTION enumerations.";
                    break;
                case StatusCodes.PICO_SIG_GEN_PRBS_NOT_SUPPORTED:
                    message = "Signal generator does not generate pseudo-random binary sequence.";
                    break;
                case StatusCodes.PICO_ETS_NOT_AVAILABLE_WITH_LOGIC_CHANNELS:
                    message = "When a digital port is enabled, ETS sample mode is not available for use.";
                    break;
                case StatusCodes.PICO_POWER_SUPPLY_CONNECTED:
                    message = "4-channel scopes only: The DC power supply is connected.";
                    break;
                case StatusCodes.PICO_POWER_SUPPLY_NOT_CONNECTED:
                    message = "4-channel scopes only: The DC power supply is not connected.";
                    break;
                case StatusCodes.PICO_POWER_SUPPLY_REQUEST_INVALID:
                    message = "Incorrect power mode passed for current power source.";
                    break;
                case StatusCodes.PICO_POWER_SUPPLY_UNDERVOLTAGE:
                    message = "The supply voltage from the USB source is too low.";
                    break;
                case StatusCodes.PICO_CAPTURING_DATA:
                    message = "The oscilloscope is in the process of capturing data.";
                    break;
                case StatusCodes.PICO_USB3_0_DEVICE_NON_USB3_0_PORT:
                    message = "A USB 3.0 device is connected to a non-USB 3.0 port.";
                    break;
                case StatusCodes.PICO_NOT_SUPPORTED_BY_THIS_DEVICE:
                    message = "A function has been called that is not supported by the current device.";
                    break;
                case StatusCodes.PICO_INVALID_DEVICE_RESOLUTION:
                    message = "The device resolution is invalid (out of range).";
                    break;
                case StatusCodes.PICO_INVALID_NUMBER_CHANNELS_FOR_RESOLUTION:
                    message = "The number of channels that can be enabled is limited in 15 and 16-bit modes. (Flexible Resolution Oscilloscopes only)";
                    break;
                case StatusCodes.PICO_CHANNEL_DISABLED_DUE_TO_USB_POWERED:
                    message = "USB power not sufficient for all requested channels.";
                    break;
                case StatusCodes.PICO_SIGGEN_DC_VOLTAGE_NOT_CONFIGURABLE:
                    message = "The signal generator does not have a configurable DC offset.";
                    break;
                case StatusCodes.PICO_NO_TRIGGER_ENABLED_FOR_TRIGGER_IN_PRE_TRIG:
                    message = "An attempt has been made to define pre-trigger delay without first enabling a trigger.";
                    break;
                case StatusCodes.PICO_TRIGGER_WITHIN_PRE_TRIG_NOT_ARMED:
                    message = "An attempt has been made to define pre-trigger delay without first arming a trigger.";
                    break;
                case StatusCodes.PICO_TRIGGER_WITHIN_PRE_NOT_ALLOWED_WITH_DELAY:
                    message = "Pre-trigger delay and post-trigger delay cannot be used at the same time.";
                    break;
                case StatusCodes.PICO_TRIGGER_INDEX_UNAVAILABLE:
                    message = "The array index points to a nonexistent trigger.";
                    break;
                case StatusCodes.PICO_TOO_MANY_CHANNELS_IN_USE:
                    message = "There are more 4 analog channels with a trigger condition set.";
                    break;
                case StatusCodes.PICO_NULL_CONDITIONS:
                    message = "The condition parameter is a null pointer.";
                    break;
                case StatusCodes.PICO_DUPLICATE_CONDITION_SOURCE:
                    message = "There is more than one condition pertaining to the same channel.";
                    break;
                case StatusCodes.PICO_INVALID_CONDITION_INFO:
                    message = "The parameter relating to condition information is out of range.";
                    break;
                case StatusCodes.PICO_SETTINGS_READ_FAILED:
                    message = "Reading the metadata has failed.";
                    break;
                case StatusCodes.PICO_SETTINGS_WRITE_FAILED:
                    message = "Writing the metadata has failed.";
                    break;
                case StatusCodes.PICO_ARGUMENT_OUT_OF_RANGE:
                    message = "A parameter has a value out of the expected range.";
                    break;
                case StatusCodes.PICO_HARDWARE_VERSION_NOT_SUPPORTED:
                    message = "The driver does not support the hardware variant connected.";
                    break;
                case StatusCodes.PICO_DIGITAL_HARDWARE_VERSION_NOT_SUPPORTED:
                    message = "The driver does not support the digital hardware variant connected.";
                    break;
                case StatusCodes.PICO_ANALOGUE_HARDWARE_VERSION_NOT_SUPPORTED:
                    message = "The driver does not support the analog hardware variant connected.";
                    break;
                case StatusCodes.PICO_UNABLE_TO_CONVERT_TO_RESISTANCE:
                    message = "Converting a channel's ADC value to resistance has failed.";
                    break;
                case StatusCodes.PICO_DUPLICATED_CHANNEL:
                    message = "The channel is listed more than once in the function call.";
                    break;
                case StatusCodes.PICO_INVALID_RESISTANCE_CONVERSION:
                    message = "The range cannot have resistance conversion applied.";
                    break;
                case StatusCodes.PICO_INVALID_VALUE_IN_MAX_BUFFER:
                    message = "An invalid value is in the max buffer.";
                    break;
                case StatusCodes.PICO_INVALID_VALUE_IN_MIN_BUFFER:
                    message = "An invalid value is in the min buffer.";
                    break;
                case StatusCodes.PICO_SIGGEN_FREQUENCY_OUT_OF_RANGE:
                    message = "When calculating the frequency for phase conversion, the frequency is greater than that supported by the current variant.";
                    break;
                case StatusCodes.PICO_EEPROM2_CORRUPT:
                    message = "The device's EEPROM is corrupt. Contact Pico Technology support: https://www.picotech.com/tech-support.";
                    break;
                case StatusCodes.PICO_EEPROM2_FAIL:
                    message = "The EEPROM has failed.";
                    break;
                case StatusCodes.PICO_SERIAL_BUFFER_TOO_SMALL:
                    message = "The serial buffer is too small for the required information.";
                    break;
                case StatusCodes.PICO_SIGGEN_TRIGGER_AND_EXTERNAL_CLOCK_CLASH:
                    message = "The signal generator trigger and the external clock have both been set. This is not allowed.";
                    break;
                case StatusCodes.PICO_WARNING_SIGGEN_AUXIO_TRIGGER_DISABLED:
                    message = "The AUX trigger was enabled and the external clock has been enabled, so the AUX has been automatically disabled.";
                    break;
                case StatusCodes.PICO_SIGGEN_GATING_AUXIO_NOT_AVAILABLE:
                    message = "The AUX I/O was set as a scope trigger and is now being set as a signal generator gating trigger. This is not allowed.";
                    break;
                case StatusCodes.PICO_SIGGEN_GATING_AUXIO_ENABLED:
                    message = "The AUX I/O was set by the signal generator as a gating trigger and is now being set as a scope trigger. This is not allowed.";
                    break;
                case StatusCodes.PICO_RESOURCE_ERROR:
                    message = "A resource has failed to initialize.";
                    break;
                case StatusCodes.PICO_TEMPERATURE_TYPE_INVALID:
                    message = "The temperature type is out of range.";
                    break;
                case StatusCodes.PICO_TEMPERATURE_TYPE_NOT_SUPPORTED:
                    message = "A requested temperature type is not supported on this device.";
                    break;
                case StatusCodes.PICO_TIMEOUT:
                    message = "A read/write to the device has timed out.";
                    break;
                case StatusCodes.PICO_DEVICE_NOT_FUNCTIONING:
                    message = "The device cannot be connected correctly.";
                    break;
                case StatusCodes.PICO_INTERNAL_ERROR:
                    message = "The driver has experienced an unknown error and is unable to recover from this error.";
                    break;
                case StatusCodes.PICO_MULTIPLE_DEVICES_FOUND:
                    message = "Used when opening units via IP and more than multiple units have the same ip address.";
                    break;
                case StatusCodes.PICO_CAL_PINS_STATES:
                    message = "The calibration pin states argument is out of range.";
                    break;
                case StatusCodes.PICO_CAL_PINS_FREQUENCY:
                    message = "The calibration pin frequency argument is out of range.";
                    break;
                case StatusCodes.PICO_CAL_PINS_AMPLITUDE:
                    message = "The calibration pin amplitude argument is out of range.";
                    break;
                case StatusCodes.PICO_CAL_PINS_WAVETYPE:
                    message = "The calibration pin wavetype argument is out of range.";
                    break;
                case StatusCodes.PICO_CAL_PINS_OFFSET:
                    message = "The calibration pin offset argument is out of range.";
                    break;
                case StatusCodes.PICO_PROBE_FAULT:
                    message = "The probe's identity has a problem.";
                    break;
                case StatusCodes.PICO_PROBE_IDENTITY_UNKNOWN:
                    message = "The probe has not been identified.";
                    break;
                case StatusCodes.PICO_PROBE_POWER_DC_POWER_SUPPLY_REQUIRED:
                    message = "Enabling the probe would cause the device to exceed the allowable current limit.";
                    break;
                case StatusCodes.PICO_PROBE_NOT_POWERED_WITH_DC_POWER_SUPPLY:
                    message = "The DC power supply is connected; enabling the probe would cause the device to exceed the allowable current limit.";
                    break;
                case StatusCodes.PICO_PROBE_CONFIG_FAILURE:
                    message = "Failed to complete probe configuration.";
                    break;
                case StatusCodes.PICO_PROBE_INTERACTION_CALLBACK:
                    message = "Failed to set the callback function, as currently in current callback function.";
                    break;
                case StatusCodes.PICO_UNKNOWN_INTELLIGENT_PROBE:
                    message = "The probe has been verified but not know on this driver.";
                    break;
                case StatusCodes.PICO_INTELLIGENT_PROBE_CORRUPT:
                    message = "The intelligent probe cannot be verified.";
                    break;
                case StatusCodes.PICO_PROBE_COLLECTION_NOT_STARTED:
                    message = "The callback is null, probe collection will only start when first callback is a none null pointer";
                    break;
                case StatusCodes.PICO_PROBE_POWER_CONSUMPTION_EXCEEDED:
                    message = "The current drawn by the probe(s) has exceeded the allowed limit.";
                    break;
                case StatusCodes.PICO_WARNING_PROBE_CHANNEL_OUT_OF_SYNC:
                    message = "The channel range limits have changed due to connecting or disconnecting a probe the channel has been enabled";
                    break;
                case StatusCodes.PICO_DEVICE_TIME_STAMP_RESET:
                    message = "The time stamp per waveform segment has been reset.";
                    break;
                case StatusCodes.PICO_WATCHDOGTIMER:
                    message = "An internal error has occurred and a watchdog timer has been called.";
                    break;
                case StatusCodes.PICO_IPP_NOT_FOUND:
                    message = "The picoipp.dll has not been found.";
                    break;
                case StatusCodes.PICO_IPP_NO_FUNCTION:
                    message = "A function in the picoipp.dll does not exist.";
                    break;
                case StatusCodes.PICO_IPP_ERROR:
                    message = "The Pico IPP call has failed.";
                    break;
                case StatusCodes.PICO_SHADOW_CAL_NOT_AVAILABLE:
                    message = "Shadow calibration is not available on this device.";
                    break;
                case StatusCodes.PICO_SHADOW_CAL_DISABLED:
                    message = "Shadow calibration is currently disabled.";
                    break;
                case StatusCodes.PICO_SHADOW_CAL_ERROR:
                    message = "Shadow calibration error has occurred.";
                    break;
                case StatusCodes.PICO_SHADOW_CAL_CORRUPT:
                    message = "The shadow calibration is corrupt.";
                    break;
                case StatusCodes.PICO_DEVICE_MEMORY_OVERFLOW:
                    message = "The memory onboard the device has overflowed.";
                    break;
                case StatusCodes.PICO_RESERVED_1:
                    message = "Reserved.";
                    break;
                default:
                    message = $"ErrorCode: {status}";
                    break;
            }

            return message;
        }
    }
}
