using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework;
namespace Game
{
    public partial class Systems
    {
        void Handle()
        {
            PacketReader Reader = new PacketReader(PacketInformation.buffer);
            byte iSlot = Reader.Byte();
            Global.slotItem uItemID = GetItem((uint)Karakter.Information.CharacterID, iSlot, false);
            if (uItemID.ID == 0 || Karakter.State.Die || Karakter.Information.Scroll) return;
            switch (uItemID.ID)
            {
                case 61:
                case 2198:
                case 2199:
                    HandleReturnScroll(uItemID.ID);
                    HandleUpdateSlot(iSlot, uItemID, Reader.UInt16());
                    break;
                //hp pot
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 3817:
                case 3818:
                case 3819:
                case 5912:
                    byte type = 1;
                    HandlePotion(type, uItemID.ID);
                    HandleUpdateSlot(iSlot, uItemID, Reader.UInt16());
                    break;
                //mp pot
                case 11:
                case 12:
                case 13:
                case 14:
                case 15:
                case 3820:
                case 3821:
                case 3822:
                case 5913:
                    type = 2;
                    HandlePotion(type, uItemID.ID);
                    HandleUpdateSlot(iSlot, uItemID, Reader.UInt16());
                    break;
                case 9:
                    type = 3;
                    HandlePotion(type, uItemID.ID);
                    HandleUpdateSlot(iSlot, uItemID, Reader.UInt16());
                    break;
                case 16:
                    type = 4;
                    HandlePotion(type, uItemID.ID);
                    HandleUpdateSlot(iSlot, uItemID, Reader.UInt16());
                    break;
                case 2137:
                case 2138:
                case 2139:
                case 3909:
                case 23330:
                case 22953:
                case 23396:
                case 23395:
                case 22952:
                    if (!Karakter.Transport.Right && Karakter.Action.MonsterID.Count == 0)
                    {
                        bool r = HandleHorseScroll(uItemID.ID);
                        if(!r)
                            HandleUpdateSlot(iSlot, uItemID, Reader.UInt16());
                    }
                    break;
                case 3851:
                    HandleUpdateSlot(iSlot, uItemID, Reader.UInt16());
                    Reader.Skip(1);
                    short chatcount = Reader.Int16();
                    HandleGlobalChat(Encoding.ASCII.GetString(PacketInformation.buffer, 6, chatcount));
                    break;
                /*case 18:
                case 19:
                case 20:
                case 21:
                case 22:
                    type = 5;
                    HandlePotion(type, uItemID.ID);
                    HandleUpdateSlot(iSlot, uItemID, Reader.UInt16());
                    break;*/
                /*case 23:
                    type = 3;
                    HandlePotion(type, uItemID.ID);
                    HandleUpdateSlot(iSlot, uItemID, Reader.UInt16());
                    break;*/
                // buffer items
                case 7100:
                    ItemBuff(3977);
                    HandleUpdateSlot(iSlot, uItemID, Reader.UInt16());
                    break;
                case 7098:
                    ItemBuff(3975);
                    HandleUpdateSlot(iSlot, uItemID, Reader.UInt16());
                    break;
                default:
                    //Print.Format("{0} using a {1} item", Karakter.Information.Name, uItemID.ID);
                    break;
            }
            Reader.Close();
        }
        void HandleGlobalChat(string text)
        {
            SendAll(Public.Packet.ChatPacket(6, 0, text, Karakter.Information.Name));
        }
        bool HandleHorseScroll(int ItemID)
        {
            int model = Global.objectdata.GetItem(Data.ItemBase[ItemID].ObjectName);
            if (this.Karakter.Information.Level < Data.ItemBase[ItemID].Level) return true;

            if (model == 0)
            {
                string extrapath = null;
                if (this.Karakter.Information.Level >= 1 && this.Karakter.Information.Level <= 5)
                    extrapath = "_5";
                else if (this.Karakter.Information.Level >= 6 && this.Karakter.Information.Level <= 10)
                    extrapath = "_10";
                else if (this.Karakter.Information.Level >= 11 && this.Karakter.Information.Level <= 20)
                    extrapath = "_20";
                else if (this.Karakter.Information.Level >= 21 && this.Karakter.Information.Level <= 30)
                    extrapath = "_30";
                else if (this.Karakter.Information.Level >= 31 && this.Karakter.Information.Level <= 45)
                    extrapath = "_45";
                else if (this.Karakter.Information.Level >= 46 && this.Karakter.Information.Level <= 60)
                    extrapath = "_60";
                else if (this.Karakter.Information.Level >= 61 && this.Karakter.Information.Level <= 75)
                    extrapath = "_75";
                else if (this.Karakter.Information.Level >= 76 && this.Karakter.Information.Level <= 90)
                    extrapath = "_90";
                else if (this.Karakter.Information.Level >= 91 && this.Karakter.Information.Level <= 105)
                    extrapath = "_105";
                else if (this.Karakter.Information.Level >= 106 && this.Karakter.Information.Level <= 120)
                    extrapath = "_120";
                model = Global.objectdata.GetItem(Data.ItemBase[ItemID].ObjectName + extrapath);
                if (model == 0) return true;
            }
            pet_obj o = new pet_obj();
            o.Model = model;
            o.Ids = new Global.ID(Global.ID.IDS.Object);
            o.UniqueID = o.Ids.GetUniqueID;
            o.x = Karakter.Position.x;
            o.z = Karakter.Position.z;
            o.y = Karakter.Position.y;
            o.xSec = Karakter.Position.xSec;
            o.ySec = Karakter.Position.ySec;
            o.Hp = Data.ObjectBase[model].HP;
            o.OwnerID = this.Karakter.Information.UniqueID;
            this.Karakter.Transport.Right = true;

            List<int> S = o.SpawnMe();
            o.Information = true;
            client.Send(Public.Packet.Pet_Information(o.UniqueID, o.Model, o.Hp));
            Send(S, Public.Packet.Player_UpToHorse(this.Karakter.Information.UniqueID, true, o.UniqueID));
            Systems.HelperObject.Add(o);
            this.Karakter.Transport.Horse = o;
            return false;
        }
        void HandleReturnScroll(int ItemID)
        {
            Send(Private.Packet.StatePack(Karakter.Information.UniqueID, 0x0B, 0x01, false));
            Karakter.Information.Scroll = true;
            StartScrollTimer(Data.ItemBase[ItemID].Use_Time);
        }
        
        void HandleUpdateSlot(byte slot, Global.slotItem item, ushort packet)
        {
            item.Amount--;
            client.Send(Public.Packet.Player_HandleUpdateSlot(slot, (ushort)item.Amount, packet));
            if (item.Amount > 0)
            {
                MsSQL.UpdateData("UPDATE char_items SET quantity='" + Math.Abs(item.Amount) + "' WHERE owner='" + Karakter.Information.CharacterID + "' AND itemnumber='item" + item.Slot + "' AND id='" + item.dbID + "'");
            }
            else MsSQL.UpdateData("delete from char_items where id='" + item.dbID + "'");
            Send(Public.Packet.Player_HandleEffect(Karakter.Information.UniqueID, item.ID));
        }
        void HandlePotion(byte type, int ItemID)
        {
            if (type == 1) // hp
            {
                long miktar = (Karakter.Stat.Hp * Karakter.Information.Level * (long)Data.ItemBase[ItemID].Use_Time) / HandlePotionLevel(Karakter.Stat.Hp);
                byte pslot = Getfreepotslot(Karakter.Information.Item.Potion);
                StartPotionTimer(920, new int[] { (int)miktar, type, pslot }, pslot);
            }
            else if (type == 2)//mp
            {
                long miktar = (Karakter.Stat.Mp * Karakter.Information.Level * (long)Data.ItemBase[ItemID].Use_Time2) / HandlePotionLevel(Karakter.Stat.Mp);
                byte pslot = Getfreepotslot(Karakter.Information.Item.Potion);
                StartPotionTimer(920, new int[] { (int)miktar, type, pslot }, pslot);
            }
            else if (type == 3) //hp %25
            {
                long miktar = (Karakter.Stat.Hp * 25) / 100;
                byte pslot = Getfreepotslot(Karakter.Information.Item.Potion);
                Karakter.Information.Item.Potion[pslot] = 4;
                StartPotionTimer(920, new int[] { (int)miktar, type, pslot }, pslot);
            }
            else if (type == 4) //mp %25
            {
                long miktar = (Karakter.Stat.Mp * 25) / 100;
                byte pslot = Getfreepotslot(Karakter.Information.Item.Potion);
                Karakter.Information.Item.Potion[pslot] = 4;
                StartPotionTimer(920, new int[] { (int)miktar, type, pslot }, pslot);
            }
            /*else if (type == 5)
            {
                long miktar = (Karakter.Stat.Mp * 25) / 100;
                byte pslot = Getfreepotslot(Karakter.Information.Item.Potion);
                Karakter.Information.Item.Potion[pslot] = 4;
                StartPotionTimer(920, new int[] { (int)miktar, type, pslot }, pslot);
            }*/
        }
        public static int HandlePotionLevel(int hp)
        {
            int sayi = hp;
            byte basamak = 0;

            while (sayi > 0)
            {
                basamak++;
                sayi /= 10;
            }
            int b = 10;
            for (int i = 1; i <= basamak; i++)
            {
                b *= 10;
            }

            return b;
        }
        public static byte Getfreepotslot(byte[] r)
        {
            for (byte b = 0; b < r.Length; b++)
                if (r[b] == 0) return b;
            return 255;
        }
    }
}
