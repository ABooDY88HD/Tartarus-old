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

	public class DialogData
	{
		public string Title { get; set; }
		public string Messsage { get; set; }
		public List<string> Options { get; set; }
		public List<string> Functions { get; set; }

		public DialogData()
		{
			this.Title = "";
			this.Messsage = "";
			this.Options = new List<string>();
			this.Functions = new List<string>();
		}
	}

	public static partial class LuaMain
	{
		private static Lua LuaEngine;
		private static LuaChunk Chunk;
		private static LuaGlobalPortable Global;

		private static DialogData DialData;

		private static Object FuncLock = new object();
		private static uint PlayerHandle;

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

					chunkText.Append(" ");
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
			/*
			 * var luaFunc = LuaSysGlobal["GetStruct"] as Func<LuaResult>;
			if (luaFunc == null)
				return null;
			LuaResult r = luaFunc();
			 */

			ConsoleUtils.Write(ConsoleMsgType.Status, "Lua Engine initialized...\n");
			return true;
		}

		public static void DoFunction(Player player, string function)
		{
			lock (FuncLock)
			{
				DialData = new DialogData();

				PlayerHandle = player.Handle;

				Global.DoChunk(function, "main");
			}
		}

		internal static void Contact(Player player, uint npc_handle)
		{
			player.ContactHandle = npc_handle;
			DoFunction(player, GObjectManager.Npcs[npc_handle].ContactScript);
		}
	}
}
