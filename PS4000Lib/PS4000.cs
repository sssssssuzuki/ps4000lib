using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PS4000Lib
{
    public class PS4000 : IDisposable
    {
        #region field
        private bool _disposed = false;
        #endregion

        #region property
        public Channel ChannelA { get; }
        public Channel ChannelB { get; }
        #endregion

        #region ctor
        public PS4000()
        {
            ChannelA = new Channel();
            ChannelB = new Channel();
        }

        ~PS4000()
        {
            this.Dispose(false);
        }
        #endregion

        #region control
        public void Open() { }
        public void Close() { }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                // Free any other managed objects here.
                //
            }

            // Free any unmanaged objects here.
            //
            _disposed = true;
        }
        #endregion
    }
}
