using common;
// Copyright (c) Tartarus Dev Team, licensed under GNU GPL.
// See the LICENSE file
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace game
{
	public static class ConsoleHelper
	{
		public static int[] Values = new int[10];

		public static bool SetVal(object[] args)
		{
			int index, value;

			index = Convert.ToInt32(args[0]);
			value = Convert.ToInt32(args[1]);

			if (index >= Values.Length || index < 0)
			{
				ConsoleUtils.Write(ConsoleMsgType.Error, "Invalid index.\n");
				return true;
			}

			Values[index] = value;

			return true;
		}

		internal static bool PrintVal(object[] args)
		{
			int index;
			index = Convert.ToInt32(args[0]);
			
			if (index >= Values.Length || index < 0)
			{
				ConsoleUtils.Write(ConsoleMsgType.Error, "Invalid index.\n");
				return true;
			}

			ConsoleUtils.Write(ConsoleMsgType.Debug, "Val: {0}\n", Values[index]);
			return true;
		}

		internal static bool ConsolePrint(object[] args)
		{
			ConsoleUtils.Write(ConsoleMsgType.Debug, "{0}", args[0].ToString());
			return true;
		}

		internal static bool Exit(object[] args)
		{
			return false;
		}
	}
}
