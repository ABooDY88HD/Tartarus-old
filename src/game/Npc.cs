// Copyright (c) Tartarus Dev Team, licensed under GNU GPL.
// See the LICENSE file
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace game
{
	public class Npc : GameObject
	{
		public int Id { get; set; }

		public Npc(uint pHandle) : base(pHandle, GameObjectType.NPC, GameObjectSubType.NPC) {
			
		}
	}
}
