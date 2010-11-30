using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using Game.Global;
using System.Threading;
using Framework;
namespace Game
{
    public class Data
    {
        /// <summary>
        /// Összes tárgy tömbje.
        /// </summary>
        public static item_database[] ItemBase = new item_database[35000];
        public static objectdata[] ObjectBase = new objectdata[35000];
        public static point[] PointBase = new point[250];
        public static s_data[] SkillBase = new s_data[35000];
        public static short[] MasteryBase = new short[111];
        public static levelgold[] LevelGold = new levelgold[141];
        public static List<shop_data> ShopData = new List<shop_data>(500);
        public static long[] LevelData = new long[141];
    }
    public class player
    {
        public string AccountName, Password;
        public int ID, CreatingCharID;
        public long pGold;
        public int Silk;
        public void Dispose()
        {
            GC.Collect(GC.GetGeneration(this));
        }
    }
    public class character
    {
        public _Information Information;
        public _Stats Stat;
        public _pos Position;
        public _speed Speed;
        public _action Action;
        public _network Network;
        public _alchemy Alchemy;
        public _stall Stall;
        public _state State;
        public _account Account;
        public BuyPack Buy_Pack = new BuyPack();
        public _trans Transport;
        public ID Ids;
        public List<int> Spawn = new List<int>();
        public bool[] aRound = new bool[10];
        public bool InGame, Spawning, deSpawning, Teleport;
        public int LogNum;
        
        public character()
        {
            this.Action.Buff.OverID = new int[20];
            this.Action.Buff.SkillID = new int[20];
            this.Information.Item.Potion = new byte[20];
            this.Action.MonsterID = new List<int>(8);
            this.LogNum = Global.RandomID.GetRandom(50, 100);
        }

        public void Dispose()
        {
            GC.Collect(GC.GetGeneration(this));
        }

        public struct _account
        {
            public long StorageGold;
            public int ID;
        }

        public bool Spawned(int id)
        {
            bool result = Spawn.Exists(
                    delegate(int bk)
                    {
                        return bk == id;
                    }
                    );
            return result;
        }
        public struct _trans
        {
            public bool Right;
            public bool Spawned;
            public pet_obj Horse;
        }
        public struct _Information
        {
            public bool FirstLogin, Handle, Quit, Scroll, Casting, Skill, TuruncuZerk;
            public int Model, SpBar, SkillPoint, CharacterID;
            public int UniqueID;
            public byte BerserkOran;
            public byte Level, HighLevel,BerserkBar, Volume, GM, Place, Title, Phy_Balance, Mag_Balance;
            public short Attributes;
            public string Name;
            public long XP, Gold;
            public bool Berserking, PvP;
            public _item Item;
        }
        public struct _state
        {
            public byte type1, type2;
            public bool Die, Busy;
            public byte LastState, DeadType;
        }
        public struct _item
        {
            public int wID, sID;
            public short sAmount;
            public byte[] Potion;
        }

        public struct _Stats
        {
            public double MinPhyAttack, MaxPhyAttack, MinMagAttack, MaxMagAttack, PhyDef, MagDef, uMagDef, uPhyDef;
            public double Absorb_mp, UpdatededPhyAttack, UpdatededMagAttack, AttackPower, EkstraMetre;
            public short Strength, Intelligence;
            public short phy_Absorb, mag_Absorb;
            public double Hit, Parry;
            public int Hp, Mp, SecondHp, SecondMP;
            public _skill Skill;
        }
        public struct _pos
        {
            public float x, y, z;
            public float wX, wY, wZ, kX, kY;
            public byte packetxSec, packetySec;
            public ushort packetX, packetZ, packetY;
            public byte xSec, ySec, wxSec, wySec;
            public double RecordedTime, Time;
            public bool Walking;
            public ushort Angle;
        }
        public struct _speed
        {
            public float WalkSpeed, RunSpeed, BerserkSpeed, Updateded, DefaultSpeed;
            public int DefaultSpeedSkill;
        }
        public struct _skill
        {
            public int[] Mastery;
            public byte[] Mastery_Level;
            public int[] Skill;
            public int SkillCastingID;
            public int AmountSkill;
        }
        public struct _action
        {
            public int Target, UsingSkillID, CastingSkill, ImbueID, AttackingID;
            public bool Cast, nAttack, PickUping, sAttack, sCasting, Tree;
            public _buff Buff;
            public object Object;
            public _usingSkill Skill;
            public List<int> MonsterID;
            public byte sSira;
            public bool Check()
            {
                if (MonsterID == null) return false;
                if (MonsterID.Count >= 8) return true;
                return false;
            }
            public bool MonsterIDCheck(int id)
            {
                bool result = MonsterID.Exists(
                        delegate(int bk)
                        {
                            return bk == id;
                        }
                        );
                return result;
            }
        }
        public struct _usingSkill
        {
            public int MainSkill, MainCasting;
            public bool P_M; //is magical dmg?
            public byte Uzak /*distance*/, Instant, NumberOfAttack, Sekme/*hány mobot üthet térsebzésből*/, SekmeMetre/*térsebzés range*/, Found, sSira;
            public int[] SkillID;
            public int[] FoundID;
            public bool[] TargetType;
            public byte OzelEffect;
            public bool canUse;
        }
        public struct _buff
        {
            public int[] OverID;
            public int[] SkillID;
            public byte slot, count;
            public short castingtime;
        }
        public struct _network
        {
            public int TargetID;
            public party Party;
            public _exchange Exchange;
        }
        public struct _exchange
        {
            public List<slotItem> ItemList;
            public long Gold;
            public bool Approved;
            public bool Window;
        }
        public struct _stall
        {
            public List<slotItem> ItemList;
            public List<string> StallName;
            public Thread StallThread;
        }
        public struct _alchemy
        {
            public List<slotItem> ItemList;
            public Thread AlchemyThread;
            /*public slotItem AlchemyItem;
            public slotItem Elixir;
            public slotItem LuckyPowder;*/
        }
    }
    public class pet_obj
    {
        public int Model;
        public int UniqueID, OwnerID;
        public double x, z, y;
        public byte xSec, ySec;
        public int Hp;
        public bool Information;
        public ID Ids;
        public float Walk = 45, Run = 95, Zerk = 100;
        public List<int> Spawn = new List<int>();

        public List<int> SpawnMe()
        {
            try
            {
                if (this.Model != 0)
                {
                    for (int i = 0; i < Systems.clients.Count; i++)
                    {
                        if (!Spawned(Systems.clients[i].Karakter.Information.UniqueID))
                        {
                            if (Systems.clients[i].Karakter.Position.x >= (this.x - 50) && Systems.clients[i].Karakter.Position.x <= ((this.x - 50) + 100) && Systems.clients[i].Karakter.Position.y >= (this.y - 50) && Systems.clients[i].Karakter.Position.y <= ((this.y - 50) + 100))
                            {
                                Spawn.Add(Systems.clients[i].Karakter.Information.UniqueID);
                                Systems.clients[i].client.Send(Public.Packet.ObjectSpawn(this));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("worlditem::send error");
                deBug.Write(ex);
            }
            return Spawn;
        }
        public void Send(byte[] buff)
        {
            try
            {
                for (int i = 0; i < Systems.clients.Count; i++)
                {
                    if (Spawned(Systems.clients[i].Karakter.Information.UniqueID))
                    {
                        Systems.clients[i].client.Send(buff);
                    }
                }
            }
            catch (Exception ex)
            {
                deBug.Write(ex);
                Console.WriteLine("pet_obj::send error");
            }
        }
        public void Send(Systems sys, byte[] buff)
        {
            if (Spawned(sys.Karakter.Information.UniqueID))
            {
                sys.client.Send(buff);
            }
        }
        public void DeSpawnMe()
        {
            try
            {
                byte[] buff = Public.Packet.ObjectDeSpawn(this.UniqueID);
                for (int i = 0; i < Systems.clients.Count; i++)
                {
                    if (Spawned(Systems.clients[i].Karakter.Information.UniqueID))
                    {
                        Systems.clients[i].client.Send(buff);
                    }
                }
                Spawn.Clear();
            }
            catch (Exception ex)
            {
                Console.WriteLine("pet_obj::DeSpawnMe error");
                deBug.Write(ex);
            }
            finally
            {
                Global.ID.Delete(this.UniqueID);
                Systems.HelperObject.Remove(this);
                Dispose();
            }
        }
        public void DeSpawnMe(bool t)
        {
            try
            {
                byte[] buff = Public.Packet.ObjectDeSpawn(this.UniqueID);
                for (int i = 0; i < Systems.clients.Count; i++)
                {
                    if (Spawned(Systems.clients[i].Karakter.Information.UniqueID))
                    {
                        Systems.clients[i].client.Send(buff);
                    }
                }
                Spawn.Clear();
            }
            catch (Exception ex)
            {
                Console.WriteLine("worlditem::DeSpawnMe error");
                deBug.Write(ex);
            }
        }
        public void Dispose()
        {
            GC.Collect(GC.GetGeneration(this));
        }
        public bool Spawned(int id)
        {
            bool result = Spawn.Exists(
                    delegate(int bk)
                    {
                        return bk == id;
                    }
                    );
            return result;
        }
    }
    public class world_item
    {
        public int Model, amount = 1;
        public int UniqueID, fromOwner, Owner;
        public double x, z, y;
        public byte xSec, ySec;
        public byte PlusValue, Type, fromType;
        public bool downType;
        public Time timer;
        /// <summary>
        /// Objektum típusa. Lehet: World, Item, Mob.
        /// </summary>
        public ID Ids;
        public List<int> Spawn = new List<int>();

        public void Send(byte[] buff, bool b)
        {
            try
            {
                if (b && this.Model != 0)
                {
                    for (int i = 0; i <= Systems.clients.Count - 1; i++)
                    {
                        if (!Spawned(Systems.clients[i].Karakter.Information.UniqueID))
                        {
                            if (Systems.clients[i].Karakter.Position.x >= (this.x - 50) && Systems.clients[i].Karakter.Position.x <= ((this.x - 50) + 100) && Systems.clients[i].Karakter.Position.y >= (this.y - 50) && Systems.clients[i].Karakter.Position.y <= ((this.y - 50) + 100))
                            {
                                Spawn.Add(Systems.clients[i].Karakter.Information.UniqueID);
                                Systems.clients[i].client.Send(buff);
                            }
                        }
                    }
                    StartDeleteTimer(1 * 60000);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("worlditem::send error");
                deBug.Write(ex);
            }
        }
        public void Send(byte[] buff)
        {
            try
            {
                for (int i = 0; i <= Systems.clients.Count - 1; i++)
                {
                    if (Spawned(Systems.clients[i].Karakter.Information.UniqueID))
                    {
                        Systems.clients[i].client.Send(buff);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("worlditem::send error");
                deBug.Write(ex);
            }
        }
        public void DeSpawnMe()
        {
            try
            {
                byte[] buff = Public.Packet.ObjectDeSpawn(this.UniqueID);
                for (int i = 0; i < Systems.clients.Count; i++)
                {
                    if (Spawned(Systems.clients[i].Karakter.Information.UniqueID))
                    {
                        Systems.clients[i].client.Send(buff);
                    }
                }
                Spawn.Clear();
            }
            catch (Exception ex)
            {
                Console.WriteLine("worlditem::DeSpawnMe error");
                deBug.Write(ex);
            }
        }

        public bool Spawned(int id)
        {
            bool result = Spawn.Exists(
                    delegate(int bk)
                    {
                        return bk == id;
                    }
                    );
            return result;
        }
        public struct Time
        {
            public Timer Delete;
        }
        protected void delete_callback(object e)
        {
            try
            {
                if (this != null)
                {
                    DeSpawnMe();
                    //RandomID.Delete(this.UniqueID);
                    StopDeleteTimer();
                    Dispose();
                    Systems.WorldItem.Remove(this);
                    return;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("world item error..");
                deBug.Write(ex);
            }
        }
        public void StopDeleteTimer()
        {
            if (timer.Delete != null)
            {
                timer.Delete.Dispose();
                timer.Delete = null;
            }
        }
        void StartDeleteTimer(int time)
        {
            if(timer.Delete == null) timer.Delete = new Timer(new TimerCallback(delete_callback), 0, time, 0);
        }
        public void Dispose()
        {
            GC.Collect(GC.GetGeneration(this));
        }
    }

    #region Monster Class
    public partial class obj
    {
        public List<int> Spawn = new List<int>();
        public byte xSec, ySec, Agresif, Type, Kat, State, LocalType;
        public int ID;
        public double x, z, y, wx, wy, WalkingTime, RecordedTime, oX, oY;
        public short area;
        public object Target;
        public int HP, aTime;
        public int UniqueID, LastCasting;
        public sbyte Move;
        public bool Busy, AutoMovement, OrgMovement, Walking, Attacking, Die, GetDie ,AutoSpawn, bSleep;
        public bool[] aRound = new bool[10];
        public bool[] guard = new bool[3];
        public byte spawnOran;
        public List<_agro> Agro = new List<_agro>(10);
        public ID Ids;
        public Random rnd = new Random();
        /// <summary>
        /// Mob: Cél játékos megszerzése célpontként.
        /// </summary>
        /// <returns>Játékos azonosítója</returns>
        public object GetTarget()
        {
            int id = 0;
            if(Agro != null && Agro.Count > 0)
            for(byte b = 0; b < Agro.Count; b++)
            {
                if (Agro[b].playerDMD == Agro.Max(f => f.playerDMD))
                {
                    id = Agro[b].playerID;
                    break;
                }
            }
            return Systems.GetPlayer(id);
        }

        public void DeleteTarget()
        {
            try
            {
                for (byte b = 0; b < Agro.Count; b++)
                {
                    if (Agro[b].playerDMD == Agro.Max(f => f.playerDMD))
                    {
                        if (Agro.Count > 1)
                        {
                            Agro.Remove(Agro[b]);
                            return;
                        }
                        else StopAttackTimer();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("DeleteTarget()::error");
                deBug.Write(ex);
            }
        }
        public void AddAgroDmg(int playerid ,int dmg)
        {
            try
            {
                if (Agro != null)
                {
                    for (byte b = 0; b < Agro.Count; b++)
                    {
                        if (Agro[b].playerID == playerid)
                        {
                            Agro[b].playerDMD += dmg;
                            return;
                        }
                    }
                    _agro asf = new _agro();
                    asf.playerID = playerid;
                    asf.playerDMD = dmg;
                    Agro.Add(asf);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("AddAgroDmg())::error");
                deBug.Write(ex);
            }
        }
        public _agro GetAgroClass(int id)
        {
            for(byte b = 0; b <= Agro.Count - 1; b++)
            {
                if (Agro[b].playerID == id) return Agro[b];
            }
            return null;
        }

        Timer Time;
        Timer Movement;
        Timer Dead;
        public Timer Attack;
        Timer AutoRun;
        Timer ganimet;
        Timer objeSleep;
        public void StartObjeSleep(int time)
        {
            if (!bSleep)
            {
                bSleep = true;
                objeSleep = new Timer(new TimerCallback(ObjeSleepCallBack), 0, time, 0);
            }
        }
        public void ObjeSleepCallBack(object e)
        {
            try
            {
                this.State = 3;
                this.bSleep = false;
                Send(Public.Packet.Movement(new Game.Global.vektor(this.UniqueID,
                (float)Function.Formule.packetx((float)this.x, this.xSec),
                (float)this.z,
                (float)Function.Formule.packety((float)this.y, this.ySec),
                this.xSec,
                this.ySec)));
                objeSleep.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine("ObjeSleepCallBack()::error");
                deBug.Write(ex);
            }
        }
        public bool Spawned(int id)
        {
            bool result = Spawn.Exists(
                    delegate(int bk)
                    {
                        return bk == id;
                    }
                    );

            return result;
        }
        public void StartRunTimer(int time)
        {
            if (AutoRun != null) AutoRun.Dispose();
            AutoRun = new Timer(new TimerCallback(AutoRunCallBack), 0, 0, time);
        }
        public void StopAutoRunTimer()
        {
            if (AutoRun != null) AutoRun.Dispose();
        }
        public void AutoRunCallBack(object e)
        {
            try
            {
                if (this.AutoMovement && !this.Die && this.LocalType == 1 && !this.Busy && !this.bSleep)
                {
                    double reX = oX, reY = oY;
                    Systems.aRound(ref reX, ref reY, 1);
                    this.x = reX;
                    this.y = reY;
                    this.aRound = new bool[8];
                    Send(Public.Packet.Movement(new Game.Global.vektor(this.UniqueID,
                                                    (float)Function.Formule.packetx((float)this.x, this.xSec),
                                                    (float)z,
                                                    (float)Function.Formule.packety((float)this.y, this.ySec),
                                                    this.xSec,
                                                    this.ySec)));

                }
                CheckEveryOne();
            }
            catch (Exception ex)
            {
                Console.WriteLine("AutoRunCallBack::error");
                deBug.Write(ex);
            }
        }
        
        public void Sleep(int time)
        {
            this.Busy = true;
            Time = new Timer(new TimerCallback(sleepcallback), 0, time, 0);
        }
        public void StartGanimet(int time)
        {
            this.Busy = true;
            ganimet = new Timer(new TimerCallback(ganicallback), 0, time, 0);
        }
        void ganicallback(object e)
        {
            try
            {
                SetExperience();
                SetDrop();
                ganimet.Dispose();
                ganimet = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine("ganicallback::error");
                deBug.Write(ex);
            }
        }
        /// <summary>
        /// Támadás időzítésének indítása. (AttackTimer = milyen időközönként üssön 1-et a mob/player.)
        /// </summary>
        /// <param name="time">Hány másodpercenként üssön...</param>
        public void StartAttackTimer(int time)
        {
            if (Attack != null) Attack.Dispose();
            aTime = time;
            Attack = new Timer(new TimerCallback(AttackCallBack), 0, 0, time);
        }
        /// <summary>
        /// Támadás időzítésének leállítása. (AttackTimer = milyen időközönként üssön 1-et a mob/player.)
        /// </summary>
        public void StopAttackTimer()
        {
            if(Attack != null) Attack.Dispose();
            Attacking = false;
            Attack = null;
        }
        void AttackCallBack(object e)
        {
            try
            {
                if (Attack != null)
                {
                    if (aTime < 1999)
                    {
                        aTime = 2000;
                        Attack.Change(0, aTime);
                    }
                    AttackMain();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("AttackCallBack::error");
                deBug.Write(ex);
            }
        }
        void sleepcallback(object e)
        {
            try
            {
                this.Time.Dispose();
                if (this.AutoSpawn) reSpawn();
                else { if (this.Spawn.Count != 0) Game.GlobalUnique.ClearObject(this); this.DeSpawnMe(); /*RandomID.Delete(this.UniqueID);*/ Systems.Objects.Remove(this); if (this.LastCasting != 0) /*Global.RandomID.Delete(this.LastCasting);*/ this.Dispose(); return; }
            }
            catch (Exception ex)
            {
                Console.WriteLine("sleepcallback(e)::eror...");
                deBug.Write(ex);
            }
        }
        void deadcallback(object e)
        {
            try
            {
                this.Die = true;
                this.DeSpawnMe();
                this.Dead.Dispose();
                this.Sleep(Global.RandomID.GetRandom(3000, 5000));
            }
            catch (Exception ex)
            {
                Console.WriteLine("deadcallback::error");
                deBug.Write(ex);
            }
        }
        public void StartDeadTimer(int time)
        {
            if (Dead != null) Dead.Dispose();
            Dead = new Timer(new TimerCallback(deadcallback), 0, time, 0);
        }
        public void StartMovement(int perTime)
        {
            Movement = new Timer(new TimerCallback(walkcallback), 0, 0, perTime);
        }
        public void StopMovement()
        {
            if (Movement != null) Movement.Dispose();
        }
        void walkcallback(object e)
        {
            try
            {
                if (this.RecordedTime <= 0) //yurume bitti
                {
                    Walking = false;
                    if (Attacking) AttackHim();
                    Movement.Dispose();
                }
                else
                {
                    this.x += (wx * 10) / 100;
                    this.y += (wy * 10) / 100;
                    RecordedTime -= (WalkingTime * 0.1);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("walkcallback::erro");
                deBug.Write(ex);
            }
        }
        public void Dispose()
        {
            GC.Collect(GC.GetGeneration(this));
        }
    }
    #endregion

    #region Object
    public class _agro
    {
        public int playerID;
        /// <summary>
        /// Játékos sebzése.
        /// </summary>
        public int playerDMD;
    }
    public sealed class targetObject
    {
        private float o_x, o_y;
        private double magdef, phydef;
        public obj os;
        public Systems sys, main;
        private bool type; //true player // false mob
        private int id;
        private short absorbphy, absorbmag;
        private int hps;
        private byte xsec, ysec;
        private byte state;
        private double mabsrob;
        public targetObject(object o, Systems player)
        {
            try
            {
                {
                    os = null;
                    o_x = 0;
                    o_y = 0;
                    magdef = 0;
                    phydef = 0;
                    type = false;
                }
                if (o == null) return;
                main = player;
                if (o.GetType().ToString() == "Game.obj")
                {
                    os = o as obj;
                    if (os.Die) { player.StopAttackTimer(); return; }
                    o_x = (float)os.x;
                    o_y = (float)os.y;
                    xsec = os.xSec;
                    ysec = os.ySec;
                    magdef = Data.ObjectBase[os.ID].MagDef;
                    phydef = Data.ObjectBase[os.ID].PhyDef;
                    id = os.UniqueID;
                    type = false;
                    hps = os.HP;
                    state = os.State;
                    //main.Karakter.Action.MonsterID.Add(os.UniqueID);
                    mabsrob = 0;
                    os.Target = player;
                }
                if (o.GetType().ToString() == "Game.Systems")
                {
                    sys = o as Systems;
                    o_x = sys.Karakter.Position.x;
                    o_y = sys.Karakter.Position.y;
                    xsec = sys.Karakter.Position.xSec;
                    ysec = sys.Karakter.Position.ySec;
                    magdef = sys.Karakter.Stat.MagDef;
                    phydef = sys.Karakter.Stat.PhyDef;
                    id = sys.Karakter.Information.UniqueID;
                    absorbphy = sys.Karakter.Stat.phy_Absorb;
                    absorbmag = sys.Karakter.Stat.mag_Absorb;
                    state = sys.Karakter.State.LastState;
                    hps = sys.Karakter.Stat.SecondHp;
                    type = true;
                    mabsrob = sys.Karakter.Stat.Absorb_mp;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("targetObject::error");
                deBug.Write(ex);
            }
        }
        public void GetDead()
        {
            try
            {
                if (type)
                {
                    if (main.Karakter.Information.PvP && sys.Karakter.Information.PvP)
                        sys.Karakter.State.DeadType = 2;
                    else
                        sys.Karakter.State.DeadType = 1;
                    sys.BuffAllClose();
                    sys.Karakter.State.Die = true;
                }
                else
                {
                    os.Die = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetDead()::Error");
                deBug.Write(ex);
            }
        }
        public void Sleep(byte types)
        {
            if (!type)
            {
                os.State = types;
                os.StartObjeSleep(5000);
            }
        }
        public void MP(int mpdusur)
        {
            if (type)
            {
                sys.Karakter.Stat.SecondMP -= mpdusur;
                if (sys.Karakter.Stat.SecondMP < 0) sys.Karakter.Stat.SecondMP = 0;
                sys.UpdateMp();
            }
        }
        public byte HP(int hpdusur)
        {
            try
            {
                if (type)
                {
                    sys.Karakter.Stat.SecondHp -= hpdusur;
                    sys.UpdateHp();
                    if (sys.Karakter.Stat.SecondHp <= 0)
                    {
                        sys.Karakter.Stat.SecondHp = 0;
                        sys.Karakter.State.Die = true;
                        sys.BuffAllClose();
                        main.StopAttackTimer();
                        sys.StopAttackTimer();
                        sys.StopSkillTimer();
                        return 128;
                    }
                }
                else
                {
                    if (os != null)
                    {
                        if (os.Attack == null) os.StartAttackTimer(3500);
                        os.CheckUnique();
                        if(!os.GetDie)
                            os.HP -= hpdusur;

                        //if(!os.GetDie || !os.Die) os.AddAgroDmg(main.Karakter.Information.UniqueID, hpdusur);
                        os.AddAgroDmg(main.Karakter.Information.UniqueID, hpdusur);

                        main.GetBerserkOrb();
                        //os.ChangeState(8, 1);
                        if (os.HP <= 0)
                        {
                            if (!os.GetDie)
                            {
                                Systems tg = (Systems)os.GetTarget();
                                if (tg.Karakter.Action.MonsterID != null && tg.Karakter.Action.MonsterIDCheck(os.UniqueID)) tg.Karakter.Action.MonsterID.Remove(os.UniqueID);
                                os.CheckUnique(tg);
                                os.StopAttackTimer();
                                os.StopMovement();
                                os.GetDie = true;
                                os.StartGanimet(1);
                                main.StopAttackTimer();
                                os.StartDeadTimer(4000);
                            }
                            return 128;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                deBug.Write(ex);
            }
            return 0;
        }
        public double MagDef
        {
            get { return magdef; }
        }
        public double PhyDef
        {
            get { return phydef; }
        }
        public float x
        {
            get { return o_x; }
        }
        public float y
        {
            get { return o_y; }
        }
        public int ID
        {
            get { return id; }
        }
        public short AbsrobPhy
        {
            get { return absorbphy; }
        }
        public short AbsrobMag
        {
            get { return absorbmag; }
        }
        public int GetHp
        {
            get { return hps; }
        }
        public byte xSec
        {
            get { return xsec; }
        }
        public byte ySec
        {
            get { return ysec; }
        }
        public byte State
        {
            get { return state; }
        }
        public double mAbsorb()
        {
            if (type)
                return mabsrob;
            else return 0;
        }
        public void Dispose()
        {
            GC.Collect(GC.GetGeneration(this));
        }

    }
    #endregion

    public sealed partial class party
    {
        public byte Type;
        public List<int> Members = new List<int>();
        public List<Client> MembersClient = new List<Client>();
        public int LeaderID;
        public bool IsFormed;
        public void Send(byte[] buff)
        {
            for (byte b = 0; b <= MembersClient.Count - 1; b++)
            {
                MembersClient[b].Send(buff);
            }
        }
        public void UpdateCoordinate()
        {
            for (byte b = 0; b <= MembersClient.Count - 1; b++)
            {
                /*Systems s = Systems.GetPlayer(Members[b]);
                MembersClient[b].Send(Public.Packet.Party_Data(6, Members[b]));*/
            }
        }
        public void Dispose()
        {
            GC.Collect(GC.GetGeneration(this));
        }
    }
}
