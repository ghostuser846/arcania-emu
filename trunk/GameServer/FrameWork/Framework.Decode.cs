using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net.Sockets;

namespace Framework
{
    public class Decode
    {
        private ushort OPCODE;

        private byte[] BUFFER;
        private Socket socket;
        private object NET;
        private object packet;
        public ushort opcode
        {
            get { return OPCODE; }
        }
        public byte[] buffer
        {
            get { return BUFFER; }
        }
        public Socket Client
        {
            get { return socket; }
        }
        public object Networking
        {
            get { return NET; }
        }
        public object Packet
        {
            get { return packet; }
        }
        MemoryStream ms;
        BinaryReader br;

        public Decode(Socket wSock, byte[] buffer,Client net, object packetf)
        {
            try
            {
                UInt16 datasize;
                UInt16 security;
                packet = packetf;
                if (buffer.Length != 0)
                {
                    ms = new MemoryStream(buffer);
                    br = new BinaryReader(ms);

                    datasize = br.ReadUInt16();

                    byte[] b = new byte[datasize];
                    Array.Copy(buffer, 6, b, 0, datasize);

                    BUFFER = b;
                    OPCODE = br.ReadUInt16();

                    security = br.ReadUInt16();
                }
                socket = wSock;
                NET = net;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        public static string StringToPack(byte[] buff)
        {
            string s = null;
            foreach (byte b in buff) s += b.ToString("X2");
            return s;
        }
    }
}
