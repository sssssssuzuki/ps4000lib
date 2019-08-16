using PS4000Lib;
using System;

namespace ps4000lib.example
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var ps4000 = new PS4000())
            {
                ps4000.Open();

                ps4000.SamplingRateHz = 2000;
                Console.WriteLine(ps4000.SamplingRateHz);

                ps4000.BufferSize = 1024;

                var blockdata = ps4000.CollectBlockImmediate();
                Console.WriteLine(blockdata);

                BlockData.Delimiter = ",";
                BlockData.IgnoreHeader = true;

                Console.WriteLine(blockdata);

                ps4000.ChannelB.Enabled = false;
                ps4000.ChannelA.Range = Range.Range_1V;

                Console.WriteLine(ps4000.CollectBlockImmediate());
            }
        }
    }
}
