using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework;
namespace Game
{
    public partial class Systems
    {
        void PartyMain()
        {
            if (Karakter.Information.Level < 5) return;
            PacketReader Reader = new PacketReader(PacketInformation.buffer);
            Systems s = GetPlayer(Reader.Int32());
            Print.Format(Decode.StringToPack(PacketInformation.buffer));
            byte partytype = Reader.Byte();
            Reader.Close();
            s.Karakter.Network.TargetID = this.Karakter.Information.UniqueID;
            party p;
            if (Karakter.Network.Party == null && s.Karakter.Network.Party == null)
            {
                p = new party();
                p.LeaderID = this.Karakter.Information.UniqueID;
                p.Type = 5;
                Karakter.Network.Party = p;
            }
            if(s.Karakter.Network.Party == null)
                s.client.Send(Public.Packet.P_Request(2, this.Karakter.Information.UniqueID, partytype));
        }
        void PartyRequest()
        {
            PacketReader Reader = new PacketReader(PacketInformation.buffer);
            Print.Format(Decode.StringToPack(PacketInformation.buffer));
            if (Reader.Byte() == 1 && Reader.Byte() == 1)
            {
                Systems s = GetPlayer(Karakter.Network.TargetID);
                party mainParty = s.Karakter.Network.Party;

                client.Send(Public.Packet.Party_Member(Karakter.Information.UniqueID)); //party OK..!
                s.client.Send(Public.Packet.Party_Leader(s.Karakter.Information.UniqueID));//party OK..!

                if (mainParty.Members.Count == 0)
                {
                    mainParty.Members.Add(s.Karakter.Information.UniqueID);
                    mainParty.MembersClient.Add(s.client);
                    s.client.Send(Public.Packet.Party_DataMember(mainParty));
                    s.Karakter.Network.Party = mainParty;
                }

                mainParty.Members.Add(Karakter.Information.UniqueID);
                mainParty.MembersClient.Add(this.client);
                for (byte b = 0; b <= mainParty.Members.Count - 1; b++)
                {
                    if (mainParty.Members[b] == Karakter.Information.UniqueID)
                    {
                        client.Send(Public.Packet.Party_DataMember(mainParty));
                    }
                    else
                    {
                        s = GetPlayer(mainParty.Members[b]);
                        s.client.Send(Public.Packet.Party_Data(2, Karakter.Information.UniqueID));
                        s.Karakter.Network.Party = mainParty;
                    }
                }
                
                Karakter.Network.Party = mainParty;
            }
        }
        void PartyAddmembers()
        {
            PacketReader Reader = new PacketReader(PacketInformation.buffer);
            Systems s = GetPlayer(Reader.Int32());
            s.Karakter.Network.TargetID = this.Karakter.Information.UniqueID;
            s.client.Send(Public.Packet.P_Request(2, this.Karakter.Information.UniqueID, this.Karakter.Network.Party.Type));
        }
        void PartyLeave()
        {
            Karakter.Network.Party.Members.Remove(this.Karakter.Information.UniqueID);
            Karakter.Network.Party.MembersClient.Remove(this.client);
            client.Send(Public.Packet.Party_Data(1, 0));
            for (byte b = 0; b <= Karakter.Network.Party.Members.Count - 1; b++)
            {
                Systems s = GetPlayer(Karakter.Network.Party.Members[b]);
                s.Karakter.Network.Party.Members.Remove(this.Karakter.Information.UniqueID);
                s.Karakter.Network.Party.MembersClient.Remove(this.client);
                if (Karakter.Network.Party.Members.Count == 1)
                {
                    s.client.Send(Public.Packet.Party_Data(1, 0));
                    s.Karakter.Network.Party = null;
                    Karakter.Network.Party = null;
                    return;
                }
                else
                    s.client.Send(Public.Packet.Party_Data(3, this.Karakter.Information.UniqueID));
            }

            if (Karakter.Information.UniqueID == Karakter.Network.Party.LeaderID)
            {
                for (byte b = 0; b <= Karakter.Network.Party.Members.Count - 1; b++)
                {
                    Systems s = GetPlayer(Karakter.Network.Party.Members[b]);
                    s.client.Send(Public.Packet.Party_Data(9, Karakter.Network.Party.Members[0]));
                    s.Karakter.Network.Party.LeaderID = Karakter.Network.Party.Members[0];
                }
            }
            Karakter.Network.Party = null;
        }
        void PartyBan()
        {
            PacketReader Reader = new PacketReader(PacketInformation.buffer);
            int TargetID = Reader.Int32();

            Systems s = GetPlayer(TargetID);
            Client TargetClient = s.client;
            Karakter.Network.Party.Members.Remove(TargetID);
            Karakter.Network.Party.MembersClient.Remove(TargetClient);
            
            s.client.Send(Public.Packet.Party_Data(1, 0));
            for (byte b = 0; b <= Karakter.Network.Party.Members.Count - 1; b++)
            {
                s = GetPlayer(Karakter.Network.Party.Members[b]);
                s.Karakter.Network.Party.Members.Remove(TargetID);
                s.Karakter.Network.Party.MembersClient.Remove(TargetClient);

                if (Karakter.Network.Party.Members.Count == 1)
                {
                    s.client.Send(Public.Packet.Party_Data(1, 0));
                }
                else
                    s.client.Send(Public.Packet.Party_Data(3, TargetID));
            }
            Print.Format("TargetID {0} Karakter.Network.Party.LeaderID {1}", TargetID, Karakter.Network.Party.LeaderID);
            if (TargetID == Karakter.Network.Party.LeaderID)
            {
                Print.Format("LeaderID {0}", TargetID);
                for (byte b = 0; b <= Karakter.Network.Party.Members.Count - 1; b++)
                {
                    s = GetPlayer(Karakter.Network.Party.Members[b]);
                    s.client.Send(Public.Packet.Party_Data(9, Karakter.Network.Party.Members[0]));
                    s.Karakter.Network.Party.LeaderID = Karakter.Network.Party.Members[0];
                }
            }
            s.Karakter.Network.Party = null;
        }
        void Exchange_Request()
        {
            PacketReader Reader = new PacketReader(PacketInformation.buffer);
            int targetid = Reader.Int32();
            Systems sys = GetPlayer(targetid);
            Karakter.Network.TargetID = targetid;
            sys.Karakter.Network.TargetID = this.Karakter.Information.UniqueID;
            sys.client.Send(Public.Packet.P_Request(1, this.Karakter.Information.UniqueID, 0));
        }
        void Request()
        {
            switch (PacketInformation.buffer[0])
            {
                case 1:
                    if (PacketInformation.buffer[1] == 0) //Cancel
                    {
                        Systems sys = GetPlayer(Karakter.Network.TargetID);
                        sys.client.Send(Private.Packet.Exchange_Cancel());
                    }
                    else if (PacketInformation.buffer[1] == 1) // Accept
                    {
                        Systems sys = GetPlayer(Karakter.Network.TargetID);
                        sys.client.Send(Public.Packet.OpenExhangeWindow(1, this.Karakter.Information.UniqueID));
                        client.Send(Public.Packet.OpenExhangeWindow(sys.Karakter.Information.UniqueID));

                        Karakter.Network.Exchange.Window = true;
                        Karakter.Network.Exchange.ItemList = new List<Game.Global.slotItem>();
                        sys.Karakter.Network.Exchange.Window = true;
                        sys.Karakter.Network.Exchange.ItemList = new List<Game.Global.slotItem>();
                    }
                    break;
            }

        }
        void Exchange_Close()
        {
            try
            {
                Systems sys = GetPlayer(Karakter.Network.TargetID);
                client.Send(Private.Packet.Exchange_Cancel());
                client.Send(Public.Packet.CloseExhangeWindow());
                Karakter.Network.Exchange.Window = false;
                Karakter.Network.Exchange.ItemList = null;

                if (sys != null)
                {
                    sys.client.Send(Private.Packet.Exchange_Cancel());
                    sys.client.Send(Public.Packet.CloseExhangeWindow());
                    sys.Karakter.Network.Exchange.Window = false;
                    sys.Karakter.Network.Exchange.ItemList = null;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

        }
        void Exchange_Accept()
        {
            Systems sys = GetPlayer(Karakter.Network.TargetID);
            client.Send(Private.Packet.Exchange_ItemPacket(this.Karakter.Information.UniqueID, this.Karakter.Network.Exchange.ItemList, true));
            client.Send(Private.Packet.Exchange_Accept());

            sys.client.Send(Private.Packet.Exchange_ItemPacket(this.Karakter.Information.UniqueID, this.Karakter.Network.Exchange.ItemList, false));
            sys.client.Send(Private.Packet.Exchange_Gold(Karakter.Network.Exchange.Gold));
            sys.client.Send(Private.Packet.Exchange_Accept2());
        }
        void Exchange_Approve()
        {
            Systems sys = GetPlayer(Karakter.Network.TargetID);
            client.Send(Private.Packet.Exchange_Approve());
            Karakter.Network.Exchange.Approved = true;
            if (sys.Karakter.Network.Exchange.Approved)
            {
                #region Gold update
                if (Karakter.Network.Exchange.Gold != 0)
                {
                    Karakter.Information.Gold -= Karakter.Network.Exchange.Gold;
                    client.Send(Private.Packet.UpdateGold(Karakter.Information.Gold));
                    SaveGold();

                    sys.Karakter.Information.Gold += Karakter.Network.Exchange.Gold;
                    sys.client.Send(Private.Packet.UpdateGold(sys.Karakter.Information.Gold));
                    sys.SaveGold();
                }
                if (sys.Karakter.Network.Exchange.Gold != 0)
                {
                    sys.Karakter.Information.Gold -= sys.Karakter.Network.Exchange.Gold;
                    sys.client.Send(Private.Packet.UpdateGold(sys.Karakter.Information.Gold));
                    sys.SaveGold();

                    Karakter.Information.Gold += sys.Karakter.Network.Exchange.Gold;
                    client.Send(Private.Packet.UpdateGold(Karakter.Information.Gold));
                    SaveGold();
                }
                #endregion

                #region Gold
                if (Karakter.Network.Exchange.ItemList.Count > 0)
                {
                    foreach (Global.slotItem item in Karakter.Network.Exchange.ItemList)
                    {
                        byte t_slot = sys.GetFreeSlot();
                        if (Data.ItemBase[item.ID].Class_D == 1)
                        {
                            MsSQL.UpdateData("Insert Into char_items (itemid,plusvalue,durability,owner,itemnumber,slot,type) VALUES ('" + item.ID + "','" + item.PlusValue + "','" + Data.ItemBase[item.ID].Defans.Durability + "','" + sys.Karakter.Information.CharacterID + "','item" + t_slot + "','" + t_slot + "','" + 0 + "' )");
                        }
                        else if (Data.ItemBase[item.ID].Class_D == 3)
                        {
                            MsSQL.UpdateData("Insert Into char_items (itemid,quantity,owner,itemnumber,slot,type) VALUES ('" + item.ID + "','" + item.Amount + "','" + sys.Karakter.Information.CharacterID + "','item" + t_slot + "','" + t_slot + "','" + 1 + "' )");
                        }
                        MsSQL.DeleteData("delete from char_items where id='" + item.dbID + "'");
                    }
                }

                if (sys.Karakter.Network.Exchange.ItemList.Count > 0)
                {
                    foreach (Global.slotItem item in sys.Karakter.Network.Exchange.ItemList)
                    {
                        byte t_slot = GetFreeSlot();
                        if (Data.ItemBase[item.ID].Class_D == 1)
                        {
                            MsSQL.UpdateData("Insert Into char_items (itemid,plusvalue,durability,owner,itemnumber,slot,type) VALUES ('" + item.ID + "','" + item.PlusValue + "','" + Data.ItemBase[item.ID].Defans.Durability + "','" + Karakter.Information.CharacterID + "','item" + t_slot + "','" + t_slot + "','" + 0 + "' )");
                        }
                        else if (Data.ItemBase[item.ID].Class_D == 3)
                        {
                            MsSQL.UpdateData("Insert Into char_items (itemid,quantity,owner,itemnumber,slot,type) VALUES ('" + item.ID + "','" + item.Amount + "','" + Karakter.Information.CharacterID + "','item" + t_slot + "','" + t_slot + "','" + 1 + "' )");
                        }
                        MsSQL.DeleteData("delete from char_items where id='" + item.dbID + "'");
                    }
                }
                #endregion

                client.Send(Private.Packet.Exchange_Finish());
                sys.client.Send(Private.Packet.Exchange_Finish());
            }
        }
    }
}
