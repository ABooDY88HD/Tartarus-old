// Copyright (c) Tartarus Dev Team, licensed under GNU GPL.
// See the LICENSE file
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using common;

namespace game
{
	// Game-Server initialization
	// Loads config files and start the server.
	class Program
	{
		static void Main(string[] args)
		{
			ConsoleUtils.ShowHeader();

			Config conf = new Config();
			if (!conf.Read("conf/game-server.txt"))
			{
				ConsoleUtils.Write(ConsoleMsgType.Info, "Press a key to end.\n");
				Console.ReadKey();
				return;
			}
			else if (!conf.Read("conf/inter-server.txt"))
			{
				ConsoleUtils.Write(ConsoleMsgType.Info, "Press a key to end.\n");
				Console.ReadKey();
				return;
			}

			if (conf.Data.ContainsKey("console_silent"))
			{
				int consoleSilent;
				if (Int32.TryParse(conf.Data["console_silent"], out consoleSilent))
				{
					ConsoleUtils.SetDisplaySettings((ConsoleMsgType)consoleSilent);
				}
				else
				{
					ConsoleUtils.Write(ConsoleMsgType.Error, "Invalid 'console_silent' value.\n");
				}
			}

			Settings.Set(conf.Data);
			Server sv = new Server();

			ConsoleCommands.LoadCommands(GetConsoleCmdList());
			do
			{
				ConsoleCommands.OnInputReceived(Console.ReadLine());
			} while (true);

			/* Console Debug
			ConsoleUtils.Write(ConsoleMsgType.None, "Msg0\n");
			ConsoleUtils.Write(ConsoleMsgType.Info, "Msg1\n");
			ConsoleUtils.Write(ConsoleMsgType.Status, "Msg2\n");
			ConsoleUtils.Write(ConsoleMsgType.Notice, "Msg4\n");
			ConsoleUtils.Write(ConsoleMsgType.Warning, "Msg8\n");
			ConsoleUtils.Write(ConsoleMsgType.Error, "Msg16\n");
			ConsoleUtils.Write(ConsoleMsgType.Debug, "Msg32\n");
			ConsoleUtils.Write(ConsoleMsgType.SQL, "Msg64\n");
			ConsoleUtils.Write(ConsoleMsgType.FatalError, "Msg128\n");
			ConsoleUtils.Write(ConsoleMsgType.PacketDebug, "Msg256\n");
			*/
		}

		private static Dictionary<string, ConsoleCommands.Command> GetConsoleCmdList()
		{
			Dictionary<string, ConsoleCommands.Command> cmdList = new Dictionary<string, ConsoleCommands.Command>();

			cmdList.Add("set_val", new ConsoleCommands.Command("i", ConsoleHelper.SetVal));
			cmdList.Add("print_val", new ConsoleCommands.Command("", ConsoleHelper.PrintVal));

			return cmdList;
		}
	}
}
