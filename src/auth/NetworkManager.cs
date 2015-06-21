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

namespace auth
{
	public class NetworkManager
	{
		public NetworkManager()
		{
			ServerPacketHandler.Start();
			ClientPacketHandler.Start();
		}

		internal void InitServerListener()
		{
			BackgroundWorker bwSvListener = new BackgroundWorker();
			bwSvListener.WorkerSupportsCancellation = true;
			bwSvListener.DoWork += new DoWorkEventHandler(StartServerListener);

			bwSvListener.RunWorkerAsync();
		}

		private void StartServerListener(object sender, DoWorkEventArgs e)
		{
			Socket serverListenerSocket;

			ConsoleUtils.Write(ConsoleMsgType.Status, "Listening to Game Servers on port {0}\r\n", Settings.GameServerPort);
			serverListenerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			serverListenerSocket.Bind(new IPEndPoint(IPAddress.Parse(Settings.ServerIP), Settings.GameServerPort));
			serverListenerSocket.Listen(Byte.MaxValue);
			while (true)
				this.CreateServerManager(serverListenerSocket.Accept());
		}

		private void CreateServerManager(Socket socket)
		{
			GameServer newManager = new GameServer(socket);
			ConsoleUtils.Write(
				ConsoleMsgType.Info,
				"Server with index {0} Connected. (At: {1}:{2})\n",
				newManager.Index,
				newManager.Ip,
				newManager.Port
			);
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
