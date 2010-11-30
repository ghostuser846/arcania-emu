using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework;

namespace Data
{
    public static class Base
    {
        public static void Load_NewsList()
        {
            try
            {
                MsSQL ms = new MsSQL("SELECT TOP 4 * FROM news"); 
                    System.Data.SqlClient.SqlDataReader reader = ms.Read();
                    while (reader.Read())
                    {
                        NewsList nl = new NewsList();
                        nl.Head = reader.GetString(2);
                        nl.Msg = reader.GetString(2);
                        nl.Year = reader.GetInt16(3);
                        nl.Month = reader.GetInt16(4);
                        Systems.News_List.Add(nl);
                    }
                    ms.Close();
                    reader.Close();
                    reader.Dispose();
                Print.Format("Hír lista kész...");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

    }
    public class NewsList
    {
        public string Head, Msg;
        public short Year, Month;
    }
}
