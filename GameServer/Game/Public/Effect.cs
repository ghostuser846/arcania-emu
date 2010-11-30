using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework;

namespace Game
{
    abstract class Effect
    {
        public Effect(byte sw)
        {

        }
        public static byte[] testEf(int id)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(0x33AE);
            //  E4ED6F00 00 01 05 36000000 00 40000002
            //  255B2A00 04 00 02 73000000
            //  255B2A00 01 00 01 59010000
            //  1E6C0000 00 01 05 DA020000 00 40000002

            /*freeze*/
            //  15DD6200 01 01 05 4F04000002000000


            Writer.DWord(id);
            Writer.Byte(0);
            Writer.Byte(1);
            Writer.Byte(5);
            Writer.Byte(0xDA);
            Writer.Byte(0x02);
            Writer.Word(0);
            Writer.Byte(0);
            Writer.Byte(0x40);
            Writer.Byte(0);
            Writer.Byte(0);
            Writer.Byte(0x02);
            /*Writer.Byte(1);
            Writer.Byte(5);

            Writer.Byte(0);
            Writer.Byte(0);
            Writer.Byte(0);
            Writer.Byte(0);
            Writer.Byte(0);
           /* Writer.Byte(0);
            Writer.Byte(0x40);*/
            /*Writer.Byte(64);
            Writer.Byte(0);
            Writer.Byte(0);
            Writer.Byte(0x01);*/
            return Writer.GetBytes();
        }
    }
}
