// Copyright (c) Tartarus Dev Team, licensed under GNU GPL.
// See the LICENSE file
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace common
{
	public static class ConsoleCommands
	{
		public delegate void CommandAction(object[] args);

		public class Command
		{
			public CommandAction Action { get; private set; }
			public string Args { get; private set; }

			public Command(string args, CommandAction act)
			{
				this.Action = act;
				this.Args = args;
			}
		}

		private static Dictionary<string, Command> Commands = new Dictionary<string, Command>();

		public static void LoadCommands(Dictionary<string, Command> cmds)
		{
			Commands = cmds;
		}

		public static void OnInputReceived(string input)
		{
			string[] cmdData = input.Split(' ');

			if (!Commands.ContainsKey(cmdData[0]))
			{
				ConsoleUtils.Write(ConsoleMsgType.Error, "Command {0} not found.\n", cmdData[0]);
				return;
			}
			if (Commands[cmdData[0]].Args.Length > 0)
				Commands[cmdData[0]].Action(new object[] { cmdData[1] });
			else
				Commands[cmdData[0]].Action(new object[0]);
		}
	}
}
