using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework;
namespace Game
{
    public partial class obj
    {
        public void FollowHim(Systems sys)
        {
            try
            {
                if (sys != null && this.LocalType == 1 && !this.Walking && !sys.Karakter.State.Die && !sys.Karakter.Action.Check() && !sys.Karakter.Transport.Right)
                {
                    if (!this.Attacking && this.Agresif == 1)
                    {
                        if (this.x >= (sys.Karakter.Position.x - 10) && this.x <= ((sys.Karakter.Position.x - 10) + 20) && this.y >= (sys.Karakter.Position.y - 10) && this.y <= ((sys.Karakter.Position.y - 10) + 20))
                        {
                            Target = sys;
                            StartAttackTimer(3500);
                            this.Attacking = true;

                            AddAgroDmg(sys.Karakter.Information.UniqueID, 1);
                            return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("FollowHim()::Error...");
                deBug.Write(ex);
            }
        }
        public void CheckEveryOne()
        {
            try
            {
                for (int b = 0; b <= Systems.clients.Count - 1; b++)
                {
                    if (this.Spawned(Systems.clients[b].Karakter.Information.UniqueID))
                        FollowHim(Systems.clients[b]);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("CheckEveryOne()::error...");
                deBug.Write(ex);
            }
        }
        public void AttackMain()
        {
            try
            {
                Systems sys;
                if (Agro != null) sys = (Systems)GetTarget();
                else { return; }

                if (bSleep) return;

                if (sys == null || Die || GetDie) { StopAttackTimer(); return; }
                if (sys != null && !this.Spawned(sys.Karakter.Information.UniqueID)) return;
                if (!sys.MonsterCheck(this.UniqueID)) sys.Karakter.Action.MonsterID.Add(this.UniqueID);
                if (!sys.Karakter.InGame) { StopAttackTimer(); return; }

                ChangeState(1, 3);
                float farkx = sys.Karakter.Position.x;
                float farky = sys.Karakter.Position.y;

                double distance = Function.Formule.gamedistance((float)this.x,
                (float)this.y,
                farkx,
                farky);

                this.Busy = true;
                Attacking = true;

                if (distance >= 100 || !this.AutoMovement && distance > 50)
                {
                    StopAttackTimer();
                    if (this.AutoMovement) StartMovement(rnd.Next(8000, 10000));
                    return;
                }
                
                if (distance >= 3 && this.AutoMovement)
                {
                    Systems.aRound(ref sys.Karakter.aRound, ref farkx, ref farky);

                    this.xSec = (byte)((farkx / 192) + 135);
                    this.ySec = (byte)((farky / 192) + 92);

                    Send(Public.Packet.Movement(new Game.Global.vektor(this.UniqueID,
                        (float)Function.Formule.packetx((float)farkx, sys.Karakter.Position.xSec),
                        (float)sys.Karakter.Position.z,
                        (float)Function.Formule.packety((float)farky, sys.Karakter.Position.ySec),
                        this.xSec,
                        this.ySec)));

                    this.wx = farkx - this.x;
                    this.wy = farky - this.y;

                    WalkingTime = (double)(distance / 40) * 10000;
                    RecordedTime = WalkingTime;

                    Walking = true;
        

                    this.StartMovement((int)(WalkingTime / 10));
                    this.Busy = true;
                    return;
                }
                AttackHim();
            }
            catch (Exception ex)
            {
                Console.WriteLine("AttackMain()::error..");
                deBug.Write(ex);
            }
        }
        /// <summary>
        /// Célpont megtámadása.
        /// </summary>
        void AttackHim()
        {
            try
            {
                if (!Walking && Attacking && !bSleep)
                {
                    Systems sys = (Systems)GetTarget();
                    if (sys == null || Die || GetDie) { StopAttackTimer(); return; }
                    if (sys != null && !this.Spawned(sys.Karakter.Information.UniqueID)) { StopAttackTimer(); return; }
                    if (!sys.Karakter.InGame) { StopAttackTimer(); return; }

                    byte NumberAttack = 1;
                    int AttackType = Data.ObjectBase[this.ID].Skill[rnd.Next(0, Data.ObjectBase[this.ID].amountSkill)];

                    int p_dmg = 0;
                    byte status = 0, crit = 1;
                    
                    PacketWriter Writer = new PacketWriter();
                    Writer.Create(Systems.SERVER_ACTION_DATA);
                    Writer.Byte(1);
                    Writer.Byte(2);
                    Writer.Byte(0x30);

                    Writer.DWord(AttackType);
                    Writer.DWord(this.UniqueID);
                    //if (this.LastCasting != 0) Global.ID.Delete((int)this.LastCasting);
                    this.LastCasting = this.Ids.GetCastingID();
                    //Console.WriteLine("Cast:{0}",this.LastCasting);
                    Writer.DWord(this.LastCasting);
                    Writer.DWord(sys.Karakter.Information.UniqueID);

                    Writer.Bool(true);
                    Writer.Byte(NumberAttack);
                    Writer.Byte(1);

                    Writer.DWord(sys.Karakter.Information.UniqueID);

                    for (byte n = 1; n <= NumberAttack; n++)
                    {
                        bool block = false;

                        if (sys.Karakter.Information.Item.sID != 0 && Data.ItemBase[sys.Karakter.Information.Item.sID].Class_D == 1)
                        {
                            if (sys.rnd.Next(25) < 10) block = true;
                        }
                        if (!block)
                        {
                            status = 0;
                            crit = 1;

                            p_dmg = (int)Function.Formule.gamedamage(Data.SkillBase[AttackType].Properties2, 0, sys.Karakter.Stat.phy_Absorb, sys.Karakter.Stat.PhyDef, 50, Data.SkillBase[AttackType].Per);
                            p_dmg += rnd.Next(0, p_dmg.ToString().Length);
                            if (p_dmg <= 0) p_dmg = 1;

                            if (rnd.Next(20) > 15)
                            {
                                p_dmg *= 2;
                                crit = 2;
                            }

                            if (sys.Karakter.Stat.Absorb_mp > 0)
                            {
                                int static_dmg = (p_dmg * (100 - (int)sys.Karakter.Stat.Absorb_mp)) / 100;
                                sys.Karakter.Stat.SecondMP -= static_dmg;
                                if (sys.Karakter.Stat.SecondMP < 0) sys.Karakter.Stat.SecondMP = 0;
                                sys.UpdateMp();
                                p_dmg = static_dmg;
                            }

                            sys.Karakter.Stat.SecondHp -= p_dmg;

                            if (sys.Karakter.Stat.SecondHp <= 0)
                            {
                                sys.BuffAllClose();
                                status = 128;
                                sys.Karakter.Stat.SecondHp = 0;
                                sys.Karakter.State.Die = true;
                                sys.Karakter.State.DeadType = 1;

                                if (sys.Karakter.Action.nAttack) sys.StopAttackTimer();
                                else if (sys.Karakter.Action.sAttack || sys.Karakter.Action.sCasting) sys.StopSkillTimer();

                                _agro agro = GetAgroClass(sys.Karakter.Information.UniqueID);
                                if (agro != null) Agro.Remove(agro);
                                DeleteTarget();
                                StopAttackTimer();
                                CheckAgro();
                            }

                            sys.UpdateHp();

                            Writer.Byte(status);
                            Writer.Byte(crit);
                            Writer.DWord(p_dmg);
                            Writer.Byte(0);
                            Writer.Word(0);
                        }
                        else
                            Writer.Byte(2);
                    }

                    this.Attacking = false;
                    Send(Writer.GetBytes());
                //StartAttackTimer(2500);
            }
            }
            catch (Exception ex)
            {
                Console.WriteLine("AttackHim()::error...");
                deBug.Write(ex);
            }
        }
        /// <summary>
        /// Aggro ellenőrzése
        /// </summary>
        public void CheckAgro()
        {
            if (this.Agro.Count > 0)
            {
                StartAttackTimer(3500);
            }
            else
                StartRunTimer(rnd.Next(7000, 20000));
        }
        public void ChangeState(byte type, byte type2)
        {
            Send(Private.Packet.StatePack(this.UniqueID, type, type2, false));
        }
        public void Send(byte[] buff)
        {
            for (int i = 0; i <= Systems.clients.Count - 1; i++)
            {
                if (Systems.clients[i] != null && this.Spawned(Systems.clients[i].Karakter.Information.UniqueID))
                {
                    Systems.clients[i].client.Send(buff);
                }
            }
        }
        /// <summary>
        /// Objektum spawnolása.
        /// </summary>
        public void SpawnMe()
        {

            if (!this.Die)
            for (int i = 0; i <= Systems.clients.Count - 1; i++)
            {
                Systems sys = Systems.clients[i];
                if (this.x >= (sys.Karakter.Position.x - 50) && this.x <= ((sys.Karakter.Position.x - 50) + 100) && this.y >= (sys.Karakter.Position.y - 50) && this.y <= ((sys.Karakter.Position.y - 50) + 100) && this.Spawned(sys.Karakter.Information.UniqueID) == false)
                {
                    this.Spawn.Add(sys.Karakter.Information.UniqueID);
                    sys.client.Send(Public.Packet.ObjectSpawn(this));
                }
            }
            StartRunTimer(rnd.Next(7000, 20000));
        }
        public void DeSpawnMe()
        {
            StopAutoRunTimer();
            if(this.Die)
            for (int i = 0; i <= Systems.clients.Count - 1; i++)
            {
                Systems sys = Systems.clients[i];
                if (Spawned(sys.Karakter.Information.UniqueID))
                {
                    Spawn.Remove(sys.Karakter.Information.UniqueID);
                    sys.client.Send(Public.Packet.ObjectDeSpawn(this.UniqueID));
                }
            }
        }
        public void reSpawn()
        {
            this.Agro = null;
            this.HP = Data.ObjectBase[ID].HP;
            this.Type = Systems.RandomType(Data.ObjectBase[this.ID].Level, ref this.Kat);
            this.x = oX;
            this.y = oY;
            this.HP *= this.Kat;
            this.GetDie = false;
            this.Die = false;
            this.Busy = false;

            this.SpawnMe();

            if (this.spawnOran > 0 && rnd.Next(0, 100) < this.spawnOran)
            {
                this.spawnOran -= 5;
                RandomMonster(this.ID, 0);
            }
        }
        /// <summary>
        /// Véletlenszerű mobot hozz létre, és beállítja annak jellemzőit.
        /// </summary>
        /// <param name="sID">A mob azonosítója</param>
        /// <param name="randomTYPE">véletlenszerű típus</param>
        public void RandomMonster(int sID, byte randomTYPE)
        {
            try
            {
                obj o = new obj();
                o.ID = sID;
                o.Ids = new Global.ID(Global.ID.IDS.Object);
                o.UniqueID = o.Ids.GetUniqueID;
                o.x = this.x;
                o.z = this.z;
                o.y = this.y;
                o.oY = this.oY;
                o.oX = this.oX;
                //Systems.aRound(ref o.oX, ref o.oY, 1);

                o.xSec = this.xSec;
                o.ySec = this.ySec;

                o.AutoMovement = this.AutoMovement;
                if (ID == 1979 || ID == 2101 || ID == 2124 || ID == 2111 || ID == 2112) o.AutoMovement = false;
                o.AutoSpawn = true;
                o.Move = 1;

                o.HP = Data.ObjectBase[o.ID].HP;
                o.Agresif = 0;
                o.LocalType = 1;
                o.State = 2;
                o.Kat = 1;
                o.Agro = new List<_agro>();
                o.spawnOran = 0;

                if (randomTYPE == 0) // Standart
                {
                    o.Type = Systems.RandomType(Data.ObjectBase[o.ID].Level, ref this.Kat);
                    if (o.Type == 1) o.Agresif = 1;
                    o.HP *= this.Kat;
                }
                else
                {
                    if (randomTYPE == 6)
                        o.HP *= 4;
                    else if (randomTYPE == 4)
                        o.HP *= 20;
                    else if (randomTYPE == 1)
                        o.HP *= 2;
                    o.AutoSpawn = false;
                    o.Type = randomTYPE;
                    o.Agresif = 1;
                }

                o.SpawnMe();
                Systems.Objects.Add(o);
                o.CheckEveryOne();
            }
            catch (Exception ex)
            {
                Console.WriteLine("random mob error..");
                deBug.Write(ex);
            }
        }

        public void SetDrop()
        {
            try
            {
                if (this.GetDie || this.Die)
                {
                    if (this.Type != 3)
                    {
                        Systems sys = (Systems)this.GetTarget();
                        if (sys == null) return;
                        sbyte levelFark = (sbyte)(sys.Karakter.Information.Level - Data.ObjectBase[ID].Level);
                        byte test = 1;
                        if (Math.Abs(levelFark) < 25 || levelFark == 0)
                        {
                            #region Gold
                            int goldMiktar = rnd.Next(Data.LevelGold[Data.ObjectBase[ID].Level].min, Data.LevelGold[Data.ObjectBase[ID].Level].max);
                            world_item item = new world_item();
                            item.amount = goldMiktar * Rate.Gold;
                            item.Model = 1;

                            if (item.amount < 1000) item.Model = 1;
                            else if (item.amount > 1000 && item.amount < 10000) item.Model = 2;
                            else if (item.amount > 10000) item.Model = 3;

                            item.Ids = new Global.ID(Global.ID.IDS.World);
                            item.UniqueID = item.Ids.GetUniqueID;
                            item.x = this.x;
                            item.z = this.z;
                            item.y = this.y;
                            Systems.aRound(ref item.x, ref item.y, 1);
                            item.xSec = this.xSec;
                            item.ySec = this.ySec;
                            item.Type = 1;
                            item.downType = true;
                            item.fromType = 5;

                            Systems.WorldItem.Add(item);
                            item.Send(Public.Packet.ObjectSpawn(item), true);
                            #endregion
                            
                            #region Item and cop
                            if (rnd.Next(0, 100) < 2 * Rate.Item)
                            {
                                List<int> itemList = GetLevelItem(Data.ObjectBase[ID].Level);

                                if (2 * Rate.Item > 50) test = 2;
                                if (itemList.Count > 0)
                                {
                                    for (byte b = 1; b <= test; b++)
                                    {

                                        #region Item
                                        world_item sitem = new world_item();
                                        sitem.Model = itemList[rnd.Next(0, itemList.Count - 1)];
                                        sitem.Ids = new Global.ID(Global.ID.IDS.World);
                                        sitem.UniqueID = sitem.Ids.GetUniqueID;
                                        sitem.PlusValue = Function.Items.RandomPlusValue();
                                        sitem.x = this.x;
                                        sitem.z = this.z;
                                        sitem.y = this.y;
                                        Systems.aRound(ref sitem.x, ref sitem.y, 1);
                                        sitem.xSec = this.xSec;
                                        sitem.ySec = this.ySec;
                                        sitem.Type = 2;
                                        sitem.fromType = 5;
                                        sitem.downType = true;
                                        sitem.fromOwner = this.UniqueID;
                                        sitem.Owner = ((Systems)this.GetTarget()).Karakter.Account.ID;

                                        Systems.WorldItem.Add(sitem);
                                        sitem.Send(Public.Packet.ObjectSpawn(sitem), true);
                                        #endregion
                                    }
                                }
                            }

                            int[] etc = Drop.SetEtc(Data.ObjectBase[ID].Level);

                            if (rnd.Next(0, 100) < 50 * Rate.Item)
                            {
                                world_item sitem2 = new world_item();

                                sitem2.Model = etc[rnd.Next(0, etc.Length)];
                                sitem2.amount = 1;

                                if (sitem2.Model == 62) sitem2.amount = 200 / rnd.Next(4, 5);
                                if (sitem2.amount == 0) sitem2.amount = 20;

                                sitem2.Ids = new Global.ID(Global.ID.IDS.World);
                                sitem2.UniqueID = sitem2.Ids.GetUniqueID;
                                sitem2.x = this.x;
                                sitem2.z = this.z;
                                sitem2.y = this.y;
                                Systems.aRound(ref sitem2.y, ref sitem2.x, 1);
                                sitem2.xSec = this.xSec;
                                sitem2.ySec = this.ySec;
                                sitem2.Type = 3;
                                sitem2.fromType = 5;
                                sitem2.fromOwner = this.UniqueID;
                                sitem2.downType = true;
                                sitem2.Owner = ((Systems)this.GetTarget()).Karakter.Account.ID;

                                Systems.WorldItem.Add(sitem2);
                                sitem2.Send(Public.Packet.ObjectSpawn(sitem2), true);
                            }
                            #endregion

                            List<int> SoxList = GetLevelItemSOX(Data.ObjectBase[ID].Level);
                            if (SoxList.Count > 0)
                            {
                                if (rnd.Next(0, 100) <= Rate.Sox)
                                {
                                    #region Item
                                    world_item sitem = new world_item();
                                    sitem.Model = SoxList[rnd.Next(0, SoxList.Count - 1)];
                                    sitem.Ids = new Global.ID(Global.ID.IDS.World);
                                    sitem.UniqueID = sitem.Ids.GetUniqueID;
                                    sitem.PlusValue = Function.Items.RandomPlusValue();
                                    sitem.x = this.x;
                                    sitem.z = this.z;
                                    sitem.y = this.y;
                                    Systems.aRound(ref sitem.x, ref sitem.y, 1);
                                    sitem.xSec = this.xSec;
                                    sitem.ySec = this.ySec;
                                    sitem.Type = 2;
                                    sitem.fromType = 5;
                                    sitem.downType = true;
                                    sitem.fromOwner = this.UniqueID;
                                    sitem.Owner = ((Systems)this.GetTarget()).Karakter.Account.ID;

                                    Systems.WorldItem.Add(sitem);
                                    sitem.Send(Public.Packet.ObjectSpawn(sitem), true);
                                    #endregion
                                }
                            }
                        }
                    }
                    else
                    {
                        for (byte b = 0; b <= rnd.Next(7, 15); b++)
                        {
                            #region Gold
                            int goldMiktar = rnd.Next(Data.LevelGold[Data.ObjectBase[ID].Level].min, Data.LevelGold[Data.ObjectBase[ID].Level].max);
                            world_item item = new world_item();
                            item.amount = goldMiktar * Rate.Gold;
                            item.Model = 1;
                            if (item.amount < 1000) item.Model = 1;
                            else if (item.amount > 1000 && item.amount < 10000) item.Model = 2;
                            else if (item.amount > 10000) item.Model = 3;
                            item.Ids = new Global.ID(Global.ID.IDS.World);
                            item.UniqueID = item.Ids.GetUniqueID;
                            item.x = this.x;
                            item.z = this.z;
                            item.y = this.y;
                            Systems.aRound(ref item.x, ref item.y, 1);
                            item.xSec = this.xSec;
                            item.ySec = this.ySec;
                            item.Type = 1;
                            item.downType = true;
                            item.fromType = 5;

                            Systems.WorldItem.Add(item);
                            item.Send(Public.Packet.ObjectSpawn(item), true);
                            #endregion
                        }
                        List<int> itemList = GetLevelItem(Data.ObjectBase[ID].Level);

                        if (itemList.Count > 1)
                        {
                            for (byte b = 1; b <= rnd.Next(7, 15); b++)
                            {
                                #region Item
                                world_item sitem = new world_item();
                                sitem.Model = itemList[rnd.Next(0, itemList.Count - 1)];
                                sitem.PlusValue = Function.Items.RandomPlusValue();
                                sitem.Ids = new Global.ID(Global.ID.IDS.World);
                                sitem.UniqueID = sitem.Ids.GetUniqueID;
                                sitem.x = this.x;
                                sitem.z = this.z;
                                sitem.y = this.y;
                                Systems.aRound(ref sitem.x, ref sitem.y, 1);
                                sitem.xSec = this.xSec;
                                sitem.ySec = this.ySec;
                                sitem.Type = 2;
                                sitem.fromType = 5;
                                sitem.downType = true;
                                sitem.fromOwner = this.UniqueID;
                                sitem.Owner = ((Systems)this.GetTarget()).Karakter.Account.ID;

                                Systems.WorldItem.Add(sitem);
                                sitem.Send(Public.Packet.ObjectSpawn(sitem), true);
                                #endregion
                            }
                        }
                        for (byte b = 1; b <= rnd.Next(7, 15); b++)
                        {

                            int[] etc = Drop.SetEtc(Data.ObjectBase[ID].Level);

                            world_item sitem2 = new world_item();

                            sitem2.Model = etc[rnd.Next(0, etc.Length)];
                            sitem2.amount = 1;

                            if (sitem2.Model == 62) sitem2.amount = 200 / rnd.Next(4, 5);
                            if (sitem2.amount == 0) sitem2.amount = 20;

                            sitem2.Ids = new Global.ID(Global.ID.IDS.World);
                            sitem2.UniqueID = sitem2.Ids.GetUniqueID;
                            sitem2.x = this.x;
                            sitem2.z = this.z;
                            sitem2.y = this.y;
                            Systems.aRound(ref sitem2.y, ref sitem2.x, 1);
                            sitem2.xSec = this.xSec;
                            sitem2.ySec = this.ySec;
                            sitem2.Type = 3;
                            sitem2.fromType = 5;
                            sitem2.fromOwner = this.UniqueID;
                            sitem2.downType = true;
                            sitem2.Owner = ((Systems)this.GetTarget()).Karakter.Account.ID;

                            Systems.WorldItem.Add(sitem2);
                            sitem2.Send(Public.Packet.ObjectSpawn(sitem2), true);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("SetDrop()::error..");
                deBug.Write(ex);
            }
        }
        public List<int> GetLevelItem(byte level)
        {
            List<int> item = new List<int>();
            for (int i = 0; i < Data.ItemBase.Length; i++)
            {
                if (Data.ItemBase[i] != null)
                {
                    if (Data.ItemBase[i].Level != 0 && Data.ItemBase[i].Level >= level - 2 && Data.ItemBase[i].Level <= (level + 4) && GetItemType(Data.ItemBase[i].ID) == 0 && Data.ItemBase[i].SOX == 0 && Data.ItemBase[i].SoulBound == 1 && Data.ItemBase[i].Race == 0)
                    {
                        if(i != 0)
                            item.Add(i);
                    }
                }
            }
            return item;
        }
        public List<int> GetLevelItemSOX(byte level)
        {
            List<int> item = new List<int>();
            for (int i = 0; i < Data.ItemBase.Length; i++)
            {
                if (Data.ItemBase[i] != null)
                {
                    if (Data.ItemBase[i].Level >= level - 2 && Data.ItemBase[i].Level <= (level + 4) && GetItemType(Data.ItemBase[i].ID) == 0 && Data.ItemBase[i].SOX == 2 && Data.ItemBase[i].Race == 0)
                    {
                        if (i != 0)
                            item.Add(i);
                    }
                }
            }
            return item;
        }
        byte GetItemType(int id)
        {
            if (Data.ItemBase[id].Class_D == 1) return 0;
            else if (Data.ItemBase[id].Class_D == 3) return 1;

            return 255;
        }
        public void SetExperience()
        {
            try
            {
                if (this.GetDie)
                {
                    int yuzde = 0;
                    long exp = Data.ObjectBase[this.ID].Exp;
                    long sp = 97;
                    bool level = false;
                    byte mainyuzde = 100;
                    Systems enFazlaVuranPlayer;
                    if(Agro != null)
                    for (byte b = 0; b < Agro.Count; b++)
                    {
                        if (Agro[b].playerID != 0)
                        {
                            enFazlaVuranPlayer = Systems.GetPlayer(Agro[b].playerID);
                            if (enFazlaVuranPlayer != null)
                            {
                                short stat = enFazlaVuranPlayer.Karakter.Information.Attributes;

                                yuzde = (Agro[b].playerDMD * 100) / (Data.ObjectBase[this.ID].HP * this.Kat);
                                if (yuzde >= mainyuzde) yuzde = mainyuzde;
                                exp = Data.ObjectBase[this.ID].Exp;
                                sp = 97;
                                level = false;
                                if (mainyuzde > 0)
                                {
                                    int gap = Math.Abs(enFazlaVuranPlayer.Karakter.Information.Level - enFazlaVuranPlayer.MasteryGetBigLevel) * 10;
                                    if (gap >= 90) gap = 90;

                                    mainyuzde -= (byte)yuzde;
                                    exp *= Rate.Xp;
                                    exp -= (exp * ((Math.Abs(Data.ObjectBase[this.ID].Level - enFazlaVuranPlayer.Karakter.Information.Level) - Math.Abs(enFazlaVuranPlayer.Karakter.Information.Level - enFazlaVuranPlayer.MasteryGetBigLevel)) * 10)) / 100;

                                    byte kat = this.Kat;
                                    if (kat == 20) kat = 5;
                                    exp *= kat;

                                    //Console.WriteLine("playerid:{0}  Yuzde:{1} mainyuzde:{2} firstex:{3} calcex:{4}", Agro[b].playerID, yuzde, mainyuzde, exp, (exp * yuzde) / 100);
                                    exp = (exp * yuzde) / 100;
                                    exp = (exp * (100 - gap)) / 100;

                                    if (enFazlaVuranPlayer.Karakter.Information.Level == 110 && enFazlaVuranPlayer.Karakter.Information.XP >= 4000000000)
                                    {
                                        exp = 0;
                                    }

                                    if (Math.Abs(Data.ObjectBase[this.ID].Level - enFazlaVuranPlayer.Karakter.Information.Level) < 10)
                                    {
                                        int gaplevel = Math.Abs(enFazlaVuranPlayer.Karakter.Information.Level - enFazlaVuranPlayer.Karakter.Information.Level) * 10;
                                        sp = (sp * (100 + gap)) / 100;
                                        sp *= kat;
                                        sp *= Rate.Sp;
                                    }
                                    else sp = 10;

                                    if (exp <= 1) exp = 1;
                                    if (sp <= 1) sp = 10;
                                    enFazlaVuranPlayer.Karakter.Information.XP += exp;

                                    while (enFazlaVuranPlayer.Karakter.Information.XP >= Data.LevelData[enFazlaVuranPlayer.Karakter.Information.Level])
                                    {
                                        enFazlaVuranPlayer.Karakter.Information.XP -= Data.LevelData[enFazlaVuranPlayer.Karakter.Information.Level];
                                        if (enFazlaVuranPlayer.Karakter.Information.XP < 1) enFazlaVuranPlayer.Karakter.Information.XP = 0;
                                        stat += 3;
                                        enFazlaVuranPlayer.Karakter.Information.Attributes += 3;
                                        enFazlaVuranPlayer.Karakter.Information.Level++;
                                        enFazlaVuranPlayer.Karakter.Stat.Intelligence++;
                                        enFazlaVuranPlayer.Karakter.Stat.Strength++;
                                        enFazlaVuranPlayer.UpdateIntelligenceInfo(1);
                                        enFazlaVuranPlayer.UpdateStrengthInfo(1);
                                        enFazlaVuranPlayer.SetStat();
                                        level = true;
                                    }

                                    SetSp(enFazlaVuranPlayer, sp);
                                    if (level)
                                    {
                                        enFazlaVuranPlayer.Send(Public.Packet.Player_LevelUpEffect(enFazlaVuranPlayer.Karakter.Information.UniqueID));
                                        enFazlaVuranPlayer.client.Send(Public.Packet.Player_getExp(enFazlaVuranPlayer.Karakter.Information.UniqueID, exp, sp, stat));
                                        enFazlaVuranPlayer.SavePlayerInfo();
                                    }
                                    else
                                    {
                                        enFazlaVuranPlayer.client.Send(Public.Packet.Player_getExp(enFazlaVuranPlayer.Karakter.Information.UniqueID, exp, sp, 0));
                                        enFazlaVuranPlayer.SavePlayerExperince();
                                    }
                                }
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("SetExperience():error...");
                deBug.Write(ex);
            }
        }
        public static void SetSp(Systems sys, long sp)
        {
            sys.Karakter.Information.SkillPoint += (((int)sp + sys.Karakter.Information.SpBar) / 400);
            sys.Karakter.Information.SpBar = (((int)sp + sys.Karakter.Information.SpBar) % 400);
            sys.client.Send(Private.Packet.InfoUpdate(2, sys.Karakter.Information.SkillPoint,0));
        }
        public void CheckUnique()
        {
            if(this.Type == 3)
                if (this.ID == 1954 || this.ID == 5871 || this.ID == 1982 || this.ID == 2002 || this.ID == 3810 || this.ID == 3875 || this.ID == 14538)
                { 
                int yuzde = ((this.HP * 100) / Data.ObjectBase[this.ID].HP);
                int[] bs = Systems.GetEliteIds(this.ID);
                if (yuzde > 99)
                {
                    if(!guard[0])
                    for (byte b = 0; b <= 8; b++)
                    {
                        if (bs[b] != 0)
                            RandomMonster((int)bs[b], 1);
                        guard[0] = true;
                    }
                }
                else if (yuzde < 80 && yuzde > 70)
                {
                    if (!guard[1])
                    for (byte b = 0; b <= 8; b++)
                    {
                        if (bs[b] != 0)
                            RandomMonster((int)bs[b], 6);
                        guard[1] = true;
                    }
                }
                else if (yuzde < 60 && yuzde > 50)
                {
                    if (!guard[2])
                        for (byte b = 0; b <= 8; b++)
                        {
                            if (bs[b] != 0)
                                RandomMonster((int)bs[b], 4);
                            guard[2] = true;
                        }
                }
            }
        }
        public void CheckUnique(Systems s)
        {
            if (this.Type == 3)
            {
                Systems.SendAll(Public.Packet.Unique_Data(6, (int)this.ID, s.Karakter.Information.Name));
            }
        }
    }
}
