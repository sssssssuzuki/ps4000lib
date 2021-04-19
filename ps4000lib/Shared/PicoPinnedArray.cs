/*****************************************************************************
 *
 * Filename: PicoPinnedArray.cs
 * 
 * Description:
 *   This file defines an object to hold an array in memory when 
 *   registering a data buffer with a driver.
 *   
 * Copyright ｩ 2017-2018 Pico Technology Ltd. See LICENSE file for terms.
 *
 ****************************************************************************/

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace PS4000Lib.Shared
{
    public sealed class PinnedArray<T> : IDisposable
    {
        private GCHandle _pinnedHandle;
        private bool _disposed;

        public PinnedArray(int arraySize) : this(new T[arraySize]) { }

        public PinnedArray(IList<T> array)
        {
            _pinnedHandle = GCHandle.Alloc(array, GCHandleType.Pinned);
        }

        ~PinnedArray()
        {
            Dispose(false);
        }

        public T[] Target => (T[])_pinnedHandle.Target;

        public static implicit operator T[](PinnedArray<T> a)
        {
            return (T[]) a?._pinnedHandle.Target;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;

            if (disposing)
            {
                // Dispose of any IDisposable members
            }

            _pinnedHandle.Free();
        }
    }
}