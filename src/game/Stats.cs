// Copyright (c) Tartarus Dev Team, licensed under GNU GPL.
// See the LICENSE file
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace game
{
	public class Stats
	{
		public short JobID;
		public short Strength;
		public short Vitality;
		public short Dexterity;
		public short Agility;
		public short Intelligence;
		public short Wisdom;
		public short Luck;
		public short CritRate;
		public short CritPower;
		public short PAtkRight;
		public short PAtkLeft;
		public short PDef;
		public short BlockDef;
		public short MAtk;
		public short MDef;
		public short AccuracyRight;
		public short AccuracyLeft;
		public short MAccuracy;
		public short Evasion;
		public short MRes;
		public short BlockPer;
		public short MovSpd;
		public short AtkSpd;
		//public short Unknown;
		public short MaxWeight;
		//public short Unknown;
		public short CastSpd;
		public short ReCastSpd;
		//public short Unknown;
		public short HPRegen;
		public short HPRecov;
		public short MPRegen;
		public short MPRecov;
		public short PerfBlock;
		public short MIgnore;
		public short MIgnorePer;
		public short PIgnore;
		public short PIgnorePer;
		public short MPierce;
		public short MPiercePer;
		public short PPierce;
		public short PPiercePer;

		/// <summary>
		/// Recalculate the stats (this must not be used by SC)
		/// </summary>
		/// <param name="level">entity level</param>
		public void Recalculate(int level)
		{
			// Formulas from: http://rappelz.wikia.com/wiki/Stats_%26_Ability

			// Meelee
			this.PAtkRight = (short)Math.Ceiling((decimal)(14 / 5f * this.Strength + level + 9));
			// Ranged
			//this.PAtkRight = (short)Math.Ceiling((decimal)(6 / 5f * this.Agility + (11/5f)*this.Dexterity + level));

			this.AccuracyLeft = (short)Math.Ceiling((decimal)(1 / 2f) * this.Dexterity + level);
			//TODO : Accuracy Right
			this.MAtk = (short)(2 * this.Intelligence + level);
			this.PDef = (short)Math.Ceiling((decimal)(5 / 3f) * this.Vitality + level);
			this.Evasion = (short)Math.Ceiling((decimal)(this.Vitality / 2f + level));
			this.AtkSpd = (short)Math.Ceiling((decimal)(100 + this.Agility / 10f));

			this.MAccuracy = (short)Math.Ceiling((decimal)((4 / 10f) * this.Wisdom + (1 / 10f) * this.Dexterity + level));
			this.MDef = (short) (2 * this.Wisdom + level);
			this.MRes = (short)Math.Ceiling((decimal)((1 / 2f) * this.Wisdom + level));
			this.MovSpd = 120;

			this.HPRegen = 5;
			this.MPRegen = 5;
			this.BlockPer = 0;
			this.CritRate = (short)Math.Ceiling(this.Luck / 5f + 3);
			this.CastSpd = 100;

			this.HPRecov = (short)(2 * level + 48);
			this.MPRecov = (short)(4.1f * this.Wisdom + 2 * level + 48);
			this.BlockDef = 0;
			this.CritPower = 80;
			this.ReCastSpd = 100;
			this.MaxWeight = (short)(10 * this.Strength + 10 * level);
		}
	}
}
