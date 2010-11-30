using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework;
using System.Data.SqlClient;

namespace Game.Function
{
    class Items
    {
        public static void PrivateItemPacket(PacketWriter Writer, int id, byte iType, byte max, byte avatar)
        {
            List<byte> slots = new List<byte>();
            MsSQL ms = new MsSQL("SELECT * FROM char_items WHERE owner='" + id + "' AND type='" + iType + "' AND slot >= '0' AND slot <= '" + max + "' AND inavatar='" + avatar + "'");
            using (SqlDataReader reader = ms.Read())
            {
                    int count = ms.Count();
                    Writer.Byte(count);
                    while (reader.Read())
                    {
                        if (reader.GetInt32(2) != 0)
                        {
                            if (!slots.Exists(delegate(byte bk) { return bk == reader.GetByte(5); }))
                            {
                                slots.Add(reader.GetByte(5));
                            }
                            else
                            {
                                Console.WriteLine("item bug found::{0}::{1}::{2}::{3}" ,avatar, reader.GetByte(5), reader.GetInt32(2), id);
                            }
                            Writer.DWord(reader.GetInt32(2));
                            Writer.Byte(reader.GetByte(4));
                        }
                    }
            }
            ms.Close();
        }

        public static byte RandomPlusValue()
        {
            int RandomNumber = Global.RandomID.GetRandom(0, 100);
            if (RandomNumber == 100) return 8;
            else if (RandomNumber == 99) return 7;
            else if (RandomNumber == 98) return 6;
            else if (RandomNumber <= 97 && RandomNumber >= 95) return 5;
            else if (RandomNumber <= 80 && RandomNumber >= 75) return 4;
            else if (RandomNumber <= 74 && RandomNumber >= 68) return 3;
            else if (RandomNumber <= 67 && RandomNumber >= 60) return 2;
            else if (RandomNumber <= 59 && RandomNumber >= 50) return 1;

            return 0;
        }
        public static bool CheckCape(int ItemID)
        {
            switch (ItemID)
            {
                case 3726:
                case 3727:
                case 3728:
                case 3729:
                case 3730:
                case 3731:
                case 3732:
                case 3733:
                case 3734:
                case 3735:
                    return true;
                default:
                    return false;
            }
        }
    }
}
