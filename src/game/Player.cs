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
	public partial class Player : CreatureObject
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

		/* *************************************
		 * http://stackoverflow.com/questions/33469/caching-patterns-in-asp-net
		 * I believe this way of adding dirty could improve saving performance
		 * if we split dirty in, let's say, 3 dirty types:
		 * appearance changes ; equip changes ; level/stats changes
		 * so we can reduce the size of the update query and speed it up.
		 * ************************************* */

		public int CharId { get; set; }
		public int Sex { get; set; }
		public int HairId { get; set; } // model_00
		public int FaceId { get; set; } // model_01
		public int BodyId { get; set; } // model_02
		public int HandsId { get; set; } // model_03
		public int FeetId { get; set; } // model_04
		public int HairColor { get; set; }

		public Stats BaseStats { get; set; }
		public Stats SCStats { get; set; }

		public int Havoc { get; set; }

		public int GuildId { get; set; }

		public short Job { get; set; }
		public int JobLevel { get; set; }
		public string Name { get; set; }

		public long Gold { get; set; }
		public int Chaos { get; set; }

		public long Exp { get; set; }
		public long JP { get; set; }
		public int TP { get; set; }

		public int MaxStamina { get; set; }
		public int MaxChaos { get; set; }
		public int MaxHavoc { get; set; }

		public string ClientInfo { get; set; }

		public Dictionary<uint, Item> Inventory { get; set; }
		public List<Quest> QuestList { get; set; }

		public uint[] Equip { get; set; }

		public Dictionary<int, Skill> SkillList { get; set; }

		public uint RegionX { get; set; }
		public uint RegionY { get; set; }

		public uint ContactHandle { get; set; }
		
		public Player(uint pHandle) : base(pHandle, GameObjectSubType.Player)
		{
			this.NetData = new Network();

			Equip = new uint[(int)Item.WearType.WearType_Max];
			Position = new Point(0, 0);

			this.BaseStats = new Stats();
			this.SCStats = new Stats();
		}

		private void LoadStats()
		{
			this.BaseStats = new Stats();
			this.SCStats = new Stats();
			this.BaseStats.JobID = this.Job;

			// Load Job Stats
			this.BaseStats.Strength = StatsDb.DB[this.Job].Str;
			this.BaseStats.Vitality = StatsDb.DB[this.Job].Vit;
			this.BaseStats.Dexterity = StatsDb.DB[this.Job].Dex;
			this.BaseStats.Agility = StatsDb.DB[this.Job].Agi;
			this.BaseStats.Intelligence = StatsDb.DB[this.Job].Int;
			this.BaseStats.Wisdom = StatsDb.DB[this.Job].Wis;
			this.BaseStats.Luck = StatsDb.DB[this.Job].Luk;

			Stats.PCCalculate(this);
			Stats.PCCalculateSC(this);
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
				c.Job = (short)reader["job"];
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
			short job = 0;
			int startWeapon = 0;
			int startBag = 480001;
			int startOutfit = 0;

			// Defines start job and position
			switch (race)
			{
				case (int)Races.Asura:
					x = 168356; y = 55399;
					job = 300;
					startWeapon = 103100; // Beginner's Dirk
					
					if (clothesId == 601)
						startOutfit = 230100;
					else
						startOutfit = 230109;
					break;

				case (int)Races.Deva:
					x = 164335; y = 49510;
					job = 100;
					startWeapon = 106100; // Beginner's Mace

					if (clothesId == 601)
						startOutfit = 240100;
					else
						startOutfit = 240109;
					break;

				case (int)Races.Gaia:
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
					"`save_x`,`save_y`,`level`,`exp`,`job`,`job_level`, `client_info`," +
					"`jp`" +
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

			this.MaxStamina = Globals.MaxStamina;
			this.MaxChaos = 0;
			this.MaxHavoc = 0;

			this.Gold = 0;
			this.Chaos = 0;
			this.TP = 0;

			this.HairColor = (int)reader["hair_color"];
			this.HairId = (int)reader["hair_id"];
			this.FaceId = (int)reader["face_id"];
			this.BodyId = (int)reader["body_id"];
			this.HandsId = (int)reader["hands_id"];
			this.FeetId = (int)reader["feet_id"];

			this.Name = charName;
			this.Race = (Races)(int)reader["race"];
			this.Sex = (int)reader["sex"];
			// TODO : FIX
			this.SkinColor = (uint)(int)reader["skin_color"];
			this.CharId = (int)reader["char_id"];

			this.Position = new Point((int)reader["x"], (int)reader["y"]);
			this.Layer = (byte)(int)reader["layer"];

			this.ClientInfo = (string)reader["client_info"];

			this.LoadQuest();

			this.RegionX = RegionMngr.GetRegionX(this.Position.X);
			this.RegionY = RegionMngr.GetRegionY(this.Position.Y);

			//==== Character level/job/skills/buffs loading
			this.Level = (int)reader["level"];
			this.Job = (short)reader["job"];
			this.JobLevel = (int)reader["job_level"];
			this.JP = (long)reader["jp"];

			this.LoadInventory();
			this.LoadSummons();

			this.LoadStats();
			this.LoadSkills();

			// TODO  Most of these packets probably can be placed in their own methods
			ClientPacketHandler.send_UpdateStats(this, false);
			ClientPacketHandler.send_UpdateStats(this, true);

			ClientPacketHandler.send_Property(this, "max_havoc", this.MaxHavoc);
			ClientPacketHandler.send_Property(this, "max_chaos", this.MaxChaos);
			ClientPacketHandler.send_Property(this, "max_stamina", this.MaxStamina);

			ClientPacketHandler.send_LoginResult(this);

			ClientPacketHandler.send_InventoryList(this);
			ClientPacketHandler.send_EquipSummon(this);
			ClientPacketHandler.send_CharacterView(this);
			ClientPacketHandler.send_UpdateGoldChaos(this);

			ClientPacketHandler.send_Property(this, "tp", this.TP);
			ClientPacketHandler.send_Property(this, "chaos", this.Chaos);
			
			ClientPacketHandler.send_UpdateLevel(this);
			ClientPacketHandler.send_UpdateExp(this);

			ClientPacketHandler.send_Property(this, "job", this.Job);

			ClientPacketHandler.send_Property(this, "job_level", this.JobLevel);
			ClientPacketHandler.send_Property(this, "job_0", 0);
			ClientPacketHandler.send_Property(this, "jlv_0", 0);
			ClientPacketHandler.send_Property(this, "job_1", 0);
			ClientPacketHandler.send_Property(this, "jlv_1", 0);
			ClientPacketHandler.send_Property(this, "job_2", 0);
			ClientPacketHandler.send_Property(this, "jlv_2", 0);

			if (this.SkillList.Count > 0)
				ClientPacketHandler.send_SkillList(this, this.SkillList.Values.ToArray());

			ClientPacketHandler.send_Packet404(this);
			ClientPacketHandler.send_Packet1005(this);

			ClientPacketHandler.send_BeltSlotInfo(this);
			ClientPacketHandler.send_GameTime(this);

			ClientPacketHandler.send_Property(this, "huntaholic_point", 0);
			ClientPacketHandler.send_Property(this, "huntaholic_ent", 12);
			ClientPacketHandler.send_Property(this, "ap", 0);

			ClientPacketHandler.send_Packet4700(this);

			ClientPacketHandler.send_Property(this, "alias", 0, true, "");
			ClientPacketHandler.send_Property(this, "ethereal_stone", 0);
			ClientPacketHandler.send_Property(this, "dk_count", 0);
			ClientPacketHandler.send_Property(this, "pk_count", 0);
			ClientPacketHandler.send_Property(this, "immoral", 0);
			ClientPacketHandler.send_Property(this, "stamina", 0);
			ClientPacketHandler.send_Property(this, "max_stamina", this.MaxStamina);
			ClientPacketHandler.send_Property(this, "channel", 1);

			ClientPacketHandler.send_EntityState(this);

			if (this.ClientInfo != "")
				ClientPacketHandler.send_Property(this, "client_info", 0, true, this.ClientInfo);


			ClientPacketHandler.send_QuestList(this);
			ClientPacketHandler.send_Packet625(this);
			ClientPacketHandler.send_Packet626(this);
			ClientPacketHandler.send_Packet627(this);
			ClientPacketHandler.send_Packet629(this);

			ClientPacketHandler.send_Packet631(this, 1);
			ClientPacketHandler.send_Packet631(this, 2);
			ClientPacketHandler.send_Packet631(this, 3);
			ClientPacketHandler.send_Packet631(this, 4);
			ClientPacketHandler.send_Packet631(this, 5);

			//PacketParser.send_Packet22(sid, 2);
			//PacketParser.send_Packet22(sid, 3);

			ClientPacketHandler.send_Property(this, "playtime", 0);
			ClientPacketHandler.send_Property(this, "playtime_limit1", 1080000);
			ClientPacketHandler.send_Property(this, "playtime_limit2", 1800000);

			ClientPacketHandler.send_LocationInfo(this);
			ClientPacketHandler.send_WeatherInfo(this);

			ClientPacketHandler.send_Property(this, "playtime", 0);

			//PacketParser.send_Packet22(sid, 4);

			//PacketParser.send_Packet08(sid, 1);

			///ClientPacketHandler.send_GameTime(this);
			//PacketParser.send_Packet1101(this, 2);

			///ClientPacketHandler.send_LocationInfo(this);

			ClientPacketHandler.send_Property(this, "stamina_regen", 30);

			ClientPacketHandler.send_UpdateStats(this, false);
			ClientPacketHandler.send_UpdateStats(this, true);

			return true;
		}

		/// <summary>
		/// Sets the character's position
		/// </summary>
		/// <param name="jobLv"></param>
		private void SetPosition(float x, float y, byte layer)
		{
			this.Position = new Point(x, y);
			this.Layer = layer;

			ClientPacketHandler.send_Property(this, "x", 0, true, this.Position.X.ToString());
			ClientPacketHandler.send_Property(this, "y", 0, true, this.Position.Y.ToString());
			ClientPacketHandler.send_Property(this, "layer", 0, true, this.Layer.ToString());
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
		/// Loads user's skill list
		/// </summary>
		private void LoadSkills()
		{
			this.SkillList = new Dictionary<int,Skill>();

			Database db = new Database(Server.UserDbConString);

			MySqlDataReader reader =
				db.ReaderQuery(
					"SELECT `id`, `level` " +
					"FROM `skill` " +
					"WHERE `char_id` = @charId",
					new string[] { "charId" },
					new object[] { this.CharId }
				);

			while (reader.Read())
			{
				Skill skill = new Skill();

				skill.Id = (int)reader["id"];
				skill.Level = (byte)reader["level"];

				this.SkillList.Add(skill.Id, skill);
			}
		}

		private void LoadSummons()
		{
			///ClientPacketHandler.send_EquipSummon(this);
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
		/// Save character
		/// </summary>
		internal void Save()
		{
			Database db = new Database(Server.UserDbConString);
			
			db.WriteQuery(
				"UPDATE `char` SET `x` = @x, `y` = @y, `jp` = @jp, `job_level` = @jlv, `level` = @lv, `job` = @job WHERE `char_id` = @cid",
				new string[] { 
					"cid", "x", "y", "jp", "jlv", "lv", "job"
				},
				new object[] { 
					this.CharId,  (int) this.Position.X, (int) this.Position.Y,
					this.JP, this.JobLevel, this.Level, this.Job
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

		internal void UnequipItem(int wearType)
		{
			uint handle = this.Equip[wearType];
			if (handle == 0)
			{
				ConsoleUtils.Write(ConsoleMsgType.Error, "Trying to unequip inexistant item.");
				return;
			}
			this.Equip[wearType] = 0;

			ClientPacketHandler.send_WearChange(this, handle, -1, this.Inventory[handle].Enhance);

			if (Stats.PCCalculate(this))
			{
				ClientPacketHandler.send_UpdateStats(this, false);
				ClientPacketHandler.send_UpdateStats(this, true);
			}

			ClientPacketHandler.send_Property(this, "max_havoc", Globals.MaxHavoc);
			ClientPacketHandler.send_Property(this, "max_chaos", Globals.MaxChaos);
			ClientPacketHandler.send_Property(this, "max_stamina", Globals.MaxStamina);

			ClientPacketHandler.send_PacketResponse(this, 0x00C9);

			ClientPacketHandler.send_CharacterView(this);
		}

		internal void EquipItem(int wearType, uint itemHandle)
		{
			if (!this.Inventory.ContainsKey(itemHandle))
			{
				ConsoleUtils.Write(ConsoleMsgType.Error, "Trying to access invalid handle in player inventory.\n");
				return;
			}
			Item i = this.Inventory[itemHandle];
			this.Equip[(int)i.WearInfo] = itemHandle;

			ClientPacketHandler.send_WearChange(this, itemHandle, (short)i.WearInfo, this.Inventory[itemHandle].Enhance);

			if (Stats.PCCalculate(this))
			{
				ClientPacketHandler.send_UpdateStats(this, false);
				ClientPacketHandler.send_UpdateStats(this, true);
			}

			ClientPacketHandler.send_Property(this, "max_havoc", Globals.MaxHavoc);
			ClientPacketHandler.send_Property(this, "max_chaos", Globals.MaxChaos);
			ClientPacketHandler.send_Property(this, "max_stamina", Globals.MaxStamina);

			ClientPacketHandler.send_PacketResponse(this, 0x00C8);

			ClientPacketHandler.send_CharacterView(this);
		}

		/// <summary>
		/// Tries to Level UP character Job
		/// </summary>
		internal void JobLevelUp()
		{
			int reqJp = 0;
			switch (JobId2Depth(this.Job))
			{
				case Db.JobDepth.Basic:
					reqJp = Jp0Table[this.JobLevel];
					break;
				case Db.JobDepth.First:
					reqJp = Jp1Table[this.JobLevel];
					break;
				case Db.JobDepth.Second:
					reqJp = Jp2Table[this.JobLevel];
					break;
				case Db.JobDepth.Master:
					reqJp = Jp3Table[this.JobLevel];
					break;
			}

			if (reqJp <= 0) // Max level reached
				return;
			else if (this.JP < reqJp) // Not enough JP
				return;

			this.JP -= reqJp;
			this.JobLevel++;

			ClientPacketHandler.send_Property(this, "jp", this.JP);
			ClientPacketHandler.send_Property(this, "job_level", this.JobLevel);
			ClientPacketHandler.send_UpdateStats(this, false);
			ClientPacketHandler.send_UpdateStats(this, true);
			ClientPacketHandler.send_Property(this, "max_havoc", this.MaxHavoc);
			ClientPacketHandler.send_Property(this, "max_chaos", this.MaxChaos);
			ClientPacketHandler.send_Property(this, "max_stamina", this.MaxStamina);
			ClientPacketHandler.send_PacketResponse(this, 0x019A, 0, this.Handle);
			
		}

		/// <summary>
		/// Checks if there's an item equipped at this position
		/// </summary>
		/// <param name="pos"></param>
		/// <returns></returns>
		internal bool IsEquipped(Item.WearType pos)
		{
			return (this.Equip[(int)pos] > 0);
		}
	}
}
