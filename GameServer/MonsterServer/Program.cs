using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game;
using System.IO;
using Framework;
namespace gmConsole
{
    public class Program
    {
        static void Main(string[] args)
        {
            /* HERE JUST FOR TEST! */
            Console.Title = "Silkroad Project: GM CONSOLE";
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine("Arcane Project, Silkroad Online gm console 2010");
            Console.WriteLine("-----------------------------------------------");
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Gray;
            /*
            MsSQL.OnDatabaseError += new MsSQL.dError(db_OnDatabaseError);

            IniFile ini = new IniFile(Environment.CurrentDirectory + @"\config\server.ini");
            MsSQL.Connection(ini.IniReadValue("DATABASE", "connectionstring"));*/
            while (true)
            {
                string read = Console.ReadLine();
                if (read == "online")
                {
                    MsSQL.UpdateData("UPDATE users SET online='" + 1 + "' WHERE id='" + "gokhan" + "'");
                }
                else Systems.Announce(read);
            }
        }

        private static void db_OnDatabaseError(Exception ex)
        {
            Print.Format("Database error::{0}", ex.Message);
            Print.Format("Database source::{0}", ex.StackTrace);
        }

    }
}



