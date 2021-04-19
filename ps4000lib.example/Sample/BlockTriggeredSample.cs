/*
 * File: BlockTriggeredSample.cs
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

using PS4000Lib;
using System;
using System.Threading.Tasks;

namespace ps4000lib.example.Sample
{
    public class BlockTriggeredSample
    {
        public static void Run()
        {
            Console.WriteLine("Collect BlockData Triggered.");

            using (PS4000 ps4000 = new PS4000())
            {
                ps4000.Open();

                ps4000.ChannelA.Range = PS4000Lib.Range.Range_5V;
                ps4000.ChannelA.Attenuation = 10;

                ps4000.ChannelB.Enabled = false;

                ps4000.SamplingRateHz = 10_000;
                ps4000.BufferSize = 10;

                BlockData.Delimiter = ",";
                BlockData.IgnoreHeader = true;

                ps4000.ChannelA.TriggerVoltageMV = 1000;
                ps4000.ChannelA.TriggerMode = ThresholdMode.Level;
                ps4000.ChannelA.TriggerDirection = ThresholdDirection.Rising;
                ps4000.AddTriggerConditions(new TriggerConditions
                {
                    ChannelA = TriggerState.True,
                });

                BlockData blockdata = ps4000.CollectBlockTriggered();
                Console.WriteLine(blockdata);
            }
        }
    }
}
