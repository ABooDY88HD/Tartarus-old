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
	public static class StatsDb
	{
		public class StatsEntry
		{
			public short Str { get; set; }
			public short Vit { get; set; }
			public short Dex { get; set; }
			public short Agi { get; set; }
			public short Int { get; set; }
			public short Wis { get; set; }
			public short Luk { get; set; }
		}

		public static Dictionary<Int32, StatsEntry> DB { get; private set; }

		public static void Start()
		{
			ConsoleUtils.Write(ConsoleMsgType.Status, "Loading Stats Database...\n");

			DB = new Dictionary<int, StatsEntry>();
			
			Database db = new Database(Server.GameDbConString);
			MySqlDataReader reader = 
				db.ReaderQuery(
					"SELECT `id`, `str`, `vit`, `dex`, `agi`, `int`, `wis`, `luk` " +
					"FROM `stats_db`", null, null
				);

			while (reader.Read())
			{
				StatsEntry stats = new StatsEntry();

				stats.Str = (short)reader["str"];
				stats.Vit = (short)reader["vit"];
				stats.Dex = (short)reader["dex"];
				stats.Agi = (short)reader["agi"];
				stats.Int = (short)reader["int"];
				stats.Wis = (short)reader["wis"];
				stats.Luk = (short)reader["luk"];

				DB.Add((int)reader["id"], stats);
			}

			ConsoleUtils.Write(ConsoleMsgType.Status, "Stats Database Loaded.\n");
		}
	}
}
