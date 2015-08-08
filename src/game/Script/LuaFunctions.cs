using common;
// Copyright (c) Tartarus Dev Team, licensed under GNU GPL.
// See the LICENSE file
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace game.Script
{
	// This file extends the LuaMain class,
	// and adds the script commands

	public static partial class LuaMain
	{
		private static void GetFunctions()
		{
			Global.DefineFunction("DebugLog", new Action<object[]>(DebugLog));

			// Dialog Commands
			Global.DefineFunction("dlg_title", new Action<object[]>(DlgTitle));
			Global.DefineFunction("dlg_text", new Action<object[]>(DlgText));
			Global.DefineFunction("dlg_menu", new Action<object[]>(DlgMenu));
			Global.DefineFunction("dlg_show", new Action<object[]>(DlgShow));

			//
			Global.DefineFunction("get_value", new Func<string, object>(GetValue));

			// Placeholders
			Global.DefineFunction("cprint", new Action<object[]>(DebugLog));
			Global.DefineFunction("set_npc_name", new Action<object[]>(SetNpcName));
			Global.DefineFunction("get_quest_progress", new Func<int, int>(GetQuestProg));
			Global.DefineFunction("dlg_text_without_quest_menu", new Action<object>(DlgTextWOMenu));
			Global.DefineFunction("add_state", new Action<int, int, int>(AddState));
			Global.DefineFunction("get_npc_id", new Func<int, int>(GetNpcId));
		}

		

		private static void DebugLog(object[] texts)
		{
			foreach (object o in texts)
			{
				ConsoleUtils.Write(ConsoleMsgType.Debug, "{0}", o);
			}
			Console.Write("\n");
		}

		// ===========================
		//		Dialog Commands
		// ===========================

		private static void DlgTitle(object[] obj)
		{
			DialData.Title = (string)obj[0];
		}

		private static void DlgText(object[] obj)
		{
			DialData.Messsage = (string)obj[0];
		}

		private static void DlgMenu(object[] obj)
		{
			DialData.Options.Add((string)obj[0]);
			DialData.Functions.Add((string)obj[1]);
		}

		private static void DlgShow(object[] obj)
		{
			ClientPacketHandler.send_Dialog(GObjectManager.Players[PlayerHandle], Npc.DialogType.Npc, DialData);
		}

		// ===========================
		//		Value Commands
		// ===========================

		private static object GetValue(string name)
		{
			switch (name)
			{
				case "race":
					return GObjectManager.Players[PlayerHandle].Race;
			}

			return null;
		}

		///
		/// Place Holders
		/// 

		private static int GetNpcId(int arg)
		{
			return GObjectManager.Npcs[GObjectManager.Players[PlayerHandle].ContactHandle].Id;
		}

		private static void AddState(int arg1, int arg2, int arg3)
		{
			
		}

		private static void DlgTextWOMenu(object obj)
		{
			DialData.Messsage = (string)obj;
		}

		private static int GetQuestProg(int arg)
		{
			return 1;
		}

		private static void SetNpcName(object[] obj)
		{
			
		}
	}
}
