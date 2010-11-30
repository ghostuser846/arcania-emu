using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework;
namespace Game.Private
{
    public class Packet
    {
        public static byte[] LoadGame_1()
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_PATCH);
            Writer.Word(0x0101);
            Writer.Word(0x0500);
            Writer.Byte(0x20);
            return Writer.GetBytes();
        }
        public static byte[] LoadGame_2()
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_PATCH);
            Writer.Word(0x0100);
            Writer.Word(0x0100);
            Writer.Byte(0x69);
            Writer.Byte(0x0C);
            Writer.DWord(0x00000005);
            Writer.Byte(0x02);
            return Writer.GetBytes();
        }
        public static byte[] LoadGame_3()
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_PATCH);
            Writer.Word(0x0101);
            Writer.Word(0x0500);
            Writer.Byte(0x60);
            return Writer.GetBytes();
        }
        public static byte[] LoadGame_4()
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_PATCH);
            Writer.Word(0x0300);
            Writer.Word(0x0200);
            Writer.Word(0x0200);
            return Writer.GetBytes();
        }
        public static byte[] LoadGame_5()
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_PATCH);
            Writer.Word(0x0101);
            Writer.Word(0);
            Writer.Byte(0xA1);
            return Writer.GetBytes();
        }
        public static byte[] LoadGame_6()
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_PATCH);
            Writer.Word(0x0100);
            return Writer.GetBytes();
        }
        public static byte[] AgentServer()
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_AGENTSERVER);
            Writer.Text("AgentServer");
            Writer.Bool(true);
            return Writer.GetBytes();
        }
        public static byte[] ConnectSuccess()
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_CONNECTION);
            Writer.Bool(true);
            return Writer.GetBytes();
        }
        public static byte[] CharacterListing(string name)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_CHARACTERSCREEN);
            Writer.Byte(2);
            Writer.Byte(1);
            MsSQL ms = new MsSQL("SELECT TOP 4 * FROM karakterler WHERE account='" + name + "' AND deleted != '1'"); 
            Writer.Byte(ms.Count());
            using (System.Data.SqlClient.SqlDataReader reader = ms.Read())
            {
                while (reader.Read())
                {
                    Writer.DWord(reader.GetInt32(3));
                    Writer.Text(reader.GetString(2));
                    Writer.Word(0);
                    Writer.Byte(reader.GetByte(4));
                    Writer.Byte(reader.GetByte(5));
                    Writer.LWord(reader.GetInt64(12));
                    Writer.Word(reader.GetInt16(6));
                    Writer.Word(reader.GetInt16(7));
                    Writer.Word(reader.GetInt16(8));
                    Writer.DWord(reader.GetInt32(9));
                    Writer.DWord(reader.GetInt32(10));

                    TimeSpan ts = Convert.ToDateTime(reader.GetDateTime(43)) - DateTime.Now;
                    double time = ts.TotalMinutes;

                    if (Math.Round(time) > 0)
                    {
                        Writer.Byte(1);
                        Writer.DWord(Math.Round(time));
                    }
                    else
                    {
                        Writer.Byte(0);
                    }

                    if (Math.Round(time) <= 0 && Convert.ToDateTime("01.01.1900 00:00:00") != reader.GetDateTime(43))
                    {
                        MsSQL.UpdateData("UPDATE karakterler SET deleted='1' Where id='" + reader.GetInt32(0) + "'");
                    }

                    Writer.Word(0);
                    Writer.Byte(0);


                    //Writer.Byte((byte)MsSQL.GetRowsCount("select * from char_items where owner='" + reader.GetInt32(0) + "' AND slot >= '0' AND slot <= '" + 8 + "' AND type='0'"));
                    Function.Items.PrivateItemPacket(Writer, reader.GetInt32(0), 0, 8, 0);

                    //Writer.Byte((byte)MsSQL.GetRowsCount("select * from char_items where owner='" + reader.GetInt32(0) + "' AND slot >= '0' AND slot <= '" + 4 + "' AND type='3'"));
                    Function.Items.PrivateItemPacket(Writer, reader.GetInt32(0), 3, 5, 1);
                }
            }
            ms.Close();
            
            return Writer.GetBytes();
        }
        public static byte[] CharacterName(byte errocode)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_CHARACTERSCREEN);
            Writer.Byte(4);
            Writer.Byte(2);
            Writer.Byte(0x10);
            Writer.Byte(errocode);
            return Writer.GetBytes();
        }
        public static byte[] ScreenSuccess(byte type)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_CHARACTERSCREEN);
            Writer.Byte(type);
            Writer.Byte(1);
            return Writer.GetBytes();
        }
        public static byte[] LoginScreen()
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_LOGINSCREEN_ACCEPT);
            Writer.Byte(1);
            return Writer.GetBytes();
        }
        public static byte[] StartPlayerLoad()
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_STARTPLAYERDATA);
            return Writer.GetBytes();
        }
        public static byte[] EndPlayerLoad()
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_ENDPLAYERDATA);
            return Writer.GetBytes();
        }
        public static byte[] Load(character c)
        {
            PacketWriter Writer = new PacketWriter();
            //0A71129E
            //77070000 44 0505 C102000000000000 F3000000 F412000000000000 09000000
            //0000 04 00000000 85010000 03010000 01 00 0000 00000000 00 
            //2D 0701000000003B0E000000000000000000000023000000000100020004000000003C0E0000000000000000000000240000000001000200            //05000000003D0E00000000000000000000002500000000010002000600000000B3000000004F841448000000002E00000000010002000D00000000421D000001000E00000000431D000001000F000000008F4C00001300
            //05 00 00 
            //010101000000
            //010201000000
            //010301000000
            //011101000000
            //011201000005
            //011301000000
            //011401000000
           // uint t = Game.Global.RandomID.Add();
            Writer.Create(Systems.SERVER_PLAYERDATA);
            Writer.DWord(c.Ids.GetLoginID);
            Writer.DWord(c.Information.Model);
            Writer.Byte(c.Information.Volume);
            Writer.Byte(c.Information.Level);
            Writer.Byte(c.Information.Level);
            Writer.LWord(c.Information.XP);
            Writer.DWord(c.Information.SpBar);
            Writer.LWord(c.Information.Gold);
            Writer.DWord(c.Information.SkillPoint);
            Writer.Word(c.Information.Attributes);
            Writer.Byte(c.Information.BerserkBar);
            Writer.DWord(0);
            Writer.DWord(c.Stat.SecondHp);
            Writer.DWord(c.Stat.SecondMP);
            Writer.Bool(c.Information.Level < 20 ? true : false);
            Writer.Byte(0);
            Writer.Word(0);
            Writer.DWord(0);
            Writer.Byte(c.Information.Title);
            
            #region Item
            Writer.Byte(0x2D);


            MsSQL ms = new MsSQL("SELECT * FROM char_items WHERE owner='" + c.Information.CharacterID + "' AND slot >= '0' AND slot <= '" + 44 + "' AND inavatar='0'");
            Writer.Byte(ms.Count());
            using (System.Data.SqlClient.SqlDataReader msreader = ms.Read())
            {
                while (msreader.Read())
                {
                    short amount = msreader.GetInt16(7);

                    if (amount < 1) amount = 1;
                    MsSQL.InsertData("UPDATE char_items SET quantity='" + amount + "' WHERE owner='" + c.Information.CharacterID + "' AND itemid='" + msreader.GetInt32(2) + "' AND id='" + msreader.GetInt32(0) + "'");

                    if (msreader.GetByte(5) == 6) c.Information.Item.wID = Convert.ToInt32(msreader.GetInt32(2));
                    if (msreader.GetByte(5) == 7) { c.Information.Item.sID = msreader.GetInt32(2); c.Information.Item.sAmount = msreader.GetInt16(7); }
                    if (msreader.GetByte(5) == 8) if (msreader.GetInt32(2) != 0 && Function.Items.CheckCape(msreader.GetInt32(2))) c.Information.PvP = true;
                    

                    Item.AddItemPacket(Writer, msreader.GetByte(5), msreader.GetInt32(2), msreader.GetByte(6), msreader.GetByte(4), amount, msreader.GetInt32(8));
                }
            }
            ms.Close();

            //Avatar
            Writer.Byte(5);

            ms = new MsSQL("SELECT * FROM char_items WHERE owner='" + c.Information.CharacterID + "' AND slot >= '0' AND slot <= '" + 5 + "' AND type='3' AND inavatar='1'");
            
            Writer.Byte(ms.Count());
            using (System.Data.SqlClient.SqlDataReader msreader = ms.Read())
            {
                while (msreader.Read())
                {
                    Item.AddItemPacket(Writer, msreader.GetByte(5), msreader.GetInt32(2), msreader.GetByte(6), msreader.GetByte(4), msreader.GetInt16(7), msreader.GetInt32(8));
                }
            }
            ms.Close();

            Writer.Byte(0);

            #endregion

            for (byte i = 1; i <= 7; i++)
            {
                Writer.Bool(true);
                Writer.DWord(c.Stat.Skill.Mastery[i]);
                Writer.Byte(c.Stat.Skill.Mastery_Level[i]);
            }

            #region Skill
            Writer.Word(2);

            for (int i = 1; i <= c.Stat.Skill.AmountSkill; i++)
            {
                Writer.Byte(1);
                Writer.DWord(c.Stat.Skill.Skill[i]);
                Writer.Byte(1);
            }

            Writer.Byte(2);
            Writer.Word(3);
            Writer.DWord(1);
            #endregion
            //0200 016B00000001 02 0300 01000000 81 01 0000 8D010000 0000 9EBE2B00 A861 0E1C7544 9781DEBE 50C19D42 

            Writer.Byte(0x81);
            Writer.Byte(1);
            Writer.Word(0);

            Writer.Byte(0x8D);
            Writer.Byte(1);
            Writer.Word(0);

            #region Quest
            Writer.Byte(0);
            Writer.Byte(0);
            #endregion


            #region Diğer Veriler

            Writer.DWord(c.Information.UniqueID);

            Writer.Byte(c.Position.xSec);
            Writer.Byte(c.Position.ySec);
            Writer.Float(Function.Formule.packetx(c.Position.x, c.Position.xSec));
            Writer.Float(c.Position.z);
            Writer.Float(Function.Formule.packety(c.Position.y, c.Position.ySec));
            Writer.Word(0);							// Angle
            Writer.Byte(0);
            Writer.Byte(1);
            Writer.Byte(0);
            Writer.Word(0);							// Angle
            Writer.Word(0);
            //D422 00 01 00 D422 0000 00 00008041 00004842 0000C842 00[[0006]Bowerr]
            Writer.Bool(false); //berserk

            Writer.Float(c.Speed.WalkSpeed);
            Writer.Float(c.Speed.RunSpeed);
            Writer.Float(c.Speed.BerserkSpeed);
            //>03 01 421D0000 80510100 302A0000 00
            /*Writer.Byte(1);
            Writer.DWord(20010);
            Writer.DWord(100000);*/
            Writer.Byte(0); //ITEM_MALL_GOLD_TIME_SERVICE_TICKET_4W

            Writer.Text(c.Information.Name);
            //0000 00010000 00000000 00000000 00000000 00 FF D7 02 A0 03000000 00 E9791700 
            //00 07 01 01 49 6B000000 00000000 00000000 0100 0100 0000
            Writer.Word(0);							// Job alias
            Writer.DWord(0x00000100);
            Writer.DWord(0);
            Writer.DWord(0);
            Writer.DWord(0);

            Writer.Byte(0);

            Writer.Byte(0xFF);			// PK Flag (02: enabled, FF: disabled)

            Writer.Byte(c.LogNum++); //ilk giriş
            Writer.Byte(1);
            Writer.Byte(0);
            Writer.Byte(0);
            Writer.Byte(1);
            Writer.Byte(0);
            Writer.Byte(0);
            Writer.Byte(0);

            Writer.DWord(c.Account.ID);
            
            Writer.Byte(c.Information.GM);
            Writer.Byte(7);//

            #region Qucik Bar
            PacketReader reader = new PacketReader(System.IO.File.ReadAllBytes(Environment.CurrentDirectory + @"\player\info\quickbar\" + c.Information.Name + ".dat"));
            PlayerQuickBar(reader, Writer);
            reader = new PacketReader(System.IO.File.ReadAllBytes(Environment.CurrentDirectory + @"\player\info\autopot\" + c.Information.Name + ".dat"));
            PlayerAutoPot(reader, Writer);
            #endregion

            Writer.Byte(0);
            Writer.Word(1);
            Writer.Word(1);
            Writer.Byte(0);

            Writer.Byte(0);
            #endregion

            return Writer.GetBytes();
        }
        public static void PlayerAutoPot(PacketReader Reader, PacketWriter Writer)
        {
            Writer.Byte(Reader.Byte());
            Writer.Byte(Reader.Byte());
            Writer.Byte(Reader.Byte());
            Writer.Byte(Reader.Byte());
            Writer.Byte(Reader.Byte());
            Writer.Byte(Reader.Byte());
            Writer.Byte(Reader.Byte());
            Reader.Close();
        }
        public static void PlayerQuickBar(PacketReader Reader, PacketWriter Writer)
        {
            byte amm = 0;
            int[] skillid = new int[51];
            byte[] slotn = new byte[51];
            for (byte i = 0; i <= 50; i++)
            {
                slotn[i] = Reader.Byte();
                if (slotn[i] != 0)
                {
                    skillid[i] = Reader.Int32();
                    amm++;
                }
                else Reader.Skip(4);
            }
            Writer.Byte(amm);
            for (byte i = 0; i <= 50; i++)
            {
                if (slotn[i] != 0)
                {
                    Writer.Byte(i);
                    Writer.Byte(slotn[i]);
                    Writer.DWord(skillid[i]);
                }
            }
            Reader.Close();
        }
        public static byte[] PlayerStat(character c)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_PLAYERSTAT);
            Writer.DWord((int)c.Stat.MinPhyAttack);
            Writer.DWord((int)c.Stat.MaxPhyAttack);
            Writer.DWord((int)c.Stat.MinMagAttack);
            Writer.DWord((int)c.Stat.MaxMagAttack);
            Writer.Word((ushort)c.Stat.PhyDef);
            Writer.Word((ushort)c.Stat.MagDef);
            Writer.Word((ushort)c.Stat.Hit);
            Writer.Word((ushort)c.Stat.Parry);
            Writer.DWord((int)c.Stat.Hp);
            Writer.DWord((int)c.Stat.Mp);
            Writer.Word((ushort)c.Stat.Strength);
            Writer.Word((ushort)c.Stat.Intelligence);
            return Writer.GetBytes();
        }
        public static byte[] PlayerUnknowPack(int id)
        {
            TimeSpan ts = Systems.ServerStartedTime - DateTime.Now;
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_UNKNOWNPACK);
            Writer.DWord(id);
            Writer.DWord(Math.Abs(ts.TotalSeconds));

            return Writer.GetBytes();
        }
        public static byte[] StartingLeaveGame(byte time, byte type)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_LEAVE_ACCEPT);
            Writer.Byte(1);
            Writer.DWord(time);
            Writer.Byte(type);
            return Writer.GetBytes();
        }
        public static byte[] EndLeaveGame()
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_LEAVE_SUCCESS);
            return Writer.GetBytes();
        }
        public static byte[] CalcelLeaveGame()
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_LEAVE_CALCEL);
            Writer.Bool(true);
            return Writer.GetBytes();
        }
        public static byte[] MoveItem(byte type, byte fromSlot, byte toSlot, short quantity)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_ITEM_MOVE);
            Writer.Byte(1);
            Writer.Byte(type);
            Writer.Byte(fromSlot);
            Writer.Byte(toSlot);
            Writer.Word(quantity);
            Writer.Byte(0);
            return Writer.GetBytes();
        }
        public static byte[] MoveItemBuy(byte type,byte shopLine,byte itemLine,byte max, byte slot,short amount)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_ITEM_MOVE);
            Writer.Bool(true);
            Writer.Byte(type);
            Writer.Byte(shopLine);
            Writer.Byte(itemLine);
            Writer.Byte(1);
            Writer.Byte(slot);
            Writer.Word(amount);
            return Writer.GetBytes();
        }
        public static byte[] MoveItemSell(byte type, byte slot, short amount, int id)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_ITEM_MOVE);
            Writer.Bool(true);
            Writer.Byte(type);
            Writer.Byte(slot);
            Writer.Word(amount);
            Writer.DWord(id);
            Writer.Byte(1);
            return Writer.GetBytes();
        }
        public static byte[] MoveItemBuyGetBack(byte slot, byte b_slot, short amount)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_ITEM_MOVE);
            Writer.Bool(true);
            Writer.Byte(0x22);
            Writer.Byte(slot);
            Writer.Byte(b_slot);
            Writer.Word(amount);
            return Writer.GetBytes();
        }
        public static byte[] MoveItemWareHouseGold(byte type, long gold)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_ITEM_MOVE);
            Writer.Bool(true);
            Writer.Byte(type);
            Writer.LWord(gold);
            return Writer.GetBytes();
        }
        public static byte[] MoveItemError(byte action, byte type)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_ITEM_MOVE);
            Writer.Byte(2);
            Writer.Byte(action);
            Writer.Byte(type);
            return Writer.GetBytes();
        }
        public static byte[] MoveItemUnequipEffect(int id,byte Slot,int iid)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_ITEM_UN_EFFECT);
            Writer.DWord(id);
            Writer.Byte(Slot);
            Writer.DWord(iid);
            return Writer.GetBytes();
        }
        public static byte[] MoveItemEnquipEffect(int id,byte slot,int iid, byte plus)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_ITEM_EFFECT);
            Writer.DWord(id);
            Writer.Byte(slot);
            Writer.DWord(iid);
            Writer.Byte(plus);
            return Writer.GetBytes();
        }
        public static byte[] MoveItemStorage(byte type, byte f_slot, byte t_slot)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_ITEM_MOVE);
            Writer.Byte(1);
            Writer.Byte(type);
            Writer.Byte(f_slot);
            Writer.Byte(t_slot);
            return Writer.GetBytes();
        }
        public static byte[] BuyItemFromMall(byte type1, byte type2, byte type3, byte type4, byte type5, byte slot)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_ITEM_MOVE);
            Writer.Byte(1);
            Writer.Byte(0x18);
            Writer.Byte(type1);
            Writer.Byte(type2);
            Writer.Byte(type3);
            Writer.Byte(type4);
            Writer.Byte(type5);
            Writer.Byte(1);
            Writer.Byte(slot);
            Writer.Word(1);
            return Writer.GetBytes();
        }
        public static byte[] SelectObject(int id,int model, byte type, int hp)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_SELECT_OBJECT);
            Writer.Bool(true);
            Writer.DWord(id);
            switch (type)
            {
                case 1://monster
                    Writer.Byte(1);
                    Writer.DWord(hp);
                    Writer.Byte(1);
                    Writer.Byte(5);
                    break;
                case 2://npc
                    //015100000000020102001400
                    //S>C[B445]->01 18000000 00 04 01 02 04 20 00 1400
                    NPC.Chat(model, Writer);
                    break;
                case 3:
                    /*Writer.Byte(2);
                    Writer.Byte(7);
                    Writer.Byte(8);
                    Writer.Word(0);*/
                    //01 5B010000 00 00 00
                    //01 1D000000 00 02 01 02 00 1400
                    //01 93080000 00 03 01 04 20 00
                    //01 17000000 00 02 02 08 00
                    //01 0A000000 02 07081400
                    NPC.Chat(model, Writer);
                    break;
                    
                case 5://player
                    Writer.Byte(1);
                    Writer.Byte(5);
                    Writer.Byte(4);
                    break;
            }
            return Writer.GetBytes();
        }
        public static byte[] GM_MAKEITEM(byte type ,byte Slot, int id, short plus, int durability)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_ITEM_MOVE);
            Writer.Bool(true);
            Writer.Byte(6);
            Item.AddItemPacket(Writer, Slot, id, type, (byte)plus, plus, durability);

            return Writer.GetBytes();
        }
        public static byte[] AlchemyResponse(bool isSuccess, Global.slotItem sItem)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_ALCHEMY);
            Writer.Byte(1);
            Writer.Byte(2);
            Writer.Bool(isSuccess);
            Writer.Byte(sItem.Slot);
            if (!isSuccess) { Writer.Byte(0); }
            Writer.DWord(0);
            Writer.DWord(sItem.ID);
            Writer.Byte(sItem.PlusValue);
            Writer.LWord(0);
            Writer.DWord(sItem.Durability);
            Writer.Byte(0);
            Writer.Word(1);
            Writer.Word(2);
            //Writer.Word(3);

            return Writer.GetBytes();
        }

        public static byte[] AlchemyCancel()
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_ALCHEMY);
            Writer.Byte(1);
            Writer.Byte(1);
            
            return Writer.GetBytes();
        }
        public static byte[] Stallopenresponse(short stallnamelength, string stallname,int karakter)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_STALL_OPEN);
            Writer.DWord(karakter);
            Writer.Word(stallnamelength);
            Writer.String(stallname);
            Writer.DWord(0);

            return Writer.GetBytes();
        }
        public static byte[] Stallopened()
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_STALL_OPENED);
            Writer.Byte(1);
            return Writer.GetBytes();
        }
        public static byte[] Stallopened2(short welcomelength, string welcome)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_STALL_ADDITEM);
            Writer.Byte(1);
            Writer.Byte(6);
            Writer.Word(welcomelength);
            Writer.String(welcome);
            return Writer.GetBytes();
        }
        public static byte[] Stallitemadd(byte itemnum, byte invpos, int price)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_STALL_ADDITEM);
            Writer.Byte(1);
            Writer.Byte(2);
            Writer.Word(0);
            Writer.Byte(0);
            Writer.DWord(0);
            Writer.DWord(4);
            Writer.Byte(itemnum);
            Writer.Byte(invpos);
            Writer.Byte(price);
            Writer.Word(0xFF);
            return Writer.GetBytes();
        }
            
        public static byte[] Player_Emote(int id ,byte type)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_EMOTE);
            Writer.DWord(id);
            Writer.Byte(type);
            return Writer.GetBytes();
        }
        public static byte[] StatePack(int id, byte type1, byte type2, bool type3)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_CHANGE_STATUS);
            Writer.DWord(id);
            Writer.Byte(type1);
            Writer.Byte(type2);
            if (type1 == 4 && type2 == 1) Writer.Bool(type3); 
            return Writer.GetBytes();
        }
        public static byte[] TeleportStart()
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(0xB3AF);
            Writer.Byte(1);
            return Writer.GetBytes();
        }
        public static byte[] TeleportOtherStart()
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_TELEPORTOTHERSTART);
            Writer.DWord(0);
            return Writer.GetBytes();
        }
        public static byte[] TeleportImage(byte x, byte y)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(0x37D5);
            Writer.Byte(x);
            Writer.Byte(y);
            return Writer.GetBytes();
        }
        public static byte[] test()
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(0x3B9D);
            Writer.Byte(1);
            Writer.Byte(0xC8);
            return Writer.GetBytes();
        }
        public static byte[] Silk(int deger1, int deger2, int deger3)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_SILKPACK);
            Writer.DWord(deger1);
            Writer.DWord(deger2);
            Writer.DWord(deger3);
            return Writer.GetBytes();
        }
        public static byte[] Test1()
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(0x3288);
            Writer.Word(0);
            return Writer.GetBytes();
        }
        public static byte[] Test2()
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(0x37ED);
            Writer.Byte(0);
            return Writer.GetBytes();
        }
        public static byte[] ActionState(byte b1, byte b2)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_ACTIONSTATE);
            Writer.Byte(b1);
            Writer.Byte(b2);
            return Writer.GetBytes();
        }
        public static byte[] MasteryUpPacket(int mastery, byte level)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_MASTERY_UP);
            Writer.Bool(true);
            Writer.DWord(mastery);
            Writer.Byte(level);
            return Writer.GetBytes();
        }
        public static byte[] InfoUpdate(byte type, int obje, byte bT)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_INFO_UPDATE);
            //01 4F0E0000 00000000 00
            //04 00000000 00
            Writer.Byte(type);
            switch (type)
            {
                case 1:
                    Writer.LWord(obje);
                    Writer.Byte(0);
                    break;
                case 2:
                    Writer.DWord(obje);
                    Writer.Byte(0);
                    break;
                case 4:
                    Writer.Byte(bT);
                    Writer.DWord(obje);
                    break;
                    
            }

            return Writer.GetBytes();
        }
        public static byte[] SkillUpdate(int skillid)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_SKILL_UPDATE);
            Writer.Byte(1);
            Writer.DWord(skillid);
            return Writer.GetBytes();
        }
        public static byte[] Arrow(short amount)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_ARROW_UPDATE);
            Writer.Word(amount);
            return Writer.GetBytes();
        }
        public static byte[] UpdateGold(long gold)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_UPDATEGOLD);
            Writer.Byte(1);
            Writer.LWord(gold);
            Writer.Byte(0);
            return Writer.GetBytes();
        }
        public static byte[] UpdatePlayer(int objectid, ushort packetcode, byte type, int prob)
        {
            //F45E3300 3000 02 28060000
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_PLAYER_STATE_UPDATE);
            Writer.DWord(objectid);
            Writer.Word(packetcode);
            Writer.Byte(type);
            Writer.DWord(prob);
            return Writer.GetBytes();
        }
        public static byte[] UpdateStr()
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_PLAYER_UPDATE_STR);
            Writer.Bool(true);
            return Writer.GetBytes();
        }
        public static byte[] UpdateInt()
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_PLAYER_UPDATE_INT);
            Writer.Bool(true);
            return Writer.GetBytes();
        }
        public static byte[] ItemDelete(int id)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_ITEM_DELETE);
            Writer.DWord(id);
            return Writer.GetBytes();
        }
        public static byte[] ItemDelete2(byte type, byte slot)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_ITEM_MOVE);
            Writer.Byte(1);
            Writer.Byte(type);
            Writer.Byte(slot);
            if(type == 0xF) Writer.Byte(4);
            return Writer.GetBytes();
        }
        public static byte[] ItemDelete(byte type, long gold)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_ITEM_MOVE);
            Writer.Byte(1);
            Writer.Byte(type);
            Writer.LWord(gold);
            //01 0A 07000000 00000000
            return Writer.GetBytes();
        }
        public static byte[] CloseNPC()
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_CLOSE_NPC);
            Writer.Bool(true);
            return Writer.GetBytes();
        }
        public static byte[] OpenNPC(byte type)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_OPEN_NPC);
            Writer.Bool(true);
            Writer.Byte(type);
            return Writer.GetBytes();
        }
        public static byte[] OpenWarehouse(long t)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_OPEN_WAREHOUSE);
            Writer.LWord(t);
            return Writer.GetBytes();
        }
        public static byte[] OpenWarehouse2(byte t, player c)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_OPEN_WAREPROB);
            Writer.Byte(t);

            MsSQL ms = new MsSQL("SELECT * FROM storage_items WHERE owner='" + c.ID + "'");

            Writer.Byte(ms.Count());
            using (System.Data.SqlClient.SqlDataReader reader = ms.Read())
            {
                while (reader.Read())
                    Item.AddItemPacket(Writer, reader.GetByte(5), reader.GetInt32(2), reader.GetByte(6), reader.GetByte(4), reader.GetInt16(7), reader.GetInt32(8));
            }
            ms.Close();
            return Writer.GetBytes();
        }
        public static byte[] OpenWarehouse3()
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_OPEN_WAREHOUSE2);
            return Writer.GetBytes();
        }
        public static byte[] UpdatePlace()
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_SAVE_PLACE);
            Writer.Byte(1);
            return Writer.GetBytes();
        }
        public static byte[] Exchange_ItemPacket(int id, List<Global.slotItem> Exhange, bool mine)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_EXCHANGE_ITEM);
            Writer.DWord(id);
            Writer.Byte(Exhange.Count);
            //>87502000 03 
            //12 0000000000 07000000 3000 
            //11 0100000000 07000000 3000
            //13 0200000000 5A080000 2E00
            //3CB71E00 01 
            //0000000000 D3190000 0100
            //opCodes.Client.Player.CHARACTERSCREEN
            for (byte i = 0; i < Exhange.Count; i++)
            {
                if(mine) Writer.Byte(Exhange[i].Slot);
                Writer.Byte(i);
                Writer.DWord(0);
                Writer.DWord(Exhange[i].ID);
                switch ((byte)Exhange[i].Type)
                {
                    case 0:
                    case 3:
                        Writer.Byte(Exhange[i].PlusValue);
                        Writer.LWord(0);
                        Writer.DWord(Data.ItemBase[Exhange[i].ID].Defans.Durability);
                        Writer.Byte(0);
                        Writer.Word(1);
                        Writer.Word(2);
                        break;
                    case 1:
                        Writer.Word(Exhange[i].Amount);
                        break;
                    case 2:
                        Writer.Byte(0);
                        Writer.Word(1);
                        break;
                }
            }
            return Writer.GetBytes();
        }
        public static byte[] Exchange_ItemSlot(byte type , byte slot)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_ITEM_MOVE);
            Writer.Byte(1);
            Writer.Byte(type);
            Writer.Byte(slot);
            if(type == 4) Writer.Byte(0);
            
            return Writer.GetBytes();
        }
        public static byte[] Exchange_Accept()
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_EXCHANGE_ACCEPT);
            Writer.Byte(1);
            return Writer.GetBytes();
        }
        public static byte[] Exchange_Accept2()
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_EXCHANGE_ACCEPT2);
            return Writer.GetBytes();
        }
        public static byte[] Exchange_Gold(long gold)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_EXCHANGE_GOLD);
            Writer.Byte(2);
            Writer.LWord(gold);
            return Writer.GetBytes();
        }
        public static byte[] Exchange_Approve()
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_EXCHANGE_APPROVE);
            Writer.Byte(1);
            return Writer.GetBytes();
        }
        public static byte[] Exchange_Finish()
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_EXCHANGE_FINISHED);
            return Writer.GetBytes();
        }
        public static byte[] Exchange_Cancel()
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_EXCHANGE_CANCEL);
            Writer.Byte(0x2C);
            Writer.Byte(0x18);
            return Writer.GetBytes();
        }
        public static byte[] ItemExchange_Gold(long gold)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_ITEM_MOVE);
            Writer.Byte(1);
            Writer.Byte(13);
            Writer.LWord(gold);
            return Writer.GetBytes();
        }

        public static byte[] ItemUpdate_Quantity(byte slot, short amount)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_ITEM_QUANTITY_UPDATE);
            Writer.Byte(slot);
            Writer.Byte(8);
            Writer.Word(amount);
            return Writer.GetBytes();
        }

    }
    class Item
    {
        public static void AddItemPacket(PacketWriter Writer, byte slot, int id,byte type, byte plus,short amount, int durability)
        {
            Writer.Byte(slot); // slot
            Writer.DWord(0); //idk
            Writer.DWord(id); //item id
            switch ((byte)type)
            {
                case 0:
                case 3:
                    Writer.Byte(plus);
                    Writer.LWord(0);
                    Writer.DWord(durability);
                    Writer.Byte(0);
                    Writer.Word(1);
                    Writer.Word(2);
                    break;
                case 1:
                    Writer.Word(amount);
                    break;
                case 2:
                    Writer.Byte(0);
                    Writer.Word(1);
                    break;
                case 4:
                    Writer.Byte(0); // actived 2
                    Writer.Byte(2); // level
                    Writer.Byte(24);
                    Writer.Word(0);
                    Writer.Word(0);
                    Writer.Byte(0);
                    break;
            }
            //1D 00000000 401D0000 03 23180000 000000  1E 0000000009 000000070020000000000F000000320021000000000F0000
        }
        
    }
}
