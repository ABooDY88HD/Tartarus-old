DROP TABLE IF EXISTS `quest_db`;
CREATE TABLE IF NOT EXISTS `quest_db` (
  `id` int(11) NOT NULL,
  `name` varchar(50) NOT NULL DEFAULT '',
  `min_level` int(11) NOT NULL,
  `max_level` int(11) NOT NULL,
  `min_job_level` int(11) NOT NULL,
  `max_job_level` int(11) NOT NULL,
  `limit_races` tinyint(4) NOT NULL,
  `limit_classes` tinyint(4) NOT NULL,
  `limit_depth` tinyint(4) NOT NULL,
  `repeatable` boolean NOT NULL,
  `exp` int(11) NOT NULL,
  `jp` int(11) NOT NULL,
  `holic_point` int(11) NOT NULL,
  `gold` int(11) NOT NULL,
  `drop_group` int(11) NOT NULL,
  `type` int(11) NOT NULL,
  `objective0` int(11) NOT NULL,
  `objective1` int(11) NOT NULL,
  `objective2` int(11) NOT NULL,
  `objective3` int(11) NOT NULL,
  `objective4` int(11) NOT NULL,
  `objective5` int(11) NOT NULL,
  `objective6` int(11) NOT NULL,
  `objective7` int(11) NOT NULL,
  `objective8` int(11) NOT NULL,
  `objective9` int(11) NOT NULL,
  `objective10` int(11) NOT NULL,
  `objective11` int(11) NOT NULL,
  `default_reward_id` int(11) NOT NULL,
  `default_reward_level` tinyint(4) NOT NULL,
  `default_reward_quantity` int(11) NOT NULL,
  `optional_reward_id1` int(11) NOT NULL,
  `optional_reward_level1` tinyint(4) NOT NULL,
  `optional_reward_quantity1` int(11) NOT NULL,
  `optional_reward_id2` int(11) NOT NULL,
  `optional_reward_level2` tinyint(4) NOT NULL,
  `optional_reward_quantity2` int(11) NOT NULL,
  `optional_reward_id3` int(11) NOT NULL,
  `optional_reward_level3` tinyint(4) NOT NULL,
  `optional_reward_quantity3` int(11) NOT NULL,
  `optional_reward_id4` int(11) NOT NULL,
  `optional_reward_level4` tinyint(4) NOT NULL,
  `optional_reward_quantity4` int(11) NOT NULL,
  `optional_reward_id5` int(11) NOT NULL,
  `optional_reward_level5` tinyint(4) NOT NULL,
  `optional_reward_quantity5` int(11) NOT NULL,
  `optional_reward_id6` int(11) NOT NULL,
  `optional_reward_level6` tinyint(4) NOT NULL,
  `optional_reward_quantity6` int(11) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=MyISAM;

REPLACE INTO `quest_db` VALUES ('1005', 'Your First Hunt<Deva>', '1', '0', '0', '0', '1', '15', '15', '0', '5', '0', '2', '300', '0', '101', '1003', '3', '2003', '0', '3004', '0', '0', '0', '0', '0', '0', '0', '603001', '1', '3', '106101', '1', '1', '103101', '1', '1', '101102', '1', '1', '108101', '1', '1', '112101', '1', '1', '111101', '1', '1');
REPLACE INTO `quest_db` VALUES ('1006', 'Deva\'s Feather Collection', '1', '0', '0', '0', '1', '15', '15', '0', '10', '0', '3', '600', '-1', '106', '1000000', '3', '0', '0', '0', '0', '1003', '2003', '3004', '0', '0', '0', '603002', '1', '3', '107101', '1', '1', '104101', '1', '1', '102101', '1', '1', '109101', '1', '1', '105101', '1', '1', '0', '0', '0');
REPLACE INTO `quest_db` VALUES ('1007', 'Meet Tutor Sistina', '1', '0', '0', '0', '7', '15', '15', '0', '32', '2', '6', '1500', '0', '401', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '602506', '1', '1', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0');
REPLACE INTO `quest_db` VALUES ('1008', 'Your First Hunt<Asura>', '1', '0', '0', '0', '4', '15', '15', '0', '5', '0', '2', '300', '0', '101', '1004', '3', '2004', '0', '3005', '0', '0', '0', '0', '0', '0', '0', '603001', '1', '3', '106101', '1', '1', '103101', '1', '1', '101102', '1', '1', '108101', '1', '1', '112101', '1', '1', '111101', '1', '1');
REPLACE INTO `quest_db` VALUES ('1009', 'Asura\'s Claw Collection', '1', '0', '0', '0', '4', '15', '15', '0', '10', '0', '3', '600', '-8', '106', '1000006', '3', '0', '0', '0', '0', '1004', '2004', '3005', '0', '0', '0', '603002', '1', '3', '107101', '1', '1', '104101', '1', '1', '102101', '1', '1', '109101', '1', '1', '105101', '1', '1', '0', '0', '0');
REPLACE INTO `quest_db` VALUES ('1010', 'Your First Hunt<Gaia>', '1', '0', '0', '0', '2', '15', '15', '0', '5', '0', '2', '300', '0', '101', '1002', '3', '2002', '0', '3003', '0', '0', '0', '0', '0', '0', '0', '603001', '1', '3', '106101', '1', '1', '103101', '1', '1', '101102', '1', '1', '108101', '1', '1', '112101', '1', '1', '111101', '1', '1');
REPLACE INTO `quest_db` VALUES ('1011', 'Gaia\'s Feather Collection', '1', '0', '0', '0', '2', '15', '15', '0', '10', '0', '3', '600', '-1', '106', '1000000', '3', '0', '0', '0', '0', '1002', '2002', '3003', '0', '0', '0', '603002', '1', '3', '107101', '1', '1', '104101', '1', '1', '102101', '1', '1', '109101', '1', '1', '105101', '1', '1', '0', '0', '0');