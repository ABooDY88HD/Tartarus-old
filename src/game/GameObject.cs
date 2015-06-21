// Copyright (c) Tartarus Dev Team, licensed under GNU GPL.
// See the LICENSE file
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace game
{
	public enum GameObjectType
	{
		Player,
		Item
	}

	public abstract class GameObject
	{
		public uint Handle { get; private set; }
		public GameObjectType Type { get; private set; }

		public float X = 0f;
		public float Y = 0f;
		public int Layer = 0;

		public GameObject(uint pHandle, GameObjectType pType)
		{
			this.Handle = pHandle;
			this.Type = pType;
		}

		public GameObject(){}
	}
}
