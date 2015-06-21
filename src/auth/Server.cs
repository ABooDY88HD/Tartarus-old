// Copyright (c) Tartarus Dev Team, licensed under GNU GPL.
// See the LICENSE file
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql;
using common;

namespace auth
{
	public class Server
	{
		public static string AuthDbConString;
		public GameServer[] ServerList;

		private static Server Instance;

		public Server()
		{
			Instance = this;
			this.Start();
		}

		private void Start()
		{
			AuthDbConString = "Server=" + Settings.SqlIp + "; Database=" + Settings.SqlDatabase + "; Uid=" + Settings.SqlUsername + "; Pwd=" + Settings.SqlPassword + ";";
			
			// Test database connection
			ConsoleUtils.Write(ConsoleMsgType.Status, "Testing MySQL Connections...\n");
			Database db = new Database(AuthDbConString);
			if (!db.Test())
			{
				ConsoleUtils.Write(ConsoleMsgType.Info, "Fix the errors and restart the server. Press any key to exit.\n");
				Console.ReadKey();
				return;
			}
			ConsoleUtils.Write(ConsoleMsgType.Status, "MySQL Connections Test OK\n");
			
			// Start Network Manager
			ConsoleUtils.Write(ConsoleMsgType.Status, "Initializing Network\n");

			NetworkManager network = new NetworkManager();

			ConsoleUtils.Write(ConsoleMsgType.Status, "Network Initialized Successfully\n");

			// Start Server Listener
			ConsoleUtils.Write(ConsoleMsgType.Status, "Initializing Server Acceptor\n");

			ServerList = new GameServer[Byte.MaxValue];
			network.InitServerListener();

			ConsoleUtils.Write(ConsoleMsgType.Status, "Server Acceptor Initialized\n");
			
			// Start Client Listener
			ConsoleUtils.Write(ConsoleMsgType.Status, "Initializing Client Acceptor\n");

			network.InitClientListener();

			ConsoleUtils.Write(ConsoleMsgType.Status, "Client Acceptor Initialized\n");

			ConsoleUtils.Write(ConsoleMsgType.Status, "Auth Server is ready.\n");

		}

		private void RemoveGameServer(byte index)
		{
			if (ServerList[index] != null)
			{
				ServerList[index] = null;
			}
		}

		private void AddGameServer(GameServer game)
		{
			ServerList[game.Index] = game;
		}

		private void UserJoinGame(Client client, byte server_index)
		{
			byte[] key = { 0x00, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16 }; // TODO : Generate a key
			ServerPacketHandler.send_ClientJoin(ServerList[server_index], client, key);
			ClientPacketHandler.send_JoinGame(client, key);
		}

		/* *************************
		 *		Static
		 * *************************/
		internal static void OnGameServerDisconnect(byte index)
		{
			Server.Instance.RemoveGameServer(index);
		}

		internal static void OnGameServerConnect(GameServer game, int index, string name, string image, string ip, short port, string key)
		{
			if (!key.Equals(Settings.AcceptorKey))
			{
				ConsoleUtils.Write(
					ConsoleMsgType.Warning,
					"Invalid Server Acceptor Key received from {0}:{1}. '{2}' received ; '{3}' expected.",
					ip, port, key, Settings.AcceptorKey
					);
				ServerPacketHandler.send_ServerConnectionResult(game, false);
			}

			game.Ip = System.Net.IPAddress.Parse(ip);
			game.Port = port;
			game.Index = (byte)index;
			game.Name = name;
			game.NoticeUrl = image;

			Server.Instance.AddGameServer(game);
			ServerPacketHandler.send_ServerConnectionResult(game, true);
		}

		internal static void OnUserRequestServerList(Client client)
		{
			ClientPacketHandler.send_ServerList(client, Server.Instance.ServerList);
		}

		internal static void OnUserJoinGame(Client client, byte server_index)
		{
			Server.Instance.UserJoinGame(client, server_index);
		}

	}
}
