using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework;
using Game;
using System.IO;
using System.Threading;
using System.Diagnostics;

namespace GameServer
{
    //#define DEBUG_LOGGER
    class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine(0x02E2BE);
            Console.Title = "Silkroad Project: GameServer";
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine("HunSRO Project, Silkroad Online GameServer 2010");
            Console.WriteLine("-----------------------------------------------");
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Gray;
            //Console.WriteLine(Decode.StringToPack(BitConverter.GetBytes((int)30890)));
            Program pro = new Program();
            MsSQL.OnDatabaseError += new MsSQL.dError(db_OnDatabaseError);
            MsSQL.OnConnectedToDatabase += new MsSQL.dConnected(db_OnConnectedToDatabase);

            //Biztonsági intézkedés, hogy ne merüljenek fel kérdések, mi a francé nem indul...
            if (File.Exists("./config/server.ini"))
            {
                Ini ini = new Ini(Environment.CurrentDirectory + @"\config\server.ini");
                MsSQL.Connection(ini.GetValue("DATABASE", "connectionstring").ToString());
                ini = new Ini(Environment.CurrentDirectory + @"\config\rates.ini");
                Game.Rate.Gold = Convert.ToByte(ini.GetValue("Rate", "Gold"));
                Game.Rate.Item = Convert.ToByte(ini.GetValue("Rate", "Item"));
                Game.Rate.Xp = Convert.ToByte(ini.GetValue("Rate", "Xp"));
                Game.Rate.Sp = Convert.ToByte(ini.GetValue("Rate", "Sp"));
                Game.Rate.Sox = Convert.ToByte(ini.GetValue("Rate", "SOX"));
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Print.Format("Nem található a következő fájl: {0}\\config\\server.ini", Environment.CurrentDirectory);
                Thread.Sleep(5000);
                return;
            }

            Print.Format("Rate Xp:{0} Sp:{1} Gold:{2} Item:{3} SOX:{4}", Game.Rate.Xp, Game.Rate.Sp, Game.Rate.Gold, Game.Rate.Item, Game.Rate.Sox);
            Server net = new Server();
            net.Start("127.0.0.1", 15780);
            Users.StartUserTablo(15779);
            net.OnConnect += new Server.dConnect(pro._OnClientConnect);
            net.OnError += new Server.dError(pro._ServerError);

            Systems.ServerStartedTime = DateTime.Now;

            Client.OnReceiveData += new Client.dReceive(pro._OnReceiveData);
            Client.OnDisconnect += new Client.dDisconnect(pro._OnClientDisconnect);

            Print.Format("Game Server Indítása... ");

            Game.File.FileLoad.Load();
            Game.Systems.clients.update += new EventHandler(Users.updateServerList);


           
                
                //A boss mobok spawn időzítésének az indítása:
           
                 Random rand = new Random();
            
                Game.GlobalUnique.StartTGUnique(rand.Next(10,20) * 60000,  600);
                Game.GlobalUnique.StartUriUnique(rand.Next(10,20) * 60000,  600);
                Game.GlobalUnique.StartIsyUnique(rand.Next(10,20) * 60000,  600);
                Game.GlobalUnique.StartLordUnique(rand.Next(10,20) * 60000,  600);
                Game.GlobalUnique.StartDemonUnique(rand.Next(10,20) * 60000,  600);
                Game.GlobalUnique.StartCerbUnique(rand.Next(10,20) * 60000,  600);
                Game.GlobalUnique.StartIvyUnique(rand.Next(10,20)  * 60000,  60000);
           
            
            //Szenya:
            //A konzolba írt parancsok kezelésének külön szálra való helyezése (ez a program stabilitása miatt fontos, ennélkül néha kifagyna):
            Program progi = new Program();
            Thread parancskezelés = new Thread(new ThreadStart(progi.ParancsVárás));
            parancskezelés.Start();
            while (!parancskezelés.IsAlive) ;
            Thread.Sleep(1);

        }
        public void _OnReceiveData(Decode de)
        {
            Systems.oPCode(de);
        }
        public void _OnClientConnect(ref object de, Client net)
        {
            de = new Systems(net);
        }
        public void _OnClientDisconnect(object o)
        {
            Systems s = (Systems)o;
            s.PrintLastPack();
            s.Disconnect();
        }
        private void _ServerError(Exception ex)
        {
            Print.Format("Server error::{0}", ex.Message);
            Print.Format("Server source::{0}", ex.StackTrace);
        }
        private static void db_OnDatabaseError(Exception ex)
        {
            Print.Format("Database error::{0}", ex);
        }
        private static void db_OnConnectedToDatabase()
        {
            Print.Format("Adatbázishoz kapcsolódás...");
        }

        public void ParancsVárás()
        {
            while (true)
            {
                string read = Console.ReadLine();
                string[] command = read.Split(' ');
                if (command[0] == "online")
                {
                    Systems.AnnounceOnlines();
                }
                else if (command[0] == "updateonline")
                {
                    MsSQL.InsertData("update server set users_current='" + Systems.GetOnlineClientCount + "' where serverid='" + 1905 + "'");
                }
                else if (command[0] == "clear1")
                {
                    System.GC.Collect();
                }
                else if (command[0] == "clear2")
                {
                    GC.Collect(0, GCCollectionMode.Forced);
                }
                else if (command[0] == "fixitem")
                {
                    int fixeditem = 0;
                    MsSQL ms = new MsSQL("SELECT * FROM char_items");
                    using (System.Data.SqlClient.SqlDataReader reader = ms.Read())
                    {
                        while (reader.Read())
                        {
                            short amount = reader.GetInt16(7);
                            if (amount < 1)
                            {
                                fixeditem++;
                                amount = 1;
                                MsSQL.InsertData("UPDATE char_items SET quantity='" + amount + "' WHERE itemnumber='" + "item" + reader.GetByte(5) + "' AND owner='" + reader.GetInt32(3) + "' AND itemid='" + reader.GetInt32(2) + "'");
                            }
                        }
                    }
                    Console.WriteLine("{0} Item FIXED!!", fixeditem);
                }
                else if (command[0] == "test")
                {
                    MsSQL ms = new MsSQL("SELECT * FROM char_items WHERE owner='" + 129 + "' AND type='" + 0 + "' AND slot >= '0' AND slot <= '" + 50 + "' AND inavatar='" + 0 + "'");
                    using (System.Data.SqlClient.SqlDataReader reader = ms.Read())
                    {
                        Console.WriteLine(ms.Count());
                    }
                }
                else if (command[0] == "fix" && command[1] == "all" && command[2] == "char")
                {
                    int chars = 0;
                    MsSQL ms = new MsSQL("SELECT * FROM karakterler");
                    using (System.Data.SqlClient.SqlDataReader reader = ms.Read())
                    {
                        while (reader.Read())
                        {
                            bool ifixed = false;
                            List<byte> slots = new List<byte>();
                            List<byte> needDeleteSlot = new List<byte>();
                            MsSQL msf = new MsSQL("SELECT * FROM char_items WHERE type='" + 0 + "' AND slot >= '0' AND slot <= '" + 8 + "' AND inavatar='" + 0 + "' AND owner='" + reader.GetInt32(0) + "'");

                            using (System.Data.SqlClient.SqlDataReader itemreader = msf.Read())
                            {
                                while (itemreader.Read())
                                    if (!slots.Exists(delegate(byte bk) { return bk == itemreader.GetByte(5); })) slots.Add(itemreader.GetByte(5));
                                    else
                                    {
                                        ifixed = true;
                                        needDeleteSlot.Add(itemreader.GetByte(5));
                                        chars++;
                                    }
                            }
                            msf.Close();
                            if (ifixed)
                            {
                                MsSQL.UpdateData("update karakterler set min_phyatk='" + Convert.ToInt32(Math.Round(6 + (0.45 * reader.GetInt16(6)))) +
                                                        "', max_phyatk='" + Convert.ToInt32(Math.Round(9 + (0.65 * reader.GetInt16(6)))) +
                                                        "', min_magatk='" + Convert.ToInt32(Math.Round(6 + (0.45 * reader.GetInt16(7)))) +
                                                        "', max_magatk='" + Convert.ToInt32(Math.Round(10 + (0.65 * reader.GetInt16(7)))) +
                                                        "', phydef='" + Math.Round(6 + (reader.GetInt16(6) * 0.40)) + //(0.40 * amount)
                                                        "', magdef='" + Math.Round(3 + (reader.GetInt16(7) * 0.40)) +
                                                        "', hit='" + 11 +
                                                        "', parry='" + 11 +
                                                        "', gold='" + Convert.ToInt64((reader.GetInt64(11) + (reader.GetByte(5) * 500000))) +
                                                        "' where id='" + reader.GetInt32(0) + "'");

                                MsSQL.InsertData("delete from char_items WHERE type='" + 0 + "' AND slot >= '0' AND slot <= '" + 8 + "' AND inavatar='" + 0 + "' AND owner='" + reader.GetInt32(0) + "'");

                                /*foreach (byte slot in needDeleteSlot)
                                {
                                    MsSQL.InsertData("delete from char_items WHERE type='" + 0 + "' AND slot='" + slot + "' AND owner='" + reader.GetInt32(0) + "'");
                                }*/
                            }
                        }
                    }

                    ms.Close();

                    Console.WriteLine("Fixed Character Num:{0}", chars);
                }
                else if (command[0] == "fix" && command[1] == "all" && command[2] == "avatar")
                {
                    int chars = 0;
                    MsSQL ms = new MsSQL("SELECT * FROM karakterler");
                    using (System.Data.SqlClient.SqlDataReader reader = ms.Read())
                    {
                        while (reader.Read())
                        {
                            bool ifixed = false;
                            List<byte> slots = new List<byte>();
                            List<byte> needDeleteSlot = new List<byte>();
                            MsSQL msf = new MsSQL("SELECT * FROM char_items WHERE slot >= '0' AND slot <= '" + 8 + "' AND inavatar='" + 1 + "' AND owner='" + reader.GetInt32(0) + "'");

                            using (System.Data.SqlClient.SqlDataReader itemreader = msf.Read())
                            {
                                while (itemreader.Read())
                                    if (!slots.Exists(delegate(byte bk) { return bk == itemreader.GetByte(5); })) slots.Add(itemreader.GetByte(5));
                                    else
                                    {
                                        ifixed = true;
                                        needDeleteSlot.Add(itemreader.GetByte(5));
                                        chars++;
                                    }
                            }
                            msf.Close();
                            if (ifixed)
                            {
                                foreach (byte slot in needDeleteSlot)
                                {
                                    MsSQL.InsertData("delete from char_items WHERE slot='" + slot + "' AND owner='" + reader.GetInt32(0) + "' AND inavatar='" + 1 + "'");
                                }
                            }
                        }
                    }

                    ms.Close();

                    Console.WriteLine("Fixed Character avatar Num:{0}", chars);
                }
                else if (command[0] == "fix" && command[1] == "c")
                {
                    int chars = 0;
                    MsSQL ms = new MsSQL("SELECT * FROM karakterler where name='" + command[2] + "'");
                    using (System.Data.SqlClient.SqlDataReader reader = ms.Read())
                    {
                        while (reader.Read())
                        {

                            MsSQL.UpdateData("update karakterler set min_phyatk='" + Convert.ToInt32(Math.Round(6 + (0.45 * reader.GetInt16(6)))) +
                                    "', max_phyatk='" + Convert.ToInt32(Math.Round(9 + (0.65 * reader.GetInt16(6)))) +
                                    "', min_magatk='" + Convert.ToInt32(Math.Round(6 + (0.45 * reader.GetInt16(7)))) +
                                    "', max_magatk='" + Convert.ToInt32(Math.Round(10 + (0.65 * reader.GetInt16(7)))) +
                                    "', phydef='" + Math.Round(6 + (reader.GetInt16(6) * 0.40)) + //(0.40 * amount)
                                    "', magdef='" + Math.Round(3 + (reader.GetInt16(7) * 0.40)) +
                                    "', hit='" + 11 +
                                    "', parry='" + 11 +
                                    "', gold='" + Convert.ToInt64((reader.GetInt64(11) + (reader.GetByte(5) * 500000))) +
                                    "' where id='" + reader.GetInt32(0) + "'");

                            MsSQL.InsertData("delete from char_items WHERE type='" + 0 + "' AND slot >= '0' AND slot <= '" + 8 + "' AND inavatar='" + 0 + "' AND owner='" + reader.GetInt32(0) + "'");

                            chars++;
                            break;
                        }
                    }
                }
                else if (command[0] == "update")
                {
                    if (command[1] == "silk")
                    {
                        MsSQL.UpdateData("update users set silk='" + Convert.ToInt32(command[2]) + "'");
                    }
                }
                else if (command[0] == "start" && command[1] == "unique")
                {
                    Game.GlobalUnique.StartTGUnique(6000 * 10, 6000 * 10);
                    Game.GlobalUnique.StartUriUnique(7000 * 10, 7000 * 10);
                    Game.GlobalUnique.StartIsyUnique(8000 * 10, 8000 * 10);
                    Game.GlobalUnique.StartLordUnique(9000 * 10, 9000 * 10);
                    Game.GlobalUnique.StartDemonUnique(10000 * 10, 10000 * 10);
                    Game.GlobalUnique.StartCerbUnique(11000 * 10, 11000 * 10);
                    //Game.GlobalUnique.StartIvyUnique(12000 * 10, 12000 * 10);
                }
            }
        }
    }
}
