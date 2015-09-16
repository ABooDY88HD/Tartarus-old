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
	public enum Races : byte
	{
		None = 0,
		Deva = 3,
		Gaia = 4,
		Asura = 5,
	}

	public enum GameObjectType : byte
	{
		Player = 0,
		Creature = 1,
		StaticObject = 2
	}

	public enum GameObjectSubType : byte
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

		public int LastUpdate = Environment.TickCount/10;

		public GameObject(uint pHandle, GameObjectType pType, GameObjectSubType subType)
		{
			this.Handle = pHandle;
			this.Type = pType;
			this.SubType = subType;
		}

		public GameObject(){}
	}

	public abstract class CreatureObject : GameObject
	{
		public uint Status { get; set; }
		public float FaceDirection { get; set; }
		public int Hp { get; set; }
		public int MaxHp { get; set; }
		public int Mp { get; set; }
		public int MaxMp { get; set; }
		public int Level { get; set; }
		public Races Race { get; set; }
		public uint SkinColor { get; set; }
		public int Energy { get; set; }

		public CreatureObject(uint pHandle, GameObjectSubType subType) : base (pHandle, GameObjectType.Creature, subType)
		{
			this.Status = 0;
			this.FaceDirection = 0;
			this.Hp = 100;
			this.MaxHp = 100;
			this.Mp = 0;
			this.MaxMp = 0;
			this.Level = 1;
			this.Race = Races.None;
			this.SkinColor = 0;
			this.Energy = 0;
		}

		public CreatureObject()
		{

		}
	}
}
