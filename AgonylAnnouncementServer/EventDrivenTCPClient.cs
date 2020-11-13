﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace AgonylAnnouncementServer
{
    public class EventDrivenTCPClient : IDisposable
    {
        #region Consts/Default values

        private const int DEFAULTTIMEOUT = 5000; //Default to 5 seconds on all timeouts
        private const int RECONNECTINTERVAL = 2000; //Default to 2 seconds reconnect attempt rate

        #endregion Consts/Default values

        #region Components, Events, Delegates, and CTOR

        //Timer used to detect receive timeouts
        private System.Timers.Timer tmrReceiveTimeout = new System.Timers.Timer();

        private System.Timers.Timer tmrSendTimeout = new System.Timers.Timer();
        private System.Timers.Timer tmrConnectTimeout = new System.Timers.Timer();

        public delegate void delDataReceived(EventDrivenTCPClient sender, object data);

        public event delDataReceived DataReceived;

        public delegate void delConnectionStatusChanged(EventDrivenTCPClient sender, ConnectionStatus status);

        public event delConnectionStatusChanged ConnectionStatusChanged;

        public enum ConnectionStatus
        {
            NeverConnected,
            Connecting,
            Connected,
            AutoReconnecting,
            DisconnectedByUser,
            DisconnectedByHost,
            ConnectFail_Timeout,
            ReceiveFail_Timeout,
            SendFail_Timeout,
            SendFail_NotConnected,
            Error
        }

        public EventDrivenTCPClient(IPAddress ip, int port, byte id = 200, bool autoreconnect = true)
        {
            this._ID = id;
            this._IP = ip;
            this._Port = port;
            this._AutoReconnect = autoreconnect;
            this._client = new TcpClient(AddressFamily.InterNetwork);
            this._client.NoDelay = true; //Disable the nagel algorithm for simplicity
            ReceiveTimeout = DEFAULTTIMEOUT;
            SendTimeout = DEFAULTTIMEOUT;
            ConnectTimeout = DEFAULTTIMEOUT;
            ReconnectInterval = RECONNECTINTERVAL;
            tmrReceiveTimeout.AutoReset = false;
            tmrReceiveTimeout.Elapsed += new System.Timers.ElapsedEventHandler(tmrReceiveTimeout_Elapsed);
            tmrConnectTimeout.AutoReset = false;
            tmrConnectTimeout.Elapsed += new System.Timers.ElapsedEventHandler(tmrConnectTimeout_Elapsed);
            tmrSendTimeout.AutoReset = false;
            tmrSendTimeout.Elapsed += new System.Timers.ElapsedEventHandler(tmrSendTimeout_Elapsed);

            ConnectionState = ConnectionStatus.NeverConnected;
        }

        #endregion Components, Events, Delegates, and CTOR

        #region Private methods/Event Handlers

        private void tmrSendTimeout_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            this.ConnectionState = ConnectionStatus.SendFail_Timeout;
            DisconnectByHost();
        }

        private void tmrReceiveTimeout_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            this.ConnectionState = ConnectionStatus.ReceiveFail_Timeout;
            DisconnectByHost();
        }

        private void tmrConnectTimeout_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            ConnectionState = ConnectionStatus.ConnectFail_Timeout;
            DisconnectByHost();
        }

        private void DisconnectByHost()
        {
            this.ConnectionState = ConnectionStatus.DisconnectedByHost;
            tmrReceiveTimeout.Stop();
            if (AutoReconnect)
            {
                Reconnect();
            }
        }

        private void Reconnect()
        {
            if (this.ConnectionState == ConnectionStatus.Connected)
            {
                return;
            }

            this.ConnectionState = ConnectionStatus.AutoReconnecting;
            try
            {
                if (this._client.Connected)
                    this._client.Client.BeginDisconnect(true, new AsyncCallback(cbDisconnectByHostComplete), this._client.Client);
            }
            catch { }
        }

        #endregion Private methods/Event Handlers

        #region Public Methods

        /// <summary>
        /// Try connecting to the remote host
        /// </summary>
        public void Connect()
        {
            if (this.ConnectionState == ConnectionStatus.Connected)
            {
                return;
            }

            this.ConnectionState = ConnectionStatus.Connecting;
            tmrConnectTimeout.Start();
            this._client.BeginConnect(this._IP, this._Port, new AsyncCallback(cbConnect), this._client.Client);
        }

        /// <summary>
        /// Try disconnecting from the remote host
        /// </summary>
        public void Disconnect()
        {
            if (this.ConnectionState != ConnectionStatus.Connected)
            {
                return;
            }

            this._client.Client.BeginDisconnect(true, new AsyncCallback(cbDisconnectComplete), this._client.Client);
        }

        /// <summary>
        /// Try sending a string to the remote host
        /// </summary>
        /// <param name="data">The data to send</param>
        public void Send(string data)
        {
            if (this.ConnectionState != ConnectionStatus.Connected)
            {
                this.ConnectionState = ConnectionStatus.SendFail_NotConnected;
                return;
            }
            var bytes = _encode.GetBytes(data);
            SocketError err = new SocketError();
            tmrSendTimeout.Start();
            this._client.Client.BeginSend(bytes, 0, bytes.Length, SocketFlags.None, out err, new AsyncCallback(cbSendComplete), this._client.Client);
            if (err != SocketError.Success)
            {
                Action doDCHost = new Action(DisconnectByHost);
                doDCHost.Invoke();
            }
        }

        /// <summary>
        /// Try sending byte data to the remote host
        /// </summary>
        /// <param name="data">The data to send</param>
        public void Send(byte[] data)
        {
            if (this.ConnectionState != ConnectionStatus.Connected)
            {
                return;
            }
            //throw new InvalidOperationException("Cannot send data, socket is not connected");
            SocketError err = new SocketError();
            this._client.Client.BeginSend(data, 0, data.Length, SocketFlags.None, out err, new AsyncCallback(cbSendComplete), this._client.Client);
            if (err != SocketError.Success)
            {
                Action doDCHost = new Action(DisconnectByHost);
                doDCHost.Invoke();
            }
        }

        public void Dispose()
        {
            this._client.Close();
            this._client.Client.Dispose();
            this.tmrReceiveTimeout.Dispose();
            this.tmrSendTimeout.Dispose();
            this.tmrConnectTimeout.Dispose();
        }

        #endregion Public Methods

        #region Callbacks

        private void cbConnectComplete()
        {
            if (_client.Connected == true)
            {
                tmrConnectTimeout.Stop();
                ConnectionState = ConnectionStatus.Connected;
                this._client.Client.BeginReceive(this.dataBuffer, 0, this.dataBuffer.Length, SocketFlags.None, new AsyncCallback(cbDataReceived), this._client.Client);
            }
            else
            {
                ConnectionState = ConnectionStatus.Error;
            }
        }

        private void cbDisconnectByHostComplete(IAsyncResult result)
        {
            var r = result.AsyncState as Socket;
            if (r == null)
            {
                throw new InvalidOperationException("Invalid IAsyncResult - Could not interpret as a socket object");
            }

            r.EndDisconnect(result);
            if (this.AutoReconnect)
            {
                Action doConnect = new Action(Connect);
                doConnect.Invoke();
                return;
            }
        }

        private void cbDisconnectComplete(IAsyncResult result)
        {
            var r = result.AsyncState as Socket;
            if (r == null)
            {
                throw new InvalidOperationException("Invalid IAsyncResult - Could not interpret as a socket object");
            }

            r.EndDisconnect(result);
            this.ConnectionState = ConnectionStatus.DisconnectedByUser;
        }

        private void cbConnect(IAsyncResult result)
        {
            var sock = result.AsyncState as Socket;
            if (result == null)
            {
                throw new InvalidOperationException("Invalid IAsyncResult - Could not interpret as a socket object");
            }

            if (!sock.Connected)
            {
                if (AutoReconnect)
                {
                    System.Threading.Thread.Sleep(ReconnectInterval);
                    //////////////////////////////////////
                    // Framework 4.x onwards, if BeginConnect is called asynchronously more than once it will generate exception.
                    // Close the socket which will dispose the object and create new object and connect again.
                    sock.Close();
                    this._client = new TcpClient(AddressFamily.InterNetwork);
                    this._client.NoDelay = true; //Disable the nagel algorithm for simplicity
                                                 /////////////////////////////////////
                    Action reconnect = new Action(Connect);
                    reconnect.Invoke();
                    return;
                }
                else
                {
                    return;
                }
            }
            sock.EndConnect(result);
            var callBack = new Action(cbConnectComplete);
            callBack.Invoke();
        }

        private void cbSendComplete(IAsyncResult result)
        {
            var r = result.AsyncState as Socket;
            if (r == null)
            {
                throw new InvalidOperationException("Invalid IAsyncResult - Could not interpret as a socket object");
            }

            SocketError err = new SocketError();
            r.EndSend(result, out err);
            if (err != SocketError.Success)
            {
                Action doDCHost = new Action(DisconnectByHost);
                doDCHost.Invoke();
            }
            else
            {
                lock (SyncLock)
                {
                    tmrSendTimeout.Stop();
                }
            }
        }

        private void cbChangeConnectionStateComplete(IAsyncResult result)
        {
            var r = result.AsyncState as EventDrivenTCPClient;
            if (r == null)
            {
                throw new InvalidOperationException("Invalid IAsyncResult - Could not interpret as a EDTC object");
            }

            r.ConnectionStatusChanged.EndInvoke(result);
        }

        private void cbDataReceived(IAsyncResult result)
        {
            var sock = result.AsyncState as Socket;
            if (sock == null)
            {
                throw new InvalidOperationException("Invalid IASyncResult - Could not interpret as a socket");
            }

            SocketError err = new SocketError();
            int bytes = sock.EndReceive(result, out err);
            if (bytes == 0 || err != SocketError.Success)
            {
                lock (SyncLock)
                {
                    tmrReceiveTimeout.Start();
                    return;
                }
            }
            else
            {
                lock (SyncLock)
                {
                    tmrReceiveTimeout.Stop();
                }
            }
            if (DataReceived != null)
            {
                DataReceived.BeginInvoke(this, getBytes(dataBuffer, bytes), new AsyncCallback(cbDataRecievedCallbackComplete), this);
            }
        }

        private void cbDataRecievedCallbackComplete(IAsyncResult result)
        {
            var r = result.AsyncState as EventDrivenTCPClient;
            if (r == null)
            {
                throw new InvalidOperationException("Invalid IAsyncResult - Could not interpret as EDTC object");
            }

            r.DataReceived.EndInvoke(result);
            SocketError err = new SocketError();
            this._client.Client.BeginReceive(this.dataBuffer, 0, this.dataBuffer.Length, SocketFlags.None, out err, new AsyncCallback(cbDataReceived), this._client.Client);
            if (err != SocketError.Success)
            {
                Action doDCHost = new Action(DisconnectByHost);
                doDCHost.Invoke();
            }
        }

        #endregion Callbacks

        private byte[] getBytes(byte[] Buffer, int length)
        {
            byte[] dataBuff = new byte[length];
            Array.Copy(Buffer, dataBuff, length);
            return dataBuff;
        }

        #region Properties and members

        private byte _ID = 0;
        private IPAddress _IP = IPAddress.None;
        private ConnectionStatus _ConStat;
        private TcpClient _client;
        private byte[] dataBuffer = new byte[5000];
        private bool _AutoReconnect = false;
        private int _Port = 0;
        private Encoding _encode = Encoding.Default;
        private object _SyncLock = new object();

        /// <summary>
        /// Syncronizing object for asyncronous operations
        /// </summary>
        public object SyncLock
        {
            get
            {
                return _SyncLock;
            }
        }

        /// <summary>
        /// Encoding to use for sending and receiving
        /// </summary>
        public Encoding DataEncoding
        {
            get
            {
                return _encode;
            }
            set
            {
                _encode = value;
            }
        }

        /// <summary>
        /// Current state that the connection is in
        /// </summary>
        public ConnectionStatus ConnectionState
        {
            get
            {
                return _ConStat;
            }
            private set
            {
                bool raiseEvent = value != _ConStat;
                _ConStat = value;
                if (ConnectionStatusChanged != null && raiseEvent)
                {
                    ConnectionStatusChanged.BeginInvoke(this, _ConStat, new AsyncCallback(cbChangeConnectionStateComplete), this);
                }
            }
        }

        /// <summary>
        /// True to autoreconnect at the given reconnection interval after a remote host closes the connection
        /// </summary>
        public bool AutoReconnect
        {
            get
            {
                return _AutoReconnect;
            }
            set
            {
                _AutoReconnect = value;
            }
        }

        public int ReconnectInterval { get; set; }

        /// <summary>
        /// Port to connect to on the remote host
        /// </summary>
        public byte ID
        {
            get
            {
                return _ID;
            }
        }

        /// <summary>
        /// IP of the remote host
        /// </summary>
        public IPAddress IP
        {
            get
            {
                return _IP;
            }
        }

        /// <summary>
        /// Port to connect to on the remote host
        /// </summary>
        public int Port
        {
            get
            {
                return _Port;
            }
        }

        /// <summary>
        /// Time to wait after a receive operation is attempted before a timeout event occurs
        /// </summary>
        public int ReceiveTimeout
        {
            get
            {
                return (int)tmrReceiveTimeout.Interval;
            }
            set
            {
                tmrReceiveTimeout.Interval = (double)value;
            }
        }

        /// <summary>
        /// Time to wait after a send operation is attempted before a timeout event occurs
        /// </summary>
        public int SendTimeout
        {
            get
            {
                return (int)tmrSendTimeout.Interval;
            }
            set
            {
                tmrSendTimeout.Interval = (double)value;
            }
        }

        /// <summary>
        /// Time to wait after a connection is attempted before a timeout event occurs
        /// </summary>
        public int ConnectTimeout
        {
            get
            {
                return (int)tmrConnectTimeout.Interval;
            }
            set
            {
                tmrConnectTimeout.Interval = (double)value;
            }
        }

        #endregion Properties and members
    }
}
