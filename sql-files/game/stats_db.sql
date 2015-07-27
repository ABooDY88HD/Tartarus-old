DROP TABLE IF EXISTS `stats_db`;
CREATE TABLE IF NOT EXISTS `stats_db` (
  `id` int(11) NOT NULL,
  `str` smallint(5) NOT NULL,
  `vit` smallint(5) NOT NULL,
  `dex` smallint(5) NOT NULL,
  `agi` smallint(5) NOT NULL,
  `int` smallint(5) NOT NULL,
  `wis` smallint(5) NOT NULL,
  `luk` smallint(5) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=MyISAM;

-- Classes
REPLACE INTO `stats_db` VALUES ('100', '10', '10', '10', '10', '10', '10', '10');
REPLACE INTO `stats_db` VALUES ('101', '20', '19', '16', '16', '13', '12', '10');
REPLACE INTO `stats_db` VALUES ('102', '13', '17', '13', '14', '20', '19', '10');
REPLACE INTO `stats_db` VALUES ('103', '17', '18', '16', '16', '15', '14', '10');
REPLACE INTO `stats_db` VALUES ('110', '37', '33', '27', '26', '17', '17', '12');
REPLACE INTO `stats_db` VALUES ('111', '32', '30', '32', '29', '17', '17', '12');
REPLACE INTO `stats_db` VALUES ('112', '21', '26', '20', '20', '35', '35', '12');
REPLACE INTO `stats_db` VALUES ('113', '23', '27', '22', '22', '32', '31', '12');
REPLACE INTO `stats_db` VALUES ('114', '30', '29', '27', '25', '23', '23', '12');
REPLACE INTO `stats_db` VALUES ('120', '70', '63', '54', '52', '38', '38', '20');
REPLACE INTO `stats_db` VALUES ('121', '64', '59', '60', '56', '38', '38', '20');
REPLACE INTO `stats_db` VALUES ('122', '44', '53', '43', '43', '66', '66', '20');
REPLACE INTO `stats_db` VALUES ('123', '46', '54', '45', '45', '63', '62', '20');
REPLACE INTO `stats_db` VALUES ('124', '59', '57', '54', '51', '47', '47', '20');
REPLACE INTO `stats_db` VALUES ('200', '10', '10', '10', '10', '10', '10', '10');
REPLACE INTO `stats_db` VALUES ('201', '19', '20', '16', '15', '13', '13', '10');
REPLACE INTO `stats_db` VALUES ('202', '13', '15', '14', '14', '20', '20', '10');
REPLACE INTO `stats_db` VALUES ('203', '16', '16', '15', '15', '17', '17', '10');
REPLACE INTO `stats_db` VALUES ('210', '32', '37', '24', '22', '21', '21', '12');
REPLACE INTO `stats_db` VALUES ('211', '35', '31', '26', '23', '21', '21', '12');
REPLACE INTO `stats_db` VALUES ('212', '18', '24', '20', '21', '37', '37', '12');
REPLACE INTO `stats_db` VALUES ('213', '18', '25', '21', '21', '36', '36', '12');
REPLACE INTO `stats_db` VALUES ('214', '23', '27', '23', '22', '31', '31', '12');
REPLACE INTO `stats_db` VALUES ('220', '62', '70', '49', '46', '44', '44', '20');
REPLACE INTO `stats_db` VALUES ('221', '65', '63', '52', '47', '44', '44', '20');
REPLACE INTO `stats_db` VALUES ('222', '39', '49', '43', '44', '70', '70', '20');
REPLACE INTO `stats_db` VALUES ('223', '39', '52', '44', '44', '68', '68', '20');
REPLACE INTO `stats_db` VALUES ('224', '48', '54', '47', '46', '60', '60', '20');
REPLACE INTO `stats_db` VALUES ('300', '10', '10', '10', '10', '10', '10', '10');
REPLACE INTO `stats_db` VALUES ('301', '16', '17', '19', '19', '12', '13', '10');
REPLACE INTO `stats_db` VALUES ('302', '14', '16', '13', '13', '20', '20', '10');
REPLACE INTO `stats_db` VALUES ('303', '16', '17', '13', '14', '18', '18', '10');
REPLACE INTO `stats_db` VALUES ('310', '28', '27', '33', '35', '17', '17', '12');
REPLACE INTO `stats_db` VALUES ('311', '25', '27', '36', '35', '17', '17', '12');
REPLACE INTO `stats_db` VALUES ('312', '20', '24', '20', '20', '37', '36', '12');
REPLACE INTO `stats_db` VALUES ('313', '21', '25', '22', '22', '34', '33', '12');
REPLACE INTO `stats_db` VALUES ('314', '23', '27', '21', '23', '33', '30', '12');
REPLACE INTO `stats_db` VALUES ('320', '55', '54', '64', '66', '38', '38', '20');
REPLACE INTO `stats_db` VALUES ('321', '51', '54', '68', '66', '38', '38', '20');
REPLACE INTO `stats_db` VALUES ('322', '43', '49', '43', '43', '70', '67', '20');
REPLACE INTO `stats_db` VALUES ('323', '44', '51', '45', '45', '66', '64', '20');
REPLACE INTO `stats_db` VALUES ('324', '48', '54', '44', '47', '63', '59', '20');