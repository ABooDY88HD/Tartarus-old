// Copyright (c) Tartarus Dev Team, licensed under GNU GPL.
// See the LICENSE file
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using common;
using common.DES;
using common.Utilities;

namespace auth
{
	public static class ClientPacketHandler
	{
		private static XDes Des;
		private static Dictionary<short, Packets.Packet> packet_db;

		internal static void Start()
		{
			Des = new XDes();
			Des.Init(Globals.DESKey);

			packet_db = Packets.LoadClientPackets();
		}

		internal static void PacketReceived(Client client, PacketStream stream)
		{
			// Header
			// [Size:4]
			// [ID:2]
			// [Checksum(?):1]
			short PacketId = stream.GetId();

			if (!packet_db.ContainsKey(PacketId))
			{
				ConsoleUtils.HexDump(stream.ToArray(), "Unknown Packet Received", PacketId, stream.GetSize());
				return;
			}

			ConsoleUtils.HexDump(
				stream.ToArray(),
				"Packet Received",
				PacketId,
				stream.GetSize()
			);
			packet_db[PacketId].func(client, ref stream, packet_db[PacketId].pos);

			return;
		}
		
		#region Parse Packet

		/// [0x270F] 9999 -> (CA) Unknown1
		internal static void parse_Unknown1(Client client, ref PacketStream pStream, short[] pos) { return; }
		
		/// [0x2711] 10001 -> (CA) Client Version
		/// <version>.20S
		internal static void parse_ClientVersion(Client client, ref PacketStream pStream, short[] pos) { return; }
		
		/// [0x271A] 10010 -> (CA) Login
		/// <username>.60S <password>.8B
		internal static void parse_LoginTry(Client client, ref PacketStream pStream, short[] pos)
		{
			string user_id = ByteUtils.toString(pStream.ReadBytes((pos[0]), 60));
			string password = Des.Decrypt(pStream.ReadBytes((pos[1]), 8)).Trim('\0');

			client.TryLogin(user_id, password);
		}
		
		/// [0x2725] 10021 -> (CA) Request Server List
		internal static void parse_RequestServerList(Client client, ref PacketStream pStream, short[] pos)
		{
			Server.OnUserRequestServerList(client);
		}

		/// [0x2726] 10023 -> (CA) Request Game Server Connection
		/// <server index>.W
		internal static void parse_JoinGameServer(Client client, ref PacketStream pStream, short[] pos)
		{
			byte server_index = pStream.ReadByte(pos[0]);
			Server.OnUserJoinGame(client, server_index);
		}

		#endregion

		#region Send Packet

		/// [0x2710] 10000 -> (AC) Login Result
		/// <packet id>.W <result>.W <0>.L
		internal static void send_LoginResult(Client client, Packets.LoginResult result)
		{
			PacketStream data = new PacketStream((short)0x2710);
			data.WriteInt16(0x271A);
			data.WriteInt16((short) result);
			data.WriteInt32(0);
			ClientManager.Instance.Send(client, data);
		}

		/// [0x2726] 10022 -> (AC) Server List
		/// <unknown>.W <servers count>.W 
		/// { <index>.W <name>.22S <notice url>.256S <ip>.16S 
		///   <port>.W <status>.W <unknown2>.W}*(server count)
		internal static void send_ServerList(Client client, GameServer[] servers)
		{
			PacketStream data = new PacketStream((short)0x2726);
			// TODO: Check these values
			data.WriteInt16(1);
			data.WriteInt16(1); //servers.Length);

			foreach(GameServer sv in servers.Where(s => s != null))
			{
				data.WriteByte(sv.Index);
				data.WriteByte(0x00);
				data.WriteString(sv.Name, 21);
				data.WriteByte(0x00);
				data.WriteString(sv.NoticeUrl, 255);
				data.WriteByte(0x00);
				data.WriteString(sv.Ip.ToString(), 15);
				data.WriteByte(0x00);
				data.WriteInt16(sv.Port);

				data.WriteInt16(0); // Server Status
				data.WriteInt16(0);
			}

			ClientManager.Instance.Send(client, data);
		}

		/// [0x2728] 10024 -> (AC) Join Game (Login Token)
		/// <unknown>.W <key>.8B <0A 00 00 00>.L
		internal static void send_JoinGame(Client client, byte[] key)
		{
			PacketStream data = new PacketStream((short)0x2728);
			data.WriteInt16(0);
			data.Write(key, 0, 8);
			data.WriteInt32(10);

			ClientManager.Instance.Send(client, data);
		}

		#endregion

		
	}
}
