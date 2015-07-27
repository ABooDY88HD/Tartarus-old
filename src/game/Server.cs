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

			ItemDb.Start();
			StatsDb.Start();
			//QuestDb.Start();

			RegionMngr.Start();

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
			if (!player.LoadCharacter(charName))
			{
				return;
			}
			else if (!player.LoadInventory())
			{
				return;
			}

			ClientPacketHandler.send_UrlList(player, UrlList);
			ClientPacketHandler.send_Property(player, "max_havoc", Globals.MaxHavoc);
			ClientPacketHandler.send_Property(player, "max_chaos", Globals.MaxChaos);
			ClientPacketHandler.send_Property(player, "max_stamina", Globals.MaxStamina);
			ClientPacketHandler.send_Property(player, "max_havoc", Globals.MaxHavoc);
			ClientPacketHandler.send_Property(player, "max_chaos", Globals.MaxChaos);
			ClientPacketHandler.send_Property(player, "max_stamina", Globals.MaxStamina);
			ClientPacketHandler.send_Property(player, "max_havoc", Globals.MaxHavoc);
			ClientPacketHandler.send_Property(player, "max_chaos", Globals.MaxChaos);
			ClientPacketHandler.send_Property(player, "max_stamina", Globals.MaxStamina);

			//PacketParser.send_Packet02(sid, 10262);

			//PacketParser.send_Packet22(sid, 1);

			ClientPacketHandler.send_Property(player, "job", player.Job);
			ClientPacketHandler.send_Property(player, "job_level", player.JobLevel);
			ClientPacketHandler.send_Property(player, "job_0", 0);
			ClientPacketHandler.send_Property(player, "jlv_0", 0);
			ClientPacketHandler.send_Property(player, "job_1", 0);
			ClientPacketHandler.send_Property(player, "jlv_1", 0);
			ClientPacketHandler.send_Property(player, "job_2", 0);
			ClientPacketHandler.send_Property(player, "jlv_2", 0);
			ClientPacketHandler.send_Property(player, "max_havoc", Globals.MaxHavoc);
			ClientPacketHandler.send_Property(player, "max_chaos", Globals.MaxChaos);
			ClientPacketHandler.send_Property(player, "max_stamina", Globals.MaxStamina);

			//PacketParser.send_Packet10(sid, 4);

			ClientPacketHandler.send_Property(player, "job", player.Job, true);
			
			//PacketParser.send_OpenPopUp(sid, "www.google.com");

			ClientPacketHandler.send_Property(player, "x", player.X, true);
			ClientPacketHandler.send_Property(player, "y", player.Y, true);
			ClientPacketHandler.send_Property(player, "layer", player.Layer, true);
			
			//PacketParser.send_Packet636(sid, 1);

			ClientPacketHandler.send_LoginResult(player);

			ClientPacketHandler.send_Property(player, "max_havoc", Globals.MaxHavoc);
			ClientPacketHandler.send_Property(player, "max_chaos", Globals.MaxChaos);
			ClientPacketHandler.send_Property(player, "max_stamina", Globals.MaxStamina);

			ClientPacketHandler.send_InventoryList(player);

			ClientPacketHandler.send_EquipSummon(player);
			ClientPacketHandler.send_CharacterView(player);

			ClientPacketHandler.send_UpdateGoldChaos(player);

			ClientPacketHandler.send_Property(player, "tp", 0);
			ClientPacketHandler.send_Property(player, "chaos", 0);

			ClientPacketHandler.send_UpdateLevel(player);
			ClientPacketHandler.send_UpdateExp(player);

			ClientPacketHandler.send_Property(player, "job", player.Job);

			ClientPacketHandler.send_Property(player, "job_level", player.JobLevel);
			ClientPacketHandler.send_Property(player, "job_0", 0);
			ClientPacketHandler.send_Property(player, "jlv_0", 0);
			ClientPacketHandler.send_Property(player, "job_1", 0);
			ClientPacketHandler.send_Property(player, "jlv_1", 0);
			ClientPacketHandler.send_Property(player, "job_2", 0);
			ClientPacketHandler.send_Property(player, "jlv_2", 0);

			ClientPacketHandler.send_Packet404(player);

			ClientPacketHandler.send_Packet1005(player);
			ClientPacketHandler.send_BeltSlotInfo(player);
			ClientPacketHandler.send_GameTime(player);

			ClientPacketHandler.send_Property(player, "huntaholic_point", 0);
			ClientPacketHandler.send_Property(player, "ap", 0);
			ClientPacketHandler.send_Property(player, "huntaholic_ent", 12);

			ClientPacketHandler.send_Packet4700(player);

			ClientPacketHandler.send_Property(player, "alias", 0);
			ClientPacketHandler.send_Property(player, "ethereal_stone", 0);
			ClientPacketHandler.send_Property(player, "dk_count", 0);
			ClientPacketHandler.send_Property(player, "pk_count", 0);
			ClientPacketHandler.send_Property(player, "pk_count", 0);
			ClientPacketHandler.send_Property(player, "immoral", 0);

			//PacketParser.send_Packet03(sid, 1, player);

			ClientPacketHandler.send_Property(player, "stamina", 0);
			ClientPacketHandler.send_Property(player, "max_stamina", Globals.MaxStamina);
			ClientPacketHandler.send_Property(player, "channel", 1);
			
			ClientPacketHandler.send_EntityState(player);

			ClientPacketHandler.send_Property(player, "client_info", player.ClientInfo, true);

			ClientPacketHandler.send_QuestList(player);
			ClientPacketHandler.send_Packet625(player);
			ClientPacketHandler.send_Packet626(player);
			ClientPacketHandler.send_Packet627(player);
			ClientPacketHandler.send_Packet629(player);

			ClientPacketHandler.send_Packet631(player, 1);
			ClientPacketHandler.send_Packet631(player, 2);
			ClientPacketHandler.send_Packet631(player, 3);
			ClientPacketHandler.send_Packet631(player, 4);
			ClientPacketHandler.send_Packet631(player, 5);

			//PacketParser.send_Packet22(sid, 2);
			//PacketParser.send_Packet22(sid, 3);

			ClientPacketHandler.send_Property(player, "playtime", 0);
			ClientPacketHandler.send_Property(player, "playtime_limit1", 1080000);
			ClientPacketHandler.send_Property(player, "playtime_limit2", 1800000);

			ClientPacketHandler.send_LocationInfo(player);
			ClientPacketHandler.send_WeatherInfo(player);

			ClientPacketHandler.send_Property(player, "playtime", 0);

			//PacketParser.send_Packet22(sid, 4);

			//PacketParser.send_Packet08(sid, 1);

			ClientPacketHandler.send_GameTime(player);
			//PacketParser.send_Packet1101(player, 2);

			ClientPacketHandler.send_LocationInfo(player);

			ClientPacketHandler.send_Property(player, "stamina_regen", 30);

			ClientPacketHandler.send_UpdateStats(player, false);
			ClientPacketHandler.send_UpdateStats(player, true);

			ClientPacketHandler.send_Property(player, "max_havoc", Globals.MaxHavoc);
			ClientPacketHandler.send_Property(player, "max_chaos", Globals.MaxChaos);
			ClientPacketHandler.send_Property(player, "max_stamina", Globals.MaxStamina);
		}
	}
}
