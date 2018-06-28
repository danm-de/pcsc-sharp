using System;

namespace PCSC.Utils
{
    internal sealed class DisposeAction : IDisposable
    {
        private readonly object _sync = new object();
        private readonly Action _disposeAction;
        private bool _disposed;

        public DisposeAction(Action disposeAction) {
            _disposeAction = disposeAction;
        }

        public static IDisposable Create(Action disposeAction) => new DisposeAction(disposeAction);

        public void Dispose() {
            if (_disposed) return;
            lock (_sync) {
                if (_disposed) return;
                try {
                    _disposeAction?.Invoke();
                } finally {
                    _disposed = true;
                }
            }
        }
    }
}
