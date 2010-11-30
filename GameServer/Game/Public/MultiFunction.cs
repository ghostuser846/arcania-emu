using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework;
namespace Game
{
    public partial class Systems
    {
        public void Movement()
        {
            StopAttackTimer();
            PacketReader Reader = new PacketReader(PacketInformation.buffer);
            Karakter.Action.PickUping = false;
            if (Reader.Byte() == 1 && !Karakter.State.Die && !Karakter.Action.sCasting && !Karakter.Action.sAttack && !Karakter.Information.Scroll)
            {
                Karakter.Position.Walking = true;
                byte xsec = Reader.Byte();
                byte ysec = Reader.Byte();
                float x = Reader.Int16();
                float z = Reader.Int16();
                float y = Reader.Int16();
                Reader.Close();

                double distance = Function.Formule.gamedistance(Karakter.Position.x,
                    Karakter.Position.y,
                    Function.Formule.gamex(x, xsec),
                    Function.Formule.gamey(y, ysec));

                Karakter.Position.xSec = xsec;
                Karakter.Position.ySec = ysec;
                Karakter.Position.wX = Function.Formule.gamex(x, xsec) - Karakter.Position.x;
                Karakter.Position.wZ = z;
                Karakter.Position.wY = Function.Formule.gamey(y, ysec) - Karakter.Position.y;

                Karakter.Position.packetxSec = xsec;
                Karakter.Position.packetySec = ysec;
                Karakter.Position.packetX = (ushort)x;
                Karakter.Position.packetZ = (ushort)z;
                Karakter.Position.packetY = (ushort)y;

                Send(Public.Packet.Movement(new Global.vektor(Karakter.Information.UniqueID, x, z, y, xsec, ysec)));
                Karakter.Position.Time = (distance / Karakter.Speed.RunSpeed) * 10000;
                Karakter.Position.RecordedTime = Karakter.Position.Time;

                StartMovementTimer((int)(Karakter.Position.Time * 0.1));
            }
        }
        void MovementTransport()
        {
            StopAttackTimer();
            PacketReader Reader = new PacketReader(PacketInformation.buffer);
            Karakter.Action.PickUping = false;
            int horseid = Reader.Int32();
            Reader.Skip(1);
            if (Reader.Byte() == 1 && !Karakter.State.Die && !Karakter.Action.sCasting && !Karakter.Action.sAttack && !Karakter.Information.Scroll)
            {
                Karakter.Position.Walking = true;
                byte xsec = Reader.Byte();
                byte ysec = Reader.Byte();
                float x = Reader.Int16();
                float z = Reader.Int16();
                float y = Reader.Int16();
                Reader.Close();
                double distance = Function.Formule.gamedistance(Karakter.Position.x,
                    Karakter.Position.y,
                    Function.Formule.gamex(x, xsec),
                    Function.Formule.gamey(y, ysec));

                Karakter.Position.xSec = xsec;
                Karakter.Position.ySec = ysec;
                Karakter.Position.wX = Function.Formule.gamex(x, xsec) - Karakter.Position.x;
                Karakter.Position.wZ = z;
                Karakter.Position.wY = Function.Formule.gamey(y, ysec) - Karakter.Position.y;

                Karakter.Position.packetxSec = xsec;
                Karakter.Position.packetySec = ysec;
                Karakter.Position.packetX = (ushort)x;
                Karakter.Position.packetZ = (ushort)z;
                Karakter.Position.packetY = (ushort)y;

                Send(Public.Packet.Movement(new Global.vektor(horseid, x, z, y, xsec, ysec)));
                Karakter.Position.Time = (distance / 95) * 10000;
                Karakter.Position.RecordedTime = Karakter.Position.Time;

                StartMovementTimer((int)(Karakter.Position.Time * 0.1));
            }
        }
        void LeaveGame()
        {
            byte type = PacketInformation.buffer[0];

            Karakter.Information.Quit = true;
            client.Send(Private.Packet.StartingLeaveGame(5, type));
            StartWaitingTimer(5000);
        }
        void CancelLeaveGame()
        {
            client.Send(Private.Packet.CalcelLeaveGame());
            Karakter.Information.Quit = false;
            Timer.Waiting.Dispose();
        }
        void Emote()
        {
            client.Send(Private.Packet.Player_Emote(Karakter.Information.UniqueID, PacketInformation.buffer[0]));
        }
        void Chat()
        {
            try
            {
                List<int> lis = Karakter.Spawn;
                PacketReader Reader = new PacketReader(PacketInformation.buffer);
                byte chatType = Reader.Byte();
                byte chatIndex = 0;
                string Text = null;
                switch (chatType)
                {
                    case 1:
                    case 3:
                        chatIndex = Reader.Byte();
                        Reader.Byte();
                        ushort chatcount = Reader.UInt16();
                        string EndCODE = Encoding.ASCII.GetString(PacketInformation.buffer, 5, chatcount);
                        Reader.Close();
                        /*EndCODE = EndCODE.Replace("ı", "i");
                        EndCODE = EndCODE.Replace("İ", "I");
                        EndCODE = EndCODE.Replace("ş", "s");
                        EndCODE = EndCODE.Replace("Ş", "S");
                        EndCODE = EndCODE.Replace("ğ", "g");
                        EndCODE = EndCODE.Replace("Ğ", "G");
                        EndCODE = EndCODE.Replace("Ü", "U");
                        EndCODE = EndCODE.Replace("ü", "u");
                        EndCODE = EndCODE.Replace("Ç", "C");
                        EndCODE = EndCODE.Replace("ç", "c");
                        EndCODE = EndCODE.Replace("Ö", "O");
                        EndCODE = EndCODE.Replace("ö", "o");*/

                        Send(lis, Public.Packet.ChatPacket(chatType, Karakter.Information.UniqueID, EndCODE, null));
                        client.Send(Public.Packet.ChatIndexPacket(chatType, chatIndex));
                        break;
                    case 2:
                        chatIndex = Reader.Byte();
                        Reader.Byte();
                        string toName = Reader.Text();
                        Systems sys = null;
                        for (int b = 0; b < Systems.clients.Count; b++)
                        {
                            if (Systems.clients[b] != null && Systems.clients[b].Karakter.Information.Name == toName && Systems.clients[b].Karakter.Information.GM == 0) sys = Systems.clients[b];
                        }

                        if (sys == null) return;
                        if (!sys.Karakter.InGame) return;

                        chatcount = Reader.UInt16();
                        EndCODE = Encoding.ASCII.GetString(PacketInformation.buffer, 5 + toName.Length + 2, chatcount);
                        Reader.Close();
                        // török
                        /*EndCODE = EndCODE.Replace("ı", "i");
                        EndCODE = EndCODE.Replace("İ", "I");
                        EndCODE = EndCODE.Replace("ş", "s");
                        EndCODE = EndCODE.Replace("Ş", "S");
                        EndCODE = EndCODE.Replace("ğ", "g");
                        EndCODE = EndCODE.Replace("Ğ", "G");
                        EndCODE = EndCODE.Replace("Ü", "U");
                        EndCODE = EndCODE.Replace("ü", "u");
                        EndCODE = EndCODE.Replace("Ç", "C");
                        EndCODE = EndCODE.Replace("ç", "c");
                        EndCODE = EndCODE.Replace("Ö", "O");
                        EndCODE = EndCODE.Replace("ö", "o");*/

                        // magyar
                        EndCODE = EndCODE.Replace("ö", "o");
                        EndCODE = EndCODE.Replace("ü", "u");
                        EndCODE = EndCODE.Replace("ó", "o");
                        EndCODE = EndCODE.Replace("ő", "o");
                        EndCODE = EndCODE.Replace("ú", "u");
                        EndCODE = EndCODE.Replace("é", "e");
                        EndCODE = EndCODE.Replace("á", "a");
                        EndCODE = EndCODE.Replace("ű", "u");
                        EndCODE = EndCODE.Replace("í", "i");

                        sys.client.Send(Public.Packet.ChatPacket(chatType, 0, EndCODE, this.Karakter.Information.Name));
                        client.Send(Public.Packet.ChatIndexPacket(chatType, chatIndex));
                        break;
                    case 4:
                        if (Karakter.Network.Party != null)
                        {
                            chatIndex = Reader.Byte();

                            Reader.Byte();
                            chatcount = Reader.UInt16();
                            EndCODE = Encoding.ASCII.GetString(PacketInformation.buffer, 5, chatcount);
                            Reader.Close();
                            // török
                            /*
                            EndCODE = EndCODE.Replace("ı", "i");
                            EndCODE = EndCODE.Replace("İ", "I");
                            EndCODE = EndCODE.Replace("ş", "s");
                            EndCODE = EndCODE.Replace("Ş", "S");
                            EndCODE = EndCODE.Replace("ğ", "g");
                            EndCODE = EndCODE.Replace("Ğ", "G");
                            EndCODE = EndCODE.Replace("Ü", "U");
                            EndCODE = EndCODE.Replace("ü", "u");
                            EndCODE = EndCODE.Replace("Ç", "C");
                            EndCODE = EndCODE.Replace("ç", "c");
                            EndCODE = EndCODE.Replace("Ö", "O");
                            EndCODE = EndCODE.Replace("ö", "o");
                            */

                            // magyar
                            EndCODE = EndCODE.Replace("ö", "o");
                            EndCODE = EndCODE.Replace("ü", "u");
                            EndCODE = EndCODE.Replace("ó", "o");
                            EndCODE = EndCODE.Replace("ő", "o");
                            EndCODE = EndCODE.Replace("ú", "u");
                            EndCODE = EndCODE.Replace("é", "e");
                            EndCODE = EndCODE.Replace("á", "a");
                            EndCODE = EndCODE.Replace("ű", "u");
                            EndCODE = EndCODE.Replace("í", "i");

                            Karakter.Network.Party.Send(Public.Packet.ChatPacket(chatType, Karakter.Information.UniqueID, EndCODE, this.Karakter.Information.Name));
                            client.Send(Public.Packet.ChatIndexPacket(chatType, chatIndex));
                        }
                        break;
                    case 7:
                        chatIndex = Reader.Byte();
                        Reader.Byte();
                        Text = Reader.Text();
                        Reader.Close();
                        SendAll(Public.Packet.ChatPacket(chatType, Karakter.Information.UniqueID, Text, null));
                        client.Send(Public.Packet.ChatIndexPacket(chatType, chatIndex));
                        break;
                    default:
                        Print.Format("Unknown chatType: {0}", chatType);
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Chat()::error..");
                deBug.Write(ex);
            }
        }
        bool PlayerCheckFreeSlot()
        {
            return false;
        }

        void Teleport_Start()
        {
            PacketReader Reader = new PacketReader(PacketInformation.buffer);
            byte number;
            Reader.Skip(5);
            number = Reader.Byte();
            Reader.Close();


            BuffAllClose();

            client.Send(Private.Packet.TeleportStart());

            DeSpawnMe();
            ObjectDeSpawn();

            Karakter.InGame = false;
            Teleport_UpdateXYZ(number);
           
            client.Send(Private.Packet.TeleportImage(Data.PointBase[number].xSec, Data.PointBase[number].ySec));
            Karakter.Teleport = true;
        }
        void Teleport_Data()
        {
            try
            {
                if (Karakter.Teleport)
                {
                    StopBerserkTimer();

                    client.Send(Private.Packet.StartPlayerLoad());
                    client.Send(Private.Packet.Load(Karakter));
                    client.Send(Private.Packet.EndPlayerLoad());

                    SavePlayerPosition();

                    ObjectSpawnCheck();
                    Karakter.Teleport = false;

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Teleport_Data()::error..");
                deBug.Write(ex);
            }
        }
        void Teleport_UpdateXYZ(byte number)
        {
            Karakter.Position.xSec = Data.PointBase[number].xSec;
            Karakter.Position.ySec = Data.PointBase[number].ySec;
            Karakter.Position.x = (float)Data.PointBase[number].x;
            Karakter.Position.z = (float)Data.PointBase[number].z;
            Karakter.Position.y = (float)Data.PointBase[number].y;
            if (Karakter.Transport.Right)
            {
                Karakter.Transport.Horse.xSec = Data.PointBase[number].xSec;
                Karakter.Transport.Horse.ySec = Data.PointBase[number].ySec;
                Karakter.Transport.Horse.x = (float)Data.PointBase[number].x;
                Karakter.Transport.Horse.z = (float)Data.PointBase[number].z;
                Karakter.Transport.Horse.y = (float)Data.PointBase[number].y;
            }
            //return BitConverter.ToInt16(new byte[2] { Data.PointBase[number].xSec, Data.PointBase[number].ySec }, 0);
        }
        void Player_Up()
        {
            try
            {
                byte type = PacketInformation.buffer[0];

                if (type == 1)
                {
                    if (Karakter.State.Die)
                    {
                        if (Karakter.State.DeadType == 1)
                        {
                            BuffAllClose();

                            Karakter.InGame = false;

                            DeSpawnMe();
                            ObjectDeSpawn();

                            client.Send(Private.Packet.TeleportOtherStart());

                            Teleport_UpdateXYZ(Karakter.Information.Place);
                            this.Karakter.Stat.SecondHp = this.Karakter.Stat.Hp / 2;

                            Karakter.State.Die = false;

                            client.Send(Private.Packet.TeleportImage(Data.PointBase[Karakter.Information.Place].xSec, Data.PointBase[Karakter.Information.Place].ySec));
                            Karakter.Teleport = true;
                        }
                        else type = 2;
                    }
                }
                if (type == 2)
                {
                    if (Karakter.State.Die)
                    {
                        StopBerserkTimer();
                        Send(Private.Packet.StatePack(this.Karakter.Information.UniqueID, 0, 1, false));
                        this.Karakter.Stat.SecondHp = this.Karakter.Stat.Hp / 2;
                        this.UpdateHp();
                        //client.Send(Private.Packet.UpdatePlayer(this.Karakter.Information.UniqueID, 0x20, 1, (uint)this.Karakter.Stat.SecondHp));
                        Karakter.State.Die = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Player_Up()::error...");
                deBug.Write(ex);
            }
        }
        void Player_PickUp()
        {
            try
            {
                if (Karakter.Action.Target != 0)
                {
                    world_item item = GetWorldItem(Karakter.Action.Target);
                    if (item == null) return;
                    double distance = Function.Formule.gamedistance(Karakter.Position.x,
                            Karakter.Position.y,
                            (float)item.x,
                            (float)item.y);

                    if (distance >= 1)
                    {
                        Karakter.Position.wX = (float)item.x - Karakter.Position.x; //- Data.ItemBase[Karakter.Information.Item.wID].ATTACK_DISTANCE;
                        Karakter.Position.wY = (float)item.y - Karakter.Position.y; //- Data.ItemBase[Karakter.Information.Item.wID].ATTACK_DISTANCE;

                        Send(Public.Packet.Movement(new Game.Global.vektor(Karakter.Information.UniqueID,
                                    (float)Function.Formule.packetx((float)item.x, item.xSec),
                                    (float)Karakter.Position.z,
                                    (float)Function.Formule.packety((float)(float)item.y, item.ySec),
                                    Karakter.Position.xSec,
                                    Karakter.Position.ySec)));

                        Karakter.Position.Time = (distance / Karakter.Speed.RunSpeed) * 10000;
                        Karakter.Position.RecordedTime = Karakter.Position.Time;

                        StartMovementTimer((int)(Karakter.Position.Time * 0.2));
                        return;
                    }
                    Player_PickUpItem();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Player_PickUp()::error..");
                deBug.Write(ex);
            }
        }
        void Player_PickUpItem()
        {
            try
            {
                if (Karakter.Action.PickUping)
                {
                    Karakter.Action.PickUping = false;
                    world_item item = GetWorldItem(Karakter.Action.Target);
                    if (item == null) { Karakter.Action.PickUping = false; return; }
                    if (item.amount < 1) item.amount = 1;

                    if (item.Model > 3 && !Karakter.Action.PickUping)
                    {
                        byte slot = GetFreeSlot();
                        if (slot <= 12) { Karakter.Action.PickUping = false; return; }

                        item.StopDeleteTimer();
                        Systems.WorldItem.Remove(item);
                        Global.ID.Delete(item.UniqueID);

                        Send(Public.Packet.MovementOnPickup(new Game.Global.vektor(Karakter.Information.UniqueID,
                                        (float)Function.Formule.packetx((float)item.x, item.xSec),
                                        (float)Karakter.Position.z,
                                        (float)Function.Formule.packety((float)(float)item.y, item.ySec),
                                        Karakter.Position.xSec,
                                        Karakter.Position.ySec)));

                        Send(Public.Packet.Pickup_egilme(Karakter.Information.UniqueID, 0));

                        int prob = 0;
                        byte type = GetItemType(item.Model);

                        if (type == 1) prob = item.amount;
                        else if (type == 0) prob = item.PlusValue;
                        client.Send(Private.Packet.GM_MAKEITEM(type, slot, item.Model, (short)prob, (int)Data.ItemBase[item.Model].Defans.Durability));
                        AddItem(item.Model, (short)prob, slot, type, Karakter.Information.CharacterID);
                    }
                    else
                    {
                        Systems.WorldItem.Remove(item);
                        Global.ID.Delete(item.UniqueID);

                        Send(Public.Packet.MovementOnPickup(new Game.Global.vektor(Karakter.Information.UniqueID,
                                    (float)Function.Formule.packetx((float)item.x, item.xSec),
                                    (float)Karakter.Position.z,
                                    (float)Function.Formule.packety((float)(float)item.y, item.ySec),
                                    Karakter.Position.xSec,
                                    Karakter.Position.ySec)));


                        Send(Public.Packet.Pickup_egilme(Karakter.Information.UniqueID, 0));
                            Karakter.Information.Gold += item.amount;
                            client.Send(Private.Packet.UpdateGold(Karakter.Information.Gold));
                            SaveGold();
                    }

                    item.DeSpawnMe();
                    item.Dispose();
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("Player_PickUpItem()::error..");
                deBug.Write(ex);
            }
        }
        void Player_Berserk_Up()
        {
            try
            {
                if (!Karakter.Information.Berserking)
                {
                    Karakter.Information.BerserkBar = 0;
                    client.Send(Private.Packet.InfoUpdate(4, 0, Karakter.Information.BerserkBar));
                    Karakter.Information.Berserking = true;

                    if (Karakter.Information.Title != 0) Karakter.Information.BerserkOran = 230;
                    else Karakter.Information.BerserkOran = 200;
                    Send(Private.Packet.StatePack(Karakter.Information.UniqueID, 4, 1, false));

                    Karakter.Speed.Updateded += 100;
                    Player_SetNewSpeed();
                    Send(Public.Packet.SetSpeed(Karakter.Information.UniqueID, Karakter.Speed.WalkSpeed, Karakter.Speed.RunSpeed));

                    MsSQL.UpdateData("update karakterler set berserkbar='" + Karakter.Information.BerserkBar + "' where id='" + Karakter.Information.CharacterID + "'");
                    StartBerserkerTimer(60000);
                }
            }
            catch (Exception ex)
            {
                deBug.Write(ex);
            }
        }
        public void Player_Berserk_Down()
        {
            try
            {
                Karakter.Information.Berserking = false;
                Karakter.Information.BerserkBar = 0;
                Karakter.Speed.Updateded -= 100;
                Player_SetNewSpeed();
                Send(Public.Packet.SetSpeed(Karakter.Information.UniqueID, Karakter.Speed.WalkSpeed, Karakter.Speed.RunSpeed));
                Send(Private.Packet.StatePack(Karakter.Information.UniqueID, 4, 0, false));
            }
            catch (Exception ex)
            {
                deBug.Write(ex);
            }
        }
        public void GetBerserkOrb()
        {
            try
            {
                if (Karakter.Information.BerserkBar < 5 && !Karakter.Information.Berserking)
                {
                    if (Global.RandomID.GetRandom(0, 100) > 70)
                    {
                        Karakter.Information.BerserkBar++;
                        client.Send(Private.Packet.InfoUpdate(4, 0, Karakter.Information.BerserkBar));
                        MsSQL.UpdateData("update karakterler set berserkbar='" + Karakter.Information.BerserkBar + "' where id='" + Karakter.Information.CharacterID + "'");
                    }
                }
            }
            catch (Exception ex)
            {
                deBug.Write(ex);
            }
        }
        protected void Player_SetNewSpeed()
        {
            Karakter.Speed.RunSpeed = Karakter.Speed.DefaultSpeed + ((Karakter.Speed.DefaultSpeed * Karakter.Speed.Updateded) / 100);
        }

        protected void HandleClosePet()
        {
            int petid = BitConverter.ToInt32(PacketInformation.buffer, 0);
            DownFromHorse(petid);
        }
        public void DownFromHorse(int petid)
        {
            Send(Public.Packet.Player_UpToHorse(this.Karakter.Information.UniqueID, false, petid));
            client.Send(Public.Packet.Player_DownToHorse(petid));
            Karakter.Transport.Horse.DeSpawnMe();
            Karakter.Transport.Right = false;
            if (Karakter.Position.Walking) Timer.Movement.Dispose();
        }

    }
}
