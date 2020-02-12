using PicoPinnedArray;
using System;
using System.Text;

namespace PS4000Lib
{
    public class BlockData : IDisposable
    {
        private readonly PS4000 _ps4000;
        private string _data;

        private static bool _changed;
        private static bool _ignoreHeader;
        private static string _delimiter = " ";

        internal PinnedArray<short>[] MinPinned;
        internal PinnedArray<short>[] MaxPinned;
        internal uint SampleCount;
        internal int TimeInterval;

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
                sb.AppendLine("For each of the active channels, results shown are....");
                sb.AppendLine("Time interval (ns), Maximum Aggregated value ADC Count & mV, Minimum Aggregated value ADC Count & mV");
                sb.AppendLine();

                string[] heading = new[] { "Time", "Channel", "Max ADC", "Max mV", "Min ADC", "Min mV" };
                sb.AppendFormat("{0, 10}", heading[0]);
                foreach (Channel ch in _ps4000.EnumerateChannel(false, false))
                {
                    if (ch.Enabled)
                    {
                        sb.AppendFormat("{0,10} {1,10} {2,10} {3,10} {4,10}",
                                        heading[1],
                                        heading[2],
                                        heading[3],
                                        heading[4],
                                        heading[5]);
                    }
                }

                sb.AppendLine();
            }

            // Build Body
            for (long i = 0; i < SampleCount; i++)
            {
                sb.AppendFormat("{0,10}", (i * TimeInterval));
                sb.Append(Delimiter);

                foreach (Channel ch in _ps4000.EnumerateChannel(false, false))
                {
                    if (ch.Enabled)
                    {
                        sb.Append(
                            string.Join(Delimiter,
                                string.Format("{0, 10}", ch.Name),
                                string.Format("{0, 10}", MaxPinned[ch.ChannelNum].Target[i]),
                                string.Format("{0, 10}", PS4000.ConvertADC2mV(MaxPinned[ch.ChannelNum].Target[i], ch.Range) * ch.Attenuation),
                                string.Format("{0, 10}", MinPinned[ch.ChannelNum].Target[i]),
                                string.Format("{0, 10}", PS4000.ConvertADC2mV(MinPinned[ch.ChannelNum].Target[i], ch.Range) * ch.Attenuation)));
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
