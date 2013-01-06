using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer_Online_Server.Database
{
    public class ItemLog
    {
        public enum ItemLogAction
        {
            Add = 1,
            Remove = 2
        }
        public static void LogItem(uint uid, uint param1, ItemLogAction action)
        {
            //DateTime Date = DateTime.Now;
            //MySqlCommand cmd = new MySqlCommand(MySqlCommandType.INSERT);
            //cmd.Insert("itemlog").Insert("itemuid", uid).Insert("param1", param1).Insert("action", (byte)action)
            //    .Insert("datestring", Date.ToString()).Insert("datebinary", Date.ToBinary()).Execute();
        }
    }
}
