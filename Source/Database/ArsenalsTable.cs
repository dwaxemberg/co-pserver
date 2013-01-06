using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Conquer_Online_Server.Game.ConquerStructures.Society;
namespace Conquer_Online_Server.Database
{
    public class ArsenalsTable
    {
        public static void Load(Game.ConquerStructures.Society.Guild g)
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT);
            cmd.Select("guild_arsenalsdonation").Where("guild_uid", g.ID);
            MySqlReader r = new MySqlReader(cmd);
            SafeDictionary<uint, ArsenalSingle> ass = new SafeDictionary<uint, ArsenalSingle>(1000);
            while (r.Read())
            {
                ArsenalSingle s = new ArsenalSingle();
                s.D_UID = r.ReadUInt32("d_uid");
                s.Name = r.ReadString("name");
                s.UID = r.ReadUInt32("item_uid");
                s.Donation = r.ReadUInt32("item_donation");
                s.Type = (ArsenalType)r.ReadByte("item_arsenal_type");
                ass.Add(s.UID, s);
            }
            r.Close();
            foreach (ArsenalSingle s in ass.Values)
            {
                s.Item = ConquerItemTable.GetSingleItem(s.UID);
                g.Arsenal.Inscribe(s.Type, s);
            }
            ass = new SafeDictionary<uint, ArsenalSingle>(1000);
            ass = null;

            cmd = new MySqlCommand(MySqlCommandType.SELECT);
            cmd.Select("guild_arsenals").Where("guild_uid", g.ID);
            r = new MySqlReader(cmd);
            if (r.Read())
            {
                g.A_Packet.Headgear_Avaliable = r.ReadByte("head_allowed") == 1;
                g.A_Packet.Armor_Avaliable = r.ReadByte("armor_allowed") == 1;
                g.A_Packet.Weapon_Avaliable = r.ReadByte("weapon_allowed") == 1;
                g.A_Packet.Ring_Avaliable = r.ReadByte("ring_allowed") == 1;
                g.A_Packet.Boots_Avaliable = r.ReadByte("boots_allowed") == 1;
                g.A_Packet.Necklace_Avaliable = r.ReadByte("neck_allowed") == 1;
                g.A_Packet.Fan_Avaliable = r.ReadByte("fan_allowed") == 1;
                g.A_Packet.Tower_Avaliable = r.ReadByte("tower_allowed") == 1;
            }
            r.Close();
        }

        public static void DeleteAll(Dictionary<uint, Game.ConquerStructures.Society.ArsenalSingle> Items)
        {
            foreach (ArsenalSingle AS in Items.Values)
            {
                MySqlCommand cmd = new MySqlCommand(MySqlCommandType.DELETE);
                cmd.Delete("guild_arsenalsdonation", "item_uid", AS.UID).Execute();
                cmd = new MySqlCommand(MySqlCommandType.UPDATE);
                cmd.Update("items").Set("Inscribed", 0).Where("UID", AS.UID).Execute();
            }
        }

        public static void Inscribe(Game.ConquerStructures.Society.ArsenalType Type, uint Donation, Interfaces.IConquerItem item, Game.Entity Entity)
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.INSERT);
            cmd.Insert("guild_arsenalsdonation").Insert("d_uid", Entity.UID).Insert("guild_uid", Entity.GuildID).Insert("name", Entity.Name).Insert("item_uid", item.UID).Insert("item_donation", Donation).Insert("item_arsenal_type", (byte)Type).Execute();
            cmd = new MySqlCommand(MySqlCommandType.UPDATE);
            cmd.Update("items").Set("Inscribed", 1).Where("UID", item.UID).Execute();
        }

        public static void Delete(uint UID)
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.DELETE);
            cmd.Delete("guild_arsenalsdonation", "item_uid", UID).Execute();
            cmd = new MySqlCommand(MySqlCommandType.UPDATE);
            cmd.Update("items").Set("Inscribed", 0).Where("UID", UID).Execute();
        }

        public static bool ContainsArsenal(ushort gID)
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT);
            cmd.Select("guild_arsenals").Where("guild_uid", gID).Execute();
            MySqlReader r = new MySqlReader(cmd);
            if (r.Read())
            {
                r.Close();
                return true;
            }
            else
            {
                r.Close();
                return false;
            }
        }

        public static void CreateArsenal(ushort gID, Game.ConquerStructures.Society.ArsenalType Type)
        {
            if (!ContainsArsenal(gID))
            {
                MySqlCommand cmd = new MySqlCommand(MySqlCommandType.INSERT);
                cmd.Insert("guild_arsenals").Insert("guild_uid", gID).Execute();
            }
            else
            {
                string val = "";
                switch (Type)
                {
                    case ArsenalType.Headgear: val = "head_allowed"; break;
                    case ArsenalType.Armor: val = "armor_allowed"; break;
                    case ArsenalType.Weapon: val = "weapon_allowed"; break;
                    case ArsenalType.Ring: val = "ring_allowed"; break;
                    case ArsenalType.Boots: val = "boots_allowed"; break;
                    case ArsenalType.Necklace: val = "neck_allowed"; break;
                    case ArsenalType.Fan: val = "fan_allowed"; break;
                    case ArsenalType.Tower: val = "tower_allowed"; break;
                }
                if (val != "")
                {
                    MySqlCommand cmd = new MySqlCommand(MySqlCommandType.UPDATE);
                    cmd.Update("guild_arsenals").Set(val, 1).Execute();
                }
            }
        }
    }
}
