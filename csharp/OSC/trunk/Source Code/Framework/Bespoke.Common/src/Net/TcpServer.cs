using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Text;
using Bespoke.Common;
using System.Linq;

namespace Bespoke.Common.Net
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void TcpConnectedHandler(object sender, TcpConnectedEventArgs e);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void TcpDataSentHandler(object sender, TcpDataSentEventArgs e);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void TcpDataReceivedHandler(object sender, TcpDataReceivedEventArgs e);

    /// <summary>
    /// 
    /// </summary>
    public class TcpServer : IDisposable
    {
        #region Events

        /// <summary>
        /// 
        /// </summary>
        public event TcpConnectedHandler Connected;

        /// <summary>
        /// 
        /// </summary>
        public event TcpConnectedHandler Disconnected;

        /// <summary>
        /// 
        /// </summary>
        public event TcpDataReceivedHandler DataReceived;

        /// <summary>
        /// 
        /// </summary>
        public event TcpDataSentHandler DataSent;

        #endregion

        /// <summary>
        /// 
        /// </summary>
        public int Port
        {
            get
            {
                return mPort;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public IPAddress IPAddress
        {
            get
            {
                return mIPAddress;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int ActiveConnectionCount
        {
            get
            {
                return mClientConnections.Count;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public ReadOnlyCollection<TcpConnection> ActiveConnections
        {
            get
            {
                return mClientConnections.AsReadOnly();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool ReceiveDataInline
        {
            get
            {
                return mReceiveDataInline;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool UseSynchronousCommunication
        {
            get
            {
                return mUseSynchronousCommunication;
            }
        }

        /// <summary>
        /// Gets or sets the expected endianness of integral value types.
        /// </summary>
        public bool LittleEndianByteOrder
        {
            get
            {
                return mLittleEndianByteOrder;
            }
            set
            {
                mLittleEndianByteOrder = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="port"></param>
        public TcpServer(int port)
            : this(IPAddress.Loopback, port, false, false, true)
        {
        }

        /// <summary>
        /// Binds the server to the loopback address.
        /// </summary>
        /// <param name="port"></param>
        /// <param name="receiveDataInline"></param>
        /// <param name="useSynchronousCommunication"></param>
        public TcpServer(int port, bool receiveDataInline, bool useSynchronousCommunication)
            : this(IPAddress.Loopback, port, receiveDataInline, useSynchronousCommunication, true)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <param name="port"></param>
        /// <param name="receiveDataInline"></param>
        /// <param name="useSynchronousCommunication"></param>
        public TcpServer(IPAddress ipAddress, int port, bool receiveDataInline, bool useSynchronousCommunication)
            : this(ipAddress, port, receiveDataInline, useSynchronousCommunication, true)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <param name="port"></param>
        /// <param name="receiveDataInline"></param>
        /// <param name="useSynchronousCommunication"></param>
        /// <param name="littleEndianByteOrder">The expected endianness of integral value types.</param>
        public TcpServer(IPAddress ipAddress, int port, bool receiveDataInline, bool useSynchronousCommunication, bool littleEndianByteOrder)
        {
            mPort = port;
            mIPAddress = ipAddress;
            mReceiveDataInline = receiveDataInline;
            mUseSynchronousCommunication = useSynchronousCommunication;
            mClientConnections = new List<TcpConnection>();
            mConnectionsToClose = new List<TcpConnection>();
            mDataReceiveThreads = new List<Thread>();
            mIsShuttingDown = false;
            mMessageLengthData = new byte[4];
            mTotalBytesReceived = 0;            
            mListenerReady = new ManualResetEvent(false);
            mLittleEndianByteOrder = littleEndianByteOrder;
        }

#if WINDOWS
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static IPAddress[] GetLocalIPAddress()
        {
            IPAddress[] localAddresses = Dns.GetHostAddresses(Dns.GetHostName());
            if (localAddresses.Length == 0)
            {
                throw new Exception("No local IP Address address found.");
            }

            return localAddresses;
        }
#endif

        /// <summary>
        /// 
        /// </summary>
        public void Start()
        {
            TcpClient client = new TcpClient(mIPAddress.ToString(), mPort);            
            Thread workerThread = new Thread(RunWorker);
            workerThread.Start(client.Client);

            // **NAGIC**
            //TcpListener listener = null;

            //try
            //{                
            //    mIsShuttingDown = false;
            //    mAcceptingConnections = true;
            //    mReceiveData = true;

            //    listener = new TcpListener(mIPAddress, mPort);				
            //    listener.Start(MaxPendingConnections);

            //    while (mAcceptingConnections)
            //    {
            //        mListenerReady.Reset();
            //        listener.BeginAcceptSocket(OnAcceptSocket, listener);
            //        mListenerReady.WaitOne();                    
            //    }
            //}
            //finally
            //{
            //    if (listener != null)
            //    {
            //        listener.Stop();
            //    }

            //    if (mReceiveDataInline && mUseSynchronousCommunication)
            //    {
            //        lock (mDataReceiveThreads)
            //        {
            //            foreach (Thread dataReceivedThread in mDataReceiveThreads)
            //            {
            //                // Note: this doesn't function against a blocked thread.
            //                dataReceivedThread.Abort();
            //                dataReceivedThread.Join();
            //            }
            //            mDataReceiveThreads.Clear();
            //        }
            //    }

            //    lock (this)
            //    {
            //        foreach (TcpConnection connection in mClientConnections)
            //        {
            //            MarkConnectionForClose(connection);
            //        }

            //        CloseMarkedConnections();
            //    }

            //    mIsShuttingDown = true;
            //}
        }

        /// <summary>
        /// 
        /// </summary>
        public void Stop()
        {
            mAcceptingConnections = false;
            mReceiveData = false;
            mIsShuttingDown = true;
            mListenerReady.Set();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        public void SendToAllClients(MemoryStream stream)
        {
            SendToAllClients(stream.GetBuffer(), (int)stream.Length);
        }

        /// <summary>
        /// 
        /// </summary>
		/// <param name="message"></param>
        public void SendToAllClients(string message)
        {
            byte[] data = Encoding.ASCII.GetBytes(message);
            SendToAllClients(data, data.Length);
        }

        /// <summary>
        /// 
        /// </summary>
		/// <param name="value"></param>
        public void SendToAllClients(long value)
        {
            lock (this)
            {
                if (mIsShuttingDown == false)
                {
                    foreach (TcpConnection connection in mClientConnections)
                    {
                        try
                        {
                            connection.Writer.Write(value);
                            OnDataSent(new TcpDataSentEventArgs(connection, value));
                        }
                        catch
                        {
                            MarkConnectionForClose(connection);
                        }
                    }
                }

                CloseMarkedConnections();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="length"></param>
        public void SendToAllClients(byte[] data, int length)
        {
            lock (this)
            {
                if (mIsShuttingDown == false)
                {
                    foreach (TcpConnection connection in mClientConnections)
                    {
                        try
                        {
                            connection.Client.Send(data, length, SocketFlags.None);
                            OnDataSent(new TcpDataSentEventArgs(connection, data));
                        }
                        catch
                        {
                            MarkConnectionForClose(connection);
                        }
                    }
                }

                CloseMarkedConnections();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            lock (this)
            {
                if (mIsShuttingDown == false)
                {
                    Stop();
                }

                foreach (TcpConnection connection in mClientConnections)
                {
                    connection.Dispose();
                }

                mClientConnections.Clear();
                mClientConnections = null;

                mConnectionsToClose.Clear();
                mConnectionsToClose = null;

                if (mListenerReady != null)
                {
                    mListenerReady.Close();
                    mListenerReady = null;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        public void CloseConnection(TcpConnection connection)
        {
            try
            {
                connection.Dispose();

                Thread dataReceivedThread = connection.Tag as Thread;
                if (dataReceivedThread != null)
                {
                    lock (mDataReceiveThreads)
                    {
                        mDataReceiveThreads.Remove(dataReceivedThread);
                    }
                }
            }
            catch
            {
                // Igore any exceptions
            }
            finally
            {
                mClientConnections.Remove(connection);
            }

            OnDisconnected(new TcpConnectedEventArgs(connection));
        }

        #region Private Methods

        /// <summary>
        /// Raise the Connected event.
        /// </summary>
        /// <param name="e">An EventArgs object that contains the event data.</param>
        private void OnConnected(TcpConnectedEventArgs e)
        {
            if (Connected != null)
            {
                Connected(this, e);
            }
        }

        /// <summary>
        /// Raise the Disconnected event.
        /// </summary>
        /// <param name="e">An EventArgs object that contains the event data.</param>
        private void OnDisconnected(TcpConnectedEventArgs e)
        {
            if (Disconnected != null)
            {
                Disconnected(this, e);
            }
        }

        /// <summary>
        /// Raise the DataReceived event.
        /// </summary>
        /// <param name="e">An EventArgs object that contains the event data.</param>
        private void OnDataReceived(TcpDataReceivedEventArgs e)
        {
            if (DataReceived != null)
            {
                DataReceived(this, e);
            }
        }

        // **NAGIC**
        /// <summary>
        /// Asynchronous callback paired with TcpListener.BeginAcceptSocket()
        /// </summary>
        /// <param name="asyncResult"></param>
//        private void OnAcceptSocket(IAsyncResult asyncResult)
//        {
//            try
//            {
//                TcpListener listener = (TcpListener)asyncResult.AsyncState;
//                Socket workerSocket = listener.EndAcceptSocket(asyncResult);

//#if WINDOWS
//                Thread workerThread = new Thread(RunWorker);
//                workerThread.Start(workerSocket);
//#else
//            ThreadStartDelegateWrapper threadStartWrapper = new ThreadStartDelegateWrapper(new ThreadStartWrapperHandler(RunWorker), workerSocket);
//            Thread workerThread = new Thread(new ThreadStart(threadStartWrapper.Start));
//            workerThread.Start();
//#endif
//            }
//            catch (ObjectDisposedException)
//            {
//                // Consume exception
//            }
//            finally
//            {
//                mListenerReady.Set();
//            }
//        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="asyncResult"></param>
        private void OnDataReceived(IAsyncResult asyncResult)
        {
            TcpConnection connection = (TcpConnection)asyncResult.AsyncState;

            lock (this)
            {
                if (mIsShuttingDown)
                    return;
                                
                try
                {
                    int bytesReceived = connection.Client.EndReceive(asyncResult);

                    if (bytesReceived == 0)
                    {
                        // bytesReceived == 0 when the socket gets shutdown; therefore, remove the connection.
                        connection.ClearReceivedData();
                        return;
                    }

                    var bytes = new List<byte>(connection.ReceivedData); // ReceivedData creates unnecessary array copy each call. Call it as few times as possible.
                    connection.ClearReceivedData();
       
                    bytes.AddRange(connection.ReceivedDataBuffer.Take(bytesReceived));

                    while (true)
                    {
                        if (mMessageLength == int.MinValue)
                        {
                            if (bytes.Count < 4)
                                break;

                            var messageLengthData = bytes.Take(4).ToArray();

                            if (BitConverter.IsLittleEndian != mLittleEndianByteOrder)
                                messageLengthData = Library.SwapEndian(messageLengthData);
                                    
                            mMessageLength = BitConverter.ToInt32(messageLengthData, 0);
                            Assert.IsTrue(mMessageLength > 0);

                            bytes.RemoveRange(0, 4);
                        }
                        else
                        {
                            if (bytes.Count < mMessageLength)
                                break;

                            OnDataReceived(new TcpDataReceivedEventArgs(connection, bytes.Take(mMessageLength).ToArray()));

                            bytes.RemoveRange(0, mMessageLength);

                            mMessageLength = int.MinValue;
                        }
                    }

                    connection.AppendReceivedData(bytes);
                    connection.InitDataReceivedCallback(OnDataReceived);
                }
                catch
                {
                    CloseConnection(connection);
                }                
            }
        }

        /// <summary>
        /// Raise the DataSent event.
        /// </summary>
        /// <param name="e">An EventArgs object that contains the event data.</param>
        private void OnDataSent(TcpDataSentEventArgs e)
        {
            if (DataSent != null)
            {
                DataSent(this, e);
            }
        }

        /// <summary>
        /// Primary worker thread.
        /// </summary>
        /// <param name="socket"></param>
        private void RunWorker(object socket)
        {
            lock (this)
            {
                if (mIsShuttingDown == false)
                {
                    AsyncCallback dataReceivedCallback = ((mReceiveDataInline) && (mUseSynchronousCommunication == false) ? new AsyncCallback(OnDataReceived) : null);
                    TcpConnection connection = new TcpConnection((Socket)socket, dataReceivedCallback);
                    mMessageLength = int.MinValue;
                    mClientConnections.Add(connection);

                    if (mReceiveDataInline && mUseSynchronousCommunication)
                    {
#if WINDOWS
                        Thread dataReceiveThread = new Thread(SynchronousReceiveData);
                        dataReceiveThread.Name = "TcpServer ReceiveData Thread";
                        connection.Tag = dataReceiveThread;
                        mDataReceiveThreads.Add(dataReceiveThread);
                        dataReceiveThread.Start(connection);
#else                   
                        ThreadStartDelegateWrapper threadStartWrapper = new ThreadStartDelegateWrapper(new ThreadStartWrapperHandler(SynchronousReceiveData), connection);
                        Thread dataReceiveThread = new Thread(threadStartWrapper.Start);
                        dataReceiveThread.Name = "TcpServer ReceiveData Thread";
                        connection.Tag = dataReceiveThread;
                        mDataReceiveThreads.Add(dataReceiveThread);
                        dataReceiveThread.Start();
#endif
                    }

                    OnConnected(new TcpConnectedEventArgs(connection));
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        private void MarkConnectionForClose(TcpConnection connection)
        {
            mConnectionsToClose.Add(connection);
        }

        /// <summary>
        /// 
        /// </summary>
        private void CloseMarkedConnections()
        {
            foreach (TcpConnection connection in mConnectionsToClose)
            {
                CloseConnection(connection);
            }

            mConnectionsToClose.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tcpConnection"></param>
        private void SynchronousReceiveData(object tcpConnection)
        {
            TcpConnection connection = (TcpConnection)tcpConnection;

            while (mReceiveData)
            {
                try
                {
                    byte[] data = connection.Reader.ReadBytes(4);
                    if (BitConverter.IsLittleEndian != mLittleEndianByteOrder)
                    {
                        data = Library.SwapEndian(data);
                    }
                    int length = BitConverter.ToInt32(data, 0);

                    data = connection.Reader.ReadBytes(length);
                    OnDataReceived(new TcpDataReceivedEventArgs(connection, data));
                }
                catch
                {
                    CloseConnection(connection);
                    break;
                }
            }
        }

        #endregion

		/// <summary>
		/// 
		/// </summary>
        public static readonly int MaxPendingConnections = 3;

        private IPAddress mIPAddress;
        private int mPort;
        private List<TcpConnection> mClientConnections;
        private List<TcpConnection> mConnectionsToClose;
        private int mMessageLength;
        private byte[] mMessageLengthData;
        private int mTotalBytesReceived;
        private bool mReceiveDataInline;
        private bool mUseSynchronousCommunication;
        private List<Thread> mDataReceiveThreads;
        private volatile bool mIsShuttingDown;
        private volatile bool mAcceptingConnections;
        private volatile bool mReceiveData;
        private ManualResetEvent mListenerReady;
        private bool mLittleEndianByteOrder;
    }
}