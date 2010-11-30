using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework;
using System.Data.SqlClient;
namespace Data
{
    public partial class Systems
    {
        internal Client client;
        internal Decode PacketInformation;
        private byte WrongForPassword;
        static short User_Current;
        public static List<NewsList> News_List = new List<NewsList>();
        public Systems(Client de)
        {
            client = de;
        }
        public static void oPCode(Decode de)
        {
            Systems sys = (Systems)de.Packet;
            sys.PacketInformation = de;
            try
            {
                switch (de.opcode)
                {
                    case 0x2001:
                        sys.GateWay();
                        break;
                    case 0x6100:
                        sys.Patch();
                        break;
                    case 0x6101:
                        sys.ServerList();
                        break;
                    case 0x6102:
                        sys.Connect();
                        break;
                    case 0x6104:
                        sys.Launcher();
                        break;
                    case 1905:
                        UserTablo(de.buffer);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        static void UserTablo(byte[] buffer)
        {
            User_Current = BitConverter.ToInt16(buffer, 0);
        }
        public void GateWay()
        {
            PacketReader Reader = new PacketReader(PacketInformation.buffer);

            if(Reader.Text() == "SR_Client")
                client.Send(Data.Systems.GateWayPacket());


        }
        public void Patch()
        {
            Console.WriteLine(Decode.StringToPack(PacketInformation.buffer));
            PacketReader Reader = new PacketReader(PacketInformation.buffer);
            Reader.Skip(1);
            if (Reader.Text() == "SR_Client")
            {
                int version = Reader.Int32();
                if (version == current_version)
                {
                    client.Send(Data.Systems.LoadGame_1());
                    client.Send(Data.Systems.LoadGame_2());
                    client.Send(Data.Systems.LoadGame_3());
                    client.Send(Data.Systems.LoadGame_4());
                    client.Send(Data.Systems.LoadGame_5());
                    client.Send(Data.Systems.Version_1());
                }
            }
        }
        public void ServerList()
        {
            client.Send(Data.Systems.ServerListPacket());
        }
        public void Launcher()
        {
            client.Send(Data.Systems.LoadGame_7());
            client.Send(Data.Systems.NewsListPacket());

        }
        public static byte[] ToByteArray(String HexString)
        {
            if (HexString.Length % 2 == 0)
            {
                int NumberChars = HexString.Length;
                byte[] bytes = new byte[NumberChars / 2];
                for (int i = 0; i < NumberChars; i += 2)
                {
                    bytes[i / 2] = Convert.ToByte(HexString.Substring(i, 2), 16);
                }
                return bytes;
            }
            else
            {
                Console.WriteLine("Packet Error: " + HexString);
                return null;
            }
        }
        public void Connect()
        {
            try
            {
                PacketReader Reader = new PacketReader(PacketInformation.buffer);
                if (Reader.Byte() == 2)
                {
                    string ID = Reader.Text();
                    string PW = Reader.Text();
                    Reader.Int16();
                    ushort ServerID = Reader.UInt16();

                    bool login = false;

                    MsSQL ms = new MsSQL("SELECT * FROM users WHERE id='" + ID + "'");
                    using (SqlDataReader reader = ms.Read())
                    {
                        while (reader.Read())
                        {
                            if (reader.GetByte(3) == 1) // karakter banlanmisa
                            {
                                client.Disconnect(PacketInformation.Client);
                                return;
                            }
                            if (reader.GetByte(4) == 1) // karakterin oyunda olup olmadıgını kontrol et
                            {
                                client.Send(ConnectWrong(0x0302));
                                client.Disconnect(PacketInformation.Client);
                                return;
                            }
                            if (CheckCrowed(ServerID)) { client.Disconnect(PacketInformation.Client); return; }
                            if (WrongForPassword >= 3)
                            {
                                client.Disconnect(PacketInformation.Client);
                                return;
                            }
                            if (reader.GetString(1).ToLower() == ID.ToLower() && !cMD5.Equals(PW, reader.GetString(2))) // PW yanlisa
                            {
                                WrongForPassword++;
                                client.Send(WorngInformation());
                                if (WrongForPassword >= 3)
                                {
                                    client.Disconnect(PacketInformation.Client);
                                    return;
                                }
                            }
                            if (reader.GetString(1).ToLower() == ID.ToLower() && cMD5.Equals(PW, reader.GetString(2))) // PW dogruysa
                            {
                                login = true;
                            }

                        }
                    }
                    ms.Close();
                    if (login)
                    {
                        ms = new MsSQL("SELECT * FROM server WHERE serverid='" + ServerID + "'");
                        using (SqlDataReader reader = ms.Read())
                        {
                            while (reader.Read())
                            {
                                if (User_Current >= reader.GetInt16(4)) // server dolu ise.
                                {
                                    client.Send(ConnectWrong(0x0402));
                                    client.Disconnect(PacketInformation.Client);
                                    return;
                                }
                                {
                                    client.Send(ConnectSucces(reader.GetString(6), reader.GetInt16(7), reader.GetByte(8)));
                                    return;
                                }
                            }
                        }
                        ms.Close();
                    }
                    else
                    {
                        client.Send(ConnectWrong(0x0402));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.ReadLine();
            }
        }
        bool CheckCrowed(ushort serverid)
        {
            MsSQL ms = new MsSQL("SELECT * FROM server WHERE serverid='" + serverid + "'");
            using (SqlDataReader reader = ms.Read())
            {
                while (reader.Read())
                    if (User_Current >= reader.GetInt16(4)) { ms.Close(); return true; }
            }
            ms.Close();
            return false;
        }
    }
}
