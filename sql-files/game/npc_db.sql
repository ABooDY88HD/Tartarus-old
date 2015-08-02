DROP TABLE IF EXISTS `npc_db`;
CREATE TABLE IF NOT EXISTS `npc_db` (
  `id` int(11) NOT NULL,
  `name` varchar(50) NOT NULL DEFAULT '',
  `x` int(11) NOT NULL,
  `y` int(11) NOT NULL,
  `face` tinyint(4) NOT NULL,
  `script` varchar(256) NOT NULL DEFAULT '',
  PRIMARY KEY (`id`)
) ENGINE=MyISAM;

REPLACE INTO `npc_db` VALUES ('3011', 'Guide Eusys', '165220', '52454', '150', 'NPC_Tutorial_Guide_Deva_contact()');
REPLACE INTO `npc_db` VALUES ('3012', 'Guide Arocel', '168402', '54325', '90', 'NPC_Tutorial_Guide_Asura_contact()');
REPLACE INTO `npc_db` VALUES ('3013', 'Guide Canopus', '165223', '49560', '180', 'NPC_Tutorial_Guide_Gaia_contact()');