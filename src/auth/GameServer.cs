// Copyright (c) Tartarus Dev Team, licensed under GNU GPL.
// See the LICENSE file
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using common.RC4;
using common;

namespace auth
{
	public class GameServer
	{
		// These properties are used by
		// auth-server to send Server List to users
		public byte Index { get; set; }
		public string Name { get; set; }
		public IPAddress Ip { get; set; }
		public short Port { get; set; }
		public string NoticeUrl { get; set; }

		private Socket Socket;

        private NetworkStream NetStream;
        private BackgroundWorker bwReceiver;

		public GameServer(Socket socket)
        {
			this.Socket = socket;
            this.NetStream = new NetworkStream(this.Socket);
            this.bwReceiver = new BackgroundWorker();
            this.bwReceiver.DoWork += new DoWorkEventHandler(StartReceive);
            this.bwReceiver.RunWorkerAsync();
		}
		
		private void StartReceive(object sender, DoWorkEventArgs e)
		{
			try
			{
				while (this.Socket.Connected && this.NetStream.CanRead)
				{
					lock (this)
					{
						// Reads the Packet Header
						byte[] buffer = new byte[Globals.HeaderLength];

						// TODO: If server disconect, this crashes
						if (this.NetStream.Read(buffer, 0, Globals.HeaderLength) < Globals.HeaderLength)
						{
							ConsoleUtils.Write(
								ConsoleMsgType.Info,
								"Invalid Packet Size Received from {0}. Connection Closed.\r\n",
								this.Ip);
							break;
						}

						int packetSize = BitConverter.ToInt32(buffer, 0);
						short packetId = BitConverter.ToInt16(buffer, 4);

						if (packetSize > 4096 || packetSize < 0)
						{
							ConsoleUtils.Write(
								ConsoleMsgType.Error,
								"Unexpected Packet Size {0} from {1}. Closing Connection.",
								packetSize, this.Ip);
							break;
						}

						buffer = new byte[packetSize - 7];

						if (this.NetStream.Read(buffer, 0, (packetSize - 7)) < (packetSize - 7))
						{
							ConsoleUtils.Write(
								ConsoleMsgType.Info,
								"Connection to {0} closed.\r\n",
								this.Ip);
							break;
						}

						// TODO : Maybe there's no need to store packet size/id, and this can be
						//		  changed to a MemoryStream or byte array
						PacketStream stream = new PacketStream();
						stream.WriteInt32(packetSize);
						stream.WriteInt16(packetId);
						stream.WriteByte((byte)0x00);
						stream.Write(buffer, 0, buffer.Length);
						this.OnCommandReceived(stream);
					}
				}
			}
			catch (SocketException)
			{
				ConsoleUtils.Write(ConsoleMsgType.Info, "Connection to {0} closed.\r\n", this.Ip);
			}

			this.NetStream.Close();
			this.NetStream.Dispose();

			this.Socket.Close();
			this.Socket.Dispose();
			Server.OnGameServerDisconnect(this.Index);
		}

		private void OnCommandReceived(PacketStream stream)
		{
			ServerPacketHandler.PacketReceived(this, stream);
		}

		public void SendPacket(PacketStream packet)
		{
			try
			{
				if (Socket.Connected)
				{
					byte[] buffer = packet.GetPacket().ToArray();

					this.NetStream.Write(buffer, 0, buffer.Length);
					this.NetStream.Flush();

					return;
				}
				else
				{
					ConsoleUtils.Write(ConsoleMsgType.Info, "Connection to {0} closed.\r\n", this.Ip);
					Socket.Close();
					//NetworkManager.Instance.RemoveGame(this);
					return;
				}
			}
			catch (SocketException)
			{
				ConsoleUtils.Write(ConsoleMsgType.Info, "Connection to {0} closed.\r\n", this.Ip);
				this.Socket.Close();
				//NetworkManager.Instance.RemoveGame(this);

				return;
			}
		}
	}
}
