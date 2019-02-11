using PS4000Lib.Enum;

namespace PS4000Lib
{
    public class Channel
    {
        public string Name { get; }
        public bool Enabled { get; set; }
        public Range Range { get; set; }
        public short TriggerVoltageMV { get; set; }
        public CouplingMode Coupling { get; set; }

        internal ChannelType Type;
        internal int ChannelNum;

        internal Channel(ChannelType type, string name)
        {
            Type = type;
            ChannelNum = (int)type;

            Name = name;
            Enabled = true;
            Range = Range.Range_5V;
            TriggerVoltageMV = 0;
            Coupling = CouplingMode.DC;
        }
    }
}
