﻿using System;

namespace PS4000Lib.Example
{
    public class Program
    {
        static void Main()
        {
            var ps4000 = new PS4000();
            ps4000.Open();

            //ps4000.Timebase = 4999;
            ps4000.SamplingRateHz = 2000;
            Console.WriteLine(ps4000.Timebase);

            ps4000.BufferSize = 10;

            Console.WriteLine(ps4000.CollectBlockImmediate());

            ps4000.ChannelB.Enabled = false;
            ps4000.ChannelA.Range = Range.Range_1V;

            Console.WriteLine(ps4000.CollectBlockImmediate());
        }
    }
}
