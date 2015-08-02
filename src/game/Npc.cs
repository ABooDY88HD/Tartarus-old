using common;
using MySql.Data.MySqlClient;
// Copyright (c) Tartarus Dev Team, licensed under GNU GPL.
// See the LICENSE file
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace game
{
	public class Npc : GameObject
	{
		public enum DialogType : int
		{
			Npc = 0x0,
			DungeonStone = 0x1,
			DonationProp = 0x2,
			QuestInfoAndStart = 0x3,
			AuctionWindow = 0x4,
			HuntaholicLobby = 0x5,
			DonationRewardNotify = 0x6,
			QuestInfoInProgress = 0x7,
			QuestInfoAndEnd = 0x8
		}

		public int Id { get; set; }
		public sbyte Face { get; set; }
		public string ContactScript { get; set; }

		public Npc(uint pHandle) : base(pHandle, GameObjectType.NPC, GameObjectSubType.NPC) {
			
		}

		public static void Init()
		{
			ConsoleUtils.Write(ConsoleMsgType.Status, "Loading NPC Database...\n");

			Database db = new Database(Server.GameDbConString);
			MySqlDataReader reader =
				db.ReaderQuery(
					"SELECT `id`, `x`, `y`, `face`, `script` " +
					"FROM `npc_db`", null, null
				);

			while (reader.Read())
			{
				Npc npc = GObjectManager.GetNewNpc();

				npc.Id = (int)reader["id"];
				npc.Position = new Point((int)reader["x"], (int)reader["y"]);
				npc.Face = (sbyte)reader["face"];
				npc.ContactScript = (string)reader["script"];
			}

			ConsoleUtils.Write(ConsoleMsgType.Status, "NPC Database Loaded.\n");
		}
	}
}
