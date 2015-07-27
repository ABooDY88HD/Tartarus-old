// Copyright (c) Tartarus Dev Team, licensed under GNU GPL.
// See the LICENSE file
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using common;
using MySql.Data.MySqlClient;

namespace game
{
	public static class ItemDb
	{
		public class ItemEntry
		{
			public Item.ItemType Type { get; set; }
			public Item.Class Class { get; set; }
			public Item.WearType WearType { get; set; }
			public SByte Grade { get; set; }
			public SByte Rank { get; set; }
			public SByte Level { get; set; }
			public SByte Enhance { get; set; }
			public SByte Sockets { get; set; }
			public Db.Races EquipRaces { get; set; }
			public Db.Classes EquipClasses { get; set; }
			public Db.JobDepth EquipDepth { get; set; }
			public Int32 Weight { get; set; }
		}
		public static Dictionary<Int32, ItemEntry> Db { get; private set; }

		public static void Start()
		{
			ConsoleUtils.Write(ConsoleMsgType.Status, "Loading Item Database...\n");

			Db = new Dictionary<int, ItemEntry>();
			Database db = new Database(Server.GameDbConString);
			MySqlDataReader reader = 
				db.ReaderQuery(
					"SELECT `id`, `type`, `class`, `wear_type`, `grade`, `rank`, " +
							"`level`, `enhance`, `sockets`, " +
							"`equip_races`, `equip_classes`, `equip_depth`, " +
							"`weight` " +
					"FROM `item_db`", null, null
				);

			while (reader.Read())
			{
				ItemEntry item = new ItemEntry();

				item.Type = (Item.ItemType)(int)reader["type"];
				item.Class = (Item.Class)(int)reader["class"];
				item.WearType = (Item.WearType)(int)reader["wear_type"];
				item.Grade = (sbyte)reader["grade"];
				item.Rank = (sbyte)reader["rank"];
				item.Level = (sbyte)reader["level"];
				item.Enhance = (sbyte)reader["enhance"];
				item.Sockets = (sbyte)reader["sockets"];
				item.EquipRaces = (Db.Races)(sbyte)reader["equip_races"];
				item.EquipClasses = (Db.Classes)(sbyte)reader["equip_classes"];
				item.EquipDepth = (Db.JobDepth)(sbyte)reader["equip_depth"];
				item.Weight = (int)reader["weight"];

				Db.Add((int)reader["id"], item);
			}

			ConsoleUtils.Write(ConsoleMsgType.Status, "Item Database Loaded.\n");
		}
	}
}
