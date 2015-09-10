using common;
using MySql.Data.MySqlClient;
// Copyright (c) Tartarus Dev Team, licensed under GNU GPL.
// See the LICENSE file
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace game
{
	/// <summary>
	/// Includes initialization and Databases
	/// </summary>
	public partial class Player
	{
		public static long[] ExpTable;
		public static int[] Jp0Table;
		public static int[] Jp1Table;
		public static int[] Jp2Table;
		public static int[] Jp3Table;

		public static Dictionary<int, JobDBEntry> JobDB;

		public class JobDBEntry
		{
			public int MaxLevel { get; set; }
			public Db.JobDepth JobDepth { get; set; }

			public float StrMult { get; set; }
			public float VitMult { get; set; }
			public float DexMult { get; set; }
			public float AgiMult { get; set; }
			public float IntMult { get; set; }
			public float WisMult { get; set; }
			public float LuckMult { get; set; }
		}

		/// <summary>
		/// Initializes Level Database
		/// </summary>
		internal static void LoadLevelDb()
		{
			ConsoleUtils.Write(ConsoleMsgType.Status, "Loading Player EXP Database...\n");

			List<long> expt = new List<long>();
			List<int> jp0t = new List<int>();
			List<int> jp1t = new List<int>();
			List<int> jp2t = new List<int>();
			List<int> jp3t = new List<int>();

			Database db = new Database(Server.GameDbConString);
			MySqlDataReader reader =
				db.ReaderQuery(
					"SELECT `level`, `exp`, `jp_0`,`jp_1`,`jp_2`,`jp_3` " +
					"FROM `level_db`", null, null
				);

			expt.Add(0);
			jp0t.Add(0);
			jp1t.Add(0);
			jp2t.Add(0);
			jp3t.Add(0);

			while (reader.Read())
			{
				int level = (int)reader["level"];

				long exp = (long)reader["exp"];
				int jp0 = (int)reader["jp_0"];
				int jp1 = (int)reader["jp_1"];
				int jp2 = (int)reader["jp_2"];
				int jp3 = (int)reader["jp_3"];

				if (exp > 0)
					expt.Add(exp);
				if (jp0 > 0)
					jp0t.Add(jp0);
				if (jp1 > 0)
					jp1t.Add(jp1);
				if (jp2 > 0)
					jp2t.Add(jp2);
				if (jp3 > 0)
					jp3t.Add(jp3);
			}

			expt.Add(0);
			jp0t.Add(0);
			jp1t.Add(0);
			jp2t.Add(0);
			jp3t.Add(0);

			ExpTable = expt.ToArray();
			Jp0Table = jp0t.ToArray();
			Jp1Table = jp1t.ToArray();
			Jp2Table = jp2t.ToArray();
			Jp3Table = jp3t.ToArray();

			ConsoleUtils.Write(ConsoleMsgType.Status, "Level Database Loaded.\n");
		}

		/// <summary>
		/// Initializes Job Database
		/// </summary>
		internal static void LoadJobDb()
		{
			// TODO : 
			ConsoleUtils.Write(ConsoleMsgType.Status, "Loading Jobs Database...\n");

			JobDB = new Dictionary<int, JobDBEntry>();

			Database db = new Database(Server.GameDbConString);
			MySqlDataReader reader =
				db.ReaderQuery(
					"SELECT `id`, `job_depth`," +
					"`str_mult`, `vit_mult`, `dex_mult`, `agi_mult`,`int_mult`,`wis_mult`,`luk_mult` " +
					"FROM `job_db`", null, null
				);

			while (reader.Read())
			{
				JobDBEntry job = new JobDBEntry();
				int jobId = (int)reader["id"];
				byte i = (byte)reader["job_depth"];
				//job.JobDepth = (Db.JobDepth)
				job.StrMult = (float)reader["str_mult"];
				job.VitMult = (float)reader["vit_mult"];
				job.DexMult = (float)reader["dex_mult"];
				job.AgiMult = (float)reader["agi_mult"];
				job.IntMult = (float)reader["int_mult"];
				job.WisMult = (float)reader["wis_mult"];
				job.LuckMult = (float)reader["luk_mult"];

				if (JobDB.ContainsKey(jobId))
				{
					ConsoleUtils.Write(ConsoleMsgType.Warning, "Duplicated job ID {0} at job_db\r\n", jobId);
				}
				else
				{
					JobDB.Add(jobId, job);
				}
			}
			
			ConsoleUtils.Write(ConsoleMsgType.Status, "Jobs Database Loaded.\n");
		}

		public static void Start()
		{
			LoadLevelDb();
			LoadJobDb();
		}
	}
}
