using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Framework;

namespace Game
{
    public partial class Systems
    {
        public void AlchemyMain()
        {
            PacketReader Reader = new PacketReader(PacketInformation.buffer);
            Karakter.Alchemy.ItemList = new List<Global.slotItem>();

            byte Type = Reader.Byte();

            if (Type == 1)
            {
                try
                {
                    Karakter.Alchemy.AlchemyThread.Abort();
                    client.Send(Private.Packet.AlchemyCancel());
                }
                catch (Exception ex)
                {
                    deBug.Write(ex);
                }
            }
            else if (Type == 2)
            {
                Reader.Skip(1);
                byte numItem = Reader.Byte();

                if (numItem == 2)
                {
                    Karakter.Alchemy.ItemList.Add(GetItem((uint)Karakter.Information.CharacterID, Reader.Byte(), false));
                    Karakter.Alchemy.ItemList.Add(GetItem((uint)Karakter.Information.CharacterID, Reader.Byte(), false));

                }
                else if (numItem == 3)
                {
                    Karakter.Alchemy.ItemList.Add(GetItem((uint)Karakter.Information.CharacterID, Reader.Byte(), false));
                    Karakter.Alchemy.ItemList.Add(GetItem((uint)Karakter.Information.CharacterID, Reader.Byte(), false));
                    Karakter.Alchemy.ItemList.Add(GetItem((uint)Karakter.Information.CharacterID, Reader.Byte(), false));
                }

                Karakter.Alchemy.AlchemyThread = new Thread(new ThreadStart(AlchemyResponse));
                Karakter.Alchemy.AlchemyThread.Start();
                while (!Karakter.Alchemy.AlchemyThread.IsAlive) ;
                Thread.Sleep(1);

            }
        }
        
        public void AlchemyResponse()
        {
            try
            {
                Thread.Sleep(3000);

                Random plus = new Random();
                int random = plus.Next(1, 100);
                int chance = 0;
                bool success = false;

                // successrate table
                switch (Karakter.Alchemy.ItemList[0].PlusValue)
                {
                    case 0: chance = 95;
                        break;
                    case 1: chance = 60;
                        break;
                    case 2: chance = 53;
                        break;
                    case 3: chance = 41;
                        break;
                    case 4: chance = 34;
                        break;
                    case 5: chance = 31;
                        break;
                    case 6: chance = 27;
                        break;
                    case 7: chance = 21;
                        break;
                    case 8: chance = 13;
                        break;
                    case 9: chance = 8;
                        break;
                    default: chance = 5;
                        break;
                }

                // if with lucky
                if (Karakter.Alchemy.ItemList.Count == 3)
                {
                    chance += 5; // +5% :))

                    // dec lucky powder amount
                    Karakter.Alchemy.ItemList[2].Amount--;
                    ItemUpdateAmount(Karakter.Alchemy.ItemList[2], Karakter.Information.CharacterID);
                }

                // success or not :P
                if (random > (100 - chance))
                    success = true;

                // update plus value
                if (success)
                {
                    Karakter.Alchemy.ItemList[0].PlusValue++;
                    MsSQL.InsertData("UPDATE char_items SET plusvalue='" + Karakter.Alchemy.ItemList[0].PlusValue + "' WHERE slot='" + Karakter.Alchemy.ItemList[0].Slot + "' AND owner='" + Karakter.Information.CharacterID + "'");
                }
                else
                {
                    Karakter.Alchemy.ItemList[0].PlusValue = 0;
                    MsSQL.InsertData("UPDATE char_items SET plusvalue='0' WHERE slot ='" + Karakter.Alchemy.ItemList[0].Slot + "' AND owner='" + Karakter.Information.CharacterID + "'");
                }

                client.Send(Private.Packet.AlchemyResponse(success, Karakter.Alchemy.ItemList[0]));

                //delete elixir
                MsSQL.InsertData("DELETE FROM char_items WHERE slot='" + Karakter.Alchemy.ItemList[1].Slot + "' AND owner='" + Karakter.Information.CharacterID + "'");
                client.Send(Private.Packet.ItemDelete2(0xF, Karakter.Alchemy.ItemList[1].Slot));

            }
            catch (Exception ex)
            {
                Console.WriteLine("Alchemy Terminated...");
                deBug.Write(ex);
            }

        }
    }
}
