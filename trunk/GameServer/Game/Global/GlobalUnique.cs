using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Framework;



namespace Game
{
    public partial class GlobalUnique
    {
        
        public static List<obj> TigerGirl = new List<obj>();
        public static List<obj> Urichi = new List<obj>();
        public static List<obj> Isytaru = new List<obj>();
        public static List<obj> LordYarkan = new List<obj>();
        public static List<obj> DemonShaitan = new List<obj>();
        public static List<obj> Cerberus = new List<obj>();
        public static List<obj> CapIvy = new List<obj>();
        static bool Tiger, Uri, Isy, Lord, Demon,Cerb,Ivy;
        static Random rnd = new Random();
        public static Timer tiger1;
        public static Timer uri1;
        public static Timer isy1;
        public static Timer lord1;
        public static Timer demon1;
        public static Timer cerb1;
        public static Timer ivy1;
        /// <summary>
        /// TigerGirl spawn timer indító
        /// </summary>
        /// <param name="time"></param>
        /// <param name="per"></param>
        public static void StartTGUnique(int time, int per)
        {
            tiger1 = new Timer(new TimerCallback(TG), 0, time, per);
        }
        /// <summary>
        /// Uruchi spawn timer indító
        /// </summary>
        /// <param name="time"></param>
        /// <param name="per"></param>
        public static void StartUriUnique(int time, int per)
        {
            uri1 = new Timer(new TimerCallback(URI), 0, time, per);
        }
        /// <summary>
        /// Uruchi spawn timer indító
        /// </summary>
        /// <param name="time"></param>
        /// <param name="per"></param>
        public static void StartIsyUnique(int time,int per)
        {
            isy1 = new Timer(new TimerCallback(ISY), 0, time,per);
        }
        /// <summary>
        /// Uruchi spawn timer indító
        /// </summary>
        /// <param name="time"></param>
        /// <param name="per"></param>
         public static void StartLordUnique(int time,int per)
        {
            lord1 = new Timer(new TimerCallback(LORD), 0, time,per);
        }
        /// <summary>
        /// Demon spawn timer indító
        /// </summary>
        /// <param name="time"></param>
        /// <param name="per"></param>
         public static void StartDemonUnique(int time,int per)
        {
            demon1 = new Timer(new TimerCallback(DEMON), 0, time,per);
        }
        /// <summary>
         /// Cerberus spawn timer indító
        /// </summary>
        /// <param name="time"></param>
        /// <param name="per"></param>
         public static void StartCerbUnique(int time, int per)
         {
             cerb1 = new Timer(new TimerCallback(CERB), 0, time, per);
         }
        /// <summary>
        /// Captain Ivy spawn timer indító
        /// </summary>
        /// <param name="time"></param>
        /// <param name="per"></param>
         public static void StartIvyUnique(int time, int per)
         {
             ivy1 = new Timer(new TimerCallback(IVY), 0, time, per);
         }
        /// <summary>
        /// TigerGirl spawn
        /// </summary>
        /// <param name="e"></param>
        public static void TG(object e)
        {
            try
            {
                if (!Tiger)
                {
                    obj o = TigerGirl[rnd.Next(0, TigerGirl.Count - 1)];
                    Systems.Objects.Add(o);
                    o.SpawnMe();
                    Systems.SendAll(Public.Packet.Unique_Data(5, (int)o.ID, null));
                    Tiger = true;
                    Print.Format("Tiger girl spawned : {0}, {1}", o.x, o.y);
                }
            }
            catch (Exception ex)
            {
                deBug.Write(ex);
                Console.WriteLine("Tiger Girl-t nem sikerült spawnolni!");
            }
        }
        /// <summary>
        /// Uruchi spawn
        /// </summary>
        /// <param name="e"></param>
        public static void URI(object e)
        {
            try
            {
                if (!Uri)
                {
                    obj o = Urichi[rnd.Next(0, Urichi.Count - 1)];
                    Systems.Objects.Add(o);
                    o.SpawnMe();
                    Systems.SendAll(Public.Packet.Unique_Data(5, (int)o.ID, null));
                    Uri = true;
                    Print.Format("Urichi spawned : {0}, {1}", o.x, o.y);
                }
            }
            catch (Exception ex)
            {
                deBug.Write(ex);
                Console.WriteLine("Uruchi-t nem sikerült spawnolni!");
            }
        }
        /// <summary>
        /// Isyutaru spawn
        /// </summary>
        /// <param name="e"></param>
        public static void ISY(object e)
        {
            try
            {
                if (!Isy)
                {
                    obj o = Isytaru[rnd.Next(0, Isytaru.Count - 1)];
                    Systems.Objects.Add(o);
                    o.SpawnMe();
                    Systems.SendAll(Public.Packet.Unique_Data(5, (int)o.ID, null));
                    Isy = true;
                    Print.Format("Isy spawned : {0}, {1}", o.x, o.y);
                }
            }
            catch (Exception ex)
            {
                deBug.Write(ex);
                Console.WriteLine("Isyutaru-t nem sikerült spawnolni!");
            }
        }
        /// <summary>
        /// Isyutaru spawn
        /// </summary>
        /// <param name="e"></param>
        public static void LORD(object e)
        {
            try
            {
                  if (!Lord)
                {
                    obj o = LordYarkan[rnd.Next(0, LordYarkan.Count - 1)];
                    Systems.Objects.Add(o);
                    o.SpawnMe();
                    Systems.SendAll(Public.Packet.Unique_Data(5, (int)o.ID, null));
                    Lord = true;
                    Print.Format("Lord Yarkan spawned : {0}, {1}", o.x, o.y);
                }
            }
            catch (Exception ex)
            {
                deBug.Write(ex);
                Console.WriteLine("Lord Yarkant nem sikerült spawnolni!");
            }
        }
        /// <summary>
        /// Isyutaru spawn
        /// </summary>
        /// <param name="e"></param>
        public static void DEMON(object e)
        {
            try
            {
                 if (!Demon)
                {
                    obj o = DemonShaitan[rnd.Next(0, DemonShaitan.Count - 1)];
                    Systems.Objects.Add(o);
                    o.SpawnMe();
                    Systems.SendAll(Public.Packet.Unique_Data(5, (int)o.ID, null));
                    Demon = true;
                    Print.Format("Demon Shaitan spawned: {0}, {1}", o.x, o.y);
                }
            }
            catch (Exception ex)
            {
                deBug.Write(ex);
                Console.WriteLine("Demon Shaitant nem sikerült spawnolni!");
            }
        }
        /// <summary>
        /// Cerberus spawn
        /// </summary>
        /// <param name="e"></param>
        public static void CERB(object e)
        {
            try
            {
                if (!Cerb)
                {
                    obj o = Cerberus[rnd.Next(0, Cerberus.Count - 1)];
                    Systems.Objects.Add(o);
                    o.SpawnMe();
                    Systems.SendAll(Public.Packet.Unique_Data(5, (int)o.ID, null));
                    Cerb = true;
                    Print.Format("Cerberus spawned : {0}, {1}", o.x, o.y);
                }
            }
            catch (Exception ex)
            {
                deBug.Write(ex);
                Console.WriteLine("Cerberus-t nem sikerült spawnolni!");
            }
        }
        /// <summary>
        /// Captain Ivy spawn
        /// </summary>
        /// <param name="e"></param>
        public static void IVY(object e)
        {
            try
            {
                if (!Ivy)
                {
                    obj o = CapIvy[rnd.Next(0, CapIvy.Count - 1)];
                    Systems.Objects.Add(o);
                    o.SpawnMe();
                    Systems.SendAll(Public.Packet.Unique_Data(5, (int)o.ID, null));
                    Ivy = true;
                    Print.Format("Captain Ivy spawned : {0}, {1}", o.x, o.y);
                }
            }
            catch (Exception ex)
            {
                deBug.Write(ex);
                Console.WriteLine("Ivy-t nem sikerült spawnolni!");
            }
        }

        
        

        public static void AddObject(obj o)
        {
            switch (o.ID)
            {
                case 1954:
                    TigerGirl.Add(o);
                    break;
                case 1982:
                    Urichi.Add(o);
                    break;
                case 2002:
                    Isytaru.Add(o);
                    break;
                case 3810:
                    LordYarkan.Add(o);
                    break;
                case 3875:
                    DemonShaitan.Add(o);
                    break;
                case 5871:
                    Cerberus.Add(o);
                    break;
                case 14538:
                    CapIvy.Add(o);
                    break;
                default:
                    break;
            }
        }
        public static void ClearObject(obj o)
        {
            try
            {
                switch (o.ID)
                {
                    case 1954:
                        Tiger = false;
                        break;
                    case 1982:
                        Uri = false;
                        break;
                    case 2002:
                        Isy = false;
                        break;
                    case 3810:
                        Lord = false;
                        break;
                    case 3875:
                        Demon = false;
                        break;
                    case 5871:
                        Cerb = false;
                        break;
                    case 14538:
                        Ivy = false;
                        break;
                }
            }
            catch (Exception ex)
            {
                deBug.Write(ex);
                Console.WriteLine("unique::clearobject::error");
            }
        }
    }
}

