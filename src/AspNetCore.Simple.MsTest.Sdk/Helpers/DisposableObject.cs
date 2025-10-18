using System;

namespace AspNetCore.Simple.MsTest.Sdk
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

#pragma warning disable CA1063
        private void Dispose(bool disposing)
#pragma warning restore CA1063
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
