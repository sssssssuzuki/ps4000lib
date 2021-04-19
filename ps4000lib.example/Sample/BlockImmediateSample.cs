/*
 * File: BlockImmediateSample.cs
 * Project: Sample
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
using PS4000Lib;
using Range = PS4000Lib.Enum.Range;

namespace ps4000lib.example.Sample
{
    public class BlockImmediateSample
    {
        public static void Run()
        {
            Console.WriteLine("Collect BlockData Immediately.");

            using var ps4000 = new PS4000();
            ps4000.Open();

            ps4000.SamplingRateHz = 10_000_000;
            ps4000.BufferSize = 10_000;

            ps4000.ChannelA.Range = Range.Range5V;
            ps4000.ChannelB.Range = Range.Range5V;
            ps4000.ChannelA.Attenuation = 10;
            ps4000.ChannelB.Attenuation = 10;
            ps4000.ChannelB.Enabled = false;

            var blockData = ps4000.CollectBlockImmediate();
            Console.WriteLine("******************************************************************************");
            Console.WriteLine(blockData);

            BlockData.Delimiter = ",";
            BlockData.ShowADC = false;

            Console.WriteLine("******************************************************************************");
            Console.WriteLine(blockData);

            ps4000.ChannelB.Enabled = false;
            ps4000.BufferSize = 20;
            ps4000.SamplingRateHz = 20_000;
            ps4000.ChannelA.Range = Range.Range20V;

            Console.WriteLine("******************************************************************************");
            Console.WriteLine(ps4000.CollectBlockImmediate());
        }
    }
}
