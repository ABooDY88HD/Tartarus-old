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

		internal static void PacketReceived(Player player, PacketStream stream)
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
			packet_db[PacketId].func(player, ref stream, packet_db[PacketId].pos);

			return;
		}

		#region General Uses

		/// [0x0000] 0000 -> (GC) Sends the result of a packet
		/// <packet id>.W <result>.L <0>.W
		internal static void send_PacketResponse(Player player, short packetId, short response = 0, uint parameter = 0)
		{
			PacketStream data = new PacketStream((short)0x0000);

			data.WriteInt16(packetId);
			data.WriteInt16(response);
			data.WriteUInt32(parameter);

			ClientManager.Instance.Send(player, data);
		}

		#endregion

		#region Character Selection Screen

		/// [0x07D1] 2001 -> Requests the list of characters
		/// <user id>.60S
		internal static void parse_CharListRequest(Player player, ref PacketStream stream, short[] pos)
		{
			string userId = stream.ReadString(pos[0], 60);

			player.SendCharacterList();
		}

		/// [0x07D2] 2002 -> (CS) Requests the creation of a new character
		/// <sex>.L <race>.L <hair id>.L <face id>.L <body id>.L <hands id>.L
		/// <feet id>.L <hair color>.L <face detail>.L <clothes id>.L 
		/// <char name>.19S <skin color>.L
		internal static void parse_CreateCharacter(Player player, ref PacketStream stream, short[] pos)
		{
			int sex = stream.ReadInt32(pos[0]);
			int race = stream.ReadInt32(pos[1]);
			int hairId = stream.ReadInt32(pos[2]);
			int faceId = stream.ReadInt32(pos[3]);
			int bodyId = stream.ReadInt32(pos[4]);
			int handsId = stream.ReadInt32(pos[5]);
			int feetId = stream.ReadInt32(pos[6]);
			int hairColor = stream.ReadInt32(pos[7]);

			int faceDetail = stream.ReadInt32(pos[8]);

			int clothesId = stream.ReadInt32(pos[9]);

			string name = stream.ReadString(pos[10], 19);
			int skinColor = stream.ReadInt32(pos[11]);

			player.CreateCharacter(name, sex, race, hairId, faceId, bodyId, handsId, feetId, hairColor, faceDetail, clothesId, skinColor);
		}

		/// [0x07D3] 2003 -> Requests the deletion of a character
		/// <char name>.19S
		internal static void parse_DeleteCharacter(Player player, ref PacketStream stream, short[] pos)
		{
			string charName = stream.ReadString(pos[0], 19);

			player.DeleteCharacter(charName);
		}

		/// [0x07D4] 2004 -> Sends the list of account characters
		/// <00>.6B <char count>.W { <sex>.L <race>.L <hair id>.L <face id>.L
		/// <body id>.L <hands id>.L <feet id>.L <hair color>.L <00>.Q 
		/// <0x05, 0x00, 0x00, 0x00> <right hand>.L <>.L <armor id>.L
		/// <4>.L <8>.L <12>.L <16>.L <20>.L <24>.L <28>.L <32>.L
		/// <36>.L <40>.L <44>.L <48>.L <52>.L <56>.L <60>.L <64>.L
		/// <68>.L <72>.L <76>.L <80>.L <bag id>.L <level>.L <job>.L
		/// <job level>.L <0>.17B <char name>.19S <skin color>.L
		/// <create date>.30S <delete date>.30S
		/// <0x14>.B <0>7B <0x14>.B
		/// <0>.87B <0x0A>.B <0>.7B <0x0A>.B <0>.7B
		/// <0>.83B <0x01>.B <0>.123B
		internal static void send_CharacterList(Player player, CharacterListEntry[] characters)
		{
			PacketStream data = new PacketStream(0x07D4);
			data.Write(new byte[6], 0, 6);
			data.WriteInt16((short)characters.Length);
			for (short i = 0; i < characters.Length; i++)
			{
				data.WriteInt32(characters[i].Sex);
				data.WriteInt32(characters[i].Race);
				data.WriteInt32(characters[i].HairId);
				data.WriteInt32(characters[i].FaceId);
				data.WriteInt32(characters[i].BodyId);
				data.WriteInt32(characters[i].HandsId);
				data.WriteInt32(characters[i].FeetId);
				data.WriteInt32(characters[i].HairColor);

				data.Write(new byte[8], 0, 8);
				data.Write(new byte[4] { 0x05, 0x00, 0x00, 0x00 }, 0, 4);
				data.WriteInt32(characters[i].Equip[(int)Item.WearType.RightHand]); // Right Hand Item
				data.WriteInt32(characters[i].Equip[(int)Item.WearType.LeftHand]); // 
				data.WriteInt32(characters[i].Equip[(int)Item.WearType.Armor]); // Armor ID
				data.WriteInt32(characters[i].Equip[(int)Item.WearType.Helm]); // 4
				
				int temp = characters[i].Equip[(int)Item.WearType.Glove];
				if (temp == 0)
					temp = characters[i].HandsId;
				data.WriteInt32(temp); // 8
				
				temp = characters[i].Equip[(int)Item.WearType.Boots];
				if (temp == 0)
					temp = characters[i].FeetId;
				data.WriteInt32(temp); // 12
				
				data.WriteInt32(characters[i].Equip[(int)Item.WearType.Belt]); // 16
				data.WriteInt32(characters[i].Equip[(int)Item.WearType.Mantle]); // 20
				data.WriteInt32(characters[i].Equip[(int)Item.WearType.Necklace]); // 24
				data.WriteInt32(characters[i].Equip[(int)Item.WearType.Ring]); // 28
				data.WriteInt32(characters[i].Equip[(int)Item.WearType.SecondRing]); // 32
				data.WriteInt32(characters[i].Equip[(int)Item.WearType.Ear]); // 36
				data.WriteInt32(characters[i].Equip[(int)Item.WearType.Face]); // 40
				data.WriteInt32(0); // 44
				data.WriteInt32(0); // 48
				data.WriteInt32(characters[i].Equip[(int)Item.WearType.DecoShield]); // 52
				data.WriteInt32(characters[i].Equip[(int)Item.WearType.DecoArmor]); // 56
				data.WriteInt32(characters[i].Equip[(int)Item.WearType.DecoHelm]); // 60
				data.WriteInt32(characters[i].Equip[(int)Item.WearType.DecoGlove]); // 64
				data.WriteInt32(characters[i].Equip[(int)Item.WearType.DecoBoots]); // 68
				data.WriteInt32(characters[i].Equip[(int)Item.WearType.DecoMantle]); // 72
				data.WriteInt32(characters[i].Equip[(int)Item.WearType.DecoShoulder]); // 76
				data.WriteInt32(characters[i].Equip[(int)Item.WearType.RideItem]); // 80
				data.WriteInt32(characters[i].Equip[(int)Item.WearType.BagSlot]); // Bag ID
				data.WriteInt32(characters[i].Level); // Level
				data.WriteInt32(characters[i].Job); // Job
				data.WriteInt32(characters[i].JobLevel); // Job Level
				data.Write(new byte[17], 0, 17);
				data.WriteString(characters[i].Name, 19);
				data.WriteInt32(characters[i].SkinColor);
				data.WriteString(TimeUtils.GetDateTime(characters[i].CreateDate).ToString("yyyy/MM/dd"), 30); // Create Date
				// Deleted characters doesn't even get to this function
				// so there's no point in sending a different delete date
				data.WriteString("9999/12/31", 30); // Delete Date
				//data.Write(new byte[312], 0, 312);
				// Unknown
				data.Write(new byte[1] { 0x14 }, 0, 1);
				data.Write(new byte[7], 0, 7);
				data.Write(new byte[1] { 0x14 }, 0, 1);
				data.Write(new byte[87], 0, 87);
				data.Write(new byte[1] { 0x0A }, 0, 1);
				data.Write(new byte[7], 0, 7);
				data.Write(new byte[1] { 0x0A }, 0, 1);
				data.Write(new byte[83], 0, 83);
				data.Write(new byte[1] { 0x01 }, 0, 1);
				data.Write(new byte[123], 0, 123);
			}

			ClientManager.Instance.Send(player, data);
		}

		/// [0x07D5] 2005 -> (CG) Sent by the client to join the game server
		/// <user id>.60S <00>.B <auth token>8B
		internal static void parse_UserJoinServer(Player player, ref PacketStream stream, short[] pos)
		{
			string userId = stream.ReadString(pos[0], 60);
			byte[] key = stream.ReadBytes(pos[1], 8);

			Server.OnUserJoin(player, userId, key);
		}

		/// [0x07D6] 2006 -> (CG) Checks if a name is available
		/// <new name>.19S
		internal static void parse_CharNameCheck(Player player, ref PacketStream stream, short[] pos)
		{
			string charName = stream.ReadString(pos[0], 19);
			player.CharNameExists(charName);
		}

		#endregion

		#region PC Movement
		
		/// [0x0005] 5 - (CG) Requests to move from a position to another
		/// <player handle>.L <from x>.L <from y>.L <dest x>.L <dest y>.L
		internal static void parse_PCMoveRequest(Player player, ref PacketStream stream, short[] pos)
		{
			uint handle = stream.ReadUInt32(pos[0]);
			float fromX = stream.ReadFloat(pos[1]);
			float fromY = stream.ReadFloat(pos[2]);
			uint time = stream.ReadUInt32(pos[3]);
			byte speedSync = stream.ReadByte(pos[4]);
			short moveCount = stream.ReadInt16(pos[5]);

			Point[] movePos = new Point[moveCount];
			for (int i = 0; i < moveCount; i++)
			{
				movePos[i] = new Point();
				movePos[i].X = stream.ReadFloat(pos[6] + 8*i);
				movePos[i].Y = stream.ReadFloat(pos[7] + 8*i);
			}

			RegionMngr.PcWalkCheck(player, fromX, fromY, movePos);
		}

		/// [0x0008] 8 - (GC) Sends data to make character move
		/// <time>.L <player handle>.L <layer>.B <move speed>.B 
		/// <point count>.W { <to x>.L <to y>.L }*count
		internal static void send_PCMoveTo(Player player, Point[] movePoints)
		{
			PacketStream res = new PacketStream((short)0x0008);

			byte[] b = new byte[]
			{
				0x5E, 0x1F, 0x00, 0x00, 
			};
			res.Write(b, 0, b.Length);

			res.WriteUInt32(player.Handle);

			b = new byte[] {
				0x01, 0x11
			};
			res.Write(b, 0, b.Length);

			res.WriteInt16((short)movePoints.Length);
			for (int i = 0; i < movePoints.Length; i++)
			{
				res.WriteFloat(movePoints[i].X);
				res.WriteFloat(movePoints[i].Y);
			}

			ClientManager.Instance.Send(player, res);
		}

		/// [0x0007] 7 - (CG) Tells where the player currently is while moving
		/// <player handle>.L <current x>.L <current y>.L <stop>.B
		internal static void parse_PCMoveUpdate(Player player, ref PacketStream stream, short[] pos)
		{
			uint handle = stream.ReadUInt32(pos[0]);
			float curX = stream.ReadFloat(pos[1]);
			float curY = stream.ReadFloat(pos[2]);
			//int unknown = stream.ReadInt32(pos[3]);
			bool stop = stream.ReadBool(pos[4]);

			RegionMngr.UpdatePCPos(player, curX, curY, stop);
		}

		#endregion

		/// [0x0001] 1 - (CG) Request to join the game with a character
		/// <char name>.19S
		internal static void parse_JoinGame(Player player, ref PacketStream stream, short[] pos)
		{
			string charName = stream.ReadString(pos[0], 19);
			Server.OnUserJoinGame(player, charName);
		}

		/// [0x2329] 9001 - (GC) Send a list of URLs
		/// <str length>.W <url list>.(str length)B
		internal static void send_UrlList(Player player, string urlList)
		{
			PacketStream data = new PacketStream((short)0x2329);

			data.WriteInt16((short)urlList.Length);
			data.WriteString(urlList, urlList.Length);

			ClientManager.Instance.Send(player, data);
		}

		/// [0x01FB] 507 - (GC) Send the value of a property
		/// <player handle>.L <as int>.B <property name>.16S <value/0>.L <0>.L [str value].?S
		/// For a properties list check the full doc at "docs/packets/Game/"
		internal static void send_Property(Player player, string property, long value, bool asString = false, string sValue = "")
		{
			PacketStream data = new PacketStream((short)0x01FB);

			data.WriteUInt32(player.Handle);
			if (asString)
				data.WriteByte(0x00);
			else
				data.WriteByte(0x01);

			data.WriteString(property, 16);
			data.WriteInt64(value);

			if (asString)
			{
				data.WriteString(sValue);
				data.WriteByte(0x00);
			}

			ClientManager.Instance.Send(player, data);
		}

		internal static void send_LoginResult(Player player)
		{
			PacketStream data = new PacketStream((short)0x0004);

			data.WriteInt16(0);
			data.WriteUInt32(player.Handle);
			data.WriteFloat(player.Position.X);
			data.WriteFloat(player.Position.Y);

			data.WriteFloat(0f); // Z
			data.WriteByte((byte)player.Layer);
			data.WriteFloat(180f);//player.Chara.Face);
			data.WriteInt32(RegionMngr.RegionSize);
			data.WriteInt32(player.Hp);
			data.WriteInt32(player.Mp);
			data.WriteInt32(player.MaxHp);
			data.WriteInt32(player.MaxMp);
			data.WriteInt32(player.Havoc);
			data.WriteInt32(Globals.MaxHavoc);

			data.WriteInt32(player.Sex);
			data.WriteInt32(player.Race);
			data.WriteInt32(player.SkinColor);
			data.WriteInt32(player.FaceId);
			data.WriteInt32(player.HairId);
			data.WriteString(player.Name, 19);

			data.WriteInt32(Globals.CellSize);
			data.WriteInt32(player.GuildId);

			ClientManager.Instance.Send(player, data);
		}

		internal static void send_InventoryList(Player player)
		{
			PacketStream data = new PacketStream((short)0x00CF);

			data.WriteInt16((short)player.Inventory.Count);
			int i = 0;
			foreach (Item item in player.Inventory.Values)
			{
				data.WriteUInt32(item.Handle);
				data.WriteInt32(item.Code);
				data.WriteInt64(item.UId);
				data.WriteInt64(item.Count);
				data.WriteInt32(0);
				data.WriteInt32(0);
				data.WriteByte(item.Enhance);
				data.WriteByte(item.Level);
				data.Write(new byte[81], 0, 81);
				data.WriteInt32((int)item.WearInfo);
				data.Write(new byte[2], 0, 2);
				data.WriteInt32(i); // IdX
				i++;
			}

			ClientManager.Instance.Send(player, data);
		}

		internal static void send_EquipSummon(Player player)
		{
			PacketStream data = new PacketStream((short)0x012F);

			// TODO 
			data.Write(
				new byte[] {
					0x00, 0x00, 0x00, 0x00,
					0x00, 0x00, 0x00, 0x00,
					0x00, 0x00, 0x00, 0x00,
					0x00, 0x00, 0x00, 0x00,
					0x00, 0x00, 0x00, 0x00,
					0x00, 0x00, 0x00, 0x00,
					0x00,
				}, 0, 25
			);

			ClientManager.Instance.Send(player, data);
		}

		// [0x00CA] 202 -> Character View
		internal static void send_CharacterView(Player player)
		{
			PacketStream data = new PacketStream((short)0x00CA);

			data.WriteUInt32(player.Handle);

			data.WriteInt32(player.GetViewId(Item.WearType.RightHand));
			data.WriteInt32(player.GetViewId(Item.WearType.LeftHand));
			data.WriteInt32(player.GetViewId(Item.WearType.Armor));
			data.WriteInt32(player.GetViewId(Item.WearType.Helm));

			data.WriteInt32(player.GetViewId(Item.WearType.Glove));
			data.WriteInt32(player.GetViewId(Item.WearType.Boots));
			data.WriteInt32(player.GetViewId(Item.WearType.Belt));
			data.WriteInt32(player.GetViewId(Item.WearType.Mantle));
			data.WriteInt32(player.GetViewId(Item.WearType.Necklace));
			data.WriteInt32(player.GetViewId(Item.WearType.Ring));
			data.WriteInt32(player.GetViewId(Item.WearType.SecondRing));
			data.WriteInt32(player.GetViewId(Item.WearType.Ear));
			data.WriteInt32(player.GetViewId(Item.WearType.Face));
			data.WriteInt32(0); // Unknown
			data.WriteInt32(0); // Unknown
			data.WriteInt32(player.GetViewId(Item.WearType.DecoShield));
			data.WriteInt32(player.GetViewId(Item.WearType.DecoArmor));
			data.WriteInt32(player.GetViewId(Item.WearType.DecoHelm));
			data.WriteInt32(player.GetViewId(Item.WearType.DecoGlove));
			data.WriteInt32(player.GetViewId(Item.WearType.DecoBoots));
			data.WriteInt32(player.GetViewId(Item.WearType.DecoMantle));
			data.WriteInt32(player.GetViewId(Item.WearType.DecoShoulder));
			data.WriteInt32(player.GetViewId(Item.WearType.RideItem));
			data.WriteInt32(player.GetViewId(Item.WearType.BagSlot));

			byte[] b = new byte[]
			{
				0x14, 0x00, 0x00, 0x00, // ??
				0x00, 0x00, 0x00, 0x00,
				0x14, 0x00, 0x00, 0x00, // ??
				0x00, 0x00, 0x00, 0x00,
			};
			data.Write(b, 0, b.Length);

			data.Write(new byte[80], 0, 80);

			b = new byte[]
			{
				0x0A, 0x00, 0x00, 0x00, // ??
				0x00, 0x00, 0x00, 0x00,
				0x0A, 0x00, 0x00, 0x00, // ??
				0x00, 0x00, 0x00, 0x00,
			};
			data.Write(b, 0, b.Length);

			data.Write(new byte[] { 0x01 }, 0, 1);
			data.Write(new byte[75], 0, 75);
			data.Write(new byte[] { 0x01 }, 0, 1);
			data.Write(new byte[123], 0, 123);

			ClientManager.Instance.Send(player, data);
		}

		// TODO : 
		internal static void send_Packet625(Player player)
		{
			PacketStream data = new PacketStream((short)0x0271);

			byte[] b = new byte[] {
				0x00, 0x00
			};

			data.Write(b, 0, b.Length);

			ClientManager.Instance.Send(player, data);
		}
		// TODO : 
		internal static void send_Packet626(Player player)
		{
			PacketStream data = new PacketStream((short)0x0272);

			byte[] b = new byte[] {
				0x01, 0x00, 0x11, 0x5C,
				0x89, 0x00, 0x01, 0x00,
				0x00, 0x00, 0x00, 0x00,
				0x00, 0x00,
			};

			data.Write(b, 0, b.Length);
			ClientManager.Instance.Send(player, data);
		}
		// TODO : 
		internal static void send_Packet627(Player player)
		{
			PacketStream data = new PacketStream((short)0x0273);

			byte[] b = new byte[] {
				0x00, 0x00, 0x00, 0x00,
			};

			data.Write(b, 0, b.Length);

			ClientManager.Instance.Send(player, data);
		}
		// TODO : 
		internal static void send_Packet629(Player player)
		{
			PacketStream data = new PacketStream((short)0x0275);

			data.WriteUInt32(player.Handle);
			byte[] b = new byte[] {
				0x00, 0x00, 0x00, 0x00
			};

			data.Write(b, 0, b.Length);

			ClientManager.Instance.Send(player, data);
		}
		// TODO : 
		internal static void send_Packet631(Player player, int p)
		{
			PacketStream data = new PacketStream((short)0x0277);
			byte[] b;
			switch (p)
			{
				case 1:
					b = new byte[] {
						0x00, 0x00, 0x00, 0x00,
						0x00, 0x00, 0x00, 0x00
					};
					break;

				case 2:
					b = new byte[] {
						0x01, 0x00, 0x00, 0x00,
						0x00, 0x00, 0x00, 0x00
					};
					break;

				case 3:
					b = new byte[] {
						0x02, 0x00, 0x00, 0x00,
						0x00, 0x00, 0x00, 0x00
					};
					break;

				case 4:
					b = new byte[] {
						0x03, 0x00, 0x00, 0x00,
						0x00, 0x00, 0x00, 0x00
					};
					break;

				case 5:
					b = new byte[] {
						0x04, 0x00, 0x00, 0x00,
						0x00, 0x00, 0x00, 0x00
					};
					break;

				default:
					return;
			}

			data.Write(b, 0, b.Length);
			ClientManager.Instance.Send(player, data);
		}

		/// [1000]
		internal static void send_UpdateStats(Player player, bool isTemp)
		{
			PacketStream res = new PacketStream((short)0x03E8);
			res.WriteUInt32(player.Handle);

			if (!isTemp)
			{
				res.WriteInt16(player.BaseStats.JobID);
				res.WriteInt16(player.BaseStats.Strength);
				res.WriteInt16(player.BaseStats.Vitality);
				res.WriteInt16(player.BaseStats.Dexterity);
				res.WriteInt16(player.BaseStats.Agility);
				res.WriteInt16(player.BaseStats.Intelligence);
				res.WriteInt16(player.BaseStats.Wisdom);
				res.WriteInt16(player.BaseStats.Luck);

				res.WriteInt16(player.BaseStats.CritPower);
				res.WriteInt16(player.BaseStats.CritRate);
				res.WriteInt16(player.BaseStats.PAtkRight);
				res.WriteInt16(player.BaseStats.PAtkLeft);
				res.WriteInt16(player.BaseStats.PDef);
				res.WriteInt16(player.BaseStats.BlockDef);
				res.WriteInt16(player.BaseStats.MAtk);
				res.WriteInt16(player.BaseStats.MDef);
				res.WriteInt16(player.BaseStats.AccuracyRight);
				res.WriteInt16(player.BaseStats.AccuracyLeft);
				res.WriteInt16(player.BaseStats.MAccuracy);
				res.WriteInt16(player.BaseStats.Evasion);
				res.WriteInt16(player.BaseStats.MRes);
				res.WriteInt16(player.BaseStats.BlockPer);

				res.WriteInt16(player.BaseStats.MovSpd);
				res.WriteInt16(player.BaseStats.AtkSpd);

				res.WriteInt16(60);
				res.WriteInt16(player.BaseStats.MaxWeight);
				res.WriteInt16(0);
				res.WriteInt16(player.BaseStats.CastSpd);
				res.WriteInt16(player.BaseStats.ReCastSpd);
				res.WriteInt16(2);

				res.WriteInt16(player.BaseStats.HPRegen);
				res.WriteInt16(player.BaseStats.HPRecov);
				res.WriteInt16(player.BaseStats.MPRegen);
				res.WriteInt16(player.BaseStats.MPRecov);

				res.WriteInt16(player.BaseStats.PerfBlock);
				res.WriteInt16(player.BaseStats.MIgnore);
				res.WriteInt16(player.BaseStats.MIgnorePer);
				res.WriteInt16(player.BaseStats.PIgnore);
				res.WriteInt16(player.BaseStats.PIgnorePer);
				res.WriteInt16(player.BaseStats.MPierce);
				res.WriteInt16(player.BaseStats.MPiercePer);
				res.WriteInt16(player.BaseStats.PPierce);
				res.WriteInt16(player.BaseStats.PPiercePer);
			}
			else
			{
				res.WriteInt16(0);
				res.WriteInt16(player.SCStats.Strength);
				res.WriteInt16(player.SCStats.Vitality);
				res.WriteInt16(player.SCStats.Dexterity);
				res.WriteInt16(player.SCStats.Agility);
				res.WriteInt16(player.SCStats.Intelligence);
				res.WriteInt16(player.SCStats.Wisdom);
				res.WriteInt16(player.SCStats.Luck);

				res.WriteInt16(player.SCStats.CritPower);
				res.WriteInt16(player.SCStats.CritRate);
				res.WriteInt16(player.SCStats.PAtkRight);
				res.WriteInt16(player.SCStats.PAtkLeft);
				res.WriteInt16(player.SCStats.PDef);
				res.WriteInt16(player.SCStats.BlockDef);
				res.WriteInt16(player.SCStats.MAtk);
				res.WriteInt16(player.SCStats.MDef);
				res.WriteInt16(player.SCStats.AccuracyRight);
				res.WriteInt16(player.SCStats.AccuracyLeft);
				res.WriteInt16(player.SCStats.MAccuracy);
				res.WriteInt16(player.SCStats.Evasion);
				res.WriteInt16(player.SCStats.MRes);
				res.WriteInt16(player.SCStats.BlockPer);

				res.WriteInt16(player.SCStats.MovSpd);
				res.WriteInt16(player.SCStats.AtkSpd);

				res.WriteInt16(60);
				res.WriteInt16(player.SCStats.MaxWeight);
				res.WriteInt16(0);
				res.WriteInt16(player.SCStats.CastSpd);
				res.WriteInt16(player.SCStats.ReCastSpd);
				res.WriteInt16(2);

				res.WriteInt16(player.SCStats.HPRegen);
				res.WriteInt16(player.SCStats.HPRecov);
				res.WriteInt16(player.SCStats.MPRegen);
				res.WriteInt16(player.SCStats.MPRecov);

				res.WriteInt16(player.SCStats.PerfBlock);
				res.WriteInt16(player.SCStats.MIgnore);
				res.WriteInt16(player.SCStats.MIgnorePer);
				res.WriteInt16(player.SCStats.PIgnore);
				res.WriteInt16(player.SCStats.PIgnorePer);
				res.WriteInt16(player.SCStats.MPierce);
				res.WriteInt16(player.SCStats.MPiercePer);
				res.WriteInt16(player.SCStats.PPierce);
				res.WriteInt16(player.SCStats.PPiercePer);
			}

			res.WriteBool(isTemp);
			ClientManager.Instance.Send(player, res);
		}
		
		// TODO : 
		internal static void send_Packet4700(Player player)
		{
			PacketStream res = new PacketStream((short)0x125C);

			byte[] b = new byte[12];

			res.Write(b, 0, b.Length);

			ClientManager.Instance.Send(player, res);
		}
		// TODO : 
		internal static void send_Packet1005(Player player)
		{
			PacketStream res = new PacketStream((short)0x03ED);
			res.WriteUInt32(player.Handle);
			byte[] b = new byte[] {
				0x00, 0x00, 0x00, 0x00
			};

			res.Write(b, 0, b.Length);

			ClientManager.Instance.Send(player, res);
		}

		// TODO : 
		internal static void send_Packet404(Player player)
		{
			PacketStream res = new PacketStream((short)0x0194);
			//res.WriteUInt32(NetworkManager.sid2handle(sid));
			res.WriteUInt32(player.Handle);
			byte[] b = new byte[] {
				0x00, 0x00
			};

			res.Write(b, 0, b.Length);

			ClientManager.Instance.Send(player, res);
		}

		// [0x03E9] 01001 -> Update Gold and Chaos
		internal static void send_UpdateGoldChaos(Player player)
		{
			PacketStream data = new PacketStream((short)0x03E9);

			data.WriteInt64(player.Gold);
			data.WriteInt32(player.Chaos);

			ClientManager.Instance.Send(player, data);
		}

		// [0x03EA] 1002 -> Update Level
		internal static void send_UpdateLevel(Player player)
		{
			PacketStream data = new PacketStream((short)0x03EA);

			data.WriteUInt32(player.Handle);
			data.WriteInt32(player.Level);
			data.WriteInt32(player.JobLevel);

			ClientManager.Instance.Send(player, data);
		}

		// [0x03EB] 1003 -> Update EXP
		internal static void send_UpdateExp(Player player)
		{
			PacketStream data = new PacketStream((short)0x03EB);

			data.WriteUInt32(player.Handle);
			data.WriteInt64(player.Exp);
			data.WriteInt64(player.JP);

			ClientManager.Instance.Send(player, data);
		}

		// [0x00D8] 216 -> Belt Slot Info
		internal static void send_BeltSlotInfo(Player player)
		{
			PacketStream data = new PacketStream((short)0x00D8);

			// TODO : This must use belts handles
			for (int i = 0; i < 6; i++)
			{
				data.WriteUInt32(0);
			}

			ClientManager.Instance.Send(player, data);
		}

		// [0x044D] 01101 -> Game Time
		internal static void send_GameTime(Player player)
		{
			PacketStream data = new PacketStream((short)0x044D);

			data.WriteInt32(Environment.TickCount/10);
			data.WriteInt64(TimeUtils.GetTimeStamp(DateTime.Now));

			ClientManager.Instance.Send(player, data);
		}

		// [0x01F4] 500 -> Entity State
		internal static void send_EntityState(Player player)
		{
			PacketStream data = new PacketStream((short)0x01F4);
			data.WriteUInt32(player.Handle);
			byte[] b = new byte[] {
				0x00, 0x00, 0x00, 0x00
			};

			data.Write(b, 0, b.Length);

			ClientManager.Instance.Send(player, data);
		}

		// [0x0258] 0600 -> Quest List
		internal static void send_QuestList(Player player)
		{
			PacketStream data = new PacketStream((short)0x0258);

			byte[] b = new byte[] {
				0x00, 0x00, 0x00
			};

			data.Write(b, 0, b.Length);

			ClientManager.Instance.Send(player, data);
		}

		internal static void send_LocationInfo(Player player)
		{
			PacketStream data = new PacketStream((short)0x0385);

			data.WriteInt32(1001001);
			data.WriteInt32(1001001);

			ClientManager.Instance.Send(player, data);
		}

		internal static void send_WeatherInfo(Player player)
		{
			PacketStream data = new PacketStream((short)0x0386);

			data.WriteInt32(1001001);
			data.WriteInt16(0);

			ClientManager.Instance.Send(player, data);
		}

		/// [0x0017] 23 -> (CG) Player Logout
		internal static void parse_PCLogoutToCharScreen(Player player, ref PacketStream stream, short[] pos)
		{
			// TODO : Logout conditions
			send_PacketResponse(player, 0x0017, 0);
		}

		/// [0x0019] 25 -> (CG) Player Logout Check
		internal static void parse_PCLogoutToCharCheck(Player player, ref PacketStream stream, short[] pos)
		{
			// TODO : Logout conditions
			send_PacketResponse(player, 0x0019, 0);
		}

		/// [0x001A] 26 -> (CG) Player Quit Check
		internal static void parse_PCQuitGameCheck(Player player, ref PacketStream stream, short[] pos)
		{
			// TODO : Logout conditions
			send_PacketResponse(player, 0x001A, 0);
		}

		/// [0x001B] 27 -> (CG) Player Quit
		internal static void parse_PCQuitGame(Player player, ref PacketStream stream, short[] pos)
		{
			// TODO : Logout conditions
		}

		/// [0x01FC] 508 -> (CG) Set Property
		/// <property name>.16B <property value>.?S
		internal static void parse_SetProperty(Player player, ref PacketStream stream, short[] pos)
		{
			string propertyName = stream.ReadString(pos[0], 16);
			string propertyValue = stream.ReadString(pos[1], (stream.GetSize() - 23));

			player.SetProperty(propertyName, propertyValue);
		}

		/// [0x00C9] 201 -> (CG) Unequip Item
		/// <wear type>.L
		internal static void parse_Unequip(Player player, ref PacketStream stream, short[] pos)
		{
			int wearType = stream.ReadInt32(pos[0]);

			player.UnequipItem(wearType);
		}

		/// [0x011F] -> (GC) Wear Change
		/// <item handle>.L <wear info>.W <player handle>.L <item enhance>.B
		/// If wear info is -1, unequip item
		internal static void send_WearChange(Player player, uint handle, short wearType, byte enhance)
		{
			PacketStream data = new PacketStream(0x011F);

			data.WriteUInt32(handle);
			data.WriteInt16(wearType);
			data.WriteUInt32(player.Handle);
			data.WriteByte(enhance);
			data.Write(new byte[8], 0, 8);

			ClientManager.Instance.Send(player, data);
		}

		internal static void parse_Equip(Player player, ref PacketStream stream, short[] pos)
		{
			byte wearType = stream.ReadByte(pos[0]);
			uint handle = stream.ReadUInt32(pos[1]);

			player.EquipItem(wearType, handle);
		}

		/// [0x000B] 11 -> (GC) Updates current player region
		/// <region x>.L <region y>.L
		internal static void send_RegionAck(Player player, uint rx, uint ry)
		{
			PacketStream data = new PacketStream(0x000B);
			
			data.WriteUInt32(rx);
			data.WriteUInt32(ry);

			ClientManager.Instance.Send(player, data);
		}

		/// [0x0003]  3 -> (GC) Informs that an entitity entered
		///					player surroundings
		///	<main type>.B <object handle>.L <x>.L <y>.L <z>.L <layer>.B
		///	<sub type>.B <extra data>.?B
		///	Read the full documentation for extra data values
		internal static void send_EntityAck(Player player, GameObject objData)
		{
			PacketStream data = new PacketStream(0x0003);

			data.WriteByte((byte)objData.Type);
			data.WriteUInt32(objData.Handle);
			data.WriteFloat(objData.Position.X);
			data.WriteFloat(objData.Position.Y);
			data.WriteFloat(0); // Z
			data.WriteByte(objData.Layer);
			data.WriteByte((byte)objData.SubType);

			switch (objData.Type)
			{
				case GameObjectType.Creature:
					{
						CreatureObject co = (CreatureObject)objData;
						data.WriteUInt32(co.Status);
						data.WriteFloat(co.FaceDirection);
						data.WriteInt32(co.Hp);
						data.WriteInt32(co.MaxHp);
						data.WriteInt32(co.Mp);
						data.WriteInt32(co.MaxMp);
						data.WriteInt32(co.Level);
						data.WriteByte((byte)co.Race);
						data.WriteUInt32(co.SkinColor);
						data.WriteBool(false);
						data.WriteInt32(co.Energy);

						switch (co.SubType)
						{
							case GameObjectSubType.Player:
								{
									Player pl = (Player)co;
									data.WriteByte((byte)pl.Sex);
									data.WriteInt32(pl.FaceId);
									data.WriteInt32(0); // TODO : FaceTextureID
									data.WriteInt32(pl.HairId);
									data.WriteString(pl.Name, 19);
									data.WriteUInt16((ushort)pl.Job);
									data.WriteUInt32(0); // TODO : Ride Handle
									data.WriteInt32(pl.GuildId);
								}
								break;

							case GameObjectSubType.NPC:
								{
									Npc n = (Npc)co;
									EncryptedInt crInt = new EncryptedInt(n.Id);
									crInt.WriteToPacket(data);
								}
								break;

							case GameObjectSubType.Mob:
								{
									//Npc n = (Npc)co;
									//EncryptedInt crInt = new EncryptedInt(n.Id);
									//crInt.WriteToPacket(data);
								}
								break;

							case GameObjectSubType.Summon:
								{
									//Npc n = (Npc)co;
									//EncryptedInt crInt = new EncryptedInt(n.Id);
									//crInt.WriteToPacket(data);
								}
								break;

							case GameObjectSubType.Pet:
								{
									//Npc n = (Npc)co;
									//EncryptedInt crInt = new EncryptedInt(n.Id);
									//crInt.WriteToPacket(data);
								}
								break;

							default:
								ConsoleUtils.Write(ConsoleMsgType.Error, "Unhandled Entity SubType\n");
								break;
						}
					}
					break;

				case GameObjectType.StaticObject:
					{
						switch (objData.SubType)
						{
							case GameObjectSubType.Item:
								{

								}
								break;

							case GameObjectSubType.SkillProp:
								{

								}
								break;

							case GameObjectSubType.FieldProp:
								{

								}
								break;

							default:
								ConsoleUtils.Write(ConsoleMsgType.Error, "Unhandled Entity SubType\n");
								break;
						}
					}
					break;

				default:
					ConsoleUtils.Write(ConsoleMsgType.Error, "Unhandled Entity SubType\n");
					break;
			}

			ClientManager.Instance.Send(player, data);
		}

		///
		public static void send_EntityState(Player player, uint handle, uint state)
		{
			PacketStream data = new PacketStream(0x01F4);

			data.WriteUInt32(handle);
			data.WriteUInt32(state);

			ClientManager.Instance.Send(player, data);
		}

		///
		public static void send_Packet516(Player player, uint handle)
		{
			PacketStream data = new PacketStream(0x0204);
			// TODO 
			data.WriteUInt32(handle);
			data.WriteInt32(0);
			data.WriteInt32(19);
			data.WriteInt32(20);
			data.WriteInt32(20);

			ClientManager.Instance.Send(player, data);
		}

		internal static void parse_Packet511(Player player, ref PacketStream stream, short[] pos)
		{
			return;
		}

		internal static void parse_DialogOption(Player player, ref PacketStream stream, short[] pos)
		{
			string function = stream.ReadString(pos[1], stream.ReadInt16(pos[0]));

			Script.LuaMain.DoFunction(player, function);
		}

		internal static void parse_Contact(Player player, ref PacketStream stream, short[] pos)
		{
			uint npc_handle = stream.ReadUInt32(pos[0]);
			player.ContactHandle = npc_handle;

			Script.LuaMain.Contact(player, npc_handle);
		}

		internal static void send_Dialog(Player player, Npc.DialogType dialogType, Script.DialogData dialData)
		{
			PacketStream data = new PacketStream(0x0BB8);

			data.WriteInt32((int)dialogType);
			data.WriteUInt32(player.ContactHandle);
			data.WriteInt16((short)dialData.Title.Length);
			data.WriteInt16((short)dialData.Messsage.Length);

			StringBuilder menu = new StringBuilder();
			for (int i = 0; i < dialData.Options.Count; i++)
			{
				menu.Append((char)0x09);
				menu.Append(dialData.Options[i]);
				menu.Append((char)0x09);
				menu.Append(dialData.Functions[i]);
				menu.Append((char)0x09);
			}

			data.WriteInt16((short)menu.Length);
			data.WriteString(dialData.Title);
			data.WriteString(dialData.Messsage);
			data.WriteString(menu.ToString());

			ClientManager.Instance.Send(player, data);
		}


		internal static void parse_ClientCommand(Player player, ref PacketStream stream, short[] pos)
		{
			short size = stream.ReadInt16(pos[0]);
			string cmd = stream.ReadString(pos[1], size);
			// TODO : Action
		}

		internal static void parse_LearnSkill(Player player, ref PacketStream stream, short[] pos)
		{
			uint handle = stream.ReadUInt32(pos[0]);
			int skillId = stream.ReadInt32(pos[1]);
			byte targetLevel = stream.ReadByte(pos[2]);

			Skill.LevelUp(player, skillId, targetLevel);
		}

		internal static void send_SkillList(Player player, Skill[] skills)
		{
			PacketStream stream = new PacketStream(0x0193);

			stream.WriteUInt32(player.Handle);
			stream.WriteInt16((short)skills.Length);
			stream.WriteByte(0);
			
			for (int i = 0; i < skills.Length; i++)
			{
				stream.WriteInt32(skills[i].Id);
				stream.WriteByte(skills[i].Level);
				stream.WriteByte(skills[i].Level);
				stream.WriteUInt32(skills[i].Cooldown);
				stream.WriteUInt32(0);
			}

			ClientManager.Instance.Send(player, stream);
		}

		internal static void parse_JobLevelUp(Player player, ref PacketStream stream, short[] pos)
		{
			//uint handle = stream.ReadUInt32(pos[0]);

			player.JobLevelUp();
		}

		internal static void send_MaxHPMPUpdate(Player player, int oldHp, int oldMp)
		{
			PacketStream stream = new PacketStream(0x01FD);
			
			stream.WriteUInt32(player.Handle);
			stream.WriteInt32(0);
			stream.WriteInt32(oldHp);
			stream.WriteInt32(player.MaxHp);
			stream.WriteInt32(0);
			stream.WriteInt32(oldMp);
			stream.WriteInt32(player.MaxMp);
			stream.WriteByte(0);

			ClientManager.Instance.Send(player, stream);
		}

		internal static void send_RecoverHPMP(Player player, int hpRec, int mpRec)
		{
			PacketStream stream = new PacketStream(0x0204);

			stream.WriteUInt32(player.Handle);
			stream.WriteInt32(hpRec);
			stream.WriteInt32(mpRec);
			stream.WriteInt32(player.Hp);
			stream.WriteInt32(player.Mp);
		}
	}

	/// <summary>
	/// Creates an encrypted value
	/// of an integer
	/// </summary>
	public class EncryptedInt
	{
		public class HiLo
		{
			public short h;
			public short l;
		}

		public EncryptedInt(int n)
		{
			short r1 = 0;// (short)Globals.GetRandomInt32();
			short r2 = 0;// (short)Globals.GetRandomInt32();

			m_H.h = r1;
			m_H.l = (short)(ByteUtils.HiWord(n) + (2 * (r2 - r1)));
			m_L.h = r2;
			m_L.l = (short)(ByteUtils.LoWord(n) - (2 * (r2 + r1)));
		}

		public HiLo m_H = new HiLo();
		public HiLo m_L = new HiLo();

		public void WriteToPacket(PacketStream packet)
		{
			packet.WriteInt16(m_H.h);
			packet.WriteInt16(m_H.l);
			packet.WriteInt16(m_L.h);
			packet.WriteInt16(m_L.l);
		}
	}
}
