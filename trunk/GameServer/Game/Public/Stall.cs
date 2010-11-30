using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework;
using System.Threading;

namespace Game
{
    public partial class Systems
    {
        public void Stalllol()
        {
            PacketReader Reader = new PacketReader(PacketInformation.buffer);
            byte type = Reader.Byte();
            switch (type)
            {
                case 6:
                Karakter.Stall.StallThread = new Thread(new ThreadStart(Stallopen));
                Karakter.Stall.StallThread.Start();
                Thread.Sleep(1);
                    break;
                case 2:
                Karakter.Stall.StallThread = new Thread(new ThreadStart(StallAddItem));
                Karakter.Stall.StallThread.Start();
                Thread.Sleep(1);
                break;
            }
        }
        public void Stallopen()
        {
            PacketReader Reader = new PacketReader(PacketInformation.buffer);
            
            Karakter.Stall.ItemList = new List<Global.slotItem>();
            byte type = Reader.Byte();
                short Namelength = Reader.Int16();
                string Stallname = Reader.String(Namelength);
                Console.WriteLine("Új Stall, név:" + Stallname);
                Karakter.Stall.StallName.Add(Stallname);
                client.Send(Private.Packet.Stallopenresponse(Namelength, Stallname, Karakter.Information.UniqueID));
           
        }
        public  void StallWelcome()
        {
            PacketReader Reader = new PacketReader(PacketInformation.buffer);
            byte Type = Reader.Byte();
            if (Type == 6)
            {
                short length = Reader.Int16();
                string welcome = Reader.String(length);
                Console.WriteLine("Welcome üzenet:" + welcome);
                
                client.Send(Private.Packet.Stallopened());
                client.Send(Private.Packet.Stallopened2(length, welcome));
            }
            
            
        }
        public void StallAddItem()
        {
            PacketReader Reader = new PacketReader(PacketInformation.buffer);

            Karakter.Stall.ItemList = new List<Global.slotItem>();
            byte type = Reader.Byte();
            
                byte stallpos = Reader.Byte();
                byte invpos = Reader.Byte();
                byte itemnum = Reader.Byte();
                int price = Reader.Int32();
                short itemid = Reader.Int16();
                client.Send(Private.Packet.Stallitemadd(itemnum, invpos, price));
        }
    }
}
