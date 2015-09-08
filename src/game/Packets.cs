// Copyright (c) Tartarus Dev Team, licensed under GNU GPL.
// See the LICENSE file
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using common;

namespace game
{
	public class Packets
	{
		public delegate void PacketAction(Player player, ref PacketStream stream, short[] pos);
		public delegate void ServerPacketAction(AuthServer server, ref PacketStream stream, short[] pos);

		public struct Packet
		{
			public PacketAction func;
			public short[] pos;
		}

		public struct SPacket
		{
			public ServerPacketAction func;
			public short[] pos;
		}

		public static Dictionary<short, Packet> LoadClientPackets()
		{
			Dictionary<short, Packet> packets_db = new Dictionary<short, Packet>();
			// [0x0001] 0001 -> Join Game
			packets_db.Add(0x0001, new Packet() { func = ClientPacketHandler.parse_JoinGame, pos = new short[] { 0 } });
			// [0x0002] 0002 -> Unknown
			//packets_db.Add(0x0001, new Packet() { func = ClientPacketHandler.parse_JoinGame, pos = new short[] { 0 });

			// [0x0005] 0005 -> PC Move Request
			packets_db.Add(0x0005, new Packet() { func = ClientPacketHandler.parse_PCMoveRequest, pos = new short[] { 0, 4, 8, 12, 16, 17, 19, 23 }});
			// [0x0007] 0007 -> PC Move Update
			packets_db.Add(0x0007, new Packet() { func = ClientPacketHandler.parse_PCMoveUpdate, pos = new short[] { 0, 4, 8, 12, 16 }});

			// [0x0014] 0020 -> Client Command
			packets_db.Add(0x0014, new Packet() { func = ClientPacketHandler.parse_ClientCommand, pos = new short[] { 22, 24 } });

			// [0x0017] 0023 -> Logout to Char
			packets_db.Add(0x0017, new Packet() { func = ClientPacketHandler.parse_PCLogoutToCharScreen, pos = new short[0] });
			// [0x0019] 0025 -> Logout Check
			packets_db.Add(0x0019, new Packet() { func = ClientPacketHandler.parse_PCLogoutToCharCheck, pos = new short[0] });
			// [0x001A] 0026 -> Quit Check
			packets_db.Add(0x001A, new Packet() { func = ClientPacketHandler.parse_PCQuitGameCheck, pos = new short[0] });
			// [0x001B] 0027 -> Quit
			packets_db.Add(0x001B, new Packet() { func = ClientPacketHandler.parse_PCQuitGame, pos = new short[0] });

			// [0x00C8] 0200 -> Equip Item
			packets_db.Add(0x00C8, new Packet() { func = ClientPacketHandler.parse_Equip, pos = new short[] { 0, 1 } });
			// [0x00C9] 0201 -> Unequip Item
			packets_db.Add(0x00C9, new Packet() { func = ClientPacketHandler.parse_Unequip, pos = new short[] { 0 } });

			// [0x0192] 0402 -> Learn Skill
			packets_db.Add(0x0192, new Packet() { func = ClientPacketHandler.parse_LearnSkill, pos = new short[] { 0, 4, 8 } });

			// [0x019A] 0410 -> Job LevelUp
			packets_db.Add(0x019A, new Packet() { func = ClientPacketHandler.parse_JobLevelUp, pos = new short[] { 0 } });

			// [0x01FC] 0508 -> Set Property
			packets_db.Add(0x01FC, new Packet() { func = ClientPacketHandler.parse_SetProperty, pos = new short[] { 0, 16 } });
			
			// [0x01FF] 0511 -> Unknown
			packets_db.Add(0x01FF, new Packet() { func = ClientPacketHandler.parse_Packet511, pos = new short[] { 0 } });
			
			// [0x07D1] 2001 -> Character List Request
			packets_db.Add(0x07D1, new Packet() { func = ClientPacketHandler.parse_CharListRequest, pos = new short[] { 0 }});
			// [0x07D2] 2002 -> Create Character
			packets_db.Add(0x07D2, new Packet() { func = ClientPacketHandler.parse_CreateCharacter, pos = new short[] { 0, 4, 8, 12, 16, 20, 24, 28, 40, 52, 169, 188 } });
			// [0x07D3] 2003 -> Delete Character
			packets_db.Add(0x07D3, new Packet() { func = ClientPacketHandler.parse_DeleteCharacter, pos = new short[] { 0 } });
			// [0x07D5] 2005 -> User Join Game Server
			packets_db.Add(0x07D5, new Packet() { func = ClientPacketHandler.parse_UserJoinServer, pos = new short[] { 0, 61 } });
			// [0x07D6] 2006 -> Character Name Check
			packets_db.Add(0x07D6, new Packet() { func = ClientPacketHandler.parse_CharNameCheck, pos = new short[] { 0 } });

			// [0x0BB9] 3001 - Dialog Option
			packets_db.Add(0x0BB9, new Packet() { func = ClientPacketHandler.parse_DialogOption, pos = new short[] { 0, 2 } });
			// [0x0BBA] 3002 - Contact
			packets_db.Add(0x0BBA, new Packet() { func = ClientPacketHandler.parse_Contact, pos = new short[] { 0 } });

			return packets_db;
		}

		public static Dictionary<short, SPacket> LoadServerPackets()
		{
			Dictionary<short, SPacket> packets_db = new Dictionary<short, SPacket>();

			// [0x4E22] 20002 -> Response to Game Server Connect
			packets_db.Add(0x4E22, new SPacket() { func = ServerPacketHandler.parse_ListServerResult, pos = new short[] { 0 } });
			// [0x4E2B] 20011 -> User Join
			packets_db.Add(0x4E2B, new SPacket() { func = ServerPacketHandler.parse_UserJoinNotice, pos = new short[] { 0, 60, 64, 65 } });

			return packets_db;
		}
	}
}
