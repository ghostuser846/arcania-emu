using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Global
{
    public static class RandomID
    {
        public static List<int> List = new List<int>();

        public static Random rnd = new Random();

        public static bool Already(int id)
        {
            bool result = List.Exists(
                    delegate(int bk)
                    {
                        return bk == id;
                    }
                    );
            return result;
        }
        public static int Adds()
        {
            Guid uniqueID = Guid.NewGuid();
            while (true)
            {
                uniqueID = Guid.NewGuid();
                byte[] bArr = uniqueID.ToByteArray();
                int id = BitConverter.ToInt32(bArr, 0);

                if (!Already(id) && id != 0)
                {
                    List.Add(id);
                    return id;
                }
            }
        }
        public static void Deletes(int deger)
        {
            if(Already(deger))
                List.Remove(deger);
        }
        
        public static int GetRandom(int aralik1, int aralik2)
        {
            return rnd.Next(aralik1, aralik2);
        }
        public static int GetRandom(int maxValue)
        {
            return rnd.Next(maxValue);
        }
    }
    public class ID
    {
        static List<int> List = new List<int>();
        static int auto_id = 100;
        int p_id, min, max, capacity, casting;

        public ID(IDS type)
        {
            //lock (auto_id)
            {
                auto_id++;
                p_id = auto_id;
                Calculate(type);
            }
        }
        public ID(int id)
        {
            p_id = id;
            Calculate(IDS.Player);
        }
        public ID(int id, IDS type)
        {
            p_id = id;
            Calculate(type);
        }
        void Calculate(IDS type)
        {
            if (type == IDS.Object) // monster
            {
                capacity = 10;
                casting = 8;
                max = p_id * capacity + 2000000;
                min = max - (capacity - 1);
                //Console.WriteLine("{0} {1}~{2}", type, min, max);
            }
            else if (type == IDS.Player)
            {
                capacity = 50;
                casting = 10;
                max = p_id * capacity;
                min = max - (capacity - 1);
                /*Console.WriteLine("{0} {1}~{2}", type, min, max);*/
            }
            else if (type == IDS.World)
            {
                max = p_id + 4000000; 
                min = max - 1;
                /*Console.WriteLine("{0} {1}~{2}", type, min, max);*/
            }
            List.Add(min + 1);
        }
        public int GetUniqueID
        {
            get { return min + 1; }
        }
        public int GetCastingID()
        {
            return GetID(1, casting); // 1~8 ids
        }
        public int GetLoginID
        {
            get { return max - 1; }
        }
        public int GetBuffID()
        {
            return GetID(casting + 1, 21);
        }
        int GetID(int valuecap, int cap)
        {
            lock (this)
            {
                int i = 1;
                while (true)
                {
                    int value = min + valuecap + i;
                    if (!Already(value) && value <= min + cap)
                    {
                        List.Add(value);
                        return value;
                    }
                    else
                    {
                        if (value <= max)
                            i++;
                        else
                        {
                            for (int f = 1; f <= cap; f++)
                                if (Already(min + f)) List.Remove(min + f);
                            i = 1;
                        }
                    }
                }
            }
        }
        public static bool Already(int id)
        {
            bool result = List.Exists(
                    delegate(int bk)
                    {
                        return bk == id;
                    }
                    );
            return result;
        }
        public static void Delete(int id)
        {
            if (Already(id)) List.Remove(id);
        }
        public static int ObjectCount
        {
            get { return List.Count; }
        }
        /// <summary>
        /// Objektum típusainak felsorolása.
        /// </summary>
        public enum IDS { Object , Player , World};
    }
}
