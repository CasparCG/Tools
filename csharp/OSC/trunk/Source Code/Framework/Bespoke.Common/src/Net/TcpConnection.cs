using System;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Collections.Generic;

namespace Bespoke.Common.Net
{
	/// <summary>
	/// 
	/// </summary>
	public class TcpConnection : IDisposable
	{
		/// <summary>
		/// 
		/// </summary>
		public static readonly int ReceivedDataBufferSize = 65535;

		/// <summary>
		/// 
		/// </summary>
		public Socket Client
		{
			get
			{
				return mClient;
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		public BinaryReader Reader
		{
			get
			{
				return mReader;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public BinaryWriter Writer
		{
			get
			{
				return mWriter;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public byte[] ReceivedDataBuffer
		{
			get
			{
				return mReceivedDataBuffer;
			}
		}

        /// <summary>
        /// 
        /// </summary>
        public byte[] ReceivedData
        {
            get
            {
                return mReceivedData.ToArray();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public object Tag
        {
            get
            {
                return mTag;
            }
            set
            {
                mTag = value;
            }
        }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="client"></param>
		/// <param name="dataReceivedCallback"></param>
		public TcpConnection(Socket client, AsyncCallback dataReceivedCallback)
		{
			if (client == null)
			{
				throw new ArgumentNullException("client");
			}

			mClient = client;
			mNetworkStream = new NetworkStream(client);
			mReader = new BinaryReader(mNetworkStream);
			mWriter = new BinaryWriter(mNetworkStream);
			mReceivedDataBuffer = new byte[ReceivedDataBufferSize];
            mReceivedData = new List<byte>();
			InitDataReceivedCallback(dataReceivedCallback);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="dataReceivedCallback"></param>
		public void InitDataReceivedCallback(AsyncCallback dataReceivedCallback)
		{
			if (dataReceivedCallback != null)
			{
				mDataReceivedCallback = dataReceivedCallback;
				mClient.BeginReceive(mReceivedDataBuffer, 0, mReceivedDataBuffer.Length, SocketFlags.None, mDataReceivedCallback, this);
			}
			else
			{
				mDataReceivedCallback = null;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public void Dispose()
		{
			if (mIsClosed == false)
			{
				mReader.Close();
				mWriter.Close();
				mNetworkStream.Close();
                if (mClient.Connected)
                {
                    mClient.Shutdown(SocketShutdown.Both);
                    mClient.Close();
                }
				
				mDataReceivedCallback = null;
				mIsClosed = true;
			}
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public void AppendReceivedData(IEnumerable<byte> data)
        {
            mReceivedData.AddRange(data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public void AppendReceivedData(ArraySegment<byte> data)
        {
            mReceivedData.AddRange(data.Array);
        }

        /// <summary>
        /// 
        /// </summary>
        public void ClearReceivedData()
        {
            lock (mReceivedData)
            {
                mReceivedData.Clear();
            }
        }

        private Socket mClient;
		private NetworkStream mNetworkStream;
		private BinaryReader mReader;
		private BinaryWriter mWriter;
		private AsyncCallback mDataReceivedCallback;
		private byte[] mReceivedDataBuffer;
        private List<byte> mReceivedData;
		private bool mIsClosed;
        private object mTag;
	}
}