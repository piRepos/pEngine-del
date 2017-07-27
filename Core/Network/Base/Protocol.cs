// Copyright (c) 2016 PK IT Andrea Demontis
//
//		pEngine / 2D Graphic engine for rythm games.
//

using System;
using System.Text;
using System.Threading;
using System.Collections.Generic;

using pEngine.Core.Data.Files;

namespace pEngine.Core.Network.Base
{
	/// <summary>
	/// Implements the pEngine network protocol.
	/// 
	/// packet structure:
	/// 
	///     0 bits			8 bits			16 bits			24 bits			32 bits
	///     +---------------+-------------------------------+---------------+ 0 Bytes
	///     |   01010101    |         Header Length         |   Type flag   |
	///     +---------------+-------------------------------+---------------+ 4 Bytes
	///     |                         Content Length                        |
	///     +-------------------------------+-+-+-----------+---------------+ 8 Bytes
	///     |        Session ID             |L|A|           |///////////////|
	///     +-------------------------------+-+-+-----------+---------------+ 12 Bytes
	///     |                                                               |
	///     |                                                               |
	///     |                                                               |
	///     :                                                               : Max 1011 Bytes
	///     |                                                               |
	///     |                                                               |
	///     |                                                               |
	///     |                                                               |
	///     |                                                +--------------+ 1023 Bytes
	///     |                                                |   01010101   |
	///     +------------------------------------------------+--------------+ 1024 Bytes
	///     
	///         L : Last packet
	///         A : Abort transfer
	///         R : Response request
	/// 
	/// file message structure:
	/// 
	///     0 bits			8 bits			16 bits			24 bits			32 bits
	///     +---------------+---------------+-------------------------------+
	///     |     Type      |     Flags     |         Header Length         |
	///     +---------------+-----------------------------------------------+
	///     |        Command Length         |       Filename Length         |
	///     +-------------------------------+-------------------------------+
	///     |                           File Length                         |
	///     +---------------------------------------------------------------+
	///     |                                                               |
	///     |                            Command                            |
	///     |                                                               |
	///     |                                                               |
	///     +---------------------------------------------------------------+
	///     |                                                               |
	///     |                            Filename                           |
	///     |                                                               |
	///     |                                                               |
	///     +---------------------------------------------------------------+
	///     |                                                               |
	///     |                                                               |
	///     |                                                               |
	///     |                                                               |
	///     |                                                               |
	///     |                              File                             |
	///     |                                                               |
	///     |                                                               |
	///     |                                                               |
	///     |                                                               |
	///     +---------------------------------------------------------------+
	/// 
	///  TODO: Change port
	/// 
	/// </summary>
	public class Protocol : Socket
    {

        #region Packet structure

        /// <summary>
        /// Specify the message content type.
        /// </summary>
        public enum MessageType
        {
            ASCII = 0x01,
            Unicode = 0x02,
            File = 0x04,
            Bytes = 0x00
        }

        /// <summary>
        /// Protocol flags (max 8 bit)
        /// </summary>
        [Flags]
        public enum PacketFlags
        {
            Last = 0x01,
            Abort = 0x02,
            Response = 0x04,
            None = 0x00
        }

        /// <summary>
        /// File packet flags.
        /// </summary>
        [Flags]
        public enum FileFlags
        {
            None = 0x00
        }

        /// <summary>
        /// Packet header.
        /// </summary>
        public struct PacketHeader
        {
            public Int16 SessionId;
            public Int16 HeaderLen;
            public Int32 DataLen;

            public PacketFlags Flags;
            public MessageType Type;

            public const byte HeadTail = 0x55;
        }

        /// <summary>
        /// Message informations.
        /// </summary>
        public struct MessageInfo
        {
            public MessageType Type;

            public int ContentLen;
            public int ReadedBytes;

            public bool Response;

            public Int16 SessionId;

            public FileTransfer TransferSession;
        }

        /// <summary>
        /// File transfer informations.
        /// </summary>
        public struct FileHeader
        {
            public FileType Type;
            public FileFlags Flags;

            public Int16 HeaderLen;
            public Int16 CommandLen;
            public Int16 FileNameLen;

            public Int32 FileLen;

            public int DataPos { get { return HeaderLen + CommandLen + FileNameLen; } }
            public int FileNamePos { get { return HeaderLen + CommandLen; } }
            public int CommandPos { get { return HeaderLen; } }
        }

        #endregion

        public Protocol(string Address, int Port) : base (Address, Port)
        {
            SendingPackets = new List<MessageInfo>();
            FileUplodas = new Dictionary<int, FileTransfer>();
            ReceiveQueues = new Dictionary<int, List<byte>>();
            ReceiveQueuesInfo = new Dictionary<int, MessageInfo>();
            FileStatus = new Dictionary<int, FileTransfer>();
        }

        #region Packet dispatcher

        /// <summary>
        /// Returns the header informations from a raw header data.
        /// </summary>
        /// <param name="RawData">Header bytes.</param>
        /// <returns>Header informations.</returns>
        PacketHeader GetHeader(byte[] RawData)
        {
            PacketHeader Header = new PacketHeader();

            if (RawData[0] != PacketHeader.HeadTail)
                throw new PacketProtocolFormatException();

            Header.HeaderLen = BitConverter.ToInt16(RawData, 1);
            Header.Type = (MessageType)RawData[3];
            Header.DataLen = BitConverter.ToInt32(RawData, 4);
            Header.SessionId = BitConverter.ToInt16(RawData, 8);
            Header.Flags = (PacketFlags)RawData[10];

            return Header;
        }

        /// <summary>
        /// Returns the header informations from a raw header data.
        /// </summary>
        /// <param name="RawData">Header bytes.</param>
        /// <returns>Header informations.</returns>
        FileHeader GetFileHeader(byte[] RawData)
        {
            FileHeader Header = new FileHeader();

            Header.Type = (FileType)RawData[0];
            Header.Flags = (FileFlags)RawData[1];

            Header.HeaderLen = BitConverter.ToInt16(RawData, 2);
            Header.CommandLen = BitConverter.ToInt16(RawData, 4);
            Header.FileNameLen = BitConverter.ToInt16(RawData, 6);

            Header.FileLen = BitConverter.ToInt32(RawData, 8);

            return Header;
        }

        /// <summary>
        /// Returns a byte array which contains the header.
        /// </summary>
        /// <param name="Header">Header informations.</param>
        /// <returns>Raw header bytes.</returns>
        byte[] GetBytes(PacketHeader Header)
        {
            byte[] RawHeader = new byte[12];

            RawHeader[0] = PacketHeader.HeadTail;

            BitConverter.GetBytes(Header.HeaderLen).CopyTo(RawHeader, 1);
            RawHeader[3] = (byte)Header.Type;
            BitConverter.GetBytes(Header.DataLen).CopyTo(RawHeader, 4);
            BitConverter.GetBytes(Header.SessionId).CopyTo(RawHeader, 8);
            RawHeader[10] = (byte)Header.Flags;
            RawHeader[11] = 0x00;

            return RawHeader;
        }

        /// <summary>
        /// Returns a byte array which contains the header.
        /// </summary>
        /// <param name="Header">Header informations.</param>
        /// <returns>Raw header bytes.</returns>
        byte[] GetFileHeaderBytes(FileHeader Header)
        {
            byte[] RawHeader = new byte[12];

            RawHeader[0] = (byte)Header.Type;
            RawHeader[1] = (byte)Header.Flags;

            BitConverter.GetBytes(Header.HeaderLen).CopyTo(RawHeader, 2);
            BitConverter.GetBytes(Header.CommandLen).CopyTo(RawHeader, 4);
            BitConverter.GetBytes(Header.FileNameLen).CopyTo(RawHeader, 6);

            BitConverter.GetBytes(Header.FileLen).CopyTo(RawHeader, 8);

            return RawHeader;
        }

        /// <summary>
        /// Get the packet data field.
        /// </summary>
        /// <param name="Data">Raw packet.</param>
        /// <param name="Header">Packet header.</param>
        /// <returns>Raw data.</returns>
        byte[] GetData(byte[] Data, PacketHeader Header)
        {
            byte[] ToReturn = new byte[Header.DataLen];

            Array.Copy(Data, Header.HeaderLen, ToReturn, 0, Header.DataLen);

            return ToReturn;
        }

        /// <summary>
        /// Get the packet file data field.
        /// </summary>
        /// <param name="Data">Raw packet.</param>
        /// <param name="Header">Packet header.</param>
        /// <returns>Raw data.</returns>
        byte[] GetFileData(byte[] Data, FileHeader Header)
        {
            byte[] ToReturn = new byte[Header.FileLen];

            Array.Copy(Data, Header.DataPos, ToReturn, 0, Header.FileLen);

            return ToReturn;
        }

        /// <summary>
        /// Get the file name from bytes.
        /// </summary>
        /// <param name="Data">Raw packet.</param>
        /// <param name="Header">Packet header.</param>
        /// <returns>Name.</returns>
        string GetFileName(byte[] Data, FileHeader Header)
        {
            byte[] Temp = new byte[Header.FileNameLen];

            if (Header.FileNameLen <= 0)
                return "";

            Array.Copy(Data, Header.FileNamePos, Temp, 0, Header.FileNameLen);

            return Encoding.UTF8.GetString(Temp);
        }

        /// <summary>
        /// Get the packet file data command.
        /// </summary>
        /// <param name="Data">Raw packet.</param>
        /// <param name="Header">Packet header.</param>
        /// <returns>Raw data.</returns>
        byte[] GetFileCommand(byte[] Data, FileHeader Header)
        {
            byte[] ToReturn = new byte[Header.CommandLen];

            Array.Copy(Data, Header.CommandPos, ToReturn, 0, Header.CommandLen);

            return ToReturn;
        }

        /// <summary>
        /// Get the packet file data command.
        /// </summary>
        /// <param name="Data">Raw packet.</param>
        /// <param name="Header">Packet header.</param>
        /// <returns>Raw data.</returns>
        string GetFileCommandString(byte[] Data, FileHeader Header)
        {
            byte[] ToReturn = new byte[Header.CommandLen];

            Array.Copy(Data, Header.CommandPos, ToReturn, 0, Header.CommandLen);

            return Encoding.UTF8.GetString(ToReturn);
        }

        #endregion

        #region Receive

        #region Events

        public delegate void BytesReceiveEventHandler(byte[] Data, MessageInfo Info);
        public delegate void ASCIIReceiveEventHandler(string Data, MessageInfo Info);
        public delegate void FilesReceiveEventHandler(File Data, FileTransfer State);
        public delegate void UnicodeReceiveEventHandler(string Data, MessageInfo Info);

        /// <summary>
        /// Triggered On raw byte receive.
        /// </summary>
        public event BytesReceiveEventHandler OnReceiveBytes;

        /// <summary>
        /// Triggered On ASCII message receive.
        /// </summary>
        public event ASCIIReceiveEventHandler OnReceiveASCII;

        /// <summary>
        /// Triggered On File receive.
        /// </summary>
        public event FilesReceiveEventHandler OnReceiveFile;

        /// <summary>
        /// Triggered On unicode message receive.
        /// </summary>
        public event UnicodeReceiveEventHandler OnReceiveUnicode;

        #endregion

        #region Callbacks

        protected virtual void BytesReceived(byte[] Data, MessageInfo Info) { }
        protected virtual void ASCIIReceived(string Data, MessageInfo Info) { }
        protected virtual void FilesReceived(File Data, FileTransfer State) { }
        protected virtual void UnicodeReceived(string Data, MessageInfo Info) { }

        #endregion

        /// <summary>
        /// Receive sessions.
        /// </summary>
        Dictionary<int, List<byte>> ReceiveQueues;

        /// <summary>
        /// Receive infos.
        /// </summary>
        Dictionary<int, MessageInfo> ReceiveQueuesInfo;

        /// <summary>
        /// Status for each file transfer.
        /// </summary>
        Dictionary<int, FileTransfer> FileStatus;

        protected override void ReceivedPacket(PacketState Session)
        {
            // Get Packet header
            PacketHeader Header = GetHeader(Session.PacketBuffer);

            // Check session
            if (!ReceiveQueues.ContainsKey(Header.SessionId))
            {
                MessageInfo Info = new MessageInfo()
                {
                    Type = Header.Type,
                    ContentLen = Header.DataLen,
                    ReadedBytes = 0,
                    SessionId = Header.SessionId,
                    TransferSession = Header.Type == MessageType.File ? new FileTransfer(new File()) : null
                };

                ReceiveQueues[Header.SessionId] = new List<byte>();
                ReceiveQueuesInfo[Header.SessionId] = Info;

                if (Header.Type == MessageType.File)
                {
                    byte[] FileData = GetData(Session.PacketBuffer, Header);
                    FileHeader H = GetFileHeader(FileData);

                    File F = new File(GetFileName(FileData, H), new byte[H.FileLen]);
                    FileStatus[Header.SessionId] = new FileTransfer(F);

					FileTransfer Transfer = FileStatus[Header.SessionId];
					OnReceiveFile?.Invoke(Transfer.CurrentFile, Transfer);
                }
            }

            // Append data
            List<byte> Buffer = ReceiveQueues[Header.SessionId];
			byte[] currData = GetData(Session.PacketBuffer, Header);
			Buffer.AddRange(currData);

            if (Header.Type == MessageType.File)
            {
                FileStatus[Header.SessionId].Report(currData.Length);
            }

            // Check message end
            if (Header.Flags.HasFlag(PacketFlags.Last))
            {
                MessageInfo Info = ReceiveQueuesInfo[Header.SessionId];
                NotifyMessage(Buffer.ToArray(), Info);

                // Remove session
                ReceiveQueues.Remove(Header.SessionId);
                ReceiveQueuesInfo.Remove(Header.SessionId);
            }

            Receive();
        }

        void NotifyMessage(byte[] Data, MessageInfo Info)
        {
            // Notify the packet
            switch (Info.Type)
            {
                case MessageType.Bytes:

                    BytesReceived(Data, Info);

                    OnReceiveBytes?.Invoke(Data, Info);

                    break;

                case MessageType.ASCII:

                    string Message = Encoding.ASCII.GetString(Data);

                    ASCIIReceived(Message, Info);

                    OnReceiveASCII?.Invoke(Message, Info);

                    break;

                case MessageType.Unicode:

                    string UniMessage = Encoding.UTF8.GetString(Data);

                    UnicodeReceived(UniMessage, Info);

                    OnReceiveUnicode?.Invoke(UniMessage, Info);

                    break;

                case MessageType.File:

                    FileHeader Header = GetFileHeader(Data);

                    File F = FileStatus[Info.SessionId].CurrentFile;

                    Array.Copy(GetFileData(Data, Header), F.Content, Header.FileLen);

					FileStatus[Info.SessionId].Report(-1);

                    FileStatus.Remove(Info.SessionId);

                    break;
            }
        }

        #endregion

        #region Send

        // - Session -----------------------------------------------------------------

        /// <summary>
        /// List of messages in queue.
        /// </summary>
        List<MessageInfo> SendingPackets;

        /// <summary>
        /// File transfer sessions.
        /// </summary>
        Dictionary<int, FileTransfer> FileUplodas;

        ManualResetEvent WaitAndSend = new ManualResetEvent(true);

        // ---------------------------------------------------------------------------

        /// <summary>
        /// Send a message to the server.
        /// </summary>
        /// <param name="Data">Raw message data.</param>
        /// <param name="Type">Content type.</param>
        /// <param name="TransferSession">[Optional] File session.</param>
        void Send(byte[] Data, MessageType Type, FileTransfer TransferSession = null)
        {
            short CurrentSession = 0;

            lock (SendingPackets) lock (FileUplodas) lock (WaitAndSend)
            {
                #region Session check

                Random Rnd = new Random();

                bool Found = true;
                do
                {
                    Found = true;

                    if (SendingPackets.Exists((x) => x.SessionId == CurrentSession))
                    {
                        Found = false;

                        CurrentSession = (short)Rnd.Next();
                    }
                } while (!Found);

                #endregion

                MessageInfo Info = new MessageInfo()
                {
                    ReadedBytes = 0,
                    ContentLen = Data.Length,
                    SessionId = CurrentSession,
                    Type = Type,
                    TransferSession = TransferSession
                };

                // Add the current packet to sending queue
                SendingPackets.Add(Info);

                // If file ad session for the current file
                if (TransferSession != null)
                {
                    FileUplodas.Add(CurrentSession, TransferSession);
                }

            }

            // Split data into 1011 bytes chunks
            for (int CurrByte = 0; CurrByte < Data.Length; CurrByte += PacketState.BufferSize - 13)
            {
                if (TransferSession != null)
                {
                    WaitAndSend.WaitOne();
                    WaitAndSend.Reset();
                }

                // Split data and insert into a temp buffer
                int Size = Math.Min(PacketState.BufferSize - 13, Data.Length - CurrByte);
                byte[] TempBuffer = new byte[Size];
                Array.Copy(Data, CurrByte, TempBuffer, 0, Size);

                // Prepare flags
                PacketFlags Flags = PacketFlags.None;

                // Flags for abort
                if (TransferSession != null)
                    lock (TransferSession)
                        if (TransferSession.IsAborted)
                            Flags |= PacketFlags.Abort;

                // Flags for last message
                if (Data.Length - CurrByte <= PacketState.BufferSize - 13) Flags |= PacketFlags.Last;

                SendPacket(TempBuffer, Type, Flags, CurrentSession);

                if (TransferSession != null)
                    lock (TransferSession)
                        if (TransferSession.IsAborted)
                            return;
            }

        }
        
        /// <summary>
        /// Send a message packet to the server.
        /// (Piece of message)
        /// </summary>
        /// <param name="Data">Raw packet data.</param>
        /// <param name="Type">Content type.</param>
        /// <param name="Flags">Protocol flags.</param>
        /// <param name="Session">Message session.</param>
        void SendPacket(byte[] Data, MessageType Type, PacketFlags Flags, short Session)
        {
            
            PacketHeader Header = new PacketHeader()
            {
                HeaderLen = 12,
                DataLen = Data.Length,
                Type = Type,
                Flags = Flags,
                SessionId = Session
            };

            // Check data length
            if (Data.Length > PacketState.BufferSize - Header.HeaderLen - 1)
                throw new PacketProtocolFormatException();

            // Prepare header
            byte[] RawHeader = GetBytes(Header);

            // Alloc memory for the send buffer
            byte[] FinalBuffer = new byte[PacketState.BufferSize];

            // Build send buffer
            RawHeader.CopyTo(FinalBuffer, 0);
            Data.CopyTo(FinalBuffer, RawHeader.Length);
            FinalBuffer[FinalBuffer.Length - 1] = PacketHeader.HeadTail;

            // Send packet
            base.Send(FinalBuffer);
        }

        protected override void SendCallback(IAsyncResult Result)
        {
            // Get send session
            PacketState SentPacket;
            SentPacket = (PacketState)Result.AsyncState;

            PacketHeader Info = GetHeader(SentPacket.PacketBuffer);
            
            lock (FileUplodas)
            {
                if (Info.Type == MessageType.File)
                {
                    FileUplodas[Info.SessionId].Report(SentPacket.PacketBuffer.Length - 13);
                }
            }

            lock (SendingPackets)
            {
                if (Info.Flags == PacketFlags.Last || Info.Flags == PacketFlags.Abort
                || Info.Flags == (PacketFlags.Last | PacketFlags.Abort))
                {
                    lock (SendingPackets)
                        SendingPackets.RemoveAll((x) => x.SessionId == Info.SessionId);

                    base.SendCallback(Result);

                    return;
                }
            }


            base.SendCallback(Result);
            
            WaitAndSend.Set();
        }

        /// <summary>
        /// Send a message to the server.
        /// </summary>
        /// <param name="Data">Bytes to send.</param>
        public void SendBytes(byte[] Data)
        {
            Send(Data, MessageType.Bytes);
        }

        /// <summary>
        /// Send a message to the server.
        /// </summary>
        /// <param name="Data">String to send.</param>
        public void SendAscii(string Message)
        {
            byte[] Data = Encoding.ASCII.GetBytes(Message);
            Send(Data, MessageType.ASCII);
        }

        /// <summary>
        /// Send a message to the server.
        /// </summary>
        /// <param name="Data">String to send.</param>
        public void SendUnicode(string Message)
        {
            byte[] Data = Encoding.UTF8.GetBytes(Message);

            Send(Data, MessageType.Unicode);
        }

        /// <summary>
        /// Send a file to the server.
        /// </summary>
        /// <param name="Data">File to send.</param>
        /// <param name="Header">Header for file commands.</param>
        public FileTransfer SendFile(File Data, byte[] Message)
        {
            byte[] RawTitle = Encoding.UTF8.GetBytes(Data.Name);

            FileHeader Header = new FileHeader
            {
                FileLen = (int)Data.Size,
                FileNameLen = (short)RawTitle.Length,
                CommandLen = (short)Message.Length,
                HeaderLen = (short)12,
                Flags = FileFlags.None,
                Type = FileType.Binary
            };

            byte[] RawHeader = GetFileHeaderBytes(Header);

            int Size = Header.FileNameLen + Header.CommandLen + Header.FileLen + Header.HeaderLen;

            byte[] FinalBuffer = new byte[Size];

            // Make final buffer
            RawHeader.CopyTo(FinalBuffer, 0);
            Message.CopyTo(FinalBuffer, Header.CommandPos);
            RawTitle.CopyTo(FinalBuffer, Header.FileNamePos);
            Data.Content.CopyTo(FinalBuffer, Header.DataPos);

            FileTransfer Transfer = new FileTransfer(Data);

            Thread Sender = new Thread(() =>
            {
                Send(FinalBuffer, MessageType.File, Transfer);
            });

            Sender.Priority = ThreadPriority.BelowNormal;

            Sender.Start();

            return Transfer;
        }

        /// <summary>
        /// Send a file to the server.
        /// </summary>
        /// <param name="Data">File to send.</param>
        /// <param name="Header">Header for file commands.</param>
        public FileTransfer SendFile(File Data, string Header)
        {
            byte[] HeaderData = Encoding.UTF8.GetBytes(Header);
            return SendFile(Data, HeaderData);
        }


        #endregion

    }

    public class PacketProtocolFormatException : Exception
    {

    }
}