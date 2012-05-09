/*
* Copyright (c) 2012 Robert Nagy and Thomas III Kaltz
*
* This file is part of CasparRx Framework.
*
* CasparRx is free software: you can redistribute it and/or modify
* it under the terms of the GNU General Public License as published by
* the Free Software Foundation, either version 3 of the License, or
* (at your option) any later version.
*
* CasparRx is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
* GNU General Public License for more details.
*
* You should have received a copy of the GNU General Public License
* along with CasparRX. If not, see <http://www.gnu.org/licenses/>.
*
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Disposables;
using System.Reactive.Concurrency;
using System.IO;
using System.Net.Sockets;
using System.Reactive.Subjects;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Diagnostics;

namespace CasparRx
{
    public class Connection : IDisposable
    {
        private string host;
        private int port;

        private CompositeDisposable     disposables = new CompositeDisposable();
        private BehaviorSubject<bool>   connectedSubject = new BehaviorSubject<bool>(false);
        private TcpClient               client = null;
        private EventLoopScheduler      scheduler = new EventLoopScheduler(ts => new Thread(ts));
        private IDisposable             reconnectSubscription;

        public IObservable<bool> OnConnected
        {
            get { return this.connectedSubject.DistinctUntilChanged(); }
        }

        public Connection()
        {
            this.disposables.Add(scheduler);
        }

        public Connection(string host, int port = 5250) : this()
        {
            this.Connect(host, port);
        }

        public void Connect(string host, int port = 5250)
        {
            this.Close();

            this.host = host;
            this.port = port;

            this.Connect();

            this.reconnectSubscription = Observable
                .Interval(TimeSpan.FromSeconds(1))
                .ObserveOn(scheduler)
                .Subscribe(x => this.Connect());
        }

        public void Close()
        {
            if (this.reconnectSubscription != null)
                this.reconnectSubscription.Dispose();
            this.reconnectSubscription = null;
            this.Reset();
        }

        public void Dispose()
        {
            this.Close();
            this.disposables.Dispose();
        }

        public IEnumerable<string> Send(string cmd)
        {
            var result = this.AsyncSend(cmd);

            // Block only until we get response code and know that the command has executed.
            result.ToEnumerable()
                .FirstOrDefault();

            return result.ToEnumerable();
        }

        public IObservable<string> AsyncSend(string cmd)
        {
            var subject = new ReplaySubject<string>();

            this.scheduler.Schedule(() =>
            {
                try
                {
                    if (!this.Connect())
                        return;

                    var writer = new StreamWriter(client.GetStream()) { AutoFlush = true };
                    var reader = new StreamReader(client.GetStream());

                    writer.WriteLine(cmd);
                    var reply = ReadLine(reader);
                    
                    if (Regex.IsMatch(reply, "201.*"))
                        subject.OnNext(ReadLine(reader));
                    else if (Regex.IsMatch(reply, "200.*"))
                    {
                        while (reply != string.Empty)
                        {
                            reply = ReadLine(reader);
                            subject.OnNext(reply);
                        }
                    }
                    else if (Regex.IsMatch(reply, "400.*"))
                        throw new Exception("Command not understood.");
                    else if (Regex.IsMatch(reply, "401.*"))
                        throw new Exception("Illegal Command.");
                    else if (Regex.IsMatch(reply, "402.*"))
                        throw new Exception("Parameter missing.");
                    else if (Regex.IsMatch(reply, "403.*"))
                        throw new Exception("Illegal parameter.");
                    else if (Regex.IsMatch(reply, "404.*"))
                        throw new Exception("Media file not found.");
                    else if (Regex.IsMatch(reply, "500.*"))
                        throw new Exception("Internal server error.");
                    else if (Regex.IsMatch(reply, "501.*"))
                        throw new Exception("Internal server error.");
                    else if (Regex.IsMatch(reply, "502.*"))
                        throw new Exception("Media file unreadable.");
                }
                catch (IOException ex)
                {
                    this.Reset();
                    subject.OnError(ex);
                }
                catch(ObjectDisposedException ex)
                {
                    this.Reset();
                    subject.OnError(ex);
                }
                catch (Exception ex)
                {
                    subject.OnError(ex);
                }

                subject.OnCompleted();
            });

            subject
                .Timeout(TimeSpan.FromSeconds(5))
                .Subscribe(x => { }, ex => this.Reset());

            return subject;
        }

        private void Reset()
        {
            if (this.client != null)
                this.client.Close();
            this.client = null;
            this.connectedSubject.OnNext(false);
        }

        private bool Connect()
        {
            try
            {
                if (!this.IsConnected)
                {
                    this.Reset();
                    this.client = new TcpClient(this.host, this.port);
                }
            }
            catch
            {
                this.Reset();
            }

            this.connectedSubject.OnNext(this.IsConnected);
            return this.IsConnected;
        }

        private bool IsConnected
        {
            get
            {
                if (this.client == null || !this.client.Connected)
                    return false;

                bool blockingState = this.client.Client.Blocking;
                try
                {
                    byte[] tmp = new byte[1];
                    this.client.Client.Blocking = false;
                    this.client.Client.Send(tmp, 0, 0);
                    return true;
                }
                catch (SocketException e)
                {
                    // 10035 == WSAEWOULDBLOCK
                    if (e.NativeErrorCode.Equals(10035))
                        return true;
                    else
                    {
                        this.Reset();
                        return false;
                    }
                }
                finally
                {
                    this.client.Client.Blocking = blockingState;
                }           
            }
        }

        private string ReadLine(StreamReader reader)
        {
            var str = new StringBuilder();
            while (true)
            {
                var c = reader.Read();
                if (c == -1)
                    throw new IOException();

                str.Append((char)c);

                if (str.Length >= 2 && str[str.Length - 2] == '\r' && str[str.Length - 1] == '\n')
                    break;
            }

            return str.ToString(0, str.Length - 2);
        }
    }
}
