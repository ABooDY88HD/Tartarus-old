using common;
// Copyright (c) Tartarus Dev Team, licensed under GNU GPL.
// See the LICENSE file
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace auth
{
	public static class ConsoleHelper
	{
		public static int TestVal1 { get; set; }
		public static int TestVal2 { get; set; }
		public static int TestVal3 { get; set; }
		public static int TestVal4 { get; set; }

		public static bool SetVal1(object[] args)
		{
			TestVal1 = Convert.ToInt32(args[0]);

			return true;
		}

		public static bool SetVal2(object[] args)
		{
			TestVal2 = Convert.ToInt32(args[0]);

			return true;
		}

		internal static bool PrintVal(object[] args)
		{
			ConsoleUtils.Write(ConsoleMsgType.Debug, "Val1: {0}\n", TestVal1);
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
