using PicoPinnedArray;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PS4000Lib
{
    public class BlockData : IDisposable
    {
        private readonly PS4000 _ps4000;
        private string _data = null;

        private static bool _changed = true;
        private static bool _ignoreHeader = false;
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
                if (_data == null || _changed)
                {
                    _data = Format();
                    _changed = false;
                }

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
            StringBuilder sb = new StringBuilder();

            // Build Header
            if (!IgnoreHeader)
            {
                List<string> header = new List<string>();
                header.Add("Time [ns]");
                if (_showADC) header.Add("Max [ADC]");
                header.Add("Max [mV]");
                if (_ps4000.DownSampleRatio > 1)
                {
                    if (_showADC)
                        header.Add("Min [ADC]");
                    header.Add("Min [mV]");
                }

                sb.AppendFormat("{0, 12}", header[0]);
                foreach (Channel ch in _ps4000.EnumerateChannel(false, false).Where(ch => ch.Enabled))
                {
                    sb.Append(Delimiter);
                    sb.AppendJoin(
                            Delimiter,
                            header.Skip(1).Select(s => string.Format("{0,12}", $"{ch.Name} {s}"))
                        );
                }

                sb.AppendLine();
            }

            // Build Body
            for (long i = 0; i < SampleCount; i++)
            {
                sb.AppendFormat("{0,12}", (i * TimeInterval));
                foreach (Channel ch in _ps4000.EnumerateChannel(false, false).Where(ch => ch.Enabled))
                {
                    if (_showADC)
                    {
                        sb.Append(Delimiter);
                        sb.AppendFormat("{0, 12}", MaxPinned[ch.ChannelNum].Target[i]);
                    }

                    sb.Append(Delimiter);
                    sb.AppendFormat("{0, 12}", PS4000.ConvertADC2mV(MaxPinned[ch.ChannelNum].Target[i], ch.Range, ch.Attenuation));

                    if (_ps4000.DownSampleRatio > 1)
                    {
                        if (_showADC)
                        {
                            sb.Append(Delimiter);
                            sb.AppendFormat("{0, 12}", MinPinned[ch.ChannelNum].Target[i]);
                        }
                        sb.Append(Delimiter);
                        sb.AppendFormat("{0, 12}", PS4000.ConvertADC2mV(MinPinned[ch.ChannelNum].Target[i], ch.Range, ch.Attenuation));
                    }
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }

        private bool disposed = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                foreach (PinnedArray<short> p in MinPinned)
                {
                    p?.Dispose();
                }

                foreach (PinnedArray<short> p in MaxPinned)
                {
                    p?.Dispose();
                }
            }

            disposed = true;
        }
    }
}
