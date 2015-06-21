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
		public enum Type
		{
			Misc = 100,
			KillTotal = 101,
			KillIndividual = 102,
			Collect = 103,
			HuntItem = 106,
			HuntItemFromAnyMonster = 107,
			KillPlayer = 108,
			LearnSkill = 201,
			UpgradeItem = 301,
			QuestContact = 401,
			JobLevel = 501,
			Parameter = 601,
			RandomKillIndividual = 901,
			RandomCollect = 902
		}

		public class Quest
		{
			public Type qType { get; set; }
			//public int NameId { get; set; }
			//public int TargetId { get; set; }
			//public int TextId { get; set; }
			public int MinLevel { get; set; }
			public int MaxLevel { get; set; }
			public int MinJobLevel { get; set; }
			public int MaxJobLevel { get; set; }
			//public int LimitRace
			//public int LimitJobs
			public bool Repeatable { get; set; }
			public int Exp { get; set; }
			public int JP { get; set; }
			public int HolicPoint { get; set; }
			public int Gold { get; set; }
			public int[] Objectives;
			public int DropGroup;
		}
		public static Dictionary<int, Quest> Db { get; private set; }

		public static void Start()
		{
			ConsoleUtils.Write(ConsoleMsgType.Status, "Loading Quest Database...\n");

			Db = new Dictionary<int, Quest>();
			List<string[]> entries = game.Db.LoadDb("db/quest_db.txt", "iiiiiiiiiiiiiiggg");

			for (int i = 0; i < entries.Count; i++)
			{
				int qId;
				if (!Int32.TryParse(entries[i][0], out qId))
				{
					ConsoleUtils.Write(ConsoleMsgType.Error, "Invalid quest Id '{0}'. Int expected. Skipping line...\n", entries);
				}
				else
				{
					try
					{
						Quest q = new Quest();
						q.qType = (Type)Int32.Parse(entries[i][1]);
						q.MinLevel = Int32.Parse(entries[i][2]);
						q.MaxLevel = Int32.Parse(entries[i][3]);
						q.MinJobLevel = Int32.Parse(entries[i][4]);
						q.MaxJobLevel = Int32.Parse(entries[i][5]);
						//public int LimitRace = entries[i][6];
						//public int LimitJobs = entries[i][7];
						q.Repeatable = (Int32.Parse(entries[i][8]) > 0);
						q.Exp = Int32.Parse(entries[i][9]);
						q.JP = Int32.Parse(entries[i][10]);
						q.HolicPoint = Int32.Parse(entries[i][11]);
						q.Gold = Int32.Parse(entries[i][12]);
						q.DropGroup = Int32.Parse(entries[i][13]);

						string[] objectives = entries[i][14].Split(',');
						//string[] main_reward = entries[i][15].Split(',');
						//string[] opt_reward = entries[i][16].Split(',');

						q.Objectives = new int[objectives.Length];
						for (int j = 0; j < objectives.Length; j++)
						{
							q.Objectives[j] = Int32.Parse(objectives[j].Trim());
						}
					}
					catch (Exception e)
					{
						ConsoleUtils.Write(ConsoleMsgType.Error, "Error parsing quest Id {0}. Skipping line...\n", qId);
						ConsoleUtils.Write(ConsoleMsgType.Error, "Error: {0}\n", e.Message);
					}
				}

				ConsoleUtils.Write(ConsoleMsgType.Status, "Quest Database Loaded.\n");
			}
		}
	}
}
