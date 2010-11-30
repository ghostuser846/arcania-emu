using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Global
{
    public class BuyPack
    {
        List<slotItem> item = new List<slotItem>(5);
        public void Add(slotItem objects)
        {
            if (item.Count < 5)
            {
                item.Add(objects);
            }
            else
            {
                if (item.Count >= 5)
                {
                    item[0] = item[1];
                    item[1] = item[2];
                    item[2] = item[3];
                    item[3] = item[4];
                    item[4] = objects;
                }
            }
        }
        public slotItem Get(byte index)
        {
            slotItem ret = item[index];
            item.Remove(ret);
            return ret;
        }
    }
}
