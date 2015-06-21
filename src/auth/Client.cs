// Copyright (c) Tartarus Dev Team, licensed under GNU GPL.
// See the LICENSE file
using common;
using common.RC4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace auth
{
	public class Client
	{
		public class Network
		{
			public Socket ClSocket { get; set; }
			public XRC4Cipher Encoder { get; set; }
			public XRC4Cipher Decoder { get; set; }
			public byte[] Buffer;
			public int PacketSize;
			public int Offset;
			public PacketStream Data;
		}

		public string UserId;
		public int AccountId;
		public byte Permission;

		public Network NetData;
		
		public Client() {
			NetData = new Network();
		}

		public void TryLogin(string username, string password)
		{
			if (Settings.LoginDebug)
				ConsoleUtils.Write(ConsoleMsgType.Info, "User {0} is trying to login\n", username);
			Database db = new Database(Server.AuthDbConString);
			MySqlDataReader reader = db.ReaderQuery(
				"SELECT `account_id`, `permission` FROM `login` WHERE `userid` = @userid AND `password` = @password",
				new string[] { "userid", "password" },
				new object[] { username, password }
			);
			if (!reader.HasRows)
			{
				ClientPacketHandler.send_LoginResult(this, Packets.LoginResult.LOGINRESULT_FAIL);
				if (Settings.LoginDebug)
					ConsoleUtils.Write(ConsoleMsgType.Info, "User {0} login refused (Invalid Credentials)\n", username);
				return;
			}
			
			reader.Read();
			this.AccountId = (int)reader["account_id"];
			this.Permission = (byte)reader["permission"];
			this.UserId = (String)username;

			if (Settings.LoginDebug)
				ConsoleUtils.Write(ConsoleMsgType.Info, "User {0} login accepted\n", username);
			ClientPacketHandler.send_LoginResult(this, Packets.LoginResult.LOGINRESULT_SUCCESS);
		}
	}
}
