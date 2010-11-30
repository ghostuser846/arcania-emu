using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Global
{
    /// <summary>
    /// Tárgyak adatbázisa, jellemzői.
    /// </summary>
    public sealed class item_database
    {
        public static int GetItem(string name)
        {
            for (int i = 0; i < Data.ItemBase.Length; i++)
            {
                if (Data.ItemBase[i] != null && Data.ItemBase[i].Name == name) return i;
            }
            return 0;
        }

        public int ID;
        public byte Level, SOX, Gender, Race, SoulBound;
        public int Class_B, Class_C, Class_A, Shop_price, Max_Stack, Use_Time, Use_Time2, Sell_Price, Class_D, Storage_price;
        public short ATTACK_DISTANCE;
        public string Name, ObjectName, SkillName;
        public bool needEquip;
        
        attack_items attack = new attack_items();
        public attack_items Attack { get { return attack; } }

        def_items def = new def_items();
        public def_items Defans { get { return def; } }
    }
    public sealed class attack_items
    {
        public double Min_LPhyAttack, Min_HPhyAttack, PhyAttackInc, Min_LMagAttack, Min_HMagAttack, MagAttackINC, MinAttackRating, MaxAttackRating;
        public byte MinCrit, MaxCrit;
    }
    public sealed class def_items
    {
        public double MinMagDef, MagDefINC, MinPhyDef, PhyDefINC;
        public double PhyAbsorb, MagAbsorb, AbsorbINC, Durability, Parry;
        public byte MinBlock, MaxBlock;
    }
    public sealed class objectdata
    {
        public static int GetItem(string name)
        {
            for (int i = 0; i < Data.ObjectBase.Length; i++)
            {
                if (Data.ObjectBase[i] != null && Data.ObjectBase[i].Name == name) return i;
            }
            return 0;
        }
        public int ID;
        public string Name;
        public int HP, Exp;
        public int[] Skill = new int[9];
        public byte amountSkill;
        public int MagDef, PhyDef, ParryRatio, HitRatio;
        public byte Agresif, Type, ObjectType, Level;
        public string[] Shop = new string[10];
        public string[] Tab = new string[30];
        public string StoreName;
    }
    public sealed class vektor
    {
        public float x, y, z;
        public byte xSec, ySec;
        public int ID;

        public vektor(int ID, float x, float z, float y, byte xSec, byte ySec)
        {
            this.x = x;
            this.z = z;
            this.y = y;
            this.xSec = xSec;
            this.ySec = ySec;
            this.ID = ID;
        }
    }
    public sealed class slotItem
    {
        public int ID, dbID, Durability;
        public byte PlusValue, Type, Slot;
        public short Amount = 1;
    }

    public sealed class point
    {
        public double x, z, y;
        public byte xSec, ySec, test;
        public int ID, Number;
        public string Name;
    }
    public class s_data
    {
        public int Properties1, Properties2, Properties3, Properties4, Properties5, Properties6;
        public int ID, SkillPoint, NextSkill, Mana, Per, CastingTime, Time;
        public short Mastery;
        public byte sType;
        public string Name, Series;
        public byte Weapon1, Weapon2;
    }
    public class levelgold
    {
        public short min, max;
    }
    public class shop_data
    {
        public string tab;
        public string[] Item = new string[300];
        public static shop_data GetShopIndex(string name)
        {
            shop_data result = Data.ShopData.Find(
                    delegate(shop_data bk)
                    {
                        return bk.tab == name;
                    }
                    );
            return result;
            //if (result == id) return true;
            /*for (int i = 0; i < Data.ShopData.Count; i++)
            {
                if (Data.ShopData[i].tab == name) return Data.ShopData[i];
            }
            return null;*/
        }
    }
}
