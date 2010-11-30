using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework;

namespace Game
{
    public partial class Systems
    {
        //No permission = a party tagjai nem tudnak invelni, csak a party leader.
        public enum PartyTypes { NONSHARE_NO_PERMISSION, EXPSHARE_NO_PERMISSION, ITEMSHARE_NO_PERMISSION, FULLSHARE_NO_PERMISSION,
        NONSHARE, EXPSHARE, ITEMSHARE, FULLSHARE};
        public enum PartyPurpose { HUNTING, QUEST, TRADE, THIEF };
        void CreateFormedParty()
        {
            /*foreach (party partik in Party)
            {
                if (partik.Members.Contains(this.Karakter.Information.UniqueID)) return;
            }*/
            PacketReader Reader = new PacketReader(PacketInformation.buffer);
            Print.Format(Decode.StringToPack(PacketInformation.buffer));
            party newparty = new party();
            newparty.IsFormed = true;
            Reader.Skip(8); /*a következőket skipeli: 
                             * 00 00 00 00 (<-- pt ID, de ha nem formolt a pt akkor 0 az értéke. Ez a party szerkesztéséhez kell majd.)
                             * 00 00 00 00
                             */
            newparty.Type = Reader.Byte();
            newparty.ptpurpose = Reader.Byte();
            newparty.minlevel = Reader.Byte();
            newparty.maxlevel = Reader.Byte(); //a mi kliensünknél 105 lehet, packeteknél ez 69 ként jelenik meg. (új ksro-nál max 6E  lehet, azaz 110)
            short strlen = Reader.Int16();
            newparty.partyname = Reader.String(strlen);
            newparty.LeaderID = this.Karakter.Information.UniqueID;
            newparty.Members.Add(this.Karakter.Information.UniqueID);
            newparty.MembersClient.Add(this.client);
            newparty.ptid = Party.Count+1;
            Party.Add(newparty);
            Print.Format("Új party: \n Típusa: {0} \n LeaderID: {1} \n Száma: {2}", newparty.Type, newparty.LeaderID, newparty.ptid);
            client.Send(Public.Packet.CreateFormedParty(newparty));
        }
        void ListPartyMatching()
        {
            client.Send(Public.Packet.ListPartyMatching(Party));
        }
        void DeleteFormedParty()
        {
            PacketReader Reader = new PacketReader(PacketInformation.buffer);
            int partyid = Reader.Int32();

            Party.Remove(
            Party.Find(
            delegate(party pt)
            {
                return pt.IsFormed && (pt.ptid == partyid);
            }
            ));

            client.Send(Public.Packet.DeleteFormedParty(partyid));
        }
    }
    public sealed partial class party
    {
        public string partyname = "";
        public int ptid = 0;
        public byte ptpurpose;
        //Levelrange:
        public byte minlevel;
        public byte maxlevel;
    }
}
