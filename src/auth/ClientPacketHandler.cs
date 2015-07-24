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

		// [0x270F] 9999 -> Unknown1
		internal static void parse_Unknown1(Client client, ref PacketStream pStream, short[] pos) { return; }
		
		// [0x2711] 10001 -> Client Version
		internal static void parse_ClientVersion(Client client, ref PacketStream pStream, short[] pos) { return; }
		
		// [0x271A] 10010 -> Login Try
		internal static void parse_LoginTry(Client client, ref PacketStream pStream, short[] pos)
		{
			string user_id = ByteUtils.toString(pStream.ReadBytes((pos[0]), 60));
			string password = Des.Decrypt(pStream.ReadBytes((pos[1]), 8)).Trim('\0');

			client.TryLogin(user_id, password);
		}
		
		// [0x2725] 10021 -> Request Server List
		internal static void parse_RequestServerList(Client client, ref PacketStream pStream, short[] pos)
		{
			Server.OnUserRequestServerList(client);
		}

		// [0x2726] 10023 -> Request Game Server Connection
		internal static void parse_JoinGameServer(Client client, ref PacketStream pStream, short[] pos)
		{
			byte server_index = pStream.ReadByte(pos[0]);
			Server.OnUserJoinGame(client, server_index);
		}

		#endregion

		#region Send Packet

		// [0x2710] 10000 -> Login Result
		internal static void send_LoginResult(Client client, Packets.LoginResult result)
		{
			PacketStream data = new PacketStream((short)0x2710);
			data.WriteInt16(0x271A);
			data.WriteInt32((Int32)result);
			// official packet has 2 extra zeroes
			ClientManager.Instance.Send(client, data);
		}

		// [0x2726] 10022 -> Server List
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

		// [0x2728] 10024 -> Join Game (Login Token)
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
