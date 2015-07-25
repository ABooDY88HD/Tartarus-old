// Copyright (c) Tartarus Dev Team, licensed under GNU GPL.
// See the LICENSE file
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;
using common;

namespace game
{
	public static class Db
	{
		[Flags]
		public enum Races
		{
			Deva = 1,
			Gaia = 2,
			Asura = 4
		}

		[Flags]
		public enum Classes
		{
			Fighter = 1,
			Hunter = 2,
			Magician = 4,
			Summoner = 8
		}

		[Flags]
		public enum JobDepth
		{
			Basic = 1,
			First = 2,
			Second = 4,
			Master = 8
		}

		public static List<string[]> LoadDb(string fileName, string fields)
		{
			if (!File.Exists(fileName)) {
				ConsoleUtils.Write(ConsoleMsgType.Error, "Database file not found at {0}.\n", fileName);
				return null;
			}

			string file = File.ReadAllText(fileName);
			file = file.Replace("\r", "");
			file = Regex.Replace(file, "//(.*?)\r?\n", "", RegexOptions.Singleline);
			
			StringBuilder str = new StringBuilder();

			for(int i = 0; i < fields.Length; i++) {
				if (i > 0) str.Append(", ");

				if(fields[i].Equals('s')) { // String Field
					str.Append("\"(.+)\"");
				} else if (fields[i].Equals('i')) { // Int Field
					str.Append("(.+)");
				} else if (fields[i].Equals('g')) { // Group Field
					str.Append("\\{(.+)\\}");
				}
			}
			str.Append("?$");
			
			Regex r = new Regex(str.ToString(), RegexOptions.Multiline);
			MatchCollection mc = r.Matches(file);

			List<string[]> dbData = new List<string[]>();
			for (int i = 1; i < mc.Count; i++)
			{
				string[] data = new string[fields.Length];
				for (int j = 1; j < mc[i].Groups.Count; j++)
				{
					data[j-1] = mc[i].Groups[j].Value;
				}
				dbData.Add(data);
			}
		
			return dbData;
		}
	}
}
