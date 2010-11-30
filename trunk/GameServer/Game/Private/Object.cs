using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework;

namespace Game
{
    public partial class Systems
    {
        void SelectObject()
        {
            PacketReader Reader = new PacketReader(PacketInformation.buffer);
            int objectid = Reader.Int32();
            if (objectid == 0) return;
            if (objectid == Karakter.Information.UniqueID) return;
            obj o = GetObject(objectid);
            if (o != null)
            {
                //client.Send(Effect.testEf(objectid));
                byte[] bb = Game.Private.Packet.SelectObject(objectid, o.ID, o.LocalType, o.HP);
                if (bb == null) return;
                client.Send(bb);
                return;
            }

            Systems sys = GetPlayers(objectid);
            if (o == null && sys != null)
            {
                client.Send(Game.Private.Packet.SelectObject(objectid,0 , 5, sys.Karakter.Stat.Hp));
                return;
            }
            Reader.Close();
        }
        public bool MonsterCheck(int id)
        {
            if(Karakter.Action.MonsterID != null)
            for (int i = 0; i < Karakter.Action.MonsterID.Count; i++)
            {
                if (Karakter.Action.MonsterID != null && Karakter.Action.MonsterID[i] != 0 && Karakter.Action.MonsterID[i] == id) return true;
            }
            return false;
        }
        obj GetObject(int id)
        {
            for (int i = 0; i <= Systems.Objects.Count - 1; i++)
            {
                if (Systems.Objects[i] != null && Systems.Objects[i].UniqueID == id)
                    return Systems.Objects[i];
            }
            return null;
        }
        /// <summary>
        /// Játékosok megszerzése id alapján.
        /// </summary>
        /// <param name="id">A játékos azonosító száma.</param>
        /// <returns>Ha van az id alapján megadott játékos, akkor azt adja vissza, ha nincs akkor null -t add vissza.</returns>
        Systems GetPlayers(int id)
        {
            for (int i = 0; i <= Systems.clients.Count - 1; i++)
            {
                if (Systems.clients[i] != null && Systems.clients[i].Karakter.Information.UniqueID == id)
                    return Systems.clients[i];
            }
            return null;
        }
        world_item GetWorldItem(int id)
        {
            for (int i = 0; i <= Systems.WorldItem.Count - 1; i++)
            {
                if (Systems.WorldItem[i] != null && Systems.WorldItem[i].UniqueID == id)
                    return Systems.WorldItem[i];
            }
            return null;
        }
        object GetObjects(int id)
        {
            obj o = GetObject(id);
            if (o != null) return o;

            Systems sys = GetPlayers(id);
            if (sys != null) return sys;

            return null;
        }
        public static bool cRound(bool[] b)
        {
            foreach (bool bol in b)
            {
                if (!bol) return true;
            }
            return false;
        }
        public static bool aRound(ref bool[] b, ref float x, ref float y)
        {
            if (!b[0])
            {
                x -= 1.5f;
                y -= 1.8f;
                b[0] = true;
                return false;
            }
            else if (!b[1])
            {
                y -= 3;
                b[1] = true;
                return false;
            }
            else if (!b[2])
            {
                x += 1.8f;
                y -= 1.5f;
                b[2] = true;
                return false;
            }
            else if (!b[3])
            {
                x += 3;
                b[3] = true;
                return false;
                
            }
            else if (!b[4])
            {
                x -= 3;
                b[4] = true;
                return false;
            }
            else if (!b[5])
            {
                x -= 1.2f;
                y += 1.8f;
                b[5] = true;
                return false;

            }
            else if (!b[6])
            {
                y += 3;
                b[6] = true;
                return false;
            }
            else if (!b[7])
            {
                x += 1.5f;
                y += 1.5f;
                b[7] = true;
                return false;

            }

            return true; // eger hic bi slot boş degilse
        }
        public static void aRound(ref double x, ref double y, byte oran)
        {
            switch (grnd.Next(0, 10))
            {
                case 0:
                    x -= 1.5f * oran;
                    y -= 1.8f * oran;
                    break;
                case 1:
                    y -= 3 * oran;
                    break;
                case 2:
                    x += 1.8f * oran;
                    y -= 1.5f * oran;
                    break;
                case 3:
                    x += 3 * oran;
                    break;
                case 4:
                    x -= 3 * oran;
                    break;
                case 5:
                    x -= 1.2f * oran;
                    y += 1.8f * oran;
                    break;
                case 6:
                    y += 3 * oran;
                    break;
                case 7:
                    x += 1.5f * oran;
                    y += 1.5f * oran;
                    break;
                case 8:
                    x += 2f * oran;
                    y += 1.5f * oran;
                    break;
                case 9:
                    x -= 2 * oran;
                    break;
                case 10:
                    y -= 2 * oran;
                    break;
            }
        }
        public static void aRound(ref float x, ref float y, byte oran)
        {
            switch (grnd.Next(0, 10))
            {
                case 0:
                    x -= 1.5f * oran;
                    y -= 1.8f * oran;
                    break;
                case 1:
                    y -= 3 * oran;
                    break;
                case 2:
                    x += 1.8f * oran;
                    y -= 1.5f * oran;
                    break;
                case 3:
                    x += 3 * oran;
                    break;
                case 4:
                    x -= 3 * oran;
                    break;
                case 5:
                    x -= 1.2f * oran;
                    y += 1.8f * oran;
                    break;
                case 6:
                    y += 3 * oran;
                    break;
                case 7:
                    x += 1.5f * oran;
                    y += 1.5f * oran;
                    break;
                case 8:
                    x += 2f * oran;
                    y += 1.5f * oran;
                    break;
                case 9:
                    x -= 2 * oran;
                    break;
                case 10:
                    y -= 2 * oran;
                    break;
            }
        }
        public static byte RandomType(byte level, ref byte Kat)
        {
            int rnd = Global.RandomID.GetRandom(0,100);

            if (rnd > 70 && rnd < 89)
            {
                Kat = 2;
                return 1;
            }
            else if (rnd > 90 && level > 14)
            {
                Kat = 20;
                return 4;
            }
            else if (rnd < 70)
            {
                Kat = 1;
                return 0;
            }
            return 255;
        }
        public static int[] GetEliteIds(int ID)
        {
            int[] mid = new int[10];

            switch (ID)
            {
                case 1954: // TIGER GIRL
                    mid[0] = 1953;
                    mid[1] = 1952;
                    mid[2] = 1947;
                    mid[3] = 1953;
                    mid[4] = 1952;
                    mid[5] = 1947;
                    mid[6] = 1953;
                    mid[7] = 1952;
                    mid[8] = 1947;
                    break;
                case 5871: // CERBERUS
                    mid[0] = 5870;
                    mid[1] = 5868;
                    mid[2] = 5865;
                    mid[3] = 5866;
                    mid[4] = 5870;
                    mid[5] = 5868;
                    mid[6] = 5865;
                    mid[7] = 5866;
                    mid[8] = 5866;
                    break;
                case 1982: //Urichi
                    mid[0] = 1980;
                    mid[1] = 1980;
                    mid[2] = 1981;
                    mid[3] = 1981;
                    mid[4] = 1980;
                    mid[5] = 1981;
                    break;
                case 2002: //ISYTARU
                    mid[0] = 1995;
                    mid[1] = 2118;
                    mid[2] = 1996;
                    mid[3] = 2125;
                    mid[4] = 1996;
                    mid[5] = 2125;
                    break;

                case 3810: // LORD YARKAN
                    mid[0] = 3802;
                    mid[1] = 3803;
                    mid[2] = 3804;
                    mid[3] = 3805;
                    mid[4] = 3806;
                    mid[5] = 3807;
                    mid[6] = 3808;
                    mid[7] = 3809;
                    break;

                case 3875: // DEMON SHATİAN
                    mid[0] = 3874;
                    mid[1] = 3874;
                    mid[2] = 3872;
                    mid[3] = 3873;
                    mid[4] = 3874;
                    mid[5] = 3874;
                    mid[6] = 3872;
                    mid[7] = 3873;
                    break;
            }

            return mid;
        }
    }
}
