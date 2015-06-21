// Copyright (c) Tartarus Dev Team, licensed under GNU GPL.
// See the LICENSE file
using common;
using common.RC4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using common.Utilities;

namespace game
{
	/// <summary>
	/// Player Races
	/// </summary>
	public enum PCRace
	{
		Deva = 3,
		Gaia = 4,
		Asura = 5,
	}

	public class Player : GameObject
	{
		public class Network
		{
			public Socket ClSocket { get; set; }
			public XRC4Cipher Encoder { get; set; }
			public XRC4Cipher Decoder { get; set; }
			public byte[] Buffer;
			public int PacketSize;
			public int Offset;
			public PacketStream Data;
		}

		public string UserId;
		public int AccountId;
		public byte Permission;
		public Network NetData;
		public Character Chara;
		
		public Player(uint pHandle) : base(pHandle, GameObjectType.Player) {
			this.NetData = new Network();
		}

		internal void SendCharacterList()
		{
			Database db = new Database(Server.UserDbConString);
			// TODO : Query CleanUP
			MySqlDataReader reader = 
				db.ReaderQuery(
					"SELECT `char_id`,`slot`,`name`,`sex`,`race`," +
					"`hair_id`,`face_id`,`body_id`,`hands_id`,`feet_id`," +
					"`face_detail_id`,`hair_color`,`skin_color`,`x`,`y`,`layer`," +
					"`save_x`,`save_y`,`level`,`exp`,`job`,`job_level`,`create_date`" +
					" FROM `char`" +
					" WHERE `account_id` = @accId AND `delete_date`= 0",
					new string[] { "accId" },
					new object[] { this.AccountId }
				);
			
			List<Character> chars = new List<Character>();

			while (reader.Read())
			{
				Character c = new Character();

				c.Name = (string)reader["name"];
				c.Race = (int)reader["race"];
				c.Sex = (int)reader["sex"];
				c.SkinColor = (int)reader["skin_color"];
				c.HairColor = (int)reader["hair_color"];
				c.Job = (int)reader["job"];
				c.JobLevel = (int)reader["job_level"];
				c.Level = (int)reader["level"];
				c.HairId = (int)reader["hair_id"];
				c.FaceId = (int)reader["face_id"];
				c.BodyId = (int)reader["body_id"];
				c.HandsId = (int)reader["hands_id"];
				c.FeetId = (int)reader["feet_id"];

				c.CreateDate = (int)reader["create_date"];

				chars.Add(c);
			}
			db.Dispose();

			ClientPacketHandler.send_CharacterList(this, chars.ToArray());
		}

		internal void CreateCharacter(string name, int sex, int race, int hairId, int faceId, int bodyId, int handsId, int feetId, int hairColor, int faceDetail, int clothesId, int skinColor)
		{
			Database db = new Database(Server.UserDbConString);
			float x = 0; float y = 0;
			int job = 0;

			// Defines start job and position
			switch (race)
			{
				case (int)PCRace.Asura:
					x = 168356; y = 55399;
					job = 300;
					break;

				case (int)PCRace.Deva:
					x = 164474; y = 52932;
					job = 200;
					break;

				case (int)PCRace.Gaia:
					x = 164335; y = 49510;
					job = 100;
					break;
			}

			db.WriteQuery(
				"INSERT INTO `char`(" +
					"`account_id`,`slot`," +
					"`name`,`sex`,`race`," +
					"`hair_id`, `face_id`, `body_id`, `hands_id`," +
					"`feet_id`, `face_detail_id`, `hair_color`," +
					"`skin_color`, `x`, `y`, `layer`," +
					"`save_x`, `save_y`," +
					"`level`, `exp`," +
					"`job`, `job_level`, `job_exp`,`jp`," +
					"`job_0`, `job0_level`," +
					"`job_1`, `job1_level`," +
					"`job_2`, `job2_level`," +
					"`create_date`, `delete_date`" +
				") VALUES (" +
					"@accId,0," +
					"@name,@sex,@race," +
					"@hairId,@faceId,@bodyId,@handId," +
					"@feetId,@faceDetailId,@hairColor," +
					"@skinColor,@x,@y,0," +
					"0,0," +
					"1,0," +
					"@job,1,0,0," +
					"0,0," +
					"0,0," +
					"0,0," +
					"@createDate,0" +
				")",
				new string[] { 
					"accId", "name", "sex", "race",
					"hairId", "faceId", "bodyId",
					"handId", "feetId", "faceDetailId",
					"hairColor", "skinColor", "x", "y",
					"job", "createDate"
				},
				new object[] { 
					this.AccountId, name, sex, race,
					hairId, faceId, bodyId,
					handsId, feetId, faceDetail,
					hairColor, skinColor, x, y,
					job, TimeUtils.GetTimeStamp(DateTime.Now)
				}
			);

			ClientPacketHandler.send_PacketResponse(this, (short)0x07D2);
		}

		internal void CharNameExists(string name)
		{
			Database db = new Database(Server.UserDbConString);
			MySqlDataReader reader = 
				db.ReaderQuery(
					"SELECT `char_id` " +
					"FROM `char` " +
					"WHERE `name` = @name " +
					"LIMIT 0,1",
					new string[] { "@name" },
					new object[] { name }
				);
			if (reader.Read())
			{
				// Character exists
				ClientPacketHandler.send_PacketResponse(this, (short)0x07D6, 1);
			}
			else
			{
				// Character doesn't exists
				ClientPacketHandler.send_PacketResponse(this, (short)0x07D6);
			}
		}

		internal bool LoadCharacter(string charName)
		{
			Database db = new Database(Server.UserDbConString);
			// TODO : Query CleanUP
			
			MySqlDataReader reader =
				db.ReaderQuery(
					"SELECT `char_id`,`slot`,`sex`,`race`," +
					"`hair_id`,`face_id`,`body_id`,`hands_id`,`feet_id`," +
					"`face_detail_id`,`hair_color`,`skin_color`,`x`,`y`,`layer`," +
					"`save_x`,`save_y`,`level`,`exp`,`job`,`job_level`" +
					" FROM `char`" +
					" WHERE `account_id` = @accId AND `name` = @name AND `delete_date`= 0",
					new string[] { "accId", "name" },
					new object[] { this.AccountId, charName }
				);
			if (!reader.Read())
			{
				// Character not in account
				ConsoleUtils.Write(ConsoleMsgType.Warning, "User trying to get a character that is not in his account.\n");
				return false;
			}

			this.Chara = new Character()
			{
				HairColor = (int)reader["hair_color"],
				Job = (int)reader["job"],
				JobLevel = (int)reader["job_level"],
				Level = (int)reader["level"],
				HairId = (int)reader["hair_id"],
				FaceId = (int)reader["face_id"],
				BodyId = (int)reader["body_id"],
				HandsId = (int)reader["hands_id"],
				FeetId = (int)reader["feet_id"],
				Name = charName,
				Race = (int)reader["race"],
				Sex = (int)reader["sex"],
				SkinColor = (int)reader["skin_color"],
				CharId = (int)reader["char_id"]
			};

			this.X = (float)(int)reader["x"];
			this.Y = (float)(int)reader["y"];
			this.Layer = (int)reader["layer"];

			this.Chara.LoadInventory();
			this.Chara.LoadQuest();
			
			return true;
		}

		internal int GetViewId(Item.WearType wearType)
		{
			if (this.Chara.Equip[(int)wearType] > 0)
			{
				return this.Chara.Inventory[this.Chara.Equip[(int)wearType]].Code;
			}

			switch (wearType)
			{
				case Item.WearType.Armor: return this.Chara.BodyId;
				case Item.WearType.Glove: return this.Chara.HandsId;
				case Item.WearType.Boots: return this.Chara.FeetId;
				case Item.WearType.Face: return this.Chara.FaceId;
				case Item.WearType.Hair: return this.Chara.HairId;
				default: return 0;
			}
		}
	}
}
