using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework;

namespace Game.Private
{
    /// <summary>
    /// Non-player control object
    /// </summary>
    class NPC
    {
        public static void Chat(int model, PacketWriter Writer)
        {

            string[] name = Data.ObjectBase[model].Name.Split('_');
            switch (name[2])
            {
                case "SMITH":
                    //01 18000000 00 04 01 02 04 20 00 1400
                    Writer.Byte(0);
                    Writer.Byte(2);
                    Writer.Byte(1);
                    Writer.Byte(0x20);
                    Writer.Byte(0);
                    if (Vergi(Data.ObjectBase[model].Name)) Writer.Word(0);
                    break;
               /* case "ARMOR":
                    break;*/
                case "POTION":
                    //01 51000000 00 02010200 1400
                    Writer.Byte(0);
                    Writer.Byte(2); //end conversion
                    Writer.Byte(0);
                    Writer.Byte(1); //ozellik
                    Writer.Byte(0);
                    if (Vergi(Data.ObjectBase[model].Name)) Writer.Word(0);
                    break;
                case "ACCESSORY":
                    Writer.Byte(0);
                    Writer.Byte(2);
                    Writer.Byte(1);
                    Writer.Byte(0);
                    Writer.Byte(0);
                    if (Vergi(Data.ObjectBase[model].Name)) Writer.Word(0);
                    break;
                case "WAREHOUSE":
                    //chat strogae 01 FB060000 00 03 01 02 0300
                    Writer.Byte(0);
                    Writer.Byte(3);
                    Writer.Byte(1);
                    Writer.Byte(1);
                    Writer.Byte(3);
                    Writer.Byte(0);
                    break;
                case "GATE":
                case "GATE1":
                case "GATE2":
                    //01 0A000000 02 07 08 1400
                    Writer.Byte(2);
                    Writer.Byte(7);
                    Writer.Byte(8);
                    if (Vergi(Data.ObjectBase[model].Name)) Writer.Word(0);
                    break;
                case "FERRY":
                case "FERRY2":
                case "FERRY3":
                case "FLYSHIP":
                case "FLYSHIP1":
                case "FLYSHIP2":
                    //01 F5080000 00 02 02 08 00
                    //01 0D020000 00 01 08 00
                    //01 B4010000 00 01 08 00 1400
                    Writer.Byte(0);
                    Writer.Byte(1);
                    Writer.Byte(8);
                    Writer.Byte(0);
                    if (Vergi(Data.ObjectBase[model].Name)) Writer.Word(0);
                    break;
                default:
                    Writer.Byte(0);
                    Writer.Byte(2);
                    Writer.Byte(1);
                    Writer.Byte(0);
                    Writer.Byte(0);
                    if (Vergi(Data.ObjectBase[model].Name)) Writer.Word(0);
                    break;
                
            }

        }
        protected static bool Vergi(string name)
        {
            switch (name)
            {
                case "NPC_CH_SMITH":
                case "NPC_CH_ARMOR":
                case "NPC_CH_POTION":
                case "NPC_CH_ACCESSORY":
                case "NPC_KT_SMITH":
                case "NPC_KT_ARMOR":
                case "NPC_KT_POTION":
                case "NPC_KT_ACCESSORY":
                case "STORE_CH_GATE":
                case "STORE_KT_GATE":
                case "NPC_CH_FERRY":
                case "NPC_CH_FERRY2":
                case "NPC_KT_FERRY":
                case "NPC_KT_FERRY2":
                case "NPC_KT_FERRY3":

                    return true;
            }
            return false;
        }
    }
}
