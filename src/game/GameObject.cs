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
	/// <summary>
	/// Player Races
	/// </summary>
	public enum Races
	{
		Deva = 3,
		Gaia = 4,
		Asura = 5,
	}

	public enum GameObjectType
	{
		Player = 0,
		NPC = 1,
		StaticObject = 2
	}

	public enum GameObjectSubType
	{
		Player = 0,
		NPC = 1,
		Item = 2,
		Mob = 3,
		Summon = 4,
		SkillProp = 5,
		FieldProp = 6,
		Pet = 7,
	}

	public abstract class GameObject
	{
		public uint Handle { get; private set; }
		public GameObjectType Type { get; private set; }
		public GameObjectSubType SubType { get; private set; }

		public Point Position { get; set; }
		public byte Layer = 0;

		public GameObject(uint pHandle, GameObjectType pType, GameObjectSubType subType)
		{
			this.Handle = pHandle;
			this.Type = pType;
			this.SubType = subType;
		}

		public GameObject(){}
	}
}
