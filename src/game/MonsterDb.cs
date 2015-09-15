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
	public static class MonsterDb
	{
		public class MonsterEntry
		{
			public Int32 Hp;
			public Int32 Mp;
		}

		public static Dictionary<Int32, MonsterEntry> DB { get; private set; }

		public static void Start()
		{
			ConsoleUtils.Write(ConsoleMsgType.Status, "Loading Monster Database...\n");

			DB = new Dictionary<int, MonsterEntry>();
			Database db = new Database(Server.GameDbConString);
			MySqlDataReader reader = 
				db.ReaderQuery(
					"SELECT `id`, `hp`, `mp` " +
					"FROM `mob_db`", null, null
				);

			while (reader.Read())
			{
				MonsterEntry mob = new MonsterEntry();

				int id = (int)reader["id"];
				mob.Hp = (int)reader["hp"];
				mob.Mp = (int)reader["mp"];

				DB.Add(id, mob);
			}

			ConsoleUtils.Write(ConsoleMsgType.Status, "Mob Database Loaded.\n");
		}
	}
}
