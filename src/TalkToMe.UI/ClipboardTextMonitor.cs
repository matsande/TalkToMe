namespace TalkToMe.UI
{
    using System;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using System.Windows;
    using System.Windows.Interop;
    using TalkToMe.Core;

    /// <summary>
    /// Defines the <see cref="ClipboardTextMonitor" />
    /// </summary>
    public class ClipboardTextMonitor : IClipboardTextMonitor, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClipboardTextMonitor"/> class.
        /// </summary>
        /// <param name="window">The <see cref="Window"/></param>
        public ClipboardTextMonitor(Window window)
        {
            this.textSubject = new Subject<string>();
            this.windowHandle = SetupClipboardHook(window, WndProc);
        }

        /// <summary>
        /// Gets the ClipboardTextObservable
        /// </summary>
        public IObservable<string> ClipboardTextObservable => this.textSubject.AsObservable();

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this.textSubject.Dispose();
                }

                NativeMethods.RemoveClipboardFormatListener(this.windowHandle);

                disposedValue = true;
            }
        }

        // To detect redundant calls
        ~ClipboardTextMonitor()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(false);
        }

        private static IntPtr SetupClipboardHook(Window window, HwndSourceHook wndProc)
        {
            var source = PresentationSource.FromVisual(window) as HwndSource;
            if (source == null)
            {
                throw new ArgumentException("Window source MUST be initialized first, such as in the Window's OnSourceInitialized handler.", nameof(window));
            }

            source.AddHook(wndProc);

            // get window handle for interop
            var windowHandle = new WindowInteropHelper(window).Handle;

            // register for clipboard events
            NativeMethods.AddClipboardFormatListener(windowHandle);

            return windowHandle;
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == NativeMethods.WM_CLIPBOARDUPDATE)
            {
                OnClipboardChanged();
                handled = true;
            }

            return IntPtr.Zero;
        }

        private void OnClipboardChanged()
        {
            try
            {
                if (Clipboard.ContainsText())
                {
                    this.textSubject.OnNext(Clipboard.GetText());
                }
            }
            catch (Exception)
            {
                // TODO: Trace
            }
        }

        private readonly Subject<string> textSubject;

        private readonly IntPtr windowHandle;

        private bool disposedValue = false;
    }
}