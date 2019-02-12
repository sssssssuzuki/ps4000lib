using PS4000Lib.Enum;
using System;

namespace PS4000Lib
{
    public class Channel
    {
        public event Action SettingUpdate;

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
        public CouplingMode Coupling
        {
            get => _coupling;
            set
            {
                _coupling = value;
                SettingUpdate?.Invoke();
            }
        }

        internal ChannelType Type;
        internal int ChannelNum;
        private bool _enabled;
        private Range _range;
        private CouplingMode _coupling;

        internal Channel(ChannelType type, string name)
        {
            Type = type;
            ChannelNum = (int)type;

            Name = name;
            _enabled = true;
            _range = Range.Range_5V;
            TriggerVoltageMV = 0;
            _coupling = CouplingMode.DC;
        }
    }
}
