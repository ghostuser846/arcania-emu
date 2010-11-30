using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game
{
    public class Drop
    {
        public static int[] SetEtc(byte Level)
        {
            int[] getID = new int[8];

            if (Level >= 70 && Level <= 120)
            {
                getID[0] = 8;
                getID[1] = 15;
                getID[2] = 22;
                getID[3] = 58;
                getID[4] = 9;
                getID[5] = 23;
                getID[6] = 61;
                getID[7] = 62;
            }
            else if (Level >= 60 && Level <= 70)
            {
                getID[0] = 7;
                getID[1] = 14;
                getID[2] = 21;
                getID[3] = 58;
                getID[4] = 9;
                getID[5] = 23;
                getID[6] = 61;
                getID[7] = 62;
            }
            else if (Level >= 40 && Level <= 60)
            {
                getID[0] = 6;
                getID[1] = 13;
                getID[2] = 20;
                getID[3] = 57;
                getID[4] = 9;
                getID[5] = 23;
                getID[6] = 61;
                getID[7] = 62;
            }
            else if (Level >= 20 && Level <= 40)
            {
                getID[0] = 5;
                getID[1] = 12;
                getID[2] = 19;
                getID[3] = 56;
                getID[4] = 9;
                getID[5] = 23;
                getID[6] = 61;
                getID[7] = 62;

            }
            else if (Level >= 1 && Level <= 20)
            {
                getID[0] = 4;
                getID[1] = 11;
                getID[2] = 18;
                getID[3] = 55;
                getID[4] = 9;
                getID[5] = 23;
                getID[6] = 61;
                getID[7] = 62;
            }

            return getID;
        }
    }
}
