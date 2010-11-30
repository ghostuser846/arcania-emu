
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System;
namespace Game
{
    public partial class Systems
    {
        public _time Timer;

        public struct _time
        {
            public Timer Waiting;
            public Timer Movement;
            public Timer Attack;
            public Timer[] Buff;
            public Timer Casting;
            public Timer SkillCasting;
            public Timer[] Potion;
            public Timer sWait;
            public Timer Berserker;
            public Timer Scroll;
        }
        void OpenTimer()
        {
            Timer.Buff = new Timer[20];
            Timer.Potion = new Timer[20];
        }
        void Player_Wait_CallBack(object e)
        {
            if (Karakter.Information.Quit) { client.Send(Private.Packet.EndLeaveGame()); Disconnect(); }
            Timer.Waiting.Dispose();
        }
        void Player_sWait_CallBack(object e)
        {
            try
            {
                object[] es = (object[])e;
                targetObject[] target = (targetObject[])es[0];
                int[,] p_dmg = (int[,])es[1];
                byte[] staticstatus = (byte[])es[2];

                for (byte f = 0; f < Karakter.Action.Skill.Found; f++)
                {
                    if (staticstatus[f] == 4)
                        target[f].Sleep(4);

                    target[f].HP(p_dmg[f, Karakter.Action.sSira]);
                    if (Karakter.Action.sSira + 1 == Karakter.Action.Skill.NumberOfAttack)
                    {
                        target[f].Dispose();
                    }
                }
                Karakter.Action.sSira++;
                if (Karakter.Action.sSira == Karakter.Action.Skill.NumberOfAttack)
                {

                    StopSkillTimer();
                }
                else
                {
                    if (Karakter.Action.Skill.NumberOfAttack != 1) StartsWaitTimer(Data.SkillBase[Karakter.Action.Skill.SkillID[Karakter.Action.sSira - 1]].CastingTime, target, p_dmg, staticstatus);
                }
            }
            catch (Exception ex)
            {
                StopSkillTimer();
                deBug.Write(ex);
            }
        }
        public void StopSkillTimer()
        {
            try
            {
                Karakter.Action.sSira = 0;
                Karakter.Action.sAttack = false;
                Karakter.Action.sCasting = false;
                if (Timer.sWait != null) Timer.sWait.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine("StopSkillTimer()...error");
                deBug.Write(ex);
            }
        }

        void Player_Attack_CallBack(object e)
        {
            if (Timer.Attack != null)
            {
                ActionNormalAttack();
            }
        }
        void Player_Movement(object e)
        {
            try
            {
                if (Karakter.Position.RecordedTime <= 0) // bittir
                {
                    Karakter.aRound = new bool[10];

                    Timer.Movement.Dispose();
                    Karakter.Position.Walking = false;
                    if (Karakter.Action.PickUping) Player_PickUpItem();
                    Karakter.Position.z = Karakter.Position.wZ;
                    this.ObjectSpawnCheck();
                }
                else
                {
                    if (Karakter.Action.nAttack)
                    {
                        Karakter.Position.kX -= (Karakter.Position.wX * 10) / 100;
                        Karakter.Position.kY -= (Karakter.Position.wY * 10) / 100;
                        if (Math.Sqrt(Karakter.Position.kX * Karakter.Position.kX + Karakter.Position.kY * Karakter.Position.kY) < Data.ItemBase[Karakter.Information.Item.wID].ATTACK_DISTANCE)
                        {
                            Karakter.Position.RecordedTime = 0;
                            Karakter.aRound = new bool[10];
                            if (Karakter.Action.nAttack) ActionAttack();
                            this.ObjectSpawnCheck();
                            Timer.Movement.Dispose();
                            Karakter.Position.z = Karakter.Position.wZ;
                            Karakter.Action.PickUping = false;
                            Karakter.Position.Walking = false;
                            return;
                        }
                    }
                    else if (Karakter.Action.sAttack)
                    {
                        Karakter.Position.kX -= (Karakter.Position.wX * 10) / 100;
                        Karakter.Position.kY -= (Karakter.Position.wY * 10) / 100;
                        short test = Karakter.Action.Skill.Uzak;
                        if (test == 0)
                            test = Data.ItemBase[Karakter.Information.Item.wID].ATTACK_DISTANCE;
                        if (Math.Sqrt(Karakter.Position.kX * Karakter.Position.kX + Karakter.Position.kY * Karakter.Position.kY) < test)
                        {
                            if (Karakter.Action.sAttack) StartSkill();
                            Karakter.Position.RecordedTime = 0;
                            Karakter.aRound = new bool[10];
                            this.ObjectSpawnCheck();
                            Timer.Movement.Dispose();
                            Karakter.Position.z = Karakter.Position.wZ;
                            Karakter.Action.PickUping = false;
                            Karakter.Position.Walking = false;
                            return;
                        }
                    }

                    Karakter.aRound = new bool[10];
                    Karakter.Position.x += (Karakter.Position.wX * 10) / 100;
                    Karakter.Position.y += (Karakter.Position.wY * 10) / 100;
                    if (Karakter.Transport.Right)
                    {
                        Karakter.Transport.Horse.x = Karakter.Position.x;
                        Karakter.Transport.Horse.y = Karakter.Position.y;
                    }
                    Karakter.Position.RecordedTime -= (Karakter.Position.Time * 0.1);
                    this.ObjectSpawnCheck();
                    this.ObjectAttackCheck();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("movement call back error..");
                deBug.Write(ex);
            }
        }
        void Player_Buff_CallBack(object e)
        {
            try
            {
                SkillBuffEnd((byte)e);
                Timer.Buff[(byte)e].Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine("buff callback error");
                deBug.Write(ex);
            }
        }
        void Player_Scroll_CallBack(object e)
        {
            try
            {
                if (Karakter.Information.Scroll == false) return;

                Karakter.InGame = false;

                BuffAllClose();

                DeSpawnMe();
                ObjectDeSpawn();

                client.Send(Private.Packet.TeleportOtherStart());

                Teleport_UpdateXYZ(Karakter.Information.Place);
                client.Send(Private.Packet.TeleportImage(Data.PointBase[Karakter.Information.Place].xSec, Data.PointBase[Karakter.Information.Place].ySec));
                Karakter.Teleport = true;
                Timer.Scroll.Dispose();
                Timer.Scroll = null;
                Karakter.Information.Scroll = false;
                
            }
            catch (Exception ex)
            {
                Console.WriteLine("scroll timer error");
                deBug.Write(ex);
            }
        }
        void Player_Casting_CallBack(object e)
        {
            if (Karakter.Action.Cast)
            {
                SkillBuffCasting((List<int>)e);
                Karakter.Action.Cast = false;
            }
            Timer.Casting.Dispose();
        }
        void Player_Berserker_CallBack(object e)
        {
            StopBerserkTimer();
        }
        void StopBerserkTimer()
        {
            if (Timer.Berserker != null)
            {
                Player_Berserk_Down();
                Timer.Berserker.Dispose();
                Timer.Berserker = null;
            }
        }
        void Player_SkillCasting_CallBack(object e)
        {
            MainSkill((List<int>)e);
            Timer.SkillCasting.Dispose();
        }
        void StartSkillCastingTimer(int time, object list)
        {
            if (Timer.SkillCasting != null) Timer.SkillCasting.Dispose();
            Timer.SkillCasting = new Timer(new TimerCallback(Player_SkillCasting_CallBack), list, time, 0);
        }
        void StartBerserkerTimer(int time)
        {
            if (Timer.Berserker != null) Timer.Berserker.Dispose();
            Timer.Berserker = new Timer(new TimerCallback(Player_Berserker_CallBack), 0, time, 0);
        }
        void StartWaitingTimer(int time)
        {
            if (Timer.Waiting != null) Timer.Waiting.Dispose();
            Timer.Waiting = new Timer(new TimerCallback(Player_Wait_CallBack), 0, time, 0);
        }
        void StartsWaitTimer(int time, targetObject[] t, int[,] p_dmg, byte[] status)
        {
            if (Timer.sWait != null) { Timer.sWait.Dispose(); }
            Timer.sWait = new Timer(new TimerCallback(Player_sWait_CallBack), new object[] { t, p_dmg, status }, time, 0);
        }
        void StartMovementTimer(int perTime)
        {
            if(Timer.Movement != null)Timer.Movement.Dispose();
            Timer.Movement = new Timer(new TimerCallback(Player_Movement), 0, 0, perTime);
        }
        void StartBuffTimer(int time, byte b_index)
        {
            if (Timer.Buff[b_index] != null) Timer.Buff[b_index].Dispose();
            Timer.Buff[b_index] = new Timer(new TimerCallback(Player_Buff_CallBack), b_index, time, 0);
        }
        void StartCastingTimer(int time, object list)
        {
            if (Timer.Casting != null) Timer.Casting.Dispose();
            Timer.Casting = new Timer(new TimerCallback(Player_Casting_CallBack), list, time, 0);
        }
        void StartAttackTimer(int time)
        {
            if (Timer.Attack != null) Timer.Attack.Dispose();
            Timer.Attack = new Timer(new TimerCallback(Player_Attack_CallBack), 0, 0, time);
        }
        void StartScrollTimer(int time)
        {
            if (Timer.Scroll != null) Timer.Scroll.Dispose();
            Timer.Scroll = new Timer(new TimerCallback(Player_Scroll_CallBack), 0, time, 0);
        }
        void StopScrollTimer()
        {
            try
            {
                if(Timer.Scroll != null)
                    Timer.Scroll.Dispose();

                Timer.Scroll = null;

                Send(Private.Packet.StatePack(Karakter.Information.UniqueID, 0x0B, 0x00, false));
                Karakter.Information.Scroll = false;
            }
            catch (Exception ex)
            {
                deBug.Write(ex);
                Console.WriteLine("stop scroll timer");
            }

        }
        public void StopAttackTimer()
        {
            try
            {
                if (Timer.Attack != null)
                {
                    client.Send(Private.Packet.ActionState(2, 0));
                    Timer.Attack.Dispose();
                    Karakter.Action.nAttack = false;
                    Karakter.Action.Object = null;
                }
                Timer.Attack = null;
            }
            catch
            {
                Console.WriteLine("Error::StopAttackTimer({0})", Karakter.Information.Name);
            }
        }
        void StartSpeedPotTimer(int time)
        {
            if (Timer.Scroll != null) Timer.Scroll.Dispose();
            Timer.Scroll = new Timer(new TimerCallback(Player_Scroll_CallBack), 0, time, 0);

        }
        void StartPotionTimer(int time, object e, ushort i)
        {
            Timer.Potion[i] = new Timer(new TimerCallback(Player_Potion_CallBack), e, 0, time);
        }
        void Player_Potion_CallBack(object e)
        {
            //prob[0] ne kadar hp dolduracağı HP TÖLTÉS MINDEN 920ms ALATT
            //prob[1] TYPE
            //prob[2] ne kadar doldurdugu PSLOT
            int[] prob = (int[]) e;
            
            if (Karakter.Information.Item.Potion[prob[2]] == 5 || Karakter.State.Die)
            {
                if (Timer.Potion[prob[2]] != null)
                {
                    Karakter.Information.Item.Potion[prob[2]] = 0;
                    Timer.Potion[prob[2]].Dispose();
                    Timer.Potion[prob[2]] = null;
                    prob = null;
                }
                return;
            }
            if (prob[1] == 1)
            {
                Karakter.Information.Item.Potion[prob[2]]++;
                Karakter.Stat.SecondHp += prob[0];
                if (Karakter.Stat.SecondHp > Karakter.Stat.Hp) { Karakter.Stat.SecondHp = Karakter.Stat.Hp; }
                UpdateHp();
            }
            if (prob[1] == 2)
            {
                Karakter.Information.Item.Potion[prob[2]]++;
                Karakter.Stat.SecondMP += prob[0];
                if (Karakter.Stat.SecondMP > Karakter.Stat.Mp) { Karakter.Stat.SecondMP = Karakter.Stat.Mp; }
                UpdateMp();
            }
            if (prob[1] == 3)
            {
                Karakter.Information.Item.Potion[prob[2]]++;
                Karakter.Stat.SecondHp += prob[0];
                if (Karakter.Stat.SecondHp > Karakter.Stat.Hp) { Karakter.Stat.SecondHp = Karakter.Stat.Hp; }
                UpdateHp();
            }
            if (prob[1] == 4)
            {
                Karakter.Information.Item.Potion[prob[2]]++;
                Karakter.Stat.SecondMP += prob[0];
                if (Karakter.Stat.SecondMP > Karakter.Stat.Mp) { Karakter.Stat.SecondMP = Karakter.Stat.Mp; }
                UpdateMp();
            }
            prob = null;
        }
        Timer pTimer;
        void PingTimer()
        {
            if(pTimer == null)
                pTimer = new Timer(new TimerCallback(PingTimerCallBack), 0, 0, 2 * 60000);
        }
        void PingTimerCallBack(object e)
        {
            TimeSpan t = DateTime.Now - lastPing;

            if (t.TotalMinutes > 5)
            {
                Console.WriteLine("PING DC");
                this.Disconnect();
            }
        }
        void PingStop()
        {
            if (pTimer != null)
            {
                pTimer.Dispose();
                pTimer = null;
            }
        }
    }
}
