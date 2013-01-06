using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Conquer_Online_Server.Network.GamePackets;
namespace Conquer_Online_Server.Database
{
    public class ItemAddingTable
    {
        public static void GetAddingsForItem(Interfaces.IConquerItem item)
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT);
            cmd.Select("itemadding").Where("UID", item.UID);
            MySqlReader r = new MySqlReader(cmd);
            while (r.Read())
            {
                uint type = r.ReadUInt32("addingtype");
                if (type == 0)
                {
                    ItemAdding.Purification_ purification = new ItemAdding.Purification_();
                    purification.ItemUID = item.UID;
                    purification.Available = true;
                    purification.PurificationItemID = r.ReadUInt32("addingid");
                    purification.PurificationDuration = r.ReadUInt32("duration");
                    purification.PurificationLevel = r.ReadUInt32("addinglevel");
                    purification.AddedOn = DateTime.FromBinary(r.ReadInt64("addedon"));
                    if (purification.PurificationDuration != 0)
                    {
                        TimeSpan span1 = new TimeSpan(purification.AddedOn.AddSeconds(purification.PurificationDuration).Ticks);
                        TimeSpan span2 = new TimeSpan(DateTime.Now.Ticks);
                        int secondsleft = (int)(span1.TotalSeconds - span2.TotalSeconds);
                        if (secondsleft <= 0)
                        {
                            purification.Available = false;
                            RemoveAdding(item.UID, purification.PurificationItemID);
                            continue; 
                        }
                    }
                    item.Purification = purification;
                }
                else
                {
                    ItemAdding.Refinery_ extraeffect = new ItemAdding.Refinery_();
                    extraeffect.ItemUID = item.UID;
                    extraeffect.Available = true;
                    extraeffect.EffectID = r.ReadUInt32("addingid");
                    extraeffect.EffectLevel = r.ReadUInt32("addinglevel");
                    extraeffect.EffectPercent = r.ReadUInt32("addingpercent");
                    extraeffect.EffectDuration = r.ReadUInt32("duration");
                    extraeffect.AddedOn = DateTime.FromBinary(r.ReadInt64("addedon"));
                    if (extraeffect.EffectDuration != 0)
                    {
                        TimeSpan span1 = new TimeSpan(extraeffect.AddedOn.AddSeconds(extraeffect.EffectDuration).Ticks);
                        TimeSpan span2 = new TimeSpan(DateTime.Now.Ticks);
                        int secondsleft = (int)(span1.TotalSeconds - span2.TotalSeconds);
                        if (secondsleft <= 0)
                        {
                            extraeffect.Available = false;
                            RemoveAdding(item.UID, extraeffect.EffectID);
                            continue;
                        }
                    }
                    item.ExtraEffect = extraeffect;
                }
            }
            r.Close();
        }
        public static void RemoveAdding(uint UID, uint addingid)
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.DELETE);
            cmd.Delete("itemadding", "uid", UID).And("addingid", addingid).Execute();
        }
        public static void AddPurification(ItemAdding.Purification_ purification)
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.INSERT);
            cmd.Insert("itemadding")
                .Insert("uid", purification.ItemUID)
                .Insert("addingtype", 0)
                .Insert("addingid", purification.PurificationItemID)
                .Insert("addinglevel", purification.PurificationLevel)
                .Insert("duration", purification.PurificationDuration)
                .Insert("addedon", purification.AddedOn.Ticks).Execute();
        }
        public static void Stabilize(uint UID, uint addingid)
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.UPDATE);
            cmd.Update("itemadding").Set("duration", 0).Where("uid", UID).And("addingid", addingid).Execute();
        }
                        
        public static void AddExtraEffect(ItemAdding.Refinery_ extraeffect)
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.INSERT);
            cmd.Insert("itemadding")
                .Insert("uid", extraeffect.ItemUID)
                .Insert("addingtype", 1)
                .Insert("addingid", extraeffect.EffectID)
                .Insert("addinglevel", extraeffect.EffectLevel)
                .Insert("addingpercent", extraeffect.EffectPercent)
                .Insert("duration", extraeffect.EffectDuration)
                .Insert("addedon", extraeffect.AddedOn.Ticks).Execute();
        }
    }
}
