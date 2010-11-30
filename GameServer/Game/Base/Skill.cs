using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Base
{
    class Skill
    {
        public static Game.character._usingSkill Info(int SkillID)
        {
            Game.character._usingSkill u = new Game.character._usingSkill();
            u.SkillID = new int[10];
            u.FoundID = new int[10];
            u.TargetType = new bool[10];
            u.NumberOfAttack = NumberAttack(SkillID, ref u.SkillID);

            u.Sekme = 1;
            u.Uzak = 0;
            u.SekmeMetre = 0;
            u.canUse = true;
            switch (Data.SkillBase[SkillID].Series)
            {

                #region Bicheon
                #region Smashing Series
                case "SKILL_CH_SWORD_SMASH_A":
                    u.Instant = 0;
                    u.P_M = false;
                    break;
                case "SKILL_CH_SWORD_SMASH_B":
                    u.Instant = 0;
                    u.P_M = false;
                    break;
                case "SKILL_CH_SWORD_SMASH_C":
                    u.Instant = 0;
                    u.P_M = false;
                    break;
                case "SKILL_CH_SWORD_SMASH_D":
                    u.Instant = 0;
                    u.P_M = false;
                    break;
                case "SKILL_CH_SWORD_SMASH_E":
                    u.Instant = 0;
                    u.P_M = false;
                    break;
                #endregion
                #region Chain Sword Attack Series
                case "SKILL_CH_SWORD_CHAIN_A":
                    u.Instant = 0;
                    u.P_M = false;
                    break;
                case "SKILL_CH_SWORD_CHAIN_B":
                    u.Instant = 0;
                    u.P_M = false;
                    break;
                case "SKILL_CH_SWORD_CHAIN_C":
                    u.Instant = 0;
                    u.P_M = false;
                    break;
                case "SKILL_CH_SWORD_CHAIN_D":
                    u.Instant = 0;
                    u.P_M = false;
                    break;
                case "SKILL_CH_SWORD_CHAIN_E":
                    u.Instant = 0;
                    u.P_M = false;
                    break;
                case "SKILL_CH_SWORD_CHAIN_F":
                    u.Instant = 0;
                    u.P_M = false;
                    break;
                case "SKILL_CH_SWORD_CHAIN_G":
                    u.Instant = 0;
                    u.P_M = false;
                    break;
                #endregion
                #region Blade Force Series
                case "SKILL_CH_SWORD_GEOMGI_A":
                    u.Instant = 0;
                    u.Uzak = 17;
                    u.P_M = false;
                    break;
                case "SKILL_CH_SWORD_GEOMGI_B":
                    u.Instant = 0;
                    u.Uzak = 20;
                    u.P_M = false;
                    break;
                case "SKILL_CH_SWORD_GEOMGI_C":
                    u.Instant = 0;
                    u.Uzak = 20;
                    u.P_M = false;
                    break;
                case "SKILL_CH_SWORD_GEOMGI_D":
                    u.Instant = 0;
                    u.Uzak = 23;
                    u.P_M = false;
                    break;
                case "SKILL_CH_SWORD_GEOMGI_E":
                    u.Instant = 0;
                    u.Uzak = 25;
                    u.P_M = false;
                    break;
                #endregion
                #region Hidden Blade Series
                case "SKILL_CH_SWORD_KNOCKDOWN_A":
                    u.Instant = 1;
                    u.P_M = false;
                    u.OzelEffect = 4;
                    break;
                case "SKILL_CH_SWORD_KNOCKDOWN_B":
                    u.Instant = 1;
                    u.P_M = false;
                    u.Sekme = 3;
                    u.SekmeMetre = 2;
                    u.OzelEffect = 4;
                    break;
                case "SKILL_CH_SWORD_KNOCKDOWN_C":
                    u.Instant = 1;
                    u.P_M = false;
                    u.OzelEffect = 4;
                    break;
                case "SKILL_CH_SWORD_KNOCKDOWN_D":
                    u.Instant = 1;
                    u.P_M = false;
                    u.Sekme = 3;
                    u.SekmeMetre = 3;
                    u.OzelEffect = 4;
                    break;
                #endregion
                #region Killing Heaven Blade Series
                case "SKILL_CH_SWORD_DOWNATTACK_A":
                    u.Instant = 1;
                    u.P_M = false;
                    u.OzelEffect = 5;
                    break;
                case "SKILL_CH_SWORD_DOWNATTACK_B":
                    u.Instant = 1;
                    u.P_M = false;
                    u.OzelEffect = 5;
                    break;
                case "SKILL_CH_SWORD_DOWNATTACK_C":
                    u.Instant = 1;
                    u.P_M = false;
                    u.OzelEffect = 5;
                    break;
                case "SKILL_CH_SWORD_DOWNATTACK_D":
                    u.Instant = 1;
                    u.P_M = false;
                    u.OzelEffect = 5;
                    break;
                #endregion
                #region Sword Dance Series
                case "SKILL_CH_SWORD_SPECIAL_A":
                    u.Instant = 1;
                    u.Uzak = 23;
                    u.P_M = false;
                    u.Sekme = 3;
                    u.SekmeMetre = 7;
                    break;
                case "SKILL_CH_SWORD_SPECIAL_B":
                    u.Instant = 1;
                    u.Uzak = 23;
                    u.P_M = false;
                    u.Sekme = 3;
                    u.SekmeMetre = 15;
                    break;
                case "SKILL_CH_SWORD_SPECIAL_C":
                    u.Instant = 1;
                    u.Uzak = 23;
                    u.P_M = false;
                    u.Sekme = 3;
                    u.SekmeMetre = 15;
                    break;
                case "SKILL_CH_SWORD_SPECIAL_D":
                    u.Instant = 1;
                    u.Uzak = 23;
                    u.P_M = false;
                    u.Sekme = 4;
                    u.SekmeMetre = 15;
                    break;
                #endregion
                #endregion

                #region Heuksal
                #region Annihilating Blade Series
                case "SKILL_CH_SPEAR_PIERCE_A":
                    u.Instant = 1;
                    u.P_M = false;
                    break;
                case "SKILL_CH_SPEAR_PIERCE_B":
                    u.Instant = 1;
                    u.P_M = false;
                    break;
                case "SKILL_CH_SPEAR_PIERCE_C":
                    u.Instant = 1;
                    u.P_M = false;
                    break;
                case "SKILL_CH_SPEAR_PIERCE_D":
                    u.Instant = 1;
                    u.P_M = false;
                    break;
                case "SKILL_CH_SPEAR_PIERCE_E":
                    u.Instant = 1;
                    u.P_M = false;
                    break;
                #endregion
                #region Heuksal Spear Series
                case "SKILL_CH_SPEAR_FRONTAREA_A":
                    u.Instant = 1;
                    u.P_M = false;
                    u.Sekme = 3;
                    u.SekmeMetre = 1;
                    break;
                case "SKILL_CH_SPEAR_FRONTAREA_B":
                    u.Instant = 1;
                    u.P_M = false;
                    u.Sekme = 3;
                    u.SekmeMetre = 1;
                    break;
                case "SKILL_CH_SPEAR_FRONTAREA_C":
                    u.Instant = 1;
                    u.P_M = false;
                    u.Sekme = 3;
                    u.SekmeMetre = 1;
                    break;
                case "SKILL_CH_SPEAR_FRONTAREA_D":
                    u.Instant = 1;
                    u.P_M = false;
                    u.Sekme = 3;
                    u.SekmeMetre = 2;
                    break;
                case "SKILL_CH_SPEAR_FRONTAREA_E":
                    u.Instant = 1;
                    u.P_M = false;
                    u.Sekme = 3;
                    u.SekmeMetre = 2;
                    break;
                #endregion
                #region Soul Departs Spear Series
                case "SKILL_CH_SPEAR_STUN_A":
                    u.Instant = 1;
                    u.P_M = false;
                    break;
                case "SKILL_CH_SPEAR_STUN_B":
                    u.Instant = 1;
                    u.P_M = false;
                    break;
                case "SKILL_CH_SPEAR_STUN_C":
                    u.Instant = 1;
                    u.P_M = false;
                    break;
                case "SKILL_CH_SPEAR_STUN_D":
                    u.Instant = 1;
                    u.P_M = false;
                    break;
                case "SKILL_CH_SPEAR_STUN_E":
                    u.Instant = 0;
                    u.P_M = false;
                    break;
                #endregion
                #region Ghost Spear Attack Series
                case "SKILL_CH_SPEAR_ROUNDAREA_A":
                    u.Instant = 0;
                    u.P_M = false;
                    u.Sekme = 5;
                    u.SekmeMetre = 2;
                    break;
                case "SKILL_CH_SPEAR_ROUNDAREA_B":
                    u.Instant = 0;
                    u.P_M = false;
                    u.Sekme = 5;
                    u.SekmeMetre = 3;
                    u.OzelEffect = 6;
                    break;
                case "SKILL_CH_SPEAR_ROUNDAREA_C":
                    u.Instant = 0;
                    u.P_M = false;
                    u.Sekme = 5;
                    u.SekmeMetre = 4;
                    u.OzelEffect = 6;
                    break;
                case "SKILL_CH_SPEAR_ROUNDAREA_D":
                    u.Instant = 0;
                    u.P_M = false;
                    u.Sekme = 5;
                    u.SekmeMetre = 5;
                    u.OzelEffect = 6;
                    break;
                #endregion
                #region Chain Spear Attack Series
                case "SKILL_CH_SPEAR_CHAIN_A":
                    u.Instant = 0;
                    u.P_M = false;
                    break;
                case "SKILL_CH_SPEAR_CHAIN_B":
                    u.Instant = 0;
                    u.P_M = false;
                    break;
                case "SKILL_CH_SPEAR_CHAIN_C":
                    u.Instant = 0;
                    u.P_M = false;
                    u.Sekme = 3;
                    u.SekmeMetre = 2;
                    break;
                case "SKILL_CH_SPEAR_CHAIN_D":
                    u.Instant = 0;
                    u.P_M = false;
                    break;
                case "SKILL_CH_SPEAR_CHAIN_E":
                    u.Instant = 0;
                    u.P_M = false;
                    u.Sekme = 5;
                    u.SekmeMetre = 3;
                    break;
                case "SKILL_CH_SPEAR_CHAIN_F":
                    u.Instant = 0;
                    u.P_M = false;
                    u.Sekme = 5;
                    u.SekmeMetre = 3;
                    break;
                #endregion
                #region Flying Dragon Spear Series
                case "SKILL_CH_SPEAR_SHOOT_A":
                    u.Instant = 1;
                    u.Uzak = 23;
                    u.P_M = false;
                    break;
                case "SKILL_CH_SPEAR_SHOOT_B":
                    u.Instant = 1;
                    u.Uzak = 25;
                    u.P_M = false;
                    break;
                case "SKILL_CH_SPEAR_SHOOT_C":
                    u.Instant = 1;
                    u.Uzak = 26;
                    u.P_M = false;
                    break;
                case "SKILL_CH_SPEAR_SHOOT_D":
                    u.Instant = 1;
                    u.Uzak = 27;
                    u.P_M = false;
                    break;
                #endregion
                #endregion

                #region Pacheon
                #region Anti Devil Bow Series
                case "SKILL_CH_BOW_CRITICAL_A":
                    u.Instant = 2;
                    u.Uzak = 27;
                    u.P_M = false;
                    break;
                case "SKILL_CH_BOW_CRITICAL_B":
                    u.Instant = 2;
                    u.Uzak = 27;
                    u.P_M = false;
                    break;
                case "SKILL_CH_BOW_CRITICAL_C":
                    u.Instant = 2;
                    u.Uzak = 27;
                    u.P_M = false;
                    break;
                case "SKILL_CH_BOW_CRITICAL_D":
                    u.Instant = 2;
                    u.Uzak = 27;
                    u.P_M = false;
                    break;
                case "SKILL_CH_BOW_CRITICAL_E":
                    u.Instant = 2;
                    u.Uzak = 27;
                    u.P_M = false;
                    break;
                #endregion
                #region Arrow Combo Attack Series
                case "SKILL_CH_BOW_CHAIN_A":
                    u.NumberOfAttack = 2;
                    u.SkillID[1] = SkillID;
                    u.SkillID[2] = SkillID;
                    u.Instant = 0;
                    u.Uzak = 27;
                    u.P_M = false;
                    break;
                case "SKILL_CH_BOW_CHAIN_B":
                    u.NumberOfAttack = 3;
                    u.SkillID[1] = SkillID;
                    u.SkillID[2] = SkillID;
                    u.SkillID[3] = SkillID;
                    u.Instant = 0;
                    u.Uzak = 27;
                    u.P_M = false;
                    break;
                case "SKILL_CH_BOW_CHAIN_C":
                    u.NumberOfAttack = 4;
                    u.SkillID[1] = SkillID;
                    u.SkillID[2] = SkillID;
                    u.SkillID[3] = SkillID;
                    u.SkillID[4] = SkillID;
                    u.Instant = 0;
                    u.Uzak = 27;
                    u.P_M = false;
                    break;
                case "SKILL_CH_BOW_CHAIN_D":
                    u.NumberOfAttack = 5;
                    u.SkillID[1] = SkillID;
                    u.SkillID[2] = SkillID;
                    u.SkillID[3] = SkillID;
                    u.SkillID[4] = SkillID;
                    u.SkillID[5] = SkillID;
                    u.Instant = 0;
                    u.Uzak = 27;
                    u.P_M = false;
                    break;
                case "SKILL_CH_BOW_CHAIN_E":
                    u.NumberOfAttack = 6;
                    u.SkillID[1] = SkillID;
                    u.SkillID[2] = SkillID;
                    u.SkillID[3] = SkillID;
                    u.SkillID[4] = SkillID;
                    u.SkillID[5] = SkillID;
                    u.SkillID[6] = SkillID;
                    u.Instant = 0;
                    u.Uzak = 27;
                    u.P_M = false;
                    break;
                #endregion
                #region Autumn Wind Arrow Series
                case "SKILL_CH_BOW_PIERCE_A":
                    u.Instant = 2;
                    u.Uzak = 27;
                    u.P_M = false;
                    break;
                case "SKILL_CH_BOW_PIERCE_B":
                    u.Instant = 2;
                    u.Uzak = 27;
                    u.P_M = false;
                    break;
                case "SKILL_CH_BOW_PIERCE_C":
                    u.Instant = 2;
                    u.Uzak = 27;
                    u.P_M = false;
                    break;
                case "SKILL_CH_BOW_PIERCE_D":
                    u.Instant = 2;
                    u.Uzak = 27;
                    u.P_M = false;
                    break;
                #endregion
                #region Explosion Arrow Series
                case "SKILL_CH_BOW_AREA_A":
                    u.Instant = 2;
                    u.Uzak = 27;
                    u.P_M = false;
                    u.Sekme = 4;
                    u.SekmeMetre = 4;
                    break;
                case "SKILL_CH_BOW_AREA_B":
                    u.Instant = 2;
                    u.Uzak = 27;
                    u.P_M = false;
                    u.Sekme = 4;
                    u.SekmeMetre = 4;
                    break;
                case "SKILL_CH_BOW_AREA_C":
                    u.Instant = 2;
                    u.Uzak = 27;
                    u.P_M = false;
                    u.Sekme = 4;
                    u.SekmeMetre = 4;
                    break;
                case "SKILL_CH_BOW_AREA_D":
                    u.Instant = 2;
                    u.Uzak = 27;
                    u.P_M = false;
                    u.Sekme = 4;
                    u.SekmeMetre = 5;
                    break;
                #endregion
                #region Strong Bow Series
                case "SKILL_CH_BOW_POWER_A":
                    u.Instant = 2;
                    u.Uzak = 27;
                    u.P_M = false;
                    break;
                case "SKILL_CH_BOW_POWER_B":
                    u.Instant = 2;
                    u.Uzak = 27;
                    u.P_M = false;
                    break;
                case "SKILL_CH_BOW_POWER_C":
                    u.Instant = 2;
                    u.Uzak = 27;
                    u.P_M = false;
                    break;
                case "SKILL_CH_BOW_POWER_D":
                    u.Instant = 2;
                    u.Uzak = 27;
                    u.P_M = false;
                    break;
                #endregion
                #region Mind Bow Series
                case "SKILL_CH_BOW_SPECIAL_A":
                    u.Instant = 2;
                    u.Uzak = 27;
                    u.P_M = false;
                    u.Sekme = 2;
                    u.SekmeMetre = 20;
                    break;
                case "SKILL_CH_BOW_SPECIAL_B":
                    u.Instant = 2;
                    u.Uzak = 27;
                    u.P_M = false;
                    u.Sekme = 3;
                    u.SekmeMetre = 22;
                    break;
                case "SKILL_CH_BOW_SPECIAL_C":
                    u.Instant = 2;
                    u.Uzak = 27;
                    u.P_M = false;
                    u.Sekme = 4;
                    u.SekmeMetre = 23;
                    break;
                case "SKILL_CH_BOW_SPECIAL_D":
                    u.Instant = 2;
                    u.Uzak = 27;
                    u.P_M = false;
                    u.Sekme = 5;
                    u.SekmeMetre = 24;
                    break;
                #endregion
                #endregion

                #region Cold
                #region Snow Storm Series
                case "SKILL_CH_COLD_GIGONGSUL_A":
                    u.Instant = 2;
                    u.Uzak = 20;
                    u.P_M = true;
                    break;
                case "SKILL_CH_COLD_GIGONGSUL_B":
                    u.Instant = 2;
                    u.Uzak = 20;
                    u.P_M = true;
                    u.Sekme = 5;
                    u.SekmeMetre = 6;
                    break;
                case "SKILL_CH_COLD_GIGONGSUL_C":
                    u.Instant = 2;
                    u.Uzak = 20;
                    u.P_M = true;
                    break;
                case "SKILL_CH_COLD_GIGONGSUL_D":
                    u.Instant = 2;
                    u.Uzak = 20;
                    u.P_M = true;
                    u.Sekme = 5;
                    u.SekmeMetre = 7;
                    break;
                #endregion
                #endregion

                #region Light
                #region Lion Shout Series
                case "SKILL_CH_LIGHTNING_CHUNDUNG_A":
                case "SKILL_CH_LIGHTNING_CHUNDUNG_B":
                case "SKILL_CH_LIGHTNING_CHUNDUNG_C":
                case "SKILL_CH_LIGHTNING_CHUNDUNG_D":
                case "SKILL_CH_LIGHTNING_CHUNDUNG_E":
                    u.Instant = 0;
                    u.Uzak = 20;
                    u.P_M = true;
                    u.Sekme = 3;
                    u.SekmeMetre = 2;
                    break;
                #endregion
                #region Thunderbolt Force Series
                case "SKILL_CH_LIGHTNING_STORM_A":
                case "SKILL_CH_LIGHTNING_STORM_B":
                case "SKILL_CH_LIGHTNING_STORM_C":
                case "SKILL_CH_LIGHTNING_STORM_D":
                    u.Instant = 2;
                    u.Uzak = 15;
                    u.P_M = true;
                    u.Sekme = 3;
                    u.SekmeMetre = 10;
                    break;
                #endregion
                #endregion

                #region Fire
                #region Flame Wave Series
                case "SKILL_CH_FIRE_GIGONGSUL_A":
                case "SKILL_CH_FIRE_GIGONGSUL_B":
                case "SKILL_CH_FIRE_GIGONGSUL_D":
                case "SKILL_CH_FIRE_GIGONGSUL_E":
                    u.Instant = 2;
                    u.Uzak = 23;
                    u.P_M = true;
                    break;
                case "SKILL_CH_FIRE_GIGONGSUL_F":
                case "SKILL_CH_FIRE_GIGONGSUL_C":
                    u.Instant = 2;
                    u.Uzak = 23;
                    u.P_M = true;
                    u.Sekme = 3;
                    u.SekmeMetre = 6;
                    break;
                #endregion
                #endregion

                default:
                    u.Sekme = 1;
                    u.SekmeMetre = 0;
                    u.Instant = 2;
                    u.P_M = false;
                    u.canUse = false;
                    break;
            }
            return u;
        }
        public static byte NumberAttack(int SkillID, ref int[] ID)
        {
            byte NumberAttack = 0;
            int Next1 = Data.SkillBase[SkillID].NextSkill;
            ID[NumberAttack] = SkillID;
            NumberAttack++;
            if (Next1 != 0)
            {
                ID[NumberAttack] = Next1;
                NumberAttack++;
                while (Next1 != 0)
                {
                    if (Data.SkillBase[Next1].NextSkill != 0)
                    {
                        ID[NumberAttack] = Data.SkillBase[Next1].NextSkill;
                        NumberAttack++;
                        Next1 = Data.SkillBase[Next1].NextSkill;
                    }
                    else
                        break;
                }
            }
            return NumberAttack;
        }
        public static bool CheckWeapon(int itemid, int skillid)
        {
            if (Data.SkillBase[skillid].Weapon1 == 255) return true; // for magical skill
            byte[] weapons = new byte[2];
            weapons[0] = Data.SkillBase[skillid].Weapon1;
            weapons[1] = Data.SkillBase[skillid].Weapon2;
            if (weapons[1] == 255) weapons[1] = 6;
            foreach (byte b in weapons)
            {
                if (b == Data.ItemBase[itemid].Class_C) return true;
            }

            return false;
        }
    }
}
