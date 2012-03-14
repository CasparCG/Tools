using System;
using System.Diagnostics;
using System.Threading;
using System.Runtime.InteropServices;
using DirectShowLib;

namespace Bespoke.Common.Video
{
    /// <summary>
    /// 
    /// </summary>
    public class WebCamCapture : ISampleGrabberCB, IDisposable
    {
        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public int FrameRate
        {
            get
            {
                return mFrameRate;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int Width
        {
            get
            {
                return mWidth;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int Height
        {
            get
            {
                return mHeight;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int Stride
        {
            get
            {
                return mStride;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int DroppedImaged
        {
            get
            {
                return mDroppedImages;
            }
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="deviceNumber"></param>
        /// <param name="frameRate"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public WebCamCapture(int deviceNumber, int frameRate, int width, int height)
        {
            DsDevice[] devices = DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice);

            if (deviceNumber > devices.Length - 1)
            {
                throw new ArgumentException("No video capture device found at index " + deviceNumber);
            }

            try
            {
                mFrameRate = frameRate;
                mWidth = width;
                mHeight = height;
                mDevice = devices[deviceNumber];
                InitCaptureGraph();

                mPictureReady = new ManualResetEvent(false);
                mImageCaptured = true;
                mIsRunning = false;
            }
            catch
            {
                Dispose();
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            try
            {
                if (mMediaControl != null)
                {
                    int hr = mMediaControl.Stop();
                    mIsRunning = false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            if (mGraphBuilder != null)
            {
                Marshal.ReleaseComObject(mGraphBuilder);
                mGraphBuilder = null;
            }

            if (mPictureReady != null)
            {
                mPictureReady.Close();
                mPictureReady = null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Start()
        {
            if (mIsRunning == false)
            {
                int hr = mMediaControl.Run();
                DsError.ThrowExceptionForHR(hr);

                mIsRunning = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Pause()
        {
            if (mIsRunning)
            {
                int hr = mMediaControl.Pause();
                DsError.ThrowExceptionForHR(hr);

                mIsRunning = false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IntPtr GetBitMap()
        {
            mHandle = Marshal.AllocCoTaskMem(mStride * mHeight);

            try
            {
                mPictureReady.Reset();
                mImageCaptured = false;

                Start();

                if (mPictureReady.WaitOne(5000, false) == false)
                {
                    throw new Exception("Timeout waiting to get picture");
                }
            }
            catch (Exception ex)
            {
                Marshal.FreeCoTaskMem(mHandle);
				LogManager.Write("Error retrieving camera frame: " + ex.Message);
            }

            return mHandle;
        }

        #region ISampleGrabberCB Members

        /// <summary>
        /// Unused.
        /// </summary>
        /// <param name="sampleTime"></param>
        /// <param name="pSample"></param>
        /// <returns></returns>
        public int SampleCB(double sampleTime, IMediaSample pSample)
        {
            return 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sampleTime"></param>
        /// <param name="buffer"></param>
        /// <param name="bufferLength"></param>
        /// <returns></returns>
        public int BufferCB(double sampleTime, IntPtr buffer, int bufferLength)
        {
            if (mImageCaptured == false)
            {
                if (bufferLength <= mStride * mHeight)
                {
                    CopyMemory(mHandle, buffer, mStride * mHeight);
                }
                else
                {
                    throw new Exception("Invalid buffer size.");
                }

                mImageCaptured = true;

                mPictureReady.Set();
            }
            else
            {
                mDroppedImages++;
            }
            return 0;
        }

        #endregion

        #region Private Methods

        [DllImport("Kernel32.dll", EntryPoint = "RtlMoveMemory")]
        private static extern void CopyMemory(IntPtr Destination, IntPtr Source, int Length);

        /// <summary>
        /// 
        /// </summary>
        private void InitCaptureGraph()
        {
            mGraphBuilder = (IFilterGraph2)new FilterGraph();
            mMediaControl = (IMediaControl)mGraphBuilder;

            ISampleGrabber sampleGrabber = null;
            IBaseFilter captureFilter = null;
            ICaptureGraphBuilder2 captureGraph = null;
            try
            {
                captureGraph = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
                sampleGrabber = (ISampleGrabber)new SampleGrabber();

                int hr = captureGraph.SetFiltergraph(mGraphBuilder);
                DsError.ThrowExceptionForHR(hr);

                hr = mGraphBuilder.AddSourceFilterForMoniker(mDevice.Mon, null, "Video input", out captureFilter);
                DsError.ThrowExceptionForHR(hr);

                IBaseFilter baseGrabberFilter = (IBaseFilter)sampleGrabber;
                ConfigureSampleGrabber(sampleGrabber);

                hr = mGraphBuilder.AddFilter(baseGrabberFilter, "Ds.NET Grabber");
                DsError.ThrowExceptionForHR(hr);

                if (mFrameRate + mHeight + mWidth > 0)
                {
                    InitConfigParams(captureGraph, captureFilter);
                }

                hr = captureGraph.RenderStream(PinCategory.Capture, MediaType.Video, captureFilter, null, baseGrabberFilter);
                DsError.ThrowExceptionForHR(hr);

                SaveSizeInfo(sampleGrabber);
            }
            finally
            {
                if (captureFilter != null)
                {
                    Marshal.ReleaseComObject(captureFilter);
                    captureFilter = null;
                }
                if (sampleGrabber != null)
                {
                    Marshal.ReleaseComObject(sampleGrabber);
                    sampleGrabber = null;
                }
                if (captureGraph != null)
                {
                    Marshal.ReleaseComObject(captureGraph);
                    captureGraph = null;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sampleGrabber"></param>
        private void ConfigureSampleGrabber(ISampleGrabber sampleGrabber)
        {
            AMMediaType media = new AMMediaType();
            media.majorType = MediaType.Video;
            media.subType = MediaSubType.RGB24;
            media.formatType = FormatType.VideoInfo;

            int hr = sampleGrabber.SetMediaType(media);
            DsError.ThrowExceptionForHR(hr);

            DsUtils.FreeAMMediaType(media);
            media = null;

            hr = sampleGrabber.SetCallback(this, 1);
            DsError.ThrowExceptionForHR(hr);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="captureGraph"></param>
        /// <param name="captureFilter"></param>
        private void InitConfigParams(ICaptureGraphBuilder2 captureGraph, IBaseFilter captureFilter)
        {
            object obj;
            AMMediaType media;

            int hr = captureGraph.FindInterface(PinCategory.Capture, MediaType.Video, captureFilter, typeof(IAMStreamConfig).GUID, out obj);
            IAMStreamConfig videoStreamConfig = obj as IAMStreamConfig;
            if (videoStreamConfig == null)
            {
                throw new Exception("Failed to get IAMStreamConfig");
            }

            // Get the existing format block
            hr = videoStreamConfig.GetFormat(out media);
            DsError.ThrowExceptionForHR(hr);

            // copy out the videoinfoheader
            VideoInfoHeader infoHeader = new VideoInfoHeader();
            Marshal.PtrToStructure(media.formatPtr, infoHeader);

            if (mFrameRate > 0)
            {
                infoHeader.AvgTimePerFrame = 10000000 / mFrameRate;
            }

            if (mWidth > 0)
            {
                infoHeader.BmiHeader.Width = mWidth;
            }

            if (mHeight > 0)
            {
                infoHeader.BmiHeader.Height = mHeight;
            }

            // Copy the media structure back
            Marshal.StructureToPtr(infoHeader, media.formatPtr, false);

            // Set the new format
            hr = videoStreamConfig.SetFormat(media);
            DsError.ThrowExceptionForHR(hr);

            DsUtils.FreeAMMediaType(media);
            media = null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sampleGrabber"></param>
        private void SaveSizeInfo(ISampleGrabber sampleGrabber)
        {
            // Get the media type from the SampleGrabber
            AMMediaType media = new AMMediaType();
            int hr = sampleGrabber.GetConnectedMediaType(media);
            DsError.ThrowExceptionForHR(hr);

            if ((media.formatType != FormatType.VideoInfo) || (media.formatPtr == IntPtr.Zero))
            {
                throw new NotSupportedException("Unknown Grabber Media Format");
            }

            // Grab the size info
            VideoInfoHeader infoHeader = (VideoInfoHeader)Marshal.PtrToStructure(media.formatPtr, typeof(VideoInfoHeader));
            mWidth = infoHeader.BmiHeader.Width;
            mHeight = infoHeader.BmiHeader.Height;
            mStride = mWidth * (infoHeader.BmiHeader.BitCount / 8);

            DsUtils.FreeAMMediaType(media);
            media = null;
        }

        #endregion

        private DsDevice mDevice;
        private IFilterGraph2 mGraphBuilder;
        private IMediaControl mMediaControl;
        private ManualResetEvent mPictureReady;
        private volatile bool mImageCaptured;
        private bool mIsRunning;
        private IntPtr mHandle;
        private int mFrameRate;
        private int mWidth;
        private int mHeight;
        private int mStride;
        private int mDroppedImages;
    }
}