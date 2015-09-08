// Copyright (c) Tartarus Dev Team, licensed under GNU GPL.
// See the LICENSE file
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql;
using common;

namespace game
{
	public class Server
	{
		public static string GameDbConString;
		public static string UserDbConString;
		public static string UrlList;

		public static Dictionary<string, UserJoinData> UserJoinPool;
		public static Dictionary<string, int> UserIds;

		public class UserJoinData
		{
			public byte[] Key { get; set; }
			public int AccountId { get; set; }
			public byte Permission { get; set; }
		}

		private static Server Instance;
		private NetworkManager Network;

		public Server()
		{
			Instance = this;
			this.Start();
		}

		private void Start()
		{
			GameDbConString = "Server=" + Settings.GameSqlIp + "; Database=" + Settings.GameSqlDatabase + "; Uid=" + Settings.GameSqlUsername + "; Pwd=" + Settings.GameSqlPassword + ";";
			UserDbConString = "Server=" + Settings.UserSqlIp + "; Database=" + Settings.UserSqlDatabase + "; Uid=" + Settings.UserSqlUsername + "; Pwd=" + Settings.UserSqlPassword + ";";

			// TODO : Put this in settings
			/* Urls
			 * guild.url : http://guild.gamepower7.com/client/guild/login.aspx
			 * guild_test_download.url : upload/client/guild/
			 * web_download : guild.gamepower7.com
			 * web_download_port : 0
			 * shop.url : http://khroos.gamepower7.com/khroos/
			 * ghelp_url : http://help.gamepower7.com/help/help-page/help-page.html
			 * guild_icon_upload.ip : 95.211.112.10
			 * guild_icon_upload.port : 4617
			 * guild_icon_upload.url : http://guild.gamepower7.com/client/guild//iconupload.aspx
			 */
			string[] urls = new string[]
			{
				"guild.url", "http://guild.gamepower7.com/client/guild/login.aspx",
				"guild_test_download.url", "upload/client/guild/",
				"web_download", "guild.gamepower7.com",
				"web_download_port", "0",
				"shop.url", "http://khroos.gamepower7.com/khroos/",
				"ghelp_url", "http://help.gamepower7.com/help/help-page/help-page.html",
				"guild_icon_upload.ip", "95.211.112.10",
				"guild_icon_upload.port", "4617",
				"guild_icon_upload.url", "http://guild.gamepower7.com/client/guild//iconupload.aspx"
			};
			UrlList = String.Join("|", urls);

			/* ************************* *
			 * Test database connection
			 * ************************* */
			ConsoleUtils.Write(ConsoleMsgType.Status, "Testing MySQL Connections...\n");
			Database db = new Database(GameDbConString);
			if (!db.Test())
			{
				ConsoleUtils.Write(ConsoleMsgType.Info, "Fix the errors and restart the server. Press any key to exit.\n");
				Console.ReadKey();
				return;
			}
			ConsoleUtils.Write(ConsoleMsgType.Status, "MySQL Connections Test OK\n");

			/* ************************* *
			 * Load Game Data
			 * ************************* */
			// Start Game Object Manager
			GObjectManager.Start();

			RegionMngr.Start();

			Script.LuaMain.Start();
			ItemDb.Start();
			StatsDb.Start();
			Npc.Init();
			QuestDb.Start();


			/* ************************* *
			 * Start Network Manager
			 * ************************* */
			ConsoleUtils.Write(ConsoleMsgType.Status, "Initializing Network\n");

			UserJoinPool = new Dictionary<string, UserJoinData>();
			UserIds = new Dictionary<string, int>();

			Network = new NetworkManager();
			Network.ConnectToAuthServer();
			Network.InitClientListener();

			ConsoleUtils.Write(ConsoleMsgType.Status, "Network Initialized\n");
		}

		/* *************************
		 *		Static
		 * *************************/
		internal static void OnListServerResult(AuthServer server, bool result)
		{
			if (!result)
			{
				ConsoleUtils.Write(ConsoleMsgType.Error, "Can't stabilish connection to Auth-Server\n");
			}
		}

		internal static void OnUserJoin(Player player, string userId, byte[] key)
		{
			if (!UserJoinPool.ContainsKey(userId) || !UserJoinPool[userId].Key.SequenceEqual(key))
			{
				// TODO : Invalid User - disconect
				Console.Write("User Authentication Failed\n");
				return;
			}
			player.UserId = userId;
			player.AccountId = UserJoinPool[userId].AccountId;
			player.Permission = UserJoinPool[userId].Permission;

			UserJoinPool.Remove(userId);

			ClientPacketHandler.send_PacketResponse(player, 0x07D5);
		}

		internal static void OnAuthNoticeUser(AuthServer server, string userId, int accId, byte permission, byte[] key)
		{
			if (UserJoinPool.ContainsKey(userId))
			{
				UserJoinPool[userId] = new UserJoinData() { Key = key, AccountId = accId, Permission = permission };
			}
			else
			{
				UserJoinPool.Add(userId, new UserJoinData() { Key = key, AccountId = accId, Permission = permission });
			}

			if (UserIds.ContainsKey(userId))
			{
				UserIds[userId] = accId;
			}
			else
			{
				UserIds.Add(userId, accId);
			}
		}

		internal static void OnUserJoinGame(Player player, string charName)
		{
			ClientPacketHandler.send_UrlList(player, UrlList);
			
			if (!player.LoadCharacter(charName))
			{
				return;
			}
		}
	}
}
