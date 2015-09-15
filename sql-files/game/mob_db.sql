CREATE TABLE IF NOT EXISTS `mob_db` (
  `id` int(11) NOT NULL,
  `name` varchar(50) NOT NULL DEFAULT '',
  `hp` int(11) NOT NULL,
  `mp` int(11) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=MyISAM;

REPLACE INTO `mob_db` VALUES ('1004', 'Young Opud', '105', '53');