using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework;
using Game.Global;
namespace Game.File
{
    public class FileLoad
    {
        public static void Load()
        {
            Print.Format("Fájlok Betöltése...");

            ItemDatabase(@"\data\itemdata_5000.txt");
            ItemDatabase(@"\data\itemdata_10000.txt");
            ItemDatabase(@"\data\itemdata_15000.txt");
            ItemDatabase(@"\data\itemdata_20000.txt");
            ItemDatabase(@"\data\itemdata_25000.txt");
            ItemDatabase(@"\data\itemdata_30000.txt");
            ItemDatabase(@"\data\itemdata_35000.txt");

            LoadSkillData(@"\data\skilldata_5000.txt");
            LoadSkillData(@"\data\skilldata_10000.txt");
            LoadSkillData(@"\data\skilldata_15000.txt");
            LoadSkillData(@"\data\skilldata_20000.txt");
            LoadSkillData(@"\data\skilldata_25000.txt");
            LoadSkillData(@"\data\skilldata_30000.txt");
            LoadSkillData(@"\data\skilldata_35000.txt");

            ObjectDataBase(@"\data\characterdata_5000.txt");
            ObjectDataBase(@"\data\characterdata_10000.txt");
            ObjectDataBase(@"\data\characterdata_15000.txt");
            ObjectDataBase(@"\data\characterdata_20000.txt");
            ObjectDataBase(@"\data\characterdata_25000.txt");
            ObjectDataBase(@"\data\characterdata_30000.txt");
            ObjectDataBase(@"\data\characterdata_35000.txt");

            LoadObject();
            TeleportBuilding();
            LoadMasteryData();
            LoadGoldData();
            LoadLevelData();
            LoadShopTabData();
        }
        public static void ItemDatabase(string path)
        {
            TxtFile.ReadFromFile(path, '\t');

            int id = 0;
            string s1 = null;

            for (int t = 0; t <= TxtFile.amountLine - 1; t++)
            {
                s1 = TxtFile.lines[t].ToString();
                TxtFile.commands = s1.Split('\t');
                item_database it = new item_database();
                id = Convert.ToInt32(TxtFile.commands[1]);
                it.Name = TxtFile.commands[2];
                //10781	

                /*if (it.Name == "ITEM_CH_BLADE_05_B_RARE")
                {
                    //for (int i = 0; i <= TxtFile.commands.Length - 1; i++)
                    Console.WriteLine("{0}  {1} ", it.Name, TxtFile.commands[15]);
                }
                /*if (Convert.ToInt32(TxtFile.commands[10]) != 3)
                    if (Convert.ToInt32(TxtFile.commands[10]) != 1)
                        Print.Format("id {0} type {1}", it.Name, id);*/
                /*if (it.Name.StartsWith("ITEM_MALL_GLOBAL_CHATTING"))
                {
                    //for (int i = 0; i <= TxtFile.commands.Length - 1; i++)
                    Console.WriteLine("{0}  {1}  : {2} ", TxtFile.commands[119], it.Name, id);
                    for (int i = 0; i <= TxtFile.commands.Length - 1; i++)
                        Console.WriteLine(" {0}  {1}", i, TxtFile.commands[i]);
                }
                /*if (it.Name == "ITEM_MALL_AVATAR_M_FAIRY_2" || it.Name == "ITEM_CH_SWORD_06_C") //3823
                {
                    //for (int i = 0; i <= TxtFile.commands.Length - 1; i++)
                        Console.WriteLine(" {0}  {1}", it.Name, TxtFile.commands[14]);
                }*/
                it.ID = Convert.ToInt32(id);
                it.Class_A = Convert.ToInt32(TxtFile.commands[9]);
                it.Class_D = Convert.ToInt32(TxtFile.commands[10]);
                it.Class_B = Convert.ToInt32(TxtFile.commands[11]);
                it.Class_C = Convert.ToInt32(TxtFile.commands[12]);
                it.Race = Convert.ToByte(TxtFile.commands[14]);
                it.SOX = Convert.ToByte(TxtFile.commands[15]);
                it.SoulBound = Convert.ToByte(TxtFile.commands[18]);
                it.Shop_price = Convert.ToInt32(TxtFile.commands[26]);
                if (it.Shop_price == 0) it.Shop_price = SetSilk(it.Name);

                it.Storage_price = Convert.ToInt32(TxtFile.commands[30]);
                it.Sell_Price = Convert.ToInt32(TxtFile.commands[31]);
                it.Level = Convert.ToByte(TxtFile.commands[33]);
                it.Max_Stack = Convert.ToInt32(TxtFile.commands[57]);
                it.Gender = Convert.ToByte(TxtFile.commands[58]);


                it.Defans.Durability = Convert.ToDouble(TxtFile.commands[63].Replace('.', ','));
                it.Defans.MinPhyDef = Convert.ToDouble(TxtFile.commands[65].Replace('.', ','));
                it.Defans.PhyDefINC = Convert.ToDouble(TxtFile.commands[67].Replace('.', ','));
                it.Defans.Parry = Convert.ToDouble(TxtFile.commands[68].Replace('.', ','));
                it.Defans.MinBlock = Convert.ToByte(Math.Round(Convert.ToDouble(TxtFile.commands[74].Replace('.', ','))));
                it.Defans.MaxBlock = Convert.ToByte(Math.Round(Convert.ToDouble(TxtFile.commands[75].Replace('.', ','))));
                it.Defans.MinMagDef = Convert.ToDouble(TxtFile.commands[76].Replace('.', ','));
                it.Defans.MagDefINC = Convert.ToDouble(TxtFile.commands[78].Replace('.', ','));
                it.Defans.PhyAbsorb = Convert.ToDouble(TxtFile.commands[79].Replace('.', ','));
                it.Defans.MagAbsorb = Convert.ToDouble(TxtFile.commands[80].Replace('.', ','));
                it.Defans.AbsorbINC = Convert.ToDouble(TxtFile.commands[81].Replace('.', ','));


                it.needEquip = Convert.ToBoolean(Convert.ToByte(TxtFile.commands[93]));
                it.ATTACK_DISTANCE = Convert.ToInt16(TxtFile.commands[94]);
                if (it.Class_C == 2 || it.Class_C == 3) it.ATTACK_DISTANCE = 3;
                if (it.Class_C == 4 || it.Class_C == 5) it.ATTACK_DISTANCE = 5;
                if (it.Class_C == 6) it.ATTACK_DISTANCE = 30;

                it.Attack.Min_LPhyAttack = Convert.ToDouble(TxtFile.commands[95].Replace('.', ','));
                it.Attack.Min_HPhyAttack = Convert.ToDouble(TxtFile.commands[97].Replace('.', ','));
                it.Attack.PhyAttackInc = Convert.ToDouble(TxtFile.commands[99].Replace('.', ','));

                it.Attack.Min_LMagAttack = Convert.ToDouble(TxtFile.commands[100].Replace('.', ','));
                it.Attack.Min_HMagAttack = Convert.ToDouble(TxtFile.commands[102].Replace('.', ','));

                it.Attack.MagAttackINC = Convert.ToDouble(TxtFile.commands[104].Replace('.', ','));


                it.Attack.MinAttackRating = Convert.ToByte(Math.Round(Convert.ToDouble(TxtFile.commands[113].Replace('.', ','))));
                it.Attack.MaxAttackRating = Convert.ToByte(Math.Round(Convert.ToDouble(TxtFile.commands[114].Replace('.', ','))));

                it.Attack.MinCrit = Convert.ToByte(Math.Round(Convert.ToDouble(TxtFile.commands[116].Replace('.', ','))));
                it.Attack.MaxCrit = Convert.ToByte(Math.Round(Convert.ToDouble(TxtFile.commands[117].Replace('.', ','))));
                it.ObjectName = TxtFile.commands[119];
                it.Use_Time = Convert.ToInt32(TxtFile.commands[118]);
                it.SkillName = TxtFile.commands[119];
                it.Use_Time2 = Convert.ToInt32(TxtFile.commands[122]);
                Data.ItemBase[id] = it;

                /*if (it.ID == 7098)
                    Console.WriteLine("{0}", it.SkillName);*/
            }
            item_database its = new item_database();
            its.ATTACK_DISTANCE = 2;
            Data.ItemBase[0] = its;
            Print.Format("{0} Tárgy betöltése innen: {1}...", TxtFile.amountLine + 1, path);
        }

        public static void LoadObject()
        {
             TxtFile.ReadFromFile(@"\data\npcpos.txt", '\t');

             string s = null;
             int count = TxtFile.amountLine;
             uint index = 100;
             int mobx = 1;
             for (int l = 0; l <= TxtFile.amountLine - 1; l++)
             {
                 s = TxtFile.lines[l].ToString();
                 TxtFile.commands = s.Split('\t');
                 int ID = Convert.ToInt32(TxtFile.commands[0]);
                 byte race = Data.ObjectBase[ID].Type;

                 if (race == 1) mobx = 1;
                 else mobx = 1;
                 for (int i = 1; i <= mobx; i++)
                 {
                     obj o = new obj();
                     index++;
                     short AREA = short.Parse(TxtFile.commands[1]);
                     double x = Convert.ToDouble(TxtFile.commands[2].Replace('.', ','));
                     double z = Convert.ToDouble(TxtFile.commands[3].Replace('.', ','));
                     double y = Convert.ToDouble(TxtFile.commands[4].Replace('.', ','));
                     byte movement = 0;
                     o.Agresif = movement;
                     o.AutoMovement = true;
                     if (ID == 1979 || ID == 2101 || ID == 2124 || ID == 2111 || ID == 2112) o.AutoMovement = false;
                     o.OrgMovement = o.AutoMovement;
                     if (o.AutoMovement) o.StartRunTimer(Global.RandomID.GetRandom(7000,20000));
                     o.ID = ID;
                     o.Ids = new Global.ID(Global.ID.IDS.Object);
                     o.UniqueID = o.Ids.GetUniqueID;
                     o.area = AREA;

                     o.xSec = Convert.ToByte((AREA).ToString("X4").Substring(2, 2), 16);
                     o.ySec = Convert.ToByte((AREA).ToString("X4").Substring(0, 2), 16);
                     o.x = (o.xSec - 135) * 192 + (x) / 10;
                     o.z = z;
                     o.y = (o.ySec - 92) * 192 + (y) / 10;

                     o.oX = o.x;
                     o.oY = o.y;

                     o.State = 1;
                     o.Move = 1;
                     o.LocalType = race;
                     o.AutoSpawn = true;
                     o.State = 2;
                     o.HP = Data.ObjectBase[ID].HP;
                     o.Kat = 1;
                     o.Agro = new List<_agro>();
                     o.spawnOran = 20;
                     if (Data.ObjectBase[ID].ObjectType != 3)
                     {
                         o.Type = Systems.RandomType(Data.ObjectBase[ID].Level, ref o.Kat);
                         o.HP *= o.Kat;
                         if (o.Type == 1) o.Agresif = 1;
                         Systems.Objects.Add(o);
                     }
                     else { o.AutoSpawn = false; o.Type = Data.ObjectBase[ID].ObjectType; GlobalUnique.AddObject(o); }
                 }
             }
             Print.Format("{0} non-player objektum betöltése..", TxtFile.amountLine);
        }
        
        public static void ObjectDataBase(string path)
        {
            TxtFile.ReadFromFile(path, '\t');

            string s = null;
            int count = TxtFile.amountLine;

            for (int l = 0; l <= TxtFile.amountLine - 1; l++)
            {
                s = TxtFile.lines[l].ToString();
                TxtFile.commands = s.Split('\t');
                int ID = Convert.ToInt32(TxtFile.commands[1]);
                objectdata o = new objectdata();
                o.ID = ID;
                o.Name = TxtFile.commands[2];

                /*if (o.Name == "MOB_CH_MANGNYANG")
                {
                    for (int i = 0; i <= TxtFile.commands.Length - 1; i++)
                        Console.WriteLine(" {0}  {1}", i, TxtFile.commands[i]);
                }*/
                /*if (ID == 2004 || ID == 2009)
                {
                    for (int i = 0; i <= TxtFile.commands.Length - 1; i++)
                        Console.WriteLine(" {0}  {1}", i, TxtFile.commands[i]);
                }*/
                o.Level = Convert.ToByte(TxtFile.commands[57]);
                o.Exp = Convert.ToInt32(TxtFile.commands[79]);
                o.HP = Convert.ToInt32(TxtFile.commands[59]);
                o.Type = Convert.ToByte(TxtFile.commands[11]);
                o.ObjectType = Convert.ToByte(TxtFile.commands[15]);
                o.PhyDef = Convert.ToInt32(TxtFile.commands[71]);
                o.MagDef = Convert.ToInt32(TxtFile.commands[72]);
                o.HitRatio = Convert.ToInt32(TxtFile.commands[75]);
                o.ParryRatio = Convert.ToInt32(TxtFile.commands[77]);
                o.Agresif = Convert.ToByte(TxtFile.commands[93]);
                o.Skill = new int[9];
                for (byte sk = 0; sk <= 8; sk++)
                {
                    if (Convert.ToInt32(TxtFile.commands[83 + sk]) != 0 && Data.SkillBase[Convert.ToInt32(TxtFile.commands[83 + sk])].Per != 0)
                    {
                        o.Skill[o.amountSkill] = Convert.ToInt32(TxtFile.commands[83 + sk]);
                        o.amountSkill++;
                    }
                }
 
                Data.ObjectBase[ID] = o;
            }
            Print.Format("{0} Object információk betöltése innen: {1}", TxtFile.amountLine, path);
        }

        public static void TeleportBuilding()
        {
            TxtFile.ReadFromFile(@"\data\teleportbuilding.txt", '\t');

            string s = null;

            for (int l = 0; l <= TxtFile.amountLine - 1; l++)
            {
                s = TxtFile.lines[l].ToString();
                TxtFile.commands = s.Split('\t');

                obj o = new obj();
                int ID = Convert.ToInt32(TxtFile.commands[1]);
                short AREA = short.Parse(TxtFile.commands[41]);
                double x = Convert.ToDouble(TxtFile.commands[43]);
                double z = Convert.ToDouble(TxtFile.commands[44]);
                double y = Convert.ToDouble(TxtFile.commands[45]);
                o.Ids = new Global.ID(Global.ID.IDS.Object);
                o.UniqueID = o.Ids.GetUniqueID;
                objectdata os = new objectdata();
                os.Name = TxtFile.commands[2];
                Data.ObjectBase[ID] = os;

                o.ID = ID;
                o.area = AREA;
                o.xSec = Convert.ToByte((AREA).ToString("X4").Substring(2, 2), 16);
                o.ySec = Convert.ToByte((AREA).ToString("X4").Substring(0, 2), 16);
                o.x = (o.xSec - 135) * 192 + (x) / 10;
                o.z = z;
                o.y = (o.ySec - 92) * 192 + (y) / 10;
                o.HP = 0x000000C0;
                o.LocalType = 3;
                Systems.Objects.Add(o);
            }
            Print.Format("{0} teleporter betöltése..", TxtFile.amountLine);
            TeleportData();
        }

        public static void TeleportData()
        {
            TxtFile.ReadFromFile(@"\data\teleportdata.txt", '\t');

            string s = null;

            for (int l = 0; l <= TxtFile.amountLine - 1; l++)
            {
                s = TxtFile.lines[l].ToString();
                TxtFile.commands = s.Split('\t');

                point o = new point();
                int Number = Convert.ToInt32(TxtFile.commands[1]);
                int ID = Convert.ToInt32(TxtFile.commands[3]);
                short AREA = short.Parse(TxtFile.commands[5]);
                double x = Convert.ToDouble(TxtFile.commands[6]);
                double z = Convert.ToDouble(TxtFile.commands[7]);
                double y = Convert.ToDouble(TxtFile.commands[8]);

                o.test = Convert.ToByte(TxtFile.commands[12]);
                o.Name = TxtFile.commands[2];
                o.ID = ID;
                o.Number = Number;
                o.xSec = Convert.ToByte((AREA).ToString("X4").Substring(2, 2), 16);
                o.ySec = Convert.ToByte((AREA).ToString("X4").Substring(0, 2), 16);
                o.x = (o.xSec - 135) * 192 + (x) / 10;
                o.z = z;
                o.y = (o.ySec - 92) * 192 + (y) / 10;
                Data.PointBase[Number] = o;
            }
            Print.Format("{0} in-game pont betöltése...", TxtFile.amountLine);
        }

        public static byte GetTeleport(string name)
        {
            for (byte b = 0; b <= Data.PointBase.Length - 1; b++)
            {
                if (Data.PointBase[b] != null && Data.PointBase[b].Name == name) return b;
            }
            return 255;
        }

        public static void LoadSkillData(string path)
        {
            TxtFile.ReadFromFile(path, '\t');

            string s = null;

            for (int l = 0; l <= TxtFile.amountLine - 1; l++)
            {
                s = TxtFile.lines[l].ToString();
                TxtFile.commands = s.Split('\t');
                int ID = Convert.ToInt32(TxtFile.commands[1]);
                s_data sd = new s_data();
                sd.ID = ID;
                sd.Name = TxtFile.commands[3];
                sd.Series = TxtFile.commands[5];
                sd.NextSkill = Convert.ToInt32(TxtFile.commands[9]);
                sd.CastingTime = Convert.ToInt32(TxtFile.commands[13]);
                sd.Mastery = Convert.ToInt16(TxtFile.commands[34]);
                sd.SkillPoint = Convert.ToInt32(TxtFile.commands[46]);
                sd.Weapon1 = Convert.ToByte(TxtFile.commands[50]);
                sd.Weapon2 = Convert.ToByte(TxtFile.commands[51]);
                sd.Mana = Convert.ToInt32(TxtFile.commands[53]);
                sd.Time = Convert.ToInt32(TxtFile.commands[70]);
                sd.Per = Convert.ToInt32(TxtFile.commands[71]);
                sd.Properties1 = Convert.ToInt32(TxtFile.commands[72]);
                sd.Properties2 = Convert.ToInt32(TxtFile.commands[73]);
                sd.Properties3 = Convert.ToInt32(TxtFile.commands[74]);
                sd.Properties4 = Convert.ToInt32(TxtFile.commands[75]);
                sd.Properties5 = Convert.ToInt32(TxtFile.commands[76]);
                sd.Properties6 = Convert.ToInt32(TxtFile.commands[77]);


                /*if (sd.ID == 3977)
                    Console.WriteLine("{0}", sd.Properties2);*/

                //if(Convert.ToInt64(TxtFile.commands[78]) < 255) sd.eLevel = Convert.ToByte(TxtFile.commands[78]);
                /*if (ID == 32)
                {
                    for (int i = 0; i <= TxtFile.commands.Length - 1; i++)
                        Console.WriteLine(" {0}  {1}  ,= ID {2}", i, TxtFile.commands[i], ID);
                }
                /* if (ID == 160)
                {20 62
                    for (int i = 0; i <= ReadTxt.commands.Length - 1; i++)
                        Console.WriteLine(" {0}  {1}  ,= ID {2}", i, ReadTxt.commands[i], ID);
                }*/
                Data.SkillBase[ID] = sd;
            }
            Print.Format("{0} betöltése innen: {1}...", TxtFile.amountLine,path);
        }
        public static void LoadMasteryData()
        {
            TxtFile.ReadFromFile(@"\data\mastery.txt", '\t');

            string s = null;

            for (int l = 0; l <= TxtFile.amountLine - 1; l++)
            {
                s = TxtFile.lines[l].ToString();
                TxtFile.commands = s.Split('\t');
                short exp = Convert.ToInt16(TxtFile.commands[1]);
                byte level = Convert.ToByte(TxtFile.commands[0]);

                Data.MasteryBase[level] = exp;
            }
            Print.Format("{0} Mastery Data betöltése...", TxtFile.amountLine);
        }
        public static void LoadGoldData()
        {
            TxtFile.ReadFromFile(@"\data\levelgold.txt", '\t');

            string s = null;

            for (int l = 0; l <= TxtFile.amountLine - 1; l++)
            {
                levelgold lg = new levelgold();
                s = TxtFile.lines[l].ToString();
                TxtFile.commands = s.Split('\t');
                byte level = Convert.ToByte(TxtFile.commands[0]);
                lg.min = Convert.ToInt16(TxtFile.commands[1]);
                lg.max = Convert.ToInt16(TxtFile.commands[2]);

                Data.LevelGold[level] = lg;
            }
            Print.Format("{0} Gold Data betöltése..", TxtFile.amountLine);
        }
        public static void LoadLevelData()
        {
            TxtFile.ReadFromFile(@"\data\leveldata.txt", '\t');

            string s = null;

            for (int l = 0; l <= TxtFile.amountLine - 1; l++)
            {
                s = TxtFile.lines[l].ToString();
                TxtFile.commands = s.Split('\t');
                byte level = Convert.ToByte(TxtFile.commands[0]);
                long exp = Convert.ToInt64(TxtFile.commands[1]);

                Data.LevelData[level] = exp;
            }
            Print.Format("{0} Level Data betöltése..", TxtFile.amountLine);
        }
        public static void LoadShopTabData()
        {
            TxtFile.ReadFromFile(@"\data\shopdata.txt", '\t');

            string s = null;
            int count = TxtFile.amountLine;
            byte[] co = new byte[25000];
            Shop_Alexandria();
            for (int l = 0; l <= TxtFile.amountLine - 1; l++)
            {
                s = TxtFile.lines[l].ToString();
                TxtFile.commands = s.Split('\t');
                int ID = Convert.ToInt32(TxtFile.commands[5]);
                if(ID > 0)
                if (Data.ObjectBase[ID] != null)
                {
                    string name = TxtFile.commands[2];
                    Data.ObjectBase[ID].StoreName = name;
                }
            }
            TxtFile.ReadFromFile(@"\data\refmappingshopwithtab.txt", '\t');
            count += TxtFile.amountLine;
            for (int l = 0; l <= TxtFile.amountLine - 1; l++)
            {
                s = TxtFile.lines[l].ToString();
                TxtFile.commands = s.Split('\t');
                string name = TxtFile.commands[2];
                int ID = GetNpcID(name);
                if (Data.ObjectBase[ID] != null)
                {
                    Data.ObjectBase[ID].Shop[co[ID]] = TxtFile.commands[3];
                    co[ID]++;
                }
            }
            co = null;
            co = new byte[25000];
            TxtFile.ReadFromFile(@"\data\refshoptab.txt", '\t');
            count += TxtFile.amountLine;
            for (int l = 0; l <= TxtFile.amountLine - 1; l++)
            {
                s = TxtFile.lines[l].ToString();
                TxtFile.commands = s.Split('\t');
                string name = TxtFile.commands[4];
                int ID = GetNpcID_(name);
                if (Data.ObjectBase[ID] != null)
                {
                    shop_data sh = new shop_data();
                    sh.tab = TxtFile.commands[3];
                    Data.ShopData.Add(sh);
                    Data.ObjectBase[ID].Tab[co[ID]] = TxtFile.commands[3];
                    co[ID]++;
                }
            }
            co = null;

            TxtFile.ReadFromFile(@"\data\refshopgoods.txt", '\t');
            count += TxtFile.amountLine;
            for (int l = 0; l <= TxtFile.amountLine - 1; l++)
            {
                s = TxtFile.lines[l].ToString();
                TxtFile.commands = s.Split('\t');
                string name = TxtFile.commands[2];
                shop_data ID = shop_data.GetShopIndex(name);
                if (ID != null)
                {
                    TxtFile.commands[3] = TxtFile.commands[3].Remove(0, 8);
                    ID.Item[Convert.ToInt16(TxtFile.commands[4])] = TxtFile.commands[3];
                }
            }

            Print.Format("{0} Shop Data betöltése..", count);

            SetShopData();
        }
        public static int GetNpcID(string name)
        {
            for (int i = 0; i < Data.ObjectBase.Length; i++)
            {
                if (Data.ObjectBase[i] != null && Data.ObjectBase[i].StoreName == name) return (int)Data.ObjectBase[i].ID;
            }
            return 0;
        }
        public static int GetNpcID_(string name)
        {
            for (int i = 0; i <= Data.ObjectBase.Length - 1; i++)
            {
                if (Data.ObjectBase[i] != null)
                {
                    for (int b = 0; b <= 10 - 1; b++)
                        if (Data.ObjectBase[i].Shop[b] == name) return (int)Data.ObjectBase[i].ID;
                }
            }
            return 0;
        }
        public static void SetShopData()
        {
            #region Hotan
            Data.ObjectBase[2072].Tab[0] = "STORE_KT_SMITH_EU_TAB1";
            Data.ObjectBase[2072].Tab[1] = "STORE_KT_SMITH_EU_TAB2";
            Data.ObjectBase[2072].Tab[2] = "STORE_KT_SMITH_EU_TAB3";
            Data.ObjectBase[2072].Tab[3] = "STORE_KT_SMITH_TAB1";
            Data.ObjectBase[2072].Tab[4] = "STORE_KT_SMITH_TAB2";
            Data.ObjectBase[2072].Tab[5] = "STORE_KT_SMITH_TAB3";

            Data.ObjectBase[2073].Tab[0] = "STORE_KT_ARMOR_EU_TAB1";
            Data.ObjectBase[2073].Tab[1] = "STORE_KT_ARMOR_EU_TAB2";
            Data.ObjectBase[2073].Tab[2] = "STORE_KT_ARMOR_EU_TAB3";
            Data.ObjectBase[2073].Tab[3] = "STORE_KT_ARMOR_EU_TAB4";
            Data.ObjectBase[2073].Tab[4] = "STORE_KT_ARMOR_EU_TAB5";
            Data.ObjectBase[2073].Tab[5] = "STORE_KT_ARMOR_EU_TAB6";
            Data.ObjectBase[2073].Tab[6] = "STORE_KT_ARMOR_TAB1";
            Data.ObjectBase[2073].Tab[7] = "STORE_KT_ARMOR_TAB2";
            Data.ObjectBase[2073].Tab[8] = "STORE_KT_ARMOR_TAB3";
            Data.ObjectBase[2073].Tab[9] = "STORE_KT_ARMOR_TAB4";
            Data.ObjectBase[2073].Tab[10] = "STORE_KT_ARMOR_TAB5";
            Data.ObjectBase[2073].Tab[11] = "STORE_KT_ARMOR_TAB6";

            Data.ObjectBase[2075].Tab[0] = "STORE_KT_ACCESSORY_EU_TAB1";
            Data.ObjectBase[2075].Tab[1] = "STORE_KT_ACCESSORY_EU_TAB2";
            Data.ObjectBase[2075].Tab[2] = "STORE_KT_ACCESSORY_EU_TAB3";
            Data.ObjectBase[2075].Tab[3] = "STORE_KT_ACCESSORY_TAB1";
            Data.ObjectBase[2075].Tab[4] = "STORE_KT_ACCESSORY_TAB2";
            Data.ObjectBase[2075].Tab[5] = "STORE_KT_ACCESSORY_TAB3";
            #endregion
        }
        public static void Shop_Alexandria()
        {
            #region SMITH
            Data.ObjectBase[Global.objectdata.GetItem("NPC_SD_M_AREA_SMITH")].StoreName = "STORE_KT_SMITH";
            Data.ObjectBase[Global.objectdata.GetItem("NPC_SD_M_AREA_SMITH")].Tab[0] = "STORE_KT_SMITH_EU_TAB1";
            Data.ObjectBase[Global.objectdata.GetItem("NPC_SD_M_AREA_SMITH")].Tab[1] = "STORE_KT_SMITH_EU_TAB2";
            Data.ObjectBase[Global.objectdata.GetItem("NPC_SD_M_AREA_SMITH")].Tab[2] = "STORE_KT_SMITH_EU_TAB3";
            Data.ObjectBase[Global.objectdata.GetItem("NPC_SD_M_AREA_SMITH")].Tab[3] = "STORE_KT_SMITH_TAB1";
            Data.ObjectBase[Global.objectdata.GetItem("NPC_SD_M_AREA_SMITH")].Tab[4] = "STORE_KT_SMITH_TAB2";
            Data.ObjectBase[Global.objectdata.GetItem("NPC_SD_M_AREA_SMITH")].Tab[5] = "STORE_KT_SMITH_TAB3";

            Data.ObjectBase[Global.objectdata.GetItem("NPC_SD_T_AREA_SMITH")].StoreName = "STORE_KT_SMITH";
            Data.ObjectBase[Global.objectdata.GetItem("NPC_SD_T_AREA_SMITH")].Tab[0] = "STORE_KT_SMITH_EU_TAB1";
            Data.ObjectBase[Global.objectdata.GetItem("NPC_SD_T_AREA_SMITH")].Tab[1] = "STORE_KT_SMITH_EU_TAB2";
            Data.ObjectBase[Global.objectdata.GetItem("NPC_SD_T_AREA_SMITH")].Tab[2] = "STORE_KT_SMITH_EU_TAB3";
            Data.ObjectBase[Global.objectdata.GetItem("NPC_SD_T_AREA_SMITH")].Tab[3] = "STORE_KT_SMITH_TAB1";
            Data.ObjectBase[Global.objectdata.GetItem("NPC_SD_T_AREA_SMITH")].Tab[4] = "STORE_KT_SMITH_TAB2";
            Data.ObjectBase[Global.objectdata.GetItem("NPC_SD_T_AREA_SMITH")].Tab[5] = "STORE_KT_SMITH_TAB3";
            #endregion

            #region ACCESSORY
            Data.ObjectBase[Global.objectdata.GetItem("NPC_SD_M_AREA_ACCESSORY")].StoreName = "STORE_KT_ACCESSORY";
            Data.ObjectBase[Global.objectdata.GetItem("NPC_SD_M_AREA_ACCESSORY")].Tab[0] = "STORE_KT_ACCESSORY_EU_TAB1";
            Data.ObjectBase[Global.objectdata.GetItem("NPC_SD_M_AREA_ACCESSORY")].Tab[1] = "STORE_KT_ACCESSORY_EU_TAB2";
            Data.ObjectBase[Global.objectdata.GetItem("NPC_SD_M_AREA_ACCESSORY")].Tab[2] = "STORE_KT_ACCESSORY_EU_TAB3";
            Data.ObjectBase[Global.objectdata.GetItem("NPC_SD_M_AREA_ACCESSORY")].Tab[3] = "STORE_KT_ACCESSORY_TAB1";
            Data.ObjectBase[Global.objectdata.GetItem("NPC_SD_M_AREA_ACCESSORY")].Tab[4] = "STORE_KT_ACCESSORY_TAB2";
            Data.ObjectBase[Global.objectdata.GetItem("NPC_SD_M_AREA_ACCESSORY")].Tab[5] = "STORE_KT_ACCESSORY_TAB3";

            Data.ObjectBase[Global.objectdata.GetItem("NPC_SD_T_AREA_ACCESSORY")].StoreName = "STORE_KT_ACCESSORY";
            Data.ObjectBase[Global.objectdata.GetItem("NPC_SD_T_AREA_ACCESSORY")].Tab[0] = "STORE_KT_ACCESSORY_EU_TAB1";
            Data.ObjectBase[Global.objectdata.GetItem("NPC_SD_T_AREA_ACCESSORY")].Tab[1] = "STORE_KT_ACCESSORY_EU_TAB2";
            Data.ObjectBase[Global.objectdata.GetItem("NPC_SD_T_AREA_ACCESSORY")].Tab[2] = "STORE_KT_ACCESSORY_EU_TAB3";
            Data.ObjectBase[Global.objectdata.GetItem("NPC_SD_T_AREA_ACCESSORY")].Tab[3] = "STORE_KT_ACCESSORY_TAB1";
            Data.ObjectBase[Global.objectdata.GetItem("NPC_SD_T_AREA_ACCESSORY")].Tab[4] = "STORE_KT_ACCESSORY_TAB2";
            Data.ObjectBase[Global.objectdata.GetItem("NPC_SD_T_AREA_ACCESSORY")].Tab[5] = "STORE_KT_ACCESSORY_TAB3"; //STORE_KT_POTION STORE_KT_POTION_TAB1
            #endregion

            #region POTION
            Data.ObjectBase[Global.objectdata.GetItem("NPC_SD_M_AREA_POTION")].StoreName = "STORE_KT_POTION";
            Data.ObjectBase[Global.objectdata.GetItem("NPC_SD_M_AREA_POTION")].Tab[0] = "STORE_KT_POTION_TAB1";

            Data.ObjectBase[Global.objectdata.GetItem("NPC_SD_T_AREA_POTION")].StoreName = "STORE_KT_POTION";
            Data.ObjectBase[Global.objectdata.GetItem("NPC_SD_T_AREA_POTION")].Tab[0] = "STORE_KT_POTION_TAB1";
            #endregion
        }
        public static int SetSilk(string name)
        {
            /*  hat; 30
                dress; 130
                pot2;11
                pot3;20
                pot4;40
                pot5;60
                global;10
                arrow1k;4  */
            switch (name)
            {
                case "ITEM_MALL_HP_SUPERSET_2_BAG":
                case "ITEM_MALL_MP_SUPERSET_2_BAG":
                    return 11;
                case "ITEM_MALL_HP_SUPERSET_3_BAG":
                case "ITEM_MALL_MP_SUPERSET_3_BAG":
                    return 20;
                case "ITEM_MALL_HP_SUPERSET_4_BAG":
                case "ITEM_MALL_MP_SUPERSET_4_BAG":
                    return 40;
                case "ITEM_MALL_HP_SUPERSET_5_BAG":
                case "ITEM_MALL_MP_SUPERSET_5_BAG":
                    return 60;
                case "ITEM_MALL_QUIVER":
                    return 4;
                case "ITEM_MALL_AVATAR_M_CH_WEDDING":
                case "ITEM_MALL_AVATAR_W_CH_WEDDING":
                case "ITEM_MALL_AVATAR_W_EU_WEDDING":
                case "ITEM_MALL_AVATAR_M_EU_WEDDING":
                    return 130;
                case "ITEM_MALL_AVATAR_M_CH_WEDDING_HAT":
                case "ITEM_MALL_AVATAR_W_CH_WEDDING_HAT":
                case "ITEM_MALL_AVATAR_W_EU_WEDDING_HAT":
                case "ITEM_MALL_AVATAR_M_EU_WEDDING_HAT":
                    return 30;
                case "ITEM_MALL_GLOBAL_CHATTING":
                    return 10;
                case "ITEM_COS_C_TIGER_SCROLL":
                case "ITEM_COS_C_SCARABAEUS_SCROLL":
                case "ITEM_COS_C_PEGASUS_SCROLL":
                case "ITEM_COS_C_WILDPIG_SCROLL":
                case "ITEM_COS_C_OSTRICH_SCROLL":
                    return 1;
            }
            return 0;
        }
    }
}
