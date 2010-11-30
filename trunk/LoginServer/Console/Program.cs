using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Framework;
using Data;
using System.Diagnostics;
using System.Threading;
using System.Reflection;

namespace LoginServer
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Silkroad Project: LoginServer";
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine("HunSro Project, Silkroad Online LoginServer 2010");
            Console.WriteLine("------------------------------------------------");
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Gray;

            Program pro = new Program();

            #region Set error place
            MsSQL.OnDatabaseError += new MsSQL.dError(db_OnDatabaseError);
            MsSQL.OnConnectedToDatabase += new MsSQL.dConnected(db_OnConnectedToDatabase);
            #endregion

            Print.Format("Login Server Indítása...");
            Framework.Ini ini;
            #region Database Option
            //Szenya bővítmény:
            try
            {
                if (File.Exists(Environment.CurrentDirectory + @"\FrameWorkLogin.dll"))
                {
                    /*string asmpath = Environment.CurrentDirectory + @".\FrameWorkLogin.dll";
                    Assembly asm = Assembly.LoadFrom(asmpath);*/
                    if (File.Exists("./config/database.ini"))
                    {
                        ini = new Framework.Ini(Environment.CurrentDirectory + @"\config\database.ini");
                        string connectionstring = ini.GetValue("DATABASE", "connection").ToString();
                        MsSQL.Connection(connectionstring);
                    }
                    else
                    {
                        throw new Exception(string.Format("Nem található a következő fájl: {0}\\config\\database.ini \n Kérlek pótold!", Environment.CurrentDirectory));
                    }
                }
                else
                {
                    throw new Exception("Nincs meg a következő fájl: FrameWorkLogin.dll . Az alkalmazás újra telepítése megoldhatja a problémát...");
                }
            }
            catch (Exception err)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Print.Format("Hiba: {0}", err.Message);
                Console.ForegroundColor = ConsoleColor.White;
                Thread.Sleep(5000);
                return;
            }
            #endregion

            #region Server Option
            try
            {
                if (File.Exists("./config/server_login.ini"))
                {
                    ini = new Framework.Ini(Environment.CurrentDirectory + @"\config\server_login.ini");
                }
                else
                {
                    throw new Exception(string.Format("Nem található a következő fájl: {0}\\config\\server_login.ini \n Kérlek pótold!", Environment.CurrentDirectory));
                }
            }
            catch (Exception hiba)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Print.Format("Hiba: {0}", hiba.Message);
                Console.ForegroundColor = ConsoleColor.White;
                Thread.Sleep(5000);
                return;
            }
            #endregion

            Server net = new Server();

            net.OnConnect += new Server.dConnect(pro._OnClientConnect);
            net.OnError += new Server.dError(pro._ServerError);
            try
            {
                net.Start("127.0.0.1", 15779);
            }
            catch (Exception err)
            {
                Print.Format("Hiba történt az egyik beállítás betöltésekor: {0}", err.Message);
            }

            Client.OnReceiveData += new Client.dReceive(pro._OnReceiveData);
            Client.OnDisconnect += new Client.dDisconnect(pro._OnClientDisconnect);

            Data.Base.Load_NewsList();

            Console.ReadLine();
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
            /*Systems s = (Systems)o;
            s.Disconnect();*/
        }
        private void _ServerError(Exception ex)
        {
            Print.Format("Server error::{0}", ex.Message);
            Print.Format("Server source::{0}", ex.Source);
        }
        private static void db_OnDatabaseError(Exception ex)
        {
            Print.Format("Database error::{0}", ex.Message);
            Print.Format("Database source::{0}", ex.Source);
        }
        private static void db_OnConnectedToDatabase()
        {
            Print.Format("Adatbázishoz kapcsolódva...");
        }

    }
}
