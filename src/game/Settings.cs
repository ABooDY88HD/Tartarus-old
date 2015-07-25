// Copyright (c) Tartarus Dev Team, licensed under GNU GPL.
// See the LICENSE file
using common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace game
{
	public static class Settings
	{
		// Game Server Settings
		public static String ServerIP;
		public static Int16 Port;
		public static String AcceptorKey;
		public static Int16 MaxConnections;

		public static Byte ServerIndex;
		public static String ServerName;
		public static String NoticeUrl;

		// Auth-Server listener
		public static String AuthIP;
		public static Int16 AuthPort;

		// Game Database Settings
		public static String GameSqlIp;
		public static Int16 GameSqlPort;
		public static String GameSqlDatabase;
		public static String GameSqlUsername;
		public static String GameSqlPassword;

		// User Database Settings
		public static String UserSqlIp;
		public static Int16 UserSqlPort;
		public static String UserSqlDatabase;
		public static String UserSqlUsername;
		public static String UserSqlPassword;

		// Map Settings
		public static Int32 MapLengthX;
		public static Int32 MapLengthY;
		public static Boolean CheckCollision;
		/// <summary>
		/// Types of Settings
		/// </summary>
		private enum DType
		{
			String,
			Bool,
			Byte,
			Int16,
			Int32
		}

		/// <summary>
		/// Parses the values from the dictionary and
		/// add them to the variables
		/// </summary>
		/// <param name="settings">settings dictionary</param>
		public static void Set(Dictionary<string, string> settings)
		{
			// Server Settings
			ServerIP = (String) ParseSetting(ref settings, DType.String, "server_ip", "127.0.0.1");
			Port = (Int16)ParseSetting(ref settings, DType.Int16, "listen_port", (Int16) 8000);
			AcceptorKey = (String)ParseSetting(ref settings, DType.String, "acceptor_key", "1234");
			MaxConnections = (Int16)ParseSetting(ref settings, DType.Int16, "max_connections", (Int16)1000);

			ServerIndex = (Byte)ParseSetting(ref settings, DType.Byte, "server_index", (Byte)1);
			ServerName = (String)ParseSetting(ref settings, DType.String, "server_name", "Tartarus");
			NoticeUrl = (String)ParseSetting(ref settings, DType.String, "notice_url", "http://127.0.0.1/notice.htm");

			// Auth Settings
			AuthIP = (String)ParseSetting(ref settings, DType.String, "auth_ip", "127.0.0.1");
			AuthPort = (Int16)ParseSetting(ref settings, DType.Int16, "auth_port", (Int16)4444);

			// Loads default SQL Settings
			String defaultSqlHost = (String) ParseSetting(ref settings, DType.String, "sql.db_hostname", "127.0.0.1");
			Int16 defaultSqlPort = (Int16)ParseSetting(ref settings, DType.Int16, "sql.db_port", (Int16)3306);
			String defaultSqlUser = (String) ParseSetting(ref settings, DType.String, "sql.db_username", "rappelz");
			String defaultSqlPass = (String) ParseSetting(ref settings, DType.String, "sql.db_password", "rappelz");
			String defaultSqlDb = (String) ParseSetting(ref settings, DType.String, "sql.db_database", "rappelz");

			// Game (Static) Database Settings
			GameSqlIp = (String) ParseSetting(ref settings, DType.String, "sql.static_hostname", defaultSqlHost, true);
			GameSqlPort = (Int16)ParseSetting(ref settings, DType.Int16, "sql.static_port", defaultSqlPort, true);
			GameSqlUsername = (String) ParseSetting(ref settings, DType.String, "sql.static_username", defaultSqlUser, true);
			GameSqlPassword = (String) ParseSetting(ref settings, DType.String, "sql.static_password", defaultSqlPass, true);
			GameSqlDatabase = (String) ParseSetting(ref settings, DType.String, "sql.static_database", defaultSqlDb, true);

			// User (user) Database Settings
			UserSqlIp = (String)ParseSetting(ref settings, DType.String, "sql.user_hostname", defaultSqlHost, true);
			UserSqlPort = (Int16)ParseSetting(ref settings, DType.Int16, "sql.user_port", defaultSqlPort, true);
			UserSqlUsername = (String)ParseSetting(ref settings, DType.String, "sql.user_username", defaultSqlUser, true);
			UserSqlPassword = (String)ParseSetting(ref settings, DType.String, "sql.user_password", defaultSqlPass, true);
			UserSqlDatabase = (String)ParseSetting(ref settings, DType.String, "sql.user_database", defaultSqlDb, true);

			// Map Settings
			MapLengthX = (Int32)ParseSetting(ref settings, DType.Int32, "map_length_x", 0, false);
			MapLengthY = (Int32)ParseSetting(ref settings, DType.Int32, "map_length_y", 0, false);
			CheckCollision = (Boolean)ParseSetting(ref settings, DType.Bool, "check_collision", true, false);
		}

		/// <summary>
		/// Parse a setting by converting to its value type
		/// and adding defaults when necessary
		/// </summary>
		/// <param name="settings">the settings dictionary</param>
		/// <param name="type">type of return</param>
		/// <param name="name">name of the setting</param>
		/// <param name="defaultValue">default value to be used when setting can't be</param>
		/// <returns></returns>
		private static object ParseSetting(ref Dictionary<string, string> settings, DType type, string name, object defaultValue, bool optional = false)
		{
			if (!settings.ContainsKey(name))
			{
				if (!optional)
				{
					ConsoleUtils.Write(ConsoleMsgType.Warning, "Couldn't find config {0}\n", name);
					return defaultValue;
				}
			}

			switch (type)
			{
				case DType.Bool:
					return GetBool(settings[name]);

				case DType.Byte:
					return GetByte(settings[name], (byte)defaultValue);

				case DType.Int16:
					return GetInt16(settings[name], (short)defaultValue);

				case DType.Int32:
					return GetInt32(settings[name], (int)defaultValue);

				case DType.String:
					return settings[name];

				default:
					return defaultValue;
			}
		}

		private static Boolean GetBool(string value)
		{
			if (value.Equals("1")) return true;
			else return false;
		}

		private static Int32 GetInt32(string value, Int32 defaultVal)
		{
			int val;
			if (Int32.TryParse(value, out val))
			{
				return val;
			}
			else
			{
				ConsoleUtils.Write(ConsoleMsgType.Warning, "Couldn't parse value {0}, defaulting to {1}\n", value, defaultVal);
				return defaultVal;
			}
		}

		private static Int16 GetInt16(string value, Int16 defaultVal)
		{
			short val;
			if (Int16.TryParse(value, out val))
			{
				return val;
			}
			else
			{
				ConsoleUtils.Write(ConsoleMsgType.Warning, "Couldn't parse value {0}, defaulting to {1}\n", value, defaultVal);
				return defaultVal;
			}
		}

		private static Byte GetByte(string value, Byte defaultVal)
		{
			byte val;
			if (Byte.TryParse(value, out val))
			{
				return val;
			}
			else
			{
				ConsoleUtils.Write(ConsoleMsgType.Warning, "Couldn't parse value {0}, defaulting to {1}\n", value, defaultVal);
				return defaultVal;
			}
		}
	}
}
