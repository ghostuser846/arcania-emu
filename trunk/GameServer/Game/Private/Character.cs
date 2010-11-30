using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace Game
{
    public partial class Systems
    {
        #region Listing
        void CharacterListing()
        {
            try
            {
                client.Send(Private.Packet.CharacterListing(Player.AccountName));
            }
            catch (Exception ex)
            {
                Console.WriteLine("CharacterListing()::error..");
                deBug.Write(ex);
            }
        }
        #endregion

        #region Delete & Restore
        void CharacterDelete()
        {
            try
            {
                PacketReader Reader = new PacketReader(PacketInformation.buffer);
                Reader.Byte();
                string name = Reader.Text();
                Reader.Close();
                MsSQL.InsertData("UPDATE karakterler SET deletedtime=dateadd(dd,7,getdate()) WHERE name='" + name + "'");
                client.Send(Private.Packet.ScreenSuccess(3));
            }
            catch (Exception ex)
            {
                Console.WriteLine("ChracterDelete()::error..");
                deBug.Write(ex);
            }
        }
        public void CharacterRestore()
        {
            PacketReader Reader = new PacketReader(PacketInformation.buffer);
            Reader.Byte();
            string name = Reader.Text();
            Reader.Close();
            MsSQL.InsertData("UPDATE karakterler SET deletedtime=0 WHERE name='" + name + "'");
            client.Send(Private.Packet.ScreenSuccess(5));
        }
        #endregion

        #region Create & Name
        void CharacterCreate()
        {
            try
            {
                //14477	~ 14502	avrupa model
                if (MsSQL.GetRowsCount("SELECT * FROM karakterler WHERE account='" + Player.AccountName + "'") == 4) return; // Kontrol
                PacketReader Reader = new PacketReader(PacketInformation.buffer);
                Reader.Byte();
                string name = Reader.Text();
                int model = Reader.Int32();
                byte volume = Reader.Byte();
                int[] Item = new int[4];
                Item[0] = Reader.Int32();
                Item[1] = Reader.Int32();
                Item[2] = Reader.Int32();
                Item[3] = Reader.Int32();
                Reader.Close();
                if (model >= 14477 && model <= 14502) { Disconnect(); return; }

                #region Check Name
                if (CharacterCheck(name))
                {
                    client.Send(Private.Packet.CharacterName(4));
                    return;
                }
                #endregion

                else
                {

                    if (model >= 14477) { Disconnect(); return; }
                    MsSQL.InsertData("INSERT INTO karakterler (account, name, chartype, volume) VALUES ('" + Player.AccountName + "','" + name + "', '" + model + "', '" + volume + "')");

                    Player.CreatingCharID = MsSQL.GetDataInt("Select * from karakterler Where name='" + name + "'", "id");
                    MsSQL.InsertData("INSERT INTO saved_skills (owner, AmountSkill) VALUES ('" + Player.CreatingCharID + "','" + 0 + "')");

                    #region Item
                    double MagDef = 3;
                    double PhyDef = 6;
                    double Parry = 11;
                    double Hit = 11;
                    double MinPhyA = 6;
                    double MaxPhyA = 9;
                    double MinMagA = 6;
                    double MaxMagA = 10;

                    AddItem(2199, 10, 13, 1, Player.CreatingCharID); // SCROLL

                    AddItem(Item[0], 0, 1, 0, Player.CreatingCharID);
                    AddItem(Item[1], 0, 4, 0, Player.CreatingCharID);
                    AddItem(Item[2], 0, 5, 0, Player.CreatingCharID);
                    AddItem(Item[3], 0, 6, 0, Player.CreatingCharID);

                    MagDef += Data.ItemBase[Item[0]].Defans.MinMagDef;
                    MagDef += Data.ItemBase[Item[1]].Defans.MinMagDef;
                    MagDef += Data.ItemBase[Item[2]].Defans.MinMagDef;
                    PhyDef += Data.ItemBase[Item[0]].Defans.MinPhyDef;
                    PhyDef += Data.ItemBase[Item[1]].Defans.MinPhyDef;
                    PhyDef += Data.ItemBase[Item[2]].Defans.MinPhyDef;
                    Parry += Data.ItemBase[Item[0]].Defans.Parry;
                    Parry += Data.ItemBase[Item[1]].Defans.Parry;
                    Parry += Data.ItemBase[Item[2]].Defans.Parry;
                    Hit += Data.ItemBase[Item[2]].Attack.MinAttackRating;
                    MinPhyA += Data.ItemBase[Item[3]].Attack.Min_LPhyAttack;
                    MaxPhyA += Data.ItemBase[Item[3]].Attack.Min_HPhyAttack;
                    MinMagA += Data.ItemBase[Item[3]].Attack.Min_LMagAttack;
                    MaxMagA += Data.ItemBase[Item[3]].Attack.Min_HMagAttack;
                    if (Item[3] == 3632 || Item[3] == 3633)
                    {
                        MagDef += Data.ItemBase[251].Defans.MinMagDef;
                        PhyDef += Data.ItemBase[251].Defans.MinPhyDef;
                        Parry += Data.ItemBase[251].Defans.Parry;
                        AddItem(251, 0, 7, 0, Player.CreatingCharID);
                    }
                    if (Item[3] == 3636)
                    {
                        AddItem(62, 250, 7, 1, Player.CreatingCharID);
                    }
                    #endregion

                    if (model >= 14477 && model <= 14502) { Disconnect(); return; }

                    MsSQL.UpdateData("update karakterler set min_phyatk='" + (int)Math.Round(MinPhyA) +
                            "', max_phyatk='" + Math.Round(MaxPhyA) +
                            "', min_magatk='" + Math.Round(MinMagA) +
                            "', max_magatk='" + Math.Round(MaxMagA) +
                            "', phydef='" + Math.Round(PhyDef) +
                            "', magdef='" + Math.Round(MagDef) +
                            "', parry='" + Math.Round(Parry) +
                            "', hit='" + Math.Round(Hit) +
                            "' where name='" + name + "'");

                    AddMastery(257, Player.CreatingCharID); //blade
                    AddMastery(258, Player.CreatingCharID); //heuksal
                    AddMastery(259, Player.CreatingCharID); //bow
                    AddMastery(273, Player.CreatingCharID); //cold
                    AddMastery(274, Player.CreatingCharID); //light
                    AddMastery(275, Player.CreatingCharID); //fire
                    AddMastery(276, Player.CreatingCharID); //force

                    client.Send(Private.Packet.ScreenSuccess(1));

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("CharacterCreate()::Error...");
                deBug.Write(ex);
            }
        }
        public void CharacterCheck(byte[] buff)
        {
            try
            {
                PacketReader Reader = new PacketReader(buff);
                Reader.Byte();

                string name = Reader.Text();
                Reader.Close();
                if (CharacterCheck(name))
                    client.Send(Private.Packet.CharacterName(4));
                else client.Send(Private.Packet.ScreenSuccess(4));
            }
            catch (Exception ex)
            {
                Console.WriteLine("CharacterCheck()::error..");
                deBug.Write(ex);
            }
        }
        public bool CharacterCheck(string name)
        {
            bool tr = false;
            try
            {
                if (name.Length > 3 && name.Length < 12)
                {
                    string dbname = MsSQL.GetData("SELECT name FROM karakterler WHERE name='" + name + "'", "name");
                    if (dbname == null)
                    {
                        tr = false;
                    }
                    else tr = true;
                }
                else tr = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("CharacterCheck()::error..");
                deBug.Write(ex);
            }
            return tr;
        }
        #endregion

        public void LoginScreen()
        {
            try
            {
                PacketReader Reader = new PacketReader(PacketInformation.buffer);
                string name = Reader.Text();

                Reader.Close();
                Karakter = new character();

                Karakter.Information.Name = name;
                Karakter.Account.ID = Player.ID;
                PlayerDataLoad();
                checkSameChar(name, Karakter.Information.UniqueID);

                clients.Add(this);

                client.Send(Private.Packet.LoginScreen());
                client.Send(Private.Packet.StartPlayerLoad());
                client.Send(Private.Packet.Load(Karakter));
                client.Send(Private.Packet.EndPlayerLoad());

                //client.Send(Private.Packet.PlayerUnknowPack(Karakter.Information.UniqueID,0x25, 0, 0, 0x1));

                OpenTimer();
            }
            catch (Exception ex)
            {
                Console.WriteLine("LoginScreen()::Error..");
                deBug.Write(ex);
            }
        }
        void checkSameChar(string name, int id)
        {
            for (int i = 0; i < Systems.clients.Count; i++)
            {
                if (Systems.clients[i] != null && Systems.clients[i].Karakter.Information.Name == name || Systems.clients[i].Karakter.Information.UniqueID == id)
                {
                    Print.Format("Same char... :{0}", name);
                    Systems.clients[i].Disconnect();
                }
            }
        }
        void InGameSuccess()
        {
            if (!Karakter.InGame)
            {
                this.Karakter.InGame = true;
                client.Send(Private.Packet.PlayerStat(Karakter));
                client.Send(Private.Packet.PlayerUnknowPack(Karakter.Information.UniqueID));
                client.Send(Private.Packet.Test1());
                client.Send(Private.Packet.Silk(Player.Silk, 0, 0));
                /*client.Send(Private.Packet.Test2());*/
                ObjectSpawnCheck();
                UpdateHp();
                UpdateMp();

                if (!Karakter.Transport.Horse.Information)
                {
                    client.Send(Public.Packet.Pet_Information(Karakter.Transport.Horse.UniqueID, Karakter.Transport.Horse.Model, Karakter.Transport.Horse.Hp));
                    Karakter.Transport.Horse.Information = true;
                    //Karakter.Transport.Horse.Send(this, Public.Packet.Pet_Information(Karakter.Transport.Horse.UniqueID, Karakter.Transport.Horse.Model, Karakter.Transport.Horse.Hp));
                }
                PacketWriter Writer = new PacketWriter();
                Writer.Create(0x3288);
                Writer.Word(0);
                client.Send(Writer.GetBytes());
            }
        }
        void InGameSuccess2()
        {

        }
        public void PlayerDataLoad()
        {
            try
            {
                if (Karakter == null) return;
                MsSQL ms = new MsSQL("SELECT * FROM karakterler WHERE name='" + Karakter.Information.Name + "'");

                using (SqlDataReader reader = ms.Read())
                {
                    while (reader.Read())
                    {
                        Karakter.Information.CharacterID = reader.GetInt32(0);
                        Karakter.Ids = new Global.ID(Karakter.Information.CharacterID, Global.ID.IDS.Player);
                        Karakter.Information.UniqueID = Karakter.Ids.GetUniqueID;
                        Karakter.Account.ID = Player.ID;
                        Karakter.Account.ID += 410000;
                        Karakter.Information.Model = reader.GetInt32(3);
                        Karakter.Information.Volume = reader.GetByte(4);
                        Karakter.Information.Level = reader.GetByte(5);

                        Karakter.Stat.Strength = reader.GetInt16(6);
                        Karakter.Stat.Intelligence = reader.GetInt16(7);
                        Karakter.Information.Attributes = reader.GetInt16(8);

                        Karakter.Stat.Hp = reader.GetInt32(9);
                        Karakter.Stat.Mp = reader.GetInt32(10);

                        Karakter.Information.Gold = reader.GetInt64(11);
                        Karakter.Information.XP = reader.GetInt64(12);
                        Karakter.Information.SpBar = reader.GetInt32(13);
                        Karakter.Information.SkillPoint = reader.GetInt32(14);


                        Karakter.Information.GM = reader.GetByte(15);
                        Karakter.Position.xSec = reader.GetByte(16);
                        Karakter.Position.ySec = reader.GetByte(17);

                        Karakter.Position.x = reader.GetInt32(19);
                        Karakter.Position.y = reader.GetInt32(20);
                        Karakter.Position.z = reader.GetInt32(21);

                        Karakter.Stat.SecondHp = reader.GetInt32(22);
                        Karakter.Stat.SecondMP = reader.GetInt32(23);

                        Karakter.Stat.MinPhyAttack = Convert.ToDouble(reader.GetInt32(24));
                        Karakter.Stat.MaxPhyAttack = Convert.ToDouble(reader.GetInt32(25));
                        Karakter.Stat.MinMagAttack = Convert.ToDouble(reader.GetInt32(26));
                        Karakter.Stat.MaxMagAttack = Convert.ToDouble(reader.GetInt32(27));


                        Karakter.Stat.PhyDef = Convert.ToDouble(reader.GetInt16(28));
                        Karakter.Stat.MagDef = Convert.ToDouble(reader.GetInt16(29));

                        Karakter.Stat.Hit = Convert.ToDouble(reader.GetInt16(30));
                        Karakter.Stat.Parry = Convert.ToDouble(reader.GetInt16(31));

                        Karakter.Speed.WalkSpeed = reader.GetInt32(33);
                        Karakter.Speed.RunSpeed = reader.GetInt32(34);
                        Karakter.Speed.BerserkSpeed = reader.GetInt32(35);
                        Karakter.Information.BerserkBar = reader.GetByte(36);
                        Karakter.Speed.DefaultSpeed = Karakter.Speed.RunSpeed;

                        Karakter.Stat.mag_Absorb = reader.GetInt16(38);
                        Karakter.Stat.phy_Absorb = reader.GetInt16(39);

                        Karakter.Information.Place = reader.GetByte(40);
                        Karakter.Information.Title = reader.GetByte(41);
                        Karakter.Account.StorageGold = Player.pGold;
                        Karakter.State.type1 = 1;
                        Karakter.LogNum = 55;
                    }
                }
                ms.Close();

                Karakter.Stat.Skill.Mastery = new int[8];
                Karakter.Stat.Skill.Mastery_Level = new byte[8];
                Karakter.Stat.Skill.Skill = new int[250];


                ms = new MsSQL("SELECT * FROM mastery WHERE owner='" + Karakter.Information.CharacterID + "'");
                using (SqlDataReader reader = ms.Read())
                {
                    byte c = 1;
                    while (reader.Read())
                    {
                        Karakter.Stat.Skill.Mastery[c] = reader.GetInt32(2);
                        Karakter.Stat.Skill.Mastery_Level[c] = reader.GetByte(3);
                        c++;
                    }
                }
                ms.Close();

                ms = new MsSQL("SELECT * FROM saved_skills WHERE owner='" + Karakter.Information.CharacterID + "'");
                using (SqlDataReader reader = ms.Read())
                {
                    while (reader.Read())
                    {
                        Karakter.Stat.Skill.AmountSkill = reader.GetInt32(2);
                        for (int i = 1; i <= Karakter.Stat.Skill.AmountSkill; i++)
                            Karakter.Stat.Skill.Skill[i] = reader.GetInt32(i + 2);
                    }
                }
                ms.Close();

                UpdateXY();
                if (Karakter.Stat.SecondHp < 1) //eğer karakter olupde dc olmussa..
                {
                    Karakter.Stat.SecondHp = (Karakter.Stat.Hp / 2);
                    Teleport_UpdateXYZ(Karakter.Information.Place);
                }

                SetBalance();
                CheckFile();
                SetStat();
            }
            catch (Exception ex)
            {
                Console.WriteLine("PlayerDataLoad()::error..");
                deBug.Write(ex);
            }
        }
        void UpdateXY()
        {
            Karakter.Position.x = Function.Formule.gamex(Karakter.Position.x, Karakter.Position.xSec);
            Karakter.Position.y = Function.Formule.gamey(Karakter.Position.y, Karakter.Position.ySec);
        }
        void ReUpdateXY()
        {
            Karakter.Position.x = Function.Formule.packetx(Karakter.Position.x, Karakter.Position.xSec);
            Karakter.Position.y = Function.Formule.packety(Karakter.Position.y, Karakter.Position.ySec);
        }
        void CheckFile()
        {
            string player_path = Environment.CurrentDirectory + @"\player\info\";
            if (!System.IO.File.Exists(player_path + @"quickbar\" + Karakter.Information.Name + ".dat"))
            {
                byte[] by = new byte[255];
                for (byte i = 0; i <= 254; i++) by[i] = 0x00;
                System.IO.File.Create(player_path + @"quickbar\" + Karakter.Information.Name + ".dat").Close();
                System.IO.File.WriteAllBytes(player_path + @"quickbar\" + Karakter.Information.Name + ".dat", by);
            }
            if (!System.IO.File.Exists(player_path + @"autopot\" + Karakter.Information.Name + ".dat"))
            {
                byte[] by = new byte[7];
                for (byte i = 0; i <= 6; i++) by[i] = 0x00;
                System.IO.File.Create(player_path + @"autopot\" + Karakter.Information.Name + ".dat").Close();
                System.IO.File.WriteAllBytes(player_path + @"autopot\" + Karakter.Information.Name + ".dat", by);
            }
            if (!System.IO.File.Exists(player_path + @"debug\" + Karakter.Information.Name + ".txt"))
            {
                System.IO.File.Create(player_path + @"debug\" + Karakter.Information.Name + ".txt").Close();
            }
        }
        public void SavePlayerInfo()
        {
            try
            {
                MsSQL.UpdateData("update karakterler set min_phyatk='" + Convert.ToInt32(Math.Round(Karakter.Stat.MinPhyAttack)) +
                    "', max_phyatk='" + Convert.ToInt32(Math.Round(Karakter.Stat.MaxPhyAttack)) +
                    "', min_magatk='" + Convert.ToInt32(Math.Round(Karakter.Stat.MinMagAttack)) +
                    "', max_magatk='" + Convert.ToInt32(Math.Round(Karakter.Stat.MaxMagAttack)) +
                    "', phydef='" + Convert.ToInt16(Math.Round(Karakter.Stat.PhyDef - Karakter.Stat.uPhyDef)) +
                    "', magdef='" + Convert.ToInt16(Math.Round(Karakter.Stat.MagDef - Karakter.Stat.uMagDef)) +
                    "', hit='" + Convert.ToInt16(Math.Round(Karakter.Stat.Hit)) +
                    "', parry='" + Convert.ToInt16(Math.Round(Karakter.Stat.Parry)) +
                    "', hp='" + Karakter.Stat.Hp +
                    "', mp='" + Karakter.Stat.Mp +
                    "', s_hp='" + Karakter.Stat.SecondHp +
                    "', s_mp='" + Karakter.Stat.SecondMP +
                    "', attribute='" + Karakter.Information.Attributes +
                    "', strength='" + Karakter.Stat.Strength +
                    "', intelligence='" + Karakter.Stat.Intelligence +
                    "', experience='" + Convert.ToInt64(Karakter.Information.XP) +
                    "', spbar='" + Karakter.Information.SpBar +
                    "', sp='" + Karakter.Information.SkillPoint +
                    "', level='" + Karakter.Information.Level +
                    "', mag_absord='" + Karakter.Stat.mag_Absorb +
                    "', phy_absord='" + Karakter.Stat.phy_Absorb +
                    "' where id='" + Karakter.Information.CharacterID + "'");
            }
            catch (Exception ex)
            {
                Console.WriteLine("SavePlayerInfo()::error..");
                deBug.Write(ex);
            }
        }
        public void SavePlayerExperince()
        {
            try
            {
                MsSQL.UpdateData("update karakterler set  experience='" + Convert.ToInt64(Karakter.Information.XP) +
                    "', spbar='" + Karakter.Information.SpBar +
                    "', sp='" + Karakter.Information.SkillPoint +
                    "' where id='" + Karakter.Information.CharacterID + "'");
            }
            catch (Exception ex)
            {
                Console.WriteLine("SavePlayerInfo()::error..");
                deBug.Write(ex);
            }
        }
        protected void SavePlayerPosition()
        {
            MsSQL.UpdateData("update karakterler set xsect='" + Karakter.Position.xSec +
                "', ysect='" + Karakter.Position.ySec +
                "', xpos='" + Math.Round(Function.Formule.packetx(Karakter.Position.x, Karakter.Position.xSec)) +
                "', ypos='" + Math.Round(Function.Formule.packety(Karakter.Position.y, Karakter.Position.ySec)) + 
                "', zpos='" + Math.Round(Karakter.Position.z) + 
                "' where id='" + Karakter.Information.CharacterID + "'");
        }
        protected void SetBalance()
        {
            int maxstat = 28 + Karakter.Information.Level * 4;
            Karakter.Information.Phy_Balance = (byte)(99 - (100 * 2 / 3 * (maxstat - Karakter.Stat.Strength) / maxstat));
            Karakter.Information.Mag_Balance = (byte)(100 * Karakter.Stat.Intelligence / maxstat);
        }
        protected void SaveGold()
        {
            MsSQL.UpdateData("update karakterler set gold='" + Karakter.Information.Gold +
                "' where id='" + Karakter.Information.CharacterID + "'");

            MsSQL.UpdateData("update users set gold='" + Karakter.Account.StorageGold +
                "' where id='" + Player.AccountName + "'");
        }
        protected void Save()
        {
            PacketReader Reader = new PacketReader(PacketInformation.buffer);
            string player_path;
            byte[] file;
            switch (Reader.Byte())
            {
                case 1:
                    player_path = Environment.CurrentDirectory + @"\player\info\quickbar\" + Karakter.Information.Name + ".dat";
                    file = System.IO.File.ReadAllBytes(player_path);

                    byte Slot = Reader.Byte();
                    byte sType = Reader.Byte();

                    Slot *= 5;

                    file[Slot] = sType;
                    file[Slot + 1] = Reader.Byte();
                    file[Slot + 2] = Reader.Byte();
                    file[Slot + 3] = Reader.Byte();
                    file[Slot + 4] = Reader.Byte();
                    System.IO.File.WriteAllBytes(player_path,file);
                    break;
                case 2: 
                    player_path = Environment.CurrentDirectory + @"\player\info\autopot\"+ Karakter.Information.Name + ".dat";
                    file = System.IO.File.ReadAllBytes(player_path);
                    file[0] = Reader.Byte();
                    file[1] = Reader.Byte();
                    file[2] = Reader.Byte();
                    file[3] = Reader.Byte();
                    file[4] = Reader.Byte();
                    file[5] = Reader.Byte();
                    file[6] = Reader.Byte();
                    System.IO.File.WriteAllBytes(player_path, file);
                    UpdateHp();
                    UpdateMp();
                    break;
            }
            Reader.Close();
        }
        protected void SavePlace()
        {

            PacketReader reader = new PacketReader(PacketInformation.buffer);
            int ObjectID = reader.Int32();
            obj o = GetObject(ObjectID);
            byte type = 0;
            switch (Data.ObjectBase[o.ID].Name)
            {
                case "STORE_CH_GATE":
                    type = 3;
                    break;
                case "STORE_WC_GATE":
                    type = 4;
                    break;
                case "STORE_KT_GATE":
                    type = 7;
                    break;
                case "STORE_EU_GATE":
                    type = 22;
                    break;
                case "STORE_CA_GATE":
                    type = 27;
                    break;
                case "STORE_SD_GATE1":
                    type = 177;
                    break;
                case "STORE_SD_GATE2":
                    type = 178;
                    break;
            }
            Karakter.Information.Place = type;
            MsSQL.InsertData("update karakterler set savearea='" + Karakter.Information.Place + "' where id='" + Karakter.Information.CharacterID + "'");
            client.Send(Private.Packet.UpdatePlace());
        }
        public void UpdateHp()
        {
            Send(Private.Packet.UpdatePlayer(this.Karakter.Information.UniqueID, 0x20, 1, this.Karakter.Stat.SecondHp));
            if (Karakter.Network.Party != null)
            {
                Karakter.Network.Party.Send(Public.Packet.Party_Data(6, this.Karakter.Information.UniqueID));
            }
        }
        public void UpdateHp(character Karakter)
        {
            Send(Private.Packet.UpdatePlayer(Karakter.Information.UniqueID, 0x20, 1, Karakter.Stat.SecondHp));
        }
        public void UpdateMp()
        {
            Send(Private.Packet.UpdatePlayer(this.Karakter.Information.UniqueID, 0x10, 2, this.Karakter.Stat.SecondMP));
            if (Karakter.Network.Party != null)
            {
                Karakter.Network.Party.Send(Public.Packet.Party_Data(6, this.Karakter.Information.UniqueID));
            }
        }
        public void SetStat()
        {
            Karakter.Stat.Hp = Function.Formule.gamePhp(Karakter.Information.Level, Karakter.Stat.Strength);
            Karakter.Stat.Mp = Function.Formule.gamePmp(Karakter.Information.Level, Karakter.Stat.Intelligence);
            client.Send(Private.Packet.PlayerStat(Karakter));
        }
        public void UpdateStrengthInfo(byte amount)
        {
            Karakter.Stat.MinPhyAttack += (0.45 * amount);
            Karakter.Stat.MaxPhyAttack += (0.65 * amount);
            Karakter.Stat.PhyDef += (0.40 * amount);
        }
        public void UpdateIntelligenceInfo(byte amount)
        {
            Karakter.Stat.MinMagAttack += (0.45 * amount);
            Karakter.Stat.MaxMagAttack += (0.65 * amount);
            Karakter.Stat.MagDef += (0.40 * amount);
        }
        public void InsertStr()
        {
            try
            {
                if (Karakter.Information.Attributes > 0)
                {
                    Karakter.Information.Attributes -= 1;
                    Karakter.Stat.Strength++;
                    UpdateStrengthInfo(1);
                    client.Send(Private.Packet.UpdateStr());
                    SetStat();
                    SavePlayerInfo();
                }
            }
            catch(Exception ex)
            {
                Console.Write("\nGame error:InsertStr()...\n");
                deBug.Write(ex);
            }
        }
        public void InsertInt()
        {
            try
            {
                if (Karakter.Information.Attributes > 0)
                {
                    Karakter.Information.Attributes -= 1;
                    Karakter.Stat.Intelligence++;
                    UpdateIntelligenceInfo(1);
                    client.Send(Private.Packet.UpdateInt());
                    SetStat();
                    SavePlayerInfo();
                }
            }
            catch (Exception ex)
            {
                Console.Write("\nGame error:InsertInt()...\n");
                deBug.Write(ex);
            }
        }
        protected void Close_NPC()
        {
            client.Send(Private.Packet.CloseNPC());
        }
        protected void Open_NPC()
        {
            try
            {
                PacketReader Reader = new PacketReader(PacketInformation.buffer);
                Reader.UInt32();
                client.Send(Private.Packet.OpenNPC(Reader.Byte()));
                Reader.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
            //Reader.Close();
        }
        protected void Open_Warehouse()
        {
            client.Send(Private.Packet.OpenWarehouse(Karakter.Account.StorageGold));
            client.Send(Private.Packet.OpenWarehouse2(0x96, this.Player));
            client.Send(Private.Packet.OpenWarehouse3());
        }
    }
}

