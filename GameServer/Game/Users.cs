using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using Framework;
namespace Game
{
    public class Users
    {
        Socket userSocket;
        static Users user;
        byte[] buffer = new byte[8096];
        public static void updateServerList(object List, EventArgs a)
        {
            try
            {
                aList<Systems> clients = List as aList<Systems>;
                PacketWriter Writer = new PacketWriter();
                Writer.Create(1905);
                Writer.Word((short)clients.Count);
                user.Send(Writer.GetBytes());
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
            
        }
        public static void StartUserTablo(int port)
        {
            try
            {
                user = new Users();
                user.Bind(port);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        public void Bind(int port)
        {
            IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
            IPEndPoint EndPoint = new IPEndPoint(IPAddress.Any, port);
            userSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            userSocket.Connect(ipAddress, port);
            userSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ClientReceive), userSocket);
        }
        public void ClientReceive(IAsyncResult ar)
        {
            try
            {
                if(userSocket.Connected) userSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ClientReceive), userSocket);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        public void Send(byte[] buff)
        {
            if (userSocket.Connected) userSocket.Send(buff);
        }
    }
}
