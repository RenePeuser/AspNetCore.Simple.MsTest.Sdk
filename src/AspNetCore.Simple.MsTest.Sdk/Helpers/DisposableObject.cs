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

        protected virtual void Dispose(bool disposing)
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