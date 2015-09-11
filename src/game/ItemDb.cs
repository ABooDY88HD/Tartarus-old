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

			public Stats.BaseType[] BaseTypes { get; set; }
			public float[] BaseVar1 { get; set; }
			public float[] BaseVar2 { get; set; }
		}

		public static Dictionary<Int32, ItemEntry> DB { get; private set; }

		public static void Start()
		{
			ConsoleUtils.Write(ConsoleMsgType.Status, "Loading Item Database...\n");

			DB = new Dictionary<int, ItemEntry>();
			Database db = new Database(Server.GameDbConString);
			MySqlDataReader reader = 
				db.ReaderQuery(
					"SELECT `id`, `type`, `class`, `wear_type`, `grade`, `rank`, " +
							"`level`, `enhance`, `sockets`, " +
							"`equip_races`, `equip_classes`, `equip_depth`, " +
							"`weight`," +
							"`base_type_0`,`base_var1_0`,`base_var2_0`," +
							"`base_type_1`,`base_var1_1`,`base_var2_1`," +
							"`base_type_2`,`base_var1_2`,`base_var2_2`," +
							"`base_type_3`,`base_var1_3`,`base_var2_3` " +
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

				item.BaseTypes = new Stats.BaseType[Item.MaxBaseTypes];
				item.BaseVar1 = new float[Item.MaxBaseTypes];
				item.BaseVar2 = new float[Item.MaxBaseTypes];
				for (int i = 0; i < Item.MaxBaseTypes; i++)
				{
					item.BaseTypes[i] = (Stats.BaseType)(byte)reader["base_type_" + i];
					item.BaseVar1[i] = (float)(decimal)reader["base_var1_" + i];
					item.BaseVar2[i] = (float)(decimal)reader["base_var2_" + i];
				}

				DB.Add((int)reader["id"], item);
			}

			ConsoleUtils.Write(ConsoleMsgType.Status, "Item Database Loaded.\n");
		}
	}
}
