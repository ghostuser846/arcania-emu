using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework;
namespace Game
{
    public partial class Systems
    {
        public void Patch()
        {
            client.Send(Private.Packet.AgentServer());
            client.Send(Private.Packet.LoadGame_1());
            client.Send(Private.Packet.LoadGame_2());
            client.Send(Private.Packet.LoadGame_3());
            client.Send(Private.Packet.LoadGame_4());
            client.Send(Private.Packet.LoadGame_5());
            client.Send(Private.Packet.LoadGame_6());
        }
        public void Connect()
        {
            try
            {
                PacketReader Reader = new PacketReader(PacketInformation.buffer);

                byte type = Reader.Byte();
                Reader.Skip(3);
                string ID = Reader.Text();
                string PW = Reader.Text();
                Reader.Close();

                MsSQL ms = new MsSQL("SELECT * FROM users WHERE id='" + ID + "'");

                using (System.Data.SqlClient.SqlDataReader reader = ms.Read())
                {
                    bool login = false;
                    //TODO: Átírni a CheckCrowded-nek átadott számot, a jelenlegi gameserver id-jére, a jelenlegi gameserver kérdezze le a saját id-jét, és azt adja oda neki.
                    if (CheckCrowed(1905)) return;
                    while (reader.Read())
                    {
                        if (reader.GetByte(3) == 1) // karakter banlanmisa
                        {
                            client.Disconnect(PacketInformation.Client);
                            return;
                        }
                        if (reader.GetByte(4) == 1) // karakterin oyunda olup olmadıgını kontrol et
                        {
                            client.Disconnect(PacketInformation.Client);
                            return;
                        }
                        if (reader.GetString(1) == ID && !cMD5.Equals(PW, reader.GetString(2))) // PW yanlisa
                        {
                            client.Disconnect(PacketInformation.Client);
                            return;
                        }
                        if (reader.GetString(1).ToLower() == ID.ToLower() && cMD5.Equals(PW, reader.GetString(2))) // PW dogruysa
                        {
                            login = true;
                        }

                        if (login)
                        {
                            if (Player == null) MsSQL.UpdateData("UPDATE users SET online='" + 1 + "' WHERE id='" + ID + "'");
                            Player = new player();
                            Player.AccountName = ID;
                            Player.Password = PW;
                            Player.ID = reader.GetInt32(0);
                            Player.pGold = reader.GetInt64(7);
                            Player.Silk = reader.GetInt32(6);
                            //Player.ID = reader.GetInt32(0);

                            client.Send(Private.Packet.ConnectSuccess());
                        }
                    }
                }
                ms.Close();
            }
            catch(Exception ex)
            {
                Console.WriteLine("Connect()::error..");
                deBug.Write(ex);
            }
        }
        bool CheckCrowed(ushort serverid)
        {
            /*MsSQL ms = new MsSQL("SELECT * FROM server WHERE serverid='" + serverid + "'");
            using (System.Data.SqlClient.SqlDataReader reader = ms.Read())
            {
                while (reader.Read())
                {
                    if (clients.Count >= reader.GetInt16(4)) { ms.Close(); return true; }
                }
            }
            ms.Close();*/
            if (clients.Count >= 100) { return true; }
            return false;
        }
        public void CharacterScreen()
        {
            PacketReader Reader = new PacketReader(PacketInformation.buffer);
            switch (Reader.Byte())
            {
                case 1:
                    CharacterCreate();
                    break;
                case 2:
                    CharacterListing();
                    break;
                case 3:
                    CharacterDelete();
                    break;
                case 4:
                    CharacterCheck(PacketInformation.buffer);
                    break;
                case 5:
                    CharacterRestore();
                    break;
            }
            Reader.Close();
        }
    }
}
