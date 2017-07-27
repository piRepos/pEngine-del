// Copyright (c) 2016 PK IT Andrea Demontis
//
//		pEngine / 2D Graphic engine for rythm games.
//

using System;

using System.Net;
using System.Net.Sockets;

using System.Threading;

namespace pEngine.Core.Network.Base
{

    /// <summary>
    /// Basic TCP socket.
    /// </summary>
    public class Socket
    {

        #region Packet

        public class PacketState
        {
            /// <summary>
            /// Socket for the server connection.
            /// </summary>
            public System.Net.Sockets.Socket ServerSocket;

            /// <summary>
            /// This buffer will contains the full packet.
            /// </summary>
            public byte[] PacketBuffer;

            public const int BufferSize = 1024;
        }

        #endregion

        #region Meta

        /// <summary>
        /// Gets the address.
        /// </summary>
        /// <value>The address.</value>
        public string Address { get; }

        /// <summary>
        /// Gets the port.
        /// </summary>
        /// <value>The port.</value>
        public int Port { get; }

        #endregion

        #region State

        /// <summary>
        /// Gets a value indicating whether this <see cref="pEngine.Network.Socket"/> is connected.
        /// </summary>
        /// <value><c>true</c> if connected; otherwise, <c>false</c>.</value>
        public bool Connected 
        {
            get
            {
                if (ServerSocket == null)
                    return false;
                
                return ServerSocket.Connected;
            }
        }

        /// <summary>
        /// Check if listening process is running.
        /// </summary>
        public bool Listening
        {
            get
            {
                return listening;
            }
        }

        #endregion

        public Socket (string Address, int Port)
        {
            if (Address == "" || Port < 0)
                throw new InvalidAddressException ();
            
            this.Address = Address;
            this.Port = Port;
        }

        #region Connection

        IPHostEntry HostEntry;
        IPAddress ServerIP;

        IPEndPoint ServerEndPoint;

        System.Net.Sockets.Socket ServerSocket;

        AsyncCallback ConnectAsyncCallback;

        public delegate void ConnectionEventHandler(Socket Sender);

        /// <summary>
        /// Occurs when socket is connected.
        /// </summary>
        public event ConnectionEventHandler OnConnect;

        /// <summary>
        /// Occurs when the connection fails.
        /// </summary>
        public event ConnectionEventHandler OnConnectionFail;

        /// <summary>
        /// Occurs when the connection finish.
        /// </summary>
        public event ConnectionEventHandler OnDisconnect;

        /// <summary>
        /// Connect this socket to the specified server.
        /// </summary>
        public void Connect()
        {
            if (Connected)
                throw new SocketAlreadyConnetedException();

            // Resolve host
            try
            {
                HostEntry = Dns.GetHostEntry(Address);
                ServerIP = HostEntry.AddressList[0];
            }
            catch (Exception) 
            {
                throw new AddresNotSolvedException ();
            }

            // Set endpoint with port and ip
            ServerEndPoint = new IPEndPoint(ServerIP, Port);

            // Setting TCP socket
            ServerSocket = new System.Net.Sockets.Socket(ServerIP.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            // Disable the Nagle Algorithm for this tcp socket.
            ServerSocket.NoDelay = true;

            ServerSocket.DontFragment = true;

            // Start connection
            ConnectAsyncCallback = new AsyncCallback(ConnectionCallback);
            ServerSocket.BeginConnect (ServerEndPoint, ConnectAsyncCallback, ServerSocket);
        }

        /// <summary>
        /// Disconnect this socket.
        /// </summary>
        public void Disconnect()
        {
            try
            {
                ServerSocket.Shutdown(SocketShutdown.Both);
                ServerSocket.Disconnect(false);
            }
            catch (SocketException) { }
            catch (NullReferenceException) { }
            finally
            {
                OnDisconnect?.Invoke(this);

                listening = false;
            }
        }

        private void ConnectionCallback(IAsyncResult Result)
        {
            System.Net.Sockets.Socket ServerSocket;
            ServerSocket = Result.AsyncState as System.Net.Sockets.Socket;

            try
            {
                ServerSocket.EndConnect (Result);

                OnConnect?.Invoke(this);
            }
            catch (SocketException) 
            {
                OnConnectionFail?.Invoke(this);
            }
        }

        #endregion

        #region Send

        public delegate void PacketSendEventHandler(Socket Sender, PacketState PacketState);

        /// <summary>
        /// Occurs when a packet is send.
        /// </summary>
        public event PacketSendEventHandler OnPacketSend;

        /// <summary>
        /// Occurs when a packet is not sent.
        /// </summary>
        public event PacketSendEventHandler OnSendError;

        AsyncCallback SendAsyncCallback;

        /// <summary>
        /// Thread safe.
        /// </summary>
        Mutex CriticalZone = new Mutex();

        /// <summary>
        /// Send the specified Data.
        /// </summary>
        /// <param name="Data">Data to send.</param>
        public virtual void Send(byte [] Data)
        {
            if (!Connected)
                throw new SocketNotConnectedException ();

            PacketState Packet = new PacketState ();
            Packet.ServerSocket = ServerSocket;
            Packet.PacketBuffer = Data;

            SendAsyncCallback = new AsyncCallback (SendCallback);

            CriticalZone.WaitOne();
            
            ServerSocket.BeginSend (Data, 0, Data.Length, SocketFlags.None, SendAsyncCallback, Packet);

            CriticalZone.ReleaseMutex();
        }

        protected virtual void SendCallback(IAsyncResult Result)
        {
            PacketState SentPacket;
            SentPacket = (PacketState)Result.AsyncState;

            try
            {
                SentPacket.ServerSocket.EndSend (Result);

                OnPacketSend?.Invoke(this, SentPacket);
            }
            catch (SocketException) 
            {
                OnSendError?.Invoke(this, SentPacket);

                Disconnect();
            }
        }

        #endregion

        #region Receive

        public delegate void PacketReceiveEventHandler(Socket Sender, PacketState PacketState);

        /// <summary>
        /// Triggered On receive error.
        /// </summary>
        public event PacketReceiveEventHandler OnReceiveError;

        /// <summary>
        /// Triggered On packet received.
        /// </summary>
        public event PacketReceiveEventHandler OnReceivePacket;

        /// <summary>
        /// To implement for protocol managing.
        /// </summary>
        /// <param name="Session">Packet session.</param>
        protected virtual void ReceivedPacket(PacketState Session) { }

        AsyncCallback ReceiveAsyncCallback;

        bool listening;
        
        public void StartListen()
        {
            if (listening)
                throw new SocketAlreadyListeningException();

            listening = true;

            Receive();
        }


        public virtual void Receive()
        {
            PacketState ReceiveState;

            ReceiveState = new PacketState();

            try
            {
                ReceiveAsyncCallback = new AsyncCallback(ReceiveCallback);

                ReceiveState.PacketBuffer = new byte[PacketState.BufferSize];
                ReceiveState.ServerSocket = ServerSocket;

                ServerSocket.BeginReceive(ReceiveState.PacketBuffer, 0, PacketState.BufferSize, 0,
                ReceiveAsyncCallback, ReceiveState);
            }
            catch (SocketException)
            {
                if (OnReceiveError != null)
                    OnSendError(this, ReceiveState);

                Disconnect();
            }
        }


        void ReceiveCallback(IAsyncResult Result)
        {
            PacketState ReceivedPacket;
            ReceivedPacket = (PacketState)Result.AsyncState;

            try
            {
                // Read data from the remote device.
                ReceivedPacket.ServerSocket.EndReceive(Result);

                this.ReceivedPacket(ReceivedPacket);

                OnReceivePacket?.Invoke(this, ReceivedPacket);
            }
            catch (SocketException)
            {
                if (OnReceiveError != null)
                    OnSendError(this, ReceivedPacket);

                Disconnect();
            }
        }

        #endregion
    }

    #region Exceptions

    class AddresNotSolvedException : Exception
    {
        
    }

    class InvalidAddressException : Exception
    {
        
    }

    class SocketNotConnectedException : Exception
    {
        
    }

    class SocketAlreadyConnetedException : Exception
    {
        
    }   

    class SocketAlreadyListeningException : Exception
    {

    }

    #endregion
}

