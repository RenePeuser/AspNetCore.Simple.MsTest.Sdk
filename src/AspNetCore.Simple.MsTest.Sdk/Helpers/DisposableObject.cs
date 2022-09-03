using System;

namespace Pulse.Common.MsTest
{
    public abstract class DisposableObject : IDisposable
    {
        private bool _isDisposed;


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        ~DisposableObject()
        {
            Dispose(false);
        }


        private void Dispose(bool disposing)
        {
            if (_isDisposed)
            {
                return;
            }

            if (disposing)
            {
                DisposeManagedResources();
            }

            _isDisposed = true;
        }


        protected abstract void DisposeManagedResources();
    }
}
