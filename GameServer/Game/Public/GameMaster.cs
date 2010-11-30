using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework;

namespace Game
{
    public partial class Systems
    {
        void GM()
        {
            if (Karakter.Information.GM == 1)
            {
                
                
                PacketReader Reader = new PacketReader(PacketInformation.buffer);
                short gType = Reader.Int16();
                switch (gType)
                {
                    case 3:
                        GM_TOTOWN(Reader.Text());
                        break;
                    case 6://Monster
                        GM_LOADMONSTER(Reader.Int32(), Reader.Byte(), Reader.Byte());
                        break;
                    case 7://item
                        GM_MAKEITEM(Reader.Int32(), Reader.Byte());
                        break;
                    case 13:
                        GM_BAN(Reader.Text());
                        break;
                    case 16:
                        if (PacketInformation.buffer.Length > 4)
                            GM_WP(Reader.Byte(), Reader.Byte(), Reader.Single(), Reader.Single(), Reader.Single());
                        break;
                    case 17:
                        GM_RECALLUSER(Reader.Text());
                        break;
                    case 20:
                        GM_MOBKILL(Reader.Int32(), Reader.UInt16());
                        break;
                    default:
                        Print.Format(Decode.StringToPack(PacketInformation.buffer));
                        break;
                }
                Reader.Close();
            }
            else
            {
                ///hack
            }
        }
        void GM_MAKEITEM(int itemID, int itemPlus)
        {
            byte slot = GetFreeSlot();
            if (slot == 0) return;

            byte type = GetItemType(itemID);

            Print.Format("Type:{0}", type);
            if (type == 1) itemPlus = Data.ItemBase[itemID].Max_Stack;
            client.Send(Private.Packet.GM_MAKEITEM(type, slot, itemID, (short)itemPlus, (int)Data.ItemBase[itemID].Defans.Durability));
            AddItem(itemID, (short)itemPlus, slot, type, Karakter.Information.CharacterID);
        }
        void GM_LOADMONSTER(int model, byte type, byte type2)
        {
            Game.Public.GroupSpawn asd = new Public.GroupSpawn();
            obj o = new obj();
            o.ID = model;
            o.Type = type;
            o.Ids = new Global.ID(Global.ID.IDS.Object);
            o.UniqueID = o.Ids.GetUniqueID;
            o.x = Karakter.Position.x;
            o.z = Karakter.Position.z;
            o.y = Karakter.Position.y;
            o.oX = o.x;
            o.oY = o.y;
            o.xSec = Karakter.Position.xSec;
            o.ySec = Karakter.Position.ySec;
            o.AutoMovement = true;
            o.HP = Data.ObjectBase[model].HP;
            o.Agresif = 0;
            o.LocalType = 1;
            o.AutoSpawn = false;
            o.Kat = 1;
                Systems.Objects.Add(o);
                    o.SpawnMe();
            
                  

 
            
        }
        void GM_MOBKILL(int id, ushort type)
        {
            try
            {
                obj o = GetObject(id);
                o.GetDie = true;
                o.StartDeadTimer(6000);
            }
            catch (Exception ex)
            {
                Console.WriteLine("GM_MOBKILL()::error..");
                deBug.Write(ex);
            }
        }
        void GM_BAN(string name)
        {
            try
            {
                foreach (Systems sys in Systems.clients)
                {
                    if (sys.Karakter.Information.Name == name)
                    {
                        MsSQL.UpdateData("UPDATE users SET ban='" + 1 + "' WHERE id='" + sys.Player.AccountName + "'");
                        sys.client.Send(Public.Packet.ChatPacket(7, Karakter.Information.UniqueID, "from GM:You are banned.", null));
                        sys.Disconnect();
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("GM_BAN()::error..");
                deBug.Write(ex);
            }
        }
        void GM_TOTOWN(string name)
        {
            byte number = File.FileLoad.GetTeleport(name);
            if (number == 255) return;

            if (Timer.Movement != null) Timer.Movement.Dispose();



            BuffAllClose();
            //System.Threading.Thread.Sleep(1000);
            //client.Send(Private.Packet.TeleportStart());
            client.Send(Private.Packet.TeleportOtherStart());
            
            ObjectDeSpawn();
            DeSpawnMe();

            Karakter.InGame = false;
            //ObjectClear();
            
            //System.Threading.Thread.Sleep(1000);
            Teleport_UpdateXYZ(number);
            //client.Send(Private.Packet.TeleportImage(0, 0));
            client.Send(Private.Packet.TeleportImage(Data.PointBase[number].xSec, Data.PointBase[number].ySec));

            Karakter.Teleport = true;
        }
        void GM_RECALLUSER(string name)
        {
            try
            {
                foreach (Systems sys in Systems.clients)
                {
                    if (sys.Karakter.Information.Name == name && sys.Karakter.InGame)
                    {
                        sys.BuffAllClose();
                        if (sys.Timer.Movement != null) sys.Timer.Movement.Dispose();

                        sys.client.Send(Private.Packet.TeleportOtherStart());
                        sys.DeSpawnMe();
                        sys.ObjectDeSpawn();

                        sys.Karakter.Position.xSec = Karakter.Position.xSec;
                        sys.Karakter.Position.ySec = Karakter.Position.ySec;
                        sys.Karakter.Position.x = Karakter.Position.x;
                        sys.Karakter.Position.z = Karakter.Position.z;
                        sys.Karakter.Position.y = Karakter.Position.y;

                        sys.client.Send(Private.Packet.TeleportImage(Karakter.Position.xSec, Karakter.Position.xSec));
                        sys.Karakter.InGame = false;
                        sys.Karakter.Teleport = true;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("GM_RECALLUSER()::error..");
                deBug.Write(ex);
            }
        }
        void GM_WP(byte xSec, byte ySec, float x, float z, float y)
        {
            BuffAllClose();
            if (Timer.Movement != null) Timer.Movement.Dispose();

            client.Send(Private.Packet.TeleportOtherStart());
            DeSpawnMe();

            ObjectDeSpawn();

            Karakter.Position.xSec = xSec;
            Karakter.Position.ySec = ySec;
            Karakter.Position.x = Function.Formule.gamex(x, xSec);
            Karakter.Position.z = z;
            Karakter.Position.y = Function.Formule.gamey(y, ySec);
            
            //client.Send(Private.Packet.TeleportStart());
            client.Send(Private.Packet.TeleportImage(Karakter.Position.xSec, Karakter.Position.xSec));
            Karakter.InGame = false;
            Karakter.Teleport = true;
        }
    }
}
