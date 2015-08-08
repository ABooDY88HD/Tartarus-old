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
		public const short MaxRewards = 1 + 6; // 1 main + 6 optional
		public const short MaxObjectives = 12;

		public enum Type
		{
			Misc = 100,
			KillTotal = 101,
			KillIndividual = 102,
			Collect = 103,
			HuntItem = 106,
			HuntItemFromAnyMonster = 107,
			KillPlayer = 108,
			LearnSkill = 201,
			UpgradeItem = 301,
			QuestContact = 401,
			JobLevel = 501,
			Parameter = 601,
			RandomKillIndividual = 901,
			RandomCollect = 902
		}

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
