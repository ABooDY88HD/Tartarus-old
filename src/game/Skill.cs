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
	public class Skill
	{
		public Int32 Id { get; set; }
		public Byte Level { get; set; }
		public UInt32 Cooldown { get; set; }
		public UInt32 TotalCooldown { get; set; }

		public static void LevelUp(GameObject obj, int skillId, byte targetLv)
		{
			if (!SkillDb.DB.ContainsKey(skillId)) // Skill doesn't exists
				return;

			if (obj.Type == GameObjectType.Player)
			{
				Player player = (Player)obj;

				short curLevel = 0;
				bool haveSkill = false;

				if (player.SkillList.ContainsKey(skillId))
				{
					curLevel = player.SkillList[skillId].Level;
					haveSkill = true;
				}

				int jpCost = SkillDb.DB[skillId].RequiredJp[targetLv];
				
				if (jpCost > player.JP)
					return;

				player.JP -= jpCost;
				if (haveSkill)
				{
					Database db = new Database(Server.UserDbConString);
					player.SkillList[skillId].Level = targetLv;

					db.WriteQuery(
						"UPDATE `skill` SET `level` = @level WHERE `char_id` = @cid AND `id` = @skid",
						new string[] { "level", "cid", "skid" },
						new object[] { player.SkillList[skillId].Level, player.CharId, skillId }
					);
				}
				else
				{
					Skill skill = new Skill() { Id = skillId, Level = targetLv, TotalCooldown = SkillDb.DB[skillId].CooldownTime };
					Database db = new Database(Server.UserDbConString);

					player.SkillList.Add(skillId, skill);

					db.WriteQuery(
						"INSERT INTO `skill` (`char_id`, `id`, `level`) VALUES (@cid, @skid, @level)",
						new string[] { "level", "cid", "skid" },
						new object[] { player.SkillList[skillId].Level, player.CharId, skillId }
					);
				}

				ClientPacketHandler.send_Property(player, "tp", player.TP);
				ClientPacketHandler.send_UpdateExp(player);
				//ClientPacketHandler.send_UpdateStats(this, false);
				//ClientPacketHandler.send_UpdateStats(this, true);
				//ClientPacketHandler.send_Property(this, "max_havoc", this.MaxHavoc);
				//ClientPacketHandler.send_Property(this, "max_chaos", this.MaxChaos);
				//ClientPacketHandler.send_Property(this, "max_stamina", this.MaxStamina);
				ClientPacketHandler.send_SkillList(player, new Skill[] { player.SkillList[skillId] });
				ClientPacketHandler.send_PacketResponse(player, 0x0192, 0, (uint)skillId);
			}
		}
	}
}
