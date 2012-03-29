using System;
using System.IO;
using CasparCG.Conformer.Core.Events;

namespace CasparCG.Conformer.Core
{
    public class EventManager
    {
        public event EventHandler<FileSystemEventArgs> TranscodingStarted = delegate { };
        public event EventHandler<FileSystemEventArgs> TranscodingFinished = delegate { };

        public event EventHandler<TranscodingChangedEventArgs> TranscodingChanged = delegate { };

        private static EventManager Manager { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EventManager"/> class.
        /// </summary>
        private EventManager()
        {
        }

        /// <summary>
        /// Gets the instance.
        /// </summary>
        public static EventManager Instance
        {
            get
            {
                if (EventManager.Manager == null)
                    EventManager.Manager = new EventManager();

                return EventManager.Manager;
            }
        }

        /// <summary>
        /// Fires the transcoding changed event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="CasparCG.Conformer.Core.Events.TranscodingChangedEventArgs"/> instance containing the event data.</param>
        public void FireTranscodingChangedEvent(object sender, TranscodingChangedEventArgs e)
        {
            TranscodingChanged(sender, e);
        }

        /// <summary>
        /// Fires the transcoding started event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.IO.FileSystemEventArgs"/> instance containing the event data.</param>
        public void FireTranscodingStartedEvent(object sender, FileSystemEventArgs e)
        {
            TranscodingStarted(sender, e);
        }

        /// <summary>
        /// Fires the transcoding finished event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.IO.FileSystemEventArgs"/> instance containing the event data.</param>
        public void FireTranscodingFinishedEvent(object sender, FileSystemEventArgs e)
        {
            TranscodingFinished(sender, e);
        }
    }
}
