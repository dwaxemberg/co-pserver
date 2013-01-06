using System;
using System.Collections.Generic;
using Conquer_Online_Server.Game.ConquerStructures.Society;

namespace Conquer_Online_Server.Database
{
    public class KnownPersons
    {
        public static void LoadKnownPersons(Client.GameState client)
        {
            client.Friends = new SafeDictionary<uint,Friend>(50);
            client.Enemy = new SafeDictionary<uint, Enemy>(10);
            client.Partners = new SafeDictionary<uint, TradePartner>(40);
            client.Apprentices = new SafeDictionary<uint, Apprentice>(10);

            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT);
            cmd.Select("friends").Where("EntityID", client.Entity.UID);
            MySqlReader reader = new MySqlReader(cmd);
            while(reader.Read())
            {
                Friend friend = new Friend();
                friend.ID = reader.ReadUInt32("FriendID");
                friend.Name = reader.ReadString("FriendName");
                friend.Message = reader.ReadString("Message");
                client.Friends.Add(friend.ID, friend);
            }
            reader.Close();

            cmd = new MySqlCommand(MySqlCommandType.SELECT);
            cmd.Select("enemy").Where("EntityID", client.Entity.UID);
            reader = new MySqlReader(cmd);
            while (reader.Read())
            {
                Enemy enemy = new Enemy();
                enemy.ID = reader.ReadUInt32("EnemyID");
                enemy.Name = reader.ReadString("EnemyName");
                client.Enemy.Add(enemy.ID, enemy);
            }
            reader.Close();

            cmd = new MySqlCommand(MySqlCommandType.SELECT);
            cmd.Select("partners").Where("EntityID", client.Entity.UID);
            reader = new MySqlReader(cmd);
            while (reader.Read())
            {
                TradePartner partner = new TradePartner();
                partner.ID = reader.ReadUInt32("PartnerID");
                partner.Name = reader.ReadString("PartnerName");
                partner.ProbationStartedOn = DateTime.FromBinary(reader.ReadInt64("ProbationStartedOn"));
                client.Partners.Add(partner.ID, partner);
            }
            reader.Close();

            cmd = new MySqlCommand(MySqlCommandType.SELECT);
            cmd.Select("apprentice").Where("MentorID", client.Entity.UID);
            reader = new MySqlReader(cmd);
            while (reader.Read())
            {
                Apprentice app = new Apprentice();
                app.ID = reader.ReadUInt32("ApprenticeID");
                app.Name = reader.ReadString("ApprenticeName");
                app.EnroleDate = reader.ReadUInt32("EnroleDate");
                app.Actual_Experience = reader.ReadUInt64("Actual_Experience");
                app.Total_Experience = reader.ReadUInt64("Total_Experience");
                app.Actual_Plus = reader.ReadUInt16("Actual_Plus");
                app.Total_Plus = reader.ReadUInt16("Total_Plus");
                app.Actual_HeavenBlessing = reader.ReadUInt16("Actual_HeavenBlessing");
                app.Total_HeavenBlessing = reader.ReadUInt16("Total_HeavenBlessing");
                client.PrizeExperience += app.Actual_Experience;
                client.PrizePlusStone += app.Actual_Plus;
                client.PrizeHeavenBlessing += app.Actual_HeavenBlessing;
                client.Apprentices.Add(app.ID, app);

                if (client.PrizeExperience > 50 * 606)
                    client.PrizeExperience = 50 * 606;
            }
            reader.Close();

            cmd = new MySqlCommand(MySqlCommandType.SELECT);
            cmd.Select("apprentice").Where("ApprenticeID", client.Entity.UID);
            reader = new MySqlReader(cmd);
            while (reader.Read())
            {
                client.Mentor = new Mentor();
                client.Mentor.ID = reader.ReadUInt32("MentorID");
                client.Mentor.Name = reader.ReadString("MentorName");
                client.Mentor.EnroleDate = reader.ReadUInt32("EnroleDate");
                client.AsApprentice = new Apprentice();
                client.AsApprentice.ID = client.Entity.UID;
                client.AsApprentice.Name = client.Entity.Name;
                client.AsApprentice.EnroleDate = client.Mentor.EnroleDate;
                client.AsApprentice.Actual_Experience = reader.ReadUInt64("Actual_Experience");
                client.AsApprentice.Total_Experience = reader.ReadUInt64("Total_Experience");
                client.AsApprentice.Actual_Plus = reader.ReadUInt16("Actual_Plus");
                client.AsApprentice.Total_Plus = reader.ReadUInt16("Total_Plus");
                client.AsApprentice.Actual_HeavenBlessing = reader.ReadUInt16("Actual_HeavenBlessing");
                client.AsApprentice.Total_HeavenBlessing = reader.ReadUInt16("Total_HeavenBlessing");
            }
            reader.Close();
        }
        public static void SaveApprenticeInfo(Apprentice app)
        {
            if (app != null)
            {
                MySqlCommand mysqlcmd = new MySqlCommand(MySqlCommandType.UPDATE);
                mysqlcmd.Update("apprentice")
                    .Set("Actual_Experience", app.Actual_Experience.ToString())
                    .Set("Total_Experience", app.Total_Experience.ToString())
                    .Set("Actual_Plus", app.Actual_Plus.ToString())
                    .Set("Total_Plus", app.Total_Plus.ToString())
                    .Set("Actual_HeavenBlessing", app.Actual_HeavenBlessing.ToString())
                    .Set("Total_HeavenBlessing", app.Total_HeavenBlessing.ToString()).Where("ApprenticeID", app.ID).Execute();
            }
        }
        public static void AddMentor(Mentor mentor, Apprentice appr)
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.INSERT);
            cmd.Insert("apprentice").Insert("mentorid", mentor.ID).Insert("apprenticeid", appr.ID)
                .Insert("mentorname", mentor.Name).Insert("apprenticename", appr.Name).Insert("enroledate", appr.EnroleDate).Execute();
        }
        public static void RemoveMentor(uint apprenticeuid)
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.DELETE);
            cmd.Delete("apprentice", "apprenticeid", apprenticeuid).Execute();
        }
        public static void RemoveApprentice(Client.GameState client, uint apprenticeID)
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.DELETE);
            cmd.Delete("apprentice", "apprenticeid", apprenticeID).Execute();
        }
        public static void RemoveFriend(Client.GameState client, uint friendID)
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.DELETE);
            cmd.Delete("friends", "friendid", friendID).And("entityid", client.Entity.UID).Execute();
            cmd = new MySqlCommand(MySqlCommandType.DELETE);
            cmd.Delete("friends", "entityid", friendID).And("friendid", client.Entity.UID).Execute();
        }
        public static void RemovePartner(Client.GameState client, uint partnerID)
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.DELETE);
            cmd.Delete("partners", "partnerid", partnerID).And("entityid", client.Entity.UID).Execute();
            cmd = new MySqlCommand(MySqlCommandType.DELETE);
            cmd.Delete("partners", "entityid", partnerID).And("partnerid", client.Entity.UID).Execute();
        }
        public static void RemoveEnemy(Client.GameState client, uint enemyID)
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.DELETE);
            cmd.Delete("enemy", "enemyid", enemyID).And("entityid", client.Entity.UID).Execute();
        }
        public static void AddFriend(Client.GameState client, Friend friend)
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.INSERT);
            cmd.Insert("friends").Insert("entityid", client.Entity.UID).Insert("friendid", friend.ID)
                .Insert("friendname", friend.Name).Execute();
            cmd = new MySqlCommand(MySqlCommandType.INSERT);
            cmd.Insert("friends").Insert("entityid", friend.ID).Insert("friendid", client.Entity.UID)
                .Insert("friendname", client.Entity.Name).Execute();
        }
        public static void AddPartner(Client.GameState client, TradePartner partner)
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.INSERT);
            cmd.Insert("partners").Insert("entityid", client.Entity.UID).Insert("partnerid", partner.ID)
                .Insert("partnername", partner.Name).Insert("probationstartedon", partner.ProbationStartedOn.Ticks).Execute();
            cmd = new MySqlCommand(MySqlCommandType.INSERT);
            cmd.Insert("partners").Insert("entityid", partner.ID).Insert("partnerid", client.Entity.UID)
                .Insert("partnername", client.Entity.Name).Insert("probationstartedon", partner.ProbationStartedOn.Ticks).Execute();
        }
        public static void AddEnemy(Client.GameState client, Enemy enemy)
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.INSERT);
            cmd.Insert("enemy").Insert("entityid", client.Entity.UID).Insert("enemyid", enemy.ID)
                .Insert("enemyname", enemy.Name).Execute();
        }
        public static void UpdateMessageOnFriend(uint entityID, uint friendID, string message)
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.UPDATE);
            message = message.Replace("\'", "");
            cmd.Update("friends").Set("Message", message).Where("EntityID", friendID).And("FriendID", entityID).Execute();
        }
    }
}
