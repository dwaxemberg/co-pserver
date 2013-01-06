using System;
using Conquer_Online_Server.Network.GamePackets;

namespace Conquer_Online_Server.ServerBase
{
    public class Constants
    {
        public static readonly Message FullInventory = new Message("There is not enough room in your inventory!", System.Drawing.Color.MintCream, Message.TopLeft),
            TradeRequest = new Message("Trade request sent.", System.Drawing.Color.Tan, Message.TopLeft),
            Study = new Message("Congratulations. You just have got 500 StudyPoints!", System.Drawing.Color.Red, Message.TopLeft),
            TradeInventoryFull = new Message("There is not enough room in your partner inventory.", System.Drawing.Color.Tan, Message.TopLeft),
            TradeInProgress = new Message("An trade is already in progress. Try again later.", System.Drawing.Color.Tan, Message.TopLeft),
            FloorItemNotAvailable = new Message("You need to wait until you will be able to pick this item up!", System.Drawing.Color.MintCream, Message.TopLeft),
            JailItemUnusable = new Message("You can't use this item in jail!", System.Drawing.Color.MintCream, Message.TopLeft),
            PKForbidden = new Message("PK Forbidden in this map.", System.Drawing.Color.Tan, Message.TopLeft),
            ExpBallsUsed = new Message("You can use only ten exp balls a day. Try tomorrow.", System.Drawing.Color.Tan, Message.TopLeft),
            TeratoLife = new Message("Sorry, but TeratoDragon is in life now!.", System.Drawing.Color.Tan, Message.TopLeft),
            SpellLeveled = new Message("You have just leveled your spell.", System.Drawing.Color.Tan, Message.TopLeft),
            ProficiencyLeveled = new Message("You have just leveled your proficiency.", System.Drawing.Color.Tan, Message.TopLeft),
            ArrowsReloaded = new Message("Arrows Reloaded.", System.Drawing.Color.Red, Message.TopLeft),
            Warrent = new Message("The guards are looking for you!", System.Drawing.Color.Red, Message.TopLeft),
            VIPExpired = new Message("Your VIP has expired. Please reactivate your VIP if you wish to keep VIP services.", System.Drawing.Color.Red, Message.World),
            VIPLifetime = new Message("Your VIP service is unlimited.", System.Drawing.Color.Red, Message.World),
            WrongAccessory = new Message("You cannot wear this accessory and this item at the same time.", System.Drawing.Color.Red, Message.World),
            NoAccessory = new Message("You cannot wear an accessory without a support item.", System.Drawing.Color.Red, Message.World);

        public static Message VIPRemaining(string days, string hours)
        {
            return new Message("You have " + days + " day(s) and " + hours + " hour(s) of VIP service remaining.", System.Drawing.Color.Red, Message.World);
        }
        public static Message NoArrows(string name)
        {
            return new Message("Can't reload arrows, you are out of " + name +"s!", System.Drawing.Color.Red, Message.TopLeft);
        }
        public static Message Stigma(float percent, int time)
        {
            return new Message("Stigma activated. Your attack will be increased with " + percent + " for " + time + ".", System.Drawing.Color.Red, Message.TopLeft);
        }
        public static Message Accuracy(int time)
        {
            return new Message("Accuracy activated. Your agility will be increased a bit for " + time + ".", System.Drawing.Color.Red, Message.TopLeft);
        }
        public static Message Invisibility(int time)
        {
            return new Message("Invisibility activated. You will be invisible for monsters as long as you don't attack for " + time + ".", System.Drawing.Color.Red, Message.TopLeft);
        }
        public static Message Shield(float percent, int time)
        {
            return new Message("Shield activated. Your defence will be increased with " + percent + " for " + time + ".", System.Drawing.Color.Red, Message.TopLeft);
        }
        public static Message Dodge(float percent, int time)
        {
            return new Message("Dodge activated. Your dodge will be increased with " + percent + " for " + time + ".", System.Drawing.Color.Red, Message.TopLeft);
        }
        public static Message NoDrugs(int time)
        {
            return new Message("Poison star activated. You will not be able to use drugs for " + time + " seconds.", System.Drawing.Color.Red, Message.TopLeft);
        }
        public static Message ExtraExperience(uint experience)
        {
            return new Message("You have gained extra " + experience + " experience for killing the monster.", System.Drawing.Color.Red, Message.TopLeft);
        }
        public static Message VipExp(uint experience, uint vip)
        {
            return new Message("You have gained  X "  +vip+  " extra  " + experience + " experience for killing the monster cause you VIPlevel is " + (vip -1) +".", System.Drawing.Color.White, Message.TopLeft);
        }
        public static Message TeamExperience(uint experience)
        {
            return new Message("One of your teammates killed a monster so you gained " + experience + " experience.", System.Drawing.Color.Red, Message.TopLeft);
        }
        public static Message NoobTeamExperience(uint experience)
        {
            return new Message("One of your teammates killed a monster and because you have a noob inside your team, you gained " + experience + " experience.", System.Drawing.Color.Red, Message.TopLeft);
        }
        public static Message PickupGold(uint amount)
        {
            return new Message("You have picked up " + amount + " gold.", System.Drawing.Color.Red, Message.TopLeft);
        }
        public static Message PickupConquerPoints(uint amount)
        {
            return new Message("You have picked up " + amount + " conquer points.", System.Drawing.Color.Red, Message.TopLeft);
        }
        public static Message PickupItem(string name)
        {
            return new Message("You have picked up a/an " + name + " item.", System.Drawing.Color.Red, Message.TopLeft);
        }
        public static Message DummyLevelTooHigh()
        {
            return new Message("You can't attack this dummy because your level is not high enough.", System.Drawing.Color.Red, Message.TopLeft);
        }
        public static Message BoothItemSell(string buyername, string itemname, bool conquerpoints, uint cost)
        {
            return new Message("Congratulations. You just have just sold " + itemname + " to " + buyername + " for " + cost + (conquerpoints ? " ConquerPoints." : " Gold."), System.Drawing.Color.Red, Message.TopLeft);
        }
        public static Message Enchant(int origEnch, int newEnch)
        {
            if (newEnch <= origEnch)
                return new Message("You were unlucky. You didn't gain any more enchantment in your item. Your generated enchant is " + newEnch + ".", System.Drawing.Color.Red, Message.TopLeft);
            else
                return new Message("You were lucky. You gained more enchantment in your item. Your generated enchant is " + newEnch + ".", System.Drawing.Color.Red, Message.TopLeft);
        }
        public static Message VoteSpan(Client.GameState client)
        {
            if (DateTime.Now <= client.LastVote.AddHours(12))
            {
                TimeSpan agospan = client.LastVote.Subtract(DateTime.Now);
                TimeSpan tillspan = DateTime.Now.Subtract(client.LastVote);
                string message = "You last voted ";
                if (agospan.Hours >= 0)
                    message += agospan.Hours.ToString() + " hours, ";
                if (agospan.Minutes >= 0)
                    message += agospan.Minutes.ToString() + " minutes, and ";
                message += agospan.Seconds.ToString() + " ago. Please wait ";
                if (tillspan.Hours >= 0)
                    message += tillspan.Hours.ToString() + " hours, ";
                if (tillspan.Minutes >= 0)
                    message += agospan.Minutes.ToString() + " minutes, and ";
                message += tillspan.Seconds.ToString() + " ago. To vote again!";
                return new Message(message, System.Drawing.Color.Red, Message.TopLeft);
            }
            return new Message("You haven't voted in the past 12 hours. Vote now to gain an extra point!", System.Drawing.Color.Red, Message.TopLeft);
        }
        public const string DataHolderPath = "database\\",
        NpcFilePath = "database\\Npcs.txt",
        DMapsPath = "database\\",
         BannedPath = "database\\BannedList.txt",
        ShopsPath = "database\\Shops.dat",
        EShopsPath = "database\\EShops.ini",
        PortalsPath = "database\\Portals.ini",
        RevivePoints = "database\\RevivePoints.ini",
        MonstersPath = "database\\Monsters.txt",
        ItemBaseInfosPath = "database\\Items.txt",
        ItemPlusInfosPath = "database\\ItemsPlus.ini",
        UnhandledExceptionsPath = "database\\exceptions\\",
        ServerKey = "TQServer",
        WelcomeMessages = "database\\WelcomeMessages.txt",
        QuizShow = "database\\QuizShow.txt",
        GameCryptographyKey = "BC234xs45nme7HU9";
        public static string ServerName = "EternalAbyss";
        public const int MaxBroadcasts = 50;
        public static uint ExtraExperienceRate, ExtraSpellRate, ExtraProficiencyRate, MoneyDropRate, MoneyDropMultiple, ConquerPointsDropRate, ConquerPointsDropMultiple, ItemDropRate;
        public static string[] ItemDropQualityRates;
        public static string WebAccExt, ServerWebsite, WebVoteExt, WebDonateExt, ServerGMPass;
        public const sbyte pScreenDistance = 19;
        public const sbyte nScreenDistance = 19;
        public const sbyte remScreenDistance = 19;
        public const ushort DisconnectTimeOutSeconds = 10,
            FloorItemSeconds = 20,
            FloorItemAvailableAfter = 15;

        public const ushort SocketOneProgress = 100,
            SocketTwoProgress = 300;

        public static readonly System.Collections.Generic.List<ushort> PKForbiddenMaps = new System.Collections.Generic.List<ushort>()
        {
            1791,
            1036,
            1002,
            700,
            1039,
            
            1004,
            1006,
            601
        };
        public static readonly System.Collections.Generic.List<ushort> PKFreeMaps = new System.Collections.Generic.List<ushort>()
        {
            1116,
            1038,
            1005, 
            1507,
            6000,
            6001,
            6002, 
            6003,
            1844,
            1860,
            1858,
            9991,
            9992,
            9993,
            700
        };
    }
}
