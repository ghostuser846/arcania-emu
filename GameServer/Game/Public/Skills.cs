using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework;

namespace Game
{
    public partial class Systems
    {
        #region Skill Main
        void ActionMain()
        {
            if (Karakter.State.Die) return;

            PacketReader Reader = new PacketReader(PacketInformation.buffer);
            byte type = Reader.Byte();
            if (type != 2)
            {
                if (Karakter.Action.Cast || Karakter.Information.Scroll) return;
                switch (Reader.Byte())
                {
                    case 1: // normal attack
                        Reader.Byte();
                        if (Karakter.Action.nAttack) return;
                        int id = Reader.Int32();
                        object os = GetObjects(id);
                        if (id == Karakter.Information.UniqueID) return;
                        Karakter.Action.PickUping = false;
                        //Karakter.State.Busy = true;
                        Karakter.Action.Object = os;
                        Karakter.Action.nAttack = true;
                        Karakter.Action.Target = id;
                        StartAttackTimer(1200);
                        Reader.Close();
                        break;
                    case 2://pickup
                        if (Karakter.Action.sAttack || Karakter.Action.sCasting) return;
                        Reader.Byte();
                        id = Reader.Int32();
                        Karakter.Action.Target = id;
                        Reader.Close();
                        client.Send(Private.Packet.ActionState(1, 1));
                        Karakter.Action.PickUping = true;
                        Player_PickUp();
                        break;
                    case 4://use skill
                        Karakter.Action.PickUping = false;
                        Karakter.Action.UsingSkillID = Reader.Int32();
                        SkillMain(Reader.Byte(), Reader);
                        break;
                    case 5:
                        id = Reader.Int32();
                        byte b_index = SkillGetBuffIndex(id);
                        SkillBuffEnd(b_index);
                        break;
                }
            }
            else
                StopAttackTimer();
        }
        void SkillMain(byte type, PacketReader Reader)
        {
            if (!SkillGetOpened(Karakter.Action.UsingSkillID)) return;
            client.Send(Private.Packet.ActionState(1, type));
            switch (type)
            {
                case 1:
                    if (Karakter.Action.sAttack || Karakter.Action.sCasting) return;
                    if (Karakter.Action.nAttack) { Karakter.Action.nAttack = false; StopAttackTimer(); }
                    if (!Base.Skill.CheckWeapon(Karakter.Information.Item.wID, Karakter.Action.UsingSkillID)) return;
                    Karakter.Action.Target = Reader.Int32();
                    Karakter.Action.Skill = Base.Skill.Info(Karakter.Action.UsingSkillID);
                    if (!Karakter.Action.Skill.canUse || Karakter.Action.Target == Karakter.Information.UniqueID) return;
                    Karakter.Action.Skill.MainSkill = Karakter.Action.UsingSkillID;
                    Karakter.Action.UsingSkillID = 0;
                    Karakter.Action.Object = GetObjects(Karakter.Action.Target);
                    obj o = null;
                    if (Karakter.Action.Object != null && Karakter.Action.Object.GetType().ToString() == "Game.obj")
                    {
                        o = Karakter.Action.Object as obj;
                        if (o.Agro == null) o.Agro = new List<_agro>();
                        if (Karakter.Action.Skill.OzelEffect == 5 && o.State != 4) return;
                        if (o.State == 4 && Karakter.Action.Skill.OzelEffect != 5) return;
                    }

                    if (o == null && Karakter.Action.Object != null && Karakter.Action.Object.GetType().ToString() == "Game.Systems")
                    {
                        if (!Karakter.Information.PvP || Karakter.State.Die) return;
                        Systems sys = Karakter.Action.Object as Systems;
                        if (Karakter.Action.Skill.OzelEffect == 5 && sys.Karakter.State.LastState != 5) return;
                        if (sys.Karakter.State.LastState == 4 && Karakter.Action.Skill.OzelEffect != 5) return;
                    }

                    Karakter.Action.sAttack = true;
                    ActionSkillAttack();
                    Reader.Close();
                    break;
                case 0:
                    SkillBuff();
                    break;
                case 2://run skill
                    MovementSkill(Reader);
                    break;
            }
        }

        #endregion

        #region Buff
        void SkillBuff()
        {
           
            if (SkillGetSameBuff(Karakter.Action.UsingSkillID)) return;
            if (Karakter.Action.Buff.count > 21) return;
            byte slot = SkillBuffGetFreeSlot();
            if (slot == 255) return;
            if (Karakter.Stat.SecondMP < Data.SkillBase[Karakter.Action.UsingSkillID].Mana) { client.Send(Public.Packet.ActionPacket(2, 4)); Karakter.Action.Cast = false; return; }
            else
            {
                Karakter.Stat.SecondMP -= Data.SkillBase[Karakter.Action.UsingSkillID].Mana;

                if (Karakter.Stat.SecondMP < 0) Karakter.Stat.SecondMP = 1;
                UpdateMp();

                if (BuffAdd()) { Karakter.Action.Cast = false; return; }

                Karakter.Action.Buff.SkillID[slot] = Karakter.Action.UsingSkillID;
                Karakter.Action.Buff.OverID[slot] = Karakter.Ids.GetBuffID();

                Karakter.Action.CastingSkill = Karakter.Ids.GetCastingID();
                Karakter.Action.Buff.slot = slot;
                Karakter.Action.Buff.count++;
                List<int> lis = Karakter.Spawn;
                Send(lis, Public.Packet.ActionPacket(1, 0, Karakter.Action.UsingSkillID, Karakter.Information.UniqueID, Karakter.Action.CastingSkill, 0));

                Karakter.Action.Cast = true;

                StartCastingTimer(Karakter.Action.Buff.castingtime, lis);
            }
        }
        void ItemBuff(int skillID)
        {
            Karakter.Action.UsingSkillID = skillID;

            if (SkillGetSameBuff(Karakter.Action.UsingSkillID)) return;
            if (Karakter.Action.Buff.count > 21) return;
            byte slot = SkillBuffGetFreeSlot();
            if (slot == 255) return;

            if (BuffAdd()) { Karakter.Action.Cast = false; return; }

            Karakter.Action.Buff.SkillID[slot] = Karakter.Action.UsingSkillID;
            Karakter.Action.Buff.OverID[slot] = Karakter.Ids.GetBuffID();

            Karakter.Action.CastingSkill = Karakter.Ids.GetCastingID();
            Karakter.Action.Buff.slot = slot;
            Karakter.Action.Buff.count++;
            List<int> lis = Karakter.Spawn;
            Send(lis, Public.Packet.ActionPacket(1, 0, Karakter.Action.UsingSkillID, Karakter.Information.UniqueID, Karakter.Action.CastingSkill, 0));

            Send(lis, Public.Packet.SkillIconPacket(Karakter.Information.UniqueID, Karakter.Action.UsingSkillID, Karakter.Action.Buff.OverID[Karakter.Action.Buff.slot]));
            StartBuffTimer(Data.SkillBase[Karakter.Action.UsingSkillID].Per, Karakter.Action.Buff.slot);
        }
        void SkillBuffCasting(List<int> list)
        {
            Send(list, Public.Packet.SkillPacket(0, Karakter.Action.CastingSkill));
            Send(list, Public.Packet.SkillIconPacket(Karakter.Information.UniqueID, Karakter.Action.UsingSkillID, Karakter.Action.Buff.OverID[Karakter.Action.Buff.slot]));

            client.Send(Private.Packet.PlayerStat(Karakter));
            client.Send(Private.Packet.ActionState(2, 0));
            StartBuffTimer(Data.SkillBase[Karakter.Action.UsingSkillID].Time, Karakter.Action.Buff.slot);
            Karakter.Action.Buff.slot = 255;
        }
        void SkillBuffEnd(byte b)
        {
            if (Timer.Buff[b] != null)
            {
                BuffDelete(b);
                Timer.Buff[b].Dispose();

                Send(Public.Packet.SkillEndBuffPacket(Karakter.Action.Buff.OverID[b]));
                Global.ID.Delete(Karakter.Action.Buff.OverID[b]);
                Karakter.Action.Buff.OverID[b] = 0;
                Karakter.Action.Buff.SkillID[b] = 0;
                Karakter.Action.Buff.count--;
            }
        }
        byte SkillBuffGetFreeSlot()
        {
            for (byte b = 0; b <= Karakter.Action.Buff.SkillID.Length - 1; b++)
                if (Karakter.Action.Buff.SkillID[b] == 0) return b;
            return 255;
        }
        public bool SkillGetSameBuff(int SkillID)
        {
            for (byte b = 0; b <= 19; b++)
            {
                if (Karakter.Action.Buff.SkillID[b] != 0)
                {
                    if (Data.SkillBase[Karakter.Action.Buff.SkillID[b]].Series.Remove(Data.SkillBase[Karakter.Action.Buff.SkillID[b]].Series.Length - 2)
                        == Data.SkillBase[SkillID].Series.Remove(Data.SkillBase[SkillID].Series.Length - 2)) return true;
                }
            }
            return false;
        }
        public bool SkillGetOpened(int SkillID)
        {
            if (Karakter.Information.GM == 1) return true;
            for (int b = 0; b <= Karakter.Stat.Skill.AmountSkill; b++)
            {
                if (Karakter.Stat.Skill.Skill[b] != 0 && Karakter.Information.GM == 0 && Karakter.Stat.Skill.Skill[b] == SkillID) return true;
            }
            return false;
        }
        byte SkillGetBuffIndex(int SkillID)
        {
            for (byte b = 0; b <= 19; b++)
            {
                if (Karakter.Action.Buff.SkillID[b] != 0 && Karakter.Action.Buff.SkillID[b] == SkillID)
                {
                    return b;
                }
            }
            return 255;
        }
        public void BuffAllClose()
        {
            if (Karakter != null)
            {
                for (byte b = 0; b < Karakter.Action.Buff.SkillID.Length; b++) if (Karakter.Action.Buff.SkillID[b] != 0) SkillBuffEnd(b);
            }
        }
        bool BuffAdd()
        {
            try
            {
                switch (Data.SkillBase[Karakter.Action.UsingSkillID].Series.Remove(Data.SkillBase[Karakter.Action.UsingSkillID].Series.Length - 2))
                {
                    case "SKILL_CH_COLD_GIGONGTA":
                    case "SKILL_CH_LIGHTNING_GIGONGTA":
                    case "SKILL_CH_FIRE_GIGONGTA":
                        if (Karakter.Action.ImbueID != 0) return true;
                        Karakter.Action.ImbueID = Karakter.Action.UsingSkillID;
                        Karakter.Action.Buff.castingtime = 0;
                        break;
                    case "SKILL_CH_LIGHTNING_GYEONGGONG":
                        Karakter.Speed.DefaultSpeedSkill = Karakter.Action.UsingSkillID;
                        Karakter.Speed.Updateded += Data.SkillBase[Karakter.Action.UsingSkillID].Properties1;
                        Player_SetNewSpeed();
                        Send(Public.Packet.SetSpeed(Karakter.Information.UniqueID, Karakter.Speed.WalkSpeed, Karakter.Speed.RunSpeed));
                        Karakter.Action.Buff.castingtime = 0;
                        break;
                    case "SKILL_CH_LIGHTNING_GWANTONG":
                        StopAttackTimer();
                        Karakter.Stat.UpdatededMagAttack = Data.SkillBase[Karakter.Action.UsingSkillID].Properties2;
                        Karakter.Action.Buff.castingtime = 2000;
                        break;
                    case "SKILL_CH_LIGHTNING_JIPJUNG":
                        StopAttackTimer();
                        Karakter.Stat.Parry += Data.SkillBase[Karakter.Action.UsingSkillID].Properties1;
                        Karakter.Action.Buff.castingtime = 2000;
                        break;
                    case "SKILL_CH_FIRE_GONGUP":
                        StopAttackTimer();
                        Karakter.Stat.UpdatededPhyAttack += Data.SkillBase[Karakter.Action.UsingSkillID].Properties1;
                        Karakter.Action.Buff.castingtime = 2000;
                        break;
                    case "SKILL_CH_FIRE_GANGGI":
                        StopAttackTimer();
                        Karakter.Stat.MagDef += Data.SkillBase[Karakter.Action.UsingSkillID].Properties2;
                        Karakter.Stat.uMagDef += Data.SkillBase[Karakter.Action.UsingSkillID].Properties2;
                        Karakter.Action.Buff.castingtime = 2000;
                        break;
                    case "SKILL_CH_COLD_GANGGI":
                        StopAttackTimer();
                        Karakter.Stat.PhyDef += Data.SkillBase[Karakter.Action.UsingSkillID].Properties1;
                        Karakter.Stat.uPhyDef += Data.SkillBase[Karakter.Action.UsingSkillID].Properties1;
                        Karakter.Action.Buff.castingtime = 2000;
                        break;
                    case "SKILL_CH_COLD_SHIELD":
                        StopAttackTimer();
                        Karakter.Stat.Absorb_mp = Data.SkillBase[Karakter.Action.UsingSkillID].Properties1;
                        Karakter.Action.Buff.castingtime = 0;
                        break;
                    case "SKILL_CH_SPEAR_SPIN":
                        if (Data.SkillBase[Karakter.Action.UsingSkillID].Properties1 == 0)
                        {
                            Karakter.Stat.MagDef += Data.SkillBase[Karakter.Action.UsingSkillID].Properties2;
                            Karakter.Stat.uMagDef += Data.SkillBase[Karakter.Action.UsingSkillID].Properties2;
                        }

                        break;
                    case "SKILL_CH_SWORD_SHIELD":
                        if (Data.SkillBase[Karakter.Action.UsingSkillID].Properties4 != 7)
                        {
                            Karakter.Stat.PhyDef += Data.SkillBase[Karakter.Action.UsingSkillID].Properties4;
                            Karakter.Stat.uPhyDef += Data.SkillBase[Karakter.Action.UsingSkillID].Properties4;
                        }
                        break;
                    case "SKILL_CH_SWORD_SHIELDPD":
                        /*if (MsSQL.GetDataInt("SELECT * FROM char_items WHERE itemnumber='item" + 7 + "' AND owner='" + Data.Players[index].Information.CharacterID + "'", "itemid") != 0)
                        {
                            Data.Players[index].Stats.PhyDef += Object.SkillDatabase[skillID].Properties1;
                            Data.Players[index].Stats.MinPhyAttack += Object.SkillDatabase[skillID].Properties2;
                            Data.Players[index].Stats.MaxPhyAttack += Object.SkillDatabase[skillID].Properties2;
                        }*/
                        break;
                    case "SKILL_CH_BOW_CALL":
                        StopAttackTimer();
                        if (Data.SkillBase[Karakter.Action.UsingSkillID].Properties3 == 0) Karakter.Stat.AttackPower += Data.SkillBase[Karakter.Action.UsingSkillID].Properties1;
                        break;
                    case "SKILL_CH_BOW_NORMAL":
                        Karakter.Stat.EkstraMetre += (Data.SkillBase[Karakter.Action.UsingSkillID].Properties1);
                        break;
                    case "SKILL_CH_WATER_SELFHEAL":
                        if (Karakter.Stat.SecondHp + Data.SkillBase[Karakter.Action.UsingSkillID].Time < Karakter.Stat.Hp)
                            Karakter.Stat.SecondHp += Data.SkillBase[Karakter.Action.UsingSkillID].Time;
                        else
                            Karakter.Stat.SecondHp += Karakter.Stat.Hp - Karakter.Stat.SecondHp;

                        Karakter.Action.Buff.castingtime = (short)Data.SkillBase[Karakter.Action.UsingSkillID].CastingTime;
                        UpdateHp();
                        break;
                    case "SKILL_CH_WATER_HEAL":
                        Systems s = GetPlayer(Karakter.Action.Target);
                        if (s.Karakter.Stat.SecondHp + Data.SkillBase[Karakter.Action.UsingSkillID].Time < s.Karakter.Stat.Hp)
                            s.Karakter.Stat.SecondHp += Data.SkillBase[Karakter.Action.UsingSkillID].Time;
                        else
                            s.Karakter.Stat.SecondHp += s.Karakter.Stat.Hp - s.Karakter.Stat.SecondHp;

                        Karakter.Action.Buff.castingtime = (short)Data.SkillBase[Karakter.Action.UsingSkillID].CastingTime;
                        UpdateHp(s.Karakter);
                        break;
                    case "SKILL_ETC_ARCHEMY_POTION_SPEED_":
                        Karakter.Speed.DefaultSpeedSkill = Karakter.Action.UsingSkillID;
                        Karakter.Speed.Updateded += Data.SkillBase[Karakter.Action.UsingSkillID].Properties2;
                        Player_SetNewSpeed();
                        Send(Public.Packet.SetSpeed(Karakter.Information.UniqueID, Karakter.Speed.WalkSpeed, Karakter.Speed.RunSpeed));
                        Karakter.Action.Buff.castingtime = 0;
                        break;
                }
            }
            catch (Exception ex)
            {
                deBug.Write(ex);
            }
            return false;
        }
        void BuffDelete(byte b_index)
        {
            try
            {
                switch (Data.SkillBase[Karakter.Action.Buff.SkillID[b_index]].Series.Remove(Data.SkillBase[Karakter.Action.Buff.SkillID[b_index]].Series.Length - 2))
                {
                    case "SKILL_CH_COLD_GIGONGTA":
                    case "SKILL_CH_LIGHTNING_GIGONGTA":
                    case "SKILL_CH_FIRE_GIGONGTA":
                        Karakter.Action.ImbueID = 0;
                        break;
                    case "SKILL_CH_LIGHTNING_GYEONGGONG":
                        Karakter.Speed.Updateded -= Data.SkillBase[Karakter.Speed.DefaultSpeedSkill].Properties1;
                        Player_SetNewSpeed();
                        Send(Public.Packet.SetSpeed(Karakter.Information.UniqueID, Karakter.Speed.WalkSpeed, Karakter.Speed.RunSpeed));
                        break;
                    case "SKILL_CH_LIGHTNING_GWANTONG":
                        Karakter.Stat.UpdatededMagAttack = 0;
                        break;
                    case "SKILL_CH_LIGHTNING_JIPJUNG":
                        Karakter.Stat.Parry -= Data.SkillBase[Karakter.Action.Buff.SkillID[b_index]].Properties1;
                        client.Send(Private.Packet.PlayerStat(Karakter));
                        break;
                    case "SKILL_CH_FIRE_GONGUP":
                        Karakter.Stat.UpdatededPhyAttack = 0;
                        break;
                    case "SKILL_CH_FIRE_GANGGI":
                        Karakter.Stat.MagDef -= Data.SkillBase[Karakter.Action.Buff.SkillID[b_index]].Properties2;
                        Karakter.Stat.uMagDef -= Data.SkillBase[Karakter.Action.Buff.SkillID[b_index]].Properties2;
                        client.Send(Private.Packet.PlayerStat(Karakter));
                        break;
                    case "SKILL_CH_COLD_GANGGI":
                        Karakter.Stat.PhyDef -= Data.SkillBase[Karakter.Action.Buff.SkillID[b_index]].Properties1;
                        Karakter.Stat.uPhyDef -= Data.SkillBase[Karakter.Action.Buff.SkillID[b_index]].Properties1;
                        client.Send(Private.Packet.PlayerStat(Karakter));
                        break;
                    case "SKILL_CH_COLD_SHIELD":
                        Karakter.Stat.Absorb_mp = 0;
                        break;
                    case "SKILL_CH_SPEAR_SPIN":
                        if (Data.SkillBase[Karakter.Action.Buff.SkillID[b_index]].Properties1 == 0) 
                        { 
                            Karakter.Stat.MagDef -= Data.SkillBase[Karakter.Action.Buff.SkillID[b_index]].Properties2;
                            Karakter.Stat.uMagDef -= Data.SkillBase[Karakter.Action.Buff.SkillID[b_index]].Properties2;
                            client.Send(Private.Packet.PlayerStat(Karakter));
                        }
                        break;
                    case "SKILL_CH_SWORD_SHIELD":
                        if (Data.SkillBase[Karakter.Action.Buff.SkillID[b_index]].Properties4 != 7) 
                        { 
                            Karakter.Stat.PhyDef -= Data.SkillBase[Karakter.Action.Buff.SkillID[b_index]].Properties4;
                            Karakter.Stat.uPhyDef -= Data.SkillBase[Karakter.Action.Buff.SkillID[b_index]].Properties2;
                            client.Send(Private.Packet.PlayerStat(Karakter));
                        }
                        break;
                    case "SKILL_CH_SWORD_SHIELDPD":
                        /*if (MsSQL.GetDataInt("SELECT * FROM char_items WHERE itemnumber='item" + 7 + "' AND owner='" + Data.Players[index].Information.CharacterID + "'", "itemid") != 0)
                        {
                            Data.Players[index].Stats.PhyDef -= Object.SkillDatabase[skillID].Properties1;
                            Data.Players[index].Stats.MinPhyAttack -= Object.SkillDatabase[skillID].Properties2;
                            Data.Players[index].Stats.MaxPhyAttack -= Object.SkillDatabase[skillID].Properties2;
                        }*/
                        break;
                    case "SKILL_CH_BOW_CALL":
                        if (Data.SkillBase[Karakter.Action.Buff.SkillID[b_index]].Properties3 == 0) Karakter.Stat.AttackPower -= Data.SkillBase[Karakter.Action.Buff.SkillID[b_index]].Properties1;
                        break;
                    case "SKILL_CH_BOW_NORMAL":
                        Karakter.Stat.EkstraMetre -= (Data.SkillBase[Karakter.Action.Buff.SkillID[b_index]].Properties1);
                        break;
                    case "SKILL_ETC_ARCHEMY_POTION_SPEED_":
                        Karakter.Speed.Updateded -= Data.SkillBase[Karakter.Speed.DefaultSpeedSkill].Properties2;
                        Player_SetNewSpeed();
                        Send(Public.Packet.SetSpeed(Karakter.Information.UniqueID, Karakter.Speed.WalkSpeed, Karakter.Speed.RunSpeed));
                        break;
                }
            }
            catch (Exception ex)
            {
                deBug.Write(ex);
            }
        }
        #endregion

        #region Regular Attack
        void ActionNormalAttack()
        {
            try
            {
                float x = 0, y = 0;
                bool[] aRound = null;
                if (!Karakter.Action.nAttack) return;
                if (Karakter.Action.Object != null && Karakter.Action.Object.GetType().ToString() == "Game.obj")
                {
                    obj o = Karakter.Action.Object as obj;
                    if (o.State == 4 || o.LocalType != 1) { Karakter.Action.nAttack = false; StopAttackTimer(); return; }
                    if (o.Agro == null) o.Agro = new List<_agro>();
                    x = (float)o.x;
                    y = (float)o.y;
                    aRound = o.aRound;
                    if (!o.Attacking)
                        o.AddAgroDmg(Karakter.Information.UniqueID, 1);
                    if (o.Die || o.GetDie) { Karakter.Action.nAttack = false; StopAttackTimer(); return; }
                }

                if (Karakter.Action.Object != null && Karakter.Action.Object.GetType().ToString() == "Game.Systems")
                {
                    if (!Karakter.Information.PvP) { Karakter.Action.nAttack = false;  StopAttackTimer(); return; }
                    Systems sys = Karakter.Action.Object as Systems;
                    if (sys.Karakter.State.LastState == 4) { Karakter.Action.nAttack = false; StopAttackTimer(); return; }
                    if (!(Karakter.Information.PvP && sys.Karakter.Information.PvP)) { Karakter.Action.nAttack = false; StopAttackTimer(); return; }

                    x = sys.Karakter.Position.x;
                    y = sys.Karakter.Position.y;
                    aRound = sys.Karakter.aRound;
                }

                if (x == 0 && y == 0 && aRound == null) { Karakter.Action.nAttack = false;  StopAttackTimer(); return; }
                //else { StopAttackTimer(); return; }

                double distance = Function.Formule.gamedistance(Karakter.Position.x,
                        Karakter.Position.y,
                        x,
                        y);

                distance -= Data.ItemBase[Karakter.Information.Item.wID].ATTACK_DISTANCE;

                if (distance >= 0)
                {
                    float farkx = x;
                    float farky = y;

                    if (Data.ItemBase[Karakter.Information.Item.wID].ATTACK_DISTANCE <= 10)
                    {
                        if(Systems.aRound(ref aRound, ref farkx, ref farky))
                        {
                            /*StopAttackTimer();
                            return;*/
                            Systems.aRound(ref farkx, ref farky, 1);
                        }
                    }

                    Karakter.Position.wX = farkx - Karakter.Position.x; //- Data.ItemBase[Karakter.Information.Item.wID].ATTACK_DISTANCE;
                    Karakter.Position.wY = farky - Karakter.Position.y; //- Data.ItemBase[Karakter.Information.Item.wID].ATTACK_DISTANCE;
                    Karakter.Position.kX = Karakter.Position.wX;
                    Karakter.Position.kY = Karakter.Position.wY;

                    Send(Public.Packet.Movement(new Game.Global.vektor(Karakter.Information.UniqueID,
                                (float)Function.Formule.packetx((float)farkx, Karakter.Position.xSec),
                                (float)Karakter.Position.z,
                                (float)Function.Formule.packety((float)farky, Karakter.Position.ySec),
                                Karakter.Position.xSec,
                                Karakter.Position.ySec)));


                    distance += Data.ItemBase[Karakter.Information.Item.wID].ATTACK_DISTANCE;

                    Karakter.Position.Time = (distance / Karakter.Speed.RunSpeed) * 10000;
                    Karakter.Position.RecordedTime = Karakter.Position.Time;

                    Karakter.Position.packetxSec = Karakter.Position.xSec;
                    Karakter.Position.packetySec = Karakter.Position.ySec;

                    Karakter.Position.packetX = (ushort)Function.Formule.packetx((float)farkx, Karakter.Position.xSec);
                    Karakter.Position.packetY = (ushort)Function.Formule.packety((float)farky, Karakter.Position.ySec);

                    Karakter.Position.Walking = true;

                    StartMovementTimer((int)(Karakter.Position.Time * 0.1));
                    return;
                }

                ActionAttack();
            }
            catch (Exception ex)
            {
                Console.WriteLine("ActionNormalAttack()::error...");
                deBug.Write(ex);
            }
        }
        void ActionAttack()
        {
            try
            {
                client.Send(Private.Packet.ActionState(1, 1));

                #region Arrow for bow
                if (Data.ItemBase[Karakter.Information.Item.wID].Class_C == 6)
                {
                    if (Karakter.Information.Item.sID == 0) //arrow yoksa slota
                    {
                        if (!ItemCheckArrow()) //inventorydeki arrowlar kontrol et 
                        {
                            Karakter.Action.nAttack = false;
                            client.Send(Public.Packet.ActionPacket(2, 0x0e)); //hic arrow yoksa hata ver
                            StopAttackTimer();
                            return;
                        }
                    }
                    else  //arrow varsa slotta
                    {
                        Karakter.Information.Item.sAmount--;
                        client.Send(Private.Packet.Arrow(Karakter.Information.Item.sAmount));

                        if (Karakter.Information.Item.sAmount <= 0) // arrow bitti
                        {
                            Karakter.Information.Item.sID = 0;
                            MsSQL.UpdateData("delete from char_items where itemnumber='item" + 7 + "' AND owner='" + Karakter.Information.CharacterID + "'");
                            if (!ItemCheckArrow()) //inventorydeki arrowlar kontrol et 
                            {
                                Karakter.Action.nAttack = false;
                                client.Send(Public.Packet.ActionPacket(2, 0x0e)); //hic arrow yoksa hata ver
                                StopAttackTimer();
                                return;
                            }
                        }
                        else
                        {
                            MsSQL.InsertData("UPDATE char_items SET quantity='" + Math.Abs(Karakter.Information.Item.sAmount) + "' WHERE itemnumber='" + "item" + 7 + "' AND owner='" + Karakter.Information.CharacterID + "' AND itemid='" + Karakter.Information.Item.sID + "'");
                        }
                    }
                }
                #endregion

                byte NumberAttack = 1;
                int AttackType = 1;
                int[] found = new int[3];
                byte numbert = 1;

                int p_dmg = 0;
                byte status = 0, crit = 1;

                targetObject target = new targetObject(Karakter.Action.Object, this);

                if (Karakter.Action.ImbueID != 0 && Data.SkillBase[Karakter.Action.ImbueID].Series.Remove(Data.SkillBase[Karakter.Action.ImbueID].Series.Length - 2) == "SKILL_CH_LIGHTNING_GIGONGTA")
                {
                    numbert = ActionGetObject(ref found, 2, target.x, target.y, Karakter.Action.Target, 5);
                }
                else found[1] = Karakter.Action.Target;

                switch (Data.ItemBase[Karakter.Information.Item.wID].Class_C)
                {
                    case 0: break;
                    case 2:
                    case 3:
                        NumberAttack = 2;
                        AttackType = 2;
                        break;
                    case 4:
                    case 5:
                        AttackType = 40;
                        break;
                    case 6:
                        AttackType = 70;
                        break;
                }

                PacketWriter Writer = new PacketWriter();
                Writer.Create(Systems.SERVER_ACTION_DATA);
                Writer.Byte(1);
                Writer.Byte(2);
                Writer.Byte(0x30);

                Karakter.Action.AttackingID = Karakter.Ids.GetCastingID();

                Writer.DWord(AttackType);
                Writer.DWord(Karakter.Information.UniqueID);
                Writer.DWord(Karakter.Action.AttackingID);

                Writer.DWord(Karakter.Action.Target);

                Writer.Bool(true);
                Writer.Byte(NumberAttack);
                Writer.Byte(numbert);

                for (byte t = 1; t <= numbert; t++)
                {
                    Writer.DWord(found[t]);

                    for (byte n = 1; n <= NumberAttack; n++)
                    {
                        p_dmg = 0;
                        status = 0;
                        crit = 1;

                        if (t == 2) //for light skill
                        {
                            target = new targetObject(GetObjects(found[t]), this);
                            if (Karakter.Action.ImbueID != 0)
                            {
                                p_dmg = (int)Function.Formule.gamedamage((Data.SkillBase[Karakter.Action.ImbueID].Properties4), MasteryGetPower(Karakter.Action.ImbueID), 0, 0, Karakter.Information.Mag_Balance, Karakter.Stat.UpdatededMagAttack);

                                p_dmg += rnd.Next(0, p_dmg.ToString().Length);
                            }
                            else p_dmg = 1;

                            if (status != 128)
                                status = target.HP((int)p_dmg);
                            else target.GetDead();
                        }
                        else if (t == 1)
                        {
                            p_dmg = (int)Function.Formule.gamedamage(Karakter.Stat.MaxPhyAttack, Karakter.Stat.AttackPower + MasteryGetPower(AttackType), 0, (double)target.PhyDef, Karakter.Information.Phy_Balance, Karakter.Stat.UpdatededPhyAttack);
                            if (Karakter.Action.ImbueID != 0) p_dmg += (int)Function.Formule.gamedamage((Karakter.Stat.MinMagAttack + Data.SkillBase[Karakter.Action.ImbueID].Properties4), MasteryGetPower(Karakter.Action.ImbueID), 0, target.MagDef, Karakter.Information.Mag_Balance, Karakter.Stat.UpdatededMagAttack);

                            p_dmg /= NumberAttack;

                            if (rnd.Next(15) <= 5)
                            {
                                p_dmg *= 2;
                                crit = 2;
                            }

                            if (Karakter.Information.Berserking)
                                p_dmg = (p_dmg * Karakter.Information.BerserkOran) / 100;

                            if (p_dmg <= 0)
                                p_dmg = 1;
                            else
                            {
                                if (target.mAbsorb() > 0)
                                {
                                    int static_dmg = (p_dmg * (100 - (int)target.mAbsorb())) / 100;
                                    target.MP((static_dmg));
                                    p_dmg = static_dmg;
                                }
                                p_dmg += rnd.Next(0, p_dmg.ToString().Length);
                            }

                            if (status != 128)
                            {
                                status = target.HP((int)p_dmg);
                            }
                            else target.GetDead();
                        }


                        Writer.Byte(status);
                        Writer.Byte(crit);
                        Writer.DWord(p_dmg);
                        Writer.Byte(0);
                        Writer.Word(0);
                    }
                }

                //Print.Format("Attack Pack {0}", Decode.StringToPack(buff));
                Send(Writer.GetBytes());

                //Send(Private.Packet.StatePack(Karakter.Information.UniqueID, 8, 1));
                //client.Send(Public.Packet.Player_getExp(this.Karakter.Information.UniqueID, 100, 50, 0));
                client.Send(Private.Packet.ActionState(2, 0));
            }
            catch (Exception ex)
            {
                Console.WriteLine("ActionAttack()::error...");
                deBug.Write(ex);
            }
        }
        byte ActionGetObject(ref int[] found, byte max, float ox , float oy, int objectid, byte metre)
        {
            byte founded = 1;
            found[1] = Karakter.Action.Target;
            try
            {
                float x = (float)ox - metre;
                float y = (float)oy - metre;
                for (int i = 0; i <= Systems.Objects.Count - 1; i++)
                {
                    if (founded == max) return founded;
                    obj o = Systems.Objects[i];
                    if (!o.Die && o.LocalType == 1)
                    {
                        if (o.x >= x && o.x <= (x + (metre * 2)) && o.y >= y && o.y <= (y + (metre * 2)) && o.UniqueID != objectid)
                        {
                            founded++;
                            if (o.Agro == null) o.Agro = new List<_agro>();
                            found[founded] = o.UniqueID;
                        }
                    }
                }
                for (int i = 0; i <= Systems.clients.Count - 1; i++)
                {
                    if (founded == max) return founded;
                    Systems sys = Systems.clients[i];
                    if (sys.Karakter.Information.PvP)
                        if (sys.Karakter.Information.UniqueID != objectid && Karakter.Information.UniqueID != objectid && Systems.clients[i].Karakter.Information.UniqueID != this.Karakter.Information.UniqueID)
                        {
                            if (sys.Karakter.Position.x >= x && sys.Karakter.Position.x <= (x + (metre * 2)) && sys.Karakter.Position.y >= y && sys.Karakter.Position.y <= (y + (metre * 2)))
                            {
                                founded++;
                                found[founded] = sys.Karakter.Information.UniqueID;
                            }
                        }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ActionGetObject()::error...");
                deBug.Write(ex);
            }
                return founded;

        }
        #endregion

        #region Skills
        void ActionSkillAttack()
        {
            try
            {
                float x = 0, y = 0;
                bool[] aRound = null;
                if (!Karakter.Action.sAttack) return;
                obj o = null;
                if (Karakter.Action.Object != null && Karakter.Action.Object.GetType().ToString() == "Game.obj")
                {
                    o = Karakter.Action.Object as obj;
                    if (o.Die || o.GetDie || o.LocalType != 1) { Karakter.Action.sAttack = false; StopSkillTimer(); return; }
                    if (o.Agro == null) o.Agro = new List<_agro>();
                    x = (float)o.x;
                    y = (float)o.y;
                    aRound = o.aRound;
                    if (!o.Attacking)
                        o.AddAgroDmg(Karakter.Information.UniqueID, 1);

                }
                //else { StopAttackTimer(); return; }
                if (o == null && Karakter.Action.Object != null && Karakter.Action.Object.GetType().ToString() == "Game.Systems")
                {
                    Systems sys = Karakter.Action.Object as Systems;
                    if (!Karakter.Information.PvP || sys.Karakter.State.Die) { Karakter.Action.sAttack = false; StopSkillTimer(); return; }
                    if (!(Karakter.Information.PvP && sys.Karakter.Information.PvP)) { Karakter.Action.sAttack = false; StopSkillTimer(); return; }
                    x = sys.Karakter.Position.x;
                    y = sys.Karakter.Position.y;
                    aRound = sys.Karakter.aRound;
                }
                //else { StopAttackTimer(); return; }

                if (x == 0 && y == 0 && aRound == null) { Karakter.Action.sAttack = false; StopSkillTimer(); return; }

                double distance = Function.Formule.gamedistance(Karakter.Position.x,
                        Karakter.Position.y,
                        x,
                        y);

                if (Karakter.Action.Skill.Uzak == 0)
                    distance -= Data.ItemBase[Karakter.Information.Item.wID].ATTACK_DISTANCE;
                else distance -= Karakter.Action.Skill.Uzak;

                if (distance >= 0)
                {
                    float farkx = x;
                    float farky = y;

                    if (Karakter.Action.Skill.Uzak == 0)
                    {
                        if (Data.ItemBase[Karakter.Information.Item.wID].ATTACK_DISTANCE <= 10) 
                            if (Systems.aRound(ref aRound, ref farkx, ref farky))
                            { Systems.aRound(ref farkx, ref farky, 1); }
                    }

                    Karakter.Position.wX = farkx - Karakter.Position.x; //- Data.ItemBase[Karakter.Information.Item.wID].ATTACK_DISTANCE;
                    Karakter.Position.wY = farky - Karakter.Position.y; //- Data.ItemBase[Karakter.Information.Item.wID].ATTACK_DISTANCE;
                    Karakter.Position.kX = Karakter.Position.wX;
                    Karakter.Position.kY = Karakter.Position.wY;

                    Send(Public.Packet.Movement(new Game.Global.vektor(Karakter.Information.UniqueID,
                                (float)Function.Formule.packetx((float)farkx, Karakter.Position.xSec),
                                (float)Karakter.Position.z,
                                (float)Function.Formule.packety((float)farky, Karakter.Position.ySec),
                                Karakter.Position.xSec,
                                Karakter.Position.ySec)));

                    distance += Data.ItemBase[Karakter.Information.Item.wID].ATTACK_DISTANCE;
                    Karakter.Position.Time = (distance / Karakter.Speed.RunSpeed) * 10000;
                    Karakter.Position.RecordedTime = Karakter.Position.Time;

                    Karakter.Position.packetxSec = Karakter.Position.xSec;
                    Karakter.Position.packetySec = Karakter.Position.ySec;

                    Karakter.Position.packetX = (ushort)Function.Formule.packetx((float)farkx, Karakter.Position.xSec);
                    Karakter.Position.packetY = (ushort)Function.Formule.packety((float)farky, Karakter.Position.ySec);


                    Karakter.Position.Walking = true;
                    StartMovementTimer((int)(Karakter.Position.Time * 0.1));
                    return;
                }

                StartSkill();
            }
            catch (Exception ex)
            {
                Console.WriteLine("ActionSkillAttack()::error..");
                deBug.Write(ex);
            }
        }
        void StartSkill() 
        {
            try
            {
                if (!Karakter.Action.sAttack) return;

                #region Arrow for bow
                if (!Karakter.Action.Skill.P_M)
                {
                    if (Data.ItemBase[Karakter.Information.Item.wID].Class_C == 6)
                    {
                        if (Karakter.Information.Item.sID == 0) //arrow yoksa slota
                        {
                            if (!ItemCheckArrow()) //inventorydeki arrowlar kontrol et 
                            {
                                Karakter.Action.sAttack = false;
                                client.Send(Public.Packet.ActionPacket(2, 0x0e)); //hic arrow yoksa hata ver
                                StopSkillTimer();
                                return;
                            }
                        }
                        else  //arrow varsa slotta
                        {
                            Karakter.Information.Item.sAmount--;
                            client.Send(Private.Packet.Arrow(Karakter.Information.Item.sAmount));

                            if (Karakter.Information.Item.sAmount <= 0) // arrow bitti
                            {
                                Karakter.Information.Item.sID = 0;
                                MsSQL.UpdateData("delete from char_items where itemnumber='item" + 7 + "' AND owner='" + Karakter.Information.CharacterID + "'");
                                if (!ItemCheckArrow()) //inventorydeki arrowlar kontrol et 
                                {
                                    Karakter.Action.sAttack = false;
                                    client.Send(Public.Packet.ActionPacket(2, 0x0e)); //hic arrow yoksa hata ver
                                    StopSkillTimer();
                                    return;
                                }
                            }
                            else
                            {
                                MsSQL.InsertData("UPDATE char_items SET quantity='" + Math.Abs(Karakter.Information.Item.sAmount) + "' WHERE itemnumber='" + "item" + 7 + "' AND owner='" + Karakter.Information.CharacterID + "' AND itemid='" + Karakter.Information.Item.sID + "'");
                            }

                        }
                    }
                }
                #endregion

                if (Karakter.Stat.SecondMP < Data.SkillBase[Karakter.Action.Skill.MainSkill].Mana) { Karakter.Action.sAttack = false; StopSkillTimer(); client.Send(Public.Packet.ActionPacket(2, 4)); return; }
                else
                {
                    Karakter.Stat.SecondMP -= Data.SkillBase[Karakter.Action.Skill.MainSkill].Mana;

                    if (Karakter.Stat.SecondMP < 0) Karakter.Stat.SecondMP = 1;
                    UpdateMp();

                    Karakter.Action.Skill.MainCasting = Karakter.Ids.GetCastingID();
                    List<int> lis = Karakter.Spawn;
                    Send(lis, Public.Packet.ActionPacket(1, 0, Karakter.Action.Skill.MainSkill, Karakter.Information.UniqueID, Karakter.Action.Skill.MainCasting, Karakter.Action.Target));
                    

                    if (Karakter.Action.Skill.Instant == 0) MainSkill(lis);
                    else
                        StartSkillCastingTimer(Karakter.Action.Skill.Instant * 1000, lis);
                }
            }
            catch (Exception ex)
            {
                deBug.Write(ex);
            }
        }
        void MainSkill(List<int> list)
        {
            if (!Karakter.Action.sAttack) return;

            SekmeKontrolu();

            int[,] p_dmg = new int[Karakter.Action.Skill.Found, Karakter.Action.Skill.NumberOfAttack];
            int[] statichp = new int[Karakter.Action.Skill.Found];
            
            try
            {
                PacketWriter Writer = new PacketWriter();
                //01 A7240200 17004001 01 01 01 17004001 00 01 84050000 00 0000
                //01 36AE0600 CF752200 01 01 01 CF752200 04 02 EC000000000000 A960 2004 0000 0A000 00 086050000

                Writer.Create(Systems.SERVER_SKILL_DATA);
                Writer.Byte(1);
                Writer.DWord(Karakter.Action.Skill.MainCasting);
                Writer.DWord(Karakter.Action.Target);
                Writer.Byte(1);
                Writer.Byte(Karakter.Action.Skill.NumberOfAttack);
                Writer.Byte(Karakter.Action.Skill.Found);
                byte[] status;
                status = new byte[Karakter.Action.Skill.Found];
                targetObject[] target = new targetObject[Karakter.Action.Skill.Found];
                for (byte f = 0; f < Karakter.Action.Skill.Found; f++)
                {
                    if (Karakter.Action.Skill.FoundID[f] != 0)
                    {
                        Writer.DWord(Karakter.Action.Skill.FoundID[f]);
                        target[f] = new targetObject(GetObjects(Karakter.Action.Skill.FoundID[f]), this);
                        if (target[f].sys == null && target[f].os == null) { }
                        else
                        {
                            statichp[f] = target[f].GetHp;
                            for (byte n = 0; n < Karakter.Action.Skill.NumberOfAttack; n++)
                            {
                                bool block = false;
                                /*if (Karakter.Information.Item.sID != 0 && Karakter.Information.Item.sID != 62)
                                {
                                    if (Global.RandomID.GetRandom(0, 25) < 10) block = true;
                                }*/
                                if (!block)
                                {
                                    byte crit = 1;
                                    p_dmg[f, n] = 1;

                                    if (Karakter.Action.Skill.P_M) // for magic damage
                                    {
                                        p_dmg[f, n] = (int)Function.Formule.gamedamage((Karakter.Stat.MaxMagAttack + Data.SkillBase[Karakter.Action.Skill.SkillID[n]].Properties2), MasteryGetPower(Karakter.Action.Skill.SkillID[n]), target[f].AbsrobMag, target[f].MagDef, this.Karakter.Information.Mag_Balance, (Data.SkillBase[Karakter.Action.Skill.SkillID[n]].Per + Karakter.Stat.UpdatededMagAttack));
                                        if (Karakter.Action.ImbueID != 0) p_dmg[f, n] += (int)Function.Formule.gamedamage((Karakter.Stat.MinMagAttack + Data.SkillBase[Karakter.Action.ImbueID].Properties4), MasteryGetPower(Karakter.Action.ImbueID), 0, target[f].MagDef, Karakter.Information.Mag_Balance, Karakter.Stat.UpdatededMagAttack);
                                    }
                                    else // for phy damage
                                    {
                                        p_dmg[f, n] = (int)Function.Formule.gamedamage((Karakter.Stat.MaxPhyAttack + Data.SkillBase[Karakter.Action.Skill.SkillID[n]].Properties2), MasteryGetPower(Karakter.Action.Skill.SkillID[n]), target[f].AbsrobPhy, target[f].PhyDef, this.Karakter.Information.Phy_Balance, Karakter.Stat.UpdatededPhyAttack + Data.SkillBase[Karakter.Action.Skill.SkillID[n]].Per);
                                        if (Karakter.Action.ImbueID != 0) p_dmg[f, n] += (int)Function.Formule.gamedamage((Karakter.Stat.MinMagAttack + Data.SkillBase[Karakter.Action.ImbueID].Properties4), MasteryGetPower(Karakter.Action.ImbueID), 0, target[f].MagDef, Karakter.Information.Mag_Balance, Karakter.Stat.UpdatededMagAttack);
                                        if (rnd.Next(16) < 5)
                                        {
                                            p_dmg[f, n] *= 2;
                                            crit = 2;
                                        }
                                    }

                                    if (f > 0) p_dmg[f, n] = (p_dmg[f, n] * (100 - (f * 10))) / 100;

                                    if (Karakter.Information.Berserking)
                                        p_dmg[f, n] = (p_dmg[f, n] * Karakter.Information.BerserkOran) / 100;

                                    if (p_dmg[f, n] <= 0)
                                        p_dmg[f, n] = 1;
                                    else
                                    {
                                        if (target[f].mAbsorb() > 0)
                                        {
                                            int static_dmg = (p_dmg[f, n] * (100 - (int)target[f].mAbsorb())) / 100;
                                            target[f].MP(static_dmg);
                                            p_dmg[f, n] = static_dmg;
                                        }
                                        p_dmg[f, n] += rnd.Next(0, p_dmg.ToString().Length);
                                    }


                                    statichp[f] -= p_dmg[f, n];
                                    if (statichp[f] < 1)
                                    {
                                        status[f] = 128;
                                        target[f].GetDead();
                                    }


                                    if (Karakter.Action.Skill.OzelEffect == 4 && status[f] != 128)
                                    {
                                        if (rnd.Next(10) > 5)
                                            status[f] = 4;
                                    }
                                    //CF752200 04 02 EC000000000000 A960 2004 0000 0A00 0000 8605 0000
                                    Writer.Byte(status[f]);
                                    Writer.Byte(crit);
                                    Writer.DWord(p_dmg[f, n]);
                                    Writer.Byte(0);
                                    Writer.Word(0);

                                    if (status[f] == 4)
                                    {
                                        Writer.Byte(target[f].xSec);
                                        Writer.Byte(target[f].ySec);
                                        Writer.Word(Function.Formule.packetx(target[f].x + 0.1f, target[f].xSec));
                                        Writer.Word(0);
                                        Writer.Word(Function.Formule.packety(target[f].y - 0.1f, target[f].ySec));
                                        Writer.Word(0);
                                        Writer.Word(0);
                                        Writer.Word(0);
                                    }

                                }
                                else Writer.Byte(2);
                            }
                        }

                    }
                }
                Send(list, Writer.GetBytes());

                Karakter.Action.sCasting = true;
                Karakter.Action.sAttack = false;
                StartsWaitTimer(Data.SkillBase[Karakter.Action.Skill.MainSkill].CastingTime, target, p_dmg, status);
            }
            catch (Exception ex)
            {
                Console.WriteLine("MainSkill()::eror...");
                deBug.Write(ex);
            }
        }
        void SekmeKontrolu()
        {
            try
            {
                Karakter.Action.Skill.FoundID[Karakter.Action.Skill.Found] = Karakter.Action.Target;
                Karakter.Action.Skill.Found++;
                if (Karakter.Action.Skill.Sekme > 1)
                {
                    object mainObject = GetObjects(Karakter.Action.Target);
                    if (mainObject == null) return;
                    targetObject target = new targetObject(mainObject, this);
                    float x = target.x - Karakter.Action.Skill.SekmeMetre;
                    float y = target.y - Karakter.Action.Skill.SekmeMetre;
                    for (int i = 0; i < Systems.Objects.Count; i++)
                    {
                        if (Karakter.Action.Skill.Found == Karakter.Action.Skill.Sekme) return;
                        if (Systems.Objects[i] != null)
                        {
                            obj o = Systems.Objects[i];
                            if (!o.Die && o.LocalType == 1)
                            {
                                if (o.x >= x && o.x <= (x + (Karakter.Action.Skill.SekmeMetre * 2)) && o.y >= y - Karakter.Action.Skill.SekmeMetre && o.y <= (y + (Karakter.Action.Skill.SekmeMetre * 2)) && o.UniqueID != Karakter.Action.Target)
                                {
                                    if (o.Agro == null) o.Agro = new List<_agro>();
                                    Karakter.Action.Skill.FoundID[Karakter.Action.Skill.Found] = o.UniqueID;
                                    Karakter.Action.Skill.Found++;
                                }
                            }
                        }
                    }
                    for (int i = 0; i <= Systems.clients.Count - 1; i++)
                    {
                        if (Karakter.Action.Skill.Found == Karakter.Action.Skill.Sekme) return;
                        if (Systems.clients[i] != null)
                        {
                            Systems sys = Systems.clients[i];
                            if (sys.Karakter.Information.PvP && sys != this && !sys.Karakter.State.Die)
                                if (sys.Karakter.Information.UniqueID != Karakter.Action.Target && Karakter.Information.UniqueID != Karakter.Action.Target)
                                {
                                    if (sys.Karakter.Position.x >= x && sys.Karakter.Position.x <= (x + (Karakter.Action.Skill.SekmeMetre * 2)) && sys.Karakter.Position.y >= y && sys.Karakter.Position.y <= (y + (Karakter.Action.Skill.SekmeMetre * 2)))
                                    {
                                        Karakter.Action.Skill.FoundID[Karakter.Action.Skill.Found] = sys.Karakter.Information.UniqueID;
                                        Karakter.Action.Skill.Found++;
                                    }
                                }
                        }
                    }
                    target = null;
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("SekmeKontrolu()::eror...");
                deBug.Write(ex);
            }
        }
        #endregion

        #region Moving Skill
        void MovementSkill(PacketReader Reader)
        {
            if (Karakter.Action.sAttack || Karakter.Action.sCasting) return;

            if (Karakter.Stat.SecondMP < Data.SkillBase[Karakter.Action.UsingSkillID].Mana) { client.Send(Public.Packet.ActionPacket(2, 4)); return; }
            else
            {
                Karakter.Stat.SecondMP -= Data.SkillBase[Karakter.Action.UsingSkillID].Mana;
                UpdateMp();
                if (Timer.Movement != null) { Timer.Movement.Dispose(); Karakter.Position.Walking = false; }

                byte xSec = Reader.Byte(), ySec = Reader.Byte();
                int x = Reader.Int32(), z = Reader.Int32(), y = Reader.Int32();
                Reader.Close();

                float gamex = Game.Function.Formule.gamex((float)x, xSec);
                float gamey = Game.Function.Formule.gamey((float)y, ySec);

                float farkx = gamex - Karakter.Position.x;
                float farky = gamey - Karakter.Position.y;

                float hesapy = 0, hesapx = 0;

                while (hesapx + hesapy < Data.SkillBase[Karakter.Action.UsingSkillID].Per / 10)
                {
                    Karakter.Position.x += (farkx / 30);
                    Karakter.Position.y += (farky / 30);
                    hesapx += Math.Abs((farkx / 30));
                    hesapy += Math.Abs((farky / 30));
                }

                PacketWriter Writer = new PacketWriter();

                Writer.Create(SERVER_ACTION_DATA);
                Writer.Byte(1);
                Writer.Byte(2);
                Writer.Byte(0x30);
                int overid = Karakter.Ids.GetCastingID();
                Writer.DWord(Karakter.Action.UsingSkillID);//skillid
                Writer.DWord(Karakter.Information.UniqueID); //charid
                Writer.DWord(overid);//overid
                Writer.DWord(0);
                Writer.Byte(8);
                Writer.Byte(xSec);
                Writer.Byte(ySec);
                Writer.DWord(Function.Formule.packetx(Karakter.Position.x, xSec));
                Writer.DWord(0);
                Writer.DWord(Function.Formule.packety(Karakter.Position.y, ySec));

                Send(Writer.GetBytes());

                client.Send(Private.Packet.ActionState(2, 0));

                ObjectSpawnCheck();


               /* Writer.Create(0xB481); //open berserk help window
                //0100 0100 0000 0000 00
                Writer.Word(1);
                Writer.DWord(1);
                Writer.Word(0);
                Writer.Byte(0);
                client.Send(Writer.GetBytes());*/
            }
        #endregion

        }
    }
}
