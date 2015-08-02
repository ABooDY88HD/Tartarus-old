// Copyright (c) Tartarus Dev Team, licensed under GNU GPL.
// See the LICENSE file
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Neo.IronLua;
using common;

namespace game.Script
{
	public static class LuaMain
	{
		private static Lua LuaEngine;
		private static LuaChunk Chunk;
		private static LuaGlobalPortable Global;
		/// <summary>
		/// Inits the Lua Engine
		/// </summary>
		public static bool Start()
		{
			ConsoleUtils.Write(ConsoleMsgType.Status, "Initializing Lua Engine...\n");

			LuaEngine = new Lua();
			Global = LuaEngine.CreateEnvironment();

			// Register Functions
			GetFunctions();

			List<string> scriptFiles = new List<string>();

			if (!File.Exists("npc/scripts_main.txt"))
			{
				ConsoleUtils.Write(ConsoleMsgType.FatalError, "Could not find file npc/scripts_main.txt. Terminating...\n");
				return false;
			}
			scriptFiles.AddRange(File.ReadAllLines("npc/scripts_main.txt"));

			StringBuilder chunkText = new StringBuilder();

			for (int i = 0; i < scriptFiles.Count; i++)
			{
				if (scriptFiles[i].StartsWith("//"))
				{ // Ignore comments
					continue;
				}
				else if (scriptFiles[i].StartsWith("npc:"))
				{ // Loads a Lua
					string fname = scriptFiles[i].Split(new char[] { ':' }, 2)[1].TrimStart(' ');
					if (!File.Exists(fname))
					{
						ConsoleUtils.Write(ConsoleMsgType.Error, "Could not find file '{0}'...\n", fname);
						continue;
					}

					chunkText.Append(File.ReadAllText(fname));
				}
				else if (scriptFiles[i].StartsWith("import:"))
				{ // imports another txt
					string fname = scriptFiles[i].Split(new char[] { ':' }, 2)[1].TrimStart(' ');
					if (!File.Exists(fname))
					{
						ConsoleUtils.Write(ConsoleMsgType.Error, "Could not find file '{0}'...\n", fname);
						continue;
					}

					scriptFiles.AddRange(File.ReadAllLines(fname));
				}
			}

			Chunk = LuaEngine.CompileChunk(chunkText.ToString(), "main", null);

			Global.DoChunk(Chunk);

			ConsoleUtils.Write(ConsoleMsgType.Status, "Lua Engine initialized...\n");
			return true;
		}

		private static void GetFunctions()
		{
			Global.DefineFunction("DebugLog", new Action<object[]>(Print));
		}

		private static void Print(object[] texts)
		{
			foreach (object o in texts)
			{
				ConsoleUtils.Write(ConsoleMsgType.Debug, "{0}", o);
			}
			Console.Write("\n");
		}
	}
}
