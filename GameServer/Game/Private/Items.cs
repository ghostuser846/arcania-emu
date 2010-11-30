using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework;

namespace Game
{
    public partial class Systems
    {
        public void AddItem(int itemid, short prob, byte slot, byte type, int id)
        {
            if (type == 0)
            {
                MsSQL.UpdateData("Insert Into char_items (itemid,plusvalue,durability,owner,itemnumber,slot,type) VALUES ('" + itemid + "','" + prob + "','" + Data.ItemBase[itemid].Defans.Durability + "','" + id + "','item" + slot + "','" + slot + "','" + type + "' )");
            }
            else if (type == 1)
            {
                if (prob < 1) prob = 1;
                MsSQL.UpdateData("Insert Into char_items (itemid,quantity,owner,itemnumber,slot,type) VALUES ('" + itemid + "','" + prob + "','" + id + "','item" + slot + "','" + slot + "','" + type + "' )");
            }
            else if (type == 3)
            {
                MsSQL.UpdateData("Insert Into char_items (itemid,plusvalue,durability,owner,itemnumber,slot,type) VALUES ('" + itemid + "','" + prob + "','" + Data.ItemBase[itemid].Defans.Durability + "','" + id + "','avatar" + slot + "','" + slot + "','" + type + "' )");
            }
        }
        public static int GetItem(int id, byte slot)
        {
            return MsSQL.GetDataInt("SELECT * FROM char_items WHERE itemnumber='item" + slot + "' AND owner='" + id + "'", "itemid");
        }
        public byte GetFreeSlot()
        {
            List<byte> ListSlot = new List<byte>(0x2d);
            MsSQL ms = new MsSQL("SELECT * FROM char_items WHERE owner='" + Karakter.Information.CharacterID + "' AND slot >= '13' AND slot <= '" + 0x2D + "' AND inavatar='0'");
            using (System.Data.SqlClient.SqlDataReader reader = ms.Read())
            {
                while (reader.Read())
                {
                    ListSlot.Add(reader.GetByte(5));
                }
            }
            ms.Close();
            for (byte i = 13; i < 0x2d; i++)
            {
                if (!GetCheckFreeSlot(ListSlot, i)) return i;
            }

            return 0;
        }
        public byte GetFreeSlotMax()
        {
            List<byte> ListSlot = new List<byte>(0x2d);
            MsSQL ms = new MsSQL("SELECT * FROM char_items WHERE owner='" + Karakter.Information.CharacterID + "' AND slot >= '13' AND slot <= '" + 0x2D + "' AND inavatar='0'");
            using (System.Data.SqlClient.SqlDataReader reader = ms.Read())
            {
                while (reader.Read())
                {
                    ListSlot.Add(reader.GetByte(5));
                }
            }
            ms.Close();
            byte add = 0;
            for (byte i = 13; i < 0x2d; i++)
            {
                if (!GetCheckFreeSlot(ListSlot, i)) add++;
            }

            return add;
        }
        public byte GetFreeSlot(int id)
        {
            List<byte> ListSlot = new List<byte>(0x2d);
            MsSQL ms = new MsSQL("SELECT * FROM char_items WHERE owner='" + Karakter.Information.CharacterID + "' AND slot >= '13' AND slot <= '" + 0x2D + "' AND inavatar='0'");
            using (System.Data.SqlClient.SqlDataReader reader = ms.Read())
            {
                while (reader.Read())
                {
                    ListSlot.Add(reader.GetByte(5));
                }
            }
            ms.Close();
            for (byte i = 13; i < 0x2d; i++)
            {
                if (!GetCheckFreeSlot(ListSlot, i)) return i;
            }

            return 0;
        }
        bool GetCheckFreeSlot(List<byte> b, byte bs)
        {
            bool result = b.Exists(
                    delegate(byte bk)
                    {
                        return bk == bs;
                    }
                    );
            return result;
        }
        void GetUpdateSlot(Global.slotItem item, byte toSlot, int toItemID, short quantity)
        {
            client.Send(Private.Packet.MoveItem(0,item.Slot, toSlot, quantity));

            /*MsSQL.InsertData("UPDATE char_items SET itemnumber='item" + item.Slot + "',slot='" + item.Slot + "' WHERE itemnumber='" + "item" + toSlot + "' AND owner='" + Karakter.Information.CharacterID + "' AND itemid='" + toItemID + "'");
            MsSQL.InsertData("UPDATE char_items SET itemnumber='item" + toSlot + "',slot='" + toSlot + "' WHERE itemnumber='" + "item" + item.Slot + "' AND owner='" + Karakter.Information.CharacterID + "' AND itemid='" + item.ID + "'");*/
        }
        void ItemMain()
        {
            try
            {
                lock (this)
                {
                    if (Karakter.State.Die || Karakter.Information.Scroll) return;
                    PacketReader Reader = new PacketReader(PacketInformation.buffer);
                    byte iType = Reader.Byte();

                    switch (iType)
                    {
                        case 0:
                            ItemMove(Reader.Byte(), Reader.Byte(), Reader.Int16());
                            break;
                        case 1:
                            ItemMoveInStorage(Reader.Byte(), Reader.Byte(), Reader.Int16());
                            break;
                        case 2:
                            Player_MoveItemToStorage(Reader.Byte(), Reader.Byte(), Reader.Int32());
                            break;
                        case 3:
                            Player_MoveStorageItemToInv(Reader.Byte(), Reader.Byte(), Reader.Int32());
                            break;
                        case 4:
                            ItemMoveToExhangePage(Reader.Byte());
                            break;
                        case 5:
                            ItemMoveExchangeToInv(Reader.Byte());
                            break;
                        case 8:
                            Player_BuyItem(Reader.Byte(), Reader.Byte(), Reader.Int16(), Reader.Int32());
                            break;
                        case 9:
                            Player_SellItem(Reader.Byte(), Reader.Int16(), Reader.UInt16());
                            break;
                        case 7:
                            Player_DropItem(Reader.Byte());
                            break;
                        case 10:
                            Player_DropGold(Reader.UInt64());
                            break;
                        case 11:
                            Player_TakeGoldW(iType, Reader.Int64());
                            break;
                        case 12:
                            Player_GiveGoldW(iType, Reader.Int64());
                            break;
                        case 13:
                            ItemExchangeGold(Reader.Int64());
                            break;
                        case 24:
                            //36 01 01 02 04 string
                            Player_BuyItemFromMall(Reader.Byte(), Reader.Byte(), Reader.Byte(), Reader.Byte(), Reader.Byte(), Reader.Text());
                            break;
                        case 35:
                            ItemAvatarUnEquip(Reader.Byte(), Reader.Byte());
                            break;
                        case 36:
                            ItemAvatarEquip(Reader.Byte(), Reader.Byte());
                            break;
                        default:
                            Print.Format("Unknown Item Function:{0}:{1}", iType, Decode.StringToPack(PacketInformation.buffer));
                            break;
                    }
                    Reader.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("item main error...:");
                deBug.Write(ex);

            }
        }
        static Global.slotItem GetItem(uint id, byte slot, bool type)
        {
            Global.slotItem slotItem = new Global.slotItem();
            string table = "char_items";
            if (type) table = "storage_items";
            MsSQL ms = new MsSQL("SELECT * FROM "+ table+" WHERE itemnumber='item" + slot + "' AND owner='" + id + "'");
            using (System.Data.SqlClient.SqlDataReader reader = ms.Read())
            {
                while (reader.Read())
                {
                    slotItem.dbID = reader.GetInt32(0);
                    slotItem.ID = reader.GetInt32(2);
                    slotItem.PlusValue = reader.GetByte(4);
                    slotItem.Type = reader.GetByte(6);
                    slotItem.Amount = reader.GetInt16(7);
                    slotItem.Durability = reader.GetInt32(8);
                    slotItem.Slot = slot;
                }
            }
            ms.Close();
            return slotItem;
        }
        void ItemMove(byte fromSlot, byte toSlot, short quantity)
        {
            Global.slotItem fromItem = GetItem((uint)Karakter.Information.CharacterID, fromSlot,false);
            Global.slotItem toItem = GetItem((uint)Karakter.Information.CharacterID, toSlot, false);

            fromItem.Slot = fromSlot;
            toItem.Slot = toSlot;

            if (toSlot == 8 && !Function.Items.CheckCape(fromItem.ID)) return;
            if (fromSlot < 13 && toItem.ID != 0) return;
            if (toItem.ID != 0 && fromSlot < 13) 
                if (Data.ItemBase[fromItem.ID].Class_B != Data.ItemBase[toItem.ID].Class_B) return;

            if (toSlot < 13)
            {
                if (!CheckItemLevel(Karakter.Information.Level, fromItem.ID))
                {
                    return;
                }
                if (!CheckGender(Karakter.Information.Model, fromItem.ID))
                {
                    return;
                }
            }

            Global.slotItem shieldItem = GetItem((uint)Karakter.Information.CharacterID, 7, false);
            Global.slotItem weaponItem = GetItem((uint)Karakter.Information.CharacterID, 6, false);

            if (toSlot == 6)
            {
                if (shieldItem.ID != 0)
                {
                    if (Data.ItemBase[shieldItem.ID].Class_D == 3) //eğer shield slotundaki item ARROW ise
                    {
                        if (Data.ItemBase[fromItem.ID].Class_C != 6) //Bowdan farklı bir itemse Arrow'u cikart.
                        {
                            byte s = GetFreeSlot();
                            if (s <= 12) return;
                            ItemUnEquiped(shieldItem);
                            GetUpdateSlot(shieldItem, s, 0, 1);
                            MsSQL.InsertData("UPDATE char_items SET itemnumber='item" + s + "', slot='" + s + "' WHERE id='" + shieldItem.dbID + "'");
                        }
                    }
                    else if (Data.ItemBase[shieldItem.ID].Class_D == 1) //eğer shield slotundaki shield ise!
                    {
                        if (Data.ItemBase[fromItem.ID].Class_C != 2 && Data.ItemBase[fromItem.ID].Class_C != 3) // eğer item blade ve sword değilse
                        {
                            byte s = GetFreeSlot();
                            if (s <= 12) return;
                            ItemUnEquiped(shieldItem);
                            GetUpdateSlot(shieldItem, s, 0, 1);
                            MsSQL.InsertData("UPDATE char_items SET itemnumber='item" + s + "', slot='" + s + "' WHERE id='" + shieldItem.dbID + "'");
                        }
                    }
                }
            }
            if (toSlot == 7)
            {
                if (weaponItem.ID != 0)
                {
                    if (Data.ItemBase[fromItem.ID].Class_D == 3) //eğer atilan item ARROW ise
                    {
                        if (Data.ItemBase[weaponItem.ID].Class_C != 6)
                        {
                            byte s = GetFreeSlot();
                            if (s <= 12) return;
                            ItemUnEquiped(weaponItem);
                            GetUpdateSlot(weaponItem, s, 0, 1);
                            MsSQL.InsertData("UPDATE char_items SET itemnumber='item" + s + "',slot='" + s + "' WHERE id='" + weaponItem.dbID + "'");
                        }
                    }
                    else if (Data.ItemBase[fromItem.ID].Class_D == 1) //eğer atilan item Shield ise
                    {
                        if (Data.ItemBase[weaponItem.ID].Class_C != 2 && Data.ItemBase[weaponItem.ID].Class_C != 3) // eğer item blade ve sword değilse
                        {
                            byte s = GetFreeSlot();
                            if (s <= 12) return;
                            ItemUnEquiped(weaponItem);
                            GetUpdateSlot(weaponItem, s, 0, 1);
                            MsSQL.InsertData("UPDATE char_items SET itemnumber='item" + s + "',slot='" + s + "' WHERE id='" + weaponItem.dbID + "'");
                        }
                    }
                }
            }

            if (GetItemType(fromItem.ID) == 0)
            {
                if (toSlot < 13) ItemEquiped(toItem, fromItem);
                if (fromSlot < 13) ItemUnEquiped(fromItem);
            }

            #region for bow
            if (Data.ItemBase[fromItem.ID].Class_D == 3 && toSlot == 7) // for arrow
            {
                Karakter.Information.Item.sAmount = fromItem.Amount;
                Karakter.Information.Item.sID = fromItem.ID;
            }
            if (Data.ItemBase[fromItem.ID].Class_D == 3 && fromSlot == 7)
            {
                Karakter.Information.Item.sAmount = 0;
                Karakter.Information.Item.sID = 0;
            }
            #endregion

            client.Send(Private.Packet.MoveItem(0, fromSlot, toSlot, quantity));

            #region Move Data On Database
            short nQuan = 0;
            short fQuan = 0;

            if (toSlot < 13) // eğer item giyiyorsa
            {
                MsSQL.InsertData("UPDATE char_items SET itemnumber='item" + toSlot + "',slot='" + toSlot + "' WHERE id='" + fromItem.dbID + "'");
                if (toItem.ID != 0)
                {
                    MsSQL.InsertData("UPDATE char_items SET itemnumber='item" + fromSlot + "',slot='" + fromSlot + "' WHERE id='" + toItem.dbID + "'");
                }
            }
            if (fromSlot < 13) // eğer item cıkartiyorsa
            {
                MsSQL.InsertData("UPDATE char_items SET itemnumber='item" + toSlot + "',slot='" + toSlot + "' WHERE id='" + fromItem.dbID + "'");
            }
            if (fromSlot >= 13 && toSlot >= 13)
            {
                if (toItem.ID != 0)
                {
                    if (toItem.ID == fromItem.ID) // eğer item birleştiriliyorsa
                    {
                        if (Data.ItemBase[fromItem.ID].Class_D == 3 && Data.ItemBase[toItem.ID].Class_D == 3)
                        {
                            if (Data.ItemBase[fromItem.ID].Max_Stack > 1) // eğer itemin alabilecegi item sayısı 1 den fazlaysa
                            {
                                nQuan = (short)(fromItem.Amount + toItem.Amount);
                                if (nQuan <= Data.ItemBase[fromItem.ID].Max_Stack) // eğer item dolduysa
                                {
                                    MsSQL.InsertData("delete from char_items where id='" + fromItem.dbID + "'");
                                    MsSQL.InsertData("UPDATE char_items SET quantity='" + nQuan + "' WHERE id='" + toItem.dbID + "'");
                                }
                                else //eğer item fazla geldiyse
                                {
                                    fQuan = (short)(nQuan % Data.ItemBase[fromItem.ID].Max_Stack);
                                    nQuan -= fQuan;
                                    MsSQL.InsertData("UPDATE char_items SET quantity='" + fQuan + "' WHERE id='" + fromItem.dbID + "'");
                                    MsSQL.InsertData("UPDATE char_items SET quantity='" + nQuan + "' WHERE id='" + toItem.dbID + "'");
                                }
                            }
                        }
                        else
                        {
                            MsSQL.InsertData("UPDATE char_items SET itemnumber='item" + toSlot + "',slot='" + toSlot + "' WHERE id='" + fromItem.dbID + "'");
                            MsSQL.InsertData("UPDATE char_items SET itemnumber='item" + fromSlot + "',slot='" + fromSlot + "' WHERE id='" + toItem.dbID + "'");
                        }
                    }
                    else
                    {
                        MsSQL.InsertData("UPDATE char_items SET itemnumber='item" + toSlot + "',slot='" + toSlot + "' WHERE id='" + fromItem.dbID + "'");
                        MsSQL.InsertData("UPDATE char_items SET itemnumber='item" + fromSlot + "',slot='" + fromSlot + "' WHERE id='" + toItem.dbID + "'");
                    }
                }
                else
                {
                    if (fromItem.Amount != quantity && Data.ItemBase[fromItem.ID].Class_D == 3)
                    {
                        AddItem(fromItem.ID, quantity, toSlot, 1, Karakter.Information.CharacterID);
                        int calc = (fromItem.Amount - quantity);
                        if (calc < 1) calc = 1;
                        MsSQL.InsertData("UPDATE char_items SET quantity='" + calc + "' WHERE id='" + fromItem.dbID + "'");
                    }
                }
            }
            /*if (toItem.ID != 0)
            {
                if (toItem.ID == fromItem.ID) // eğer item birleştiriliyorsa
                {
                    if (Data.ItemBase[fromItem.ID].Class_D == 3 && Data.ItemBase[toItem.ID].Class_D == 3)
                    {
                        if (Data.ItemBase[fromItem.ID].Max_Stack > 1) // eğer itemin alabilecegi item sayısı 1 den fazlaysa
                        {
                            nQuan = (short)(fromItem.Amount + toItem.Amount);
                            if (nQuan <= Data.ItemBase[fromItem.ID].Max_Stack) // eğer item dolduysa
                            {
                                MsSQL.InsertData("delete from char_items where id='" + fromItem.dbID + "'");
                                MsSQL.InsertData("UPDATE char_items SET quantity='" + nQuan + "' WHERE id='" + toItem.dbID + "'");
                            }
                            else //eğer item fazla geldiyse
                            {
                                fQuan = (short)(nQuan % Data.ItemBase[fromItem.ID].Max_Stack);
                                nQuan -= fQuan;
                                MsSQL.InsertData("UPDATE char_items SET quantity='" + fQuan + "' WHERE id='" + fromItem.dbID + "'");
                                MsSQL.InsertData("UPDATE char_items SET quantity='" + nQuan + "' WHERE id='" + toItem.dbID + "'");
                            }
                        }
                    }
                    else
                    {
                        MsSQL.InsertData("UPDATE char_items SET itemnumber='item" + fromSlot + "',slot='" + fromSlot + "' WHERE id='" + toItem.dbID + "'");
                    }
                }
                else
                {
                    MsSQL.InsertData("UPDATE char_items SET itemnumber='item" + fromSlot + "',slot='" + fromSlot + "' WHERE id='" + toItem.dbID + "'");
                }
            }
            else
            {
                if (fromItem.Amount != quantity && Data.ItemBase[fromItem.ID].Class_D == 3)
                {
                    AddItem(fromItem.ID, quantity, toSlot, 1, Karakter.Information.CharacterID);
                    int calc = (fromItem.Amount - quantity);
                    if (calc < 1) calc = 1;
                    MsSQL.InsertData("UPDATE char_items SET quantity='" + calc + "' WHERE id='" + fromItem.dbID + "'");
                }
            }

            if (fQuan == 0 && fromItem.Amount == quantity)
            {
                MsSQL.InsertData("UPDATE char_items SET itemnumber='item" + toSlot + "',slot='" + toSlot + "' WHERE id='" + fromItem.dbID + "'");
            }
            else if (fQuan == 0 && quantity == 0)
            {
                MsSQL.InsertData("UPDATE char_items SET itemnumber='item" + toSlot + "',slot='" + toSlot + "' WHERE id='" + fromItem.dbID + "'");
            }*/
            SavePlayerInfo();
            #endregion
        }
        public void ItemUnEquiped(Global.slotItem item)
        {
            if (item.Slot <= 5)
            {
                Karakter.Stat.MagDef -= (Data.ItemBase[item.ID].Defans.MinMagDef + (item.PlusValue * Data.ItemBase[item.ID].Defans.MagDefINC));
                Karakter.Stat.PhyDef -= (Data.ItemBase[item.ID].Defans.MinPhyDef + (item.PlusValue * Data.ItemBase[item.ID].Defans.PhyDefINC));
                Karakter.Stat.Parry -= Data.ItemBase[item.ID].Defans.Parry;
                client.Send(Private.Packet.PlayerStat(Karakter));
            }
            else if (item.Slot == 6)
            {
                Karakter.Stat.MinPhyAttack -= (Data.ItemBase[item.ID].Attack.Min_LPhyAttack + (item.PlusValue * Data.ItemBase[item.ID].Attack.PhyAttackInc));
                Karakter.Stat.MaxPhyAttack -= (Data.ItemBase[item.ID].Attack.Min_HPhyAttack + (item.PlusValue * Data.ItemBase[item.ID].Attack.PhyAttackInc));
                Karakter.Stat.MinMagAttack -= (Data.ItemBase[item.ID].Attack.Min_LMagAttack + (item.PlusValue * Data.ItemBase[item.ID].Attack.MagAttackINC));
                Karakter.Stat.MaxMagAttack -= (Data.ItemBase[item.ID].Attack.Min_HMagAttack + (item.PlusValue * Data.ItemBase[item.ID].Attack.MagAttackINC));
                Karakter.Stat.Hit -= Data.ItemBase[item.ID].Attack.MinAttackRating;
                client.Send(Private.Packet.PlayerStat(Karakter));
                Karakter.Information.Item.wID = 0;
            }
            else if (item.Slot == 7)
            {
                if (GetItemType(item.ID) == 0)
                {
                    Karakter.Stat.MagDef -= (Data.ItemBase[item.ID].Defans.MinMagDef + (item.PlusValue * Data.ItemBase[item.ID].Defans.MagDefINC));
                    Karakter.Stat.PhyDef -= (Data.ItemBase[item.ID].Defans.MinPhyDef + (item.PlusValue * Data.ItemBase[item.ID].Defans.PhyDefINC));
                    client.Send(Private.Packet.PlayerStat(Karakter));
                }
                Karakter.Information.Item.sAmount = 0;
                Karakter.Information.Item.sID = 0;
            }
            else if (item.Slot == 8 && Function.Items.CheckCape(item.ID))
            {
                Karakter.Information.PvP = false;
            }
            else if (item.Slot >= 9 && item.Slot <= 12)
            {
                Karakter.Stat.mag_Absorb -= (short)((Data.ItemBase[item.ID].Defans.MagAbsorb + (item.PlusValue * Data.ItemBase[item.ID].Defans.AbsorbINC)) * 10);
                Karakter.Stat.phy_Absorb -= (short)((Data.ItemBase[item.ID].Defans.PhyAbsorb + (item.PlusValue * Data.ItemBase[item.ID].Defans.AbsorbINC)) * 10);
            }

            Send(Private.Packet.MoveItemUnequipEffect(Karakter.Information.UniqueID, item.Slot, item.ID));
        }
        public void ItemEquiped(Global.slotItem item, Global.slotItem item2)
        {
            if (item.Slot <= 5)
            {
                if (item.ID != 0)
                {
                    Karakter.Stat.MagDef -= (Data.ItemBase[item.ID].Defans.MinMagDef + (item.PlusValue * Data.ItemBase[item.ID].Defans.MagDefINC));
                    Karakter.Stat.PhyDef -= (Data.ItemBase[item.ID].Defans.MinPhyDef + (item.PlusValue * Data.ItemBase[item.ID].Defans.PhyDefINC));
                    Karakter.Stat.Parry -= Data.ItemBase[item.ID].Defans.Parry;
                }
                Karakter.Stat.MagDef += (Data.ItemBase[item2.ID].Defans.MinMagDef + (item2.PlusValue * Data.ItemBase[item2.ID].Defans.MagDefINC));
                Karakter.Stat.PhyDef += (Data.ItemBase[item2.ID].Defans.MinPhyDef + (item2.PlusValue * Data.ItemBase[item2.ID].Defans.PhyDefINC));
                Karakter.Stat.Parry += Data.ItemBase[item2.ID].Defans.Parry;

                client.Send(Private.Packet.PlayerStat(Karakter));
            }
            else if (item.Slot == 6)
            {
                if (item.ID != 0)
                {
                    Karakter.Stat.MinPhyAttack -= (Data.ItemBase[item.ID].Attack.Min_LPhyAttack + (item.PlusValue * Data.ItemBase[item.ID].Attack.PhyAttackInc));
                    Karakter.Stat.MaxPhyAttack -= (Data.ItemBase[item.ID].Attack.Min_HPhyAttack + (item.PlusValue * Data.ItemBase[item.ID].Attack.PhyAttackInc));
                    Karakter.Stat.MinMagAttack -= (Data.ItemBase[item.ID].Attack.Min_LMagAttack + (item.PlusValue * Data.ItemBase[item.ID].Attack.MagAttackINC));
                    Karakter.Stat.MaxMagAttack -= (Data.ItemBase[item.ID].Attack.Min_HMagAttack + (item.PlusValue * Data.ItemBase[item.ID].Attack.MagAttackINC));
                    Karakter.Stat.Hit -= Data.ItemBase[item.ID].Attack.MinAttackRating;
                }
                Karakter.Stat.MinPhyAttack += (Data.ItemBase[item2.ID].Attack.Min_LPhyAttack + (item2.PlusValue * Data.ItemBase[item2.ID].Attack.PhyAttackInc));
                Karakter.Stat.MaxPhyAttack += (Data.ItemBase[item2.ID].Attack.Min_HPhyAttack + (item2.PlusValue * Data.ItemBase[item2.ID].Attack.PhyAttackInc));
                Karakter.Stat.MinMagAttack += (Data.ItemBase[item2.ID].Attack.Min_LMagAttack + (item2.PlusValue * Data.ItemBase[item2.ID].Attack.MagAttackINC));
                Karakter.Stat.MaxMagAttack += (Data.ItemBase[item2.ID].Attack.Min_HMagAttack + (item2.PlusValue * Data.ItemBase[item2.ID].Attack.MagAttackINC));
                Karakter.Stat.Hit += Data.ItemBase[item2.ID].Attack.MinAttackRating;
                Karakter.Information.Item.wID = item2.ID;

                client.Send(Private.Packet.PlayerStat(Karakter));
            }
            else if (item.Slot == 7)
            {
                if (item.ID != 0)
                {
                    if (GetItemType(item.ID) == 0)
                    {
                        Karakter.Stat.MagDef -= (Data.ItemBase[item.ID].Defans.MinMagDef + (item.PlusValue * Data.ItemBase[item.ID].Defans.MagDefINC));
                        Karakter.Stat.PhyDef -= (Data.ItemBase[item.ID].Defans.MinPhyDef + (item.PlusValue * Data.ItemBase[item.ID].Defans.PhyDefINC));
                    }
                }
                if (GetItemType(item2.ID) == 0)
                {
                    Karakter.Stat.MagDef += (Data.ItemBase[item2.ID].Defans.MinMagDef + (item2.PlusValue * Data.ItemBase[item2.ID].Defans.MagDefINC));
                    Karakter.Stat.PhyDef += (Data.ItemBase[item2.ID].Defans.MinPhyDef + (item2.PlusValue * Data.ItemBase[item2.ID].Defans.PhyDefINC));
                }

                Karakter.Information.Item.sAmount = item2.Amount;
                Karakter.Information.Item.sID = item2.ID;
                client.Send(Private.Packet.PlayerStat(Karakter));

            }
            else if (item.Slot == 8 && Function.Items.CheckCape(item2.ID))
            {
                Karakter.Information.PvP = true;
            }
            else if (item.Slot >= 9 && item.Slot < 13)
            {
                if (item.ID != 0)
                {
                    Karakter.Stat.mag_Absorb -= (short)((Data.ItemBase[item.ID].Defans.MagAbsorb + (item.PlusValue * Data.ItemBase[item.ID].Defans.AbsorbINC)) * 10);
                    Karakter.Stat.phy_Absorb -= (short)((Data.ItemBase[item.ID].Defans.PhyAbsorb + (item.PlusValue * Data.ItemBase[item.ID].Defans.AbsorbINC)) * 10);
                }
                Karakter.Stat.mag_Absorb += (short)((Data.ItemBase[item2.ID].Defans.MagAbsorb + (item2.PlusValue * Data.ItemBase[item2.ID].Defans.AbsorbINC)) * 10);
                Karakter.Stat.phy_Absorb += (short)((Data.ItemBase[item2.ID].Defans.PhyAbsorb + (item2.PlusValue * Data.ItemBase[item2.ID].Defans.AbsorbINC)) * 10);
            }

            Send(Private.Packet.MoveItemEnquipEffect(Karakter.Information.UniqueID, item.Slot, item2.ID, item2.PlusValue));
        }
        public static bool CheckItemLevel(byte level, int itemID)
        {
            bool bln = false;
            if (Data.ItemBase[itemID].Level <= level) bln = true;
            return bln;
        }

        public static bool CheckGender(int my_gender_db, int itemID)
        {
            byte my_gender = 0;
            bool bln_gender = false;

            if (my_gender_db >= 1907 && my_gender_db <= 1919)my_gender = 1;
            if (my_gender_db >= 1920 && my_gender_db <= 1932)my_gender = 0;

            if (my_gender == Data.ItemBase[itemID].Gender) bln_gender = true; else bln_gender = false;

            if (Data.ItemBase[itemID].Gender == 2) bln_gender = true;

            return bln_gender;
        }
        void ItemAvatarUnEquip(byte fromSlot,byte toSlot)
        {
            Global.slotItem toItem = GetItem((uint)Karakter.Information.CharacterID, toSlot, false);
            int avatarid = 0;
            int dbID = 0;

            if (toItem.ID != 0) toSlot = GetFreeSlot();
            if (toSlot <= 12) return;

            MsSQL ms = new MsSQL("SELECT * FROM char_items WHERE itemnumber='avatar" + fromSlot + "' AND owner='" + Karakter.Information.CharacterID + "' AND inAvatar='1'");
            using (System.Data.SqlClient.SqlDataReader reader = ms.Read())
            {
                while (reader.Read())
                {
                    avatarid = reader.GetInt32(2);
                    dbID = reader.GetInt32(0);
                }
            }
            ms.Close();

            client.Send(Private.Packet.MoveItem(35, fromSlot, toSlot, 1));
            Send(Private.Packet.MoveItemUnequipEffect(Karakter.Information.UniqueID, fromSlot, avatarid));

            string nonquery = "UPDATE char_items SET itemnumber='item" + toSlot + "',slot='" + toSlot + "',inAvatar='0',type='0' WHERE owner='" + Karakter.Information.CharacterID + "' AND itemnumber='avatar" + fromSlot + "' AND id='" + dbID +"'";
            MsSQL.InsertData(nonquery);

        }
        void ItemAvatarEquip(byte fromSlot, byte toSlot)
        {
            Global.slotItem toItem = new Game.Global.slotItem();
            Global.slotItem fromItem = GetItem((uint)Karakter.Information.CharacterID, fromSlot, false);

            MsSQL ms = new MsSQL("SELECT * FROM char_items WHERE itemnumber='avatar" + toSlot + "' AND owner='" + Karakter.Information.CharacterID + "'");
            using (System.Data.SqlClient.SqlDataReader reader = ms.Read())
            {
                while (reader.Read())
                {
                    toItem.ID = reader.GetInt32(2);
                    toItem.dbID = reader.GetInt32(0);
                }
            }
            ms.Close();

            if (toItem.ID != 0) return;

            if (fromItem.ID == 0) return;
            if (!CheckGender(Karakter.Information.Model, fromItem.ID))
            {
                //client.Send(Private.Packet.MoveItemError(0x10));
                return;
            }


            else
            {
                string nonquery = "UPDATE char_items SET itemnumber='avatar" + toSlot + "',inAvatar='1',slot='" + toSlot + "',type='3' WHERE owner='" + Karakter.Information.CharacterID + "' AND itemnumber='item" + fromSlot + "' AND id='" + fromItem.dbID + "'";
                MsSQL.InsertData(nonquery);
            }
            Send(Private.Packet.MoveItemEnquipEffect(Karakter.Information.UniqueID, toSlot, fromItem.ID, 0));
            client.Send(Private.Packet.MoveItem(36, fromSlot, toSlot, 1));
        }
        byte GetItemType(int id)
        {
            if (Data.ItemBase[id].Class_D == 1) return 0;
            else if (Data.ItemBase[id].Class_D == 3) return 1;

            return 255;
        }
        void ItemUpdateAmount(Global.slotItem sItem, int owner)
        {
            if (sItem.Amount <= 0)
                MsSQL.UpdateData("delete from char_items where slot='" + sItem.Slot + "' AND owner='" + owner + "'");
            else
                MsSQL.InsertData("UPDATE char_items SET quantity='" + Math.Abs(sItem.Amount) + "' WHERE slot='" + sItem.Slot + "' AND owner='" + owner + "'");
                //client.Send(Private.Packet.ItemDelete(sItem.ID));

            client.Send(Private.Packet.ItemUpdate_Quantity(sItem.Slot, sItem.Amount));
        }
        bool ItemCheckArrow()
        {
            MsSQL ms = new MsSQL("SELECT * FROM char_items WHERE owner='" + Karakter.Information.CharacterID + "' AND slot >= '13' AND slot <= '" + 44 + "' AND inavatar='0' AND itemid='62'"); //OR itemid='3823'
            if (ms.Count() == 0)
            {
                ms = new MsSQL("SELECT * FROM char_items WHERE owner='" + Karakter.Information.CharacterID + "' AND slot >= '13' AND slot <= '" + 44 + "' AND inavatar='0' AND itemid='3823'"); //OR itemid='3823'
            }
            if (ms.Count() == 0) return false;
            else
            {
                Global.slotItem items = null;
                using (System.Data.SqlClient.SqlDataReader reader = ms.Read())
                {
                    while (reader.Read())
                        items = ConvertToItem(reader.GetInt32(2), reader.GetByte(5), reader.GetInt16(7), 1);
                }
                ms.Close();
                MsSQL.InsertData("UPDATE char_items SET itemnumber='item" + 7 + "',slot='" + 7 + "' WHERE itemnumber='" + "item" + items.Slot + "' AND owner='" + Karakter.Information.CharacterID + "' AND itemid='" + items.ID + "'");
                client.Send(Private.Packet.MoveItem(0, items.Slot, 7, items.Amount));

                Karakter.Information.Item.sAmount = items.Amount;
                Karakter.Information.Item.sID = items.ID;
;
                return true;
            }
        }
        Global.slotItem ConvertToItem(int id, byte slots, short amount, byte index)
        {
            Global.slotItem slot = new Game.Global.slotItem();
            slot.ID = id;
            slot.Slot = slots;
            slot.Amount = amount;
            return slot;
        }
        void Player_DropItem(byte slot)
        {
            try
            {
                Global.slotItem item = GetItem((uint)Karakter.Information.CharacterID, slot, false);
                if (item.ID < 4 || CheckItemMall(item.ID)) return;

                world_item sitem = new world_item();
                sitem.Model = item.ID;
                sitem.Ids = new Global.ID(Global.ID.IDS.World);
                sitem.UniqueID = sitem.Ids.GetUniqueID;
                sitem.amount = item.Amount;
                sitem.PlusValue = item.PlusValue;
                sitem.x = Karakter.Position.x;
                sitem.z = Karakter.Position.z;
                sitem.y = Karakter.Position.y;
                Systems.aRound(ref sitem.x, ref sitem.y, 1);
                sitem.xSec = Karakter.Position.xSec;
                sitem.ySec = Karakter.Position.ySec;

                byte type = GetItemType(item.ID);

                if (type == 0)
                    sitem.Type = 2;
                else if (type == 1)
                    sitem.Type = 3;

                sitem.fromType = 6;
                sitem.fromOwner = Karakter.Information.UniqueID;
                sitem.downType = false;
                sitem.Owner = 0;
                //A8330000 00 E4222300 4B62 F5B45844 75D131C3 BD178E443CF7 01 FFFFFFFF 0106 F45E3300

                sitem.Send(Public.Packet.ObjectSpawn(sitem), true);
                Systems.WorldItem.Add(sitem);

                client.Send(Private.Packet.ItemDelete(sitem.UniqueID));
                client.Send(Private.Packet.ItemDelete2(7, slot));

                MsSQL.UpdateData("delete from char_items where itemnumber='item" + slot + "' AND owner='" + Karakter.Information.CharacterID + "'");

            }
            catch (Exception ex)
            {
                Console.WriteLine("Player_DropItem({0})::error", slot);
                deBug.Write(ex);
            }
        }
        void Player_DropGold(ulong Gold)
        {
            try
            {
                world_item item = new world_item();
                item.amount = (int)Gold;
                item.Model = 1;
                if (item.amount < 1000) item.Model = 1;
                else if (item.amount > 1000 && item.amount < 10000) item.Model = 2;
                else if (item.amount > 10000) item.Model = 3;
                item.Ids = new Global.ID(Global.ID.IDS.World);
                item.UniqueID = item.Ids.GetUniqueID;
                item.x = Karakter.Position.x;
                item.z = Karakter.Position.z;
                item.y = Karakter.Position.y;
                Systems.aRound(ref item.x, ref item.y, 1);
                item.xSec = Karakter.Position.xSec;
                item.ySec = Karakter.Position.ySec;
                item.Type = 1;
                item.fromType = 6;
                item.Owner = 0;
                Systems.WorldItem.Add(item);
                item.Send(Public.Packet.ObjectSpawn(item), true);
                Karakter.Information.Gold -= (long)Gold;
                client.Send(Private.Packet.InfoUpdate(1, (int)Karakter.Information.Gold, 0));
                client.Send(Private.Packet.ItemDelete(0x0A, (long)Gold));
                SaveGold();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Player_DropGold({0})::error", Gold);
                deBug.Write(ex);
            }
        }
        void Player_BuyItem(byte shopLine, byte itemLine, short amount, int o_id)
        {
            try
            {
                obj o = GetObject(o_id);
                string[] ch = Data.ObjectBase[o.ID].Name.Split('_');
                if (ch[2] == "WAREHOUSE" || ch[2] == "ARENA") return;
                string s = Data.ObjectBase[o.ID].Tab[shopLine];
                Global.shop_data shopi = Global.shop_data.GetShopIndex(s);

                byte max = 1;
                short items = (short)Data.ItemBase[Global.item_database.GetItem(shopi.Item[itemLine])].ID;
                byte type = GetItemType(items);
                int gold = Data.ItemBase[items].Shop_price;
                if (type == 0)
                {
                    max = (byte)amount;
                    amount = 1;
                }
                else if (type == 1)
                {
                    gold *= amount;
                }
                string[] split = Data.ItemBase[items].Name.Split('_');
                if (split[1] == "EU") return;
                    
                for (byte i = 1; i <= max; i++)
                {
                    byte slot = GetFreeSlot();
                    if (slot <= 12) return;
                    if (Karakter.Information.Gold >= gold)
                    {
                        Karakter.Information.Gold -= gold;
                        client.Send(Private.Packet.UpdateGold(Karakter.Information.Gold));
                        SaveGold();
                    }
                    else return;

                    client.Send(Private.Packet.MoveItemBuy(8, shopLine, itemLine, max, slot, amount));
                    
                    if(type == 1)
                        AddItem(items, amount, slot, type, Karakter.Information.CharacterID);
                    else if(type == 0)
                        AddItem(items, 0, slot, type, Karakter.Information.CharacterID);
                }
            }
            catch (Exception ex)
            {
                Print.Format("Player_BuyItem({0},{1},{2},{3})::Error..", shopLine, itemLine, amount, o_id);
                deBug.Write(ex);
            }
        }
        void Player_SellItem(byte slot, short amount, int o_id)
        {
            try
            {
                Global.slotItem item = GetItem((uint)Karakter.Information.CharacterID, slot, false);

                Karakter.Information.Gold += Data.ItemBase[item.ID].Sell_Price * amount;
                client.Send(Private.Packet.UpdateGold(Karakter.Information.Gold));
                SaveGold();

                client.Send(Private.Packet.MoveItemSell(9, slot, amount, o_id));
                if (item.Amount != amount)
                {
                    int calc = (item.Amount - amount);
                    if (calc < 1) calc = 1;
                    MsSQL.UpdateData("UPDATE char_items SET quantity='" + calc + "' WHERE itemnumber='" + "item" + slot + "' AND owner='" + Karakter.Information.CharacterID + "' AND itemid='" + item.ID + "'");
                }
                else
                {
                    MsSQL.UpdateData("delete from char_items where itemnumber='item" + slot + "' AND owner='" + Karakter.Information.CharacterID + "'");
                }
                Karakter.Buy_Pack.Add(item);
            }
            catch (Exception ex)
            {
                Console.WriteLine("sell error::");
                deBug.Write(ex);
            }
        }
        void Player_BuyPack()
        {
            try
            {
                PacketReader Reader = new PacketReader(PacketInformation.buffer);

                int id = Reader.Int32(); byte b_slot = Reader.Byte();
                Reader.Close();
                byte i_slot = GetFreeSlot();
                if (i_slot <= 12) return;

                Print.Format(b_slot.ToString());

                Global.slotItem item = Karakter.Buy_Pack.Get(b_slot);
                if (item.Amount < 1) item.Amount = 1;
                Karakter.Information.Gold -= Data.ItemBase[item.ID].Sell_Price;
                client.Send(Private.Packet.UpdateGold(Karakter.Information.Gold));
                SaveGold();

                if (Data.ItemBase[item.ID].Class_D == 1)
                {
                    MsSQL.UpdateData("Insert Into char_items (itemid,plusvalue,durability,owner,itemnumber,slot,type) VALUES ('" + item.ID + "','" + item.PlusValue + "','" + Data.ItemBase[item.ID].Defans.Durability + "','" + Karakter.Information.CharacterID + "','item" + i_slot + "','" + i_slot + "','" + 0 + "' )");
                    client.Send(Private.Packet.MoveItemBuyGetBack(i_slot, b_slot, 1));
                }
                else if (Data.ItemBase[item.ID].Class_D == 3)
                {
                    MsSQL.UpdateData("Insert Into char_items (itemid,quantity,owner,itemnumber,slot,type) VALUES ('" + item.ID + "','" + item.Amount + "','" + Karakter.Information.CharacterID + "','item" + i_slot + "','" + i_slot + "','" + 1 + "' )");
                    client.Send(Private.Packet.MoveItemBuyGetBack(i_slot, b_slot, item.Amount));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("itembuypack::error");
                deBug.Write(ex);
            }
        }
        void Player_TakeGoldW(byte type, long gold)
        {
            try
            {
                Karakter.Account.StorageGold -= gold;
                Karakter.Information.Gold += gold;
                client.Send(Private.Packet.UpdateGold(Karakter.Information.Gold));
                client.Send(Private.Packet.MoveItemWareHouseGold(type, gold));
                SaveGold();
            }
            catch (Exception ex)
            {
                deBug.Write(ex);
                Console.WriteLine("Player_TakeGoldW({0},{1})::erorr..", type, gold);
            }
        }
        void Player_GiveGoldW(byte type, long gold)
        {
            try
            {
                Karakter.Account.StorageGold += gold;
                Karakter.Information.Gold -= gold;
                client.Send(Private.Packet.UpdateGold(Karakter.Information.Gold));
                client.Send(Private.Packet.MoveItemWareHouseGold(type, gold));
                SaveGold();
            }
            catch (Exception ex)
            {
                deBug.Write(ex);
                Console.WriteLine("Player_GiveGoldW({0},{1})::erorr..", type, gold);
            }
        }
        void Player_MoveItemToStorage(byte f_slot, byte t_slot, int o_id)
        {

            try
            {
                Global.slotItem item = GetItem((uint)Karakter.Information.CharacterID, f_slot, false);
                int price = Data.ItemBase[item.ID].Storage_price;
                if (item.Amount == 0) item.Amount = 1;
                price *= item.Amount;
                if (Karakter.Information.Gold >= price)
                {
                    Karakter.Information.Gold -= price;
                    client.Send(Private.Packet.UpdateGold(Karakter.Information.Gold));
                    SaveGold();
                }
                else return;

                MsSQL.UpdateData("delete from char_items where itemnumber='item" + f_slot + "' AND owner='" + Karakter.Information.CharacterID + "'");


                if (Data.ItemBase[item.ID].Class_D == 1)
                {
                    MsSQL.UpdateData("Insert Into storage_items (itemid,plusvalue,durability,owner,itemnumber,slot,type) VALUES ('" + item.ID + "','" + item.PlusValue + "','" + Data.ItemBase[item.ID].Defans.Durability + "','" + Player.ID + "','item" + t_slot + "','" + t_slot + "','" + 0 + "' )");
                }
                else if (Data.ItemBase[item.ID].Class_D == 3)
                {
                    MsSQL.UpdateData("Insert Into storage_items (itemid,quantity,owner,itemnumber,slot,type) VALUES ('" + item.ID + "','" + item.Amount + "','" + Player.ID + "','item" + t_slot + "','" + t_slot + "','" + 1 + "' )");
                }


                client.Send(Private.Packet.MoveItemStorage(2, f_slot, t_slot));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Player_MoveItemToStorage::Error..");
                deBug.Write(ex);
            }
        }
        void Player_MoveStorageItemToInv(byte f_slot, byte t_slot, int o_id)
        {
            try
            {
                Global.slotItem item = GetItem((uint)Player.ID, f_slot, true);
                t_slot = GetFreeSlot();
                if (t_slot <= 12) return;

                MsSQL.UpdateData("delete from storage_items where itemnumber='item" + f_slot + "' AND owner='" + Player.ID + "'");

                if (Data.ItemBase[item.ID].Class_D == 1)
                {
                    MsSQL.UpdateData("Insert Into char_items (itemid,plusvalue,durability,owner,itemnumber,slot,type) VALUES ('" + item.ID + "','" + item.PlusValue + "','" + Data.ItemBase[item.ID].Defans.Durability + "','" + Karakter.Information.CharacterID + "','item" + t_slot + "','" + t_slot + "','" + 0 + "' )");
                }
                else if (Data.ItemBase[item.ID].Class_D == 3)
                {
                    MsSQL.UpdateData("Insert Into char_items (itemid,quantity,owner,itemnumber,slot,type) VALUES ('" + item.ID + "','" + item.Amount + "','" + Karakter.Information.CharacterID + "','item" + t_slot + "','" + t_slot + "','" + 1 + "' )");
                }
                client.Send(Private.Packet.MoveItemStorage(3, f_slot, t_slot));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Player_MoveStorageItemToInv::Error..");
                deBug.Write(ex);
            }
        }
        void Player_BuyItemFromMall(byte type1, byte type2, byte type3, byte type4, byte type5, string itemname)
        {
            byte slot = GetFreeSlot();
            if (slot <= 12) return;

            itemname = itemname.Remove(0, 8);

            /*Print.Format(Decode.StringToPack(PacketInformation.buffer));
            Console.WriteLine(itemname + "  " + type1 + "  id + ");*/

            switch (itemname)
            {
                case "ITEM_MALL_HP_SUPERSET_2_BAG":
                case "ITEM_MALL_HP_SUPERSET_3_BAG":
                case "ITEM_MALL_HP_SUPERSET_4_BAG":
                case "ITEM_MALL_HP_SUPERSET_5_BAG":
                case "ITEM_MALL_MP_SUPERSET_2_BAG":
                case "ITEM_MALL_MP_SUPERSET_3_BAG":
                case "ITEM_MALL_MP_SUPERSET_4_BAG":
                case "ITEM_MALL_MP_SUPERSET_5_BAG":
                case "ITEM_MALL_QUIVER":
                case "ITEM_MALL_AVATAR_M_CH_WEDDING":
                case "ITEM_MALL_AVATAR_W_CH_WEDDING":
                case "ITEM_MALL_AVATAR_W_EU_WEDDING":
                case "ITEM_MALL_AVATAR_M_EU_WEDDING":
                case "ITEM_MALL_AVATAR_M_CH_WEDDING_HAT":
                case "ITEM_MALL_AVATAR_W_CH_WEDDING_HAT":
                case "ITEM_MALL_AVATAR_W_EU_WEDDING_HAT":
                case "ITEM_MALL_AVATAR_M_EU_WEDDING_HAT":
                case "ITEM_MALL_GLOBAL_CHATTING":
                case "ITEM_COS_C_TIGER_SCROLL":
                case "ITEM_COS_C_SCARABAEUS_SCROLL":
                case "ITEM_COS_C_PEGASUS_SCROLL":
                case "ITEM_COS_C_WILDPIG_SCROLL":
                case "ITEM_COS_C_OSTRICH_SCROLL":
                    int itemid = Global.item_database.GetItem(itemname);
                    if (Player.Silk < Data.ItemBase[itemid].Shop_price) return;
                    else Player.Silk -= Data.ItemBase[itemid].Shop_price;

                    MsSQL.UpdateData("UPDATE users SET silk='" + Player.Silk + "' WHERE id='" + Player.AccountName + "'");

                    client.Send(Private.Packet.BuyItemFromMall(type1, type2, type3, type4, type5, slot));
                    byte i_type = GetItemType(itemid);
                    if (i_type == 0)
                    {
                        AddItem(itemid, 0, slot, 0, Karakter.Information.CharacterID);
                    }
                    else if (i_type == 1)
                    {
                        int amount = Data.ItemBase[itemid].Max_Stack;
                        if (itemname == "ITEM_MALL_GLOBAL_CHATTING")
                            amount = 11;
                        else if (itemname == "ITEM_COS_C_TIGER_SCROLL" || itemname == "ITEM_COS_C_SCARABAEUS_SCROLL" || itemname == "ITEM_COS_C_PEGASUS_SCROLL" || itemname == "ITEM_COS_C_WILDPIG_SCROLL" || itemname == "ITEM_COS_C_OSTRICH_SCROLL")
                            amount = 1;
                        AddItem(itemid, (short)amount, slot, 1, Karakter.Information.CharacterID);
                    }
                    client.Send(Private.Packet.Silk(Player.Silk, 0, 0));
                    break;
                default:
                    client.Send(Private.Packet.MoveItemError(0x18, 1));
                    break;
            }
        }
        void ItemMoveInStorage(byte fromSlot, byte toSlot, short quantitly)
        {
            Global.slotItem fromItem = GetItem((uint)Karakter.Information.CharacterID, fromSlot, true);
            Global.slotItem toItem = GetItem((uint)Karakter.Information.CharacterID, toSlot, true);
            PacketWriter Writer = new PacketWriter();
            Writer.Create(SERVER_ITEM_MOVE);
            Writer.Bool(true);
            Writer.Byte(1);
            Writer.Byte(fromSlot);
            Writer.Byte(toSlot);
            Writer.Word(quantitly);
            //Writer.Byte(0);
            client.Send(Writer.GetBytes());

            MsSQL.UpdateData("UPDATE storage_items SET itemnumber='item" + toSlot + "',slot='" + toSlot + "' WHERE itemnumber='" + "item" + fromSlot + "' AND owner='" + Player.ID + "' AND itemid='" + fromItem.ID + "'");

            if (toItem.ID != 0) MsSQL.UpdateData("UPDATE storage_items SET itemnumber='item" + fromSlot + "',slot='" + fromSlot + "' WHERE itemnumber='" + "item" + toSlot + "' AND owner='" + Player.ID + "' AND itemid='" + toItem.ID + "' AND id='" + toItem.dbID + "'");
            
        }
        void ItemMoveToExhangePage(byte f_slot)
        {
            Systems sys = GetPlayer(Karakter.Network.TargetID);
            if (Karakter.Network.Exchange.ItemList.Count < 12 && sys.GetFreeSlotMax() > (byte)Karakter.Network.Exchange.ItemList.Count)
            {
                Global.slotItem newitem = GetItem((uint)Karakter.Information.CharacterID, f_slot, false);
                if (CheckItemMall(newitem.ID)) return; // for beta
                Karakter.Network.Exchange.ItemList.Add(newitem);

                client.Send(Private.Packet.Exchange_ItemPacket(this.Karakter.Information.UniqueID, this.Karakter.Network.Exchange.ItemList, true));

                //>D3C01E00 01 10 00000000 00 D3190000 0100
                client.Send(Private.Packet.Exchange_ItemSlot(4, f_slot));
            }
        }
        void ItemMoveExchangeToInv(byte f_slot)
        {
            Global.slotItem wannadeleteitem = Karakter.Network.Exchange.ItemList[f_slot];
            Karakter.Network.Exchange.ItemList.Remove(wannadeleteitem);

            client.Send(Private.Packet.Exchange_ItemPacket(this.Karakter.Information.UniqueID, this.Karakter.Network.Exchange.ItemList,true));
            client.Send(Private.Packet.Exchange_ItemSlot(5, f_slot));
        }
        void ItemExchangeGold(long gold)
        {
            client.Send(Private.Packet.ItemExchange_Gold(gold));
            Karakter.Network.Exchange.Gold = gold;
        }
        bool CheckItemMall(int id)
        {
            switch (Data.ItemBase[id].Name)
            {
                case "ITEM_MALL_HP_SUPERSET_2_BAG":
                case "ITEM_MALL_HP_SUPERSET_3_BAG":
                case "ITEM_MALL_HP_SUPERSET_4_BAG":
                case "ITEM_MALL_HP_SUPERSET_5_BAG":
                case "ITEM_MALL_MP_SUPERSET_2_BAG":
                case "ITEM_MALL_MP_SUPERSET_3_BAG":
                case "ITEM_MALL_MP_SUPERSET_4_BAG":
                case "ITEM_MALL_MP_SUPERSET_5_BAG":
                case "ITEM_MALL_QUIVER":
                case "ITEM_MALL_AVATAR_M_CH_WEDDING":
                case "ITEM_MALL_AVATAR_W_CH_WEDDING":
                case "ITEM_MALL_AVATAR_W_EU_WEDDING":
                case "ITEM_MALL_AVATAR_M_EU_WEDDING":
                case "ITEM_MALL_AVATAR_M_CH_WEDDING_HAT":
                case "ITEM_MALL_AVATAR_W_CH_WEDDING_HAT":
                case "ITEM_MALL_AVATAR_W_EU_WEDDING_HAT":
                case "ITEM_MALL_AVATAR_M_EU_WEDDING_HAT":
                case "ITEM_MALL_GLOBAL_CHATTING":
                case "ITEM_COS_C_TIGER_SCROLL":
                case "ITEM_COS_C_SCARABAEUS_SCROLL":
                case "ITEM_COS_C_PEGASUS_SCROLL":
                case "ITEM_COS_C_WILDPIG_SCROLL":
                case "ITEM_COS_C_OSTRICH_SCROLL":
                    return true;
            }
            return false;
        }
    }
}
