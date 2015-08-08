// Copyright (c) Tartarus Dev Team, licensed under GNU GPL.
// See the LICENSE file
using common;
using MySql.Data.MySqlClient;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace game
{
	public static class QuestDb
	{
		public class QuestEntry
		{
			public int MinLevel { get; set; }
			public int MaxLevel { get; set; }
			public int MinJobLevel { get; set; }
			public int MaxJobLevel { get; set; }
			public Db.Races LimitRaces { get; set; }
			public Db.Classes LimitClasses { get; set; }
			public Db.JobDepth LimitDepth { get; set; }
			public bool Repeatable { get; set; }
			public int Exp { get; set; }
			public int JP { get; set; }
			public int HolicPoint { get; set; }
			public int Gold { get; set; }
			public int DropGroup;
			public Quest.Type Type { get; set; }
			public int[] Objectives;
			public int[] Rewards;
			public sbyte[] RewardsLevel;
			public int[] RewardsCount;
		}
		public static Dictionary<int, QuestEntry> DB { get; private set; }

		public static void Start()
		{
			ConsoleUtils.Write(ConsoleMsgType.Status, "Loading Quest Database...\n");

			DB = new Dictionary<int, QuestEntry>();
			Database db = new Database(Server.GameDbConString);
			StringBuilder query = new StringBuilder(
							"SELECT `id`,`min_level`,`max_level`," +
							"`min_job_level`,`max_job_level`," +
							"`limit_races`,`limit_classes`,`limit_depth`,"+
							"`repeatable`,"+
							"`exp`,`jp`,`holic_point`,`gold`," +
							"`drop_group`,`type`,");

			for (int i = 0; i < Quest.MaxObjectives; i++)
				query.Append("`objective"+i+"`,");
			query.Append("`default_reward_id`,`default_reward_level`,`default_reward_quantity`,");

			for (int i = 1; i < Quest.MaxRewards; i++)
				query.Append("`optional_reward_id"+i+"`,`optional_reward_level"+i+"`,`optional_reward_quantity"+i+"`,");

			query.Remove(query.Length - 1, 1); // removes last comma
			query.Append(" FROM `quest_db`");
			
			MySqlDataReader reader =
				db.ReaderQuery(query.ToString(), null, null);

			while (reader.Read())
			{
				QuestEntry quest = new QuestEntry();
				quest.Objectives = new int[Quest.MaxObjectives];
				quest.Rewards = new int[Quest.MaxRewards];
				quest.RewardsCount = new int[Quest.MaxRewards];
				quest.RewardsLevel = new sbyte[Quest.MaxRewards];

				int id = (int)reader["id"];
				quest.MinLevel = (int)reader["min_level"];
				quest.MaxLevel = (int)reader["max_level"];
				quest.MinJobLevel = (int)reader["min_job_level"];
				quest.MaxJobLevel = (int)reader["max_job_level"];
				quest.LimitRaces = (Db.Races)(sbyte)reader["limit_races"];
				quest.LimitClasses = (Db.Classes)(sbyte)reader["limit_classes"];
				quest.LimitDepth = (Db.JobDepth)(sbyte)reader["limit_depth"];
				quest.Repeatable = (bool)reader["repeatable"];
				quest.Exp = (int)reader["exp"];
				quest.JP = (int)reader["jp"];
				quest.HolicPoint = (int)reader["holic_point"];
				quest.Gold = (int)reader["gold"];
				quest.DropGroup = (int)reader["drop_group"];
				quest.Type = (Quest.Type)(int)reader["type"];
				for (int i = 0; i < Quest.MaxObjectives; i++ )
					quest.Objectives[i] = (int)reader["objective"+i];

				quest.Rewards[0] = (int)reader["default_reward_id"];
				quest.RewardsLevel[0] = (sbyte)reader["default_reward_level"];
				quest.RewardsCount[0] = (int)reader["default_reward_quantity"];

				for (int i = 1; i < Quest.MaxRewards; i++)
				{
					quest.Rewards[i] = (int)reader["optional_reward_id"+i];
					quest.RewardsLevel[i] = (sbyte)reader["optional_reward_level"+i];
					quest.RewardsCount[i] = (int)reader["optional_reward_quantity"+i];
				}

				DB.Add(id, quest);
			}

			ConsoleUtils.Write(ConsoleMsgType.Status, "Quest Database Loaded.\n");
		}
	}
}
