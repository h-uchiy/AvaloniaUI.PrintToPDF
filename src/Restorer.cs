using System;

namespace AvaloniaUI.PrintToPDF
{
    internal static class Restorer
    {
        public static IDisposable Create<T>(T originalValue, Action<T> restore)
        {
            return new Impl(() => restore(originalValue));
        }

        private sealed class Impl : IDisposable
        {
            private readonly Action _popper;

            public Impl(Action popper)
            {
                _popper = popper;
            }

            public void Dispose()
            {
                _popper();
            }
        }
    }
}