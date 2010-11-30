using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework;

namespace Game
{
    public partial class Systems
    {
        public void ObjectSpawnCheck()
        {
            try
            {
                ObjectDeSpawnCheck();

                if (this.Karakter != null && !this.Karakter.Spawning)
                {
                    this.Karakter.Spawning = true;
                    for (int i = 0; i < Systems.HelperObject.Count; i++)
                    {
                        if (Systems.HelperObject[i] != null && !Systems.HelperObject[i].Spawned(this.Karakter.Information.UniqueID))
                        {
                            if (this.Karakter.Position.x >= (Systems.HelperObject[i].x - 50) && this.Karakter.Position.x <= ((Systems.HelperObject[i].x - 50) + 100) && this.Karakter.Position.y >= (Systems.HelperObject[i].y - 50) && this.Karakter.Position.y <= ((Systems.HelperObject[i].y - 50) + 100))
                            {
                                if (!Systems.HelperObject[i].Spawned(this.Karakter.Information.UniqueID) && Systems.HelperObject[i].UniqueID != 0)
                                {
                                    Systems.HelperObject[i].Spawn.Add(this.Karakter.Information.UniqueID);
                                    client.Send(Public.Packet.ObjectSpawn(Systems.HelperObject[i]));
                                }
                            }
                        }
                    }

                    for (int i = 0; i < Systems.Objects.Count; i++)
                    {
                        if (Systems.Objects[i] != null && !Systems.Objects[i].Die && !Systems.Objects[i].Spawned(Karakter.Information.UniqueID))
                        {
                            if (Systems.Objects[i].x >= (Karakter.Position.x - 50) && Systems.Objects[i].x <= ((Karakter.Position.x - 50) + 100) && Systems.Objects[i].y >= (Karakter.Position.y - 50) && Systems.Objects[i].y <= ((Karakter.Position.y - 50) + 100))
                            {
                                if (!Systems.Objects[i].Spawned(Karakter.Information.UniqueID) && Systems.Objects[i].UniqueID != 0 && Systems.Objects[i].ID != 0)
                                {
                                    Systems.Objects[i].Spawn.Add(Karakter.Information.UniqueID);
                                    client.Send(Public.Packet.ObjectSpawn(Systems.Objects[i]));
                                }
                            }
                        }
                    }

                    for (int i = 0; i < Systems.clients.Count; i++)
                    {
                        Systems s = Systems.clients[i];
                        if (s != null && s != this && !Karakter.Spawned(s.Karakter.Information.UniqueID) && s.Karakter.Information.Name != this.Karakter.Information.Name)
                        {
                            if (s.Karakter.Position.x >= (Karakter.Position.x - 50) && s.Karakter.Position.x <= ((Karakter.Position.x - 50) + 100) && s.Karakter.Position.y >= (Karakter.Position.y - 50) && s.Karakter.Position.y <= ((Karakter.Position.y - 50) + 100))
                            {
                                if (!Karakter.Spawned(s.Karakter.Information.UniqueID) && s.Karakter.Information.UniqueID != 0 && s.Karakter.Information.UniqueID != this.Karakter.Information.UniqueID)
                                {
                                    Karakter.Spawn.Add(s.Karakter.Information.UniqueID);
                                    client.Send(Public.Packet.ObjectSpawn(s.Karakter));
                                    if (s.Karakter.Position.Walking && !s.Karakter.Transport.Right) 
                                        client.Send(Public.Packet.Movement(new Global.vektor(s.Karakter.Information.UniqueID, s.Karakter.Position.packetX, s.Karakter.Position.packetZ, s.Karakter.Position.packetY, s.Karakter.Position.packetxSec, s.Karakter.Position.packetySec)));
                                    else if(s.Karakter.Position.Walking && s.Karakter.Transport.Right)
                                        client.Send(Public.Packet.Movement(new Global.vektor(s.Karakter.Transport.Horse.UniqueID, s.Karakter.Position.packetX, s.Karakter.Position.packetZ, s.Karakter.Position.packetY, s.Karakter.Position.packetxSec, s.Karakter.Position.packetySec)));
                                }
                                ObjectPlayerSpawn(s);
                            }
                        }
                    }

                    for (int i = 0; i < Systems.WorldItem.Count; i++)
                    {
                        if (Systems.WorldItem[i] != null && !Systems.WorldItem[i].Spawned(Karakter.Information.UniqueID))
                        {
                            if (Systems.WorldItem[i].x >= (Karakter.Position.x - 50) && Systems.WorldItem[i].x <= ((Karakter.Position.x - 50) + 100) && Systems.WorldItem[i].y >= (Karakter.Position.y - 50) && Systems.WorldItem[i].y <= ((Karakter.Position.y - 50) + 100))
                            {
                                if (!Systems.WorldItem[i].Spawned(Karakter.Information.UniqueID) && Systems.WorldItem[i].UniqueID != 0 && Systems.WorldItem[i].Model != 0)
                                {
                                    Systems.WorldItem[i].Spawn.Add(Karakter.Information.UniqueID);
                                    client.Send(Public.Packet.ObjectSpawn(Systems.WorldItem[i]));
                                }
                            }
                        }
                    }
                    if (Karakter.Transport.Right && !Karakter.Transport.Spawned)
                    {
                        if (!Karakter.Transport.Spawned)
                        {
                            Karakter.Transport.Spawned = true;
                            Karakter.Transport.Horse.SpawnMe();
                            Karakter.Transport.Horse.Send(Public.Packet.Player_UpToHorse(this.Karakter.Information.UniqueID, true, Karakter.Transport.Horse.UniqueID));
                        }
                    }

                    this.Karakter.Spawning = false;
                }
            }
            catch (Exception ex)
            {
                this.Karakter.Spawning = false;
                Console.WriteLine("ObjectSpawnCheck()::error...");
                deBug.Write(ex);
            }
        }
        void ObjectPlayerSpawn(Systems s)
        {
            if (!s.Karakter.Spawned(this.Karakter.Information.UniqueID) && this.Karakter.Information.UniqueID != 0 && !s.Karakter.Spawning)
            {
                /*s.Karakter.Spawn.Add(this.Karakter.Information.UniqueID);
                s.client.Send(Public.Packet.ObjectSpawn(this.Karakter));
                if (Karakter.Position.Walking && !Karakter.Transport.Right) 
                    s.client.Send(Public.Packet.Movement(new Global.vektor(Karakter.Information.UniqueID, Karakter.Position.packetX, Karakter.Position.packetZ, Karakter.Position.packetY, Karakter.Position.packetxSec, Karakter.Position.packetySec)));
                else if(Karakter.Position.Walking && Karakter.Transport.Right)
                    s.client.Send(Public.Packet.Movement(new Global.vektor(Karakter.Transport.Horse.UniqueID, Karakter.Position.packetX, Karakter.Position.packetZ, Karakter.Position.packetY, Karakter.Position.packetxSec, Karakter.Position.packetySec)));*/
                s.ObjectSpawnCheck();
            }
        }
        public void ObjectDeSpawnCheck()
        {
            try
            {
                if (this.Karakter != null && !this.Karakter.deSpawning && this != null)
                {
                    this.Karakter.deSpawning = true;

                    for (int i = 0; i < Systems.HelperObject.Count; i++)
                    {
                        if (Systems.HelperObject[i] != null && Systems.HelperObject[i].Spawned(this.Karakter.Information.UniqueID))
                        {
                            if (this.Karakter.Position.x >= (Systems.HelperObject[i].x - 50) && this.Karakter.Position.x <= ((Systems.HelperObject[i].x - 50) + 100) && this.Karakter.Position.y >= (Systems.HelperObject[i].y - 50) && this.Karakter.Position.y <= ((Systems.HelperObject[i].y - 50) + 100))
                            {
                                //asd
                            }
                            else
                            {
                                if (Systems.HelperObject[i].Spawned(this.Karakter.Information.UniqueID) && Systems.HelperObject[i].UniqueID != 0)
                                {
                                    Systems.HelperObject[i].Spawn.Remove(this.Karakter.Information.UniqueID);
                                    client.Send(Public.Packet.ObjectDeSpawn(Systems.HelperObject[i].UniqueID));
                                }
                            }
                        }
                    }

                    for (int i = 0; i < Systems.Objects.Count; i++)
                    {
                        if (Systems.Objects[i] != null)
                        {
                            if (Systems.Objects[i].Spawned(Karakter.Information.UniqueID) && !Systems.Objects[i].Die)
                            {
                                if (Systems.Objects[i].x >= (Karakter.Position.x - 50) && Systems.Objects[i].x <= ((Karakter.Position.x - 50) + 100) && Systems.Objects[i].y >= (Karakter.Position.y - 50) && Systems.Objects[i].y <= ((Karakter.Position.y - 50) + 100))
                                {
                                }
                                else
                                {
                                    if (Systems.Objects[i].Spawned(Karakter.Information.UniqueID) && Systems.Objects[i].UniqueID != 0 && Systems.Objects[i].ID != 0)
                                    {
                                        Systems.Objects[i].Spawn.Remove(Karakter.Information.UniqueID);
                                        client.Send(Public.Packet.ObjectDeSpawn(Systems.Objects[i].UniqueID));
                                    }
                                }
                            }
                        }
                    }

                    for (int i = 0; i < Systems.clients.Count; i++)
                    {
                        if (Systems.clients[i] != null && Systems.clients[i] != this && Karakter.Spawned(Systems.clients[i].Karakter.Information.UniqueID))
                        {
                            if (Systems.clients[i].Karakter.Position.x >= (Karakter.Position.x - 50) && Systems.clients[i].Karakter.Position.x <= ((Karakter.Position.x - 50) + 100) && Systems.clients[i].Karakter.Position.y >= (Karakter.Position.y - 50) && Systems.clients[i].Karakter.Position.y <= ((Karakter.Position.y - 50) + 100))
                            {
                            }
                            else
                            {
                                if (Karakter.Spawned(Systems.clients[i].Karakter.Information.UniqueID))
                                {
                                    Karakter.Spawn.Remove(Systems.clients[i].Karakter.Information.UniqueID);
                                    client.Send(Public.Packet.ObjectDeSpawn(Systems.clients[i].Karakter.Information.UniqueID));
                                }
                                ObjectDePlayerSpawn(Systems.clients[i]);
                            }
                        }
                    }

                    for (int i = 0; i < Systems.WorldItem.Count; i++)
                    {
                        if (Systems.WorldItem[i] != null && Systems.WorldItem[i].Spawned(Karakter.Information.UniqueID) && Systems.WorldItem[i].UniqueID != 0)
                        {
                            if (Systems.WorldItem[i].x >= (Karakter.Position.x - 50) && Systems.WorldItem[i].x <= ((Karakter.Position.x - 50) + 100) && Systems.WorldItem[i].y >= (Karakter.Position.y - 50) && Systems.WorldItem[i].y <= ((Karakter.Position.y - 50) + 100))
                            {
                            }
                            else
                            {
                                if (Systems.WorldItem[i].Spawned(Karakter.Information.UniqueID) && Systems.WorldItem[i].Model != 0 && Systems.WorldItem[i].UniqueID != 0)
                                {
                                    Systems.WorldItem[i].Spawn.Remove(Karakter.Information.UniqueID);
                                    client.Send(Public.Packet.ObjectDeSpawn(Systems.WorldItem[i].UniqueID));
                                }
                            }
                        }
                    }
                    this.Karakter.deSpawning = false;
                }
            }
            catch (Exception ex)
            {
                this.Karakter.deSpawning = false;
                Console.WriteLine("ObjectDeSpawnCheck()::error...");
                deBug.Write(ex);
            }
        }

        void ObjectDePlayerSpawn(Systems s)
        {
            if (s.Karakter.Spawned(this.Karakter.Information.UniqueID) && !s.Karakter.deSpawning)
            {
                /*s.Karakter.Spawn.Remove(this.Karakter.Information.UniqueID);
                s.client.Send(Public.Packet.ObjectDeSpawn(this.Karakter.Information.UniqueID));*/
                s.ObjectDeSpawnCheck();
            }
        }
        void ObjectAttackCheck()
        {
            for (int i = 0; i < Systems.Objects.Count; i++)
            {
                if (Systems.Objects[i] != null && Systems.Objects[i].LocalType == 1 && Systems.Objects[i].Spawned(Karakter.Information.UniqueID))
                {
                    Systems.Objects[i].FollowHim(this);
                }
            }
        }
        void ObjectDeSpawn()
        {
            try
            {
                if (!this.Karakter.deSpawning)
                {
                    this.Karakter.deSpawning = true;
                    Public.DeGroupSpawn gS = new Public.DeGroupSpawn();
                    gS.StartData();
                    for (int i = 0; i < Systems.HelperObject.Count; i++)
                    {
                        if (Systems.HelperObject[i] != null && Systems.HelperObject[i].Spawned(this.Karakter.Information.UniqueID))
                        {
                            Systems.HelperObject[i].Spawn.Remove(this.Karakter.Information.UniqueID);
                            gS.AddObject(Systems.HelperObject[i].UniqueID);
                            //client.Send(Public.Packet.ObjectDeSpawn(Systems.HelperObject[i].UniqueID));
                        }
                    }


                    for (int i = 0; i < Systems.Objects.Count; i++)
                    {
                        if (Systems.Objects[i] != null && Systems.Objects[i].Spawned(this.Karakter.Information.UniqueID))
                        {
                            Systems.Objects[i].Spawn.Remove(Karakter.Information.UniqueID);
                            gS.AddObject(Systems.Objects[i].UniqueID);
                            //client.Send(Public.Packet.ObjectDeSpawn(Systems.Objects[i].UniqueID));
                        }
                    }

                    for (int i = 0; i < Systems.WorldItem.Count; i++)
                    {
                        if (Systems.WorldItem[i] != null && Systems.WorldItem[i].Spawned(Karakter.Information.UniqueID) && Systems.WorldItem[i].UniqueID != 0)
                        {
                            if (Systems.WorldItem[i].Spawned(Karakter.Information.UniqueID) && Systems.WorldItem[i].Model != 0 && Systems.WorldItem[i].UniqueID != 0)
                            {
                                Systems.WorldItem[i].Spawn.Remove(Karakter.Information.UniqueID);
                                gS.AddObject(Systems.WorldItem[i].UniqueID);
                                //client.Send(Public.Packet.ObjectDeSpawn(Systems.WorldItem[i].UniqueID));
                            }
                        }
                    }

                    for (int i = 0; i < Systems.clients.Count; i++)
                    {
                        if (Systems.clients[i] != this && Systems.clients[i] != null)
                        {
                            if (Karakter.Spawned(Systems.clients[i].Karakter.Information.UniqueID))
                            {
                                Karakter.Spawn.Remove(Systems.clients[i].Karakter.Information.UniqueID);
                                gS.AddObject(Systems.clients[i].Karakter.Information.UniqueID);
                                //client.Send(Public.Packet.ObjectDeSpawn(Systems.clients[i].Karakter.Information.UniqueID));
                            }
                        }
                    }

                    client.Send(gS.StartDeGroup());
                    client.Send(gS.EndData());
                    client.Send(gS.EndGroup());

                    this.Karakter.Spawn.Clear();
                    this.Karakter.deSpawning = false;
                }

            }
            catch (Exception ex)
            {
                this.Karakter.deSpawning = false;
                Console.WriteLine("ObjectDeSpawn()::error..");
                deBug.Write(ex);
            }
        }
        void DeSpawnMe()
        {
            if (this.Karakter.Network.Exchange.Window) this.Exchange_Close();

            for (int b = 0; b < Systems.clients.Count; b++)
            {
                if (Systems.clients[b] != null && Systems.clients[b].Karakter.Spawned(this.Karakter.Information.UniqueID) && Systems.clients[b] != this)
                {
                    Systems.clients[b].Karakter.Spawn.Remove(this.Karakter.Information.UniqueID);
                    Systems.clients[b].client.Send(Public.Packet.ObjectDeSpawn(this.Karakter.Information.UniqueID));
                }
            }
            this.Karakter.Spawn.Clear();

            for (int i = 0; i < Systems.HelperObject.Count; i++)
            {
                if (Systems.HelperObject[i] != null && Systems.HelperObject[i].Spawned(this.Karakter.Information.UniqueID))
                {
                    Systems.HelperObject[i].Spawn.Remove(this.Karakter.Information.UniqueID);
                }
            }


            for (int i = 0; i < Systems.Objects.Count; i++)
            {
                if (Systems.Objects[i] != null && Systems.Objects[i].Spawned(this.Karakter.Information.UniqueID))
                {
                    Systems.Objects[i].Spawn.Remove(Karakter.Information.UniqueID);
                }
            }

            for (int i = 0; i < Systems.WorldItem.Count; i++)
            {
                if (Systems.WorldItem[i] != null && Systems.WorldItem[i].Spawned(Karakter.Information.UniqueID) && Systems.WorldItem[i].UniqueID != 0)
                {
                    if (Systems.WorldItem[i].Spawned(Karakter.Information.UniqueID))
                    {
                        Systems.WorldItem[i].Spawn.Remove(Karakter.Information.UniqueID);
                    }
                }
            }

            for (int i = 0; i < Systems.clients.Count; i++)
            {
                if (Systems.clients[i] != this && Systems.clients[i] != null)
                {
                    if (Karakter.Spawned(Systems.clients[i].Karakter.Information.UniqueID))
                    {
                        Karakter.Spawn.Remove(Systems.clients[i].Karakter.Information.UniqueID);
                    }
                }
            }
            
            if (Karakter.Transport.Right)
            {
                Karakter.Transport.Spawned = false;
                Karakter.Transport.Horse.Information = false;
                Karakter.Transport.Horse.DeSpawnMe(true);
            }
        }
    }
}
