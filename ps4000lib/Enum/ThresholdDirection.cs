/*
 * File: ThresholdDirection.cs
 * Project: Enum
 * Created Date: 19/04/2021
 * Author: Shun Suzuki
 * -----
 * Last Modified: 19/04/2021
 * Modified By: Shun Suzuki (suzuki@hapis.k.u-tokyo.ac.jp)
 * -----
 * Copyright (c) 2021 Hapis Lab. All rights reserved.
 * 
 */

namespace PS4000Lib.Enum
{
    public enum ThresholdDirection
    {
        // Values for level threshold mode
        Above,
        Below,
        Rising,
        Falling,
        RisingOrFalling,

        // Values for window threshold mode
        Inside = Above,
        Outside = Below,
        Enter = Rising,
        Exit = Falling,
        EnterOrExit = RisingOrFalling,

        None = Rising
    }
}
