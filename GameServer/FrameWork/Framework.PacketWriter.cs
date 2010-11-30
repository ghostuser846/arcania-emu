using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Framework
{

    public class PacketWriter
    {

        MemoryStream ms = new MemoryStream();
        BinaryWriter bw;

        public PacketWriter()
        {

        }
        public void AddBuffer(byte[] buffer)
        {
            bw.Write(buffer);
        }
        public PacketWriter(bool b)
        {
            if (b)
            {
                bw = null;
                ms = null;
                ms = new MemoryStream();

                bw = new BinaryWriter(ms);
                bw.Write((ushort)0);
            }
        }
        public int Length()
        {
            return (int)(ms.Position - 6);
        }
        public void Byte(byte data)
        {
            bw.Write(data);
        }
        public void Byte(object data)
        {
            bw.Write(Convert.ToByte(data));
        }
        public void Create(ushort opcode)
        {
            bw = null;
            ms = null;
            ms = new MemoryStream();
            bw = new BinaryWriter(ms);

            bw.Write((ushort)0);

            Word(opcode);
            Word(0);
        }
        public void Word(ushort data)
        {
            bw.Write(data);
        }
        public void Word(object data)
        {
            if(Convert.ToInt32(data) < 0)
                bw.Write(Convert.ToInt16(data));
            else
                bw.Write(Convert.ToUInt16(data));
        }
        public void Word(short data)
        {
            bw.Write(data);
        }
        public void DWord(uint data)
        {
            bw.Write(data);
        }
        public void DWord(object data)
        {
            if (Convert.ToInt64(data) > 0)
                bw.Write(Convert.ToUInt32(data));
            else bw.Write(Convert.ToInt32(data));
        }
        public void DWord(int data)
        {
            bw.Write(data);
        }
        public void LWord(ulong data)
        {
            bw.Write(data);
        }
        public void LWord(object data)
        {
            bw.Write(Convert.ToInt64(data));
        }
        public void LWord(long data)
        {
            bw.Write(data);
        }
        public void Float(float data)
        {
            bw.Write(data);
        }
        public void Float(object data)
        {
            bw.Write(Convert.ToSingle(data));
        }
        public void FloatFour(float data)
        {
            bw.Write(data);
        }
        public void Text(string data)
        {
            Word((short)data.Length);
            String(data);
        }
        public void Text(object data)
        {
            Word((short)Convert.ToString(data).Length);
            String((string)data);
        }
        public void Bool(bool b)
        {
            bw.Write(b);
        }
        public void Bool(object b)
        {
            bw.Write((bool)b);
        }
        public void Buffer(byte[] b)
        {
            bw.Write(b, 0, b.Length);
        }
        public void String(string data)
        {
            char[] chars = new char[data.Length]; // girilen stringin uzunlugunda bir char array tipi açıyoruz

            for (int x = 0; x < data.Length; x++) // ardından bu char arrayı döngüye sokuyoruz
            {
                chars[x] = Convert.ToChar(data.Substring(x, 1));//substring ile stringden tek tek harfleri alıyoruz
                bw.Write(chars[x]);//aldıgımız harfleri char tipi olarak yazdırıyoruz
            }
        }
        public void StringTest(string data)
        {
            char[] chars = new char[data.Length];

            for (int x = 0; x < data.Length; x++)
            {
                chars[x] = Convert.ToChar(data.Substring(x, 1));
                bw.Write(chars[x]);
            }
        }

        public void UString(string data)
        {
            char[] chars = new char[data.Length];

            for (int x = 0; x < data.Length; x++)
            {
                chars[x] = Convert.ToChar(data.Substring(x, 1));
                bw.Write(chars[x]);
                bw.Write((byte)0);
            }
        }

        public void HexString(string data)
        {
            char[] chars = new char[data.Length];

            for (int x = 0; x < data.Length; x++)
            {
                chars[x] = Convert.ToChar(data.Substring(x, 1));
                bw.Write(chars[x]);
            }
        }

        public byte[] GetBytes()
        {
            byte[] data = { 0 }; // girilen tüm packetleri buna depoluycaz
            ushort len = (ushort)(ms.Position - 6); // buda packetin uzunlugunu hesaplıyoruz
            ms.Position = 0; // burda girilen packetin başına uzunlugu yazdırmamız için hafızanın durumunu 0 lıyoruz

            bw.Write(len); //packetin başına packet uzunlugunu yazdırıyoruz
            bw.Flush(); 
            bw.Close();// yazıcıyı kapatıyoruz
            data = ms.ToArray(); // tum packeti data ya aktarıyoruz
            ms.Close(); // hafıza classını kapatıyoruz
            return data; //byte arrayı döndürüyoruz
        }
    }
}
