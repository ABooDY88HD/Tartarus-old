// Copyright (c) Tartarus Dev Team, licensed under GNU GPL.
// See the LICENSE file
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace game
{
	public class Quest
	{
		public enum Status
		{
			Startable  = 0x0,
			InProgress = 0x1,
			Finishable = 0x2,
		}

		public int StartText { get; set; }
		public int Code { get; set; }
		public int Status1 { get; set; }
		public int Status2 { get; set; }
		public int Status3 { get; set; }
		public int Status4 { get; set; }
		public int Status5 { get; set; }
		public int Status6 { get; set; }
		public int RemainTime { get; set; }
		public Status Progress { get; set; }
	}
}
