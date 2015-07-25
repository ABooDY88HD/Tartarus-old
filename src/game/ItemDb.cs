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
			public int Grade { get; set; }
			public int Rank { get; set; }
			public int Level { get; set; }
			public int Enhance { get; set; }
			public int Socket { get; set; }
			public Db.Races EquipRaces { get; set; }
			public Db.Classes EquipClasses { get; set; }
			public Db.JobDepth EquipDepth { get; set; }
			public int Weight { get; set; }
		}
		public static Dictionary<int, ItemEntry> Db { get; private set; }

		public static void Start()
		{
			ConsoleUtils.Write(ConsoleMsgType.Status, "Loading Item Database...\n");

			Db = new Dictionary<int, ItemEntry>();
			List<string[]> entries = game.Db.LoadDb("db/item_db.txt", "isiiiiiiiiiiii");

			for (int i = 0; i < entries.Count; i++)
			{
				int itemId;
				if (!Int32.TryParse(entries[i][0], out itemId))
				{
					ConsoleUtils.Write(ConsoleMsgType.Error, "Invalid item Id '{0}'. Int expected. Skipping line...\n", entries);
				}
				else
				{
					try
					{
						ItemEntry item = new ItemEntry();
						item.Type = (Item.ItemType)Int32.Parse(entries[i][2]);
						item.Class = (Item.Class)Int32.Parse(entries[i][3]);
						item.WearType = (Item.WearType)Int32.Parse(entries[i][4]);
						item.Grade = Int32.Parse(entries[i][5]);
						item.Rank = Int32.Parse(entries[i][6]);
						item.Level = Int32.Parse(entries[i][7]);
						item.Enhance = Int32.Parse(entries[i][8]);
						item.Socket = Int32.Parse(entries[i][9]);
						item.EquipRaces = (Db.Races)Int32.Parse(entries[i][10]);
						item.EquipClasses = (Db.Classes)Int32.Parse(entries[i][11]);
						item.EquipDepth = (Db.JobDepth)Int32.Parse(entries[i][12]);
						item.Weight = Int32.Parse(entries[i][13]);

						Db.Add(itemId, item);
					}
					catch (Exception e)
					{
						ConsoleUtils.Write(ConsoleMsgType.Error, "Error parsing item Id {0}. Skipping line...\n", itemId);
						ConsoleUtils.Write(ConsoleMsgType.Error, "Error: {0}\n", e.Message);
					}
				}
			}
			ConsoleUtils.Write(ConsoleMsgType.Status, "Item Database Loaded.\n");
		}
	}
}
