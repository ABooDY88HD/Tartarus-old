// Copyright (c) Tartarus Dev Team, licensed under GNU GPL.
// See the LICENSE file
using common;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace game
{
	public class Character
	{
		public int CharId { get; set; }
		public int Sex { get; set; }
		public int Race { get; set; }
		public int HairId { get; set; } // model_00
		public int FaceId { get; set; } // model_01
		public int BodyId { get; set; } // model_02
		public int HandsId { get; set; } // model_03
		public int FeetId { get; set; } // model_04
		public int HairColor { get; set; }

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

		public int CreateDate { get; set; }

		public Dictionary<uint, Item> Inventory { get; set; }
		public List<Quest> QuestList { get; set; }

		public uint[] Equip { get; set; }

		public Point Position { get; set; }
		public Character()
		{
			Equip = new uint[(int)Item.WearType.WearType_Max];

			//TODO : Calculate
			this.MaxHp = 100;
			this.Hp = 100;
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
	}
}
