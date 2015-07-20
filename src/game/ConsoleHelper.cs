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
		public static int TestVal1 { get; set; }
		public static int TestVal2 { get; set; }
		public static int TestVal3 { get; set; }
		public static int TestVal4 { get; set; }

		public static void SetVal(object[] args)
		{
			TestVal1 = Convert.ToInt32(args[0]);
		}

		internal static void PrintVal(object[] args)
		{
			ConsoleUtils.Write(ConsoleMsgType.Debug, "Val1: {0}\n", TestVal1);
		}

		internal static void ConsolePrint(object[] args)
		{
			ConsoleUtils.Write(ConsoleMsgType.Debug, "{0}", args[0].ToString());
		}
	}
}
