namespace TalkToMe.Core
{
    using System;

    /// <summary>
    /// Defines the <see cref="IClipboardTextMonitor" />
    /// </summary>
    public interface IClipboardTextMonitor
    {
        /// <summary>
        /// Gets the ClipboardTextObservable
        /// </summary>
        IObservable<string> ClipboardTextObservable
        {
            get;
        }
    }
}
