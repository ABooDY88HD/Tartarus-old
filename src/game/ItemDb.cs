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
		public class Item
		{
			public int Type { get; set; }
			public int Class { get; set; }
			public int WearType { get; set; }
			public int EquipRaces { get; set; }
			public int EquipClasses { get; set; }
			public int EquipDepth { get; set; }
			public int Weight { get; set; }
		}

		public static Dictionary<int, Item> Db { get; private set; }

		public static void Start()
		{
			Db = new Dictionary<int, Item>();
			Database db = new Database(Server.GameDbConString);

			MySqlDataReader reader =
				db.ReaderQuery("SELECT `id`, `name_id`, `type`, `class`," +
							"`wear_type`, `equip_races`, `equip_classes`," +
							"`equip_depth`, `weight` FROM `item`", null, null);
			
			while (reader.Read())
			{
				ItemDb.Item item = new Item()
				{
					Class = (int)reader["class"],
					//EquipClasses = (short)reader["equip_classes"],
					//EquipDepth = (short)reader["equip_depth"],
					//EquipRaces = (short)reader["equip_races"],
					Type = (int)reader["type"],
					WearType = (int)reader["wear_type"],
					Weight = (int)reader["weight"]
				};
				Db.Add((int)reader["id"], item);
			}
		}
	}
}
