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
        private static uint current_version = 702;
        public static byte[] GateWayPacket()
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(0x2001);
            Writer.Text("GatewayServer");
            Writer.Byte(0);
            return Writer.GetBytes();
        }
        public static byte[] LoadGame_1()
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(0x600D);
            Writer.Word(0x0101);
            Writer.Word(0x0500);
            Writer.Byte(0x20);
            return Writer.GetBytes();
        }
        public static byte[] LoadGame_2()
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(0x600D);
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
            Writer.Create(0x600D);
            Writer.Word(0x0101);
            Writer.Word(0x0500);
            Writer.Byte(0x60);
            return Writer.GetBytes();
        }
        public static byte[] LoadGame_4()
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(0x600D);
            Writer.Word(0x0300);
            Writer.Word(0x0200);
            Writer.Word(0x0200);
            return Writer.GetBytes();
        }
        public static byte[] LoadGame_5()
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(0x600D);
            Writer.Word(0x0101);
            Writer.Word(0);
            Writer.Byte(0xA1);
            return Writer.GetBytes();
        }
        public static byte[] LoadGame_6()
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(0x600D);
            Writer.Word(0x0100);
            return Writer.GetBytes();
        }
        public static byte[] Version_1() // if server version = client version
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(0x600D);
            Writer.Word(0x0100);
            return Writer.GetBytes();
        }
        public static byte[] Version_2() // if server version > client version, here we can implement a download server
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(0x600D);
            Writer.Byte(0);
            Writer.Byte(2);
            Writer.Byte(2);
            Ini config = new Ini(Environment.CurrentDirectory + @"\config\server_login.ini");
            string ip = config.GetValue("DOWNLOADSERVER", "ip").ToString();
            Writer.Byte((byte)ip.Length); // ip lenght            
            Writer.Text(ip); // DownloadServer IP
            Writer.Word(16002); // DownloadServer port
            Writer.DWord(current_version);

            // TODO: list new files here

            return Writer.GetBytes();
        }
        public static byte[] LoadGame_7()
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(0x600D);
            Writer.Byte(1);
            Writer.Byte(1);
            Writer.Byte(0);
            Writer.Byte(4);
            Writer.Byte(0xA1);
            return Writer.GetBytes();
        }
        public static byte[] NewsListPacket()
        {
            PacketWriter Writer = new PacketWriter();

            Writer.Create(0x600D);
            Writer.Byte(0);
            Writer.Byte((byte)Systems.News_List.Count);

            foreach (NewsList n in Systems.News_List)
            {
                Writer.Text(n.Head);
                Writer.Text(n.Msg);
                Writer.Word(0);

                Writer.Word(n.Year);
                Writer.Word(n.Month);

                Writer.Word(0);
                Writer.LWord(0);
            }

            Writer.Word(0); // close pack

            return Writer.GetBytes();
        }
        public static byte[] ServerListPacket()
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(0xA101);
            Writer.Word(0x0201);
            Writer.Text("Silkroad_Korea_Yahoo_Official"); 
            Writer.Byte(0);
            MsSQL ms = new MsSQL("SELECT * FROM server");
            using (System.Data.SqlClient.SqlDataReader reader = ms.Read())
            {
                while (reader.Read())
                {
                    Writer.Bool(true);
                    Writer.Word(reader.GetInt16(1)); //server id
                    Writer.Text(reader.GetString(2));
                    Writer.Word(User_Current);
                    Writer.Word(reader.GetInt16(4));
                    Writer.Byte(reader.GetByte(5)); // Server Status
                }
            }
            ms.Close();

            Writer.Byte(0);
            return Writer.GetBytes();
        }
        public byte[] WorngInformation()
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(0xA102);
            Writer.Byte(2); //failed
            Writer.Byte(1); //wrong password
            Writer.Byte(WrongForPassword);
            Writer.Word(0);
            Writer.Byte(0);
            Writer.Byte(3); //Max Wrong Password/Username
            Writer.Word(0);
            Writer.Byte(0);
            return Writer.GetBytes();
        }
        public static byte[] ConnectWrong(ushort type)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(0xA102);
            Writer.Word(type);
            return Writer.GetBytes();
        }
        public static byte[] ConnectSucces(string ip, short port, byte type)
        {
            //01 99 00 0000 0E003132312E3235342E3135332E3132 A33D 03
            PacketWriter Writer = new PacketWriter();
            Writer.Create(0xA102);
            Writer.Byte(1);
            Writer.Byte(type);
            Writer.Byte(0);
            Writer.Word(0);
            Writer.Text(ip);
            Writer.Word(port);
            Writer.Byte(3);
            return Writer.GetBytes();
        }
        public static byte[] ConnectTest()
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(0xA323);
            Writer.Byte(1);
            return Writer.GetBytes();
        }
    }
}
