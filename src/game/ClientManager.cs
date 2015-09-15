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
using System.IO;
using System.Threading;

namespace game
{
	public class ClientManager
	{
		public enum SendTarget
		{
			Self,
			Area,
			Server
		}

		public static readonly ClientManager Instance = new ClientManager();
		private ManualResetEvent allDone = new ManualResetEvent(false);
		
		public ClientManager() { }

		public void Start(object sender, DoWorkEventArgs evArgs)
		{
			byte[] bytes = new Byte[1024];
			IPAddress ipAddress = IPAddress.Parse(Settings.ServerIP);
			IPEndPoint localEndPoint = new IPEndPoint(ipAddress, Settings.Port);

			// Create a TCP/IP Socket
			Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			try
			{
				listener.Bind(localEndPoint);
				listener.Listen(Settings.MaxConnections);

				ConsoleUtils.Write(ConsoleMsgType.Status, "Listening at port {0}\r\n", localEndPoint.Port);

				while (true)
				{
					allDone.Reset();

					listener.BeginAccept(new AsyncCallback(AcceptCallback), listener);

					allDone.WaitOne();
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
			}
		}

		// TODO : Target must not be optional
		public void Send(GameObject srcObject, PacketStream data, SendTarget target = SendTarget.Self)
		{
			byte[] byteData = data.GetPacket().ToArray();
			if (target == SendTarget.Self && srcObject.SubType != GameObjectSubType.Player)
			{
				ConsoleUtils.Write(ConsoleMsgType.Error, "Non-Player object trying to send packet to Self");
				return;
			}
			else if (target == SendTarget.Self)
			{
				SendPacket((Player)srcObject, byteData);
			}

			uint rx = RegionMngr.GetRegionX(srcObject.Position.X);
			uint ry = RegionMngr.GetRegionY(srcObject.Position.Y);

			foreach (Player p in GObjectManager.Players.Values)
			{
				if (target == SendTarget.Area)
				{
					// If it's out of range
					if (p.RegionX < rx - 3 || p.RegionX > rx + 3 ||
						p.RegionY < ry - 3 || p.RegionY > ry + 3
					)
						continue;
				}
				
				SendPacket(p, byteData);
			}
		}

		private void SendPacket(Player player, byte[] byteData)
		{
			ConsoleUtils.HexDump(byteData, "Sending Packet");

			// Begin sending the data to the remote device.
			player.NetData.ClSocket.BeginSend(
					player.NetData.Encoder.DoCipher(ref byteData),
					0,
					byteData.Length,
					0,
					new AsyncCallback(SendCallback), player.NetData.ClSocket
				);
		}

		private void AcceptCallback(IAsyncResult ar)
		{
			allDone.Set();

			Socket listener = (Socket)ar.AsyncState;
			Socket handler = listener.EndAccept(ar);

			Player p = GObjectManager.GetNewPlayer();
			p.NetData.ClSocket = handler;
			p.NetData.Encoder = new XRC4Cipher(Globals.RC4Key);
			p.NetData.Decoder = new XRC4Cipher(Globals.RC4Key);
			p.NetData.Buffer = new byte[Globals.MaxBuffer];
			p.NetData.Data = new PacketStream();

			handler.BeginReceive(p.NetData.Buffer, 0, Globals.MaxBuffer, 0,
				new AsyncCallback(ReadCallback), p);
		}
		
		private void ReadCallback(IAsyncResult ar)
		{
			// Retrieve the state object and the handler socket
			// from the asynchronous state object.
			Player player = (Player)ar.AsyncState;
			Player.Network netData = player.NetData;
			Socket handler = netData.ClSocket;

			//try
			//{
			// Read data from the client socket. 
			int bytesRead = handler.EndReceive(ar);
			if (bytesRead > 0)
			{
				byte[] decode = netData.Decoder.DoCipher(ref netData.Buffer, bytesRead);
				int curOffset = 0;
				int bytesToRead = 0;

				do
				{
					if (netData.PacketSize == 0)
					{
						if (netData.Offset + bytesRead > 3)
						{
							bytesToRead = (4 - netData.Offset);
							netData.Data.Write(decode, curOffset, bytesToRead);
							curOffset += bytesToRead;
							netData.Offset = bytesToRead;
							netData.PacketSize = BitConverter.ToInt32(netData.Data.ReadBytes(4, 0, true), 0);
						}
						else
						{
							netData.Data.Write(decode, 0, bytesRead);
							netData.Offset += bytesRead;
							curOffset += bytesRead;
						}
					}
					else
					{
						int needBytes = netData.PacketSize - netData.Offset;

						// If there's enough bytes to complete this packet
						if (needBytes <= (bytesRead - curOffset))
						{
							netData.Data.Write(decode, curOffset, needBytes);
							curOffset += needBytes;
							// Packet is done, send to server to be parsed
							// and continue.
							PacketReceived(player, netData.Data);
							// Do needed clean up to start a new packet
							netData.Data = new PacketStream();
							netData.PacketSize = 0;
							netData.Offset = 0;
						}
						else
						{
							bytesToRead = (bytesRead - curOffset);
							netData.Data.Write(decode, curOffset, bytesToRead);
							netData.Offset += bytesToRead;
							curOffset += bytesToRead;
						}
					}
				} while (bytesRead - 1 > curOffset);

			}
			else
			{
				ConsoleUtils.Write(ConsoleMsgType.Info, "User disconected\r\n");
				return;
			}
			/*}
			catch (Exception e)
			{
				ConsoleUtils.Write(MSG_TYPE.Info, "User disconected. " + e.Message);
			}*/
			handler.BeginReceive(netData.Buffer, 0, Globals.MaxBuffer, 0,
						new AsyncCallback(ReadCallback), player);
		}

		private void PacketReceived(Player p, PacketStream data)
		{
			ClientPacketHandler.PacketReceived(p, data);
		}

		private void SendCallback(IAsyncResult ar)
		{
			try
			{
				// Retrieve the socket from the state object.
				Socket handler = (Socket)ar.AsyncState;

				// Complete sending the data to the remote device.
				int bytesSent = handler.EndSend(ar);

			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
			}
		}
	}
}