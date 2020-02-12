using PS4000Lib.Enum;
using System;

namespace PS4000Lib
{
    public class Channel
    {
        internal event Action SettingUpdate;

        public string Name { get; }
        public bool Enabled
        {
            get => _enabled;
            set
            {
                _enabled = value;
                SettingUpdate?.Invoke();
            }
        }
        public Range Range
        {
            get => _range;
            set
            {
                _range = value;
                SettingUpdate?.Invoke();
            }
        }
        public short TriggerVoltageMV { get; set; }
        public int Attenuation { get; set; }
        public CouplingMode Coupling
        {
            get => _coupling;
            set
            {
                _coupling = value;
                SettingUpdate?.Invoke();
            }
        }

        public ThresholdMode TriggerMode { get; set; }
        public ThresholdDirection TriggerDirection { get; set; }

        internal ChannelType Type;
        internal int ChannelNum;
        private bool _enabled;
        private Range _range;
        private CouplingMode _coupling;

        internal Channel(ChannelType type, string name)
        {
            Type = type;
            ChannelNum = (int)type;
            Attenuation = 1;

            Name = name;
            _enabled = true;
            _range = Range.Range_5V;

            TriggerVoltageMV = 0;
            TriggerMode = ThresholdMode.Level;
            TriggerDirection = ThresholdDirection.Rising;

            _coupling = CouplingMode.DC;
        }
    }
}
