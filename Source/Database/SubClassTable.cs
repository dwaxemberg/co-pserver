using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer_Online_Server.Database
{
    public class SubClassTable
    {
        
        public static void Load(Game.Entity Entity)
        {
            MySqlCommand Command = new MySqlCommand(MySqlCommandType.SELECT);
            Command.Select("subclasses").Where("id", Entity.UID);
            MySqlReader Reader = new MySqlReader(Command);
            while (Reader.Read())
            {
                Game.SubClass Sub = new Game.SubClass();
                Sub.ID = Reader.ReadByte("uid");
                Sub.Level = Reader.ReadByte("level");
                Sub.Phase = Reader.ReadByte("phase");
                Entity.SubClasses.Classes.Add(Sub.ID, Sub);
            }
            Reader.Close();
        }

        public static bool Contains(Game.Entity Entity, byte id)
        {
            bool Return = false;
            MySqlCommand Command = new MySqlCommand(MySqlCommandType.SELECT);
            Command.Select("subclasses").Where("id", Entity.UID).And("uid", id);
            MySqlReader Reader = new MySqlReader(Command);
            if (Reader.Read())
            {
                if (Reader.ReadByte("uid") == id)
                {

                    Return = true;
                }
            }
            Reader.Close();
            return Return;
        }

        public static void Insert(Game.Entity Entity, byte id)
        {
            MySqlCommand Command = new MySqlCommand(MySqlCommandType.INSERT);
            Command.Insert("subclasses")
                .Insert("uid", id)
                .Insert("id", Entity.UID)
                .Execute();
        }
        public static void Update56(Conquer_Online_Server.Game.Entity Entity)
        {
            MySqlCommand Command = new MySqlCommand(MySqlCommandType.UPDATE);
            Command.Update("entities")
                .Set("My_Title", Entity.TitleActivated)
                .Where("UID", Entity.UID)
                .Execute();
        }
        public static void Update(Game.Entity Entity, Game.SubClass SubClass)
        {
            MySqlCommand Command = new MySqlCommand(MySqlCommandType.UPDATE);
            Command.Update("subclasses")
                .Set("phase", SubClass.Phase)
                .Set("level", SubClass.Level)
                .Where("id", Entity.UID)
                .And("uid", SubClass.ID)
                .Execute();
        }

        public static void Update(Game.Entity Entity)
        {
            MySqlCommand Command = new MySqlCommand(MySqlCommandType.UPDATE);
            Command.Update("entities")
                .Set("StudyPoints", Entity.SubClasses.StudyPoints)
                .Where("UID", Entity.UID)
                .Execute();
        }
    }
}
