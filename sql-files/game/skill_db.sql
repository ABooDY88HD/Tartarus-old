CREATE TABLE IF NOT EXISTS `skill_db` (
  `skill_id` int(11) NOT NULL,
  `name` varchar(50) NOT NULL DEFAULT '',
  `max_level` tinyint(3) unsigned NOT NULL,
  PRIMARY KEY (`skill_id`)
) ENGINE=MyISAM;

-- Data
REPLACE INTO `skill_db` VALUES ('1003', 'Defense Training', '3');
REPLACE INTO `skill_db` VALUES ('1004', 'Body Training', '1');
REPLACE INTO `skill_db` VALUES ('1801', 'Creature Control', '1');
REPLACE INTO `skill_db` VALUES ('1891', 'Essence of Gaia', '1');
REPLACE INTO `skill_db` VALUES ('2001', 'Smite', '3');
REPLACE INTO `skill_db` VALUES ('2601', 'Deep Evasion', '1');
REPLACE INTO `skill_db` VALUES ('2602', 'Mental Concentration', '1');
REPLACE INTO `skill_db` VALUES ('3201', 'Minor Healing', '1');
REPLACE INTO `skill_db` VALUES ('4001', 'Summon Creature', '1');
REPLACE INTO `skill_db` VALUES ('4002', 'Recall Creature', '1');
REPLACE INTO `skill_db` VALUES ('4003', 'Creature Taming', '1');
REPLACE INTO `skill_db` VALUES ('6010', 'Skill: Luna Chip', '8');
REPLACE INTO `skill_db` VALUES ('50101', 'Heavenly Restoration', '1');
REPLACE INTO `skill_db` VALUES ('50102', 'Divine Purpose', '1');
REPLACE INTO `skill_db` VALUES ('50201', 'Asuran Haste', '1');
REPLACE INTO `skill_db` VALUES ('50202', 'Evasive Resilience', '1');
REPLACE INTO `skill_db` VALUES ('50301', 'Gaian Strength', '1');
REPLACE INTO `skill_db` VALUES ('50302', 'Gaian Resilience', '1');
REPLACE INTO `skill_db` VALUES ('50303', 'Wheel of Destiny', '1');
REPLACE INTO `skill_db` VALUES ('50401', 'Grace', '18');
REPLACE INTO `skill_db` VALUES ('61001', 'Offense Training', '3');
