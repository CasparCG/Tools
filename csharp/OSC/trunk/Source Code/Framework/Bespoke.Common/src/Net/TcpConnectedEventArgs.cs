using System;

namespace Bespoke.Common.Net
{
	/// <summary>
	/// 
	/// </summary>
	public class TcpConnectedEventArgs : EventArgs
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
        /// <param name="connection"></param>
		public TcpConnectedEventArgs(TcpConnection connection)
        {
			mConnection = connection;
        }

		private TcpConnection mConnection;
	}
}