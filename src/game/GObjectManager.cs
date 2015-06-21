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

		private static List<uint> HandlePool;
		private static uint NextHandle;

		public static void Start()
		{
			ConsoleUtils.Write(ConsoleMsgType.Status, "Initializing Game Object Manager\n");
			
			Players = new Dictionary<uint, Player>(Settings.MaxConnections);
			
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
			throw new NotImplementedException();
		}
	}
}
