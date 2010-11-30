using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Function
{
    public class Formule
    {
        public static float packetx(float x, byte sector)
        {
            return ((x - ((sector) - 135) * 192) * 10);
        }
        public static float packety(float y, byte sector)
        {
            return ((y - ((sector) - 92) * 192) * 10);
        }
        public static float gamex(float x, byte sector)
        {
            return ((sector - 135) * 192 + (x / 10));
        }
        public static float gamey(float y, byte sector)
        {
            return ((sector - 92) * 192 + (y / 10));
        }
        public static double gamedistance(float x1, float y1, float x2 ,float y2)
        {
            return Math.Sqrt(((x1 - x2) * (x1 - x2)) + ((y1 - y2) * (y1 - y2)));// + ((z1 - z2) * (z1 - z2))
        }
        public static double gamedamage(double maxDMG, double aPower, double absrob, double def, double pBalance, double uAttack)
        {
            return ((maxDMG) * (1 + (0.01 * aPower)) / (1 + (absrob * 0.001)) - def) * (0.01 * pBalance) * (1 + (0.01 * uAttack)) * 1;
        }
        public static int gamePhp(byte level, short str)
        {
            return Convert.ToInt32(System.Math.Pow(1.02, ((level) - 1)) * (str) * 10);
        }
        public static int gamePmp(byte level, short Int)
        {
            return Convert.ToInt32(System.Math.Pow(1.02, ((level) - 1)) * (Int) * 10);
        }
    }
}
