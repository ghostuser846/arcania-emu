using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework;
using System.IO;
namespace Game
{
    public partial class Systems
    {
        internal Client client;
        internal Decode PacketInformation;
        internal player Player;
        /// <summary>
        /// A játékos karakterének mutatója.
        /// </summary>
        internal character Karakter;
        public DateTime lastPing;
        public Random rnd = new Random();
        public static Random grnd = new Random();
        public static DateTime ServerStartedTime;
        /// <summary>
        /// A kapcsolódott kliensek vektora(tömbje).
        /// </summary>
        public static aList<Systems> clients = new aList<Systems>();
        public static List<obj> Objects = new List<obj>(13000);
        public static List<world_item> WorldItem = new List<world_item>();
        public static List<party> Party = new List<party>();
        public static List<pet_obj> HelperObject = new List<pet_obj>();

        /// <summary>
        /// Systems Konstruktora.
        /// </summary>
        /// <param name="s">Egy klienst vár paraméternek.</param>
        public Systems(Client s)
        {
            client = s;
        }
        public static void oPCode(Decode de)
        {
            try
            {
                Systems sys = (Systems)de.Packet;
                sys.PacketInformation = de;

                switch (de.opcode)
                {

                    case CLIENT_PING:
                    case CLIENT_PING2:
                        sys.Ping();
                        break;
                    case CLIENT_PATCH:
                        sys.Patch();
                        break;
                    case CLIENT_CONNECTION:
                        sys.Connect();
                        break;
                    case CLIENT_CHARACTERSCREEN:
                        sys.CharacterScreen();
                        break;
                    case CLIENT_INGAME_REQUEST:
                        sys.LoginScreen();
                        break;
                    case 0x7481:
                        //Print.Format("(0x{0}) {1}", de.opcode.ToString("X4"), Decode.StringToPack(sys.PacketInformation.buffer));
                        sys.Ping();
                        sys.InGameSuccess2();
                        break;
                    case CLIENT_INGAME_SUCCESS:
                        //Print.Format("(0x{0}) {1}", de.opcode.ToString("X4"), Decode.StringToPack(sys.PacketInformation.buffer));
                        sys.Ping();
                        sys.InGameSuccess();
                        break;
                    case CLIENT_MOVEMENT:
                        sys.Movement();
                        break;
                    case CLIENT_LEAVE_REQUEST:
                        sys.LeaveGame();
                        break;
                    case CLIENT_LEAVE_CANCEL:
                        sys.CancelLeaveGame();
                        break;
                    case CLIENT_ITEM_MOVE:
                        sys.ItemMain();
                        break;
                    case CLIENT_SELECT_OBJECT:
                        sys.SelectObject();
                        break;
                    case CLIENT_GM:
                        sys.GM();
                        break;
                    case CLIENT_EMOTE:
                        sys.Emote();
                        break;
                    case CLIENT_TELEPORTSTART:
                        sys.Teleport_Start();
                        break;
                    case CLIENT_TELEPORTDATA:
                        sys.Teleport_Data();
                        break;
                    case CLIENT_CHAT:
                        sys.Chat();
                        break;
                    case CLIENT_MAINACTION:
                        sys.ActionMain();
                        break;
                    case CLIENT_MASTERY_UP:
                        sys.Mastery_Up();
                        break;
                    case CLIENT_SKILL_UP:
                        sys.Mastery_Skill_Up();
                        break;
                    case CLIENT_SAVE_INFO:
                        sys.Save();
                        break;
                    case CLIENT_GETUP:
                        sys.Player_Up();
                        break;
                    case CLIENT_REQUEST_PARTY:
                        sys.PartyMain();
                        break;
                    case CLIENT_PARTY_REQUEST:
                        //sys.Request();
                        sys.PartyRequest();
                        break;
                    case CLIENT_EXCHANGE_REQUEST:
                        sys.Exchange_Request();
                        break;
                    case CLIENT_EXCHANGE_WINDOWS_CLOSE:
                        sys.Exchange_Close();
                        break;
                    case CLIENT_EXCHANGE_ACCEPT:
                        sys.Exchange_Accept();
                        break;
                    case CLIENT_EXCHANGE_APPROVE:
                        sys.Exchange_Approve();
                        break;
                    case CLIENT_PARTY_ADDMEMBERS:
                        sys.PartyAddmembers();
                        break;
                    case CLIENT_PARTY_LEAVE:
                        sys.PartyLeave();
                        break;
                    case CLIENT_PARTY_BANPLAYER:
                        sys.PartyBan();
                        break;
                    case CLIENT_PLAYER_UPDATE_INT:
                        sys.InsertInt();
                        break;
                    case CLIENT_PLAYER_UPDATE_STR:
                        sys.InsertStr();
                        break;
                    case CLIENT_PLAYER_HANDLE:
                        sys.Handle();
                        break;
                    case CLIENT_PLAYER_BERSERK:
                        sys.Player_Berserk_Up();
                        break;
                    case CLIENT_CLOSE_NPC:
                        sys.Close_NPC();
                        break;
                    case CLIENT_OPEN_NPC:
                        sys.Open_NPC();
                        break;
                    case CLIENT_NPC_BUYPACK:
                        sys.Player_BuyPack();
                        break;
                    case CLIENT_OPEN_WAREHOUSE:
                        sys.Open_Warehouse();
                        break;
                    case CLIENT_CLOSE_SCROLL:
                        sys.StopScrollTimer();
                        break;
                    case CLIENT_SAVE_PLACE:
                        sys.SavePlace();
                        break;
                    case CLIENT_ALCHEMY:
                        sys.AlchemyMain();
                        break;
                    case CLIENT_MOVEMENT_WITH_TRANSPORT:
                        sys.MovementTransport();
                        break;
                    case CLIENT_PET_TERMINATE:
                        sys.HandleClosePet();
                        break;
                    //Party matching:
                    case CLIENT_PARTYMATCHING_LIST_REQUEST:
                        sys.ListPartyMatching();
                        break;
                    case CLIENT_CREATE_FORMED_PARTY:
                        sys.CreateFormedParty();
                        break;
                    case CLIENT_FORMED_PARTY_DELETE:
                        sys.DeleteFormedParty();
                        break;
                    case CLIENT_STALL_OPEN:
                        sys.Stalllol();
                        break;
                    case CLIENT_STALL_WELC:
                        sys.StallWelcome();
                        break;
                    default:
                        //Print.Format("(0x{0}) {1}", de.opcode.ToString("X4"), Decode.StringToPack(sys.PacketInformation.buffer));
                        break;
                }
            }
            catch (Exception ex)
            {
                deBug.Write(ex);
            }
        }
        /// <summary>
        /// Kliens Pingelése.
        /// </summary>
        public void Ping()
        {
            PingTimer();
            lastPing = DateTime.Now;
        }
        /// <summary>
        /// Kliens kapcsolatának bontása a szerverrel.
        /// </summary>
        public void Disconnect()
        {
            try
            {
                if (Player != null)
                {
                    MsSQL.UpdateData("UPDATE users SET online='" + 0 + "' WHERE id='" + Player.AccountName + "'");
                    Player.Dispose();
                    Player = null;
                }
                if (Karakter != null)
                {
                    //Print.Format("Disconnect {0}", Karakter.Information.Name);
                    //Global.RandomID.Delete(Karakter.Information.UniqueID);
                    if (this.Karakter.Transport.Right) this.Karakter.Transport.Horse.DeSpawnMe();
                    if (this.Karakter.Network.Exchange.Window) this.Exchange_Close();
                    ClearCasts();

                    if (Karakter.InGame)
                    {
                        StopAttackTimer();
                        BuffAllClose();
                        DeSpawnMe();

                        SavePlayerPosition();
                        SavePlayerInfo();
                        this.client.Close();
                        this.Karakter.Dispose();
                        this.Dispose();
                        Karakter.InGame = false;
                    }
                }

                PingStop();
                Systems.clients.Remove(this);
                client.Disconnect(PacketInformation.Client);
                Print.Format("Online Players:{0} Global.RandomClass:{1}", Systems.clients.Count, Global.ID.ObjectCount);
                return;
            }
            catch(Exception ex)
            {
                Print.Format("Dissconect Error::");
                deBug.Write(ex);
            }
        }
        public void ClearCasts()
        {
            /*  MainCasting
                AttackingID
                CastingSkill  */
            /*if (this.Karakter.Action.Skill.MainCasting != 0) Global.RandomID.Delete(this.Karakter.Action.Skill.MainCasting);
            if (this.Karakter.Action.AttackingID != 0) Global.RandomID.Delete(this.Karakter.Action.AttackingID);
            if (this.Karakter.Action.CastingSkill != 0) Global.RandomID.Delete(this.Karakter.Action.CastingSkill);*/
        }
        public void PrintLastPack()
        {
            try
            {
                if (client.BuffList != null && Karakter != null && Karakter.InGame)
                foreach (byte[] i in client.BuffList)
                {
                    StreamWriter Writer = System.IO.File.AppendText(Environment.CurrentDirectory + @"\player\info\debug\" + Karakter.Information.Name + ".txt");
                    Writer.WriteLine(client.BytesToString(i));
                    Writer.Close();
                }
            }
            catch
            {
                Console.WriteLine("debugWriteBug()::{0}", Karakter.Information.Name);
            }
        }
        public void Dispose()
        {
            GC.Collect(GC.GetGeneration(this));
        }
        public Client GetClient
        {
            get
            {
                return this.client;
            }
        }
        public void Send(byte[] buff)
        {
            try
            {
                for (int i = 0; i < Systems.clients.Count; i++)
                {
                    if (Systems.clients[i] != null)
                    {
                        if (Systems.clients[i] != this)
                        {
                            if (Karakter.Spawned(Systems.clients[i].Karakter.Information.UniqueID) && Karakter.InGame)
                            {
                                if (Systems.clients[i].Karakter.Spawned(this.Karakter.Information.UniqueID) && Systems.clients[i].Karakter.InGame)
                                    Systems.clients[i].client.Send(buff);
                            }
                        }
                        else
                            client.Send(buff);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("player::send::error");
                deBug.Write(ex);
            }
        }
        public void Send(List<int> list, byte[] buff)
        {
            try
            {
                for (int i = 0; i < Systems.clients.Count; i++)
                {
                    if (Systems.clients[i] != null)
                    {
                        if (Systems.clients[i] != this)
                        {
                            if (CheckSpawned(list, Systems.clients[i].Karakter.Information.UniqueID) && Karakter.InGame)
                            {
                                if (Systems.clients[i].Karakter.Spawned(this.Karakter.Information.UniqueID) && Systems.clients[i].Karakter.InGame)
                                    Systems.clients[i].client.Send(buff);
                            }
                        }
                        else
                            client.Send(buff);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("player::sendlist::error");
                deBug.Write(ex);
            }
        }
        public static bool CheckSpawned(List<int> Spawn, int id)
        {
            bool result = Spawn.Exists(
                    delegate(int bk)
                    {
                        return bk == id;
                    }
                    );
            return result;
        }
        public static void SendAll(byte[] buff)
        {
            for (int i = 0; i < Systems.clients.Count; i++)
            {
                if(Systems.clients[i] != null && Systems.clients[i].Karakter.InGame)
                    Systems.clients[i].client.Send(buff);
            }
        }
        public static Systems GetPlayer(int id)
        {
            for (int i = 0; i < Systems.clients.Count; i++)
            {
                if (Systems.clients[i] != null && Systems.clients[i].Karakter.Information.UniqueID == id)
                    return Systems.clients[i];
            }
            return null;
        }
        public static pet_obj GetPet(int id)
        {
            for (int i = 0; i < Systems.HelperObject.Count; i++)
            {
                if (Systems.HelperObject[i] != null && Systems.HelperObject[i].UniqueID == id)
                    return Systems.HelperObject[i];
            }
            return null;
        }
        public static void AnnounceOnlines()
        {
            SendAll(Public.Packet.ChatPacket(7, 0, "Játékos szám : " + Systems.clients.Count, null));
            Print.Format("Online Játékosok:{0} Global.RandomClass:{1}", Systems.clients.Count, Global.RandomID.List.Count);
        }
        public static int GetOnlineClientCount
        {
            get
            {
                return Systems.clients.Count;
            }
        }
        public static void Announce(string msg)
        {
            SendAll(Public.Packet.ChatPacket(7, 0, msg, null));
        }

    }
    public class Rate
    {
        public static byte Gold, Item, Xp, Sp, Sox, MobSpawn;
    }
    public class deBug
    {
        public static void Write(Exception ex)
        {
            try
            {
                string hata_raporu = String.Format("[{0}] {1} -> {2}", DateTime.Now.ToString(), ex.Message, ex.StackTrace);
                StreamWriter Writer = System.IO.File.AppendText(Environment.CurrentDirectory + @"\Error List.txt");
                Writer.WriteLine(hata_raporu);
                Writer.Close();
            }
            catch
            {
                Console.WriteLine("deBug::Write({0})", DateTime.Now.ToString());
            }
        }

        public static void Open()
        {
            //Writer = new StreamWriter(Environment.CurrentDirectory + @"\Error List.txt");
        }
    }
}
