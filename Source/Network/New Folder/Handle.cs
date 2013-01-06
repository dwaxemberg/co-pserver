using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Conquer_Online_Server.Network.GamePackets;
using Conquer_Online_Server.Game.ConquerStructures;
using NpcDialogs;
using Conquer_Online_Server.ServerBase;
using Conquer_Online_Server.Interfaces;
using Conquer_Online_Server.Game.Features;
using Conquer_Online_Server.Network;

namespace Conquer_Online_Server.Game.Attacking
{
    public class Handle
    {
        private Attack attack;
        private Entity attacker, attacked, monster;
        public Client.GameState client2;
        ISkillInfo Spell;
        public Handle(Attack attack, Entity attacker, Entity attacked)
        {
            this.attack = attack;
            this.attacker = attacker;
            this.attacked = attacked;
            this.Execute();
        }
        #region Interations
        public class InteractionRequest
        {
            public InteractionRequest(Network.GamePackets.Attack attack, Game.Entity a_client)
            {
                Client.GameState client = a_client.Owner;

                client.Entity.InteractionInProgress = false;
                client.Entity.InteractionWith = attack.Attacked;
                client.Entity.InteractionType = 0;

                if (ServerBase.Kernel.GamePool.ContainsKey(attack.Attacked))
                {
                    Client.GameState clienttarget = ServerBase.Kernel.GamePool[attack.Attacked];
                    clienttarget.Entity.InteractionInProgress = false;
                    clienttarget.Entity.InteractionWith = client.Entity.UID;
                    clienttarget.Entity.InteractionType = 0;
                    attack.Attacker = client.Entity.UID;
                    attack.X = clienttarget.Entity.X;
                    attack.Y = clienttarget.Entity.Y;
                    attack.AttackType = 46;

                    clienttarget.Send(attack);
                }
            }
        }
        public class InteractionEffect
        {
            public InteractionEffect(Network.GamePackets.Attack attack, Game.Entity a_client)
            {
                Client.GameState client = a_client.Owner;

                if (ServerBase.Kernel.GamePool.ContainsKey(client.Entity.InteractionWith))
                {
                    Client.GameState clienttarget = ServerBase.Kernel.GamePool[client.Entity.InteractionWith];

                    if (clienttarget.Entity.X == client.Entity.X && clienttarget.Entity.Y == client.Entity.Y)
                    {
                        attack.Damage = client.Entity.InteractionType;
                        clienttarget.Entity.InteractionSet = true;
                        client.Entity.InteractionSet = true;
                        attack.Attacker = clienttarget.Entity.UID;
                        attack.Attacked = client.Entity.UID;
                        attack.AttackType = 47;
                        attack.X = clienttarget.Entity.X;
                        attack.Y = clienttarget.Entity.Y;

                        clienttarget.Send(attack);
                        attack.AttackType = 49;

                        attack.Attacker = client.Entity.UID;
                        attack.Attacked = clienttarget.Entity.UID;
                        client.SendScreen(attack, true);

                        attack.Attacker = clienttarget.Entity.UID;
                        attack.Attacked = client.Entity.UID;
                        client.SendScreen(attack, true);
                    }
                }
            }
        }
        public class InteractionAccept
        {
            public InteractionAccept(Network.GamePackets.Attack attack, Game.Entity a_client)
            {

                Client.GameState client = a_client.Owner;
                if (client.Entity.ContainsFlag(Network.GamePackets.Update.Flags.Ride))
                    client.Entity.RemoveFlag(Network.GamePackets.Update.Flags.Ride);
                if (client.Entity.InteractionWith != attack.Attacked)
                    return;
                client.Entity.InteractionSet = false;
                if (ServerBase.Kernel.GamePool.ContainsKey(attack.Attacked))
                {
                    Client.GameState clienttarget = ServerBase.Kernel.GamePool[attack.Attacked];//Conquer_Online_Server.ServerBase.Kernel.GamePool[attack.Attacked] as Client.GameState;
                    if (clienttarget.Entity.ContainsFlag(Network.GamePackets.Update.Flags.Ride))
                        clienttarget.Entity.RemoveFlag(Network.GamePackets.Update.Flags.Ride);
                    clienttarget.Entity.InteractionSet = false;
                    if (clienttarget.Entity.InteractionWith != client.Entity.UID)
                        return;
                    if (clienttarget.Entity.Body == 1003 || clienttarget.Entity.Body == 1004)
                    {
                        attack.Attacker = client.Entity.UID;
                        attack.X = client.Entity.X;
                        attack.Y = client.Entity.Y;
                        clienttarget.Send(attack);
                        clienttarget.Entity.InteractionInProgress = true;
                        client.Entity.InteractionInProgress = true;
                        clienttarget.Entity.InteractionType = attack.Damage;
                        clienttarget.Entity.InteractionX = client.Entity.X;
                        clienttarget.Entity.InteractionY = client.Entity.Y;
                        client.Entity.InteractionType = attack.Damage;
                        client.Entity.InteractionX = client.Entity.X;
                        client.Entity.InteractionY = client.Entity.Y;
                        if (clienttarget.Entity.X == client.Entity.X && clienttarget.Entity.Y == client.Entity.Y)
                        {
                            attack.Damage = client.Entity.InteractionType;
                            clienttarget.Entity.InteractionSet = true;
                            client.Entity.InteractionSet = true;
                            attack.Attacker = clienttarget.Entity.UID;
                            attack.Attacked = client.Entity.UID;
                            attack.AttackType = 47;
                            attack.X = clienttarget.Entity.X;
                            attack.Y = clienttarget.Entity.Y;
                            clienttarget.Send(attack);
                            attack.AttackType = 49;
                            attack.Attacker = client.Entity.UID;
                            attack.Attacked = clienttarget.Entity.UID;
                            client.SendScreen(attack, true);
                            attack.Attacker = clienttarget.Entity.UID;
                            attack.Attacked = client.Entity.UID;
                            client.SendScreen(attack, true);
                        }
                    }
                    else
                    {
                        attack.AttackType = 47;
                        attack.Attacker = client.Entity.UID;
                        attack.X = client.Entity.X;
                        attack.Y = client.Entity.Y;
                        clienttarget.Send(attack);
                        clienttarget.Entity.InteractionInProgress = true;
                        client.Entity.InteractionInProgress = true;
                        clienttarget.Entity.InteractionType = attack.Damage;
                        clienttarget.Entity.InteractionX = clienttarget.Entity.X;
                        clienttarget.Entity.InteractionY = clienttarget.Entity.Y;
                        client.Entity.InteractionType = attack.Damage;
                        client.Entity.InteractionX = clienttarget.Entity.X;
                        client.Entity.InteractionY = clienttarget.Entity.Y;
                        if (clienttarget.Entity.X == client.Entity.X && clienttarget.Entity.Y == client.Entity.Y)
                        {
                            clienttarget.Entity.InteractionSet = true;
                            client.Entity.InteractionSet = true;
                            attack.Attacker = clienttarget.Entity.UID;
                            attack.Attacked = client.Entity.UID;
                            attack.X = clienttarget.Entity.X;
                            attack.Y = clienttarget.Entity.Y;
                            clienttarget.Send(attack);
                            attack.AttackType = 49;
                            client.SendScreen(attack, true);
                            attack.Attacker = client.Entity.UID;
                            attack.Attacked = clienttarget.Entity.UID;
                            client.SendScreen(attack, true);
                        }
                    }
                }
            }
        }
        public class InteractionStopEffect
        {
            public InteractionStopEffect(Network.GamePackets.Attack attack, Game.Entity a_client)
            {
                Client.GameState client = a_client.Owner;

                if (ServerBase.Kernel.GamePool.ContainsKey(attack.Attacked))
                {
                    Client.GameState clienttarget = ServerBase.Kernel.GamePool[attack.Attacked];
                    attack.Attacker = client.Entity.UID;
                    attack.Attacked = clienttarget.Entity.UID;
                    attack.Damage = client.Entity.InteractionType;
                    attack.X = client.Entity.X;
                    attack.Y = client.Entity.Y;
                    attack.AttackType = 50;
                    client.SendScreen(attack, true);
                    attack.Attacker = clienttarget.Entity.UID; ;
                    attack.Attacked = client.Entity.UID;
                    clienttarget.SendScreen(attack, true);
                    client.Entity.Teleport(client.Entity.MapID, client.Entity.X, client.Entity.Y);
                    clienttarget.Entity.Teleport(clienttarget.Entity.MapID, clienttarget.Entity.X, clienttarget.Entity.Y);
                    client.Entity.InteractionType = 0;
                    client.Entity.InteractionWith = 0;
                    client.Entity.InteractionInProgress = false;
                    clienttarget.Entity.InteractionType = 0;
                    clienttarget.Entity.InteractionWith = 0;
                    clienttarget.Entity.InteractionInProgress = false;
                }
            }
        }
        public class InteractionRefuse
        {
            public InteractionRefuse(Network.GamePackets.Attack attack, Game.Entity a_client)
            {
                Client.GameState client = a_client.Owner;

                client.Entity.InteractionType = 0;
                client.Entity.InteractionWith = 0;
                client.Entity.InteractionInProgress = false;

                if (ServerBase.Kernel.GamePool.ContainsKey(attack.Attacked))
                {
                    Client.GameState clienttarget = ServerBase.Kernel.GamePool[attack.Attacked];
                    clienttarget.Entity.InteractionType = 0;
                    clienttarget.Entity.InteractionWith = 0;
                    clienttarget.Entity.InteractionInProgress = false;
                }
            }
        }
        #endregion
        private void Execute()
        {
            #region interactions
            if (attack != null)
            {
                switch (attack.AttackType)
                {
                    case (uint)Network.GamePackets.Attack.InteractionRequest:
                        new InteractionRequest(attack, attacker);
                        return;
                    case (uint)Network.GamePackets.Attack.InteractionEffect:
                        new InteractionEffect(attack, attacker);
                        return;

                    case (uint)Network.GamePackets.Attack.InteractionAccept:
                        new InteractionAccept(attack, attacker);
                        return;
                    case (uint)Network.GamePackets.Attack.InteractionRefuse:
                        new InteractionRefuse(attack, attacker);
                        return;
                    case (uint)Network.GamePackets.Attack.InteractionStopEffect:
                        new InteractionStopEffect(attack, attacker);
                        return;
                }
            }
            #endregion
            #region Monster -> Player \ Monster
            if (attack == null)
            {
                if (attacker.EntityFlag != EntityFlag.Monster)
                    return;
                if (attacker.Companion)
                {
                    if (ServerBase.Constants.PKForbiddenMaps.Contains(attacker.MapID))
                        return;
                }
                if (attacked.EntityFlag == EntityFlag.Player)
                {
                    if (!attacked.Owner.Attackable)
                        return;
                    if (attacked.Dead)
                        return;
                    //uint damageo = Calculate.Melee(attacker, attacked);
                    //uint damageo2 = Calculate.Melee(attacker, attacked);
                    if (attacked.Dead)
                    {
                        attacked.Die();

                        return;
                    }
                    #region New Monster Attack And Spells
                    #region SwordMaster
                    if (attacker.Name == "SwordMaster")
                    {

                        uint rand = (uint)Conquer_Online_Server.ServerBase.Kernel.Random.Next(1, 4);
                        switch (rand)
                        {


                            case 1:
                                attacker.MonsterInfo.SpellID = 10502;
                                break;
                            case 2:
                                attacker.MonsterInfo.SpellID = 10504;
                                break;
                            case 3:
                                attacker.MonsterInfo.SpellID = 10506;
                                break;

                            case 4:
                                attacker.MonsterInfo.SpellID = 10505;
                                break;
                        }


                        uint damage = 0;//Calculate.Magic(attacker, attacked, attacker.MonsterInfo.SpellID, 0);
                        damage += (uint)ServerBase.Kernel.Random.Next(1200, 1400);
                        // damage += attacker.MagicAttack;
                        //damage -= attacked.PhysicalDamageDecrease;
                        // damage -= attacked.MagicDamageDecrease;
                        //  damage -= attacked.MagicDefence;


                        if (attacked.Hitpoints <= damage)
                        {
                            attacked.Die();
                        }
                        else
                        {
                            attacked.Hitpoints -= damage;
                        }
                        if (attacker.Companion)
                            attacker.Owner.IncreaseExperience(Math.Min(damage, attacked.Hitpoints), true);
                        SpellUse suse = new SpellUse(true);
                        suse.Attacker = attacker.UID;
                        suse.SpellID = attacker.MonsterInfo.SpellID;
                        suse.X = attacked.X;
                        suse.Y = attacked.Y;
                        suse.Targets.Add(attacked.UID, damage);
                        attacked.Owner.SendScreen(suse, true);
                    }

                    #endregion
                    #region ThrillingSpook
                    if (attacker.Name == "ThrillingSpook" || attacker.Name == "LavaBeast")
                    {

                        uint rand = (uint)Conquer_Online_Server.ServerBase.Kernel.Random.Next(1, 4);
                        switch (rand)
                        {


                            case 1:
                                attacker.MonsterInfo.SpellID = 10363;
                                break;
                            case 2:
                                attacker.MonsterInfo.SpellID = 10360;
                                break;
                            case 3:
                                attacker.MonsterInfo.SpellID = 10361;
                                break;

                            case 4:
                                attacker.MonsterInfo.SpellID = 10362;
                                break;
                        }


                        uint damage = 0;//Calculate.Magic(attacker, attacked, attacker.MonsterInfo.SpellID, 0);
                        damage += (uint)ServerBase.Kernel.Random.Next(300, 500);
                        // damage += attacker.MagicAttack;
                        //damage -= attacked.PhysicalDamageDecrease;
                        // damage -= attacked.MagicDamageDecrease;
                        //  damage -= attacked.MagicDefence;


                        if (attacked.Hitpoints <= damage)
                        {
                            attacked.Die();
                        }
                        else
                        {
                            attacked.Hitpoints -= damage;
                        }
                        if (attacker.Companion)
                            attacker.Owner.IncreaseExperience(Math.Min(damage, attacked.Hitpoints), true);
                        SpellUse suse = new SpellUse(true);
                        suse.Attacker = attacker.UID;
                        suse.SpellID = attacker.MonsterInfo.SpellID;
                        suse.X = attacked.X;
                        suse.Y = attacked.Y;
                        suse.Targets.Add(attacked.UID, damage);
                        attacked.Owner.SendScreen(suse, true);
                    }

                    #endregion
                    #region SnowBanhe
                    if (attacker.Name == "SnowBanshee")
                    {

                        uint rand = (uint)Conquer_Online_Server.ServerBase.Kernel.Random.Next(1, 4);
                        switch (rand)
                        {


                            case 1:
                                attacker.MonsterInfo.SpellID = 30010;
                                break;
                            case 2:
                                attacker.MonsterInfo.SpellID = 30011;
                                break;
                            case 3:
                                attacker.MonsterInfo.SpellID = 30012;
                                break;

                            case 4:
                                attacker.MonsterInfo.SpellID = 10001;
                                break;
                        }


                        uint damage = 0;//Calculate.Magic(attacker, attacked, attacker.MonsterInfo.SpellID, 0);
                        damage += (uint)ServerBase.Kernel.Random.Next(1200, 1400);
                        // damage += attacker.MagicAttack;
                        //damage -= attacked.PhysicalDamageDecrease;
                        // damage -= attacked.MagicDamageDecrease;
                        //  damage -= attacked.MagicDefence;


                        if (attacked.Hitpoints <= damage)
                        {
                            attacked.Die();
                        }
                        else
                        {
                            attacked.Hitpoints -= damage;
                        }
                        if (attacker.Companion)
                            attacker.Owner.IncreaseExperience(Math.Min(damage, attacked.Hitpoints), true);
                        SpellUse suse = new SpellUse(true);
                        suse.Attacker = attacker.UID;
                        suse.SpellID = attacker.MonsterInfo.SpellID;
                        suse.X = attacked.X;
                        suse.Y = attacked.Y;
                        suse.Targets.Add(attacked.UID, damage);
                        attacked.Owner.SendScreen(suse, true);
                    }
                    #endregion
                    #region TreatoDragon
                    if (attacker.Name == "TeratoDragon")
                    {
                        uint rand = (uint)Conquer_Online_Server.ServerBase.Kernel.Random.Next(1, 5);
                        switch (rand)
                        {


                            case 1:
                                attacker.MonsterInfo.SpellID = 7012;
                                break;
                            case 2:
                                attacker.MonsterInfo.SpellID = 7013;
                                break;
                            case 3:
                                attacker.MonsterInfo.SpellID = 7015;
                                break;
                            case 4:
                                attacker.MonsterInfo.SpellID = 7016;
                                break;
                            case 5:
                                attacker.MonsterInfo.SpellID = 7017;
                                break;
                           
                        }


                        uint damage = 0;//Calculate.Magic(attacker, attacked, attacker.MonsterInfo.SpellID, 0);
                        damage += (uint)ServerBase.Kernel.Random.Next(1200, 1400);
                        // damage += attacker.MagicAttack;
                        //damage -= attacked.PhysicalDamageDecrease;
                        // damage -= attacked.MagicDamageDecrease;
                        //  damage -= attacked.MagicDefence;


                        if (attacked.Hitpoints <= damage)
                        {
                            attacked.Die();
                        }
                        else
                        {
                            attacked.Hitpoints -= damage;
                        }
                        if (attacker.Companion)
                            attacker.Owner.IncreaseExperience(Math.Min(damage, attacked.Hitpoints), true);
                        SpellUse suse = new SpellUse(true);
                        suse.Attacker = attacker.UID;
                        suse.SpellID = attacker.MonsterInfo.SpellID;
                        suse.X = attacked.X;
                        suse.Y = attacked.Y;
                        suse.Targets.Add(attacked.UID, damage);
                        attacked.Owner.SendScreen(suse, true);


                    }
                    #endregion

                    #endregion

                    if (attacker.MonsterInfo.SpellID == 0)
                    {
                        uint damage = Calculate.Melee(attacker, attacked);

                        if (attacked.Hitpoints <= damage)
                        {
                            attacked.Die();
                        }
                        else
                        {
                            attacked.Hitpoints -= damage;
                        }


                        attack = new Attack(true);
                        attack.Attacker = attacker.UID;
                        attack.Attacked = attacked.UID;
                        attack.AttackType = Attack.Melee;
                        attack.Damage = damage;
                        attack.X = attacked.X;
                        attack.Y = attacked.Y;
                        //attack.FirstEffect = EffectValue.Block;
                        attacked.Owner.SendScreen(attack, true);
                    }
                    else
                    {
                        uint damage = Calculate.Magic(attacker, attacked, attacker.MonsterInfo.SpellID, 0);

                        if (attacked.Hitpoints <= damage)
                        {
                            attacked.Die();
                        }

                        else
                        {
                            attacked.Hitpoints -= damage;
                        }
                        if (attacker.Companion)
                            attacker.Owner.IncreaseExperience(Math.Min(damage, attacked.Hitpoints), true);

                        SpellUse suse = new SpellUse(true);
                        suse.Attacker = attacker.UID;
                        suse.SpellID = attacker.MonsterInfo.SpellID;
                        suse.X = attacked.X;
                        suse.Y = attacked.Y;
                        suse.Targets.Add(attacked.UID, damage);
                        attacked.Owner.SendScreen(suse, true);
                    }
                }
                else
                {
                    if (attacker.MonsterInfo.SpellID == 0)
                    {
                        uint damage = Calculate.Melee(attacker, attacked);
                        attack = new Attack(true);
                        attack.Attacker = attacker.UID;
                        attack.Attacked = attacked.UID;
                        attack.AttackType = Attack.Melee;
                        attack.Damage = damage;
                        attack.X = attacked.X;
                        attack.Y = attacked.Y;
                        attacked.MonsterInfo.SendScreen(attack);
                        if (attacker.Companion)
                            if (damage > attacked.Hitpoints)
                                attacker.Owner.IncreaseExperience(Math.Min(damage, attacked.Hitpoints), true);
                            else
                                attacker.Owner.IncreaseExperience(damage, true);

                        if (attacked.Hitpoints <= damage)
                        {
                            attacked.Die(attacker);
                            attack = new Attack(true);
                            attack.Attacker = attacker.UID;
                            attack.Attacked = attacked.UID;
                            attack.AttackType = Network.GamePackets.Attack.Kill;
                            attack.X = attacked.X;
                            attack.Y = attacked.Y;
                            attacked.MonsterInfo.SendScreen(attack);
                        }
                        else
                        {
                            attacked.Hitpoints -= damage;
                        }
                    }
                    else
                    {
                        uint damage = Calculate.Magic(attacker, attacked, attacker.MonsterInfo.SpellID, 0);
                        SpellUse suse = new SpellUse(true);
                        suse.Attacker = attacker.UID;
                        suse.SpellID = attacker.MonsterInfo.SpellID;
                        suse.X = attacked.X;
                        suse.Y = attacked.Y;
                        suse.Targets.Add(attacked.UID, damage);
                        attacked.MonsterInfo.SendScreen(suse);
                        if (attacker.Companion)
                            if (damage > attacked.Hitpoints)
                                attacker.Owner.IncreaseExperience(Math.Min(damage, attacked.Hitpoints), true);
                            else
                                attacker.Owner.IncreaseExperience(damage, true);

                        if (attacked.Hitpoints <= damage)
                        {
                            attacked.Die(attacker);
                            attack = new Attack(true);
                            attack.Attacker = attacker.UID;
                            attack.Attacked = attacked.UID;
                            attack.AttackType = Network.GamePackets.Attack.Kill;
                            attack.X = attacked.X;
                            attack.Y = attacked.Y;
                            attacked.MonsterInfo.SendScreen(attack);
                        }
                        else
                        {
                            attacked.Hitpoints -= damage;
                        }
                    }
                }
            }
            #endregion
            #region Player -> Player \ Monster \ Sob Npc
            else
            {
                #region Merchant
                if (attack.AttackType == Attack.MerchantAccept || attack.AttackType == Attack.MerchantRefuse)
                {

                    attacker.AttackPacket = null;
                    return;
                }
                #endregion
                #region Marriage
                if (attack.AttackType == Attack.MarriageAccept || attack.AttackType == Attack.MarriageRequest)
                {
                    if (attack.AttackType == Attack.MarriageRequest)
                    {
                        Client.GameState Spouse = null;
                        uint takeout = attack.Attacked;
                        if (takeout == attacker.UID)
                            takeout = attack.Attacker;
                        if (ServerBase.Kernel.GamePool.TryGetValue(takeout, out Spouse))
                        {
                            if (attacker.Spouse != "None" || Spouse.Entity.Spouse != "None")
                            {
                                attacker.Owner.Send(new Message("You cannot marry someone that is already married with someone else!", System.Drawing.Color.Black, Message.TopLeft));
                            }
                            else
                            {
                                uint id1 = attacker.Mesh % 10, id2 = Spouse.Entity.Mesh % 10;

                                if (id1 <= 2 && id2 >= 3 || id1 >= 2 && id2 <= 3)
                                {

                                    attack.X = Spouse.Entity.X;
                                    attack.Y = Spouse.Entity.Y;

                                    Spouse.Send(attack);
                                }
                                else
                                {
                                    attacker.Owner.Send(new Message("You cannot marry someone of your gender!", System.Drawing.Color.Black, Message.TopLeft));
                                }
                            }
                        }
                    }
                    else
                    {
                        Client.GameState Spouse = null;
                        if (ServerBase.Kernel.GamePool.TryGetValue(attack.Attacked, out Spouse))
                        {
                            if (attacker.Spouse != "None" || Spouse.Entity.Spouse != "None")
                            {
                                attacker.Owner.Send(new Message("You cannot marry someone that is already married with someone else!", System.Drawing.Color.Black, Message.TopLeft));
                            }
                            else
                            {
                                if (attacker.Mesh % 10 <= 2 && Spouse.Entity.Mesh % 10 >= 3 || attacker.Mesh % 10 >= 3 && Spouse.Entity.Mesh % 10 <= 2)
                                {
                                    Spouse.Entity.Spouse = attacker.Name;
                                    attacker.Spouse = Spouse.Entity.Name;
                                    Message message = null;
                                    if (Spouse.Entity.Mesh % 10 >= 3)
                                        message = new Message("Joy and happiness! " + Spouse.Entity.Name + " and " + attacker.Name + " have joined together in the holy marriage. We wish them a stone house.", System.Drawing.Color.BurlyWood, Message.Center);
                                    else
                                        message = new Message("Joy and happiness! " + attacker.Name + " and " + attacker.Spouse + " have joined together in the holy marriage. We wish them a stone house.", System.Drawing.Color.BurlyWood, Message.Center);




                                    var varr = ServerBase.Kernel.GamePool.Values.GetEnumerator();
                                    varr.MoveNext();
                                    int COunt = ServerBase.Kernel.GamePool.Count;
                                    for (uint x = 0;
                                        x < COunt;
                                        x++)
                                    {
                                        if (x >= COunt) break;

                                        Client.GameState client = (varr.Current as Client.GameState);

                                        client.Send(message);

                                        varr.MoveNext();

                                    }


                                    Spouse.Entity.Update(_String.Effect, "firework-2love", true);
                                    attacker.Update(_String.Effect, "firework-2love", true);
                                }
                                else
                                {
                                    attacker.Owner.Send(new Message("You cannot marry someone of your gender!", System.Drawing.Color.Black, Message.TopLeft));
                                }
                            }
                        }
                    }
                }
                #endregion
            #endregion

                #region Attacking
                else
                {
                    attacker.Owner.Attackable = true;
                    Entity attacked = null;
                    SobNpcSpawn attackedsob = null;

                    #region Checks
                    if (attack.Attacker != attacker.UID)
                        return;
                    if (attacker.EntityFlag != EntityFlag.Player)
                        return;
                    attacker.RemoveFlag(Update.Flags.Invisibility);

                    bool pass = false;
                    if (attack.AttackType == Attack.Melee)
                    {
                        if (attacker.OnFatalStrike())
                        {
                            if (attack.Attacked < 600000)
                            {
                                pass = true;
                            }
                        }
                    }
                    ushort decrease = 0;
                    if (attacker.OnCyclone())
                        decrease = 700;
                    if (attacker.OnSuperman())
                        decrease = 300;
                    if (!pass)
                    {
                        int milliSeconds = 1000 - attacker.Agility - decrease;
                        if (milliSeconds < 0 || milliSeconds > 5000)
                            milliSeconds = 0;
                        if (Time32.Now < attacker.AttackStamp.AddMilliseconds(milliSeconds))
                            return;
                    }
                    if (attacker.Dead)
                    {
                        if (attacker.AttackPacket != null)
                            attacker.AttackPacket = null;
                        return;
                    }
                    attacker.AttackStamp = Time32.Now;
                    if (attacker.Owner.QualifierGroup != null)
                    {
                        if (Time32.Now < attacker.Owner.QualifierGroup.CreateTime.AddSeconds(12))
                        {
                            return;
                        }
                    }

                restart:

                    #region Extract attack information
                    ushort SpellID = 0, X = 0, Y = 0;
                    uint Target = 0;
                    if (attack.AttackType == Attack.Magic)
                    {
                        if (!attack.Decoded)
                        {
                            #region GetSkillID
                            SpellID = Convert.ToUInt16(((long)attack.ToArray()[24] & 0xFF) | (((long)attack.ToArray()[25] & 0xFF) << 8));
                            SpellID ^= (ushort)0x915d;
                            SpellID ^= (ushort)attacker.UID;
                            SpellID = (ushort)(SpellID << 0x3 | SpellID >> 0xd);
                            SpellID -= 0xeb42;
                            #endregion
                            #region GetCoords
                            X = (ushort)((attack.ToArray()[16] & 0xFF) | ((attack.ToArray()[17] & 0xFF) << 8));
                            X = (ushort)(X ^ (uint)(attacker.UID & 0xffff) ^ 0x2ed6);
                            X = (ushort)(((X << 1) | ((X & 0x8000) >> 15)) & 0xffff);
                            X = (ushort)((X | 0xffff0000) - 0xffff22ee);

                            Y = (ushort)((attack.ToArray()[18] & 0xFF) | ((attack.ToArray()[19] & 0xFF) << 8));
                            Y = (ushort)(Y ^ (uint)(attacker.UID & 0xffff) ^ 0xb99b);
                            Y = (ushort)(((Y << 5) | ((Y & 0xF800) >> 11)) & 0xffff);
                            Y = (ushort)((Y | 0xffff0000) - 0xffff8922);
                            #endregion
                            #region GetTarget
                            Target = ((uint)attack.ToArray()[12] & 0xFF) | (((uint)attack.ToArray()[13] & 0xFF) << 8) | (((uint)attack.ToArray()[14] & 0xFF) << 16) | (((uint)attack.ToArray()[15] & 0xFF) << 24);
                            Target = ((((Target & 0xffffe000) >> 13) | ((Target & 0x1fff) << 19)) ^ 0x5F2D2463 ^ attacker.UID) - 0x746F4AE6;
                            #endregion

                            attack.X = X;
                            attack.Y = Y;
                            attack.Damage = SpellID;
                            attack.Attacked = Target;
                            attack.Decoded = true;
                        }
                        else
                        {
                            X = attack.X;
                            Y = attack.Y;
                            SpellID = (ushort)attack.Damage;
                            Target = attack.Attacked;
                        }
                    }
                    #endregion
                    #endregion

                    if (attacker.ContainsFlag(Update.Flags.Ride))
                    {
                        attacker.Stamina = 100;
                        if (attacker.RidingCropID == 0)
                        {
                            if (attack.AttackType != Attack.Magic)
                                attacker.RemoveFlag(Update.Flags.Ride);
                            else
                                if (!(SpellID == 7003 || SpellID == 7002))
                                    attacker.RemoveFlag(Update.Flags.Ride);
                        }
                    }
                    //if (attacked.ContainsFlag(Update.Flags.Ride))
                    //{
                    //    //if (attack.AttackType != Attack.Magic)
                    //    //    attacker.RemoveFlag(Update.Flags.Ride);
                    //    //else
                    //    if (!(SpellID == 7003 || SpellID == 7002))
                    //        attacked.RemoveFlag(Update.Flags.Ride);
                    //}
                    if (attacker.ContainsFlag(Update.Flags.CastPray))
                        attacker.RemoveFlag(Update.Flags.CastPray);
                    if (attacker.ContainsFlag(Update.Flags.Praying))
                        attacker.RemoveFlag(Update.Flags.Praying);
                    Interfaces.IConquerItem item = new Network.GamePackets.ConquerItem(true);
                    // BlessEffect.Handler(client);

                    #region GemuriEfecte
                    {
                        #region DragonGem
                        if (!attacker.Owner.Equipment.Free((byte)ConquerItem.RightWeapon))
                        {
                            Interfaces.IConquerItem rightweapon = attacker.Owner.Equipment.TryGetItem((byte)ConquerItem.RightWeapon);
                            if (rightweapon.SocketOne == Game.Enums.Gem.SuperDragonGem || rightweapon.SocketTwo == Game.Enums.Gem.SuperDragonGem)
                            {
                                if (ServerBase.Kernel.Rate(0.5))
                                {
                                    _String str = new _String(true);
                                    str.UID = attacker.UID;
                                    str.TextsCount = 1;
                                    str.Type = _String.Effect;
                                    str.Texts.Add("goldendragon");

                                    attacker.Owner.SendScreen(str, true);
                                }
                            }
                        }

                        if (!attacker.Owner.Equipment.Free((byte)ConquerItem.LeftWeapon))
                        {
                            Interfaces.IConquerItem rightweapon = attacker.Owner.Equipment.TryGetItem((byte)ConquerItem.LeftWeapon);
                            if (rightweapon.SocketOne == Game.Enums.Gem.SuperDragonGem || rightweapon.SocketTwo == Game.Enums.Gem.SuperDragonGem)
                            {
                                if (ServerBase.Kernel.Rate(0.5))
                                {
                                    _String str = new _String(true);
                                    str.UID = attacker.UID;
                                    str.TextsCount = 1;
                                    str.Type = _String.Effect;
                                    str.Texts.Add("goldendragon");

                                    attacker.Owner.SendScreen(str, true);
                                }
                            }
                        }

                        if (!attacker.Owner.Equipment.Free((byte)ConquerItem.Head))
                        {
                            Interfaces.IConquerItem rightweapon = attacker.Owner.Equipment.TryGetItem((byte)ConquerItem.Head);
                            if (rightweapon.SocketOne == Game.Enums.Gem.SuperDragonGem || rightweapon.SocketTwo == Game.Enums.Gem.SuperDragonGem)
                            {
                                if (ServerBase.Kernel.Rate(0.5))
                                {
                                    _String str = new _String(true);
                                    str.UID = attacker.UID;
                                    str.TextsCount = 1;
                                    str.Type = _String.Effect;
                                    str.Texts.Add("goldendragon");

                                    attacker.Owner.SendScreen(str, true);
                                }
                            }
                        }

                        if (!attacker.Owner.Equipment.Free((byte)ConquerItem.Armor))
                        {
                            Interfaces.IConquerItem rightweapon = attacker.Owner.Equipment.TryGetItem((byte)ConquerItem.Armor);
                            if (rightweapon.SocketOne == Game.Enums.Gem.SuperDragonGem || rightweapon.SocketTwo == Game.Enums.Gem.SuperDragonGem)
                            {
                                if (ServerBase.Kernel.Rate(0.5))
                                {
                                    _String str = new _String(true);
                                    str.UID = attacker.UID;
                                    str.TextsCount = 1;
                                    str.Type = _String.Effect;
                                    str.Texts.Add("goldendragon");

                                    attacker.Owner.SendScreen(str, true);
                                }
                            }
                        }

                        if (!attacker.Owner.Equipment.Free((byte)ConquerItem.Boots))
                        {
                            Interfaces.IConquerItem rightweapon = attacker.Owner.Equipment.TryGetItem((byte)ConquerItem.Boots);
                            if (rightweapon.SocketOne == Game.Enums.Gem.SuperDragonGem || rightweapon.SocketTwo == Game.Enums.Gem.SuperDragonGem)
                            {
                                if (ServerBase.Kernel.Rate(0.5))
                                {
                                    _String str = new _String(true);
                                    str.UID = attacker.UID;
                                    str.TextsCount = 1;
                                    str.Type = _String.Effect;
                                    str.Texts.Add("goldendragon");

                                    attacker.Owner.SendScreen(str, true);
                                }
                            }
                        }
                        if (!attacker.Owner.Equipment.Free((byte)ConquerItem.Necklace))
                        {
                            Interfaces.IConquerItem rightweapon = attacker.Owner.Equipment.TryGetItem((byte)ConquerItem.Necklace);
                            if (rightweapon.SocketOne == Game.Enums.Gem.SuperDragonGem || rightweapon.SocketTwo == Game.Enums.Gem.SuperDragonGem)
                            {
                                if (ServerBase.Kernel.Rate(0.5))
                                {
                                    _String str = new _String(true);
                                    str.UID = attacker.UID;
                                    str.TextsCount = 1;
                                    str.Type = _String.Effect;
                                    str.Texts.Add("goldendragon");

                                    attacker.Owner.SendScreen(str, true);
                                }
                            }
                        }
                        if (!attacker.Owner.Equipment.Free((byte)ConquerItem.Ring))
                        {
                            Interfaces.IConquerItem rightweapon = attacker.Owner.Equipment.TryGetItem((byte)ConquerItem.Ring);
                            if (rightweapon.SocketOne == Game.Enums.Gem.SuperDragonGem || rightweapon.SocketTwo == Game.Enums.Gem.SuperDragonGem)
                            {
                                if (ServerBase.Kernel.Rate(0.5))
                                {
                                    _String str = new _String(true);
                                    str.UID = attacker.UID;
                                    str.TextsCount = 1;
                                    str.Type = _String.Effect;
                                    str.Texts.Add("goldendragon");

                                    attacker.Owner.SendScreen(str, true);
                                }
                            }
                        }
                        #endregion
                        #region phoenix Gem
                        else if (!attacker.Owner.Equipment.Free((byte)ConquerItem.RightWeapon))
                        {
                            Interfaces.IConquerItem rightweapon1 = attacker.Owner.Equipment.TryGetItem((byte)ConquerItem.RightWeapon);
                            if (rightweapon1.SocketOne == Game.Enums.Gem.SuperPhoenixGem || rightweapon1.SocketTwo == Game.Enums.Gem.SuperPhoenixGem)
                            {
                                if (ServerBase.Kernel.Rate(0.5))
                                {
                                    _String str = new _String(true);
                                    str.UID = attacker.UID;
                                    str.TextsCount = 1;
                                    str.Type = _String.Effect;
                                    str.Texts.Add("phoenix");

                                    attacker.Owner.SendScreen(str, true);
                                }
                            }
                        }

                        else if (!attacker.Owner.Equipment.Free((byte)ConquerItem.LeftWeapon))
                        {
                            Interfaces.IConquerItem rightweapon1 = attacker.Owner.Equipment.TryGetItem((byte)ConquerItem.LeftWeapon);
                            if (rightweapon1.SocketOne == Game.Enums.Gem.SuperPhoenixGem || rightweapon1.SocketTwo == Game.Enums.Gem.SuperPhoenixGem)
                            {
                                if (ServerBase.Kernel.Rate(0.5))
                                {
                                    _String str = new _String(true);
                                    str.UID = attacker.UID;
                                    str.TextsCount = 1;
                                    str.Type = _String.Effect;
                                    str.Texts.Add("phoenix");

                                    attacker.Owner.SendScreen(str, true);
                                }
                            }
                        }

                        else if (!attacker.Owner.Equipment.Free((byte)ConquerItem.Head))
                        {
                            Interfaces.IConquerItem rightweapon1 = attacker.Owner.Equipment.TryGetItem((byte)ConquerItem.Head);
                            if (rightweapon1.SocketOne == Game.Enums.Gem.SuperPhoenixGem || rightweapon1.SocketTwo == Game.Enums.Gem.SuperPhoenixGem)
                            {
                                if (ServerBase.Kernel.Rate(0.5))
                                {
                                    _String str = new _String(true);
                                    str.UID = attacker.UID;
                                    str.TextsCount = 1;
                                    str.Type = _String.Effect;
                                    str.Texts.Add("phoenix");

                                    attacker.Owner.SendScreen(str, true);
                                }
                            }
                        }

                        else if (!attacker.Owner.Equipment.Free((byte)ConquerItem.Armor))
                        {
                            Interfaces.IConquerItem rightweapon1 = attacker.Owner.Equipment.TryGetItem((byte)ConquerItem.Armor);
                            if (rightweapon1.SocketOne == Game.Enums.Gem.SuperPhoenixGem || rightweapon1.SocketTwo == Game.Enums.Gem.SuperPhoenixGem)
                            {
                                if (ServerBase.Kernel.Rate(0.5))
                                {
                                    _String str = new _String(true);
                                    str.UID = attacker.UID;
                                    str.TextsCount = 1;
                                    str.Type = _String.Effect;
                                    str.Texts.Add("phoenix");

                                    attacker.Owner.SendScreen(str, true);
                                }
                            }
                        }

                        else if (!attacker.Owner.Equipment.Free((byte)ConquerItem.Boots))
                        {
                            Interfaces.IConquerItem rightweapon1 = attacker.Owner.Equipment.TryGetItem((byte)ConquerItem.Boots);
                            if (rightweapon1.SocketOne == Game.Enums.Gem.SuperPhoenixGem || rightweapon1.SocketTwo == Game.Enums.Gem.SuperPhoenixGem)
                            {
                                if (ServerBase.Kernel.Rate(0.5))
                                {
                                    _String str = new _String(true);
                                    str.UID = attacker.UID;
                                    str.TextsCount = 1;
                                    str.Type = _String.Effect;
                                    str.Texts.Add("phoenix");

                                    attacker.Owner.SendScreen(str, true);
                                }
                            }
                        }
                        else if (!attacker.Owner.Equipment.Free((byte)ConquerItem.Necklace))
                        {
                            Interfaces.IConquerItem rightweapon1 = attacker.Owner.Equipment.TryGetItem((byte)ConquerItem.Necklace);
                            if (rightweapon1.SocketOne == Game.Enums.Gem.SuperPhoenixGem || rightweapon1.SocketTwo == Game.Enums.Gem.SuperPhoenixGem)
                            {
                                if (ServerBase.Kernel.Rate(0.5))
                                {
                                    _String str = new _String(true);
                                    str.UID = attacker.UID;
                                    str.TextsCount = 1;
                                    str.Type = _String.Effect;
                                    str.Texts.Add("phoenix");

                                    attacker.Owner.SendScreen(str, true);
                                }
                            }
                        }
                        else if (!attacker.Owner.Equipment.Free((byte)ConquerItem.Ring))
                        {
                            Interfaces.IConquerItem rightweapon1 = attacker.Owner.Equipment.TryGetItem((byte)ConquerItem.Ring);
                            if (rightweapon1.SocketOne == Game.Enums.Gem.SuperPhoenixGem || rightweapon1.SocketTwo == Game.Enums.Gem.SuperPhoenixGem)
                            {
                                if (ServerBase.Kernel.Rate(0.5))
                                {
                                    _String str = new _String(true);
                                    str.UID = attacker.UID;
                                    str.TextsCount = 1;
                                    str.Type = _String.Effect;
                                    str.Texts.Add("phoenix");

                                    attacker.Owner.SendScreen(str, true);
                                }
                            }
                        }
                        #endregion
                        #region RainbowGem Gem
                        else if (!attacker.Owner.Equipment.Free((byte)ConquerItem.RightWeapon))
                        {
                            Interfaces.IConquerItem rightweapon2 = attacker.Owner.Equipment.TryGetItem((byte)ConquerItem.RightWeapon);
                            if (rightweapon2.SocketOne == Game.Enums.Gem.SuperRainbowGem || rightweapon2.SocketTwo == Game.Enums.Gem.SuperRainbowGem)
                            {
                                if (ServerBase.Kernel.Rate(0.5))
                                {
                                    _String str = new _String(true);
                                    str.UID = attacker.UID;
                                    str.TextsCount = 1;
                                    str.Type = _String.Effect;
                                    str.Texts.Add("rainbow");

                                    attacker.Owner.SendScreen(str, true);
                                }
                            }
                        }

                        else if (!attacker.Owner.Equipment.Free((byte)ConquerItem.LeftWeapon))
                        {
                            Interfaces.IConquerItem rightweapon2 = attacker.Owner.Equipment.TryGetItem((byte)ConquerItem.LeftWeapon);
                            if (rightweapon2.SocketOne == Game.Enums.Gem.SuperRainbowGem || rightweapon2.SocketTwo == Game.Enums.Gem.SuperRainbowGem)
                            {
                                if (ServerBase.Kernel.Rate(0.5))
                                {
                                    _String str = new _String(true);
                                    str.UID = attacker.UID;
                                    str.TextsCount = 1;
                                    str.Type = _String.Effect;
                                    str.Texts.Add("rainbow");

                                    attacker.Owner.SendScreen(str, true);
                                }
                            }
                        }

                        else if (!attacker.Owner.Equipment.Free((byte)ConquerItem.Head))
                        {
                            Interfaces.IConquerItem rightweapon2 = attacker.Owner.Equipment.TryGetItem((byte)ConquerItem.Head);
                            if (rightweapon2.SocketOne == Game.Enums.Gem.SuperRainbowGem || rightweapon2.SocketTwo == Game.Enums.Gem.SuperRainbowGem)
                            {
                                if (ServerBase.Kernel.Rate(0.5))
                                {
                                    _String str = new _String(true);
                                    str.UID = attacker.UID;
                                    str.TextsCount = 1;
                                    str.Type = _String.Effect;
                                    str.Texts.Add("rainbow");

                                    attacker.Owner.SendScreen(str, true);
                                }
                            }
                        }

                        else if (!attacker.Owner.Equipment.Free((byte)ConquerItem.Armor))
                        {
                            Interfaces.IConquerItem rightweapon2 = attacker.Owner.Equipment.TryGetItem((byte)ConquerItem.Armor);
                            if (rightweapon2.SocketOne == Game.Enums.Gem.SuperRainbowGem || rightweapon2.SocketTwo == Game.Enums.Gem.SuperRainbowGem)
                            {
                                if (ServerBase.Kernel.Rate(0.5))
                                {
                                    _String str = new _String(true);
                                    str.UID = attacker.UID;
                                    str.TextsCount = 1;
                                    str.Type = _String.Effect;
                                    str.Texts.Add("rainbow");

                                    attacker.Owner.SendScreen(str, true);
                                }
                            }
                        }

                        else if (!attacker.Owner.Equipment.Free((byte)ConquerItem.Boots))
                        {
                            Interfaces.IConquerItem rightweapon2 = attacker.Owner.Equipment.TryGetItem((byte)ConquerItem.Boots);
                            if (rightweapon2.SocketOne == Game.Enums.Gem.SuperRainbowGem || rightweapon2.SocketTwo == Game.Enums.Gem.SuperRainbowGem)
                            {
                                if (ServerBase.Kernel.Rate(0.5))
                                {
                                    _String str = new _String(true);
                                    str.UID = attacker.UID;
                                    str.TextsCount = 1;
                                    str.Type = _String.Effect;
                                    str.Texts.Add("rainbow");

                                    attacker.Owner.SendScreen(str, true);
                                }
                            }
                        }
                        else if (!attacker.Owner.Equipment.Free((byte)ConquerItem.Necklace))
                        {
                            Interfaces.IConquerItem rightweapon2 = attacker.Owner.Equipment.TryGetItem((byte)ConquerItem.Necklace);
                            if (rightweapon2.SocketOne == Game.Enums.Gem.SuperRainbowGem || rightweapon2.SocketTwo == Game.Enums.Gem.SuperRainbowGem)
                            {
                                if (ServerBase.Kernel.Rate(0.5))
                                {
                                    _String str = new _String(true);
                                    str.UID = attacker.UID;
                                    str.TextsCount = 1;
                                    str.Type = _String.Effect;
                                    str.Texts.Add("rainbow");

                                    attacker.Owner.SendScreen(str, true);
                                }
                            }
                        }
                        else if (!attacker.Owner.Equipment.Free((byte)ConquerItem.Ring))
                        {
                            Interfaces.IConquerItem rightweapon2 = attacker.Owner.Equipment.TryGetItem((byte)ConquerItem.Ring);
                            if (rightweapon2.SocketOne == Game.Enums.Gem.SuperRainbowGem || rightweapon2.SocketTwo == Game.Enums.Gem.SuperRainbowGem)
                            {
                                if (ServerBase.Kernel.Rate(0.5))
                                {
                                    _String str = new _String(true);
                                    str.UID = attacker.UID;
                                    str.TextsCount = 1;
                                    str.Type = _String.Effect;
                                    str.Texts.Add("rainbow");

                                    attacker.Owner.SendScreen(str, true);
                                }
                            }
                        }
                        #endregion



                    }
                    #endregion
                    #region Dash
                    if (SpellID == 1051)
                    {
                        if (ServerBase.Kernel.GetDistance(attack.X, attack.Y, attacker.X, attacker.Y) > 4)
                        {
                            attacker.Owner.Disconnect();
                            return;
                        }
                        attacker.X = attack.X; attacker.Y = attack.Y;
                        ushort x = attacker.X, y = attacker.Y;
                        Game.Map.UpdateCoordonatesForAngle(ref x, ref y, (Enums.ConquerAngle)Target);
                        foreach (Interfaces.IMapObject obj in attacker.Owner.Screen.Objects)
                        {
                            if (obj == null)
                                continue;
                            if (obj.X == x && obj.Y == y && (obj.MapObjType == MapObjectType.Monster || obj.MapObjType == MapObjectType.Player))
                            {
                                Entity entity = obj as Entity;
                                if (!entity.Dead)
                                {
                                    Target = obj.UID;
                                    break;
                                }
                            }
                        }
                    }
                    #endregion
                    #region CounterKill
                    if (attack.AttackType == Attack.CounterKillSwitch)
                    {
                        if (attacked != null)
                            if (attacked.ContainsFlag(Update.Flags.Fly))
                            { attacker.AttackPacket = null; return; }
                        if (attacker != null)
                            if (attacker.ContainsFlag(Update.Flags.Fly))
                            { attacker.AttackPacket = null; return; }
                        if (attacker.Owner.Spells.ContainsKey(6003))
                        {
                            if (!attacker.CounterKillSwitch)
                            {
                                if (Time32.Now >= attacker.CounterKillStamp.AddSeconds(15))
                                {
                                    attacker.CounterKillStamp = Time32.Now;
                                    attacker.CounterKillSwitch = true;
                                    Attack m_attack = new Attack(true);
                                    m_attack.Attacked = attacker.UID;
                                    m_attack.Attacker = attacker.UID;
                                    m_attack.AttackType = Attack.CounterKillSwitch;
                                    m_attack.Damage = 1;
                                    m_attack.X = attacker.X;
                                    m_attack.Y = attacker.Y;
                                    m_attack.Send(attacker.Owner);
                                }
                            }
                            else
                            {
                                attacker.CounterKillSwitch = false;
                                Attack m_attack = new Attack(true);
                                m_attack.Attacked = attacker.UID;
                                m_attack.Attacker = attacker.UID;
                                m_attack.AttackType = Attack.CounterKillSwitch;
                                m_attack.Damage = 0;
                                m_attack.X = attacker.X;
                                m_attack.Y = attacker.Y;
                                m_attack.Send(attacker.Owner);
                            }

                            attacker.Owner.IncreaseSpellExperience(100, 6003);
                            attacker.AttackPacket = null;
                        }
                    }
                    #endregion
                    #region Melee
                    else if (attack.AttackType == Attack.Melee)
                    {
                        if (attacker.Owner.Screen.TryGetValue(attack.Attacked, out attacked))
                        {
                            CheckForExtraWeaponPowers(attacker.Owner, attacked);
                            if (!CanAttack(attacker, attacked, null, attack.AttackType == Attack.Melee))
                                return;
                            pass = false;
                            if (attacker.OnFatalStrike())
                            {
                                if (attacked.EntityFlag == EntityFlag.Monster)
                                {
                                    pass = true;
                                }
                            }
                            ushort range = attacker.AttackRange;
                            if (attacker.Transformed)
                                range = (ushort)attacker.TransformationAttackRange;
                            if (ServerBase.Kernel.GetDistance(attacker.X, attacker.Y, attacked.X, attacked.Y) <= range || pass)
                            {
                                uint damage = Game.Attacking.Calculate.Melee(attacker, attacked);
                                attack.Damage = damage;
                                if (attacker.OnFatalStrike())
                                {
                                    if (attacked.EntityFlag == EntityFlag.Monster)
                                    {
                                        bool can = false;
                                        if (!attacker.Owner.Equipment.Free((byte)ConquerItem.RightWeapon))
                                            if (attacker.Owner.Equipment.TryGetItem((byte)ConquerItem.RightWeapon).ID / 1000 == 601)
                                                can = true;
                                        if (!attacker.Owner.Equipment.Free((byte)ConquerItem.LeftWeapon))
                                            if (attacker.Owner.Equipment.TryGetItem((byte)ConquerItem.LeftWeapon).ID / 1000 == 601)
                                                can = true;
                                        if (!can)
                                            return;
                                        ushort x = attacked.X;
                                        ushort y = attacked.Y;
                                        Map.UpdateCoordonatesForAngle(ref x, ref y, ServerBase.Kernel.GetAngle(attacked.X, attacked.Y, attacker.X, attacker.Y));
                                        attacker.Shift(x, y);
                                        attack.X = x;
                                        attack.Y = y;
                                        attack.AttackType = Attack.FatalStrike;
                                    }
                                }
                                //over:
                                if (!attacker.Owner.Equipment.Free((byte)ConquerItem.RightWeapon))
                                {
                                    Interfaces.IConquerItem rightweapon = attacker.Owner.Equipment.TryGetItem((byte)ConquerItem.RightWeapon);
                                    ushort wep1subyte = (ushort)(rightweapon.ID / 1000), wep2subyte = 0;
                                    bool wep1bs = false, wep2bs = false;
                                    if (wep1subyte == 421)
                                    {
                                        wep1bs = true;
                                        wep1subyte--;
                                    }
                                    ushort wep1spellid = 0, wep2spellid = 0;
                                    if (Database.SpellTable.WeaponSpells.ContainsKey(wep1subyte))
                                        wep1spellid = Database.SpellTable.WeaponSpells[wep1subyte];
                                    Database.SpellInformation wep1spell = null, wep2spell = null;
                                    bool doWep1Spell = false, doWep2Spell = false;
                                    if (attacker.Owner.Spells.ContainsKey(wep1spellid) && Database.SpellTable.SpellInformations.ContainsKey(wep1spellid))
                                    {
                                        wep1spell = Database.SpellTable.SpellInformations[wep1spellid][attacker.Owner.Spells[wep1spellid].Level];
                                        doWep1Spell = ServerBase.Kernel.Rate(wep1spell.Percent);
                                        if (attacked.EntityFlag == EntityFlag.Player && wep1spellid == 10490)
                                            doWep1Spell = ServerBase.Kernel.Rate(5);
                                    }
                                    if (!doWep1Spell)
                                    {
                                        if (!attacker.Owner.Equipment.Free((byte)ConquerItem.LeftWeapon))
                                        {
                                            Interfaces.IConquerItem leftweapon = attacker.Owner.Equipment.TryGetItem((byte)ConquerItem.LeftWeapon);
                                            wep2subyte = (ushort)(leftweapon.ID / 1000);
                                            if (wep2subyte == 421)
                                            {
                                                wep2bs = true;
                                                wep2subyte--;
                                            }
                                            if (Database.SpellTable.WeaponSpells.ContainsKey(wep2subyte))
                                                wep2spellid = Database.SpellTable.WeaponSpells[wep2subyte];
                                            if (attacker.Owner.Spells.ContainsKey(wep2spellid) && Database.SpellTable.SpellInformations.ContainsKey(wep2spellid))
                                            {
                                                wep2spell = Database.SpellTable.SpellInformations[wep2spellid][attacker.Owner.Spells[wep2spellid].Level];
                                                doWep2Spell = ServerBase.Kernel.Rate(wep2spell.Percent);
                                                if (attacked.EntityFlag == EntityFlag.Player && wep2spellid == 10490)
                                                    doWep2Spell = ServerBase.Kernel.Rate(5);
                                            }
                                        }
                                    }

                                    if (!attacker.Transformed)
                                    {
                                        if (doWep1Spell)
                                        {
                                            attack.AttackType = Attack.Magic;
                                            attack.Decoded = true;
                                            attack.X = attacked.X;
                                            attack.Y = attacked.Y;
                                            attack.Attacked = attacked.UID;
                                            attack.Damage = wep1spell.ID;
                                            goto restart;
                                        }
                                        if (doWep2Spell)
                                        {
                                            attack.AttackType = Attack.Magic;
                                            attack.Decoded = true;
                                            attack.X = attacked.X;
                                            attack.Y = attacked.Y;
                                            attack.Attacked = attacked.UID;
                                            attack.Damage = wep2spell.ID;
                                            goto restart;
                                        }
                                        if (wep1bs)
                                            wep1subyte++;
                                        if (attacker.EntityFlag == EntityFlag.Player && attacked.EntityFlag != EntityFlag.Player)
                                            if (damage > attacked.Hitpoints)
                                            {
                                                attacker.Owner.IncreaseProficiencyExperience(Math.Min(damage, attacked.Hitpoints), wep1subyte);
                                                if (wep2subyte != 0)
                                                {
                                                    if (wep2bs)
                                                        wep2subyte++;
                                                    attacker.Owner.IncreaseProficiencyExperience(Math.Min(damage, attacked.Hitpoints), wep2subyte);
                                                }
                                            }
                                            else
                                            {
                                                attacker.Owner.IncreaseProficiencyExperience(damage, wep1subyte);
                                                if (wep2subyte != 0)
                                                {
                                                    if (wep2bs)
                                                        wep2subyte++;
                                                    attacker.Owner.IncreaseProficiencyExperience(damage, wep2subyte);
                                                }
                                            }
                                    }
                                }
                                else
                                {
                                    if (!attacker.Transformed)
                                    {
                                        if (attacker.EntityFlag == EntityFlag.Player && attacked.EntityFlag != EntityFlag.Player)
                                            if (damage > attacked.Hitpoints)
                                            {
                                                attacker.Owner.IncreaseProficiencyExperience(Math.Min(damage, attacked.Hitpoints), 0);
                                            }
                                            else
                                            {
                                                attacker.Owner.IncreaseProficiencyExperience(damage, 0);
                                            }
                                    }
                                }
                                ReceiveAttack(attacker, attacked, attack, damage, null);
                                attack.AttackType = Attack.Melee;
                            }
                            else
                            {
                                attacker.AttackPacket = null;
                            }
                        }
                        else if (attacker.Owner.Screen.TryGetSob(attack.Attacked, out attackedsob))
                        {
                            if (CanAttack(attacker, attackedsob, null))
                            {
                                ushort range = attacker.AttackRange;
                                if (attacker.Transformed)
                                    range = (ushort)attacker.TransformationAttackRange;
                                if (ServerBase.Kernel.GetDistance(attacker.X, attacker.Y, attackedsob.X, attackedsob.Y) <= range)
                                {
                                    uint damage = Game.Attacking.Calculate.Melee(attacker, attackedsob);
                                    if (!attacker.Owner.Equipment.Free((byte)ConquerItem.RightWeapon))
                                    {
                                        Interfaces.IConquerItem rightweapon = attacker.Owner.Equipment.TryGetItem((byte)ConquerItem.RightWeapon);
                                        ushort wep1subyte = (ushort)(rightweapon.ID / 1000), wep2subyte = 0;
                                        bool wep1bs = false, wep2bs = false;
                                        if (wep1subyte == 421)
                                        {
                                            wep1bs = true;
                                            wep1subyte--;
                                        }
                                        ushort wep1spellid = 0, wep2spellid = 0;
                                        if (Database.SpellTable.WeaponSpells.ContainsKey(wep1subyte))
                                            wep1spellid = Database.SpellTable.WeaponSpells[wep1subyte];
                                        Database.SpellInformation wep1spell = null, wep2spell = null;
                                        bool doWep1Spell = false, doWep2Spell = false;
                                        if (attacker.Owner.Spells.ContainsKey(wep1spellid) && Database.SpellTable.SpellInformations.ContainsKey(wep1spellid))
                                        {
                                            wep1spell = Database.SpellTable.SpellInformations[wep1spellid][attacker.Owner.Spells[wep1spellid].Level];
                                            doWep1Spell = ServerBase.Kernel.Rate(wep1spell.Percent);
                                        }
                                        if (!doWep1Spell)
                                        {
                                            if (!attacker.Owner.Equipment.Free((byte)ConquerItem.LeftWeapon))
                                            {
                                                Interfaces.IConquerItem leftweapon = attacker.Owner.Equipment.TryGetItem((byte)ConquerItem.LeftWeapon);
                                                wep2subyte = (ushort)(leftweapon.ID / 1000);
                                                if (wep2subyte == 421)
                                                {
                                                    wep2bs = true;
                                                    wep2subyte--;
                                                }
                                                if (Database.SpellTable.WeaponSpells.ContainsKey(wep2subyte))
                                                    wep2spellid = Database.SpellTable.WeaponSpells[wep2subyte];
                                                if (attacker.Owner.Spells.ContainsKey(wep2spellid) && Database.SpellTable.SpellInformations.ContainsKey(wep2spellid))
                                                {
                                                    wep2spell = Database.SpellTable.SpellInformations[wep2spellid][attacker.Owner.Spells[wep2spellid].Level];
                                                    doWep2Spell = ServerBase.Kernel.Rate(wep2spell.Percent);
                                                }
                                            }
                                        }

                                        if (!attacker.Transformed)
                                        {
                                            if (doWep1Spell)
                                            {
                                                attack.AttackType = Attack.Magic;
                                                attack.Decoded = true;
                                                attack.X = attackedsob.X;
                                                attack.Y = attackedsob.Y;
                                                attack.Attacked = attackedsob.UID;
                                                attack.Damage = wep1spell.ID;
                                                goto restart;
                                            }
                                            if (doWep2Spell)
                                            {
                                                attack.AttackType = Attack.Magic;
                                                attack.Decoded = true;
                                                attack.X = attackedsob.X;
                                                attack.Y = attackedsob.Y;
                                                attack.Attacked = attackedsob.UID;
                                                attack.Damage = wep2spell.ID;
                                                goto restart;
                                            }
                                            if (attacker.MapID == 1039)
                                            {
                                                if (wep1bs)
                                                    wep1subyte++;
                                                if (attacker.EntityFlag == EntityFlag.Player)
                                                    if (damage > attackedsob.Hitpoints)
                                                    {
                                                        attacker.Owner.IncreaseProficiencyExperience(Math.Min(damage, attackedsob.Hitpoints), wep1subyte);
                                                        if (wep2subyte != 0)
                                                        {
                                                            if (wep2bs)
                                                                wep2subyte++;
                                                            attacker.Owner.IncreaseProficiencyExperience(Math.Min(damage, attackedsob.Hitpoints), wep2subyte);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        attacker.Owner.IncreaseProficiencyExperience(damage, wep1subyte);
                                                        if (wep2subyte != 0)
                                                        {
                                                            if (wep2bs)
                                                                wep2subyte++;
                                                            attacker.Owner.IncreaseProficiencyExperience(damage, wep2subyte);
                                                        }
                                                    }
                                            }
                                        }
                                    }
                                    attack.Damage = damage;
                                    ReceiveAttack(attacker, attackedsob, attack, damage, null);
                                }
                                else
                                {
                                    attacker.AttackPacket = null;
                                }
                            }
                        }
                        else
                        {
                            attacker.AttackPacket = null;
                        }
                    }
                    #endregion
                    #region Ranged
                    else if (attack.AttackType == Attack.Ranged)
                    {
                        if (attacker.Owner.Screen.TryGetValue(attack.Attacked, out attacked))
                        {
                            CheckForExtraWeaponPowers(attacker.Owner, attacked);
                            if (attacker.Owner.Equipment.TryGetItem(ConquerItem.LeftWeapon) == null)
                                return;
                            if (!CanAttack(attacker, attacked, null, attack.AttackType == Attack.Melee))
                                return;
                            if (!attacker.Owner.Equipment.Free((byte)ConquerItem.LeftWeapon))
                            {
                                Interfaces.IConquerItem arrow = attacker.Owner.Equipment.TryGetItem(ConquerItem.LeftWeapon);
                                arrow.Durability -= 1;
                                ItemUsage usage = new ItemUsage(true) { UID = arrow.UID, dwParam = arrow.Durability, ID = ItemUsage.UpdateDurability };
                                usage.Send(attacker.Owner);
                                if (arrow.Durability <= 0 || arrow.Durability > 5000)
                                {
                                    Network.PacketHandler.ReloadArrows(attacker.Owner.Equipment.TryGetItem(ConquerItem.LeftWeapon), attacker.Owner);
                                }
                            }
                            if (ServerBase.Kernel.GetDistance(attacker.X, attacker.Y, attacked.X, attacked.Y) <= ServerBase.Constants.pScreenDistance)
                            {
                                uint damage = Game.Attacking.Calculate.Ranged(attacker, attacked);
                                attack.Damage = damage;
                                if (attacker.EntityFlag == EntityFlag.Player && attacked.EntityFlag != EntityFlag.Player)
                                    if (damage > attacked.Hitpoints)
                                    {
                                        attacker.Owner.IncreaseProficiencyExperience(Math.Min(damage, attacked.Hitpoints), 500);
                                    }
                                    else
                                    {
                                        attacker.Owner.IncreaseProficiencyExperience(damage, 500);
                                    }
                                ReceiveAttack(attacker, attacked, attack, damage, null);
                            }
                        }
                        else if (attacker.Owner.Screen.TryGetSob(attack.Attacked, out attackedsob))
                        {
                            if (CanAttack(attacker, attackedsob, null))
                            {
                                if (attacker.Owner.Equipment.TryGetItem(ConquerItem.LeftWeapon) == null)
                                    return;
                                if (attacker.MapID != 1039)
                                {
                                    if (!attacker.Owner.Equipment.Free((byte)ConquerItem.LeftWeapon))
                                    {
                                        Interfaces.IConquerItem arrow = attacker.Owner.Equipment.TryGetItem(ConquerItem.LeftWeapon);
                                        arrow.Durability -= 1;
                                        ItemUsage usage = new ItemUsage(true) { UID = arrow.UID, dwParam = arrow.Durability, ID = ItemUsage.UpdateDurability };
                                        usage.Send(attacker.Owner);
                                        if (arrow.Durability <= 0 || arrow.Durability > 5000)
                                        {
                                            Network.PacketHandler.ReloadArrows(attacker.Owner.Equipment.TryGetItem(ConquerItem.LeftWeapon), attacker.Owner);
                                        }
                                    }
                                }
                                if (ServerBase.Kernel.GetDistance(attacker.X, attacker.Y, attackedsob.X, attackedsob.Y) <= ServerBase.Constants.pScreenDistance)
                                {
                                    uint damage = Game.Attacking.Calculate.Ranged(attacker, attackedsob);
                                    attack.Damage = damage;
                                    ReceiveAttack(attacker, attackedsob, attack, damage, null);
                                    if (damage > attackedsob.Hitpoints)
                                    {
                                        attacker.Owner.IncreaseProficiencyExperience(Math.Min(damage, attackedsob.Hitpoints), 500);
                                    }
                                    else
                                    {
                                        attacker.Owner.IncreaseProficiencyExperience(damage, 500);
                                    }
                                }
                            }
                        }
                        else
                        {
                            attacker.AttackPacket = null;
                        }
                    }
                    #endregion
                    #region Magic
                    else if (attack.AttackType == Attack.Magic)
                    {
                        CheckForExtraWeaponPowers(attacker.Owner, attacked);
                        uint Experience = 100;
                        bool shuriken = false;
                        ushort spellID = SpellID;
                        if (SpellID >= 3090 && SpellID <= 3306)
                            spellID = 3090;
                        if (spellID == 6012)
                            shuriken = true;

                        if (attacker == null)
                            return;
                        if (attacker.Owner == null)
                        {
                            attacker.AttackPacket = null;
                            return;
                        }
                        if (attacker.Owner.Spells == null)
                        {
                            attacker.Owner.Spells = new SafeDictionary<ushort, Conquer_Online_Server.Interfaces.ISkill>(10000);
                            attacker.AttackPacket = null;
                            return;
                        }
                        if (attacker.Owner.Spells[spellID] == null && spellID != 6012)
                        {
                            attacker.AttackPacket = null;
                            return;
                        }

                        Database.SpellInformation spell = null;
                        if (shuriken)
                            spell = Database.SpellTable.SpellInformations[6010][0];
                        else
                        {
                            byte choselevel = 0;
                            if (spellID == SpellID)
                                choselevel = attacker.Owner.Spells[spellID].Level;
                            if (Database.SpellTable.SpellInformations[SpellID] != null && !Database.SpellTable.SpellInformations[SpellID].ContainsKey(choselevel))
                                choselevel = (byte)(Database.SpellTable.SpellInformations[SpellID].Count - 1);

                            spell = Database.SpellTable.SpellInformations[SpellID][choselevel];
                        }
                        if (spell == null)
                        {
                            attacker.AttackPacket = null;
                            return;
                        }
                        attacked = null;
                        attackedsob = null;
                        if (attacker.Owner.Screen.TryGetValue(Target, out attacked) || attacker.Owner.Screen.TryGetSob(Target, out attackedsob) || Target == attacker.UID || spell.Sort != 1)
                        {
                            if (Target == attacker.UID)
                                attacked = attacker;
                            if (attacked != null)
                            {
                                if (attacked.Dead && spell.Sort != Database.SpellSort.Revive && spell.ID != 10405)
                                {
                                    attacker.AttackPacket = null;
                                    return;
                                }
                            }
                            if (Target >= 400000 && Target <= 600000 || Target >= 800000)
                            {
                                if (attacked == null && attackedsob == null)
                                    return;
                            }
                            else if (Target != 0 && attackedsob == null)
                                return;
                            if (attacked != null)
                            {
                                if (attacked.EntityFlag == EntityFlag.Monster)
                                {
                                    if (spell.CanKill)
                                    {
                                        if (attacked.MonsterInfo.InSight == 0)
                                        {
                                            attacked.MonsterInfo.InSight = attacker.UID;
                                        }
                                    }
                                }
                            }
                            if (!attacker.Owner.Spells.ContainsKey(spellID))
                            {
                                if (spellID != 6012)
                                    return;
                            }
                            if (spell != null)
                            {
                                if (spell.OnlyWithThisWeaponSubtype != 0)
                                {
                                    uint firstwepsubtype, secondwepsubtype;
                                    if (!attacker.Owner.Equipment.Free(4))
                                    {
                                        firstwepsubtype = attacker.Owner.Equipment.Objects[3].ID / 1000;
                                        if (!attacker.Owner.Equipment.Free(5) && attacker.Owner.Equipment.Objects[4] != null)
                                        {
                                            secondwepsubtype = attacker.Owner.Equipment.Objects[4].ID / 1000;
                                            if (firstwepsubtype != spell.OnlyWithThisWeaponSubtype)
                                            {
                                                if (secondwepsubtype != spell.OnlyWithThisWeaponSubtype)
                                                {
                                                    attacker.AttackPacket = null;
                                                    return;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (firstwepsubtype != spell.OnlyWithThisWeaponSubtype)
                                            {
                                                attacker.AttackPacket = null;
                                                return;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        attacker.AttackPacket = null;
                                        return;
                                    }
                                }
                            }
                          //  Buffers buff = new Buffers();

                            switch (spellID)
                            {
                                #region Single magic damage spells
                                case 1000:
                                case 1001:
                                case 1002:
                                case 1150:
                                case 1160:
                                case 1180:
                                case 1320:
                               
                                    {
                                        if (CanUseSpell(spell, attacker.Owner))
                                        {
                                            if (attacked != null)
                                            {
                                                if (ServerBase.Kernel.GetDistance(attacker.X, attacker.Y, attacked.X, attacked.Y) <= spell.Distance)
                                                {
                                                    SpellUse suse = new SpellUse(true);
                                                    suse.Attacker = attacker.UID;
                                                    suse.SpellID = spell.ID;
                                                    suse.SpellLevel = spell.Level;
                                                    suse.X = X;
                                                    suse.Y = Y;

                                                    if (CanAttack(attacker, attacked, spell, attack.AttackType == Attack.Melee))
                                                    {
                                                        PrepareSpell(spell, attacker.Owner);
                                                        uint damage = Game.Attacking.Calculate.Magic(attacker, attacked, spell);

                                                        ReceiveAttack(attacker, attacked, attack, damage, spell);

                                                        suse.Targets.Add(attacked.UID, damage);

                                                        if (attacked.EntityFlag == EntityFlag.Player)
                                                            attacked.Owner.SendScreen(suse, true);
                                                        else
                                                            attacked.MonsterInfo.SendScreen(suse);
                                                    }
                                                }
                                                else
                                                {
                                                    attacker.AttackPacket = null;
                                                }
                                            }
                                            else
                                            {
                                                if (ServerBase.Kernel.GetDistance(attacker.X, attacker.Y, attackedsob.X, attackedsob.Y) <= spell.Distance)
                                                {
                                                    SpellUse suse = new SpellUse(true);
                                                    suse.Attacker = attacker.UID;
                                                    suse.SpellID = spell.ID;
                                                    suse.SpellLevel = spell.Level;
                                                    suse.X = X;
                                                    suse.Y = Y;

                                                    if (CanAttack(attacker, attackedsob, spell))
                                                    {
                                                        PrepareSpell(spell, attacker.Owner);
                                                        uint damage = Game.Attacking.Calculate.Magic(attacker, attackedsob, spell);

                                                        ReceiveAttack(attacker, attackedsob, attack, damage, spell);

                                                        suse.Targets.Add(attackedsob.UID, damage);

                                                        attacker.Owner.SendScreen(suse, true);
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            attacker.AttackPacket = null;
                                        }
                                        break;
                                    }
                                #endregion
                                #region Single heal/meditation spells
                                case 1190:
                                case 1195:
                                    {
                                        if (CanUseSpell(spell, attacker.Owner))
                                        {
                                            PrepareSpell(spell, attacker.Owner);

                                            SpellUse suse = new SpellUse(true);
                                            suse.Attacker = attacker.UID;
                                            suse.SpellID = spell.ID;
                                            suse.SpellLevel = spell.Level;
                                            suse.X = X;
                                            suse.Y = Y;

                                            uint damage = spell.Power;
                                            if (spell.ID == 1190)
                                            {
                                                Experience = damage = Math.Min(damage, attacker.MaxHitpoints - attacker.Hitpoints);
                                                attacker.Hitpoints += damage;
                                            }
                                            else
                                            {
                                                Experience = damage = Math.Min(damage, (uint)(attacker.MaxMana - attacker.Mana));
                                                attacker.Mana += (ushort)damage;
                                            }

                                            suse.Targets.Add(attacker.UID, spell.Power);

                                            attacker.Owner.SendScreen(suse, true);
                                        }
                                        break;
                                    }
                                #endregion
                                #region Multi heal spells
                                case 1005:
                                case 1055:
                                case 1170:
                                case 1175:
                                    {
                                        if (CanUseSpell(spell, attacker.Owner))
                                        {
                                            SpellUse suse = new SpellUse(true);
                                            suse.Attacker = attacker.UID;
                                            suse.SpellID = spell.ID;
                                            suse.SpellLevel = spell.Level;
                                            suse.X = X;
                                            suse.Y = Y;
                                            if (attackedsob != null)
                                            {
                                                if (attacker.MapID == 1038)
                                                    break;
                                                if (ServerBase.Kernel.GetDistance(attacker.X, attacker.Y, attackedsob.X, attackedsob.Y) <= spell.Distance)
                                                {
                                                    PrepareSpell(spell, attacker.Owner);

                                                    uint damage = spell.Power;
                                                    damage = Math.Min(damage, attackedsob.MaxHitpoints - attackedsob.Hitpoints);
                                                    attackedsob.Hitpoints += damage;
                                                    Experience += damage;
                                                    suse.Targets.Add(attackedsob.UID, damage);

                                                    attacker.Owner.SendScreen(suse, true);
                                                }
                                            }
                                            else
                                            {
                                                if (spell.Multi)
                                                {
                                                    if (attacker.Owner.Team != null)
                                                    {
                                                        PrepareSpell(spell, attacker.Owner);
                                                        foreach (Client.GameState teammate in attacker.Owner.Team.Teammates)
                                                        {
                                                            if (ServerBase.Kernel.GetDistance(attacker.X, attacker.Y, teammate.Entity.X, teammate.Entity.Y) <= spell.Distance)
                                                            {
                                                                uint damage = spell.Power;
                                                                damage = Math.Min(damage, teammate.Entity.MaxHitpoints - teammate.Entity.Hitpoints);
                                                                teammate.Entity.Hitpoints += damage;
                                                                Experience += damage;
                                                                suse.Targets.Add(teammate.Entity.UID, damage);

                                                                if (spell.NextSpellID != 0)
                                                                {
                                                                    attack.Damage = spell.NextSpellID;
                                                                    attacker.AttackPacket = attack;
                                                                }
                                                                else
                                                                {
                                                                    attacker.AttackPacket = null;
                                                                }
                                                            }
                                                        }
                                                        if (attacked.EntityFlag == EntityFlag.Player)
                                                            attacked.Owner.SendScreen(suse, true);
                                                        else
                                                            attacked.MonsterInfo.SendScreen(suse);
                                                    }
                                                    else
                                                    {
                                                        if (ServerBase.Kernel.GetDistance(attacker.X, attacker.Y, attacked.X, attacked.Y) <= spell.Distance)
                                                        {
                                                            PrepareSpell(spell, attacker.Owner);

                                                            uint damage = spell.Power;
                                                            damage = Math.Min(damage, attacked.MaxHitpoints - attacked.Hitpoints);
                                                            attacked.Hitpoints += damage;
                                                            Experience += damage;
                                                            suse.Targets.Add(attacked.UID, damage);

                                                            if (spell.NextSpellID != 0)
                                                            {
                                                                attack.Damage = spell.NextSpellID;
                                                                attacker.AttackPacket = attack;
                                                            }
                                                            else
                                                            {
                                                                attacker.AttackPacket = null;
                                                            }
                                                            if (attacked.EntityFlag == EntityFlag.Player)
                                                                attacked.Owner.SendScreen(suse, true);
                                                            else
                                                                attacked.MonsterInfo.SendScreen(suse);
                                                        }
                                                        else
                                                        {
                                                            attacker.AttackPacket = null;
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    if (ServerBase.Kernel.GetDistance(attacker.X, attacker.Y, attacked.X, attacked.Y) <= spell.Distance)
                                                    {
                                                        PrepareSpell(spell, attacker.Owner);

                                                        uint damage = spell.Power;
                                                        damage = Math.Min(damage, attacked.MaxHitpoints - attacked.Hitpoints);
                                                        attacked.Hitpoints += damage;
                                                        Experience += damage;
                                                        suse.Targets.Add(attacked.UID, damage);

                                                        if (spell.NextSpellID != 0)
                                                        {
                                                            attack.Damage = spell.NextSpellID;
                                                            attacker.AttackPacket = attack;
                                                        }
                                                        else
                                                        {
                                                            attacker.AttackPacket = null;
                                                        }
                                                        if (attacked.EntityFlag == EntityFlag.Player)
                                                            attacked.Owner.SendScreen(suse, true);
                                                        else
                                                            attacked.MonsterInfo.SendScreen(suse);
                                                    }
                                                    else
                                                    {
                                                        attacker.AttackPacket = null;
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            attacker.AttackPacket = null;
                                        }
                                        break;
                                    }
                                #endregion
                                #region Revive
                                case 1050:
                                case 1100:
                                    {
                                        if (attackedsob != null)
                                            return;
                                        if (ServerBase.Kernel.GetDistance(attacker.X, attacker.Y, attacked.X, attacked.Y) <= spell.Distance)
                                        {
                                            if (CanUseSpell(spell, attacker.Owner))
                                            {
                                                PrepareSpell(spell, attacker.Owner);

                                                SpellUse suse = new SpellUse(true);
                                                suse.Attacker = attacker.UID;
                                                suse.SpellID = spell.ID;
                                                suse.SpellLevel = spell.Level;
                                                suse.X = X;
                                                suse.Y = Y;

                                                suse.Targets.Add(attacked.UID, 0);

                                                attacked.Ressurect();

                                                attacked.Owner.SendScreen(suse, true);
                                            }
                                        }
                                        break;
                                    }
                                #endregion
                                #region Linear spells
                                case 1045:
                                case 11000:
                                case 11005:
                                case 1046:
                                case 1260:
                                    {
                                        if (CanUseSpell(spell, attacker.Owner))
                                        {
                                            PrepareSpell(spell, attacker.Owner);
                                            Game.Attacking.InLineAlgorithm ila = new Conquer_Online_Server.Game.Attacking.InLineAlgorithm(attacker.X,
                                        X, attacker.Y, Y, (byte)spell.Range, InLineAlgorithm.Algorithm.DDA);
                                            SpellUse suse = new SpellUse(true);
                                            suse.Attacker = attacker.UID;
                                            suse.SpellID = SpellID;
                                            suse.SpellLevel = attacker.Owner.Spells[SpellID].Level;
                                            suse.X = X;
                                            suse.Y = Y;
                                            foreach (Interfaces.IMapObject _obj in attacker.Owner.Screen.Objects)
                                            {
                                                if (_obj == null)
                                                    continue;
                                                if (_obj.MapObjType == MapObjectType.Monster || _obj.MapObjType == MapObjectType.Player)
                                                {
                                                    attacked = _obj as Entity;
                                                    if (ila.InLine(attacked.X, attacked.Y))
                                                    {
                                                        if (!CanAttack(attacker, attacked, spell, attack.AttackType == Attack.Melee))
                                                            continue;

                                                        uint damage = Game.Attacking.Calculate.Melee(attacker, attacked, spell);

                                                        attack.Damage = damage;

                                                        ReceiveAttack(attacker, attacked, attack, damage, spell);

                                                        suse.Targets.Add(attacked.UID, damage);
                                                    }
                                                }
                                                else if (_obj.MapObjType == MapObjectType.SobNpc)
                                                {
                                                    attackedsob = _obj as SobNpcSpawn;

                                                    if (ila.InLine(attackedsob.X, attackedsob.Y))
                                                    {
                                                        if (!CanAttack(attacker, attackedsob, spell))
                                                            continue;

                                                        uint damage = Game.Attacking.Calculate.Melee(attacker, attackedsob);
                                                        damage = (uint)(damage * spell.PowerPercent);
                                                        attack.Damage = damage;

                                                        ReceiveAttack(attacker, attackedsob, attack, damage, spell);

                                                        suse.Targets.Add(attackedsob.UID, damage);
                                                    }
                                                }
                                            }
                                            attacker.Owner.SendScreen(suse, true);
                                        }
                                        break;
                                    }
                                #endregion

                                #region XPSpells inofensive
                                case 1015:
                                case 1020:
                                case 1025:
                                case 1110:
                                case 6011:
                              
                                case 10390:
                                    {
                                        if (CanUseSpell(spell, attacker.Owner))
                                        {
                                            PrepareSpell(spell, attacker.Owner);

                                            SpellUse suse = new SpellUse(true);
                                            suse.Attacker = attacker.UID;
                                            suse.SpellID = spell.ID;
                                            suse.SpellLevel = spell.Level;
                                            suse.X = X;
                                            suse.Y = Y;

                                            suse.Targets.Add(attacked.UID, 0);

                                            if (spell.ID == 6011)
                                            {
                                                attacked.FatalStrikeStamp = Time32.Now;
                                                attacked.FatalStrikeTime = 60;
                                                attacked.AddFlag(Update.Flags.FatalStrike);
                                            }
                                           
                                            else
                                            {
                                                if (spell.ID == 1110 || spell.ID == 1025 || spell.ID == 10390)
                                                {
                                                    if (!attacked.OnKOSpell())
                                                        attacked.KOCount = 0;

                                                    attacked.KOSpell = spell.ID;
                                                    if (spell.ID == 1110)
                                                    {
                                                        attacked.CycloneStamp = Time32.Now;
                                                        attacked.CycloneTime = 20;
                                                        attacked.AddFlag(Update.Flags.Cyclone);
                                                    }
                                                    else if (spell.ID == 10390)
                                                    {
                                                        attacked.OblivionStamp = Time32.Now;
                                                        attacked.OblivionTime = 20;
                                                        attacked.AddFlag2(Update.Flags2.Oblivion);
                                                    }
                                                    else
                                                    {
                                                        attacked.SupermanStamp = Time32.Now;
                                                        attacked.SupermanTime = 20;
                                                        attacked.AddFlag(Update.Flags.Superman);
                                                    }
                                                }
                                                else
                                                {
                                                    if (spell.ID == 1020)
                                                    {
                                                        attacked.ShieldTime = 0;
                                                        attacked.ShieldStamp = Time32.Now;
                                                        attacked.MagicShieldStamp = Time32.Now;
                                                        attacked.MagicShieldTime = 0;

                                                        attacked.AddFlag(Update.Flags.MagicShield);
                                                        attacked.ShieldStamp = Time32.Now;
                                                        attacked.ShieldIncrease = spell.PowerPercent;
                                                        attacked.ShieldTime = (byte)spell.Duration;
                                                    }
                                                    else
                                                    {
                                                        attacked.AccuracyStamp = Time32.Now;
                                                        attacked.StarOfAccuracyStamp = Time32.Now;
                                                        attacked.StarOfAccuracyTime = 0;
                                                        attacked.AccuracyTime = 0;

                                                        attacked.AddFlag(Update.Flags.StarOfAccuracy);
                                                        attacked.AccuracyStamp = Time32.Now;
                                                        attacked.AccuracyTime = (byte)spell.Duration;
                                                    }
                                                }
                                            }
                                            attacked.Owner.SendScreen(suse, true);
                                        }
                                        break;
                                    }
                                #endregion
                                #region Circle spells
                                case 1010:
                                case 1115:
                                case 1120:
                                case 1125:
                                case 3090:
                                case 5001:
                                case 8030:
                                    {
                                        if (CanUseSpell(spell, attacker.Owner))
                                        {
                                            PrepareSpell(spell, attacker.Owner);

                                            SpellUse suse = new SpellUse(true);
                                            suse.Attacker = attacker.UID;
                                            suse.SpellID = spell.ID;
                                            suse.SpellLevel = spell.Level;
                                            suse.X = X;
                                            suse.Y = Y;

                                            if (ServerBase.Kernel.GetDistance(attacker.X, attacker.Y, X, Y) <= spell.Range)
                                            {
                                                foreach (Interfaces.IMapObject _obj in attacker.Owner.Screen.Objects)
                                                {
                                                    if (_obj == null)
                                                        continue;
                                                    if (_obj.MapObjType == MapObjectType.Monster || _obj.MapObjType == MapObjectType.Player)
                                                    {
                                                        attacked = _obj as Entity;
                                                        if (ServerBase.Kernel.GetDistance(attacker.X, attacker.Y, attacked.X, attacked.Y) <= spell.Range)
                                                        {
                                                            if (CanAttack(attacker, attacked, spell, attack.AttackType == Attack.Melee))
                                                            {
                                                                uint damage = Game.Attacking.Calculate.Melee(attacker, attacked);
                                                                if (spell.Power > 0)
                                                                    damage = Game.Attacking.Calculate.Magic(attacker, attacked, spell);
                                                                if (spell.ID == 8030)
                                                                    damage = Game.Attacking.Calculate.Ranged(attacker, attacked);

                                                                ReceiveAttack(attacker, attacked, attack, damage, spell);

                                                                suse.Targets.Add(attacked.UID, damage);
                                                            }
                                                        }
                                                    }
                                                    else if (_obj.MapObjType == MapObjectType.SobNpc)
                                                    {
                                                        attackedsob = _obj as SobNpcSpawn;
                                                        if (ServerBase.Kernel.GetDistance(attacker.X, attacker.Y, attackedsob.X, attackedsob.Y) <= spell.Range)
                                                        {
                                                            if (CanAttack(attacker, attackedsob, spell))
                                                            {
                                                                uint damage = Game.Attacking.Calculate.Melee(attacker, attackedsob);
                                                                if (spell.Power > 0)
                                                                    damage = Game.Attacking.Calculate.Magic(attacker, attackedsob, spell);
                                                                if (spell.ID == 8030)
                                                                    damage = Game.Attacking.Calculate.Ranged(attacker, attackedsob);

                                                                ReceiveAttack(attacker, attackedsob, attack, damage, spell);

                                                                suse.Targets.Add(attackedsob.UID, damage);
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                attacker.AttackPacket = null;
                                            }
                                            attacker.Owner.SendScreen(suse, true);
                                        }
                                        break;
                                    }
                                #endregion
                                #region Buffers
                                case 1075:
                                case 1085:
                                case 1090:
                                case 1095:
                                case 3080:
                            
                                    {
                                        if (attackedsob != null)
                                        {
                                            if (CanUseSpell(spell, attacker.Owner))
                                            {
                                                PrepareSpell(spell, attacker.Owner);

                                                SpellUse suse = new SpellUse(true);
                                                suse.Attacker = attacker.UID;
                                                suse.SpellID = spell.ID;
                                                suse.SpellLevel = spell.Level;
                                                suse.X = X;
                                                suse.Y = Y;

                                                suse.Targets.Add(attackedsob.UID, 0);

                                                attacker.Owner.SendScreen(suse, true);
                                            }
                                        }
                                        else
                                        {
                                            if (ServerBase.Kernel.GetDistance(attacker.X, attacker.Y, attacked.X, attacked.Y) <= spell.Distance)
                                            {
                                                if (CanUseSpell(spell, attacker.Owner))
                                                {
                                                    PrepareSpell(spell, attacker.Owner);

                                                    SpellUse suse = new SpellUse(true);
                                                    suse.Attacker = attacker.UID;
                                                    suse.SpellID = spell.ID;
                                                    suse.SpellLevel = spell.Level;
                                                    suse.X = X;
                                                    suse.Y = Y;

                                                    suse.Targets.Add(attacked.UID, 0);

                                                    if (spell.ID == 1075 || spell.ID == 1085)
                                                    {
                                                        if (spell.ID == 1075)
                                                        {
                                                            attacked.AddFlag(Update.Flags.Invisibility);
                                                            attacked.InvisibilityStamp = Time32.Now;
                                                            attacked.InvisibilityTime = (byte)spell.Duration;
                                                            if (attacked.EntityFlag == EntityFlag.Player)
                                                                attacked.Owner.Send(ServerBase.Constants.Invisibility(spell.Duration));
                                                        }
                                                        else
                                                        {
                                                            attacked.AccuracyStamp = Time32.Now;
                                                            attacked.StarOfAccuracyStamp = Time32.Now;
                                                            attacked.StarOfAccuracyTime = 0;
                                                            attacked.AccuracyTime = 0;

                                                            attacked.AddFlag(Update.Flags.StarOfAccuracy);
                                                            attacked.StarOfAccuracyStamp = Time32.Now;
                                                            attacked.StarOfAccuracyTime = (byte)spell.Duration;
                                                            if (attacked.EntityFlag == EntityFlag.Player)
                                                                attacked.Owner.Send(ServerBase.Constants.Accuracy(spell.Duration));
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (spell.ID == 1090)
                                                        {
                                                            attacked.ShieldTime = 0;
                                                            attacked.ShieldStamp = Time32.Now;
                                                            attacked.MagicShieldStamp = Time32.Now;
                                                            attacked.MagicShieldTime = 0;

                                                            attacked.AddFlag(Update.Flags.MagicShield);
                                                            attacked.MagicShieldStamp = Time32.Now;
                                                            attacked.MagicShieldIncrease = spell.PowerPercent;
                                                            attacked.MagicShieldTime = (byte)spell.Duration;
                                                            if (attacked.EntityFlag == EntityFlag.Player)
                                                                attacked.Owner.Send(ServerBase.Constants.Shield(spell.PowerPercent, spell.Duration));
                                                        }
                                                        else if (spell.ID == 1095)
                                                        {
                                                            attacked.AddFlag(Update.Flags.Stigma);
                                                            attacked.StigmaStamp = Time32.Now;
                                                            attacked.StigmaIncrease = spell.PowerPercent;
                                                            attacked.StigmaTime = (byte)spell.Duration;
                                                            if (attacked.EntityFlag == EntityFlag.Player)
                                                                attacked.Owner.Send(ServerBase.Constants.Stigma(spell.PowerPercent, spell.Duration));
                                                        }
                                                        else
                                                        {
                                                            attacked.AddFlag(Update.Flags.Dodge);
                                                            attacked.DodgeStamp = Time32.Now;
                                                            attacked.DodgeIncrease = spell.PowerPercent;
                                                            attacked.DodgeTime = (byte)spell.Duration;
                                                            if (attacked.EntityFlag == EntityFlag.Player)
                                                                attacked.Owner.Send(ServerBase.Constants.Dodge(spell.PowerPercent, spell.Duration));
                                                        }

                                                      
                                                    }


                                                    if (attacked.EntityFlag == EntityFlag.Player)
                                                        attacked.Owner.SendScreen(suse, true);
                                                    else
                                                        attacked.MonsterInfo.SendScreen(suse);
                                                }
                                            }
                                        }
                                        break;
                                    }
                                #endregion
                                #region Percent
                                case 3050:
                                    {
                                        if (CanUseSpell(spell, attacker.Owner))
                                        {
                                            if (attackedsob != null)
                                            {
                                                if (ServerBase.Kernel.GetDistance(attacker.X, attacker.Y, attackedsob.X, attackedsob.Y) <= spell.Distance)
                                                {
                                                    SpellUse suse = new SpellUse(true);
                                                    suse.Attacker = attacker.UID;
                                                    suse.SpellID = spell.ID;
                                                    suse.SpellLevel = spell.Level;
                                                    suse.X = X;
                                                    suse.Y = Y;

                                                    if (CanAttack(attacker, attackedsob, spell))
                                                    {
                                                        PrepareSpell(spell, attacker.Owner);
                                                        uint damage = Game.Attacking.Calculate.Percent(attackedsob, spell.PowerPercent);

                                                        attackedsob.Hitpoints -= damage;

                                                        suse.Targets.Add(attackedsob.UID, damage);

                                                        attacker.Owner.SendScreen(suse, true);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (ServerBase.Kernel.GetDistance(attacker.X, attacker.Y, attacked.X, attacked.Y) <= spell.Distance)
                                                {
                                                    SpellUse suse = new SpellUse(true);
                                                    suse.Attacker = attacker.UID;
                                                    suse.SpellID = spell.ID;
                                                    suse.SpellLevel = spell.Level;
                                                    suse.X = X;
                                                    suse.Y = Y;

                                                    if (CanAttack(attacker, attacked, spell, attack.AttackType == Attack.Melee))
                                                    {
                                                        PrepareSpell(spell, attacker.Owner);
                                                        uint damage = Game.Attacking.Calculate.Percent(attacked, spell.PowerPercent);

                                                        if (attacker.Owner.QualifierGroup != null)
                                                            attacker.Owner.QualifierGroup.UpdateDamage(attacker.Owner, damage);

                                                        attacked.Hitpoints -= damage;

                                                        suse.Targets.Add(attacked.UID, damage);

                                                        if (attacked.EntityFlag == EntityFlag.Player)
                                                            attacked.Owner.SendScreen(suse, true);
                                                        else
                                                            attacked.MonsterInfo.SendScreen(suse);
                                                    }
                                                }
                                            }
                                        }
                                        break;
                                    }
                                #endregion
                                #region ExtraXP
                                case 1040:
                                    {
                                        if (CanUseSpell(spell, attacker.Owner))
                                        {
                                            SpellUse suse = new SpellUse(true);
                                            suse.Attacker = attacker.UID;
                                            suse.SpellID = spell.ID;
                                            suse.SpellLevel = spell.Level;
                                            suse.X = X;
                                            suse.Y = Y;
                                            if (attacker.Owner.Team != null)
                                            {
                                                PrepareSpell(spell, attacker.Owner);
                                                foreach (Client.GameState teammate in attacker.Owner.Team.Teammates)
                                                {
                                                    if (teammate.Entity.UID != attacker.UID)
                                                    {
                                                        if (ServerBase.Kernel.GetDistance(attacker.X, attacker.Y, teammate.Entity.X, teammate.Entity.Y) <= spell.Distance)
                                                        {
                                                            teammate.XPCount += 20;
                                                            Update update = new Update(true);
                                                            update.UID = teammate.Entity.UID;
                                                            update.Append(Update.XPCircle, teammate.XPCount);
                                                            update.Send(teammate);
                                                            suse.Targets.Add(teammate.Entity.UID, 20);

                                                            if (spell.NextSpellID != 0)
                                                            {
                                                                attack.Damage = spell.NextSpellID;
                                                                attacker.AttackPacket = attack;
                                                            }
                                                            else
                                                            {
                                                                attacker.AttackPacket = null;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            if (attacked.EntityFlag == EntityFlag.Player)
                                                attacked.Owner.SendScreen(suse, true);
                                            else
                                                attacked.MonsterInfo.SendScreen(suse);
                                        }
                                        break;
                                    }
                                #endregion
                                #region Pure Skills
                                //SoulShackle
                                case 10405:
                                    {
                                        if (CanUseSpell(spell, attacker.Owner))
                                        {
                                            PrepareSpell(spell, attacker.Owner);

                                            if (attacked == null) return;
                                            if (attacked.EntityFlag == EntityFlag.Monster)
                                                return;
                                            if (attacked.UID == attacker.UID)
                                                return;
                                            if (!attacked.Dead)
                                                return;

                                            SpellUse suse = new SpellUse(true);
                                            suse.Attacker = attacker.UID;
                                            suse.SpellID = SpellID;
                                            suse.SpellLevel = attacker.Owner.Spells[SpellID].Level;
                                            suse.X = X;
                                            suse.Y = Y;

                                            if (attacked.BattlePower > attacker.BattlePower)
                                            {
                                                int diff = attacked.BattlePower - attacker.BattlePower;
                                                if (ServerBase.Kernel.Rate(100 - diff * 5))
                                                {
                                                    attacked.ShackleStamp = Time32.Now;
                                                    attacked.ShackleTime = (short)spell.Duration;
                                                    suse.Targets.Add(attacked.UID, 1);
                                                    attacked.AddFlag(Update.Flags.SoulShackle);
                                                    attacked.Owner.SendScreen(suse, true);
                                                }
                                            }
                                            else
                                            {
                                                attacked.ShackleStamp = Time32.Now;
                                                attacked.ShackleTime = (short)spell.Duration;
                                                suse.Targets.Add(attacked.UID, 1);
                                                attacked.AddFlag(Update.Flags.SoulShackle);
                                                attacked.Owner.SendScreen(suse, true);
                                            }
                                        }
                                        break;
                                    }
                                //AzureShield
                                case 30000:
                                    {
                                        if (attacked == null)
                                            return;
                                        if (ServerBase.Kernel.GetDistance(attacker.X, attacker.Y, attacked.X, attacked.Y) <= spell.Distance)
                                        {
                                            if (CanUseSpell(spell, attacker.Owner))
                                            {
                                                PrepareSpell(spell, attacker.Owner);

                                                SpellUse suse = new SpellUse(true);
                                                suse.Attacker = attacker.UID;
                                                suse.SpellID = spell.ID;
                                                suse.SpellLevel = spell.Level;
                                                suse.X = X;
                                                suse.Y = Y;

                                                suse.Targets.Add(attacked.UID, 0);
                                                attacked.Owner.SendScreen(suse, true);
                                                attacked.AzureStamp = Time32.Now;
                                                attacked.AzureTime = (short)spell.Duration;
                                                attacked.AzureDamage = spell.Power;
                                                attacked.AddFlag(Update.Flags.AzureShield);
                                            }
                                        }
                                        break;
                                    }
                                //HeavenBlade
                                case 10310:
                                    {
                                        if (attacked == null)
                                            return;
                                        spell.UseStamina = 100;
                                        if (CanUseSpell(spell, attacker.Owner))
                                        {
                                            if (attacked != null)
                                            {
                                                if (ServerBase.Kernel.GetDistance(attacker.X, attacker.Y, attacked.X, attacked.Y) <= spell.Distance)
                                                {
                                                    SpellUse suse = new SpellUse(true);
                                                    suse.Attacker = attacker.UID;
                                                    suse.SpellID = spell.ID;
                                                    suse.SpellLevel = spell.Level;
                                                    suse.X = X;
                                                    suse.Y = Y;

                                                    if (CanAttack(attacker, attacked, spell, attack.AttackType == Attack.Melee))
                                                    {
                                                        PrepareSpell(spell, attacker.Owner);
                                                        uint damage = Game.Attacking.Calculate.Magic(attacker, attacked, spell);
                                                        if (ServerBase.Kernel.Rate(spell.Percent))
                                                        {
                                                            damage = ReceiveAttack(suse, attacker, attacked, attack, damage, spell);

                                                            suse.Targets.Add(attacked.UID, damage);
                                                        }
                                                        else
                                                        {
                                                            damage = 0;
                                                            suse.Targets.Add(attacked.UID, damage);
                                                        }

                                                        if (attacked.EntityFlag == EntityFlag.Player)
                                                            attacked.Owner.SendScreen(suse, true);
                                                        else
                                                            attacked.MonsterInfo.SendScreen(suse);
                                                    }
                                                }
                                                else
                                                {
                                                    attacker.AttackPacket = null;
                                                }
                                            }
                                            else
                                            {
                                                if (ServerBase.Kernel.GetDistance(attacker.X, attacker.Y, attackedsob.X, attackedsob.Y) <= spell.Distance)
                                                {
                                                    SpellUse suse = new SpellUse(true);
                                                    suse.Attacker = attacker.UID;
                                                    suse.SpellID = spell.ID;
                                                    suse.SpellLevel = spell.Level;
                                                    suse.X = X;
                                                    suse.Y = Y;

                                                    if (CanAttack(attacker, attackedsob, spell))
                                                    {
                                                        PrepareSpell(spell, attacker.Owner);
                                                        uint damage = Game.Attacking.Calculate.Magic(attacker, attackedsob, spell);
                                                        if (ServerBase.Kernel.Rate(spell.Percent))
                                                        {
                                                            ReceiveAttack(attacker, attackedsob, attack, damage, spell);

                                                            suse.Targets.Add(attackedsob.UID, damage);
                                                        }
                                                        else
                                                        {
                                                            damage = 0;
                                                            suse.Targets.Add(attackedsob.UID, damage);
                                                        }

                                                        attacker.Owner.SendScreen(suse, true);
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            attacker.AttackPacket = null;
                                        }
                                        break;
                                    }
                                //StarArrow
                                case 10313:
                                    {
                                        spell.UseStamina = 50;
                                        if (CanUseSpell(spell, attacker.Owner))
                                        {
                                            if (attacked != null)
                                            {
                                                if (ServerBase.Kernel.GetDistance(attacker.X, attacker.Y, attacked.X, attacked.Y) <= spell.Distance)
                                                {
                                                    SpellUse suse = new SpellUse(true);
                                                    suse.Attacker = attacker.UID;
                                                    suse.SpellID = spell.ID;
                                                    suse.SpellLevel = spell.Level;
                                                    suse.X = attacked.X;
                                                    suse.Y = attacked.Y;

                                                    if (CanAttack(attacker, attacked, spell, attack.AttackType == Attack.Melee))
                                                    {
                                                        PrepareSpell(spell, attacker.Owner);
                                                        uint damage = Game.Attacking.Calculate.Ranged(attacker, attacked, spell);

                                                        damage = ReceiveAttack(suse, attacker, attacked, attack, damage, spell);

                                                        suse.Targets.Add(attacked.UID, damage);

                                                        if (attacked.EntityFlag == EntityFlag.Player)
                                                            attacked.Owner.SendScreen(suse, true);
                                                        else
                                                            attacked.MonsterInfo.SendScreen(suse);
                                                    }
                                                }
                                                else
                                                {
                                                    attacker.AttackPacket = null;
                                                }
                                            }
                                            else
                                            {
                                                if (ServerBase.Kernel.GetDistance(attacker.X, attacker.Y, attackedsob.X, attackedsob.Y) <= spell.Distance)
                                                {
                                                    SpellUse suse = new SpellUse(true);
                                                    suse.Attacker = attacker.UID;
                                                    suse.SpellID = spell.ID;
                                                    suse.SpellLevel = spell.Level;
                                                    suse.X = X;
                                                    suse.Y = Y;

                                                    if (CanAttack(attacker, attackedsob, spell))
                                                    {
                                                        PrepareSpell(spell, attacker.Owner);
                                                        uint damage = Game.Attacking.Calculate.Ranged(attacker, attackedsob);

                                                        ReceiveAttack(attacker, attackedsob, attack, damage, spell);

                                                        suse.Targets.Add(attackedsob.UID, damage);

                                                        attacker.Owner.SendScreen(suse, true);
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            attacker.AttackPacket = null;
                                        }
                                        break;
                                    }
                                //DragonWhirl
                                #region DragonWhirl
                                case 10315:
                                    {
                                        if (attacker.Stamina >= 30)
                                        {
                                            attacker.Stamina -= 30;
                                            if (CanUseSpell(spell, attacker.Owner))
                                            {
                                                PrepareSpell(spell, attacker.Owner);
                                                SpellUse suse = new SpellUse(true);
                                                suse.Attacker = attacker.UID;
                                                suse.SpellID = spell.ID;
                                                suse.SpellLevel = spell.Level;
                                                suse.X = X;
                                                suse.Y = Y;

                                                attack.X = X;
                                                attack.Y = Y;
                                                attack.Attacker = attacker.UID;
                                                attack.AttackType = 53;
                                                attack.X = X;
                                                attack.Y = Y;
                                                Writer.WriteUInt16(spell.ID, 24, attack.ToArray());
                                                Writer.WriteByte(spell.Level, 26, attack.ToArray());
                                                attacker.Owner.SendScreen(attack, true);
                                                attacker.X = X;
                                                attacker.Y = Y;

                                                if (ServerBase.Kernel.GetDistance(attacker.X, attacker.Y, X, Y) <= spell.Range)
                                                {
                                                    foreach (Interfaces.IMapObject _obj in attacker.Owner.Screen.Objects)
                                                    {
                                                        if (_obj == null)
                                                            continue;
                                                        if (_obj.MapObjType == MapObjectType.Monster || _obj.MapObjType == MapObjectType.Player)
                                                        {
                                                            attacked = _obj as Entity;
                                                            if (ServerBase.Kernel.GetDistance(attacker.X, attacker.Y, attacked.X, attacked.Y) <= spell.Range)
                                                            {
                                                                if (CanAttack(attacker, attacked, spell, attack.AttackType == Attack.Melee))
                                                                {
                                                                    uint damage = Game.Attacking.Calculate.Melee(attacker, attacked);
                                                                    uint DamageAdd = (damage * 150) / 100;
                                                                    if (spell.Power > 0)
                                                                        damage = (uint)(DamageAdd + Game.Attacking.Calculate.Magic(attacker, attacked, spell));

                                                                    ReceiveAttack(suse, attacker, attacked, attack, damage, spell);
                                                                    suse.Targets.Add(attacked.UID, damage);

                                                                }
                                                            }
                                                        }
                                                        else if (_obj.MapObjType == MapObjectType.SobNpc)
                                                        {
                                                            attackedsob = _obj as SobNpcSpawn;
                                                            if (ServerBase.Kernel.GetDistance(attacker.X, attacker.Y, attackedsob.X, attackedsob.Y) <= spell.Distance)
                                                            {
                                                                if (CanAttack(attacker, attackedsob, spell))
                                                                {
                                                                    uint damage = Game.Attacking.Calculate.Melee(attacker, attackedsob);
                                                                    uint DamageAdd = (damage * 150) / 100;
                                                                    if (spell.Power > 0)
                                                                        damage = (uint)(DamageAdd + Game.Attacking.Calculate.Magic(attacker, attacked, spell));

                                                                    ReceiveAttack(attacker, attackedsob, attack, damage, spell);

                                                                    suse.Targets.Add(attackedsob.UID, damage);

                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    attacker.AttackPacket = null;
                                                }
                                                attacker.Owner.SendScreen(suse, true);
                                            }
                                        }
                                        break;
                                    }
                                #endregion
                                #endregion
                            

                                #region WeaponSpells
                                #region Circle
                                case 5010:
                                case 7020:
                                    {
                                        if (CanUseSpell(spell, attacker.Owner))
                                        {
                                            PrepareSpell(spell, attacker.Owner);

                                            SpellUse suse = new SpellUse(true);
                                            suse.Attacker = attacker.UID;
                                            suse.SpellID = spell.ID;
                                            suse.SpellLevel = spell.Level;
                                            if (suse.SpellID != 10415)
                                            {
                                                suse.X = X;
                                                suse.Y = Y;
                                            }
                                            else
                                            {
                                                suse.X = 6;
                                            }
                                            if (ServerBase.Kernel.GetDistance(attacker.X, attacker.Y, X, Y) <= attacker.AttackRange + 1)
                                            {
                                                foreach (Interfaces.IMapObject _obj in attacker.Owner.Screen.Objects)
                                                {
                                                    if (_obj == null)
                                                        continue;
                                                    if (_obj.MapObjType == MapObjectType.Monster || _obj.MapObjType == MapObjectType.Player)
                                                    {
                                                        attacked = _obj as Entity;
                                                        if (ServerBase.Kernel.GetDistance(attacker.X, attacker.Y, attacked.X, attacked.Y) <= spell.Range)
                                                        {
                                                            if (CanAttack(attacker, attacked, spell, attack.AttackType == Attack.Melee))
                                                            {
                                                                PrepareSpell(spell, attacker.Owner);
                                                                uint damage = Game.Attacking.Calculate.Melee(attacker, attacked, spell);

                                                                ReceiveAttack(attacker, attacked, attack, damage, spell);

                                                                suse.Targets.Add(attacked.UID, damage);
                                                            }
                                                        }
                                                    }
                                                    else if (_obj.MapObjType == MapObjectType.SobNpc)
                                                    {
                                                        attackedsob = _obj as SobNpcSpawn;
                                                        if (ServerBase.Kernel.GetDistance(attacker.X, attacker.Y, attackedsob.X, attackedsob.Y) <= spell.Range)
                                                        {
                                                            if (CanAttack(attacker, attackedsob, spell))
                                                            {
                                                                PrepareSpell(spell, attacker.Owner);
                                                                uint damage = Game.Attacking.Calculate.Melee(attacker, attackedsob);
                                                                damage = (uint)(damage * spell.PowerPercent);
                                                                ReceiveAttack(attacker, attackedsob, attack, damage, spell);

                                                                suse.Targets.Add(attackedsob.UID, damage);
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                attacker.AttackPacket = null;
                                            }
                                            attacker.Owner.SendScreen(suse, true);
                                        }

                                        break;
                                    }
                                #endregion
                                #region Single target
                                case 10490:
                                    {
                                        if (CanUseSpell(spell, attacker.Owner))
                                        {
                                            PrepareSpell(spell, attacker.Owner);

                                            TryTrip suse = new TryTrip(true);
                                            suse.Attacker = attacker.UID;
                                            suse.SpellID = spell.ID;
                                            suse.SpellLevel = spell.Level;

                                            if (ServerBase.Kernel.GetDistance(attacker.X, attacker.Y, X, Y) <= attacker.AttackRange + 1)
                                            {
                                                if (attackedsob != null)
                                                {
                                                    if (CanAttack(attacker, attackedsob, spell))
                                                    {
                                                        PrepareSpell(spell, attacker.Owner);
                                                        uint damage = Game.Attacking.Calculate.Melee(attacker, attackedsob);
                                                        damage = (uint)(damage * spell.PowerPercent);
                                                        ReceiveAttack(attacker, attackedsob, attack, damage, spell);
                                                        suse.Damage = damage;
                                                        suse.Attacked = attackedsob.UID;
                                                    }
                                                }
                                                else
                                                {
                                                    if (CanAttack(attacker, attacked, spell, attack.AttackType == Attack.Melee))
                                                    {
                                                        PrepareSpell(spell, attacker.Owner);
                                                        uint damage = Game.Attacking.Calculate.Melee(attacker, attacked, spell);

                                                        ReceiveAttack(attacker, attacked, attack, damage, spell);
                                                        suse.Damage = damage;
                                                        suse.Attacked = attacked.UID;
                                                    }
                                                }
                                                attacker.AttackPacket = null;
                                            }
                                            else
                                            {
                                                attacker.AttackPacket = null;
                                            }
                                            attacker.Owner.SendScreen(suse, true);
                                        }
                                        attacker.AttackPacket = null;
                                        break;
                                    }
                                case 1290:
                                case 5030:
                                case 5040:
                                case 7000:
                                case 7010:
                                case 7030:
                                case 7040:
                                    {
                                        if (CanUseSpell(spell, attacker.Owner))
                                        {
                                            PrepareSpell(spell, attacker.Owner);

                                            SpellUse suse = new SpellUse(true);
                                            suse.Attacker = attacker.UID;
                                            suse.SpellID = spell.ID;
                                            suse.SpellLevel = spell.Level;
                                            suse.X = X;
                                            suse.Y = Y;

                                            if (ServerBase.Kernel.GetDistance(attacker.X, attacker.Y, X, Y) <= attacker.AttackRange)
                                            {
                                                if (attackedsob != null)
                                                {
                                                    if (CanAttack(attacker, attackedsob, spell))
                                                    {
                                                        PrepareSpell(spell, attacker.Owner);
                                                        uint damage = Game.Attacking.Calculate.Melee(attacker, attackedsob);
                                                        damage = (uint)(damage * spell.PowerPercent);
                                                        ReceiveAttack(attacker, attackedsob, attack, damage, spell);

                                                        suse.Targets.Add(attackedsob.UID, damage);
                                                    }
                                                }
                                                else
                                                {
                                                    if (CanAttack(attacker, attacked, spell, attack.AttackType == Attack.Melee))
                                                    {
                                                        PrepareSpell(spell, attacker.Owner);
                                                        uint damage = Game.Attacking.Calculate.Melee(attacker, attacked, spell);

                                                        ReceiveAttack(attacker, attacked, attack, damage, spell);

                                                        suse.Targets.Add(attacked.UID, damage);
                                                    }
                                                }
                                                attacker.AttackPacket = null;
                                            }
                                            else
                                            {
                                                attacker.AttackPacket = null;
                                            }
                                            attacker.Owner.SendScreen(suse, true);
                                        }
                                        attacker.AttackPacket = null;
                                        break;
                                    }
                                #endregion
                                #region Sector
                                case 1250:
                                case 5050:
                                case 5020:
                                case 1300:
                                    {
                                        if (CanUseSpell(spell, attacker.Owner))
                                        {
                                            PrepareSpell(spell, attacker.Owner);

                                            SpellUse suse = new SpellUse(true);
                                            suse.Attacker = attacker.UID;
                                            suse.SpellID = spell.ID;
                                            suse.SpellLevel = spell.Level;
                                            suse.X = X;
                                            suse.Y = Y;
                                            Sector sector = new Sector(attacker.X, attacker.Y, X, Y);
                                            sector.Arrange(spell.Sector, spell.Range);
                                            if (ServerBase.Kernel.GetDistance(attacker.X, attacker.Y, X, Y) <= spell.Distance + 1)
                                            {
                                                foreach (Interfaces.IMapObject _obj in attacker.Owner.Screen.Objects)
                                                {
                                                    if (_obj == null)
                                                        continue;
                                                    if (_obj.MapObjType == MapObjectType.Monster || _obj.MapObjType == MapObjectType.Player)
                                                    {
                                                        attacked = _obj as Entity;

                                                        if (sector.Inside(attacked.X, attacked.Y))
                                                        {
                                                            if (CanAttack(attacker, attacked, spell, attack.AttackType == Attack.Melee))
                                                            {
                                                                uint damage = Game.Attacking.Calculate.Melee(attacker, attacked, spell);

                                                                ReceiveAttack(attacker, attacked, attack, damage, spell);

                                                                suse.Targets.Add(attacked.UID, damage);
                                                            }
                                                        }
                                                    }
                                                    else if (_obj.MapObjType == MapObjectType.SobNpc)
                                                    {
                                                        attackedsob = _obj as SobNpcSpawn;

                                                        if (sector.Inside(attackedsob.X, attackedsob.Y))
                                                        {
                                                            if (CanAttack(attacker, attackedsob, spell))
                                                            {
                                                                uint damage = Game.Attacking.Calculate.Melee(attacker, attackedsob);
                                                                damage = (uint)(damage * spell.PowerPercent);
                                                                ReceiveAttack(attacker, attackedsob, attack, damage, spell);

                                                                suse.Targets.Add(attackedsob.UID, damage);
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                attacker.AttackPacket = null;
                                            }
                                            attacker.Owner.SendScreen(suse, true);
                                        }
                                        break;
                                    }
                                #endregion
                                #endregion
                                #region Fly
                                case 8002:
                                case 8003:
                                    {
                                        if (CanUseSpell(spell, attacker.Owner))
                                        {
                                            PrepareSpell(spell, attacker.Owner);

                                            SpellUse suse = new SpellUse(true);
                                            suse.Attacker = attacker.UID;
                                            suse.SpellID = spell.ID;
                                            suse.SpellLevel = spell.Level;
                                            suse.X = X;
                                            suse.Y = Y;
                                            attacked.FlyStamp = Time32.Now;
                                            attacked.FlyTime = (byte)spell.Duration;

                                            suse.Targets.Add(attacked.UID, attacked.FlyTime);
                                            attacked.RemoveFlag(Update.Flags.Ride);
                                            attacked.AddFlag(Update.Flags.Fly);

                                            attacked.Owner.SendScreen(suse, true);
                                        }
                                        break;
                                    }
                                #endregion
                                #region Ninja Spells
                                case 6010://Vortex
                                    {
                                        if (CanUseSpell(spell, attacker.Owner))
                                        {
                                            PrepareSpell(spell, attacker.Owner);

                                            SpellUse suse = new SpellUse(true);
                                            suse.Attacker = attacker.UID;
                                            suse.SpellID = spell.ID;
                                            suse.SpellLevel = spell.Level;
                                            suse.X = X;
                                            suse.Y = Y;

                                            attacker.AddFlag(Update.Flags.ShurikenVortex);
                                            attacker.ShurikenVortexStamp = Time32.Now;
                                            attacker.ShurikenVortexTime = 20;

                                            attacker.Owner.SendScreen(suse, true);

                                            attacker.VortexPacket = new Attack(true);
                                            attacker.VortexPacket.Decoded = true;
                                            attacker.VortexPacket.Damage = 6012;
                                            attacker.VortexPacket.AttackType = Attack.Magic;
                                            attacker.VortexPacket.Attacker = attacker.UID;
                                        }
                                        break;
                                    }
                                case 6012://VortexRespone
                                    {
                                        if (!attacker.ContainsFlag(Update.Flags.ShurikenVortex))
                                        {
                                            attacker.AttackPacket = null;
                                            break;
                                        }
                                        SpellUse suse = new SpellUse(true);
                                        suse.Attacker = attacker.UID;
                                        suse.SpellID = spell.ID;
                                        suse.SpellLevel = spell.Level;
                                        suse.X = attacker.X;
                                        suse.Y = attacker.Y;
                                        foreach (Interfaces.IMapObject _obj in attacker.Owner.Screen.Objects)
                                        {
                                            if (_obj == null)
                                                continue;
                                            if (_obj.MapObjType == MapObjectType.Monster || _obj.MapObjType == MapObjectType.Player)
                                            {
                                                attacked = _obj as Entity;
                                                if (ServerBase.Kernel.GetDistance(attacker.X, attacker.Y, attacked.X, attacked.Y) <= spell.Range)
                                                {
                                                    if (CanAttack(attacker, attacked, spell, attack.AttackType == Attack.Melee))
                                                    {
                                                        uint damage = Game.Attacking.Calculate.Melee(attacker, attacked);

                                                        ReceiveAttack(attacker, attacked, attack, damage, spell);

                                                        suse.Targets.Add(attacked.UID, damage);
                                                    }
                                                }
                                            }
                                            else if (_obj.MapObjType == MapObjectType.SobNpc)
                                            {
                                                attackedsob = _obj as SobNpcSpawn;
                                                if (ServerBase.Kernel.GetDistance(attacker.X, attacker.Y, attackedsob.X, attackedsob.Y) <= spell.Range)
                                                {
                                                    if (CanAttack(attacker, attackedsob, spell))
                                                    {
                                                        uint damage = Game.Attacking.Calculate.Melee(attacker, attackedsob);

                                                        ReceiveAttack(attacker, attackedsob, attack, damage, spell);

                                                        suse.Targets.Add(attackedsob.UID, damage);
                                                    }
                                                }
                                            }
                                        }
                                        attacker.Owner.SendScreen(suse, true);
                                        break;
                                    }
                                case 6001:
                                    {

                                        if (CanUseSpell(spell, attacker.Owner))
                                        {
                                            PrepareSpell(spell, attacker.Owner);

                                            SpellUse suse = new SpellUse(true);
                                            suse.Attacker = attacker.UID;
                                            suse.SpellID = spell.ID;
                                            suse.SpellLevel = spell.Level;
                                            suse.X = X;
                                            suse.Y = Y;

                                            if (ServerBase.Kernel.GetDistance(attacker.X, attacker.Y, X, Y) <= spell.Distance)
                                            {

                                                foreach (Interfaces.IMapObject _obj in attacker.Owner.Screen.Objects)
                                                {
                                                    if (_obj.MapObjType == MapObjectType.Monster || _obj.MapObjType == MapObjectType.Player)
                                                    {
                                                        attacked = _obj as Entity;
                                                        if (ServerBase.Kernel.GetDistance(X, Y, attacked.X, attacked.Y) <= spell.Range)
                                                        {
                                                            if (attacked.Name == "TeratoDragon")//SnowBanshee
                                                                continue;
                                                            if (attacked.Name == "SnowBanshee")//SnowBanshee
                                                                continue;
                                                            if (attacked.Name == "ThrillingSpook")//SnowBanshee
                                                                continue;
                                                            if (attacked.Name == "SwordMaster")//SnowBanshee
                                                                continue;
                                                            if (attacked.Name == "LavaBeast")//SnowBanshee
                                                                continue;
                                                            if (attacked.Name == "Guard1")//SnowBanshee
                                                                continue;
                                                            if (CanAttack(attacker, attacked, spell, attack.AttackType == Attack.Melee))
                                                            {
                                                                int potDifference = attacker.BattlePower - attacked.BattlePower;

                                                                int rate = spell.Percent + potDifference;
                                                                // uint dmg = (uint)((attacked.Hitpoints * ((10))) / 100);
                                                                if (attacked.Statistics.Detoxication > 0 && attacked.Statistics.Detoxication < 100)
                                                                    rate = (int)((rate * attacked.Statistics.Detoxication) / 100);
                                                                if (attacked.Statistics.Detoxication >= 100)
                                                                    rate = 0;
                                                                if (ServerBase.Kernel.Rate(rate))
                                                                {
                                                                    attacked.ToxicFogStamp = Time32.Now;
                                                                    attacked.ToxicFogLeft = 20;
                                                                    attacked.ToxicFogPercent = spell.PowerPercent;
                                                                    suse.Targets.Add(attacked.UID, 1);

                                                                }
                                                                else
                                                                {
                                                                    suse.Targets.Add(attacked.UID, 0);
                                                                    suse.Targets[attacked.UID].Hit = false;
                                                                }
                                                                if (attacked.Hitpoints <= 1)
                                                                {
                                                                    //client.Entity.Poison = null;
                                                                    suse.Targets.Add(attacked.UID, 0);
                                                                    suse.Targets[attacked.UID].Hit = false;
                                                                    //attacked.Update(true, false, true, Game.Enums.Poison);

                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                attacker.AttackPacket = null;
                                            }
                                            attacker.Owner.SendScreen(suse, true);
                                        }
                                        break;
                                    }
                                case 6000:
                                    {
                                        if (CanUseSpell(spell, attacker.Owner))
                                        {
                                            ushort Xx, Yx;
                                            if (attacked != null)
                                            {
                                                Xx = attacked.X;
                                                Yx = attacked.Y;
                                            }
                                            else
                                            {
                                                Xx = attackedsob.X;
                                                Yx = attackedsob.Y;
                                            }
                                            if (ServerBase.Kernel.GetDistance(attacker.X, attacker.Y, Xx, Yx) <= spell.Range)
                                            {
                                                if (attackedsob == null)
                                                    if (attacked.ContainsFlag(Network.GamePackets.Update.Flags.Fly))
                                                        return;
                                                if (attacked.ContainsFlag(Network.GamePackets.Update.Flags.Fly))
                                                    return;
                                                if (attacker.ContainsFlag(Network.GamePackets.Update.Flags.Fly))
                                                    return;
                                                PrepareSpell(spell, attacker.Owner);

                                                SpellUse suse = new SpellUse(true);
                                                suse.Attacker = attacker.UID;
                                                suse.SpellID = spell.ID;
                                                suse.SpellLevel = spell.Level;
                                                suse.X = X;
                                                suse.Y = Y;

                                                bool send = false;

                                                if (attackedsob == null)
                                                {
                                                    if (CanAttack(attacker, attacked, spell, attack.AttackType == Attack.Melee))
                                                    {
                                                        uint damage = Game.Attacking.Calculate.Melee(attacker, attacked, spell);

                                                        ReceiveAttack(attacker, attacked, attack, damage, spell);

                                                        suse.Targets.Add(attacked.UID, damage);
                                                        send = true;
                                                    }
                                                }
                                                else
                                                {
                                                    if (CanAttack(attacker, attackedsob, spell))
                                                    {
                                                        uint damage = Game.Attacking.Calculate.Melee(attacker, attackedsob);
                                                        damage = (uint)(damage * spell.PowerPercent);
                                                        ReceiveAttack(attacker, attackedsob, attack, damage, spell);

                                                        suse.Targets.Add(attackedsob.UID, damage);
                                                        send = true;
                                                    }
                                                }
                                                if (send)
                                                    attacker.Owner.SendScreen(suse, true);
                                            }
                                            else
                                            {
                                                attacker.AttackPacket = null;
                                            }
                                        }
                                        break;
                                    }
                                case 6002:
                                    {
                                        if (attackedsob != null)
                                            return;
                                        if (attacked.EntityFlag == EntityFlag.Monster)
                                            return;
                                        if (ServerBase.Kernel.GetDistance(attacker.X, attacker.Y, attacked.X, attacked.Y) <= spell.Distance)
                                        {
                                            if (CanUseSpell(spell, attacker.Owner))
                                            {
                                                PrepareSpell(spell, attacker.Owner);

                                                int potDifference = attacker.BattlePower - attacked.BattlePower;

                                                int rate = spell.Percent + potDifference;

                                                SpellUse suse = new SpellUse(true);
                                                suse.Attacker = attacker.UID;
                                                suse.SpellID = spell.ID;
                                                suse.SpellLevel = spell.Level;
                                                suse.X = X;
                                                suse.Y = Y;
                                                if (CanAttack(attacker, attacked, spell, false))
                                                {
                                                    suse.Targets.Add(attacked.UID, 0);
                                                    if (ServerBase.Kernel.Rate(rate))
                                                    {
                                                        attacked.NoDrugsStamp = Time32.Now;
                                                        attacked.NoDrugsTime = (short)spell.Duration;
                                                        if (attacked.EntityFlag == EntityFlag.Player)
                                                        {
                                                            attacked.Owner.Send(ServerBase.Constants.NoDrugs(spell.Duration));
                                                        }
                                                    }
                                                    else
                                                    {
                                                        suse.Targets[attacked.UID].Hit = false;
                                                    }

                                                    attacked.Owner.SendScreen(suse, true);
                                                }
                                            }
                                        }
                                        break;
                                    }
                                case 6004:
                                    {
                                        if (attackedsob != null)
                                            return;
                                        if (attacked.EntityFlag == EntityFlag.Monster)
                                            return;
                                        if (!attacked.ContainsFlag(Update.Flags.Fly))
                                            return;
                                        if (ServerBase.Kernel.GetDistance(attacker.X, attacker.Y, attacked.X, attacked.Y) <= spell.Distance)
                                        {
                                            if (CanUseSpell(spell, attacker.Owner))
                                            {
                                                PrepareSpell(spell, attacker.Owner);

                                                int potDifference = attacker.BattlePower - attacked.BattlePower;

                                                int rate = spell.Percent + potDifference;

                                                SpellUse suse = new SpellUse(true);
                                                suse.Attacker = attacker.UID;
                                                suse.SpellID = spell.ID;
                                                suse.SpellLevel = spell.Level;
                                                suse.X = X;
                                                suse.Y = Y;
                                                if (CanAttack(attacker, attacked, spell, false))
                                                {
                                                    uint dmg = Calculate.Percent(attacked, 0.1F);
                                                    suse.Targets.Add(attacked.UID, dmg);

                                                    if (ServerBase.Kernel.Rate(rate))
                                                    {
                                                        attacked.Hitpoints -= dmg;
                                                        attacked.RemoveFlag(Update.Flags.Fly);
                                                    }
                                                    else
                                                    {
                                                        suse.Targets[attacked.UID].Hit = false;
                                                    }

                                                    attacked.Owner.SendScreen(suse, true);
                                                }
                                            }
                                        }
                                        break;
                                    }
                                #endregion
                                #region Riding
                                case 7001:
                                    {
                                        if (!attacker.Owner.Equipment.Free(12))
                                        {
                                            SpellUse suse = new SpellUse(true);
                                            suse.Attacker = attacker.UID;
                                            suse.SpellID = spell.ID;
                                            suse.SpellLevel = spell.Level;
                                            suse.X = X;
                                            suse.Y = Y;

                                            if (attacker.ContainsFlag(Update.Flags.Ride))
                                            {
                                                attacker.RemoveFlag(Update.Flags.Ride);
                                            }
                                            else
                                            {
                                                if (attacker.Owner.Map.ID == 1036 && attacker.Owner.Equipment.TryGetItem((byte)12).Plus < 6)
                                                    break;
                                                if (attacker.Stamina >= 100 && (attacker.Owner.QualifierGroup == null || attacker.Owner.QualifierGroup != null && !attacker.Owner.QualifierGroup.Inside))
                                                {
                                                    attacker.AddFlag(Update.Flags.Ride);
                                                    attacker.Stamina -= 100;
                                                    Network.GamePackets.Vigor vigor = new Network.GamePackets.Vigor(true);
                                                    vigor.VigorValue = attacker.Owner.Entity.MaxVigor;
                                                    vigor.Send(attacker.Owner);
                                                }
                                            }
                                            suse.Targets.Add(attacker.UID, 0);
                                            attacker.Owner.SendScreen(suse, true);
                                        }
                                        break;
                                    }
                                case 7002:
                                    {//Spook
                                        if (attacked.ContainsFlag(Update.Flags.Ride) && attacker.ContainsFlag(Update.Flags.Ride))
                                        {
                                            Interfaces.IConquerItem attackedSteed = null, attackerSteed = null;
                                            if ((attackedSteed = attacked.Owner.Equipment.TryGetItem(ConquerItem.Steed)) != null)
                                            {
                                                if ((attackerSteed = attacker.Owner.Equipment.TryGetItem(ConquerItem.Steed)) != null)
                                                {
                                                    SpellUse suse = new SpellUse(true);
                                                    suse.Attacker = attacker.UID;
                                                    suse.SpellID = spell.ID;
                                                    suse.SpellLevel = spell.Level;
                                                    suse.X = X;
                                                    suse.Y = Y;
                                                    suse.Targets.Add(attacked.UID, 0);
                                                   
                                                    if (attackedSteed.Plus < attackerSteed.Plus)
                                                        attacked.RemoveFlag(Update.Flags.Ride);
                                                    else if (attackedSteed.Plus == attackerSteed.Plus && attackedSteed.PlusProgress <= attackerSteed.PlusProgress)
                                                        attacked.RemoveFlag(Update.Flags.Ride);
                                                    else
                                                        suse.Targets[attacked.UID].Hit = false;
                                                    attacker.Owner.SendScreen(suse, true);
                                                }
                                            }
                                        }
                                        break;
                                    }
                                case 7003:
                                    {//WarCry
                                        SpellUse suse = new SpellUse(true);
                                        suse.Attacker = attacker.UID;
                                        suse.SpellID = spell.ID;
                                        suse.SpellLevel = spell.Level;
                                        suse.X = X;
                                        suse.Y = Y;
                                        Interfaces.IConquerItem attackedSteed = null, attackerSteed = null;
                                        foreach (Interfaces.IMapObject _obj in attacker.Owner.Screen.Objects)
                                        {
                                            if (_obj == null)
                                                continue;
                                            if (_obj.MapObjType == MapObjectType.Player && _obj.UID != attacker.UID)
                                            {
                                                attacked = _obj as Entity;
                                                if ((attackedSteed = attacked.Owner.Equipment.TryGetItem(ConquerItem.Steed)) != null)
                                                {
                                                    if ((attackerSteed = attacker.Owner.Equipment.TryGetItem(ConquerItem.Steed)) != null)
                                                    {
                                                        if (ServerBase.Kernel.GetDistance(attacker.X, attacker.Y, attacked.X, attacked.Y) <= attackedSteed.Plus)
                                                        {
                                                            if (CanAttack(attacker, attacked, spell, attack.AttackType == Attack.Melee))
                                                            {
                                                                suse.Targets.Add(attacked.UID, 0);
                                                                if (attackedSteed.Plus < attackerSteed.Plus)
                                                                    attacked.RemoveFlag(Update.Flags.Ride);
                                                                else if (attackedSteed.Plus == attackerSteed.Plus && attackedSteed.PlusProgress <= attackerSteed.PlusProgress)
                                                                    attacked.RemoveFlag(Update.Flags.Ride);
                                                                else
                                                                    suse.Targets[attacked.UID].Hit = false;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        attacker.Owner.SendScreen(suse, true);
                                        break;
                                    }
                                #endregion
                                #region Dash
                                case 1051:
                                    {
                                        if (attacked != null)
                                        {
                                            if (!attacked.Dead)
                                            {
                                                var direction = ServerBase.Kernel.GetAngle(attacker.X, attacker.Y, attacked.X, attacked.Y);
                                                if (CanAttack(attacker, attacked, spell, attack.AttackType == Attack.Melee))
                                                {
                                                    uint damage = Calculate.Melee(attacker, attacked);
                                                    attack = new Attack(true);
                                                    attack.AttackType = Attack.Dash;
                                                    attack.X = attacked.X;
                                                    attack.Y = attacked.Y;
                                                    attack.Attacker = attacker.UID;
                                                    attack.Attacked = attacked.UID;
                                                    attack.Damage = damage;
                                                    attack.ToArray()[27] = (byte)direction;
                                                    attacked.Move(direction);
                                                    attacker.Move(direction);

                                                    ReceiveAttack(attacker, attacked, attack, damage, spell);

                                                    attacker.Owner.SendScreen(attack, true);
                                                }
                                            }
                                        }
                                        break;
                                    }
                                #endregion
                                #region RapidFire
                                case 8000:
                                    {
                                        if (attackedsob != null)
                                        {
                                            if (CanUseSpell(spell, attacker.Owner))
                                            {
                                                if (CanAttack(attacker, attackedsob, spell))
                                                {
                                                    PrepareSpell(spell, attacker.Owner);
                                                    SpellUse suse = new SpellUse(true);
                                                    suse.Attacker = attacker.UID;
                                                    suse.SpellID = spell.ID;
                                                    suse.SpellLevel = spell.Level;
                                                    suse.X = attackedsob.X;
                                                    suse.Y = attackedsob.Y;
                                                    uint damage = Calculate.Ranged(attacker, attackedsob);
                                                    damage = (uint)(damage * spell.PowerPercent);
                                                    suse.Targets.Add(attackedsob.UID, damage);

                                                    ReceiveAttack(attacker, attackedsob, attack, damage, spell);

                                                    attacker.Owner.SendScreen(suse, true);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (!attacked.Dead)
                                            {
                                                if (CanUseSpell(spell, attacker.Owner))
                                                {
                                                    if (CanAttack(attacker, attacked, spell, attack.AttackType == Attack.Melee))
                                                    {
                                                        PrepareSpell(spell, attacker.Owner);
                                                        SpellUse suse = new SpellUse(true);
                                                        suse.Attacker = attacker.UID;
                                                        suse.SpellID = spell.ID;
                                                        suse.SpellLevel = spell.Level;
                                                        suse.X = attacked.X;
                                                        suse.Y = attacked.Y;
                                                        uint damage = Calculate.Ranged(attacker, attacked);
                                                        damage = (uint)(damage * spell.PowerPercent);
                                                        suse.Targets.Add(attacked.UID, damage);

                                                        ReceiveAttack(attacker, attacked, attack, damage, spell);

                                                        attacker.Owner.SendScreen(suse, true);
                                                    }
                                                }
                                            }
                                        }
                                        break;
                                    }
                                #endregion
                                #region FireOfHell
                                case 1165:
                                    {
                                        if (CanUseSpell(spell, attacker.Owner))
                                        {
                                            PrepareSpell(spell, attacker.Owner);

                                            SpellUse suse = new SpellUse(true);
                                            suse.Attacker = attacker.UID;
                                            suse.SpellID = spell.ID;
                                            suse.SpellLevel = spell.Level;
                                            suse.X = X;
                                            suse.Y = Y;
                                            Sector sector = new Sector(attacker.X, attacker.Y, X, Y);
                                            sector.Arrange(spell.Sector, spell.Distance);
                                            foreach (Interfaces.IMapObject _obj in attacker.Owner.Screen.Objects)
                                            {
                                                if (_obj == null)
                                                    continue;
                                                if (_obj.MapObjType == MapObjectType.Monster || _obj.MapObjType == MapObjectType.Player)
                                                {
                                                    attacked = _obj as Entity;

                                                    if (sector.Inside(attacked.X, attacked.Y))
                                                    {
                                                        if (CanAttack(attacker, attacked, spell, attack.AttackType == Attack.Melee))
                                                        {
                                                            uint damage = Game.Attacking.Calculate.Magic(attacker, attacked, spell);

                                                            ReceiveAttack(attacker, attacked, attack, damage, spell);

                                                            suse.Targets.Add(attacked.UID, damage);
                                                        }
                                                    }
                                                }
                                                else if (_obj.MapObjType == MapObjectType.SobNpc)
                                                {
                                                    attackedsob = _obj as SobNpcSpawn;

                                                    if (sector.Inside(attackedsob.X, attackedsob.Y))
                                                    {
                                                        if (CanAttack(attacker, attackedsob, spell))
                                                        {
                                                            uint damage = Game.Attacking.Calculate.Magic(attacker, attackedsob, spell);

                                                            ReceiveAttack(attacker, attackedsob, attack, damage, spell);

                                                            suse.Targets.Add(attackedsob.UID, damage);
                                                        }
                                                    }
                                                }
                                            }
                                            attacker.Owner.SendScreen(suse, true);
                                        }
                                        break;
                                    }
                                #endregion
                                #region Scatter
                                case 8001:
                                    {
                                        if (CanUseSpell(spell, attacker.Owner))
                                        {
                                            PrepareSpell(spell, attacker.Owner);

                                            SpellUse suse = new SpellUse(true);
                                            suse.Attacker = attacker.UID;
                                            suse.SpellID = spell.ID;
                                            suse.SpellLevel = spell.Level;
                                            suse.X = X;
                                            suse.Y = Y;
                                            Sector sector = new Sector(attacker.X, attacker.Y, X, Y);
                                            sector.Arrange(spell.Sector, spell.Distance);
                                            foreach (Interfaces.IMapObject _obj in attacker.Owner.Screen.Objects)
                                            {
                                                if (_obj == null)
                                                    continue;
                                                if (_obj.MapObjType == MapObjectType.Monster || _obj.MapObjType == MapObjectType.Player)
                                                {
                                                    attacked = _obj as Entity;

                                                    if (sector.Inside(attacked.X, attacked.Y))
                                                    {
                                                        if (CanAttack(attacker, attacked, spell, attack.AttackType == Attack.Melee))
                                                        {
                                                            uint damage = Game.Attacking.Calculate.Ranged(attacker, attacked, spell);

                                                            ReceiveAttack(attacker, attacked, attack, damage, spell);

                                                            suse.Targets.Add(attacked.UID, damage);
                                                        }
                                                    }
                                                }
                                                else if (_obj.MapObjType == MapObjectType.SobNpc)
                                                {
                                                    attackedsob = _obj as SobNpcSpawn;

                                                    if (sector.Inside(attackedsob.X, attackedsob.Y))
                                                    {
                                                        if (CanAttack(attacker, attackedsob, spell))
                                                        {
                                                            uint damage = Game.Attacking.Calculate.Ranged(attacker, attackedsob);
                                                            if (damage == 0)
                                                                damage = 1;
                                                            damage = Game.Attacking.Calculate.Percent((int)damage, spell.PowerPercent);

                                                            ReceiveAttack(attacker, attackedsob, attack, damage, spell);

                                                            suse.Targets.Add(attackedsob.UID, damage);
                                                        }
                                                    }
                                                }
                                            }
                                            attacker.Owner.SendScreen(suse, true);
                                        }
                                        break;
                                    }
                                #endregion
                                #region Intensify
                                case 9000:
                                    {
                                        attacker.IntensifyStamp = Time32.Now;
                                        attacker.OnIntensify = true;
                                        SpellUse suse = new SpellUse(true);
                                        suse.Attacker = attacker.UID;
                                        suse.SpellID = spell.ID;
                                        suse.SpellLevel = spell.Level;
                                        suse.X = X;
                                        suse.Y = Y;
                                        suse.Targets.Add(attacker.UID, 0);
                                        suse.Send(attacker.Owner);
                                        break;
                                    }
                                #endregion
                                
                                #region Trasnformations
                                case 1270:
                                case 1280:
                                case 1350:
                                case 1360:
                                case 3321:
                                    {
                                        if (CanUseSpell(spell, attacker.Owner))
                                        {
                                            if (attacker.MapID == 1036)
                                                return;
                                            bool wasTransformated = attacker.Transformed;
                                            PrepareSpell(spell, attacker.Owner);

                                            #region Atributes
                                            switch (spell.ID)
                                            {
                                                case 1350:
                                                    switch (spell.Level)
                                                    {
                                                        case 0:
                                                            {
                                                                attacker.TransformationMaxAttack = 182;
                                                                attacker.TransformationMinAttack = 122;
                                                                attacker.TransformationDefence = 1300;
                                                                attacker.TransformationMagicDefence = 94;
                                                                attacker.TransformationDodge = 35;
                                                                attacker.TransformationTime = 39;
                                                                attacker.TransformationID = 207;
                                                                break;
                                                            }
                                                        case 1:
                                                            {
                                                                attacker.TransformationMaxAttack = 200;
                                                                attacker.TransformationMinAttack = 134;
                                                                attacker.TransformationDefence = 1400;
                                                                attacker.TransformationMagicDefence = 96;
                                                                attacker.TransformationDodge = 40;
                                                                attacker.TransformationTime = 49;
                                                                attacker.TransformationID = 207;
                                                                break;
                                                            }
                                                        case 2:
                                                            {
                                                                attacker.TransformationMaxAttack = 240;
                                                                attacker.TransformationMinAttack = 160;
                                                                attacker.TransformationDefence = 1500;
                                                                attacker.TransformationMagicDefence = 97;
                                                                attacker.TransformationDodge = 45;
                                                                attacker.TransformationTime = 59;
                                                                attacker.TransformationID = 207;
                                                                break;
                                                            }
                                                        case 3:
                                                            {
                                                                attacker.TransformationMaxAttack = 258;
                                                                attacker.TransformationMinAttack = 172;
                                                                attacker.TransformationDefence = 1600;
                                                                attacker.TransformationMagicDefence = 98;
                                                                attacker.TransformationDodge = 50;
                                                                attacker.TransformationTime = 69;
                                                                attacker.TransformationID = 267;
                                                                break;
                                                            }
                                                        case 4:
                                                            {
                                                                attacker.TransformationMaxAttack = 300;
                                                                attacker.TransformationMinAttack = 200;
                                                                attacker.TransformationDefence = 1900;
                                                                attacker.TransformationMagicDefence = 99;
                                                                attacker.TransformationDodge = 55;
                                                                attacker.TransformationTime = 79;
                                                                attacker.TransformationID = 267;
                                                                break;
                                                            }
                                                    }
                                                    break;
                                                case 1270:
                                                    switch (spell.Level)
                                                    {
                                                        case 0:
                                                            {
                                                                attacker.TransformationMaxAttack = 282;
                                                                attacker.TransformationMinAttack = 179;
                                                                attacker.TransformationDefence = 73;
                                                                attacker.TransformationMagicDefence = 34;
                                                                attacker.TransformationDodge = 9;
                                                                attacker.TransformationTime = 34;
                                                                attacker.TransformationID = 214;
                                                                break;
                                                            }
                                                        case 1:
                                                            {
                                                                attacker.TransformationMaxAttack = 395;
                                                                attacker.TransformationMinAttack = 245;
                                                                attacker.TransformationDefence = 126;
                                                                attacker.TransformationMagicDefence = 45;
                                                                attacker.TransformationDodge = 12;
                                                                attacker.TransformationTime = 39;
                                                                attacker.TransformationID = 214;
                                                                break;
                                                            }
                                                        case 2:
                                                            {
                                                                attacker.TransformationMaxAttack = 616;
                                                                attacker.TransformationMinAttack = 367;
                                                                attacker.TransformationDefence = 180;
                                                                attacker.TransformationMagicDefence = 53;
                                                                attacker.TransformationDodge = 15;
                                                                attacker.TransformationTime = 44;
                                                                attacker.TransformationID = 214;
                                                                break;
                                                            }
                                                        case 3:
                                                            {
                                                                attacker.TransformationMaxAttack = 724;
                                                                attacker.TransformationMinAttack = 429;
                                                                attacker.TransformationDefence = 247;
                                                                attacker.TransformationMagicDefence = 53;
                                                                attacker.TransformationDodge = 15;
                                                                attacker.TransformationTime = 49;
                                                                attacker.TransformationID = 214;
                                                                break;
                                                            }
                                                        case 4:
                                                            {
                                                                attacker.TransformationMaxAttack = 1231;
                                                                attacker.TransformationMinAttack = 704;
                                                                attacker.TransformationDefence = 499;
                                                                attacker.TransformationMagicDefence = 50;
                                                                attacker.TransformationDodge = 20;
                                                                attacker.TransformationTime = 54;
                                                                attacker.TransformationID = 274;
                                                                break;
                                                            }
                                                        case 5:
                                                            {
                                                                attacker.TransformationMaxAttack = 1573;
                                                                attacker.TransformationMinAttack = 941;
                                                                attacker.TransformationDefence = 601;
                                                                attacker.TransformationMagicDefence = 53;
                                                                attacker.TransformationDodge = 25;
                                                                attacker.TransformationTime = 59;
                                                                attacker.TransformationID = 274;
                                                                break;
                                                            }
                                                        case 6:
                                                            {
                                                                attacker.TransformationMaxAttack = 1991;
                                                                attacker.TransformationMinAttack = 1107;
                                                                attacker.TransformationDefence = 1029;
                                                                attacker.TransformationMagicDefence = 55;
                                                                attacker.TransformationDodge = 30;
                                                                attacker.TransformationTime = 64;
                                                                attacker.TransformationID = 274;
                                                                break;
                                                            }
                                                        case 7:
                                                            {
                                                                attacker.TransformationMaxAttack = 2226;
                                                                attacker.TransformationMinAttack = 1235;
                                                                attacker.TransformationDefence = 1029;
                                                                attacker.TransformationMagicDefence = 55;
                                                                attacker.TransformationDodge = 35;
                                                                attacker.TransformationTime = 69;
                                                                attacker.TransformationID = 274;
                                                                break;
                                                            }
                                                    }
                                                    break;
                                                case 1360:
                                                    switch (spell.Level)
                                                    {
                                                        case 0:
                                                            {
                                                                attacker.TransformationMaxAttack = 1215;
                                                                attacker.TransformationMinAttack = 610;
                                                                attacker.TransformationDefence = 100;
                                                                attacker.TransformationMagicDefence = 96;
                                                                attacker.TransformationDodge = 30;
                                                                attacker.TransformationTime = 59;
                                                                attacker.TransformationID = 217;
                                                                break;
                                                            }
                                                        case 1:
                                                            {
                                                                attacker.TransformationMaxAttack = 1310;
                                                                attacker.TransformationMinAttack = 650;
                                                                attacker.TransformationDefence = 400;
                                                                attacker.TransformationMagicDefence = 97;
                                                                attacker.TransformationDodge = 30;
                                                                attacker.TransformationTime = 79;
                                                                attacker.TransformationID = 217;
                                                                break;
                                                            }
                                                        case 2:
                                                            {
                                                                attacker.TransformationMaxAttack = 1420;
                                                                attacker.TransformationMinAttack = 710;
                                                                attacker.TransformationDefence = 650;
                                                                attacker.TransformationMagicDefence = 98;
                                                                attacker.TransformationDodge = 30;
                                                                attacker.TransformationTime = 89;
                                                                attacker.TransformationID = 217;
                                                                break;
                                                            }
                                                        case 3:
                                                            {
                                                                attacker.TransformationMaxAttack = 1555;
                                                                attacker.TransformationMinAttack = 780;
                                                                attacker.TransformationDefence = 720;
                                                                attacker.TransformationMagicDefence = 98;
                                                                attacker.TransformationDodge = 30;
                                                                attacker.TransformationTime = 99;
                                                                attacker.TransformationID = 277;
                                                                break;
                                                            }
                                                        case 4:
                                                            {
                                                                attacker.TransformationMaxAttack = 1660;
                                                                attacker.TransformationMinAttack = 840;
                                                                attacker.TransformationDefence = 1200;
                                                                attacker.TransformationMagicDefence = 99;
                                                                attacker.TransformationDodge = 30;
                                                                attacker.TransformationTime = 109;
                                                                attacker.TransformationID = 277;
                                                                break;
                                                            }
                                                    }
                                                    break;
                                                case 1280:
                                                    switch (spell.Level)
                                                    {
                                                        case 0:
                                                            {
                                                                attacker.TransformationMaxAttack = 930;
                                                                attacker.TransformationMinAttack = 656;
                                                                attacker.TransformationDefence = 290;
                                                                attacker.TransformationMagicDefence = 45;
                                                                attacker.TransformationDodge = 40;
                                                                attacker.TransformationTime = 29;
                                                                attacker.TransformationID = 213;
                                                                break;
                                                            }
                                                        case 1:
                                                            {
                                                                attacker.TransformationMaxAttack = 1062;
                                                                attacker.TransformationMinAttack = 750;
                                                                attacker.TransformationDefence = 320;
                                                                attacker.TransformationMagicDefence = 46;
                                                                attacker.TransformationDodge = 40;
                                                                attacker.TransformationTime = 34;
                                                                attacker.TransformationID = 213;
                                                                break;
                                                            }
                                                        case 2:
                                                            {
                                                                attacker.TransformationMaxAttack = 1292;
                                                                attacker.TransformationMinAttack = 910;
                                                                attacker.TransformationDefence = 510;
                                                                attacker.TransformationMagicDefence = 50;
                                                                attacker.TransformationDodge = 40;
                                                                attacker.TransformationTime = 39;
                                                                attacker.TransformationID = 213;
                                                                break;
                                                            }
                                                        case 3:
                                                            {
                                                                attacker.TransformationMaxAttack = 1428;
                                                                attacker.TransformationMinAttack = 1000;
                                                                attacker.TransformationDefence = 600;
                                                                attacker.TransformationMagicDefence = 53;
                                                                attacker.TransformationDodge = 40;
                                                                attacker.TransformationTime = 44;
                                                                attacker.TransformationID = 213;
                                                                break;
                                                            }
                                                        case 4:
                                                            {
                                                                attacker.TransformationMaxAttack = 1570;
                                                                attacker.TransformationMinAttack = 1100;
                                                                attacker.TransformationDefence = 700;
                                                                attacker.TransformationMagicDefence = 55;
                                                                attacker.TransformationDodge = 40;
                                                                attacker.TransformationTime = 49;
                                                                attacker.TransformationID = 213;
                                                                break;
                                                            }
                                                        case 5:
                                                            {
                                                                attacker.TransformationMaxAttack = 1700;
                                                                attacker.TransformationMinAttack = 1200;
                                                                attacker.TransformationDefence = 880;
                                                                attacker.TransformationMagicDefence = 57;
                                                                attacker.TransformationDodge = 40;
                                                                attacker.TransformationTime = 54;
                                                                attacker.TransformationID = 273;
                                                                break;
                                                            }
                                                        case 6:
                                                            {
                                                                attacker.TransformationMaxAttack = 1900;
                                                                attacker.TransformationMinAttack = 1300;
                                                                attacker.TransformationDefence = 1540;
                                                                attacker.TransformationMagicDefence = 59;
                                                                attacker.TransformationDodge = 40;
                                                                attacker.TransformationTime = 59;
                                                                attacker.TransformationID = 273;
                                                                break;
                                                            }
                                                        case 7:
                                                            {
                                                                attacker.TransformationMaxAttack = 2100;
                                                                attacker.TransformationMinAttack = 1500;
                                                                attacker.TransformationDefence = 1880;
                                                                attacker.TransformationMagicDefence = 61;
                                                                attacker.TransformationDodge = 40;
                                                                attacker.TransformationTime = 59;
                                                                attacker.TransformationID = 273;
                                                                break;
                                                            }
                                                        case 8:
                                                            {
                                                                attacker.TransformationMaxAttack = 2300;
                                                                attacker.TransformationMinAttack = 1600;
                                                                attacker.TransformationDefence = 1970;
                                                                attacker.TransformationMagicDefence = 63;
                                                                attacker.TransformationDodge = 40;
                                                                attacker.TransformationTime = 59;
                                                                attacker.TransformationID = 273;
                                                                break;
                                                            }
                                                    }
                                                    break;

                                                case 3321:
                                                    {
                                                        attacker.TransformationMaxAttack = 2000000;
                                                        attacker.TransformationMinAttack = 2000000;
                                                        attacker.TransformationDefence = 65355;
                                                        attacker.TransformationMagicDefence = 65355;
                                                        attacker.TransformationDodge = 35;
                                                        attacker.TransformationTime = 65355;
                                                        attacker.TransformationID = 223;
                                                        break;
                                                    }


                                            }
                                            #endregion

                                            SpellUse spellUse = new SpellUse(true);
                                            spellUse.Attacker = attacker.UID;
                                            spellUse.SpellID = spell.ID;
                                            spellUse.SpellLevel = spell.Level;
                                            spellUse.X = X;
                                            spellUse.Y = Y;
                                            spellUse.Targets.Add(attacker.UID, (uint)0);
                                            attacker.Owner.SendScreen(spellUse, true);
                                            attacker.TransformationStamp = Time32.Now;
                                            attacker.TransformationMaxHP = 3000;
                                            if (spell.ID == 1270)
                                                attacker.TransformationMaxHP = 50000;
                                            attacker.TransformationAttackRange = 3;
                                            if (spell.ID == 1360)
                                                attacker.TransformationAttackRange = 10;
                                            if (!wasTransformated)
                                            {
                                                double maxHP = attacker.MaxHitpoints;
                                                double HP = attacker.Hitpoints;
                                                double point = HP / maxHP;

                                                attacker.Hitpoints = (uint)(attacker.TransformationMaxHP * point);
                                            }
                                            attacker.Update(Update.MaxHitpoints, attacker.TransformationMaxHP, false);
                                        }
                                        break;
                                    }
                                #endregion
                                #region Bless
                                case 9876:
                                    {
                                        if (CanUseSpell(spell, attacker.Owner))
                                        {
                                            PrepareSpell(spell, attacker.Owner);
                                            attacker.AddFlag(Update.Flags.CastPray);
                                            SpellUse spellUse = new SpellUse(true);
                                            spellUse.Attacker = attacker.UID;
                                            spellUse.SpellID = spell.ID;
                                            spellUse.SpellLevel = spell.Level;
                                            spellUse.X = X;
                                            spellUse.Y = Y;
                                            spellUse.Targets.Add(attacker.UID, 0);
                                            attacker.Owner.SendScreen(spellUse, true);
                                        }
                                        break;
                                    }
                                #endregion
                                #region Companions
                                case 4000:
                                case 4010:
                                case 4020:
                                case 4050:
                                case 4060:
                                case 4070:
                                    {
                                        if (CanUseSpell(spell, attacker.Owner))
                                        {
                                            if (attacker.Owner.Companion != null)
                                            {
                                                if (attacker.Owner.Companion.MonsterInfo != null)
                                                {
                                                    attacker.Owner.Map.RemoveEntity(attacker.Owner.Companion);
                                                    Data data = new Data(true);
                                                    data.UID = attacker.Owner.Companion.UID;
                                                    data.ID = Data.RemoveEntity;
                                                    attacker.Owner.Companion.MonsterInfo.SendScreen(data);
                                                    attacker.Owner.Companion = null;
                                                }
                                            }
                                            PrepareSpell(spell, attacker.Owner);
                                            SpellUse spellUse = new SpellUse(true);
                                            spellUse.Attacker = attacker.UID;
                                            spellUse.SpellID = spell.ID;
                                            spellUse.SpellLevel = spell.Level;
                                            spellUse.X = X;
                                            spellUse.Y = Y;
                                            spellUse.Targets.Add(attacker.UID, 0);
                                            attacker.Owner.SendScreen(spellUse, true);
                                            attacker.Owner.Companion = new Entity(EntityFlag.Monster, true);
                                            attacker.Owner.Companion.MonsterInfo = new Conquer_Online_Server.Database.MonsterInformation();
                                            Database.MonsterInformation mt = Database.MonsterInformation.MonsterInfos[spell.Power];
                                            attacker.Owner.Companion.Owner = attacker.Owner;
                                            attacker.Owner.Companion.MapObjType = MapObjectType.Monster;
                                            attacker.Owner.Companion.MonsterInfo = mt.Copy();
                                            attacker.Owner.Companion.MonsterInfo.Owner = attacker.Owner.Companion;
                                            attacker.Owner.Companion.Name = mt.Name;
                                            attacker.Owner.Companion.MinAttack = mt.MinAttack;
                                            attacker.Owner.Companion.MaxAttack = attacker.Owner.Companion.MagicAttack = mt.MaxAttack;
                                            attacker.Owner.Companion.Hitpoints = attacker.Owner.Companion.MaxHitpoints = mt.Hitpoints;
                                            attacker.Owner.Companion.Body = mt.Mesh;
                                            attacker.Owner.Companion.Level = mt.Level;
                                            attacker.Owner.Companion.UID = (uint)(attacker.UID - 200000);
                                            attacker.Owner.Companion.MapID = attacker.Owner.Map.ID;
                                            attacker.Owner.Companion.SendUpdates = true;
                                            attacker.Owner.Companion.X = attacker.X;
                                            attacker.Owner.Companion.Y = attacker.Y;
                                            attacker.Owner.Map.AddEntity(attacker.Owner.Companion);
                                            attacker.Owner.SendScreenSpawn(attacker.Owner.Companion, true);
                                        }
                                        break;
                                    }
                                #endregion
   
                                #region MonkSpells
                                //TyrantAura
                                case 10395:
                                    {
                                        if (CanUseSpell(spell, attacker.Owner))
                                        {
                                            SpellUse suse = new SpellUse(true);
                                            suse.Attacker = attacker.UID;
                                            suse.SpellID = spell.ID;
                                            suse.SpellLevel = spell.Level;
                                            suse.SpellLevel = attacker.Owner.Spells[SpellID].Level;
                                            suse.X = X;
                                            suse.Y = Y;
                                            if (attacker.Owner.Team != null)
                                            {
                                                PrepareSpell(spell, attacker.Owner);
                                                foreach (Client.GameState teammate in attacker.Owner.Team.Teammates)
                                                {
                                                    if (ServerBase.Kernel.GetDistance(attacker.X, attacker.Y, teammate.Entity.X, teammate.Entity.Y) <= spell.Distance)
                                                        PrepareSpell(spell, attacker.Owner);
                                                    {
                                                    
                                                        if (teammate.Entity.ContainsFlag2(Update.Flags2.FendAura))//FendAura
                                                        {
                                                            teammate.Entity.RemoveFlag2(Update.Flags2.FendAura);//FendAura
                                                            teammate.Entity.Statistics.Immunity -= 200;
                                                        }
                                                        if (teammate.Entity.ContainsFlag2(Update.Flags2.MetalAura))//MetalAura
                                                        {
                                                            teammate.Entity.RemoveFlag2(Update.Flags2.MetalAura);//MetalAura
                                                            teammate.Entity.Statistics.MetalResistance -= 30;
                                                        }
                                                        if (teammate.Entity.ContainsFlag2(Update.Flags2.WoodAura))//WoodAura
                                                        {
                                                            teammate.Entity.RemoveFlag2(Update.Flags2.WoodAura);//WoodAura
                                                            teammate.Entity.Statistics.WoodResistance -= 30;
                                                        }
                                                        if (teammate.Entity.ContainsFlag2(Update.Flags2.WaterAura))//WaterAura
                                                        {
                                                            teammate.Entity.RemoveFlag2(Update.Flags2.WaterAura);//WaterAura
                                                            teammate.Entity.Statistics.WaterResistance -= 30;
                                                        }
                                                        if (teammate.Entity.ContainsFlag2(Update.Flags2.FireAura))//FireAura
                                                        {
                                                            teammate.Entity.RemoveFlag2(Update.Flags2.FireAura);//FireAura
                                                            teammate.Entity.Statistics.FireResistance -= 30;
                                                        }
                                                        if (teammate.Entity.ContainsFlag2(Update.Flags2.EarthAura))//EarthAura
                                                        {
                                                            teammate.Entity.RemoveFlag2(Update.Flags2.EarthAura);//EarthAura
                                                            teammate.Entity.Statistics.EarthResistance -= 30;
                                                        }
                                                        if (teammate.Entity.ContainsFlag2(Update.Flags2.TyrantAura))//TyrantAura
                                                        {
                                                            teammate.Entity.RemoveFlag2(Update.Flags2.TyrantAura);//TyrantAura
                                                            teammate.Entity.Statistics.CriticalStrike -= 200;
                                                        }
                                                       
                                                        teammate.Entity.AddFlag2(Update.Flags2.TyrantAura);
                                                        teammate.Entity.Statistics.CriticalStrike += 200;
                                                        suse.Targets.Add(teammate.Entity.UID, 1);
                                                        teammate.Entity.TyrantAura = Time32.Now;
                                                        teammate.Entity.TyrantAuras = (short)spell.Duration;
                                                     
                                                    }
                                                }
                                                if (attacked.EntityFlag == EntityFlag.Player)
                                                    attacked.Owner.SendScreen(suse, true);
                                                else
                                                    attacked.MonsterInfo.SendScreen(suse);
                                            }
                                            else
                                            {
                                                if (attacked == null)
                                                    return;
                                                if (ServerBase.Kernel.GetDistance(attacker.X, attacker.Y, attacked.X, attacked.Y) <= spell.Distance)
                                                {
                                                    PrepareSpell(spell, attacker.Owner);
                                                    if (attacker.ContainsFlag2(Update.Flags2.FendAura))//FendAura
                                                    {
                                                        attacker.RemoveFlag2(Update.Flags2.FendAura);//FendAura
                                                        attacker.Statistics.Immunity -= 200;
                                                    }
                                                    if (attacker.ContainsFlag2(Update.Flags2.MetalAura))//MetalAura
                                                    {
                                                        attacker.RemoveFlag2(Update.Flags2.MetalAura);//MetalAura
                                                        attacker.Statistics.MetalResistance -= 30;
                                                    }
                                                    if (attacker.ContainsFlag2(Update.Flags2.WoodAura))//WoodAura
                                                    {
                                                        attacker.Statistics.WoodResistance -= 30;
                                                        attacker.RemoveFlag2(Update.Flags2.WoodAura);//WoodAura
                                                    }
                                                    if (attacker.ContainsFlag2(Update.Flags2.WaterAura))//WaterAura
                                                    {
                                                        attacker.Statistics.WaterResistance -= 30;
                                                        attacker.RemoveFlag2(Update.Flags2.WaterAura);//WaterAura
                                                    }
                                                    if (attacker.ContainsFlag2(Update.Flags2.FireAura))//FireAura
                                                    {
                                                        attacker.Statistics.FireResistance -= 30;
                                                        attacker.RemoveFlag2(Update.Flags2.FireAura);//FireAura
                                                    }
                                                    if (attacker.ContainsFlag2(Update.Flags2.EarthAura))//EarthAura
                                                    {
                                                        attacker.Statistics.EarthResistance -= 30;
                                                        attacker.RemoveFlag2(Update.Flags2.EarthAura);//EarthAura
                                                    }
                                                    if (attacked.ContainsFlag2(Update.Flags2.TyrantAura))//TyrantAura
                                                    {
                                                        attacked.RemoveFlag2(Update.Flags2.TyrantAura);//TyrantAura
                                                        attacked.Statistics.CriticalStrike -= 200;

                                                    }
                                                    attacker.AddFlag2(Update.Flags2.TyrantAura);
                                                    attacker.Statistics.CriticalStrike += 200;
                                                    attacker.TyrantAura = Time32.Now;
                                                    attacker.TyrantAuras = (short)spell.Duration;
                                                    suse.Targets.Add(attacked.UID, 1);
                                                    if (attacked.EntityFlag == EntityFlag.Player)
                                                        attacked.Owner.SendScreen(suse, true);
                                                    else
                                                        attacked.MonsterInfo.SendScreen(suse);
                                                }
                                                else
                                                {
                                                    attacker.AttackPacket = null;
                                                }
                                            }
                                        }
                                        attacker.AttackPacket = null;
                                        break;
                                    }
                                //FendAura
                                case 10410:
                                    {

                                        if (CanUseSpell(spell, attacker.Owner))
                                        {
                                            SpellUse suse = new SpellUse(true);
                                            suse.Attacker = attacker.UID;
                                            suse.SpellID = spell.ID;
                                            suse.SpellLevel = spell.Level;
                                            suse.SpellLevel = attacker.Owner.Spells[SpellID].Level;
                                            suse.X = X;
                                            suse.Y = Y;
                                            if (attacker.Owner.Team != null)
                                            {
                                                PrepareSpell(spell, attacker.Owner);
                                                foreach (Client.GameState teammate in attacker.Owner.Team.Teammates)
                                                {
                                                    if (ServerBase.Kernel.GetDistance(attacker.X, attacker.Y, teammate.Entity.X, teammate.Entity.Y) <= spell.Distance)
                                                        PrepareSpell(spell, attacker.Owner);
                                                    {

                                                        if (teammate.Entity.ContainsFlag2(Update.Flags2.TyrantAura))//FendAura
                                                        {
                                                            teammate.Entity.RemoveFlag2(Update.Flags2.TyrantAura);//FendAura
                                                            teammate.Entity.Statistics.CriticalStrike -= 200;
                                                        }
                                                        if (teammate.Entity.ContainsFlag2(Update.Flags2.MetalAura))//MetalAura
                                                        {
                                                            teammate.Entity.RemoveFlag2(Update.Flags2.MetalAura);//MetalAura
                                                            teammate.Entity.Statistics.MetalResistance -= 30;
                                                        }
                                                        if (teammate.Entity.ContainsFlag2(Update.Flags2.WoodAura))//WoodAura
                                                        {
                                                            teammate.Entity.RemoveFlag2(Update.Flags2.WoodAura);//WoodAura
                                                            teammate.Entity.Statistics.WoodResistance -= 30;
                                                        }
                                                        if (teammate.Entity.ContainsFlag2(Update.Flags2.WaterAura))//WaterAura
                                                        {
                                                            teammate.Entity.RemoveFlag2(Update.Flags2.WaterAura);//WaterAura
                                                            teammate.Entity.Statistics.WaterResistance -= 30;
                                                        }
                                                        if (teammate.Entity.ContainsFlag2(Update.Flags2.FireAura))//FireAura
                                                        {
                                                            teammate.Entity.RemoveFlag2(Update.Flags2.FireAura);//FireAura
                                                            teammate.Entity.Statistics.FireResistance -= 30;
                                                        }
                                                        if (teammate.Entity.ContainsFlag2(Update.Flags2.EarthAura))//EarthAura
                                                        {
                                                            teammate.Entity.RemoveFlag2(Update.Flags2.EarthAura);//EarthAura
                                                            teammate.Entity.Statistics.EarthResistance -= 30;
                                                        }
                                                        if (teammate.Entity.ContainsFlag2(Update.Flags2.FendAura))//TyrantAura
                                                        {
                                                            teammate.Entity.RemoveFlag2(Update.Flags2.FendAura);//TyrantAura
                                                            teammate.Entity.Statistics.Immunity -= 200;
                                                        }

                                                        teammate.Entity.AddFlag2(Update.Flags2.FendAura);
                                                        teammate.Entity.Statistics.Immunity += 200;
                                                        suse.Targets.Add(teammate.Entity.UID, 1);
                                                        teammate.Entity.TyrantAura = Time32.Now;
                                                        teammate.Entity.TyrantAuras = (short)spell.Duration;

                                                    }
                                                }
                                                if (attacked.EntityFlag == EntityFlag.Player)
                                                    attacked.Owner.SendScreen(suse, true);
                                                else
                                                    attacked.MonsterInfo.SendScreen(suse);
                                            }
                                            else
                                            {
                                                if (attacked == null)
                                                    return;
                                                if (ServerBase.Kernel.GetDistance(attacker.X, attacker.Y, attacked.X, attacked.Y) <= spell.Distance)
                                                {
                                                    PrepareSpell(spell, attacker.Owner);
                                                    if (attacked.ContainsFlag2(Update.Flags2.TyrantAura))//FendAura
                                                    {
                                                        attacked.RemoveFlag2(Update.Flags2.TyrantAura);//FendAura
                                                        attacked.Statistics.CriticalStrike -= 200;
                                                    }
                                                    if (attacker.ContainsFlag2(Update.Flags2.MetalAura))//MetalAura
                                                    {
                                                        attacker.RemoveFlag2(Update.Flags2.MetalAura);//MetalAura
                                                        attacker.Statistics.MetalResistance -= 30;
                                                    }
                                                    if (attacker.ContainsFlag2(Update.Flags2.WoodAura))//WoodAura
                                                    {
                                                        attacker.Statistics.WoodResistance -= 30;
                                                        attacker.RemoveFlag2(Update.Flags2.WoodAura);//WoodAura
                                                    }
                                                    if (attacker.ContainsFlag2(Update.Flags2.WaterAura))//WaterAura
                                                    {
                                                        attacker.Statistics.WaterResistance -= 30;
                                                        attacker.RemoveFlag2(Update.Flags2.WaterAura);//WaterAura
                                                    }
                                                    if (attacker.ContainsFlag2(Update.Flags2.FireAura))//FireAura
                                                    {
                                                        attacker.Statistics.FireResistance -= 30;
                                                        attacker.RemoveFlag2(Update.Flags2.FireAura);//FireAura
                                                    }
                                                    if (attacker.ContainsFlag2(Update.Flags2.EarthAura))//EarthAura
                                                    {
                                                        attacker.Statistics.EarthResistance -= 30;
                                                        attacker.RemoveFlag2(Update.Flags2.EarthAura);//EarthAura
                                                    }
                                                    if (attacker.ContainsFlag2(Update.Flags2.FendAura))//TyrantAura
                                                    {
                                                        attacker.RemoveFlag2(Update.Flags2.FendAura);//TyrantAura
                                                        attacker.Statistics.Immunity += 200;

                                                    }
                                                    attacker.AddFlag2(Update.Flags2.FendAura);
                                                    attacker.Statistics.Immunity += 200;
                                                    attacker.TyrantAura = Time32.Now;
                                                    attacker.TyrantAuras = (short)spell.Duration;
                                                    suse.Targets.Add(attacked.UID, 1);
                                                    if (attacked.EntityFlag == EntityFlag.Player)
                                                        attacked.Owner.SendScreen(suse, true);
                                                    else
                                                        attacked.MonsterInfo.SendScreen(suse);
                                                }
                                                else
                                                {
                                                    attacker.AttackPacket = null;
                                                }
                                            }
                                        }
                                        attacker.AttackPacket = null;
                                        break;
                                    }
                                //ElementalAuraMetal
                                case 10420:
                                    {
                                        if (CanUseSpell(spell, attacker.Owner))
                                        {
                                            SpellUse suse = new SpellUse(true);
                                            suse.Attacker = attacker.UID;
                                            suse.SpellID = spell.ID;
                                            suse.SpellLevel = spell.Level;
                                            suse.SpellLevel = attacker.Owner.Spells[SpellID].Level;
                                            suse.X = X;
                                            suse.Y = Y;
                                            if (attacker.Owner.Team != null)
                                            {
                                                PrepareSpell(spell, attacker.Owner);
                                                foreach (Client.GameState teammate in attacker.Owner.Team.Teammates)
                                                {
                                                    if (ServerBase.Kernel.GetDistance(attacker.X, attacker.Y, teammate.Entity.X, teammate.Entity.Y) <= spell.Distance)
                                                        PrepareSpell(spell, attacker.Owner);
                                                    {

                                                        if (teammate.Entity.ContainsFlag2(Update.Flags2.TyrantAura))//FendAura
                                                        {
                                                            teammate.Entity.RemoveFlag2(Update.Flags2.TyrantAura);//FendAura
                                                            teammate.Entity.Statistics.CriticalStrike -= 200;
                                                        }
                                                        if (teammate.Entity.ContainsFlag2(Update.Flags2.MetalAura))//MetalAura
                                                        {
                                                            teammate.Entity.RemoveFlag2(Update.Flags2.MetalAura);//MetalAura
                                                            teammate.Entity.Statistics.MetalResistance -= 30;
                                                        }
                                                        if (teammate.Entity.ContainsFlag2(Update.Flags2.WoodAura))//WoodAura
                                                        {
                                                            teammate.Entity.RemoveFlag2(Update.Flags2.WoodAura);//WoodAura
                                                            teammate.Entity.Statistics.WoodResistance -= 30;
                                                        }
                                                        if (teammate.Entity.ContainsFlag2(Update.Flags2.WaterAura))//WaterAura
                                                        {
                                                            teammate.Entity.RemoveFlag2(Update.Flags2.WaterAura);//WaterAura
                                                            teammate.Entity.Statistics.WaterResistance -= 30;
                                                        }
                                                        if (teammate.Entity.ContainsFlag2(Update.Flags2.FireAura))//FireAura
                                                        {
                                                            teammate.Entity.RemoveFlag2(Update.Flags2.FireAura);//FireAura
                                                            teammate.Entity.Statistics.FireResistance -= 30;
                                                        }
                                                        if (teammate.Entity.ContainsFlag2(Update.Flags2.EarthAura))//EarthAura
                                                        {
                                                            teammate.Entity.RemoveFlag2(Update.Flags2.EarthAura);//EarthAura
                                                            teammate.Entity.Statistics.EarthResistance -= 30;
                                                        }
                                                        if (teammate.Entity.ContainsFlag2(Update.Flags2.FendAura))//TyrantAura
                                                        {
                                                            teammate.Entity.RemoveFlag2(Update.Flags2.FendAura);//TyrantAura
                                                            teammate.Entity.Statistics.Immunity -= 200;
                                                        }

                                                        teammate.Entity.AddFlag2(Update.Flags2.MetalAura);
                                                        teammate.Entity.Statistics.MetalResistance += 30;
                                                        suse.Targets.Add(teammate.Entity.UID, 1);
                                                        teammate.Entity.TyrantAura = Time32.Now;
                                                        teammate.Entity.TyrantAuras = (short)spell.Duration;

                                                    }
                                                }
                                                if (attacked.EntityFlag == EntityFlag.Player)
                                                    attacked.Owner.SendScreen(suse, true);
                                                else
                                                    attacked.MonsterInfo.SendScreen(suse);
                                            }
                                            else
                                            {
                                                if (attacked == null)
                                                    return;
                                                if (ServerBase.Kernel.GetDistance(attacker.X, attacker.Y, attacked.X, attacked.Y) <= spell.Distance)
                                                {
                                                    PrepareSpell(spell, attacker.Owner);
                                                    if (attacker.ContainsFlag2(Update.Flags2.TyrantAura))//FendAura
                                                    {
                                                        attacker.RemoveFlag2(Update.Flags2.TyrantAura);//FendAura
                                                        attacker.Statistics.CriticalStrike -= 200;
                                                    }
                                                    if (attacked.ContainsFlag2(Update.Flags2.MetalAura))//MetalAura
                                                    {
                                                        attacked.RemoveFlag2(Update.Flags2.MetalAura);//MetalAura
                                                        attacked.Statistics.MetalResistance -= 30;
                                                    }
                                                    if (attacker.ContainsFlag2(Update.Flags2.WoodAura))//WoodAura
                                                    {
                                                        attacker.Statistics.WoodResistance -= 30;
                                                        attacker.RemoveFlag2(Update.Flags2.WoodAura);//WoodAura
                                                    }
                                                    if (attacker.ContainsFlag2(Update.Flags2.WaterAura))//WaterAura
                                                    {
                                                        attacker.Statistics.WaterResistance -= 30;
                                                        attacker.RemoveFlag2(Update.Flags2.WaterAura);//WaterAura
                                                    }
                                                    if (attacker.ContainsFlag2(Update.Flags2.FireAura))//FireAura
                                                    {
                                                        attacker.Statistics.FireResistance -= 30;
                                                        attacker.RemoveFlag2(Update.Flags2.FireAura);//FireAura
                                                    }
                                                    if (attacker.ContainsFlag2(Update.Flags2.EarthAura))//EarthAura
                                                    {
                                                        attacker.Statistics.EarthResistance -= 30;
                                                        attacker.RemoveFlag2(Update.Flags2.EarthAura);//EarthAura
                                                    }
                                                    if (attacker.ContainsFlag2(Update.Flags2.FendAura))//TyrantAura
                                                    {
                                                        attacker.RemoveFlag2(Update.Flags2.FendAura);//TyrantAura
                                                        attacker.Statistics.Immunity -= 200;

                                                    }
                                                    attacker.AddFlag2(Update.Flags2.MetalAura);
                                                    attacker.Statistics.MetalResistance += 30;
                                                    attacker.TyrantAura = Time32.Now;
                                                    attacker.TyrantAuras = (short)spell.Duration;
                                                    suse.Targets.Add(attacked.UID, 1);
                                                    if (attacked.EntityFlag == EntityFlag.Player)
                                                        attacked.Owner.SendScreen(suse, true);
                                                    else
                                                        attacked.MonsterInfo.SendScreen(suse);
                                                }
                                                else
                                                {
                                                    attacker.AttackPacket = null;
                                                }
                                            }
                                        }
                                        attacker.AttackPacket = null;
                                        break;
                                    }
                                //ElementalAuraWood
                                case 10421:
                                    {
                                        if (CanUseSpell(spell, attacker.Owner))
                                        {
                                            SpellUse suse = new SpellUse(true);
                                            suse.Attacker = attacker.UID;
                                            suse.SpellID = spell.ID;
                                            suse.SpellLevel = spell.Level;
                                            suse.SpellLevel = attacker.Owner.Spells[SpellID].Level;
                                            suse.X = X;
                                            suse.Y = Y;
                                            if (attacker.Owner.Team != null)
                                            {
                                                PrepareSpell(spell, attacker.Owner);
                                                foreach (Client.GameState teammate in attacker.Owner.Team.Teammates)
                                                {
                                                    if (ServerBase.Kernel.GetDistance(attacker.X, attacker.Y, teammate.Entity.X, teammate.Entity.Y) <= spell.Distance)
                                                        PrepareSpell(spell, attacker.Owner);
                                                    {

                                                        if (teammate.Entity.ContainsFlag2(Update.Flags2.TyrantAura))//FendAura
                                                        {
                                                            teammate.Entity.RemoveFlag2(Update.Flags2.TyrantAura);//FendAura
                                                            teammate.Entity.Statistics.CriticalStrike -= 200;
                                                        }
                                                        if (teammate.Entity.ContainsFlag2(Update.Flags2.MetalAura))//MetalAura
                                                        {
                                                            teammate.Entity.RemoveFlag2(Update.Flags2.MetalAura);//MetalAura
                                                            teammate.Entity.Statistics.MetalResistance -= 30;
                                                        }
                                                        if (teammate.Entity.ContainsFlag2(Update.Flags2.WoodAura))//WoodAura
                                                        {
                                                            teammate.Entity.RemoveFlag2(Update.Flags2.WoodAura);//WoodAura
                                                            teammate.Entity.Statistics.WoodResistance -= 30;
                                                        }
                                                        if (teammate.Entity.ContainsFlag2(Update.Flags2.WaterAura))//WaterAura
                                                        {
                                                            teammate.Entity.RemoveFlag2(Update.Flags2.WaterAura);//WaterAura
                                                            teammate.Entity.Statistics.WaterResistance -= 30;
                                                        }
                                                        if (teammate.Entity.ContainsFlag2(Update.Flags2.FireAura))//FireAura
                                                        {
                                                            teammate.Entity.RemoveFlag2(Update.Flags2.FireAura);//FireAura
                                                            teammate.Entity.Statistics.FireResistance -= 30;
                                                        }
                                                        if (teammate.Entity.ContainsFlag2(Update.Flags2.EarthAura))//EarthAura
                                                        {
                                                            teammate.Entity.RemoveFlag2(Update.Flags2.EarthAura);//EarthAura
                                                            teammate.Entity.Statistics.EarthResistance -= 30;
                                                        }
                                                        if (teammate.Entity.ContainsFlag2(Update.Flags2.FendAura))//TyrantAura
                                                        {
                                                            teammate.Entity.RemoveFlag2(Update.Flags2.FendAura);//TyrantAura
                                                            teammate.Entity.Statistics.Immunity -= 200;
                                                        }

                                                        teammate.Entity.AddFlag2(Update.Flags2.WoodAura);
                                                        teammate.Entity.Statistics.WoodResistance += 30;
                                                        suse.Targets.Add(teammate.Entity.UID, 1);
                                                        teammate.Entity.TyrantAura = Time32.Now;
                                                        teammate.Entity.TyrantAuras = (short)spell.Duration;

                                                    }
                                                }
                                                if (attacked.EntityFlag == EntityFlag.Player)
                                                    attacked.Owner.SendScreen(suse, true);
                                                else
                                                    attacked.MonsterInfo.SendScreen(suse);
                                            }
                                            else
                                            {
                                                if (attacked == null)
                                                    return;
                                                if (ServerBase.Kernel.GetDistance(attacker.X, attacker.Y, attacked.X, attacked.Y) <= spell.Distance)
                                                {
                                                    PrepareSpell(spell, attacker.Owner);
                                                    if (attacker.ContainsFlag2(Update.Flags2.TyrantAura))//FendAura
                                                    {
                                                        attacker.RemoveFlag2(Update.Flags2.TyrantAura);//FendAura
                                                        attacker.Statistics.CriticalStrike -= 200;
                                                    }
                                                    if (attacker.ContainsFlag2(Update.Flags2.MetalAura))//MetalAura
                                                    {
                                                        attacker.RemoveFlag2(Update.Flags2.MetalAura);//MetalAura
                                                        attacker.Statistics.MetalResistance -= 30;
                                                    }
                                                    if (attacked.ContainsFlag2(Update.Flags2.WoodAura))//WoodAura
                                                    {
                                                        attacked.Statistics.WoodResistance -= 30;
                                                        attacked.RemoveFlag2(Update.Flags2.WoodAura);//WoodAura
                                                    }
                                                    if (attacker.ContainsFlag2(Update.Flags2.WaterAura))//WaterAura
                                                    {
                                                        attacker.Statistics.WaterResistance -= 30;
                                                        attacker.RemoveFlag2(Update.Flags2.WaterAura);//WaterAura
                                                    }
                                                    if (attacker.ContainsFlag2(Update.Flags2.FireAura))//FireAura
                                                    {
                                                        attacker.Statistics.FireResistance -= 30;
                                                        attacker.RemoveFlag2(Update.Flags2.FireAura);//FireAura
                                                    }
                                                    if (attacker.ContainsFlag2(Update.Flags2.EarthAura))//EarthAura
                                                    {
                                                        attacker.Statistics.EarthResistance -= 30;
                                                        attacker.RemoveFlag2(Update.Flags2.EarthAura);//EarthAura
                                                    }
                                                    if (attacker.ContainsFlag2(Update.Flags2.FendAura))//TyrantAura
                                                    {
                                                        attacker.RemoveFlag2(Update.Flags2.FendAura);//TyrantAura
                                                        attacker.Statistics.Immunity -= 200;

                                                    }
                                                    attacker.AddFlag2(Update.Flags2.WoodAura);
                                                    attacker.Statistics.WoodResistance += 30;
                                                    attacker.TyrantAura = Time32.Now;
                                                    attacker.TyrantAuras = (short)spell.Duration;
                                                    suse.Targets.Add(attacked.UID, 1);
                                                    if (attacked.EntityFlag == EntityFlag.Player)
                                                        attacked.Owner.SendScreen(suse, true);
                                                    else
                                                        attacked.MonsterInfo.SendScreen(suse);
                                                }
                                                else
                                                {
                                                    attacker.AttackPacket = null;
                                                }
                                            }
                                        }
                                        attacker.AttackPacket = null;
                                        break;
                                    }
                                //ElementalAuraWater
                                case 10422:
                                    {
                                        if (CanUseSpell(spell, attacker.Owner))
                                        {
                                            SpellUse suse = new SpellUse(true);
                                            suse.Attacker = attacker.UID;
                                            suse.SpellID = spell.ID;
                                            suse.SpellLevel = spell.Level;
                                            suse.SpellLevel = attacker.Owner.Spells[SpellID].Level;
                                            suse.X = X;
                                            suse.Y = Y;
                                            if (attacker.Owner.Team != null)
                                            {
                                                PrepareSpell(spell, attacker.Owner);
                                                foreach (Client.GameState teammate in attacker.Owner.Team.Teammates)
                                                {
                                                    if (ServerBase.Kernel.GetDistance(attacker.X, attacker.Y, teammate.Entity.X, teammate.Entity.Y) <= spell.Distance)
                                                        PrepareSpell(spell, attacker.Owner);
                                                    {

                                                        if (teammate.Entity.ContainsFlag2(Update.Flags2.TyrantAura))//FendAura
                                                        {
                                                            teammate.Entity.RemoveFlag2(Update.Flags2.TyrantAura);//FendAura
                                                            teammate.Entity.Statistics.CriticalStrike -= 200;
                                                        }
                                                        if (teammate.Entity.ContainsFlag2(Update.Flags2.MetalAura))//MetalAura
                                                        {
                                                            teammate.Entity.RemoveFlag2(Update.Flags2.MetalAura);//MetalAura
                                                            teammate.Entity.Statistics.MetalResistance -= 30;
                                                        }
                                                        if (teammate.Entity.ContainsFlag2(Update.Flags2.WoodAura))//WoodAura
                                                        {
                                                            teammate.Entity.RemoveFlag2(Update.Flags2.WoodAura);//WoodAura
                                                            teammate.Entity.Statistics.WoodResistance -= 30;
                                                        }
                                                        if (teammate.Entity.ContainsFlag2(Update.Flags2.WaterAura))//WaterAura
                                                        {
                                                            teammate.Entity.RemoveFlag2(Update.Flags2.WaterAura);//WaterAura
                                                            teammate.Entity.Statistics.WaterResistance -= 30;
                                                        }
                                                        if (teammate.Entity.ContainsFlag2(Update.Flags2.FireAura))//FireAura
                                                        {
                                                            teammate.Entity.RemoveFlag2(Update.Flags2.FireAura);//FireAura
                                                            teammate.Entity.Statistics.FireResistance -= 30;
                                                        }
                                                        if (teammate.Entity.ContainsFlag2(Update.Flags2.EarthAura))//EarthAura
                                                        {
                                                            teammate.Entity.RemoveFlag2(Update.Flags2.EarthAura);//EarthAura
                                                            teammate.Entity.Statistics.EarthResistance -= 30;
                                                        }
                                                        if (teammate.Entity.ContainsFlag2(Update.Flags2.FendAura))//TyrantAura
                                                        {
                                                            teammate.Entity.RemoveFlag2(Update.Flags2.FendAura);//TyrantAura
                                                            teammate.Entity.Statistics.Immunity -= 200;
                                                        }

                                                        teammate.Entity.AddFlag2(Update.Flags2.WaterAura);
                                                        teammate.Entity.Statistics.WaterResistance += 30;
                                                        suse.Targets.Add(teammate.Entity.UID, 1);
                                                        teammate.Entity.TyrantAura = Time32.Now;
                                                        teammate.Entity.TyrantAuras = (short)spell.Duration;

                                                    }
                                                }
                                                if (attacked.EntityFlag == EntityFlag.Player)
                                                    attacked.Owner.SendScreen(suse, true);
                                                else
                                                    attacked.MonsterInfo.SendScreen(suse);
                                            }
                                            else
                                            {
                                                if (attacked == null)
                                                    return;
                                                if (ServerBase.Kernel.GetDistance(attacker.X, attacker.Y, attacked.X, attacked.Y) <= spell.Distance)
                                                {
                                                    PrepareSpell(spell, attacker.Owner);
                                                    if (attacker.ContainsFlag2(Update.Flags2.TyrantAura))//FendAura
                                                    {
                                                        attacker.RemoveFlag2(Update.Flags2.TyrantAura);//FendAura
                                                        attacker.Statistics.CriticalStrike -= 200;
                                                    }
                                                    if (attacker.ContainsFlag2(Update.Flags2.MetalAura))//MetalAura
                                                    {
                                                        attacker.RemoveFlag2(Update.Flags2.MetalAura);//MetalAura
                                                        attacker.Statistics.MetalResistance -= 30;
                                                    }
                                                    if (attacker.ContainsFlag2(Update.Flags2.WoodAura))//WoodAura
                                                    {
                                                        attacker.Statistics.WoodResistance -= 30;
                                                        attacker.RemoveFlag2(Update.Flags2.WoodAura);//WoodAura
                                                    }
                                                    if (attacked.ContainsFlag2(Update.Flags2.WaterAura))//WaterAura
                                                    {
                                                        attacked.Statistics.WaterResistance -= 30;
                                                        attacked.RemoveFlag2(Update.Flags2.WaterAura);//WaterAura
                                                    }
                                                    if (attacker.ContainsFlag2(Update.Flags2.FireAura))//FireAura
                                                    {
                                                        attacker.Statistics.FireResistance -= 30;
                                                        attacker.RemoveFlag2(Update.Flags2.FireAura);//FireAura
                                                    }
                                                    if (attacker.ContainsFlag2(Update.Flags2.EarthAura))//EarthAura
                                                    {
                                                        attacker.Statistics.EarthResistance -= 30;
                                                        attacker.RemoveFlag2(Update.Flags2.EarthAura);//EarthAura
                                                    }
                                                    if (attacker.ContainsFlag2(Update.Flags2.FendAura))//TyrantAura
                                                    {
                                                        attacker.RemoveFlag2(Update.Flags2.FendAura);//TyrantAura
                                                        attacker.Statistics.Immunity -= 200;

                                                    }
                                                    attacker.AddFlag2(Update.Flags2.WaterAura);
                                                    attacker.Statistics.WaterResistance += 30;
                                                    attacker.TyrantAura = Time32.Now;
                                                    attacker.TyrantAuras = (short)spell.Duration;
                                                    suse.Targets.Add(attacked.UID, 1);
                                                    if (attacked.EntityFlag == EntityFlag.Player)
                                                        attacked.Owner.SendScreen(suse, true);
                                                    else
                                                        attacked.MonsterInfo.SendScreen(suse);
                                                }
                                                else
                                                {
                                                    attacker.AttackPacket = null;
                                                }
                                            }
                                        }
                                        attacker.AttackPacket = null;
                                        break;
                                    }
                                //ElementalAuraFire
                                case 10423:
                                    {
                                        if (CanUseSpell(spell, attacker.Owner))
                                        {
                                            SpellUse suse = new SpellUse(true);
                                            suse.Attacker = attacker.UID;
                                            suse.SpellID = spell.ID;
                                            suse.SpellLevel = spell.Level;
                                            suse.SpellLevel = attacker.Owner.Spells[SpellID].Level;
                                            suse.X = X;
                                            suse.Y = Y;
                                            if (attacker.Owner.Team != null)
                                            {
                                                PrepareSpell(spell, attacker.Owner);
                                                foreach (Client.GameState teammate in attacker.Owner.Team.Teammates)
                                                {
                                                    if (ServerBase.Kernel.GetDistance(attacker.X, attacker.Y, teammate.Entity.X, teammate.Entity.Y) <= spell.Distance)
                                                        PrepareSpell(spell, attacker.Owner);
                                                    {

                                                        if (teammate.Entity.ContainsFlag2(Update.Flags2.TyrantAura))//FendAura
                                                        {
                                                            teammate.Entity.RemoveFlag2(Update.Flags2.TyrantAura);//FendAura
                                                            teammate.Entity.Statistics.CriticalStrike -= 200;
                                                        }
                                                        if (teammate.Entity.ContainsFlag2(Update.Flags2.MetalAura))//MetalAura
                                                        {
                                                            teammate.Entity.RemoveFlag2(Update.Flags2.MetalAura);//MetalAura
                                                            teammate.Entity.Statistics.MetalResistance -= 30;
                                                        }
                                                        if (teammate.Entity.ContainsFlag2(Update.Flags2.WoodAura))//WoodAura
                                                        {
                                                            teammate.Entity.RemoveFlag2(Update.Flags2.WoodAura);//WoodAura
                                                            teammate.Entity.Statistics.WoodResistance -= 30;
                                                        }
                                                        if (teammate.Entity.ContainsFlag2(Update.Flags2.WaterAura))//WaterAura
                                                        {
                                                            teammate.Entity.RemoveFlag2(Update.Flags2.WaterAura);//WaterAura
                                                            teammate.Entity.Statistics.WaterResistance -= 30;
                                                        }
                                                        if (teammate.Entity.ContainsFlag2(Update.Flags2.FireAura))//FireAura
                                                        {
                                                            teammate.Entity.RemoveFlag2(Update.Flags2.FireAura);//FireAura
                                                            teammate.Entity.Statistics.FireResistance -= 30;
                                                        }
                                                        if (teammate.Entity.ContainsFlag2(Update.Flags2.EarthAura))//EarthAura
                                                        {
                                                            teammate.Entity.RemoveFlag2(Update.Flags2.EarthAura);//EarthAura
                                                            teammate.Entity.Statistics.EarthResistance -= 30;
                                                        }
                                                        if (teammate.Entity.ContainsFlag2(Update.Flags2.FendAura))//TyrantAura
                                                        {
                                                            teammate.Entity.RemoveFlag2(Update.Flags2.FendAura);//TyrantAura
                                                            teammate.Entity.Statistics.Immunity -= 200;
                                                        }

                                                        teammate.Entity.AddFlag2(Update.Flags2.FireAura);
                                                        teammate.Entity.Statistics.FireResistance += 30;
                                                        suse.Targets.Add(teammate.Entity.UID, 1);
                                                        teammate.Entity.TyrantAura = Time32.Now;
                                                        teammate.Entity.TyrantAuras = (short)spell.Duration;

                                                    }
                                                }
                                                if (attacked.EntityFlag == EntityFlag.Player)
                                                    attacked.Owner.SendScreen(suse, true);
                                                else
                                                    attacked.MonsterInfo.SendScreen(suse);
                                            }
                                            else
                                            {
                                                if (attacked == null)
                                                    return;
                                                if (ServerBase.Kernel.GetDistance(attacker.X, attacker.Y, attacked.X, attacked.Y) <= spell.Distance)
                                                {
                                                    PrepareSpell(spell, attacker.Owner);
                                                    if (attacker.ContainsFlag2(Update.Flags2.TyrantAura))//FendAura
                                                    {
                                                        attacker.RemoveFlag2(Update.Flags2.TyrantAura);//FendAura
                                                        attacker.Statistics.CriticalStrike -= 200;
                                                    }
                                                    if (attacker.ContainsFlag2(Update.Flags2.MetalAura))//MetalAura
                                                    {
                                                        attacker.RemoveFlag2(Update.Flags2.MetalAura);//MetalAura
                                                        attacker.Statistics.MetalResistance -= 30;
                                                    }
                                                    if (attacker.ContainsFlag2(Update.Flags2.WoodAura))//WoodAura
                                                    {
                                                        attacker.Statistics.WoodResistance -= 30;
                                                        attacker.RemoveFlag2(Update.Flags2.WoodAura);//WoodAura
                                                    }
                                                    if (attacker.ContainsFlag2(Update.Flags2.WaterAura))//WaterAura
                                                    {
                                                        attacker.Statistics.WaterResistance -= 30;
                                                        attacker.RemoveFlag2(Update.Flags2.WaterAura);//WaterAura
                                                    }
                                                    if (attacked.ContainsFlag2(Update.Flags2.FireAura))//FireAura
                                                    {
                                                        attacked.Statistics.FireResistance -= 30;
                                                        attacked.RemoveFlag2(Update.Flags2.FireAura);//FireAura
                                                    }
                                                    if (attacker.ContainsFlag2(Update.Flags2.EarthAura))//EarthAura
                                                    {
                                                        attacker.Statistics.EarthResistance -= 30;
                                                        attacker.RemoveFlag2(Update.Flags2.EarthAura);//EarthAura
                                                    }
                                                    if (attacker.ContainsFlag2(Update.Flags2.FendAura))//TyrantAura
                                                    {
                                                        attacker.RemoveFlag2(Update.Flags2.FendAura);//TyrantAura
                                                        attacker.Statistics.Immunity -= 200;

                                                    }
                                                    attacker.AddFlag2(Update.Flags2.FireAura);
                                                    attacker.Statistics.FireResistance += 30;
                                                    attacker.TyrantAura = Time32.Now;
                                                    attacker.TyrantAuras = (short)spell.Duration;
                                                    suse.Targets.Add(attacked.UID, 1);
                                                    if (attacked.EntityFlag == EntityFlag.Player)
                                                        attacked.Owner.SendScreen(suse, true);
                                                    else
                                                        attacked.MonsterInfo.SendScreen(suse);
                                                }
                                                else
                                                {
                                                    attacker.AttackPacket = null;
                                                }
                                            }
                                        }
                                        attacker.AttackPacket = null;
                                        break;
                                    }
                                //ElementalEarthAura
                                case 10424:
                                    {
                                        if (CanUseSpell(spell, attacker.Owner))
                                        {
                                            SpellUse suse = new SpellUse(true);
                                            suse.Attacker = attacker.UID;
                                            suse.SpellID = spell.ID;
                                            suse.SpellLevel = spell.Level;
                                            suse.SpellLevel = attacker.Owner.Spells[SpellID].Level;
                                            suse.X = X;
                                            suse.Y = Y;
                                            if (attacker.Owner.Team != null)
                                            {
                                                PrepareSpell(spell, attacker.Owner);
                                                foreach (Client.GameState teammate in attacker.Owner.Team.Teammates)
                                                {
                                                    if (ServerBase.Kernel.GetDistance(attacker.X, attacker.Y, teammate.Entity.X, teammate.Entity.Y) <= spell.Distance)
                                                        PrepareSpell(spell, attacker.Owner);
                                                    {

                                                        if (teammate.Entity.ContainsFlag2(Update.Flags2.TyrantAura))//FendAura
                                                        {
                                                            teammate.Entity.RemoveFlag2(Update.Flags2.TyrantAura);//FendAura
                                                            teammate.Entity.Statistics.CriticalStrike -= 200;
                                                        }
                                                        if (teammate.Entity.ContainsFlag2(Update.Flags2.MetalAura))//MetalAura
                                                        {
                                                            teammate.Entity.RemoveFlag2(Update.Flags2.MetalAura);//MetalAura
                                                            teammate.Entity.Statistics.MetalResistance -= 30;
                                                        }
                                                        if (teammate.Entity.ContainsFlag2(Update.Flags2.WoodAura))//WoodAura
                                                        {
                                                            teammate.Entity.RemoveFlag2(Update.Flags2.WoodAura);//WoodAura
                                                            teammate.Entity.Statistics.WoodResistance -= 30;
                                                        }
                                                        if (teammate.Entity.ContainsFlag2(Update.Flags2.WaterAura))//WaterAura
                                                        {
                                                            teammate.Entity.RemoveFlag2(Update.Flags2.WaterAura);//WaterAura
                                                            teammate.Entity.Statistics.WaterResistance -= 30;
                                                        }
                                                        if (teammate.Entity.ContainsFlag2(Update.Flags2.FireAura))//FireAura
                                                        {
                                                            teammate.Entity.RemoveFlag2(Update.Flags2.FireAura);//FireAura
                                                            teammate.Entity.Statistics.FireResistance -= 30;
                                                        }
                                                        if (teammate.Entity.ContainsFlag2(Update.Flags2.EarthAura))//EarthAura
                                                        {
                                                            teammate.Entity.RemoveFlag2(Update.Flags2.EarthAura);//EarthAura
                                                            teammate.Entity.Statistics.EarthResistance -= 30;
                                                        }
                                                        if (teammate.Entity.ContainsFlag2(Update.Flags2.FendAura))//TyrantAura
                                                        {
                                                            teammate.Entity.RemoveFlag2(Update.Flags2.FendAura);//TyrantAura
                                                            teammate.Entity.Statistics.Immunity -= 200;
                                                        }

                                                        teammate.Entity.AddFlag2(Update.Flags2.EarthAura);
                                                        teammate.Entity.Statistics.EarthResistance += 30;
                                                        suse.Targets.Add(teammate.Entity.UID, 1);
                                                        teammate.Entity.TyrantAura = Time32.Now;
                                                        teammate.Entity.TyrantAuras = (short)spell.Duration;

                                                    }
                                                }
                                                if (attacked.EntityFlag == EntityFlag.Player)
                                                    attacked.Owner.SendScreen(suse, true);
                                                else
                                                    attacked.MonsterInfo.SendScreen(suse);
                                            }
                                            else
                                            {
                                                if (attacked == null)
                                                    return;
                                                if (ServerBase.Kernel.GetDistance(attacker.X, attacker.Y, attacked.X, attacked.Y) <= spell.Distance)
                                                {
                                                    PrepareSpell(spell, attacker.Owner);
                                                    if (attacker.ContainsFlag2(Update.Flags2.TyrantAura))//FendAura
                                                    {
                                                        attacker.RemoveFlag2(Update.Flags2.TyrantAura);//FendAura
                                                        attacker.Statistics.CriticalStrike -= 200;
                                                    }
                                                    if (attacker.ContainsFlag2(Update.Flags2.MetalAura))//MetalAura
                                                    {
                                                        attacker.RemoveFlag2(Update.Flags2.MetalAura);//MetalAura
                                                        attacker.Statistics.MetalResistance -= 30;
                                                    }
                                                    if (attacker.ContainsFlag2(Update.Flags2.WoodAura))//WoodAura
                                                    {
                                                        attacker.Statistics.WoodResistance -= 30;
                                                        attacker.RemoveFlag2(Update.Flags2.WoodAura);//WoodAura
                                                    }
                                                    if (attacker.ContainsFlag2(Update.Flags2.WaterAura))//WaterAura
                                                    {
                                                        attacker.Statistics.WaterResistance -= 30;
                                                        attacker.RemoveFlag2(Update.Flags2.WaterAura);//WaterAura
                                                    }
                                                    if (attacker.ContainsFlag2(Update.Flags2.FireAura))//FireAura
                                                    {
                                                        attacker.Statistics.FireResistance -= 30;
                                                        attacker.RemoveFlag2(Update.Flags2.FireAura);//FireAura
                                                    }
                                                    if (attacked.ContainsFlag2(Update.Flags2.EarthAura))//EarthAura
                                                    {
                                                        attacked.Statistics.EarthResistance -= 30;
                                                        attacked.RemoveFlag2(Update.Flags2.EarthAura);//EarthAura
                                                    }
                                                    if (attacker.ContainsFlag2(Update.Flags2.FendAura))//TyrantAura
                                                    {
                                                        attacker.RemoveFlag2(Update.Flags2.FendAura);//TyrantAura
                                                        attacker.Statistics.Immunity -= 200;

                                                    }
                                                    attacker.AddFlag2(Update.Flags2.EarthAura);
                                                    attacker.Statistics.EarthResistance += 30;
                                                    attacker.TyrantAura = Time32.Now;
                                                    attacker.TyrantAuras = (short)spell.Duration;
                                                    suse.Targets.Add(attacked.UID, 1);
                                                    if (attacked.EntityFlag == EntityFlag.Player)
                                                        attacked.Owner.SendScreen(suse, true);
                                                    else
                                                        attacked.MonsterInfo.SendScreen(suse);
                                                }
                                                else
                                                {
                                                    attacker.AttackPacket = null;
                                                }
                                            }
                                        }
                                        attacker.AttackPacket = null;
                                        break;
                                    }





                                //Compassion
                                case 10430:
                                    {
                                        if (CanUseSpell(spell, attacker.Owner))
                                        {
                                            SpellUse suse = new SpellUse(true);
                                            suse.Attacker = attacker.UID;
                                            suse.SpellID = spell.ID;
                                            suse.SpellLevel = spell.Level;
                                            suse.X = X;
                                            suse.Y = Y;

                                            if (attacker.Owner.Team != null)
                                            {
                                                PrepareSpell(spell, attacker.Owner);
                                                foreach (Client.GameState teammate in attacker.Owner.Team.Teammates)
                                                {
                                                    if (ServerBase.Kernel.GetDistance(attacker.X, attacker.Y, teammate.Entity.X, teammate.Entity.Y) <= spell.Distance)
                                                    {
                                                        attacker.RemoveFlag(Update.Flags.Poisoned);

                                                        suse.Targets.Add(teammate.Entity.UID, 1);
                                                    }
                                                }
                                                if (attacked.EntityFlag == EntityFlag.Player)
                                                    attacked.Owner.SendScreen(suse, true);
                                                else
                                                    attacked.MonsterInfo.SendScreen(suse);
                                            }
                                            else
                                            {
                                                if (attacked == null)
                                                    return;
                                                if (ServerBase.Kernel.GetDistance(attacker.X, attacker.Y, attacked.X, attacked.Y) <= spell.Distance)
                                                {
                                                    PrepareSpell(spell, attacker.Owner);

                                                    attacker.RemoveFlag(Update.Flags.Poisoned);

                                                    suse.Targets.Add(attacked.UID, 1);

                                                    if (attacked.EntityFlag == EntityFlag.Player)
                                                        attacked.Owner.SendScreen(suse, true);
                                                    else
                                                        attacked.MonsterInfo.SendScreen(suse);
                                                }
                                                else
                                                {
                                                    attacker.AttackPacket = null;
                                                }
                                            }
                                        }
                                        attacker.AttackPacket = null;
                                        break;
                                    }
                                //Serenity
                                case 10400:
                                    {
                                        if (CanUseSpell(spell, attacker.Owner))
                                        {
                                            PrepareSpell(spell, attacker.Owner);

                                            if (attacker == null) return;

                                            SpellUse suse = new SpellUse(true);
                                            suse.Attacker = attacker.UID;
                                            suse.SpellID = SpellID;
                                            suse.SpellLevel = attacker.Owner.Spells[SpellID].Level;
                                            suse.X = X;
                                            suse.Y = Y;

                                            suse.Targets.Add(attacker.UID, 1);

                                            attacker.ToxicFogLeft = 0;
                                            //attacker.RemoveFlag(Update.Flags.SoulShackle);
                                            attacker.Owner.SendScreen(suse, true);
                                        }
                                        attacker.AttackPacket = null;
                                        break;
                                    }
                                //Tranquility
                                case 10425:
                                    {
                                        if (CanUseSpell(spell, attacker.Owner))
                                        {
                                            SpellUse suse = new SpellUse(true);
                                            suse.Attacker = attacker.UID;
                                            suse.SpellID = spell.ID;
                                            suse.SpellLevel = spell.Level;
                                            suse.X = X;
                                            suse.Y = Y;

                                            if (attacker.Owner.Team != null)
                                            {
                                                PrepareSpell(spell, attacker.Owner);
                                                foreach (Client.GameState teammate in attacker.Owner.Team.Teammates)
                                                {
                                                    if (ServerBase.Kernel.GetDistance(attacker.X, attacker.Y, teammate.Entity.X, teammate.Entity.Y) <= spell.Distance)
                                                    {
                                                        attacker.RemoveFlag(Update.Flags.SoulShackle);
                                                        attacked.RemoveFlag(Update.Flags.SoulShackle);
                                                        teammate.Entity.RemoveFlag(Update.Flags.SoulShackle);
                                                        suse.Targets.Add(teammate.Entity.UID, 1);
                                                    }
                                                }
                                                if (attacked.EntityFlag == EntityFlag.Player)
                                                    attacked.Owner.SendScreen(suse, true);
                                                else
                                                    attacked.MonsterInfo.SendScreen(suse);
                                            }
                                            else
                                            {
                                                if (attacked == null)
                                                    return;
                                                if (ServerBase.Kernel.GetDistance(attacker.X, attacker.Y, attacked.X, attacked.Y) <= spell.Distance)
                                                {
                                                    PrepareSpell(spell, attacker.Owner);

                                                    attacker.RemoveFlag(Update.Flags.SoulShackle);

                                                    suse.Targets.Add(attacked.UID, 1);

                                                    if (attacked.EntityFlag == EntityFlag.Player)
                                                        attacked.Owner.SendScreen(suse, true);
                                                    else
                                                        attacked.MonsterInfo.SendScreen(suse);
                                                }
                                                else
                                                {
                                                    attacker.AttackPacket = null;
                                                }
                                            }
                                        }
                                        attacker.AttackPacket = null;
                                        break;
                                    }
                                //Radiant Palm 
                                case 10381:
                                    // case 10315:
                                    {
                                        if (CanUseSpell(spell, attacker.Owner))
                                        {
                                            PrepareSpell(spell, attacker.Owner);
                                            Game.Attacking.InLineAlgorithm ila = new Conquer_Online_Server.Game.Attacking.InLineAlgorithm(attacker.X,
                                        X, attacker.Y, Y, (byte)spell.Range, InLineAlgorithm.Algorithm.DDA);
                                            SpellUse suse = new SpellUse(true);
                                            suse.Attacker = attacker.UID;
                                            suse.SpellID = SpellID;
                                            suse.SpellLevel = attacker.Owner.Spells[SpellID].Level;
                                            suse.X = X;
                                            suse.Y = Y;
                                            for (int c = 0; c < attacker.Owner.Screen.Objects.Length; c++)
                                            {
                                                //For a multi threaded application, while we go through the collection
                                                //the collection might change. We will make sure that we wont go off  
                                                //the limits with a check.
                                                if (c >= attacker.Owner.Screen.Objects.Length)
                                                    break;
                                                Interfaces.IMapObject _obj = attacker.Owner.Screen.Objects[c];
                                                if (_obj == null)
                                                    continue;
                                                if (_obj.MapObjType == MapObjectType.Monster || _obj.MapObjType == MapObjectType.Player)
                                                {
                                                    attacked = _obj as Entity;
                                                    if (ila.InLine(attacked.X, attacked.Y))
                                                    {
                                                        if (!CanAttack(attacker, attacked, spell, attack.AttackType == Attack.Melee))
                                                            continue;

                                                        uint damage = Game.Attacking.Calculate.Melee(attacker, attacked, spell);

                                                        attack.Damage = damage;

                                                        ReceiveAttack(attacker, attacked, attack, damage, spell);

                                                        suse.Targets.Add(attacked.UID, damage);
                                                    }
                                                }
                                                else if (_obj.MapObjType == MapObjectType.SobNpc)
                                                {
                                                    attackedsob = _obj as SobNpcSpawn;

                                                    if (ila.InLine(attackedsob.X, attackedsob.Y))
                                                    {
                                                        if (!CanAttack(attacker, attackedsob, spell))
                                                            continue;

                                                        uint damage = Game.Attacking.Calculate.Melee(attacker, attackedsob);
                                                        damage = (uint)(damage * spell.PowerPercent);
                                                        attack.Damage = damage;

                                                        ReceiveAttack(attacker, attackedsob, attack, damage, spell);

                                                        suse.Targets.Add(attackedsob.UID, damage);
                                                    }
                                                }
                                            }

                                            attacker.Owner.SendScreen(suse, true);
                                        }
                                        attacker.AttackPacket = null;
                                        break;
                                    }
                                //WhirlwindKick
                                case 10415:
                                    {
                                        if (Time32.Now < attacker.WhilrwindKick.AddMilliseconds(1500))
                                        { attacker.AttackPacket = null; return; }
                                        attacker.WhilrwindKick = Time32.Now;
                                        if (ServerBase.Kernel.GetDistance(attacker.X, attacker.Y, X, Y) <= 3)
                                        {
                                            if (CanUseSpell(spell, attacker.Owner))
                                            {
                                                PrepareSpell(spell, attacker.Owner);

                                                SpellUse suse = new SpellUse(true);
                                                suse.Attacker = attacker.UID;
                                                suse.SpellID = spell.ID;
                                                suse.SpellLevel = 0;
                                                suse.X = (ushort)ServerBase.Kernel.Random.Next(3, 10);
                                                suse.Y = 0;

                                                if (ServerBase.Kernel.GetDistance(attacker.X, attacker.Y, X, Y) <= 3)
                                                {
                                                    for (int c = 0; c < attacker.Owner.Screen.Objects.Length; c++)
                                                    {
                                                        //For a multi threaded application, while we go through the collection
                                                        //the collection might change. We will make sure that we wont go off  
                                                        //the limits with a check.
                                                        if (c >= attacker.Owner.Screen.Objects.Length)
                                                            break;
                                                        Interfaces.IMapObject _obj = attacker.Owner.Screen.Objects[c];
                                                        if (_obj == null)
                                                            continue;
                                                        if (_obj.MapObjType == MapObjectType.Monster || _obj.MapObjType == MapObjectType.Player)
                                                        {
                                                            attacked = _obj as Entity;
                                                            if (ServerBase.Kernel.GetDistance(attacker.X, attacker.Y, attacked.X, attacked.Y) <= spell.Range)
                                                            {
                                                                if (CanAttack(attacker, attacked, spell, attack.AttackType == Attack.Ranged))
                                                                {
                                                                    uint damage = Game.Attacking.Calculate.Melee(attacker, attacked);

                                                                    ReceiveAttack(attacker, attacked, attack, damage, spell);
                                                                    attacked.Stunned = true;
                                                                    attacked.StunStamp = Time32.Now;
                                                                    suse.Targets.Add(attacked.UID, damage);

                                                                }
                                                            }
                                                        }
                                                    }
                                                    attacker.AttackPacket = null;
                                                }
                                                else
                                                {
                                                    attacker.AttackPacket = null; return;
                                                }
                                                attacker.Owner.SendScreen(suse, true);
                                                suse.Targets = new SafeDictionary<uint, SpellUse.DamageClass>();
                                                attacker.AttackPacket = null; return;
                                            }
                                            attacker.AttackPacket = null;
                                        }
                                        attacker.AttackPacket = null; return;
                                        //break;
                                    }
                                #endregion

                            }
                            attacker.Owner.IncreaseSpellExperience(Experience, spellID);
                            if (attacker.MapID == 1039)
                            {
                                if (spell.ID == 7001 || spell.ID == 9876)
                                {
                                    attacker.AttackPacket = null;
                                    return;
                                }
                                if (attacker.AttackPacket != null)
                                {
                                    attack.Damage = spell.ID;
                                    attacker.AttackPacket = attack;
                                    if (Database.SpellTable.WeaponSpells.ContainsValue(spell.ID))
                                    {
                                        if (attacker.AttackPacket == null)
                                        {
                                            attack.AttackType = Attack.Melee;
                                            attacker.AttackPacket = attack;
                                        }
                                        else
                                        {
                                            attacker.AttackPacket.AttackType = Attack.Melee;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (spell.NextSpellID != 0)
                                {
                                    if (spell.NextSpellID >= 1000 && spell.NextSpellID <= 1002)
                                        if (Target >= 1000000)
                                        {
                                            attacker.AttackPacket = null;
                                            return;
                                        }
                                    attack.Damage = spell.NextSpellID;
                                    attacker.AttackPacket = attack;
                                }
                                else
                                {
                                    if (!Database.SpellTable.WeaponSpells.ContainsValue(spell.ID) || spell.ID == 9876)
                                        attacker.AttackPacket = null;
                                    else
                                    {
                                        if (attacker.AttackPacket == null)
                                        {
                                            attack.AttackType = Attack.Melee;
                                            attacker.AttackPacket = attack;
                                        }
                                        else
                                        {
                                            attacker.AttackPacket.AttackType = Attack.Melee;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            attacker.AttackPacket = null;
                        }
                    }
                    #endregion
                }
                #endregion
            }

        }
        public uint ReceiveAttack(Network.GamePackets.SpellUse use_spell, Game.Entity attacker, Game.Entity attacked, Attack attack, uint damage, Database.SpellInformation spell)
        {
            uint olddmg = damage;
            uint add = 0;
            if (use_spell != null)
            {
                switch ((ClassID)attacker.SubClass)
                {
                    case ClassID.Warlock:
                        {
                            uint ran = (uint)Conquer_Online_Server.ServerBase.Kernel.Random.Next(1, 100);
                            if (ran > 70)
                            {
                                add = (byte)(50 + (attacker.SubClassLevel * 2));
                                damage += (uint)(damage * (byte)(50 + (attacker.SubClassLevel * 2))) / 100;
                                if (!use_spell.Effect.ContainsKey(attacked.UID))
                                    use_spell.Effect.Add(attacked.UID, SpellUse.EffectValue.CriticalStrike);
                                attack.Damage = damage;
                                if (use_spell.Targets.ContainsKey(attacked.UID))
                                    use_spell.Targets[attacked.UID].Damage = damage;

                            }
                            break;
                        }
                }
           
            
                            if (attacker.Owner.Entity.Statistics.CriticalStrike > 0)
                {
                            uint ran = (uint)Conquer_Online_Server.ServerBase.Kernel.Random.Next(1, 100);
                            if (ran > 40 && ran < 80)
                            {
                                add = (byte)(50 + (attacker.Owner.Entity.Statistics.CriticalStrike * 2));
                                damage += (uint)(damage * (byte)(50 + (attacker.Owner.Entity.Statistics.CriticalStrike * 2))) / 100;
                                if (!use_spell.Effect.ContainsKey(attacked.UID))
                                    use_spell.Effect.Add(attacked.UID, SpellUse.EffectValue.CriticalStrike);
                                attack.Damage = damage;
                                if (use_spell.Targets.ContainsKey(attacked.UID))
                                    use_spell.Targets[attacked.UID].Damage = damage;

                          
                        }
                }
                if (attacker.Owner.Entity.Statistics.SkillCStrike > 0)
                {
                    uint ran = (uint)Conquer_Online_Server.ServerBase.Kernel.Random.Next(1, 100);
                    if (ran > 40 && ran < 80)
                    {
                        add = (byte)(50 + (attacker.Owner.Entity.Statistics.SkillCStrike * 2));
                        damage += (uint)(damage * (byte)(50 + (attacker.Owner.Entity.Statistics.SkillCStrike * 2))) / 100;
                        if (!use_spell.Effect.ContainsKey(attacked.UID))
                            use_spell.Effect.Add(attacked.UID, SpellUse.EffectValue.CriticalStrike);
                        attack.Damage = damage;
                        if (use_spell.Targets.ContainsKey(attacked.UID))
                            use_spell.Targets[attacked.UID].Damage = damage;


                    }
                }
                switch ((ClassID)attacked.SubClass)
                {
                    case ClassID.Sage:
                        {
                            uint ran = (uint)Conquer_Online_Server.ServerBase.Kernel.Random.Next(1, 100);
                            if (ran > 70)
                            {
                                add = (byte)(50 + (attacked.SubClassLevel * 2));
                                damage -= (uint)(damage * (byte)(50 + (attacked.SubClassLevel * 2))) / 100;
                                if (!use_spell.Effect.ContainsKey(attacked.UID))
                                    use_spell.Effect.Add(attacked.UID, SpellUse.EffectValue.Block);
                                attack.Damage = damage;
                                if (use_spell.Targets.ContainsKey(attacked.UID))
                                    use_spell.Targets[attacked.UID].Damage = damage;

                            }
                            break;
                        }
                }
                switch ((ClassID)attacked.SubClass)
                {
                    case ClassID.ChiMaster:
                        {
                            if ((byte)(50 + (attacked.SubClassLevel * 2)) >= add)
                            {
                                damage = olddmg;
                            }
                            else
                            {
                                damage -= (uint)(olddmg * (byte)(50 + (attacked.SubClassLevel * 2))) / 100;
                            }
                            break;
                        }
                }
                if (attacker.Owner.Entity.Statistics.Breaktrough > 0)
                {
                            uint ran = (uint)Conquer_Online_Server.ServerBase.Kernel.Random.Next(1, 100);
                            if (ran > 10 && ran < 50)
                            {
                                add = (byte)(50 + (attacker.Owner.Entity.Statistics.Breaktrough * 3));
                                damage += (uint)(damage * (byte)(50 + (attacker.Owner.Entity.Statistics.Breaktrough * 3))) / 150;
                                if (!use_spell.Effect.ContainsKey(attacked.UID))
                                    use_spell.Effect.Add(attacked.UID, SpellUse.EffectValue.Penetration);
                                attack.Damage = damage;
                                if (use_spell.Targets.ContainsKey(attacked.UID))
                                    use_spell.Targets[attacked.UID].Damage = damage;
                            }
                }
            }
            if (!(attacked.Name.Contains("Guard") && attacked.EntityFlag == EntityFlag.Monster))
                if (attacker.EntityFlag == EntityFlag.Player && attacked.EntityFlag != EntityFlag.Player && !attacked.Name.Contains("Guard"))
                {
                    if (damage > attacked.Hitpoints)
                    {

                        attacker.Owner.IncreaseExperience(Calculate.CalculateExpBonus(attacker.Level, attacked.Level, Math.Min(damage, attacked.Hitpoints)), true);
                        if (spell != null)
                            attacker.Owner.IncreaseSpellExperience((uint)Calculate.CalculateExpBonus(attacker.Level, attacked.Level, Math.Min(damage, attacked.Hitpoints)), spell.ID);
                    }
                    else
                    {
                        attacker.Owner.IncreaseExperience(Calculate.CalculateExpBonus(attacker.Level, attacked.Level, damage), true);
                        if (spell != null)
                            attacker.Owner.IncreaseSpellExperience((uint)Calculate.CalculateExpBonus(attacker.Level, attacked.Level, damage), spell.ID);
                    }
                }
            if (attacker.EntityFlag == EntityFlag.Monster && attacked.EntityFlag == EntityFlag.Player)
            {
                if (attacked.Action == Enums.ConquerAction.Sit)
                    if (attacked.Stamina > 20)
                        attacked.Stamina -= 20;
                    else
                        attacked.Stamina = 0;
                attacked.Action = Enums.ConquerAction.None;
            }

            if (attack.AttackType == Attack.Magic)
            {
                if (attacked.Hitpoints <= damage)
                {
                    if (attacker.Owner.QualifierGroup != null)
                        attacker.Owner.QualifierGroup.UpdateDamage(attacker.Owner, attacked.Hitpoints);
                    attacked.CauseOfDeathIsMagic = true;
                    attacked.Die(attacker);
                }
                else
                {
                    if (attacker.Owner.QualifierGroup != null)
                        attacker.Owner.QualifierGroup.UpdateDamage(attacker.Owner, damage);
                    attacked.Hitpoints -= damage;
                }
            }
            else
            {
                if (attacked.Hitpoints <= damage)
                {
                    if (attacked.EntityFlag == EntityFlag.Player)
                    {
                        if (attacker.Owner.QualifierGroup != null)
                            attacker.Owner.QualifierGroup.UpdateDamage(attacker.Owner, attacked.Hitpoints);
                        attacked.Owner.SendScreen(attack, true);
                        attacker.AttackPacket = null;
                    }
                    else
                    {
                        attacked.MonsterInfo.SendScreen(attack);
                    }
                    attacked.Die(attacker);
                }
                else
                {
                    attacked.Hitpoints -= damage;
                    if (attacked.EntityFlag == EntityFlag.Player)
                    {
                        if (attacker.Owner.QualifierGroup != null)
                            attacker.Owner.QualifierGroup.UpdateDamage(attacker.Owner, damage);
                        attacked.Owner.SendScreen(attack, true);
                    }
                    else
                        attacked.MonsterInfo.SendScreen(attack);
                    attacker.AttackPacket = attack;
                    attacker.AttackStamp = Time32.Now;
                }
            }
            return damage;
        }
        public static void ReceiveAttack(Game.Entity attacker, Game.Entity attacked, Attack attack, uint damage, Database.SpellInformation spell)
        {
            #region Quarantine Tournament
            if (attacker.MapID == Quarantine.Map && attacker.MapID == Quarantine.Map && Quarantine.Started) //Emme
            {
                damage = 1;
                //181315 WhiteElegance
                //181515 BlackElegance
                #region Disqualification & Spell check
                try
                {
                    if (spell == null) { attacker.Owner.Send(new Message("Only FB/SS is allowed!", System.Drawing.Color.Red, Message.TopLeft)); return; }
                    if (spell.ID != 1045 && spell.ID != 1046)
                    { attacker.Teleport(1002, 350, 350); return; }
                    if (attacker.Owner.Equipment.Objects[8] == null)
                    { attacker.Teleport(1002, 350, 350); return; }
                    uint AttackerID = attacker.Owner.Equipment.Objects[8].ID % 1000;
                    if (AttackerID != 315 && AttackerID != 515)
                    { attacker.Teleport(1002, 350, 350); return; }
                }
                catch { }
                #endregion
                if (attacked.Owner.Equipment.Objects[8].ID == attacker.Owner.Equipment.Objects[8].ID)
                    return;

                #region Add To Team
                if (Quarantine.Black.ContainsKey(attacked.UID))
                {
                    Quarantine.Black.Remove(attacked.UID);
                    Quarantine.White.Add(attacked.UID, attacked.Owner);
                    attacked.Owner.Equipment.Objects[8].UID += (uint)Kernel.Random.Next(1, 100);
                    attacked.Owner.Equipment.Objects[8].ID = 181315;
                }
                else
                {
                    Quarantine.White.Remove(attacked.UID);
                    Quarantine.Black.Add(attacked.UID, attacked.Owner);
                    attacked.Owner.Equipment.Objects[8].UID += (uint)Kernel.Random.Next(1, 100);
                    attacked.Owner.Equipment.Objects[8].ID = 181515;
                }
                attacked.Owner.Equipment.Objects[8].Send(attacked.Owner);
                #endregion


                if (Quarantine.White.Count == 0 || Quarantine.Black.Count == 0)
                {
                    if (Quarantine.White.Count == 0) //Black team win
                    {
                        Conquer_Online_Server.ServerBase.Kernel.SendWorldMessage(new Message(
                                "The black team has this round!", System.Drawing.Color.Red, Message.TopLeft), Quarantine.Black.Values);
                        Conquer_Online_Server.ServerBase.Kernel.SendWorldMessage(new Message(
                           "The black team has this round!", System.Drawing.Color.Red, Message.TopLeft), Quarantine.White.Values);
                        Quarantine.BlackScore++;
                        int addCP = 100;
                        if (Quarantine.BlackScore == 5)
                            addCP = 800;
                        foreach (Conquer_Online_Server.Client.GameState memb in Quarantine.Black.Values)
                            memb.Entity.ConquerPoints += (uint)addCP;
                        while (Quarantine.White.Count == 0)
                        {
                            foreach (Conquer_Online_Server.Client.GameState member in Quarantine.Black.Values)
                            {
                                if (Conquer_Online_Server.ServerBase.Kernel.Rate(50))
                                {
                                    Quarantine.White.Add(member.Entity.UID, member);
                                    Quarantine.Black.Remove(member.Entity.UID);
                                    member.Equipment.Objects[8].UID += (uint)Kernel.Random.Next(1, 100);
                                    member.Equipment.Objects[8].ID = 181315;
                                    member.Equipment.Objects[8].Send(member);
                                }

                            }
                        }
                    }
                    else //White team win
                    {
                        Conquer_Online_Server.ServerBase.Kernel.SendWorldMessage(new Message(
                                "The white team has this round!", System.Drawing.Color.Red, Message.TopLeft), Quarantine.Black.Values);
                        Conquer_Online_Server.ServerBase.Kernel.SendWorldMessage(new Message(
                           "The white team has this round!", System.Drawing.Color.Red, Message.TopLeft), Quarantine.White.Values);
                        Quarantine.WhiteScore++;
                        int addCP = 100;
                        if (Quarantine.WhiteScore == 5)
                            addCP = 800;
                        foreach (Conquer_Online_Server.Client.GameState member in Quarantine.White.Values)
                            member.Entity.ConquerPoints += (uint)addCP; //Add 100 cps for winning 1 round
                        while (Quarantine.Black.Count == 0)
                        {
                            foreach (Conquer_Online_Server.Client.GameState member in Quarantine.White.Values)
                            {
                                if (Conquer_Online_Server.ServerBase.Kernel.Rate(50))
                                {
                                    Quarantine.Black.Add(member.Entity.UID, member);
                                    Quarantine.White.Remove(member.Entity.UID);
                                    member.Equipment.Objects[8].UID += (uint)Kernel.Random.Next(1, 100);
                                    member.Equipment.Objects[8].ID = 181515;
                                    member.Equipment.Objects[8].Send(member);
                                }

                            }
                        }
                    }
                    if (Quarantine.BlackScore == 5 || Quarantine.WhiteScore == 5)
                    {
                        foreach (Conquer_Online_Server.Client.GameState participant in Conquer_Online_Server.ServerBase.Kernel.GamePool.Values)
                        {
                            participant.UpdateQuarantineScore();
                            participant.Entity.Teleport(1002, 437, 349);
                            participant.Send(new Message("", System.Drawing.Color.Red, Message.FirstRightCorner));
                            participant.Send(new Message("The tournament is over!", System.Drawing.Color.Red, Message.TopLeft));
                            participant.quarantineDeath = 0;
                            participant.quarantineKill = 0;
                            participant.Equipment.Remove(9);
                            participant.Equipment.Objects[8] = null;
                            participant.SendEquipment(false);
                        }
                        Quarantine.Black = new ThreadSafeDictionary<uint, Conquer_Online_Server.Client.GameState>(50);
                        Quarantine.White = new ThreadSafeDictionary<uint, Conquer_Online_Server.Client.GameState>(50);
                        Quarantine.WhiteScore = 0;
                        Quarantine.BlackScore = 0;
                        Quarantine.Started = false;
                    }
                }
                if (attacked.Owner.Equipment.Objects[8] != null)
                    attacked.Owner.Equipment.Objects[8].Send(attacked.Owner);
                attacked.Owner.SendEquipment(false);
                attacker.Owner.SendEquipment(false);
                attacked.Owner.Equipment.UpdateEntityPacket();
                attacker.Owner.AddQuarantineKill(); //Increase kills on attacker
                attacked.Owner.AddQuarantineDeath(); //Increase death on attacked

            }
            #endregion
            if (!(attacked.Name.Contains("Guard1") && attacked.EntityFlag == EntityFlag.Monster))
                if (attacker.EntityFlag == EntityFlag.Player && attacked.EntityFlag != EntityFlag.Player && !attacked.Name.Contains("Guard"))
                {
                    if (damage > attacked.Hitpoints)
                    {
                        attacker.Owner.IncreaseExperience(Calculate.CalculateExpBonus(attacker.Level, attacked.Level, Math.Min(damage, attacked.Hitpoints)), true);
                        if (spell != null)
                            attacker.Owner.IncreaseSpellExperience((uint)Calculate.CalculateExpBonus(attacker.Level, attacked.Level, Math.Min(damage, attacked.Hitpoints)), spell.ID);
                    }
                    else
                    {
                        attacker.Owner.IncreaseExperience(Calculate.CalculateExpBonus(attacker.Level, attacked.Level, damage), true);
                        if (spell != null)
                            attacker.Owner.IncreaseSpellExperience((uint)Calculate.CalculateExpBonus(attacker.Level, attacked.Level, damage), spell.ID);
                    }
                }
            if (attacker.EntityFlag == EntityFlag.Monster && attacked.EntityFlag == EntityFlag.Player)
            {
                if (attacked.Action == Enums.ConquerAction.Sit)
                    if (attacked.Stamina > 20)
                        attacked.Stamina -= 20;
                    else
                        attacked.Stamina = 0;
                attacked.Action = Enums.ConquerAction.None;
            }

            if (attack.AttackType == Attack.Magic)
            {
                if (attacked.Hitpoints <= damage)
                {
                    if (attacker.Owner.QualifierGroup != null)
                        attacker.Owner.QualifierGroup.UpdateDamage(attacker.Owner, attacked.Hitpoints);
                    attacked.CauseOfDeathIsMagic = true;
                    attacked.Die(attacker);
                }
                else
                {
                    if (attacker.Owner.QualifierGroup != null)
                        attacker.Owner.QualifierGroup.UpdateDamage(attacker.Owner, damage);
                    attacked.Hitpoints -= damage;
                }
            }
            else
            {
                if (attacked.Hitpoints <= damage)
                {
                    if (attacked.EntityFlag == EntityFlag.Player)
                    {
                        if (attacker.Owner.QualifierGroup != null)
                            attacker.Owner.QualifierGroup.UpdateDamage(attacker.Owner, attacked.Hitpoints);
                        attacked.Owner.SendScreen(attack, true);
                        attacker.AttackPacket = null;
                    }
                    else
                    {
                        attacked.MonsterInfo.SendScreen(attack);
                    }
                    attacked.Die(attacker);
                }
                else
                {
                    attacked.Hitpoints -= damage;
                    if (attacked.EntityFlag == EntityFlag.Player)
                    {
                        if (attacker.Owner.QualifierGroup != null)
                            attacker.Owner.QualifierGroup.UpdateDamage(attacker.Owner, damage);
                        attacked.Owner.SendScreen(attack, true);
                    }
                    else
                        attacked.MonsterInfo.SendScreen(attack);
                    attacker.AttackPacket = attack;
                    attacker.AttackStamp = Time32.Now;
                }
            }
        }
        public static void ReceiveAttack(Game.Entity attacker, SobNpcSpawn attacked, Attack attack, uint damage, Database.SpellInformation spell)
        {
            if (attacker.EntityFlag == EntityFlag.Player)
                if (attacker.MapID == 1017)
                {
                    if (attacked.UID == 8684)
                    {
                        if (attacked.Hitpoints == 0 && attacked.MaxHitpoints == 0)

                            attacked.Hitpoints = 0;
                        attacked.MaxHitpoints = 0;

                        attacker.AddFlag2(Update.Flags2.CaryingFlag);

                        //  Game.ConquerStructures.Society.GuildWar.AddScore(damage, attacker.Owner.Guild);
                    }
                }
            if (damage > attacked.Hitpoints)
            {
                if (attacker.MapID == 1039)
                    attacker.Owner.IncreaseExperience(Math.Min(damage, attacked.Hitpoints), true);
                if (spell != null)
                    attacker.Owner.IncreaseSpellExperience(Math.Min(damage, attacked.Hitpoints), spell.ID);
            }
            else
            {
                if (attacker.MapID == 1039)
                    attacker.Owner.IncreaseExperience(damage, true);
                if (spell != null)
                    attacker.Owner.IncreaseSpellExperience(damage, spell.ID);
            }
            if (attacker.MapID == 1038)
            {
                if (attacked.UID == 810)
                {
                    if (Game.ConquerStructures.Society.GuildWar.PoleKeeper == attacker.Owner.Guild)
                        return;
                    if (attacked.Hitpoints <= damage)
                        attacked.Hitpoints = 0;
                    Game.ConquerStructures.Society.GuildWar.AddScore(damage, attacker.Owner.Guild);
                }
            }

            if (attack.AttackType == Attack.Magic)
            {
                if (attacked.Hitpoints <= damage)
                {
                    attacked.Die(attacker);
                }
                else
                {
                    attacked.Hitpoints -= damage;
                }
            }
            else
            {
                attacker.Owner.SendScreen(attack, true);
                if (attacked.Hitpoints <= damage)
                {
                    attacked.Die(attacker);
                }
                else
                {
                    attacked.Hitpoints -= damage;
                    attacker.AttackPacket = attack;
                    attacker.AttackStamp = Time32.Now;
                }
            }
        }
        public static bool isArcherSkill(uint ID)
        {
            if (ID >= 8000 && ID <= 9875)
                return true;
            return false;
        }
        public static bool CanUseSpell(Database.SpellInformation spell, Client.GameState client)
        {
            if (client.WatchingGroup != null)
                return false;
            if (spell == null)
                return false;
            if (client.Entity.Mana < spell.UseMana)
                return false;
            if (client.Entity.Stamina < spell.UseStamina)
                return false;
            if (spell.UseArrows > 0 && isArcherSkill(spell.ID))
            {
                if (!client.Equipment.Free((byte)ConquerItem.LeftWeapon))
                {
                    Interfaces.IConquerItem arrow = client.Equipment.TryGetItem(ConquerItem.LeftWeapon);
                    if (arrow.Durability <= spell.UseArrows)
                    {
                        return false;
                    }
                    return arrow.Durability >= spell.UseArrows;
                }
                return false;
            }
            if (spell.NeedXP == 1 && !client.Entity.ContainsFlag(Update.Flags.XPList))
                return false;
            return true;
        }
        public static void PrepareSpell(Database.SpellInformation spell, Client.GameState client)
        {
            if (spell.NeedXP == 1)
                client.Entity.RemoveFlag(Update.Flags.XPList);
            if (client.Map.ID != 1039)
            {
                if (spell.UseMana > 0)
                    if (client.Entity.Mana >= spell.UseMana)
                        client.Entity.Mana -= spell.UseMana;
                if (spell.UseStamina > 0)
                    if (client.Entity.Stamina >= spell.UseStamina)
                        client.Entity.Stamina -= spell.UseStamina;
                if (spell.UseArrows > 0 && isArcherSkill(spell.ID))
                {
                    if (!client.Equipment.Free((byte)ConquerItem.LeftWeapon))
                    {
                        Interfaces.IConquerItem arrow = client.Equipment.TryGetItem(ConquerItem.LeftWeapon);
                        arrow.Durability -= spell.UseArrows;
                        ItemUsage usage = new ItemUsage(true) { UID = arrow.UID, dwParam = arrow.Durability, ID = ItemUsage.UpdateDurability };
                        usage.Send(client);
                        if (arrow.Durability <= spell.UseArrows || arrow.Durability > 5000)
                        {
                            Network.PacketHandler.ReloadArrows(client.Equipment.TryGetItem(ConquerItem.LeftWeapon), client);
                        }
                    }
                }
            }
        }
        public static bool CanAttack(Game.Entity attacker, SobNpcSpawn attacked, Database.SpellInformation spell)
        {
            if (attacker.MapID == 1038)
            {
                if (attacker.GuildID == 0 || !Game.ConquerStructures.Society.GuildWar.IsWar)
                {
                    if (attacked.UID == 810)
                    {
                        return false;
                    }
                }
                if (Game.ConquerStructures.Society.GuildWar.PoleKeeper != null)
                {
                    if (Game.ConquerStructures.Society.GuildWar.PoleKeeper == attacker.Owner.Guild)
                    {
                        if (attacked.UID == 810)
                        {
                            return false;
                        }
                    }
                    else if (attacked.UID == 516075 || attacked.UID == 516074)
                    {
                        if (Game.ConquerStructures.Society.GuildWar.PoleKeeper == attacker.Owner.Guild)
                        {
                            if (attacker.PKMode == Enums.PKMode.Team)
                                return false;
                        }
                    }
                }
            }
            if (attacker.MapID == 1039)
            {
                bool stake = true;
                if (attacked.Name.ToLower().Contains("crow"))
                    stake = false;

                ushort levelbase = (ushort)(attacked.Mesh / 10);
                if (stake)
                    levelbase -= 42;
                else
                    levelbase -= 43;

                byte level = (byte)(20 + (levelbase / 3) * 5);
                if (levelbase == 108 || levelbase == 109)
                    level = 125;
                if (attacker.Level >= level)
                    return true;
                else
                {
                    attacker.AttackPacket = null;
                    attacker.Owner.Send(ServerBase.Constants.DummyLevelTooHigh());
                    return false;
                }
            }
            return true;
        }
        public static bool CanAttack(Game.Entity attacker, Game.Entity attacked, Database.SpellInformation spell, bool melee)
        {
            if (spell != null)
                if (spell.CanKill && attacker.EntityFlag == EntityFlag.Player && ServerBase.Constants.PKForbiddenMaps.Contains(attacker.Owner.Map.ID) && attacked.EntityFlag == EntityFlag.Player)
                    return false;
            if (attacker.EntityFlag == EntityFlag.Player)
                if (attacker.Owner.WatchingGroup != null)
                    return false;
            if (attacked == null)
                return false;
            if (attacked.Dead)
            {
                attacker.AttackPacket = null;
                return false;
            }
            if (attacked.EntityFlag == EntityFlag.Monster)
            {
                if (attacked.Companion)
                {
                    if (ServerBase.Constants.PKForbiddenMaps.Contains(attacker.Owner.Map.ID))
                    {
                        if (attacked.Owner == attacker.Owner)
                            return false;
                        if (attacker.PKMode != Conquer_Online_Server.Game.Enums.PKMode.PK &&
                         attacker.PKMode != Conquer_Online_Server.Game.Enums.PKMode.Team)
                            return false;
                        else
                        {
                            attacker.AddFlag(Network.GamePackets.Update.Flags.FlashingName);
                            attacker.FlashingNameStamp = Time32.Now;
                            attacker.FlashingNameTime = 10;

                            return true;
                        }
                    }
                }
                if (attacked.Name.Contains("Guard"))
                {
                    if (attacker.PKMode != Conquer_Online_Server.Game.Enums.PKMode.PK &&
                    attacker.PKMode != Conquer_Online_Server.Game.Enums.PKMode.Team)
                        return false;
                    else
                    {
                        attacker.AddFlag(Network.GamePackets.Update.Flags.FlashingName);
                        attacker.FlashingNameStamp = Time32.Now;
                        attacker.FlashingNameTime = 10;

                        return true;
                    }
                }
                else
                    return true;
            }
            else
            {
                if (attacked.EntityFlag == EntityFlag.Player)
                    if (!attacked.Owner.Attackable)
                        return false;
                if (attacker.EntityFlag == EntityFlag.Player)
                    if (attacker.Owner.WatchingGroup == null)
                        if (attacked.EntityFlag == EntityFlag.Player)
                            if (attacked.Owner.WatchingGroup != null)
                                return false;

                if (spell != null)
                    if (spell.OnlyGround)
                        if (attacked.ContainsFlag(Update.Flags.Fly))
                            return false;
                if (melee && attacked.ContainsFlag(Update.Flags.Fly))
                    return false;

                if (ServerBase.Constants.PKForbiddenMaps.Contains(attacker.Owner.Map.ID))
                {
                    if (attacker.PKMode == Conquer_Online_Server.Game.Enums.PKMode.PK ||
                        attacker.PKMode == Conquer_Online_Server.Game.Enums.PKMode.Team || (spell != null && spell.CanKill))
                    {
                        attacker.Owner.Send(ServerBase.Constants.PKForbidden);
                        attacker.AttackPacket = null;
                    }
                    return false;
                }
                if (attacker.PKMode == Conquer_Online_Server.Game.Enums.PKMode.Capture)
                {
                    if (attacked.ContainsFlag(Update.Flags.FlashingName) || attacked.PKPoints > 99)
                    {
                        return true;
                    }
                }
                if (attacker.PKMode == Conquer_Online_Server.Game.Enums.PKMode.Team)
                {
                    if (attacker.Owner.Team != null)
                        if (attacker.Owner.Team.IsTeammate(attacked.UID))
                        {
                            attacker.AttackPacket = null;
                            return false;
                        }
                    if (attacker.Owner.Guild != null)
                    {
                        if (attacker.GuildID != 0)
                        {
                            if (attacked.GuildID != 0)
                            {
                                if (attacker.GuildID == attacked.GuildID)
                                {
                                    attacker.AttackPacket = null;
                                    return false;
                                }
                            }
                        }
                    }
                    if (attacker.ClanId != 0)
                    {
                        if (attacker.ClanId != 0)
                        {
                            if (attacked.ClanId != 0)
                            {
                                if (attacker.ClanId == attacked.ClanId)
                                {
                                    attacker.AttackPacket = null;
                                    return false;
                                }
                            }
                        }
                    }
                    if (attacker.Owner.Friends.ContainsKey(attacked.UID))
                    {
                        attacker.AttackPacket = null;
                        return false;
                    }
                    if (attacker.Owner.Guild != null)
                    {
                        if (attacker.GuildID != 0)
                        {
                            if (attacked.GuildID != 0)
                            {
                                if (attacker.Owner.Guild.Ally.ContainsKey(attacked.GuildID))
                                {
                                    attacker.AttackPacket = null;
                                    return false;
                                }
                            }
                        }
                    }
                    if (attacker.ClanId != 0)
                    {
                        if (attacked.ClanId != 0)
                        {
                            if (attacker.GetClan.Allies.ContainsKey(attacked.ClanId))
                            {
                                attacker.AttackPacket = null;
                                return false;
                            }
                        }
                    }
                }

                if (spell != null)
                    if (spell.OnlyGround)
                        if (attacked.ContainsFlag(Update.Flags.Fly))
                            return false;
                if (spell != null)
                    if (!spell.CanKill)
                    {
                        if (spell != null && !spell.CanKill && attacker.PKMode == Conquer_Online_Server.Game.Enums.PKMode.Capture)
                        {
                            return false;
                        }
                        if (spell != null && !spell.CanKill && attacker.Owner.Map.ID >= 10000)
                        {
                            attacker.AddFlag(Network.GamePackets.Update.Flags.Normal);
                            return true;
                        }
                        else
                        {
                            attacker.AddFlag(Network.GamePackets.Update.Flags.FlashingName);
                            attacker.FlashingNameStamp = Time32.Now;
                            attacker.FlashingNameTime = 10;
                            return true;

                        }
                    }



                if (attacker.PKMode != Conquer_Online_Server.Game.Enums.PKMode.PK &&
                    attacker.PKMode != Conquer_Online_Server.Game.Enums.PKMode.Team && attacked.PKPoints < 99)
                {
                    attacker.AttackPacket = null;
                    return false;
                }
                else
                {
                    if (!attacked.ContainsFlag(Update.Flags.FlashingName) || !attacked.ContainsFlag(Update.Flags.BlackName))
                    {
                        if (ServerBase.Constants.PKFreeMaps.Contains(attacker.Owner.Map.BaseID))
                            return true;
                        attacker.AddFlag(Network.GamePackets.Update.Flags.FlashingName);
                        attacker.FlashingNameStamp = Time32.Now;
                        attacker.FlashingNameTime = 10;
                    }
                }
                return true;
            }
        }
        public static void CheckForExtraWeaponPowers(Client.GameState client, Entity attacked)
        {
            #region Right Hand
            if (client.Equipment.TryGetItem(ConquerItem.RightWeapon) != null)
            {
                if (client.Equipment.TryGetItem(ConquerItem.RightWeapon).ID != 0)
                {
                    var Item = client.Equipment.TryGetItem(ConquerItem.RightWeapon);
                    if (Item.Effect != Enums.ItemEffect.None)
                    {
                        if (ServerBase.Kernel.Rate(30))
                        {
                            switch (Item.Effect)
                            {
                                case Enums.ItemEffect.HP:
                                    {
                                        SpellUse spellUse = new SpellUse(true);
                                        spellUse.SpellID = 1175;
                                        spellUse.SpellLevel = 4;
                                        spellUse.X = client.Entity.X;
                                        spellUse.Y = client.Entity.Y;
                                        spellUse.Targets.Add(client.Entity.UID, 300);
                                        uint damage = Math.Min(300, client.Entity.MaxHitpoints - client.Entity.Hitpoints);
                                        client.Entity.Hitpoints += damage;
                                        client.SendScreen(spellUse, true);
                                        break;
                                    }
                                case Enums.ItemEffect.MP:
                                    {
                                        SpellUse spellUse = new SpellUse(true);
                                        spellUse.SpellID = 1175;
                                        spellUse.SpellLevel = 2;
                                        spellUse.X = client.Entity.X;
                                        spellUse.Y = client.Entity.Y;
                                        spellUse.Targets.Add(client.Entity.UID, 300);
                                        ushort damage = (ushort)Math.Min(300, client.Entity.MaxMana - client.Entity.Mana);
                                        client.Entity.Mana += damage;
                                        client.SendScreen(spellUse, true);
                                        break;
                                    }
                                case Enums.ItemEffect.Shield:
                                    {
                                        if (client.Entity.ContainsFlag(Update.Flags.MagicShield))
                                            return;
                                        SpellUse spellUse = new SpellUse(true);
                                        spellUse.SpellID = 1020;
                                        spellUse.SpellLevel = 0;
                                        spellUse.X = client.Entity.X;
                                        spellUse.Y = client.Entity.Y;
                                        spellUse.Targets.Add(client.Entity.UID, 120);
                                        client.Entity.ShieldTime = 0;
                                        client.Entity.ShieldStamp = Time32.Now;
                                        client.Entity.MagicShieldStamp = Time32.Now;
                                        client.Entity.MagicShieldTime = 0;

                                        client.Entity.AddFlag(Update.Flags.MagicShield);
                                        client.Entity.MagicShieldStamp = Time32.Now;
                                        client.Entity.MagicShieldIncrease = 2;
                                        client.Entity.MagicShieldTime = 120;
                                        if (client.Entity.EntityFlag == EntityFlag.Player)
                                            client.Send(ServerBase.Constants.Shield(2, 120));
                                        client.SendScreen(spellUse, true);
                                        break;
                                    }
                                case Enums.ItemEffect.Poison:
                                    {
                                        if (attacked != null)
                                        {
                                            if (attacked.UID == client.Entity.UID)
                                                return;
                                            if (attacked.ToxicFogLeft > 0)
                                                return;
                                            SpellUse spellUse = new SpellUse(true);
                                            spellUse.SpellID = 5040;
                                            spellUse.Attacker = attacked.UID;
                                            spellUse.SpellLevel = 9;
                                            spellUse.X = attacked.X;
                                            spellUse.Y = attacked.Y;
                                            spellUse.Targets.Add(attacked.UID, 0);
                                            spellUse.Targets[attacked.UID].Hit = true;
                                            attacked.ToxicFogStamp = Time32.Now;
                                            attacked.ToxicFogLeft = 10;
                                            attacked.ToxicFogPercent = 0.05F;
                                            client.SendScreen(spellUse, true);
                                        }
                                        break;
                                    }
                            }
                        }
                    }
                }
            }
            #endregion
            #region Left Hand
            if (client.Equipment.TryGetItem(ConquerItem.LeftWeapon) != null)
            {
                if (client.Equipment.TryGetItem(ConquerItem.LeftWeapon).ID != 0)
                {
                    var Item = client.Equipment.TryGetItem(ConquerItem.LeftWeapon);
                    if (Item.Effect != Enums.ItemEffect.None)
                    {
                        if (ServerBase.Kernel.Rate(30))
                        {
                            switch (Item.Effect)
                            {
                                case Enums.ItemEffect.HP:
                                    {
                                        SpellUse spellUse = new SpellUse(true);
                                        spellUse.SpellID = 1175;
                                        spellUse.SpellLevel = 4;
                                        spellUse.X = client.Entity.X;
                                        spellUse.Y = client.Entity.Y;
                                        spellUse.Targets.Add(client.Entity.UID, 300);
                                        uint damage = Math.Min(300, client.Entity.MaxHitpoints - client.Entity.Hitpoints);
                                        client.Entity.Hitpoints += damage;
                                        client.SendScreen(spellUse, true);
                                        break;
                                    }
                                case Enums.ItemEffect.MP:
                                    {
                                        SpellUse spellUse = new SpellUse(true);
                                        spellUse.SpellID = 1175;
                                        spellUse.SpellLevel = 2;
                                        spellUse.X = client.Entity.X;
                                        spellUse.Y = client.Entity.Y;
                                        spellUse.Targets.Add(client.Entity.UID, 300);
                                        ushort damage = (ushort)Math.Min(300, client.Entity.MaxMana - client.Entity.Mana);
                                        client.Entity.Mana += damage;
                                        client.SendScreen(spellUse, true);
                                        break;
                                    }
                                case Enums.ItemEffect.Shield:
                                    {
                                        if (client.Entity.ContainsFlag(Update.Flags.MagicShield))
                                            return;
                                        SpellUse spellUse = new SpellUse(true);
                                        spellUse.SpellID = 1020;
                                        spellUse.SpellLevel = 0;
                                        spellUse.X = client.Entity.X;
                                        spellUse.Y = client.Entity.Y;
                                        spellUse.Targets.Add(client.Entity.UID, 120);
                                        client.Entity.ShieldTime = 0;
                                        client.Entity.ShieldStamp = Time32.Now;
                                        client.Entity.MagicShieldStamp = Time32.Now;
                                        client.Entity.MagicShieldTime = 0;

                                        client.Entity.AddFlag(Update.Flags.MagicShield);
                                        client.Entity.MagicShieldStamp = Time32.Now;
                                        client.Entity.MagicShieldIncrease = 2;
                                        client.Entity.MagicShieldTime = 120;
                                        if (client.Entity.EntityFlag == EntityFlag.Player)
                                            client.Send(ServerBase.Constants.Shield(2, 120));
                                        client.SendScreen(spellUse, true);
                                        break;
                                    }
                                case Enums.ItemEffect.Poison:
                                    {
                                        if (attacked != null)
                                        {
                                            if (attacked.UID == client.Entity.UID)
                                                return;
                                            if (attacked.ToxicFogLeft > 0)
                                                return;
                                            SpellUse spellUse = new SpellUse(true);
                                            spellUse.SpellID = 5040;
                                            spellUse.Attacker = attacked.UID;
                                            spellUse.SpellLevel = 9;
                                            spellUse.X = attacked.X;
                                            spellUse.Y = attacked.Y;
                                            spellUse.Targets.Add(attacked.UID, 0);
                                            spellUse.Targets[attacked.UID].Hit = true;
                                            attacked.ToxicFogStamp = Time32.Now;
                                            attacked.ToxicFogLeft = 10;
                                            attacked.ToxicFogPercent = 0.05F;
                                            client.SendScreen(spellUse, true);
                                        }
                                        break;
                                    }
                            }
                        }
                    }
                }
            }
            #endregion
        }
        public static void CheckForSuperGems(Client.GameState client)
        {

        }
    }
}
