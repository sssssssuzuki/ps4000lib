using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PS4000Lib;

namespace PS4000Lib.Example
{
    public class Program
    {
        static void Main()
        {
            var bt = new BlockTest();
            bt.Run();
        }
    }

    public class BlockTest
    {
        public BlockTest() { }

        public void Run()
        {
            var ps4000 = new PS4000();
            ps4000.Open();

            Console.WriteLine(ps4000.DeviceInfo);
            Console.WriteLine(ps4000.Settings);

            ps4000.DisableChannel(Channel.ChannelB);

            // timebase: PS4262だとサンプリング間隔 = 100 * (timebase + 1) ns になる
            // 以下だと 100 * 1000 ns = 100 us
            uint timebase = (1000 - 1);
            ps4000.SetTimebase(ref timebase, out int timeintvl);

            // PICOのバッファに貯めるだけ貯めるちなみにサイズは1024
            // なので100 us * 1024 = 100 ms分
            var res = ps4000.CollectBlockImmediate();

            Console.WriteLine(res);
            Console.ReadLine();

            // Trigger Setting
            ps4000.AddTriggerProperties(Channel.ChannelA, 2500); // 2500 mVでトリガー
            // 以下は💩仕様のせいで
            // 使わなくても指定しないとダメ
            ps4000.AddTriggerConditions(new TriggerConditions(
                    TriggerState.True, // ChannelA
                    TriggerState.DontCare, // ChannelB
                    TriggerState.DontCare, // ChannelC
                    TriggerState.DontCare, // ChannelD
                    TriggerState.DontCare, // Channel ext
                    TriggerState.DontCare, // Channel aux
                    TriggerState.DontCare  // Channel pwq 
                ));
            ps4000.AddTriggerThresholdDirection(ThresholdDirection.Rising); // ChannnelA
            ps4000.AddTriggerThresholdDirection(ThresholdDirection.None); // ChannnelB
            ps4000.AddTriggerThresholdDirection(ThresholdDirection.None); // ChannnelC
            ps4000.AddTriggerThresholdDirection(ThresholdDirection.None); // ChannnelD
            ps4000.AddTriggerThresholdDirection(ThresholdDirection.None); // Channnel ext
            ps4000.AddTriggerThresholdDirection(ThresholdDirection.None); // Channnel aux
            // この組み合わせで, 立ち上がりがTrueならトリガー

            // こっちはトリガーあり
            res = ps4000.CollectBlockTriggered();

            Console.WriteLine(res);
            Console.ReadLine();

            // こっちはBuffer*num分のデータ取ってくる
            // ただし生データで
            ushort num = 10;
            var res2 = ps4000.CollectBlockRapid(num);

            for (int i = 0; i < num; i++)
            {
                for (int ch = 0; ch < res2[i].Length; ch++)
                {
                    if (res2[i][ch] == null) continue;
                    Console.WriteLine(string.Join("\n", res2[i][ch]));
                }
            }

            Console.ReadLine();
        }
    }
}
