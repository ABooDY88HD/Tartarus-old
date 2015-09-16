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
using common.Utilities;

namespace game
{
	public class Monster : CreatureObject
	{
		public int Id { get; set; }

		public Monster(uint pHandle) : base(pHandle, GameObjectSubType.Mob) {
			
		}

		public Monster() {}

		public static void SpawnMonster(int mobId, Point position)
		{
			Monster mob = GObjectManager.GetNewMob();
			mob.Id = mobId;
			mob.MaxHp = MonsterDb.DB[mobId].Hp;
			mob.MaxMp = MonsterDb.DB[mobId].Mp;
			mob.Hp = mob.MaxHp;
			mob.Mp = mob.MaxMp;
			mob.Position = position;

			RegionMngr.AddMobToRegion(mob);
		}

		internal void Update()
		{
			lock (this)
			{
				int movSpd = 10;
				int deltaTime = (Environment.TickCount);
				deltaTime -= this.LastUpdate;

				float toX = this.Position.X + (new Random().Next(-movSpd * deltaTime, movSpd * deltaTime) / 1000);
				float toY = this.Position.Y + (new Random().Next(-movSpd * deltaTime, movSpd * deltaTime) / 1000);

				this.LastUpdate = Environment.TickCount;

				ClientPacketHandler.send_PCMoveTo(this, new Point(toX, toY));
			}
		}
	}
}
