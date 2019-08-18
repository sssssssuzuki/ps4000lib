using PS4000Lib;
using System;

namespace ps4000lib.example.Sample
{
    public class BlockTriggeredSample
    {
        public static void Run()
        {
            Console.WriteLine("Collect BlockData Triggered.");

            using (var ps4000 = new PS4000())
            {
                ps4000.Open();

                ps4000.ChannelA.Range = Range.Range_5V;
                ps4000.ChannelB.Enabled = false;
                ps4000.SamplingRateHz = 10_000;
                ps4000.BufferSize = 10;

                BlockData.Delimiter = ",";
                BlockData.IgnoreHeader = true;

                ps4000.ChannelA.TriggerVoltageMV = 500;
                ps4000.ChannelA.TriggerMode = ThresholdMode.Level;
                ps4000.ChannelA.TriggerDirection = ThresholdDirection.Rising;
                ps4000.AddTriggerConditions(new TriggerConditions
                {
                    ChannelA = TriggerState.True,
                });

                var blockdata = ps4000.CollectBlockTriggered();
                Console.WriteLine(blockdata);
            }
        }
    }
}
