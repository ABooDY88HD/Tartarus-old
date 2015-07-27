DROP TABLE IF EXISTS `item_db`;
CREATE TABLE IF NOT EXISTS `item_db` (
  `id` int(11) NOT NULL,
  `name` varchar(50) NOT NULL DEFAULT '',
  `type` int(11) NOT NULL,
  `class` int(11) NOT NULL,
  `wear_type` int(11) NOT NULL,
  `grade` tinyint(4) NOT NULL,
  `rank` tinyint(4) NOT NULL,
  `level` tinyint(4) NOT NULL,
  `enhance` tinyint(4) NOT NULL,
  `sockets` tinyint(4) NOT NULL,
  `equip_races` tinyint(4) NOT NULL,
  `equip_classes` tinyint(4) NOT NULL,
  `equip_depth` tinyint(4) NOT NULL,
  `weight` int(11) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=MyISAM;

-- Weapons
REPLACE INTO `item_db` VALUES ('103100', 'Beginner\'s Dirk', '1', '103', '0', '0', '1',  '1',  '0',  '2',  '7',  '15',  '15',  '54');
REPLACE INTO `item_db` VALUES ('112100', 'Trainee\'s Small Axe', '1', '113', '0', '0', '1',  '1',  '0',  '2',  '7',  '15',  '15',  '90');
REPLACE INTO `item_db` VALUES ('106100', 'Beginner\'s Mace', '1', '106', '0', '0', '1',  '1',  '0',  '2',  '7',  '15',  '15',  '108');
-- Asura Outfit
REPLACE INTO `item_db` VALUES ('230100', 'Beginner\'s Light Leather Jacket', '1', '200', '2', '0', '1',  '1',  '0',  '2',  '4',  '15',  '15',  '120');
REPLACE INTO `item_db` VALUES ('230109', 'Stepper Adventurer Jacket', '1', '200', '2', '0', '1',  '1',  '0',  '2',  '4',  '15',  '15',  '120');
-- Gaia Outfit
REPLACE INTO `item_db` VALUES ('240100', 'Training Silk Clothes', '1', '200', '2', '0', '1',  '1',  '0',  '2',  '2',  '15',  '15',  '120');
REPLACE INTO `item_db` VALUES ('240109', 'Rogue\'s Training Wear', '1', '200', '2', '0', '1',  '1',  '0',  '2',  '2',  '15',  '15',  '120');
-- Deva Outfit
REPLACE INTO `item_db` VALUES ('220100', 'Beginner\'s Devarian Suit', '1', '200', '2', '0', '1',  '1',  '0',  '2',  '1',  '15',  '15',  '120');
REPLACE INTO `item_db` VALUES ('220109', 'Adventurer Guide Suit', '1', '200', '2', '0', '1',  '1',  '0',  '2',  '1',  '15',  '15',  '120');
-- Bags
REPLACE INTO `item_db` VALUES ('480001', 'Trainee\'s Bag', '1', '350', '23', '0', '0',  '1',  '0',  '0',  '7',  '15',  '15',  '1');

