/*
 * File: BlockData.cs
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PS4000Lib.Shared;

namespace PS4000Lib
{
    public class BlockData : IDisposable
    {
        private readonly PS4000 _ps4000;
        private string _data;

        private static bool _changed = true;
        private static bool _ignoreHeader;
        private static string _delimiter = " ";

        internal PinnedArray<short>[] MinPinned;
        internal PinnedArray<short>[] MaxPinned;
        internal uint SampleCount;
        internal int TimeInterval;

        private static bool _showADC = true;

        public string Data
        {
            get
            {
                if (_data != null && !_changed) return _data;
                _data = Format();
                _changed = false;

                return _data;
            }
        }

        public static bool IgnoreHeader
        {
            get => _ignoreHeader;
            set
            {
                _changed = true;
                _ignoreHeader = value;
            }
        }

        public static string Delimiter
        {
            get => _delimiter;
            set
            {
                _changed = true;
                _delimiter = value;
            }
        }

        public static bool ShowADC
        {
            get => _showADC;
            set
            {
                _changed = true;
                _showADC = value;
            }
        }

        internal BlockData(PS4000 ps4000)
        {
            _ps4000 = ps4000;
        }

        public override string ToString()
        {
            return Data;
        }

        private string Format()
        {
            var sb = new StringBuilder();

            // Build Header
            if (!IgnoreHeader)
            {
                var header = new List<string> { "Time [ns]" };
                if (_showADC) header.Add("Max [ADC]");
                header.Add("Max [mV]");
                if (_ps4000.DownSampleRatio > 1)
                {
                    if (_showADC)
                        header.Add("Min [ADC]");
                    header.Add("Min [mV]");
                }

                sb.AppendFormat("{0, 12}", header[0]);
                foreach (var ch in _ps4000.EnumerateChannel(false, false).Where(ch => ch.Enabled))
                {
                    sb.Append(Delimiter);
                    sb.AppendJoin(
                            Delimiter,
                            header.Skip(1).Select(s => $"{$"{ch.Name} {s}",12}")
                        );
                }

                sb.AppendLine();
            }

            // Build Body
            for (long i = 0; i < SampleCount; i++)
            {
                sb.AppendFormat("{0,12}", i * TimeInterval);
                foreach (var ch in _ps4000.EnumerateChannel(false, false).Where(ch => ch.Enabled))
                {
                    if (_showADC)
                    {
                        sb.Append(Delimiter);
                        sb.AppendFormat("{0, 12}", MaxPinned[ch.ChannelNum].Target[i]);
                    }

                    sb.Append(Delimiter);
                    sb.AppendFormat("{0, 12}", PS4000.ConvertADC2MV(MaxPinned[ch.ChannelNum].Target[i], ch.Range, ch.Attenuation));

                    if (_ps4000.DownSampleRatio <= 1) continue;
                    if (_showADC)
                    {
                        sb.Append(Delimiter);
                        sb.AppendFormat("{0, 12}", MinPinned[ch.ChannelNum].Target[i]);
                    }
                    sb.Append(Delimiter);
                    sb.AppendFormat("{0, 12}", PS4000.ConvertADC2MV(MinPinned[ch.ChannelNum].Target[i], ch.Range, ch.Attenuation));
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }

        private bool _disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                foreach (var p in MinPinned)
                {
                    p?.Dispose();
                }

                foreach (var p in MaxPinned)
                {
                    p?.Dispose();
                }
            }

            _disposed = true;
        }
    }
}
