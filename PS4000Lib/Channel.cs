namespace PS4000Lib
{
    public class Channel
    {
        public bool Enabled { get; set; }
        public Range Range { get; set; }
        public short TriggerVoltageMV { get; set; }
        public CouplingMode Coupling { get; set; }

        internal Channel()
        {
            Enabled = true;
            Range = Range.Range_5V;
            TriggerVoltageMV = 0;
            Coupling = CouplingMode.DC;
        }
    }
}
