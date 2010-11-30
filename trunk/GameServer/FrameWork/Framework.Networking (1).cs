using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace Framework
{
    public class Server
    {
        public delegate void dReceive(Decode de);
        public delegate void dConnect(ref object de, Client net);
        public delegate void dError(Exception ex);
        public delegate void dDisconnect(object o);

        public event dConnect OnConnect;
        public event dError OnError;

        Socket serverSocket;

        public void Start(string ip, int PORT)
        {
            IPAddress.Parse(ip);
            IPEndPoint EndPoint = new IPEndPoint(IPAddress.Any, PORT);
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                serverSocket.Bind(EndPoint);
                serverSocket.Listen(5);
                serverSocket.BeginAccept(new AsyncCallback(ClientConnect), null);
                MsSQL.UpdateData("UPDATE users SET online='0'");
                MsSQL.UpdateData("UPDATE server SET users_current='0'");
            }
            catch (Exception ex)
            {
                OnError(ex);
            }
            finally { }
        }
        private void ClientConnect(IAsyncResult ar)
        {
            try
            {
                Socket wSocket = serverSocket.EndAccept(ar);
                wSocket.Send(new byte[] { 0x01, 0x00, 0x00, 0x50, 0x00, 0x00, 0x01 });

                object p = null;
                Client Player = new Client();
                OnConnect(ref p, Player);

                Player.Packets = p;

                wSocket.BeginReceive(Player.buffer, 0, Player.buffer.Length, SocketFlags.None, new AsyncCallback(Player.ReceiveData), wSocket);
                serverSocket.BeginAccept(new AsyncCallback(ClientConnect), null);
            }
            catch (Exception ex)
            {
               
                OnError(ex);
            }
        }
    }
    public class Client
    {
        public delegate void dReceive(Decode de);
        public delegate void dDisconnect(object o);

        public static event dReceive OnReceiveData;
        public static event dDisconnect OnDisconnect;
        Socket clientSocket;

        public object Packets { get; set; }
        public byte[] buffer = new byte[8096];
        public bList<byte[]> BuffList = new bList<byte[]>();
        public void ReceiveData(IAsyncResult ar)
        {
            Socket wSocket = (Socket)ar.AsyncState;
            clientSocket = wSocket;
            int recvSize = 0;
            Decode de = new Decode(wSocket, buffer, this, Packets);
            try
            {
                if (wSocket.Connected)
                {
                    recvSize = wSocket.EndReceive(ar);
                    if (recvSize > 0)
                    {

                        OnReceiveData(de);
                        Array.Clear(buffer, 0, buffer.Length);
                    }
                    wSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveData), wSocket);
                }
            }
            catch (SocketException se)
            {
                if (se.ErrorCode == 10054) // Disconnected Error Code
                {
                    OnDisconnect(de.Packet);

                }
            }
        }
        public string BytesToString(byte[] buff)
        {
            string pack = null;
            System.IO.MemoryStream ms = new System.IO.MemoryStream(buff);
            System.IO.BinaryReader br = new System.IO.BinaryReader(ms);
            ushort datasize = br.ReadUInt16();
            ushort opcode = br.ReadUInt16();
            br.ReadUInt16();
            pack = String.Format("{0}->{1}",opcode.ToString("X2"), Decode.StringToPack(br.ReadBytes(datasize)));
            return pack;
        }
        public void Send(byte[] buff)
        {
            try
            {
                if (clientSocket.Connected && buff != null)
                {
                    if (clientSocket.Connected)
                    {
                        this.BuffList.Add(buff);
                    }
                    if (buff.Length > 0 && clientSocket.Connected) clientSocket.Send(buff);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                System.IO.StreamWriter Writer = System.IO.File.AppendText(Environment.CurrentDirectory + @"\Error List.txt");
                Writer.WriteLine(ex);
                Writer.Close();
                Console.WriteLine(ex.StackTrace);
            }
        }

        public void Disconnect(Socket s)
        {
            if (s.Connected)
            {
                s.Shutdown(SocketShutdown.Both);
                clientSocket.Disconnect(true);
            }
        }
        public void Close()
        {
            clientSocket.Close();
            Array.Clear(buffer, 0, buffer.Length);
        }
    }
}
