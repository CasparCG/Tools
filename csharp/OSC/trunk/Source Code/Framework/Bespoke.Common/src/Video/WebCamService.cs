using System;
using System.Diagnostics;
using System.Threading;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;

namespace Bespoke.Common.Video
{
	/// <summary>
	/// 
	/// </summary>
	public class WebCamService
	{
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void FrameReceivedHandler(object sender, FrameReceivedEventArgs e);

        /// <summary>
        /// 
        /// </summary>
        public event FrameReceivedHandler FrameReceived;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="captureDeviceNumber"></param>
        /// <param name="frameRate"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public WebCamService(int captureDeviceNumber, int frameRate, int width, int height)
        {
            mCaptureDeviceNumber = captureDeviceNumber;
            mFrameRate = frameRate;
            mWidth = width;
            mHeight = height;

            mCaptureSystem = new WebCamCapture(mCaptureDeviceNumber, mFrameRate, mWidth, mHeight);
        }

		/// <summary>
		/// 
		/// </summary>
		public void Start()
		{
			try
			{
				mRetrieveImages = true;				
				RetrieveImages();
			}
			finally
			{
				if (mCaptureSystem != null)
				{
					mCaptureSystem.Dispose();
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public void Stop()
		{
			mRetrieveImages = false;
		}

		/// <summary>
		/// 
		/// </summary>
		private void RetrieveImages()
		{
			Bitmap bitmap;
			IntPtr ptr = IntPtr.Zero;

			do
			{
				mCaptureSystem.Start();

				while (mRetrieveImages)
				{
					try
					{
						ptr = mCaptureSystem.GetBitMap();
						bitmap = new Bitmap(mCaptureSystem.Width, mCaptureSystem.Height, mCaptureSystem.Stride, PixelFormat.Format24bppRgb, ptr);

                        OnFrameReceived(bitmap);
					}
					finally
					{
						if (ptr != IntPtr.Zero)
						{
							Marshal.FreeCoTaskMem(ptr);
							ptr = IntPtr.Zero;
						}
					}
				}

				mCaptureSystem.Pause();

			} while (mRetrieveImages);
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="frame"></param>
        private void OnFrameReceived(Bitmap frame)
        {
            if (FrameReceived != null)
            {
                FrameReceived(this, new FrameReceivedEventArgs(frame));
            }
        }

		private WebCamCapture mCaptureSystem;
        private int mCaptureDeviceNumber;
        private int mFrameRate;
        private int mWidth;
        private int mHeight;

		private volatile bool mRetrieveImages;
	}
}