using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework;

namespace Game
{
    public partial class Systems
    {
        public void AddMastery(short masteryid, int newCharName)
        {
            MsSQL.InsertData("INSERT INTO mastery (owner, mastery) VALUES ('" + newCharName + "','" + masteryid + "')");
        }
        void Mastery_Up()
        {
            if (!Karakter.Action.Tree)
            {
                Karakter.Action.Tree = true;
                PacketReader Reader = new PacketReader(PacketInformation.buffer);
                int masteryid = Reader.Int32();
                byte level = Reader.Byte();
                byte m_index = MasteryGet(masteryid);
                if (m_index == 0) { Karakter.Action.Tree = false; return; }
                if (Karakter.Information.SkillPoint < Data.MasteryBase[Karakter.Stat.Skill.Mastery_Level[m_index]]) { Karakter.Action.Tree = false; return; }
                else
                {
                    if (Karakter.Stat.Skill.Mastery_Level[m_index] < Karakter.Information.Level)
                    {
                        Karakter.Stat.Skill.Mastery_Level[m_index]++;
                        Karakter.Information.SkillPoint -= Data.MasteryBase[Karakter.Stat.Skill.Mastery_Level[m_index]];

                        client.Send(Private.Packet.InfoUpdate(2, Karakter.Information.SkillPoint, 0));
                        client.Send(Private.Packet.MasteryUpPacket(masteryid, Karakter.Stat.Skill.Mastery_Level[m_index]));

                        SaveMaster();
                    }
                }
                Karakter.Action.Tree = false;
            }
        }
        byte MasteryGet(int id)
        {
            for (byte b = 1; b <= 7; b++)
                if (Karakter.Stat.Skill.Mastery[b] == id) return b;
            return 0;
        }
        void SaveMaster()
        {
            for (byte b = 1; b <= 7; b++)
            {
                if (Karakter.Stat.Skill.Mastery[b] != 0) MsSQL.InsertData("update mastery set level='" + Karakter.Stat.Skill.Mastery_Level[b] + "' where owner='" + Karakter.Information.CharacterID + "' AND mastery='" + Karakter.Stat.Skill.Mastery[b] + "'");
            }
            MsSQL.InsertData("update karakterler set sp='" + Karakter.Information.SkillPoint + "' where id='" + Karakter.Information.CharacterID + "'");
        }
        void SaveSkill(int skillid)
        {
            for (int i = 1; i <= Karakter.Stat.Skill.AmountSkill; i++)
            {
                if (Data.SkillBase[Karakter.Stat.Skill.Skill[i]].Series == Data.SkillBase[skillid].Series)
                {
                    MsSQL.UpdateData("UPDATE saved_skills SET Skill" + i + "='" + skillid + "' WHERE Skill" + i + "='" + Karakter.Stat.Skill.Skill[i] + "' AND owner='" + Karakter.Information.CharacterID + "'");
                    Karakter.Stat.Skill.Skill[i] = skillid;
                    return;
                }
            }

            MsSQL.UpdateData("UPDATE saved_skills SET Skill" + (Karakter.Stat.Skill.AmountSkill + 1) + "='" + skillid + "' WHERE owner='" + Karakter.Information.CharacterID + "'");
            MsSQL.UpdateData("UPDATE saved_skills SET AmountSkill='" + (Karakter.Stat.Skill.AmountSkill + 1) + "' WHERE owner='" + Karakter.Information.CharacterID + "'");
            Karakter.Stat.Skill.AmountSkill++;
            Karakter.Stat.Skill.Skill[Karakter.Stat.Skill.AmountSkill] = skillid;
        }
        void Mastery_Skill_Up()
        {
            if (!Karakter.Action.Tree)
            {
                Karakter.Action.Tree = true;
                PacketReader Reader = new PacketReader(PacketInformation.buffer);
                int skillid = Reader.Int32();
                if (Karakter.Information.SkillPoint < Data.SkillBase[skillid].SkillPoint) { Karakter.Action.Tree = false; return; }
                else
                {
                    Karakter.Information.SkillPoint -= Data.SkillBase[skillid].SkillPoint;
                    client.Send(Private.Packet.InfoUpdate(2, Karakter.Information.SkillPoint, 0));
                    client.Send(Private.Packet.SkillUpdate(skillid));
                    SaveSkill(skillid);
                }
                Karakter.Action.Tree = false;
            }
        }
        byte MasteryGetPower(int SkillID)
        {
            return Karakter.Stat.Skill.Mastery_Level[MasteryGet(Data.SkillBase[SkillID].Mastery)];
        }
        public byte MasteryGetBigLevel
        {
            get
            {
                return Karakter.Stat.Skill.Mastery_Level.Max();
            }
        }
    }
}
