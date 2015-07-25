// Copyright (c) Tartarus Dev Team, licensed under GNU GPL.
// See the LICENSE file
using common;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace game
{
	public class CharacterListEntry
	{
		public int Sex { get; set; }
		public int Race { get; set; }
		public int HairId { get; set; } // model_00
		public int FaceId { get; set; } // model_01
		public int BodyId { get; set; } // model_02
		public int HandsId { get; set; } // model_03
		public int FeetId { get; set; } // model_04
		public int HairColor { get; set; }

		public int Job { get; set; }
		public int Level { get; set; }
		public int JobLevel { get; set; }
		public string Name { get; set; }
		public int SkinColor { get; set; }

		public int CreateDate { get; set; }

		public int[] Equip { get; set; }

		public CharacterListEntry()
		{
			Equip = new int[(int)Item.WearType.WearType_Max];
		}
	}
}
