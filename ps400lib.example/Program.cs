using System;

namespace ps400lib.example
{
    class Program
    {
        static void Main(string[] args)
        {
            var ps4000 = new PS4000();
            ps4000.Open();

            //ps4000.Timebase = 4999;
            ps4000.SamplingRateHz = 2000;
            Console.WriteLine(ps4000.Timebase);

            ps4000.BufferSize = 10;

            BlockData blockdata = ps4000.CollectBlockImmediate();
            short[][][] data = blockdata.Data; // mv

            Console.WriteLine(blockdata);
            //<Header>
            //<data...>

            BlockData.Delim = ",";
            BlockData.IgnoreHeader = true;

            Console.WriteLine(blockdata);
            //<data...>

            ps4000.ChannelB.Enabled = false;
            ps4000.ChannelA.Range = Range.Range_1V;

            Console.WriteLine(ps4000.CollectBlockImmediate());
        }
    }
}
