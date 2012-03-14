using System;
using System.Drawing;

namespace Bespoke.Common.Video
{
    /// <summary>
    /// 
    /// </summary>
    public class FrameReceivedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the received frame.
        /// </summary>
        public Bitmap Frame
        {
            get
            {
                return mFrame;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="frame"></param>
        public FrameReceivedEventArgs(Bitmap frame)
        {
            Assert.ParamIsNotNull("frame", frame);

            mFrame = frame;
        }

        private Bitmap mFrame;
    }
}
