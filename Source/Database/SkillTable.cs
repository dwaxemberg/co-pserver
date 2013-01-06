using System;
using System.IO;
using System.Linq;

namespace Conquer_Online_Server.Database
{
    public class SkillTable
    {
        public static void LoadProficiencies(Client.GameState client)
        {
            if (client.Entity == null)
                return;
            client.Proficiencies = new System.SafeDictionary<ushort, Interfaces.ISkill>(100);
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("skills").Where("Type", "Proficiency").And("EntityID", client.Entity.UID);
            MySqlReader r = new MySqlReader(cmd );
            while (r.Read())
            {
                Interfaces.ISkill proficiency = new Network.GamePackets.Proficiency(true);
                proficiency.ID = r.ReadUInt16("ID");
                proficiency.Level = r.ReadByte("Level");
                proficiency.PreviousLevel = r.ReadByte("PreviousLevel");
                proficiency.Experience = r.ReadUInt32("Experience");
                proficiency.Available = true;
                if (!client.Proficiencies.ContainsKey(proficiency.ID))
                    client.Proficiencies.Add(proficiency.ID, proficiency);
            }
            r.Close();
        }
        public static void SaveProficiencies(Client.GameState client)
        {
            if (client.Entity == null)
                return;
	        if (client.Proficiencies == null)
                return;
            if (client.Proficiencies.Count == 0)
                return;
            foreach (Interfaces.ISkill proficiency in client.Proficiencies.Values)
            {
                if(proficiency.Available)
                {
                    MySqlCommand cmd = new MySqlCommand(MySqlCommandType.UPDATE);
                    cmd.Update("skills").Set("Level", proficiency.Level).Set("PreviousLevel", proficiency.PreviousLevel)
                        .Set("Experience", proficiency.Experience).Where("EntityID", client.Entity.UID).And("ID", proficiency.ID).Execute();
                }
                else
                {
                    proficiency.Available = true;
                    MySqlCommand cmd = new MySqlCommand(MySqlCommandType.INSERT);
                    cmd.Insert("skills").Insert("Level", proficiency.Level).Insert("Experience", proficiency.Experience).Insert("EntityID", client.Entity.UID)
                        .Insert("Type", "Proficiency").Insert("ID", proficiency.ID).Execute();
                }
            }
        }
        public static void LoadSpells(Client.GameState client)
        {
            if (client.Entity == null)
                return;
            client.Spells = new System.SafeDictionary<ushort, Interfaces.ISkill>(100);
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("skills").Where("Type", "Spell").And("EntityID", client.Entity.UID);
            MySqlReader r = new MySqlReader(cmd);
            while (r.Read())
            {
                Interfaces.ISkill spell = new Network.GamePackets.Spell(true);
                spell.ID = r.ReadUInt16("ID");
                spell.Level = r.ReadByte("Level");
                spell.PreviousLevel = r.ReadByte("PreviousLevel");
                spell.Experience = r.ReadUInt32("Experience");
                spell.Available = true;
                if (!client.Spells.ContainsKey(spell.ID))
                    client.Spells.Add(spell.ID, spell);
            }
            r.Close();
        }
        public static void SaveSpells(Client.GameState client)
        {
            if (client.Entity == null)
                return;
            if (client.Spells == null)
                return;
            if (client.Spells.Count == 0)
                return;
            foreach (Interfaces.ISkill spell in client.Spells.Values)
            {
                if(spell.Available)
                {
                    MySqlCommand cmd = new MySqlCommand(MySqlCommandType.UPDATE);
                    cmd.Update("skills").Set("Level", spell.Level).Set("PreviousLevel", spell.PreviousLevel)
                        .Set("Experience", spell.Experience).Where("EntityID", client.Entity.UID).And("ID", spell.ID).Execute();
                }
                else
                {
                    spell.Available = true;
                    MySqlCommand cmd = new MySqlCommand(MySqlCommandType.INSERT);
                    cmd.Insert("skills").Insert("Level", spell.Level).Insert("Experience", spell.Experience).Insert("EntityID", client.Entity.UID)
                        .Insert("Type", "Spell").Insert("ID", spell.ID).Execute();
                }
            }
        }
        public static void DeleteSpell(Client.GameState client, ushort ID)
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.DELETE);
            cmd.Delete("skills", "ID", ID).And("EntityID", client.Entity.UID).Execute();
        }
    }
}
