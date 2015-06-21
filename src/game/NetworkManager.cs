// Copyright (c) Tartarus Dev Team, licensed under GNU GPL.
// See the LICENSE file
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using common;
using System.Net;

namespace game
{
	public class NetworkManager
	{
		public NetworkManager()
		{
			ServerPacketHandler.Start();
			ClientPacketHandler.Start();
		}

		internal void ConnectToAuthServer()
		{
			ConsoleUtils.Write(ConsoleMsgType.Info, "Connecting to Auth-Server...\n");

			try
			{
				IPEndPoint ip_endp = new IPEndPoint(IPAddress.Parse(Settings.AuthIP), Settings.AuthPort);
				Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
				server.Connect(ip_endp);
				AuthServer auth = new AuthServer(server);

				ServerPacketHandler.send_ListServer(auth);
			}
			catch (Exception e)
			{
				ConsoleUtils.Write(ConsoleMsgType.FatalError, "Can't connect to auth-server at {0}:{1}. Check your settings.\n", Settings.AuthIP, Settings.AuthPort);
				ConsoleUtils.Write(ConsoleMsgType.FatalError, "Error Message: {0}\n", e.Message);
			}
		}

		internal void InitClientListener()
		{
			ClientManager clientMngr = new ClientManager();

			BackgroundWorker bwListener = new BackgroundWorker();
			bwListener.WorkerSupportsCancellation = true;
			bwListener.DoWork += new DoWorkEventHandler(clientMngr.Start);

			bwListener.RunWorkerAsync();
		}
	}
}
