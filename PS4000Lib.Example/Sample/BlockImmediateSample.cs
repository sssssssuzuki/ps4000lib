using PS4000Lib;
using System;

namespace ps4000lib.example.Sample
{
    public class BlockImmediateSample
    {
        public static void Run()
        {
            Console.WriteLine("Collect BlockData Immediately.");

            using (var ps4000 = new PS4000())
            {
                ps4000.Open();

                ps4000.SamplingRateHz = 10_000;

                ps4000.BufferSize = 10;

                var blockdata = ps4000.CollectBlockImmediate();
                Console.WriteLine(blockdata);

                BlockData.Delimiter = ",";
                BlockData.IgnoreHeader = true;

                Console.WriteLine(blockdata);

                ps4000.ChannelB.Enabled = false;
                ps4000.BufferSize = 20;
                ps4000.SamplingRateHz = 20_000;
                ps4000.ChannelA.Range = Range.Range_20V;

                Console.WriteLine(ps4000.CollectBlockImmediate());
            }
        }
    }
}
