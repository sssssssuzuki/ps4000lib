/*
 * File: Program.cs
 * Project: ps4000lib.example
 * Created Date: 19/04/2021
 * Author: Shun Suzuki
 * -----
 * Last Modified: 19/04/2021
 * Modified By: Shun Suzuki (suzuki@hapis.k.u-tokyo.ac.jp)
 * -----
 * Copyright (c) 2021 Hapis Lab. All rights reserved.
 * 
 */

using ps4000lib.example.Sample;

namespace ps4000lib.example
{
    internal class Program
    {
        private static void Main()
        {
            BlockImmediateSample.Run();
            //BlockTriggeredSample.Run();
        }
    }
}

