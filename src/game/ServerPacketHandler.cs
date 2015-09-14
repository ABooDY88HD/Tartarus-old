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

namespace game
{
	public static class ServerPacketHandler
	{
		private static XDes Des;
		private static Dictionary<short, Packets.SPacket> packet_db;

		internal static void Start()
		{
			Des = new XDes();
			Des.Init(Globals.DESKey);

			packet_db = Packets.LoadServerPackets();
		}

		internal static void PacketReceived(AuthServer server, PacketStream stream)
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
			packet_db[PacketId].func(server, ref stream, packet_db[PacketId].pos);

			stream.Dispose();

			return;
		}

		#region Parse Packet

		internal static void parse_ListServerResult(AuthServer server, ref PacketStream stream, short[] pos)
		{
			bool result = stream.ReadBool(pos[0]);

			Server.OnListServerResult(server, result);
		}

		internal static void parse_UserJoinNotice(AuthServer server, ref PacketStream stream, short[] pos)
		{
			string userId = stream.ReadString(pos[0], 60);
			int accId = stream.ReadInt32(pos[1]);
			byte permission = stream.ReadByte(pos[2]);
			byte[] key = stream.ReadBytes(8, pos[3]);

			Server.OnAuthNoticeUser(server, userId, accId, permission, key);
		}

		#endregion

		#region Send Packet
		
		internal static void send_ListServer(AuthServer server)
		{
			PacketStream stream = new PacketStream((short)0x214E);
			
			stream.WriteByte(Settings.ServerIndex);
			stream.WriteString(Settings.ServerName, 20);
			stream.WriteString(Settings.NoticeUrl, 256);
			stream.WriteString(Settings.ServerIP, 15);
			stream.WriteInt16(Settings.Port);
			stream.WriteString(Settings.AcceptorKey, Globals.AcceptorKeyLength);
			
			server.SendPacket(stream);
		}

		#endregion

		
	}
}
