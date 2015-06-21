// Copyright (c) Tartarus Dev Team, licensed under GNU GPL.
// See the LICENSE file
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using common;

namespace auth
{
	public class Packets
	{
		public delegate void PacketAction(Client client, ref PacketStream stream, short[] pos);
		public delegate void ServerPacketAction(GameServer server, ref PacketStream stream, short[] pos);

		public struct Packet
		{
			public int lenght;
			public PacketAction func;
			public short[] pos;
		}

		public struct SPacket
		{
			public int lenght;
			public ServerPacketAction func;
			public short[] pos;
		}

		public static Dictionary<short, Packet> LoadClientPackets()
		{
			Dictionary<short, Packet> packets_db = new Dictionary<short, Packet>();

			// [0x270F] 9999 -> Unknown1 (Client)
			packets_db.Add(0x270F, new Packet() { lenght = 11, func = ClientPacketHandler.parse_Unknown1, pos = null });
			// [0x2710] 10000 -> Login Result (Server)
			packets_db.Add(0x2710, new Packet() { lenght = 15 });
			// [0x2711] 10001 -> Client Version (?) (Client)
			packets_db.Add(0x2711, new Packet() { lenght = 27, func = ClientPacketHandler.parse_ClientVersion, pos = new short[] { 0 } });

			// [0x271A] 10010 -> Login Try (Client)
			packets_db.Add(0x271A, new Packet() { lenght = 129, func = ClientPacketHandler.parse_LoginTry, pos = new short[] { 0, 61 } });

			// [0x2725] 10021 -> Request Server List (Client)
			packets_db.Add(0x2725, new Packet() { lenght = 7, func = ClientPacketHandler.parse_RequestServerList });
			// [0x2726] 10022 -> Server List (Server)
			packets_db.Add(0x2726, new Packet() { lenght = 313, func = null });
			// [0x2727] 10023 -> Join Game Server (Client)
			packets_db.Add(0x2727, new Packet() { lenght = 9, func = ClientPacketHandler.parse_JoinGameServer, pos = new short[] { 0 } });
			// [0x2728] 10024 -> Allow Join (?) (Server)
			packets_db.Add(0x2728, new Packet() { lenght = 21 });
			
			return packets_db;
		}

		public static Dictionary<short, SPacket> LoadServerPackets()
		{
			Dictionary<short, SPacket> packets_db = new Dictionary<short, SPacket>();

			// [0x4E21] 20001 -> Game Server Connect (Game Server)
			packets_db.Add(0x214E, new SPacket() { lenght = 307, func = ServerPacketHandler.parse_ServerRegister, pos = new short[] { 0, 1, 21, 277, 292, 294 } });
			// [0x4E22] 20002 -> Response to Game Server Connect (Login-Server)
			packets_db.Add(0x4E22, new SPacket() { lenght = 9 });

			return packets_db;
		}

		internal enum LoginResult
		{
			LOGINRESULT_FAIL = 0x1,
			LOGINRESULT_FAIL2 = 0x100,
			LOGINRESULT_SUCCESS = 0x10000
		}
	}
}
