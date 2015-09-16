// Copyright (c) Tartarus Dev Team, licensed under GNU GPL.
// See the LICENSE file
using common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace game
{
	public static class GObjectManager
	{
		public static Dictionary<uint, Player> Players;
		public static Dictionary<uint, Item> Items;
		public static Dictionary<uint, Npc> Npcs;
		public static Dictionary<uint, Monster> Monsters;

		/* ********************************
		 * Rappelz seems to use intervals of handles for
		 * each type of GameObject, Tartarus currently just
		 * goes from 1 for everything.
		 * Does this cause problems on client side?
		 * *********************************/

		private static List<uint> HandlePool;
		private static uint NextHandle;

		public static void Start()
		{
			ConsoleUtils.Write(ConsoleMsgType.Status, "Initializing Game Object Manager\n");
			
			Players = new Dictionary<uint, Player>(Settings.MaxConnections);
			Items = new Dictionary<uint, Item>();
			Npcs = new Dictionary<uint, Npc>();
			Monsters = new Dictionary<uint, Monster>();

			HandlePool = new List<uint>();
			NextHandle = 1;
			
			ConsoleUtils.Write(ConsoleMsgType.Status, "Game Object Manager Initialized\n");
		}

		public static Player GetNewPlayer()
		{
			uint handle = GetFreeHandle();
			Player p = new Player(handle);
			Players.Add(handle, p);
			return p;
		}

		private static uint GetFreeHandle()
		{
			if (HandlePool.Count > 0)
			{
				uint handle = HandlePool[0];
				HandlePool.RemoveAt(0);
				return handle;
			}
			else
			{
				return NextHandle++;
			}
		}

		public static void Destroy(GameObject entity)
		{

		}

		internal static Item GetNewItem()
		{
			uint handle = GetFreeHandle();
			Item p = new Item(handle);
			Items.Add(handle, p);
			return p;
		}

		internal static Npc GetNewNpc()
		{
			uint handle = GetFreeHandle();
			Npc p = new Npc(handle);
			Npcs.Add(handle, p);
			return p;
		}

		internal static Monster GetNewMob()
		{
			uint handle = GetFreeHandle();
			Monster p = new Monster(handle);
			Monsters.Add(handle, p);
			return p;
		}

		internal static void Update()
		{
			// TODO walking
			// TODO npc walking
			// TODO drop deletion
			// TODO change weather
			// Updates Monster
			foreach (Monster m in Monsters.Values)
			{
				m.Update();
			}
		}
	}
}
