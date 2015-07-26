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

		public int CharId { get; set; }
		public int Sex { get; set; }
		public int Race { get; set; }
		public int HairId { get; set; } // model_00
		public int FaceId { get; set; } // model_01
		public int BodyId { get; set; } // model_02
		public int HandsId { get; set; } // model_03
		public int FeetId { get; set; } // model_04
		public int HairColor { get; set; }

		public Stats BaseStats { get; set; }
		public Stats SCStats { get; set; }

		public int Hp { get; set; }
		public int Mp { get; set; }
		public int MaxHp { get; set; }
		public int MaxMp { get; set; }
		public int Havoc { get; set; }

		public int GuildId { get; set; }

		public int Job { get; set; }
		public int Level { get; set; }
		public int JobLevel { get; set; }
		public string Name { get; set; }
		public int SkinColor { get; set; }

		public long Gold { get; set; }
		public int Chaos { get; set; }

		public long Exp { get; set; }
		public long JP { get; set; }

		public string ClientInfo { get; set; }

		public Dictionary<uint, Item> Inventory { get; set; }
		public List<Quest> QuestList { get; set; }

		public uint[] Equip { get; set; }

		public Point Position { get; set; }
		
		public Player(uint pHandle) : base(pHandle, GameObjectType.Player)
		{
			this.NetData = new Network();

			Equip = new uint[(int)Item.WearType.WearType_Max];
			Position = new Point(0, 0);

			LoadStats();
		}

		private void LoadStats()
		{
			this.BaseStats = new Stats();
			this.SCStats = new Stats();

			this.BaseStats.Recalculate(this.Level);
			//this.RecalculateHPMP();

			this.MaxHp = 100;
			this.MaxMp = 100;
			this.Hp = this.MaxHp;
		}

		public void RecalculateHPMP()
		{
			this.MaxHp = 33 * (this.BaseStats.Vitality + this.SCStats.Vitality) + 20 * this.Level;
			this.MaxMp = 30 * (this.BaseStats.Intelligence + this.SCStats.Intelligence) + 20 * this.Level;
		}

		/// <summary>
		/// Loads and sends the list of characters of this account
		/// </summary>
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
			
			List<CharacterListEntry> chars = new List<CharacterListEntry>();

			Database db2 = new Database(Server.UserDbConString);
			while (reader.Read())
			{
				CharacterListEntry c = new CharacterListEntry();

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

				
				MySqlDataReader equipList =
					db2.ReaderQuery(
						"SELECT `code`, `equip`" +
						" FROM `item`" +
						" WHERE `char_id` = @charId AND `equip` >= 0",
						new string[] { "charId" },
						new object[] { (int)reader["char_id"] }
					);
				
				while (equipList.Read())
				{
					c.Equip[(int)equipList["equip"]] = (int)equipList["code"];
				}

				equipList.Close();

				chars.Add(c);
			}
			db.Dispose();

			ClientPacketHandler.send_CharacterList(this, chars.ToArray());
		}

		/// <summary>
		/// Creates a new character
		/// </summary>
		/// <param name="name">char name</param>
		/// <param name="sex">sex</param>
		/// <param name="race">race</param>
		/// <param name="hairId">hair ID</param>
		/// <param name="faceId">Face ID</param>
		/// <param name="bodyId">Body ID</param>
		/// <param name="handsId">Hands ID</param>
		/// <param name="feetId">Feet ID</param>
		/// <param name="hairColor">Hair Color</param>
		/// <param name="faceDetail">Face Detail</param>
		/// <param name="clothesId">Clothers ID (601/602)</param>
		/// <param name="skinColor">Skin Color</param>
		internal void CreateCharacter(string name, int sex, int race, int hairId, int faceId, int bodyId, int handsId, int feetId, int hairColor, int faceDetail, int clothesId, int skinColor)
		{
			Database db = new Database(Server.UserDbConString);
			float x = 0; float y = 0;
			int job = 0;
			int startWeapon = 0;
			int startBag = 480001;
			int startOutfit = 0;

			// Defines start job and position
			switch (race)
			{
				case (int)PCRace.Asura:
					x = 168356; y = 55399;
					job = 300;
					startWeapon = 103100; // Beginner's Dirk
					
					if (clothesId == 601)
						startOutfit = 230100;
					else
						startOutfit = 230109;
					break;

				case (int)PCRace.Deva:
					x = 164335; y = 49510;
					job = 100;
					startWeapon = 106100; // Beginner's Mace

					if (clothesId == 601)
						startOutfit = 240100;
					else
						startOutfit = 240109;
					break;

				case (int)PCRace.Gaia:
					x = 164474; y = 52932;
					job = 200;
					startWeapon = 112100; //Trainee's Small Axe

					if (clothesId == 601)
						startOutfit = 220100;
					else
						startOutfit = 220109;
					break;
			}

			int charId =
				(int) db.WriteQuery(
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
						"`create_date`, `delete_date`," +
						"`client_info`" +
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
						"@createDate,0," +
						"@clientInfo" +
					")",
					new string[] { 
						"accId", "name", "sex", "race",
						"hairId", "faceId", "bodyId",
						"handId", "feetId", "faceDetailId",
						"hairColor", "skinColor", "x", "y",
						"job", "createDate", "clientInfo"
					},
					new object[] { 
						this.AccountId, name, sex, race,
						hairId, faceId, bodyId,
						handsId, feetId, faceDetail,
						hairColor, skinColor, x, y,
						job, TimeUtils.GetTimeStamp(DateTime.Now),
						"QS2=0,2,0|QS2=1,2,2|QS2=11,2,1|QS2=24,2,7|QS2=25,2,8|QS2=35,2,28"
					}
				);

			Item startItem = new Item();
			startItem.Code = startBag;
			startItem.WearInfo = Item.WearType.BagSlot;
			startItem.Count = 1;
			Item.CharacterGetItem(charId, startItem);
			
			startItem.Code = startWeapon;
			startItem.WearInfo = Item.WearType.RightHand;
			Item.CharacterGetItem(charId, startItem);

			startItem.Code = startOutfit;
			startItem.WearInfo = Item.WearType.Armor;
			Item.CharacterGetItem(charId, startItem);

			ClientPacketHandler.send_PacketResponse(this, (short)0x07D2);
		}

		/// <summary>
		/// Checks if a character name is already in use
		/// </summary>
		/// <param name="name">the name to be checked</param>
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

		/// <summary>
		/// Loads a character data
		/// </summary>
		/// <param name="charName">the name of the character</param>
		/// <returns>true on success, false otherwise</returns>
		internal bool LoadCharacter(string charName)
		{
			Database db = new Database(Server.UserDbConString);
			// TODO : Query CleanUP
			
			MySqlDataReader reader =
				db.ReaderQuery(
					"SELECT `char_id`,`slot`,`sex`,`race`," +
					"`hair_id`,`face_id`,`body_id`,`hands_id`,`feet_id`," +
					"`face_detail_id`,`hair_color`,`skin_color`,`x`,`y`,`layer`," +
					"`save_x`,`save_y`,`level`,`exp`,`job`,`job_level`, `client_info`" +
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

			this.HairColor = (int)reader["hair_color"];
			this.Job = (int)reader["job"];
			this.JobLevel = (int)reader["job_level"];
			this.Level = (int)reader["level"];
			this.HairId = (int)reader["hair_id"];
			this.FaceId = (int)reader["face_id"];
			this.BodyId = (int)reader["body_id"];
			this.HandsId = (int)reader["hands_id"];
			this.FeetId = (int)reader["feet_id"];
			this.Name = charName;
			this.Race = (int)reader["race"];
			this.Sex = (int)reader["sex"];
			this.SkinColor = (int)reader["skin_color"];
			this.CharId = (int)reader["char_id"];

			this.X = (float)(int)reader["x"];
			this.Y = (float)(int)reader["y"];
			this.Layer = (int)reader["layer"];

			this.ClientInfo = (string)reader["client_info"];
			this.LoadInventory();
			this.LoadQuest();
			
			return true;
		}

		/// <summary>
		/// Gets the ID for appearances
		/// </summary>
		/// <param name="wearType">the slot</param>
		/// <returns>the view ID</returns>
		internal int GetViewId(Item.WearType wearType)
		{
			if (this.Equip[(int)wearType] > 0)
			{
				return this.Inventory[this.Equip[(int)wearType]].Code;
			}

			switch (wearType)
			{
				case Item.WearType.Armor: return this.BodyId;
				case Item.WearType.Glove: return this.HandsId;
				case Item.WearType.Boots: return this.FeetId;
				case Item.WearType.Face: return this.FaceId;
				case Item.WearType.Hair: return this.HairId;
				default: return 0;
			}
		}
		
		/// <summary>
		/// Loads user's inventory List
		/// </summary>
		/// <returns></returns>
		internal bool LoadInventory()
		{
			this.Inventory = new Dictionary<uint, Item>();

			Database db = new Database(Server.UserDbConString);

			MySqlDataReader reader =
				db.ReaderQuery(
					"SELECT `id`, `code`, `count`, `equip` " +
					"FROM `item` " +
					"WHERE `char_id` = @charId",
					new string[] { "charId" },
					new object[] { this.CharId }
				);

			while (reader.Read())
			{
				Item i = GObjectManager.GetNewItem();
				i.UId = (long)reader["id"];
				i.Code = (int)reader["code"];
				i.Count = (long)reader["count"];
				i.WearInfo = (Item.WearType)(int)reader["equip"];

				if (i.WearInfo != Item.WearType.None)
					this.Equip[(int)i.WearInfo] = i.Handle;

				this.Inventory.Add(i.Handle, i);
			}

			return true;
		}

		/// <summary>
		/// Loads user's quest list
		/// </summary>
		internal void LoadQuest()
		{
			this.QuestList = new List<Quest>();

			Database db = new Database(Server.UserDbConString);
			// TODO : Query CleanUP

			MySqlDataReader reader =
				db.ReaderQuery(
					"SELECT `id`, `quest_id`, `start_text`, `remain_time`, `progress`," +
					"`status1`, `status2`, `status3`, `status4`, `status5`, `status6`" +
					" FROM `quest`" +
					" WHERE `char_id` = @charId",
					new string[] { "charId" },
					new object[] { this.CharId }
				);

			while (reader.Read())
			{
				Quest q = new Quest()
				{
					StartText = (int)reader["start_text"],
					Code = (int)reader["quest_id"],
					Status1 = (int)reader["status1"],
					Status2 = (int)reader["status2"],
					Status3 = (int)reader["status3"],
					Status4 = (int)reader["status4"],
					Status5 = (int)reader["status5"],
					Status6 = (int)reader["status6"],
					RemainTime = (int)reader["remain_time"],
					Progress = (Quest.Status)(int)reader["status"]
				};
				this.QuestList.Add(q);
			}
		}

		/// <summary>
		/// Updates hotkey data
		/// </summary>
		/// <param name="newInfo">new hotkey data</param>
		internal void SetClientInfo(string newInfo)
		{
			this.ClientInfo = newInfo;

			Database db = new Database(Server.UserDbConString);

			db.WriteQuery(
				"UPDATE `char` SET `client_info` = @info WHERE `char_id` = @cid",
				new string[] { "cid", "info" },
				new object[] { this.CharId, newInfo }
			);
		}

		/// <summary>
		/// Save characters position
		/// </summary>
		internal void Save()
		{
			Database db = new Database(Server.UserDbConString);
			
			db.WriteQuery(
				"UPDATE `char` SET `x` = @x, `y` = @y WHERE `char_id` = @cid",
				new string[] { 
					"cid", "x", "y"
				},
				new object[] { 
					this.CharId,  (int) this.Position.X, (int) this.Position.Y
				}
			);
		}

		/// <summary>
		/// Updates a value of a property
		/// </summary>
		/// <param name="propertyName">the name of the property</param>
		/// <param name="propertyValue">new property value</param>
		internal void SetProperty(string propertyName, string propertyValue)
		{
			switch (propertyName)
			{
				case "client_info":
					this.SetClientInfo(propertyValue);
					break;

				default:
					ConsoleUtils.Write(ConsoleMsgType.Error, "Trying to set invalid property {0}", propertyName);
					break;
			}

			return;
		}

		/// <summary>
		/// Deletes a character form an account
		/// </summary>
		/// <param name="charName">the name of the character to delete</param>
		internal void DeleteCharacter(string charName)
		{
			Database db = new Database(Server.UserDbConString);

			int r = db.DeleteQuery(
				"DELETE FROM `char` WHERE `account_id` = @accId AND `name` = @charName",
				new string[] { "accId", "charName" },
				new object[] { this.AccountId, charName }
			);

			if (r > 0)
				ClientPacketHandler.send_PacketResponse(this, 0x07D3, 0);
			else
				ClientPacketHandler.send_PacketResponse(this, 0x07D3, 1);
		}
	}
}
