/*
 * File: Channel.cs
 * Project: ps4000lib
 * Created Date: 19/04/2021
 * Author: Shun Suzuki
 * -----
 * Last Modified: 19/04/2021
 * Modified By: Shun Suzuki (suzuki@hapis.k.u-tokyo.ac.jp)
 * -----
 * Copyright (c) 2021 Hapis Lab. All rights reserved.
 * 
 */

using PS4000Lib.Enum;
using System;
using Range = PS4000Lib.Enum.Range;

namespace PS4000Lib
{
    public class Channel
    {
        internal event Action SettingUpdate;

        public string Name { get; }
        public bool Enabled
        {
            get => _enabled;
            set
            {
                _enabled = value;
                SettingUpdate?.Invoke();
            }
        }
        public Range Range
        {
            get => _range;
            set
            {
                _range = value;
                SettingUpdate?.Invoke();
            }
        }
        public short TriggerVoltageMV { get; set; }
        public int Attenuation { get; set; }
        public CouplingMode Coupling
        {
            get => _coupling;
            set
            {
                _coupling = value;
                SettingUpdate?.Invoke();
            }
        }

        public ThresholdMode TriggerMode { get; set; }
        public ThresholdDirection TriggerDirection { get; set; }

        internal ChannelType Type;
        internal int ChannelNum;
        private bool _enabled;
        private Range _range;
        private CouplingMode _coupling;

        internal Channel(ChannelType type, string name)
        {
            Type = type;
            ChannelNum = (int)type;
            Attenuation = 1;

            Name = name;
            _enabled = true;
            _range = Range.Range5V;

            TriggerVoltageMV = 0;
            TriggerMode = ThresholdMode.Level;
            TriggerDirection = ThresholdDirection.Rising;

            _coupling = CouplingMode.DC;
        }
    }
}
