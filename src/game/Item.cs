// Copyright (c) Tartarus Dev Team, licensed under GNU GPL.
// See the LICENSE file
using common;
using common.RC4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using common.Utilities;

namespace game
{
	public class Item : GameObject
	{
		public enum WearType : int
		{
			None = -1,
			RightHand = 0,
			LeftHand = 1,
			Armor = 2,
			Helm = 3,
			Glove = 4,
			Boots = 5,
			Belt = 6,
			Mantle = 7,
			Necklace = 8,
			Ring = 9,
			SecondRing = 10,
			Ear = 11,
			Face = 12,
			Hair = 13,
			DecoWeapon = 14,
			DecoShield = 15,
			DecoArmor = 16,
			DecoHelm = 17,
			DecoGlove = 18,
			DecoBoots = 19,
			DecoMantle = 20,
			DecoShoulder = 21,
			RideItem = 21,
			BagSlot = 22,
			TwoFingerRing = 94,
			TwoHand = 99,
			Skill = 100,
			// This must always be the last one
			// for variable initialization
			WearType_Max
		}

		public int Code { get; set; }
		public long UId { get; set; }
		public long Count { get; set; }
		public byte Enhance { get; set; }
		public byte Level { get; set; }
		public int WearInfo { get; set; }
		//public int IdX { get; set; }

		public Item(uint pHandle) : base(pHandle, GameObjectType.Player) {
			
		}

		public Item() {}
	}
}
