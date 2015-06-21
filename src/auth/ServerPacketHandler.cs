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

		internal static void PacketReceived(GameServer server, PacketStream stream)
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

		internal static void parse_ServerRegister(GameServer game, ref PacketStream stream, short[] pos)
		{
			int index = stream.ReadBytes(pos[0], 1)[0]; // 0
			string name = ByteUtils.toString(stream.ReadBytes(pos[1], 20)); // 20
			string image = ByteUtils.toString(stream.ReadBytes(pos[2], 256)); // 276
			string ip = ByteUtils.toString(stream.ReadBytes(pos[3], 15)); // 291
			short port = BitConverter.ToInt16(stream.ReadBytes(pos[4], 2), 0); // 293
			string key = ByteUtils.toString(stream.ReadBytes(pos[5], Globals.AcceptorKeyLength));

			Server.OnGameServerConnect(game, index, name, image, ip, port, key);
		}

		#endregion

		#region Send Packet

		internal static void send_ServerConnectionResult(GameServer game, bool result)
		{
			PacketStream res = new PacketStream((short)0x4E22);
			res.WriteBool(result);

			game.SendPacket(res);
		}

		internal static void send_ClientJoin(GameServer gameServer, Client client, byte[] key)
		{
			PacketStream data = new PacketStream((short)0x4E2B);
			
			data.WriteString(client.UserId, 60);
			data.WriteInt32(client.AccountId);
			data.WriteByte(client.Permission);
			data.Write(key, 0, 8);

			gameServer.SendPacket(data);
		}

		#endregion

		
	}
}
