using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework;

//Itt a szerver válaszai vannak a kliens kérelmeire.
namespace Game.Public
{
    public class Packet
    {
        public static byte[] Movement(Game.Global.vektor p)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_MOVEMENT);
            Writer.DWord(p.ID);
            Writer.Bool(true);
            Writer.Byte(p.xSec);
            Writer.Byte(p.ySec);
            Writer.Word((ushort)p.x);
            Writer.Word((ushort)p.z);
            Writer.Word((ushort)p.y);
            Writer.Bool(false);
            //Writer.Bool(false);
            return Writer.GetBytes();
        }
        public static byte[] MovementOnPickup(Game.Global.vektor p)
        {
            //0EA14001 6A6B 159EDD44 00003443 AB42C242 F2FE
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_PICKUPITEM_MOVE);
            Writer.DWord(p.ID);
            Writer.Byte(p.xSec);
            Writer.Byte(p.ySec);
            Writer.Float(p.x);
            Writer.Float(p.z);
            Writer.Float(p.y);
            Writer.Word(0);
            return Writer.GetBytes();
        }
        public static byte[] Pickup_egilme(int id, byte sec)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_PICKUPITEM_EGILME);
            Writer.DWord(id);
            Writer.Byte(sec);
            //0EA14001 00
            return Writer.GetBytes();
        }
        public static byte[] Pet_Information(int id, int model, int hp)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_PET_INFORMATION);
            //E67D0300 90080000 F0090000 F0090000 00
            Writer.DWord(id);
            Writer.DWord(model);
            Writer.DWord(hp);
            Writer.DWord(hp);
            Writer.Byte(0);
            return Writer.GetBytes();
        }
        public static byte[] Player_UpToHorse(int ownerID, bool type, int petID)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_PLAYER_UPTOHORSE);
            //01 5B5C0300 01 E67D0300
            //01 87502000 00 94532000
            Writer.Byte(1);
            Writer.DWord(ownerID);
            Writer.Byte(type);
            Writer.DWord(petID);
            return Writer.GetBytes();
        }
        public static byte[] Player_DownToHorse(int petid)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_PLAYER_DESPAWN_PET);
            Writer.DWord(petid);
            Writer.Byte(1);
            return Writer.GetBytes();
        }
        public static byte[] ObjectSpawn(pet_obj o)
        {
            //90080000 E67D0300 
            //875D 00408944 C4A31243 00401844 893A 00 01 00 893A 01 0000 
            //00003442 0000BE42 0000C842 0000 01

            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_SOLO_SPAWN);

            Writer.DWord(o.Model);
            Writer.DWord(o.UniqueID);
            Writer.Byte(o.xSec);
            Writer.Byte(o.ySec);
            Writer.Float(Function.Formule.packetx((float)o.x, o.xSec));
            Writer.Float(o.z);
            Writer.Float(Function.Formule.packety((float)o.y, o.ySec));

            Writer.Word(0); //angle
            Writer.Byte(0);
            Writer.Byte(1);
            Writer.Byte(0);
            Writer.Word(0); //angle

            Writer.Byte(1);
            Writer.Word(0);
            Writer.Float(o.Walk);	// Walk speed
            Writer.Float(o.Run);	// Run speed
            Writer.Float(o.Zerk);	// Berserk speed
            Writer.Word(0);
            Writer.Byte(1);
            return Writer.GetBytes();
        }
        public static byte[] ObjectSpawn(obj o)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_SOLO_SPAWN);

            Writer.DWord(o.ID);
            Writer.DWord(o.UniqueID);
            Writer.Byte(o.xSec);
            Writer.Byte(o.ySec);
            Writer.Float(Function.Formule.packetx((float)o.x, o.xSec));
            Writer.Float(o.z);
            Writer.Float(Function.Formule.packety((float)o.y, o.ySec));
            if (o.LocalType == 1)
            {
                Writer.Word(0);
                Writer.Word(1);

                Writer.Byte(o.xSec);
                Writer.Byte(o.ySec);
                Writer.Word(Function.Formule.packetx((float)o.x, o.xSec));
                Writer.Word(o.z);
                Writer.Word(Function.Formule.packety((float)o.y, o.ySec));
                Writer.Byte(1);
                Writer.Byte(2);
                Writer.Byte(0);
                Writer.Float(10);	// Walk speed
                Writer.Float(50);	// Run speed
                Writer.Float(100);	// Berserk speed
                Writer.Byte(0);
                Writer.Byte(2);
                Writer.Byte(1);
                Writer.Byte(5);
                Writer.Byte(o.Type);
                Writer.Byte(4);
            }
            if (o.LocalType == 2)
            {
                Writer.Word(0);
                Writer.Byte(0);
                Writer.Byte(1);

                Writer.DWord(0x00000001);
                Writer.LWord(0);
                Writer.Word(0);
                Writer.Float(10);	// Walk speed
                Writer.Byte(0);
                Writer.Word(0x0402);
                Writer.Word(0x0201);
                Writer.Word(0x2004);
                Writer.Byte(4);
            }
            if (o.LocalType == 3)
            {
                Writer.Word(0);
                Writer.Byte(4);
            }
            return Writer.GetBytes();
        }
        /// <summary>
        /// Objektum Spawnolása
        /// </summary>
        /// <param name="w">Egy world_item példányt vár.</param>
        /// <returns>asd</returns>
        public static byte[] ObjectSpawn(world_item w)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_SOLO_SPAWN);
            Writer.DWord(w.Model);

            if (w.Type == 1)
            {
                Writer.DWord(w.amount);
                Writer.DWord(w.UniqueID);
                Writer.Byte(w.xSec);
                Writer.Byte(w.ySec);
                Writer.Float(Function.Formule.packetx((float)w.x, w.xSec));
                Writer.Float(w.z);
                Writer.Float(Function.Formule.packety((float)w.y, w.ySec));
                Writer.DWord(0);
                Writer.Byte(w.fromType);
                Writer.DWord(0);
            }
            else if (w.Type == 2)
            {
                //A8330000 00 E4222300 4B62 F5B45844 75D131C3 BD178E44 3CF7 01 FFFFFFFF 01 06 F45E3300
                //A8330000 00 E4222300 4B62 F5B45844 75D131C3 BD178E44 3CF7 01 FFFFFFFF 01 06 F45E3300
                //CB030000 00 EBD82700 AA62 6DB25544 BF8E4441 E566D643 723D 01 E9791700 00 05 E8D82700
                Writer.Byte(w.PlusValue);
                Writer.DWord(w.UniqueID);

                Writer.Byte(w.xSec);
                Writer.Byte(w.ySec);
                Writer.Float(Function.Formule.packetx((float)w.x, w.xSec));
                Writer.Float(w.z);
                Writer.Float(Function.Formule.packety((float)w.y, w.ySec));
                Writer.Word(0);

                //Writer.Byte(0);
                Writer.Bool(w.downType);
                if(w.downType)
                    Writer.DWord(w.Owner);

                Writer.Byte(0);
                Writer.Byte(w.fromType);
                Writer.DWord(w.fromOwner);
            }
            else if (w.Type == 3)
            {
                //BE1B0000 02422600 AA62 F2FC5144 C7D40241 7FCF9643 6CD8 01 E9791700 00 05 01422600
                //9F190000 7ED92700 AA62 36C67944 A4285FC1 86A4CC43 DAA1 01 E9791700 00 05 7DD92700
                Writer.DWord(w.UniqueID);
                Writer.Byte(w.xSec);
                Writer.Byte(w.ySec);
                Writer.Float(Function.Formule.packetx((float)w.x, w.xSec));
                Writer.Float(w.z);
                Writer.Float(Function.Formule.packety((float)w.y, w.ySec));
                Writer.Word(0);

                Writer.Bool(w.downType);
                if(w.downType) Writer.DWord(w.Owner);

                Writer.Byte(0);
                Writer.Byte(w.fromType);
                Writer.DWord(w.fromOwner);
            }
            return Writer.GetBytes();
        }
        public static byte[] ObjectSpawn(character c)
        {
            //A0380000
            //34 00 03 2D 04 
            //E22B000000 
            //E32B000000
            //E42B000000
            //FF29000000
            //05 00 00 3A2B3500 
            //4F69
            //C400F243 C3D1A042 F61CA044 
            //2B6E 00 0101 2B6E 010300000000420000C8420000C842019779000003B9000008004142444F3130323000010000000000000000000000000000000000000000000000000000000000FF04
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_SOLO_SPAWN);

            Writer.DWord(c.Information.Model);
            Writer.Byte(c.Information.Volume);
            Writer.Byte(c.Information.Title); //title
            Writer.Bool((c.Information.Level < 20 ? true : false));

            Writer.Byte(0x2D);

            //Writer.Byte((byte)MsSQL.GetRowsCount("select itemid from char_items where owner='" + c.Information.CharacterID + "' AND slot >= '0' AND slot <= '" + 8 + "' AND type='0'"));
            Function.Items.PrivateItemPacket(Writer, c.Information.CharacterID, 0 , 8, 0);

            Writer.Byte(5);

            //Writer.Byte((byte)MsSQL.GetRowsCount("select itemid from char_items where owner='" + c.Information.CharacterID + "' AND slot >= '0' AND slot <= '" + 4 + "' AND type='3' AND inAvatar='1'"));
            Function.Items.PrivateItemPacket(Writer, c.Information.CharacterID, 3, 5, 1);

            Writer.Byte(0);

            Writer.DWord(c.Information.UniqueID);

            Writer.Byte(c.Position.xSec);
            Writer.Byte(c.Position.ySec);
            Writer.Float(Function.Formule.packetx(c.Position.x, c.Position.xSec));
            Writer.Float(c.Position.z);
            Writer.Float(Function.Formule.packety(c.Position.y, c.Position.ySec));
            //D156 0101 4F69 B101 4F00 4C05 010300000000420000C8420000C842019779000003B9000008004142444F3130323000010000000000000000000000000000000000000000000000000000000000FF04
            //A861 ECD5C744 EB08CCB8 24A41344 066F 01 01 A861 9B05 0000 9702 010000CDCC0C420000DC420000C8420197790
            Writer.Word(0);
            Writer.Bool(c.Position.Walking);
            Writer.Byte(1);
            if (c.Position.Walking)
            {
                Writer.Byte(c.Position.packetxSec);
                Writer.Byte(c.Position.packetySec);

                byte[] x = BitConverter.GetBytes(c.Position.packetX);
                Array.Reverse(x);
                Writer.Buffer(x);

                Writer.Word(c.Position.packetZ);

                byte[] y = BitConverter.GetBytes(c.Position.packetY);
                Array.Reverse(y);
                Writer.Buffer(y);
            }
            else
            {
                Writer.Byte(0);
                Writer.Word(0);
            }

            Writer.Byte((byte)(c.State.LastState == 128 ? 2 : 1));
            Writer.Byte(c.State.type1);
            Writer.Byte((byte)(c.Information.Berserking ? 1 : 0));

            Writer.Float(c.Speed.WalkSpeed);
            Writer.Float(c.Speed.RunSpeed);
            Writer.Float(c.Speed.BerserkSpeed);

            Writer.Byte(c.Action.Buff.count);
            for (byte b = 0; b < c.Action.Buff.SkillID.Length ; b++)
            {
                if (c.Action.Buff.SkillID[b] != 0)
                {
                    Writer.DWord(c.Action.Buff.SkillID[b]);
                    Writer.DWord(c.Action.Buff.OverID[b]);
                }
            }

            Writer.Text(c.Information.Name);
            //0700706C73676F6C64 
            //00 01 00 01 00 94C11E000000000000000000000000000000000000000000000000000000FF
            //0001000100

            Writer.Byte(0);
            Writer.Byte(1);
            Writer.Byte(0);

            if (c.Transport.Right)
            {
                Writer.Byte(1);
                Writer.Byte(0);
                Writer.DWord(c.Transport.Horse.UniqueID);
            }
            else
            {
                Writer.Byte(0);
                Writer.Byte(0);
            }

            Writer.Byte(0);

            Writer.Byte(0);
            Writer.Byte(0);

            //guild

            Writer.Word(0); //Guild Name
            Writer.DWord(0); // GUİLD ID

            Writer.Word(0); //GRANT NAME
            Writer.DWord(0); //GUILD AMBLEM
            Writer.DWord(0); //UNION ID
            Writer.DWord(0);

            Writer.Word(0);

            Writer.Byte(0);
            Writer.Byte(0xFF);
            Writer.Byte(4);
            //00 01 00 010094C11E00 00 00 00 0000 00000000 0000 00000000 00000000 00000000 0000 00 FF
            return Writer.GetBytes();
        }
        public static byte[] ObjectDeSpawn(int id)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_SOLO_DESPAWN);
            Writer.DWord(id);
            return Writer.GetBytes();
        }
        /// <summary>
        /// Üzenetet küld a megadott típus alapján, a világba, vagy egy cél személynek.
        /// </summary>
        /// <param name="type">Típus. (Globális, Whipser, Guild, Union, All</param>
        /// <param name="id">a cél azonosítója</param>
        /// <param name="text">küldendő szöveg</param>
        /// <param name="name">küldő neve?</param>
        /// <returns>A küldendő üzenetet vissza addja byte[] buff -ba.</returns>
        public static byte[] ChatPacket(byte type, int id, string text, string name)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_CHAT);
            Writer.Byte(type);
            //04 0700 426F6F70657273 0B0046696E65207468616E6B73
            switch (type)
            {
                case 1:
                case 3: 
                    Writer.DWord(id);
                    Writer.Text(text);
                    break;
                case 2:
                    Writer.Text(name);
                    Writer.Text(text);
                    break;
                case 4:
                    Writer.Text(name);
                    Writer.Text(text);
                    break;
                case 6:
                    Writer.Text(name);
                    Writer.Text(text);
                    break;
                case 7:
                    Writer.Text(text);
                    break;
            }
            return Writer.GetBytes();
        }
        public static byte[] ChatIndexPacket(byte type, byte index)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_CHAT_INDEX);
            Writer.Bool(true);
            Writer.Byte(type);
            Writer.Byte(index);
            return Writer.GetBytes();
        }
        public static byte[] ActionPacket(byte type1, byte type2, int skillid, int ownerid, int castingid, int target)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_ACTION_DATA);
            Writer.Byte(type1);
            Writer.Byte(type2);
            Writer.Byte(0x30);

            Writer.DWord(skillid);//skillid
            Writer.DWord(ownerid); // charid
            Writer.DWord(castingid);//overid
            Writer.DWord(target);
            Writer.Byte(0);
 
            return Writer.GetBytes();
        }

        public static byte[] ActionPacket(byte type1, byte type2)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_ACTION_DATA);
            Writer.Byte(type1);
            Writer.Byte(type2);
            Writer.Byte(0x30);
            return Writer.GetBytes();
        }
        public static byte[] SkillPacket(byte type, int castingid)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_SKILL_DATA);
            Writer.Byte(1);
            Writer.DWord(castingid);
            Writer.Byte(type);
            switch (type)
            {
                case 0:
                    Writer.DWord(0);
                    break;
            }
            return Writer.GetBytes();
        }
        public static byte[] SkillIconPacket(int ownerid, int skillid, int overid)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_SKILL_ICON);
            Writer.DWord(ownerid);
            Writer.DWord(skillid);
            Writer.DWord(overid);
            return Writer.GetBytes();
        }
        public static byte[] SkillEndBuffPacket(int overid)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_SKILL_ENDBUFF);
            Writer.Bool(true);
            Writer.DWord(overid);
            return Writer.GetBytes();
        }
        public static byte[] SetSpeed(int id, float speed1, float speed2)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_SETSPEED);
            Writer.DWord(id);
            Writer.Float(speed1);
            Writer.Float(speed2);
            return Writer.GetBytes();
        }
        public static byte[] P_Request(byte swichs , int id, byte type)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_PARTY_REQUEST);

            Writer.Byte(swichs);
            switch (swichs)
            {
                case 2:
                    Writer.DWord(id);
                    Writer.Byte(type);
                    break;
                case 1:
                    Writer.DWord(id);
                    break;
            }

            return Writer.GetBytes();
        }
        public static byte[] CreateFormedParty(party pt)
        {
            Print.Format("Válasz készítése a party matching kérelméhez a következő játékosnak: {0}", pt.LeaderID);
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_FORMED_PARTY_CREATED);
            Writer.Byte(1);
            //Party azon:
            Writer.DWord(pt.ptid);
            //00 00 00 00
            Writer.DWord(0);
            //Parti típus:
            Writer.Byte(pt.Type);
            //a party célja:  (hunting, quest, trade, thief)
            Writer.Byte(pt.ptpurpose);
            Writer.Byte(pt.minlevel);
            Writer.Byte(pt.maxlevel);
            Writer.Word(pt.partyname.Length);
            Writer.String(pt.partyname);
            return Writer.GetBytes();
        }
        public static byte[] DeleteFormedParty(int PartyID)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_DELETE_FORMED_PARTY);
            Writer.Byte(1);
            Writer.DWord(PartyID);
            return Writer.GetBytes();
        }
        /// <summary>
        /// A party lista elküldése a kliensnek.
        /// </summary>
        /// <param name="pt">A party listának egy példánya.</param>
        /// <param name="client">A kliens példánya kell.</param>
        /// <param name="Char">A karakter egy példánya.</param>
        /// <returns>Az elküldendő adat bájban</returns>
        public static byte[] ListPartyMatching(List<party> pt)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_SEND_PARTYLIST);
            Writer.Byte(1);
            Writer.Byte(1);
            Writer.Byte(0);
            //Partik száma:
            Writer.Byte(pt.Count);
            if (pt.Count > 0)
            {
                //Party lista bejárása:
                foreach (party currpt in pt)
                {
                    Systems s = Systems.GetPlayer(currpt.LeaderID);
                    Writer.DWord(currpt.ptid);
                    Writer.DWord(0); //<- ez nem a megfelelő packet, csak 0-val töljük ki ezt. Még azonosítatlan.
                    Writer.Word(s.Karakter.Information.Name.Length);
                    Writer.String(s.Karakter.Information.Name);
                    Writer.Byte(0); //<- ez nem a megfelelő packet, csak 0-t küldünk. Még azonosítatlan.
                    Writer.Byte(currpt.Members.Count);
                    Writer.Byte(currpt.Type);
                    Writer.Byte(currpt.ptpurpose);
                    Writer.Byte(currpt.minlevel);
                    Writer.Byte(currpt.maxlevel);
                    Writer.Word(currpt.partyname.Length);
                    Writer.String(currpt.partyname);
                }
            }
            return Writer.GetBytes();
        }
        public static byte[] OpenExhangeWindow(int id)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_EXCHANGE_WINDOW);
            Writer.DWord(id);
            return Writer.GetBytes();
        }
        public static byte[] OpenExhangeWindow(byte type, int id)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_EXCHANGE_PROCESS);
            Writer.Bool(true);
            Writer.DWord(id);
            return Writer.GetBytes();
        }
        public static byte[] CloseExhangeWindow()
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_EXCHANGE_CLOSE);
            Writer.Bool(true);
            return Writer.GetBytes();
        }
        public static byte[] Party_Leader(int id)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_PARTY_UNKNOWN2);
            Writer.Byte(1);
            Writer.DWord(id);
            return Writer.GetBytes();
        }
        public static byte[] Party_Member(int id)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_PARTY_UNKNOWN1);
            Writer.Byte(1);
            Writer.DWord(id);
            return Writer.GetBytes();
        }
        public static byte[] Party_DataMember(party p)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_PARTYMEMBER_DATA);

            //FF E9791700 05 02
            Writer.Byte(0xFF);
            Writer.DWord(p.LeaderID);
            Writer.Byte(p.Type);
            Writer.Byte(p.Members.Count);
            for (byte b = 0; b <= p.Members.Count - 1; b++)
            {
                    Systems s = Systems.GetPlayer(p.Members[b]);
                    Writer.Byte(0xff);
                    Writer.DWord(s.Karakter.Information.UniqueID);
                    Writer.Text(s.Karakter.Information.Name);
                    Writer.DWord(s.Karakter.Information.Model);
                    Writer.Byte(s.Karakter.Information.Level);
                    Writer.Byte(0xAA); //hp-mp
                    Writer.Byte(s.Karakter.Position.xSec);
                    Writer.Byte(s.Karakter.Position.ySec);
                    Writer.Word(Function.Formule.packetx(s.Karakter.Position.x, s.Karakter.Position.xSec));
                    Writer.Word(s.Karakter.Position.z);
                    Writer.Word(Function.Formule.packety(s.Karakter.Position.y, s.Karakter.Position.ySec));
                    Writer.Word(1);
                    Writer.Word(1);
                    Writer.Word(0);
                    Writer.Byte(4); // max player
                    Writer.Byte(0);
                    Writer.Byte(0); // max player
                    Writer.DWord(0);
                    Writer.Word(0);
            }
            //FF E9791700 06004B6F4C655261 9D380000 16 AA 4D68 CF05 5000 3206 0100 0100 0000 040402 000000000000
            //FF 86DA1600 060064656E656D65 9C380000 01 AA 4D68 F705 5000 1F06 0100 0100 0000 040000 000000000000
            return Writer.GetBytes();
        }
        public static byte[] Party_Data(byte type, int id)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_PARTY_DATA);


            Writer.Byte(type);
            switch (type)
            {
                case 1:
                    Writer.Byte(0x0b);
                    Writer.Byte(0);
                    return Writer.GetBytes();
                case 2:
                    Systems s = Systems.GetPlayer(id);
                    Writer.Byte(0xff);
                    Writer.DWord(s.Karakter.Information.UniqueID);
                    Writer.Text(s.Karakter.Information.Name);
                    Writer.DWord(s.Karakter.Information.Model);
                    Writer.Byte(s.Karakter.Information.Level);
                    Writer.Byte(0xAA); //hp-mp
                    Writer.Byte(s.Karakter.Position.xSec);
                    Writer.Byte(s.Karakter.Position.ySec);
                    Writer.Word(Function.Formule.packetx(s.Karakter.Position.x, s.Karakter.Position.xSec));
                    Writer.Word(s.Karakter.Position.z);
                    Writer.Word(Function.Formule.packety(s.Karakter.Position.y, s.Karakter.Position.ySec));
                    Writer.Word(1);
                    Writer.Word(1);
                    Writer.Word(0);
                    Writer.Byte(4); // max player
                    Writer.Byte(0);
                    Writer.Byte(0); // max player
                    Writer.DWord(0);
                    Writer.Word(0);
                    return Writer.GetBytes();
                case 3:
                    Writer.DWord(id);
                    Writer.Byte(4);
                    return Writer.GetBytes();
                case 6:
                    //06 0E8D1600 04 48
                    //06 E9791700 20 4D69 6105 5000 8102 0100 0100
                    s = Systems.GetPlayer(id);
                    /*int hpkalanyuzde = (s.Karakter.Stat.SecondHp * 100) / s.Karakter.Stat.Hp;
                    int partyyuzde = (85 * hpkalanyuzde) / 100;
                    hpkalanyuzde = (s.Karakter.Stat.SecondMP * 100) / s.Karakter.Stat.Mp;
                    partyyuzde += (85 * hpkalanyuzde) / 100;
                    Writer.DWord(id);
                    Writer.Byte(4);
                    Writer.Byte(partyyuzde);*/
                    Writer.DWord(id);
                    Writer.Byte(0x20);
                    Writer.Byte(s.Karakter.Position.packetxSec);
                    Writer.Byte(s.Karakter.Position.packetySec);
                   /* byte[] bs = BitConverter.GetBytes(s.Karakter.Position.packetX);

                    Writer.Byte(bs[1]);
                    Writer.Byte(bs[0]);*/
                    Writer.Word(s.Karakter.Position.packetX);
                    Writer.Word(s.Karakter.Position.packetZ);
                   /* bs = BitConverter.GetBytes(s.Karakter.Position.packetY);
                    Writer.Byte(bs[1]);
                    Writer.Byte(bs[0]);*/
                    Writer.Word(s.Karakter.Position.packetY);

                    Writer.Word(1);
                    Writer.Word(1);
                    return Writer.GetBytes();
                case 9:
                    Writer.DWord(id);
                    return Writer.GetBytes();
            }
            return Writer.GetBytes();
        }
        public static byte[] Player_LevelUpEffect(int id)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_PLAYER_LEVELUP_EFFECT);
            Writer.DWord(id);
            return Writer.GetBytes();
        }
        public static byte[] Player_getExp(int id, long exp, long sp, short level)
        {
            PacketWriter Writer = new PacketWriter();
            //90400000 1A00000000000000 6D00000000000000 00
            //465C0000 1A00000000000000 6D00000000000000 00 0300
            //FC09C538 1405000000000000 0A32000000000000 00
            Writer.Create(Systems.SERVER_PLAYER_GET_EXP);
            Writer.DWord(id);
            Writer.LWord(exp);
            Writer.LWord(sp);
            Writer.Byte(0);
            if (level != 0) Writer.Word(level);
            return Writer.GetBytes();
        }
        public static byte[] Player_HandleEffect(int id, int itemid)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_PLAYER_HANDLE_EFFECT);
            Writer.DWord(id);
            Writer.DWord(itemid);
            return Writer.GetBytes();
        }
        public static byte[] Player_HandleUpdateSlot(byte slot, ushort amount, ushort packet)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_PLAYER_HANDLE_UPDATE_SLOT);
            Writer.Byte(1);
            Writer.Byte(slot);
            Writer.Word(amount);
            Writer.Word(packet);
            return Writer.GetBytes();
        }
        public static byte[] Unique_Data(byte type, int mobid, string name)
        {
            PacketWriter Writer = new PacketWriter();
            //06 0C004B00 00 0200BFE9
            Writer.Create(Systems.SERVER_UNIQUE_ANNOUNCE);
            Writer.Byte(type);
            switch (type)
            {
                case 5:
                    Writer.Byte(0x0C);
                    Writer.DWord(mobid);
                    break;
                case 6:
                    Writer.Byte(0x0C);
                    Writer.DWord(mobid);
                    Writer.Text(name);
                    break;
            }


            return Writer.GetBytes();
        }

    }
    public class DeGroupSpawn
    {
        PacketWriter buff_start = new PacketWriter();
        PacketWriter buff_data = new PacketWriter();
        PacketWriter buff_end = new PacketWriter();
        ushort added;
        public byte[] StartDeGroup()
        {
            buff_start.Create(Systems.SERVER_GROUPSPAWN_START);
            buff_start.Byte(2); // type
            buff_start.Word(added); // amount
            return buff_start.GetBytes();
        }
        public void StartData()
        {
            buff_data.Create(Systems.SERVER_GROUPSPAWN_DATA);
        }
        public void AddObject(int id)
        {
            buff_data.DWord(id);
            added++;
        }
        public byte[] EndData()
        {
            return buff_data.GetBytes();
        }
        public byte[] EndGroup()
        {
            buff_end.Create(Systems.SERVER_GROUPSPAWN_END);
            return buff_end.GetBytes();
        }
    }
    public class GroupSpawn
    {
        PacketWriter buff_start = new PacketWriter();
        PacketWriter buff_data = new PacketWriter();
        PacketWriter buff_end = new PacketWriter();
        public byte[] StartGroup(ushort amount)
        {
            buff_start.Create(Systems.SERVER_GROUPSPAWN_START);
            buff_start.Byte(1); // type
            buff_start.Word(amount); // amount
            return buff_start.GetBytes();
        }
        public void StartData()
        {
            buff_data.Create(Systems.SERVER_GROUPSPAWN_DATA);
        }
        public byte[] EndData()
        {
            return buff_data.GetBytes();
        }
        public void AddObject(obj o,float z)
        {
            buff_data.DWord(o.ID);
            buff_data.DWord(o.UniqueID);

            buff_data.Byte(o.xSec);
            buff_data.Byte(o.ySec);

            if (o.LocalType == 1)
            {
                buff_data.Float(Function.Formule.packetx((float)o.x, o.xSec));
                buff_data.Float(z);
                buff_data.Float(Function.Formule.packety((float)o.y, o.ySec));

                buff_data.Word(0);

                buff_data.Byte(1);
                buff_data.Byte(0);

                buff_data.Byte(o.xSec);
                buff_data.Byte(o.ySec);
                buff_data.Word(Function.Formule.packetx((float)o.x, o.xSec));
                buff_data.Word(z);
                buff_data.Word(Function.Formule.packety((float)o.y, o.ySec));

                buff_data.Byte(1);
                buff_data.Word(2);
                buff_data.Float(10);	// Walk speed
                buff_data.Float(40);	// Run speed
                buff_data.Float(100);	// Berserk speed
                buff_data.Byte(0);
                buff_data.Byte(2);
                buff_data.Byte(1);
                buff_data.Byte(5);
                buff_data.Byte(o.Type);
            }
            else if (o.LocalType == 2)
            {
                buff_data.Float(Function.Formule.packetx((float)o.x, o.xSec));
                buff_data.Float(z);
                buff_data.Float(Function.Formule.packety((float)o.y, o.ySec));

                buff_data.Word(0);

                buff_data.Byte(0);
                buff_data.Byte(1);

                buff_data.DWord(0x00000001);
                buff_data.LWord(0);
                buff_data.Word(0);
                buff_data.Float(10);	// Walk speed
                buff_data.Byte(0);
                buff_data.Word(0x0402);
                buff_data.Word(0x0201);
                buff_data.Word(0x2004);
            }
            else if (o.LocalType == 3)
            {
                buff_data.Float(Function.Formule.packetx((float)o.x, o.xSec));
                buff_data.Float(o.z);
                buff_data.Float(Function.Formule.packety((float)o.y, o.ySec));

                buff_data.Word(0);
            }
        }
        public void AddObject(character c)
        {
            buff_data.DWord(c.Information.Model);
            buff_data.Byte(c.Information.Volume);
            buff_data.Byte(c.Information.Title); //title
            buff_data.Bool((c.Information.Level < 20 ? true : false));

            buff_data.Byte(0x2D);

            Function.Items.PrivateItemPacket(buff_data, c.Information.CharacterID, 0 , 8 , 0);

            buff_data.Byte(5);

            Function.Items.PrivateItemPacket(buff_data, c.Information.CharacterID, 3, 5, 1);

            buff_data.Byte(0);

            buff_data.DWord(c.Information.UniqueID);

            buff_data.Byte(c.Position.xSec);
            buff_data.Byte(c.Position.ySec);
            buff_data.Float(Function.Formule.packetx(c.Position.x, c.Position.xSec));
            buff_data.Float(c.Position.z);
            buff_data.Float(Function.Formule.packety(c.Position.y, c.Position.ySec));

            buff_data.Word(0);
            buff_data.Byte(1);
            buff_data.Byte(1);
            buff_data.Byte(c.Position.xSec);
            buff_data.Byte(c.Position.ySec);
            buff_data.Word(Function.Formule.packetx(c.Position.x, c.Position.xSec));
            buff_data.Word(0);
            buff_data.Word(Function.Formule.packety(c.Position.y, c.Position.ySec));

            buff_data.Byte((byte)(c.State.LastState == 128 ? 2 : 1));
            buff_data.Byte(c.State.type1);
            buff_data.Byte(c.State.type2);

            buff_data.Float(c.Speed.WalkSpeed);
            buff_data.Float(c.Speed.RunSpeed);
            buff_data.Float(c.Speed.BerserkSpeed);

            buff_data.Byte(c.Action.Buff.count);
            for (byte b = 0; b <= c.Action.Buff.SkillID.Length - 1; b++)
            {
                if (c.Action.Buff.SkillID[b] != 0)
                {
                    buff_data.DWord(c.Action.Buff.SkillID[b]);
                    buff_data.DWord(c.Action.Buff.OverID[b]);
                }
            }
            buff_data.Text(c.Information.Name);

            buff_data.Byte(0);
            buff_data.Byte(1);
            buff_data.Byte(0);


            buff_data.Word(0);

            buff_data.Byte(0);

            buff_data.Byte(0);
            buff_data.Byte(0);

            //guild

            buff_data.Word(0); //Guild Name
            buff_data.DWord(0); // GUİLD ID

            buff_data.Word(0); //GRANT NAME
            buff_data.DWord(0); //GUILD AMBLEM
            buff_data.DWord(0); //UNION ID
            buff_data.DWord(0);

            buff_data.Word(0);

            buff_data.Byte(0);
            buff_data.Byte(0xFF);
        }
        public void AddObject(world_item w)
        {
            //9D3800004400002D07F033000000373400000014340000005B34000000E42B000000F25E000000932B000000050000DB5A3E014D680060B544E83DA1420020D04482F201014D68AB055000810601000000008041000048420000C8420006004B6F4C65526101010000000000000000000000000000000000000000000000000000000000FF
            //01000000 0A000000 7CCF3E01 4D68 F354B544 6E34A142 9E38D144 B444 0000

            //gold
            buff_data.DWord(w.Model);
            if (w.Type == 1)
            {
                buff_data.DWord(w.amount);
                buff_data.DWord(w.UniqueID);
                buff_data.Byte(w.xSec);
                buff_data.Byte(w.ySec);
                buff_data.Float(Function.Formule.packetx((float)w.x, w.xSec));
                buff_data.Float(w.z);
                buff_data.Float(Function.Formule.packety((float)w.y, w.ySec));
                buff_data.DWord(0);
            }
            //item
            //5B340000 00 DF5A3E01 4D68 366DB544 B82DA142 331FD244 143A0000
            if (w.Type == 2)
            {
                buff_data.Byte(w.PlusValue);
                buff_data.DWord(w.UniqueID);
                buff_data.Byte(w.xSec);
                buff_data.Byte(w.ySec);
                buff_data.Float(Function.Formule.packetx((float)w.x, w.xSec));
                buff_data.Float(w.z);
                buff_data.Float(Function.Formule.packety((float)w.y, w.ySec));
                buff_data.DWord(0);
            }
            //etc
            //1A390000 EE5A3E01 4D68 E248B444 FE5BA142 31D5CA44 103C0000
            if (w.Type == 3)
            {
                buff_data.DWord(w.UniqueID);
                buff_data.Byte(w.xSec);
                buff_data.Byte(w.ySec);
                buff_data.Float(Function.Formule.packetx((float)w.x, w.xSec));
                buff_data.Float(w.z);
                buff_data.Float(Function.Formule.packety((float)w.y, w.ySec));
                buff_data.DWord(0);
            }
        }
        public byte[] EndGroup()
        {
            buff_end.Create(Systems.SERVER_GROUPSPAWN_END);
            return buff_end.GetBytes();
        }
    }
}
