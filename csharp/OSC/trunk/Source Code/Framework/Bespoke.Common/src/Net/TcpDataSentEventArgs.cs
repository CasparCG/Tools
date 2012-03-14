using System;

namespace Bespoke.Common.Net
{
	/// <summary>
	/// 
	/// </summary>
	public class TcpDataSentEventArgs : EventArgs
	{
		/// <summary>
		/// 
		/// </summary>
		public TcpConnection Connection
		{
			get
			{
				return mConnection;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public object Data
		{
			get
			{
				return mData;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="connection"></param>
		/// <param name="data"></param>
		public TcpDataSentEventArgs(TcpConnection connection, object data)
		{
			mConnection = connection;
			mData = data;
		}

		private TcpConnection mConnection;
		private object mData;
	}
}
