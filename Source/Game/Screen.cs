using System;
using System.Linq;
using System.Threading;
using System.Collections.Generic;
using Conquer_Online_Server.Interfaces;
using Conquer_Online_Server.Client;
using Conquer_Online_Server.ServerBase;
using Conquer_Online_Server.Network.GamePackets;

namespace Conquer_Online_Server.Game
{
    public class Screen
    {
        private Timer _timer;
        private TimerCallback CallBack;
        public double interval = 100;
        public System.Timers.Timer MyTimer;
        public Interfaces.IMapObject[] Objects;
        private SafeDictionary<uint, Interfaces.IMapObject> _objects;
        public Client.GameState Owner;
        public Screen(Client.GameState client)
        {
            Owner = client;
            Objects = new Interfaces.IMapObject[0];
            _objects = new SafeDictionary<uint, IMapObject>(1000);
            //CallBack = new TimerCallback(_timerCallBack);
            //_timer = new Timer(CallBack, Owner, 5000, 100);
            MyTimer = new System.Timers.Timer(interval);
            MyTimer.AutoReset = true;

            MyTimer.Elapsed += new System.Timers.ElapsedEventHandler(_timerCallBack);
            MyTimer.Start();
        }

        private void _timerCallBack(object scrObject, System.Timers.ElapsedEventArgs arg)
        {
            if (!Owner.Socket.Connected || Owner.Entity == null)
            {
                if (_timer != null)
                {
                    _timer.Change(Timeout.Infinite, Timeout.Infinite);
                    _timer = null;
                }
                return;
            }
            #region Monsters
            List<IMapObject> toRemove = new List<IMapObject>();
            if (Owner.Map.FreezeMonsters)
                return;
            try
            {
                foreach (IMapObject obj in Objects)
                {
                    if (obj != null)
                    {
                        if (obj.MapObjType == MapObjectType.Monster)
                        {
                            if (Owner.Map.Entities.ContainsKey(obj.UID))
                            {
                                Entity monster = Owner.Map.Entities[obj.UID];
                                #region Buffers
                                if (monster.ContainsFlag(Network.GamePackets.Update.Flags.Stigma))
                                {
                                    if (Time32.Now >= monster.StigmaStamp.AddSeconds(monster.StigmaTime))
                                    {
                                        monster.StigmaTime = 0;
                                        monster.StigmaIncrease = 0;
                                        monster.RemoveFlag(Network.GamePackets.Update.Flags.Stigma);
                                    }
                                }
                                if (monster.ContainsFlag(Network.GamePackets.Update.Flags.Dodge))
                                {
                                    if (Time32.Now >= monster.DodgeStamp.AddSeconds(monster.DodgeTime))
                                    {
                                        monster.DodgeTime = 0;
                                        monster.DodgeIncrease = 0;
                                        monster.RemoveFlag(Network.GamePackets.Update.Flags.Dodge);
                                    }
                                }
                                if (monster.ContainsFlag(Network.GamePackets.Update.Flags.Invisibility))
                                {
                                    if (Time32.Now >= monster.InvisibilityStamp.AddSeconds(monster.InvisibilityTime))
                                    {
                                        monster.RemoveFlag(Network.GamePackets.Update.Flags.Invisibility);
                                    }
                                }
                                if (monster.ContainsFlag(Network.GamePackets.Update.Flags.StarOfAccuracy))
                                {
                                    if (monster.StarOfAccuracyTime != 0)
                                    {
                                        if (Time32.Now >= monster.StarOfAccuracyStamp.AddSeconds(monster.StarOfAccuracyTime))
                                        {
                                            monster.RemoveFlag(Network.GamePackets.Update.Flags.StarOfAccuracy);
                                        }
                                    }
                                    else
                                    {
                                        if (Time32.Now >= monster.AccuracyStamp.AddSeconds(monster.AccuracyTime))
                                        {
                                            monster.RemoveFlag(Network.GamePackets.Update.Flags.StarOfAccuracy);
                                        }
                                    }
                                }
                                if (monster.ContainsFlag(Network.GamePackets.Update.Flags.MagicShield))
                                {
                                    if (monster.MagicShieldTime != 0)
                                    {
                                        if (Time32.Now >= monster.MagicShieldStamp.AddSeconds(monster.MagicShieldTime))
                                        {
                                            monster.MagicShieldIncrease = 0;
                                            monster.MagicShieldTime = 0;
                                            monster.RemoveFlag(Network.GamePackets.Update.Flags.MagicShield);
                                        }
                                    }
                                    else
                                    {
                                        if (Time32.Now >= monster.ShieldStamp.AddSeconds(monster.ShieldTime))
                                        {
                                            monster.ShieldIncrease = 0;
                                            monster.ShieldTime = 0;
                                            monster.RemoveFlag(Network.GamePackets.Update.Flags.MagicShield);
                                        }
                                    }
                                }
                                #endregion
                                #region Dead monster
                                if (monster.Dead || monster.Killed)
                                {
                                    if (!monster.ContainsFlag(Network.GamePackets.Update.Flags.Ghost) || monster.Killed)
                                    {
                                        monster.Killed = false;
                                        monster.MonsterInfo.InSight = 0;
                                        monster.AddFlag(Network.GamePackets.Update.Flags.Ghost);
                                        monster.AddFlag(Network.GamePackets.Update.Flags.Dead);
                                        monster.AddFlag(Network.GamePackets.Update.Flags.FadeAway);
                                        Network.GamePackets.Attack attack = new Network.GamePackets.Attack(true);
                                        attack.Attacker = monster.Killer.UID;
                                        attack.Attacked = monster.UID;
                                        attack.AttackType = Network.GamePackets.Attack.Kill;
                                        attack.X = monster.X;
                                        attack.Y = monster.Y;
                                        Owner.Map.Floor[monster.X, monster.Y, MapObjectType.Monster, monster] = true;
                                        attack.KOCount = ++monster.Killer.KOCount;
                                        if (monster.Killer.EntityFlag == EntityFlag.Player)
                                        {
                                            monster.MonsterInfo.ExcludeFromSend = monster.Killer.UID;
                                            monster.Killer.Owner.Send(attack);
                                        }
                                        monster.MonsterInfo.SendScreen(attack);
                                        monster.MonsterInfo.ExcludeFromSend = 0;
                                    }
                                    if (Time32.Now > monster.DeathStamp.AddSeconds(4))
                                    {
                                        Network.GamePackets.Data data = new Network.GamePackets.Data(true);
                                        data.UID = monster.UID;
                                        data.ID = Network.GamePackets.Data.RemoveEntity;
                                        monster.MonsterInfo.SendScreen(data);
                                    }
                                }
                                #endregion
                                #region Alive monster
                                else
                                {
                                    if ((obj as Entity).Companion)
                                        continue;
                                    if (Time32.Now >= monster.MonsterInfo.LastMove.AddMilliseconds(monster.MonsterInfo.MinimumSpeed))
                                    {
                                        #region Guard
                                        if (monster.Name.Contains("Guard"))
                                        {
                                            if (monster.MonsterInfo.InSight == 0)
                                            {
                                                if (monster.X != monster.MonsterInfo.BoundX || monster.Y != monster.MonsterInfo.BoundY)
                                                {
                                                    monster.X = monster.MonsterInfo.BoundX;
                                                    monster.Y = monster.MonsterInfo.BoundY;
                                                    Network.GamePackets.TwoMovements jump = new Conquer_Online_Server.Network.GamePackets.TwoMovements();
                                                    jump.X = monster.MonsterInfo.BoundX;
                                                    jump.Y = monster.MonsterInfo.BoundY;
                                                    jump.EntityCount = 1;
                                                    jump.FirstEntity = monster.UID;
                                                    jump.MovementType = Network.GamePackets.TwoMovements.Jump;
                                                    Owner.SendScreen(jump, true);
                                                }
                                                if (Owner.Entity.ContainsFlag(Network.GamePackets.Update.Flags.FlashingName))
                                                    monster.MonsterInfo.InSight = Owner.Entity.UID;
                                            }
                                            else
                                            {
                                                if (Owner.Entity.ContainsFlag(Network.GamePackets.Update.Flags.FlashingName))
                                                {
                                                    if (monster.MonsterInfo.InSight == Owner.Entity.UID)
                                                    {
                                                        if (!Owner.Entity.Dead)
                                                        {
                                                            if (Time32.Now >= monster.MonsterInfo.LastMove.AddMilliseconds(monster.MonsterInfo.AttackSpeed))
                                                            {
                                                                short distance = Kernel.GetDistance(monster.X, monster.Y, Owner.Entity.X, Owner.Entity.Y);

                                                                if (distance <= monster.MonsterInfo.AttackRange)
                                                                {
                                                                    monster.MonsterInfo.LastMove = Time32.Now;
                                                                    new Game.Attacking.Handle(null, monster, Owner.Entity);
                                                                }
                                                                else
                                                                {
                                                                    if (distance <= monster.MonsterInfo.ViewRange)
                                                                    {
                                                                        Network.GamePackets.TwoMovements jump = new Conquer_Online_Server.Network.GamePackets.TwoMovements();
                                                                        jump.X = Owner.Entity.X;
                                                                        jump.Y = Owner.Entity.Y;
                                                                        monster.X = Owner.Entity.X;
                                                                        monster.Y = Owner.Entity.Y;
                                                                        jump.EntityCount = 1;
                                                                        jump.FirstEntity = monster.UID;
                                                                        jump.MovementType = Network.GamePackets.TwoMovements.Jump;
                                                                        Owner.SendScreen(jump, true);
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {

                                                    }
                                                }
                                                else
                                                {
                                                    if (monster.MonsterInfo.InSight == Owner.Entity.UID)
                                                    {
                                                        monster.MonsterInfo.InSight = 0;
                                                    }
                                                }
                                            }

                                            foreach (IMapObject obj2 in Objects)
                                            {
                                                if (obj2 == null)
                                                    continue;
                                                if (obj2.MapObjType == MapObjectType.Monster)
                                                {
                                                    Entity monster2 = Owner.Map.Entities[obj2.UID];
                                                    if (monster2 == null)
                                                        continue;
                                                    if (monster2.Dead)
                                                        continue;
                                                    if (Time32.Now >= monster.MonsterInfo.LastMove.AddMilliseconds(monster.MonsterInfo.AttackSpeed))
                                                    {
                                                        if (!monster2.Name.Contains("Guard") && !monster2.Companion)
                                                        {
                                                            short distance = Kernel.GetDistance(monster.X, monster.Y, monster2.X, monster2.Y);

                                                            if (distance <= monster.MonsterInfo.AttackRange)
                                                            {
                                                                monster.MonsterInfo.LastMove = Time32.Now;
                                                                new Game.Attacking.Handle(null, monster, monster2);
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        #endregion
                                        #region Monsters
                                        else
                                        {
                                            short distance = Kernel.GetDistance(monster.X, monster.Y, Owner.Entity.X, Owner.Entity.Y);
                                            if (monster.MonsterInfo.InSight != 0 && monster.MonsterInfo.InSight != Owner.Entity.UID)
                                            {
                                                if (monster.MonsterInfo.InSight > 1000000)
                                                {
                                                    var cl = ServerBase.Kernel.GamePool[monster.MonsterInfo.InSight];
                                                    if (cl == null)
                                                        monster.MonsterInfo.InSight = 0;
                                                    else
                                                    {
                                                        short dst = Kernel.GetDistance(monster.X, monster.Y, cl.Entity.X, cl.Entity.Y);
                                                        if (dst > ServerBase.Constants.pScreenDistance)
                                                            monster.MonsterInfo.InSight = 0;
                                                    }
                                                }
                                                else
                                                {
                                                    Entity companion = Owner.Map.Companions[monster.MonsterInfo.InSight];
                                                    if (companion == null)
                                                        monster.MonsterInfo.InSight = 0;
                                                    else
                                                    {
                                                        short dst = Kernel.GetDistance(monster.X, monster.Y, companion.X, companion.Y);
                                                        if (dst > ServerBase.Constants.pScreenDistance)
                                                            monster.MonsterInfo.InSight = 0;
                                                    }
                                                }
                                            }
                                            if (distance <= ServerBase.Constants.pScreenDistance)
                                            {
                                                #region Companions
                                                if (Owner.Companion != null)
                                                {
                                                    if (Owner.Companion.Companion && !Owner.Companion.Dead)
                                                    {
                                                        short distance2 = Kernel.GetDistance(monster.X, monster.Y, Owner.Companion.X, Owner.Companion.Y);
                                                        if (distance > distance2 || Owner.Entity.ContainsFlag(Network.GamePackets.Update.Flags.Invisibility) || Owner.Entity.ContainsFlag(Network.GamePackets.Update.Flags.Fly))
                                                        {
                                                            if (monster.MonsterInfo.InSight == 0)
                                                            {
                                                                monster.MonsterInfo.InSight = Owner.Companion.UID;
                                                            }
                                                            else
                                                            {
                                                                if (monster.MonsterInfo.InSight == Owner.Companion.UID)
                                                                {
                                                                    if (distance2 > ServerBase.Constants.pScreenDistance)
                                                                    {
                                                                        monster.MonsterInfo.InSight = 0;
                                                                    }
                                                                    else
                                                                    {
                                                                        if (distance2 <= monster.MonsterInfo.AttackRange)
                                                                        {
                                                                            if (Time32.Now >= monster.MonsterInfo.LastMove.AddMilliseconds(monster.MonsterInfo.AttackSpeed))
                                                                            {
                                                                                monster.MonsterInfo.LastMove = Time32.Now;
                                                                                new Game.Attacking.Handle(null, monster, Owner.Companion);
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            if (distance2 > monster.MonsterInfo.ViewRange / 2)
                                                                            {
                                                                                if (distance2 < ServerBase.Constants.pScreenDistance)
                                                                                {
                                                                                    if (Time32.Now >= monster.MonsterInfo.LastMove.AddMilliseconds(monster.MonsterInfo.RunSpeed))
                                                                                    {
                                                                                        monster.MonsterInfo.LastMove = Time32.Now;

                                                                                        Enums.ConquerAngle facing = ServerBase.Kernel.GetAngle(monster.X, monster.Y, Owner.Companion.X, Owner.Companion.Y);
                                                                                        if (!monster.Move(facing))
                                                                                        {
                                                                                            facing = (Enums.ConquerAngle)ServerBase.Kernel.Random.Next(7);
                                                                                            if (monster.Move(facing))
                                                                                            {
                                                                                                monster.Facing = facing;
                                                                                                Network.GamePackets.GroundMovement move = new Conquer_Online_Server.Network.GamePackets.GroundMovement(true);
                                                                                                move.Direction = facing;
                                                                                                move.UID = monster.UID;
                                                                                                move.GroundMovementType = Network.GamePackets.GroundMovement.Run;
                                                                                                monster.MonsterInfo.SendScreen(move);
                                                                                            }
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            monster.Facing = facing;
                                                                                            Network.GamePackets.GroundMovement move = new Conquer_Online_Server.Network.GamePackets.GroundMovement(true);
                                                                                            move.Direction = facing;
                                                                                            move.UID = monster.UID;
                                                                                            move.GroundMovementType = Network.GamePackets.GroundMovement.Run;
                                                                                            monster.MonsterInfo.SendScreen(move);
                                                                                        }
                                                                                    }
                                                                                }
                                                                                else
                                                                                {
                                                                                    monster.MonsterInfo.InSight = 0;
                                                                                }
                                                                            }
                                                                            else
                                                                            {
                                                                                if (Time32.Now >= monster.MonsterInfo.LastMove.AddMilliseconds(monster.MonsterInfo.MoveSpeed))
                                                                                {
                                                                                    monster.MonsterInfo.LastMove = Time32.Now;
                                                                                    Enums.ConquerAngle facing = ServerBase.Kernel.GetAngle(monster.X, monster.Y, Owner.Companion.X, Owner.Companion.Y);
                                                                                    if (!monster.Move(facing))
                                                                                    {
                                                                                        facing = (Enums.ConquerAngle)ServerBase.Kernel.Random.Next(7);
                                                                                        if (monster.Move(facing))
                                                                                        {
                                                                                            monster.Facing = facing;
                                                                                            Network.GamePackets.GroundMovement move = new Conquer_Online_Server.Network.GamePackets.GroundMovement(true);
                                                                                            move.Direction = facing;
                                                                                            move.UID = monster.UID;
                                                                                            monster.MonsterInfo.SendScreen(move);
                                                                                        }
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        monster.Facing = facing;
                                                                                        Network.GamePackets.GroundMovement move = new Conquer_Online_Server.Network.GamePackets.GroundMovement(true);
                                                                                        move.Direction = facing;
                                                                                        move.UID = monster.UID;
                                                                                        monster.MonsterInfo.SendScreen(move);
                                                                                    }
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                                else
                                                                    goto Over;
                                                            }
                                                        }
                                                    }
                                                    else
                                                        goto Over;
                                                }
                                                else
                                                    goto Over;
                                                #endregion
                                            Over:
                                                #region Player
                                                if (monster.MonsterInfo.InSight == 0)
                                                {
                                                    if (distance <= monster.MonsterInfo.ViewRange)
                                                    {
                                                        if (!Owner.Entity.ContainsFlag(Network.GamePackets.Update.Flags.Invisibility))
                                                        {
                                                            if (monster.MonsterInfo.SpellID != 0 || !Owner.Entity.ContainsFlag(Network.GamePackets.Update.Flags.Fly))
                                                            {
                                                                monster.MonsterInfo.InSight = Owner.Entity.UID;
                                                            }
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    if (monster.MonsterInfo.InSight == Owner.Entity.UID)
                                                    {
                                                        if (monster.MonsterInfo.SpellID == 0 && Owner.Entity.ContainsFlag(Network.GamePackets.Update.Flags.Fly))
                                                        {
                                                            monster.MonsterInfo.InSight = 0;
                                                            return;
                                                        }

                                                        if (Owner.Entity.Dead)
                                                        {
                                                            monster.MonsterInfo.InSight = 0;
                                                            return;
                                                        }
                                                        if (distance > ServerBase.Constants.pScreenDistance)
                                                        {
                                                            monster.MonsterInfo.InSight = 0;
                                                        }
                                                        else
                                                        {
                                                            if (distance <= monster.MonsterInfo.AttackRange)
                                                            {
                                                                if (Time32.Now >= monster.MonsterInfo.LastMove.AddMilliseconds(monster.MonsterInfo.AttackSpeed))
                                                                {
                                                                    monster.MonsterInfo.LastMove = Time32.Now;
                                                                    new Game.Attacking.Handle(null, monster, Owner.Entity);
                                                                }
                                                            }
                                                            else
                                                            {
                                                                if (distance > monster.MonsterInfo.ViewRange / 2)
                                                                {
                                                                    if (distance < ServerBase.Constants.pScreenDistance)
                                                                    {
                                                                        if (Time32.Now >= monster.MonsterInfo.LastMove.AddMilliseconds(monster.MonsterInfo.RunSpeed))
                                                                        {
                                                                            monster.MonsterInfo.LastMove = Time32.Now;

                                                                            Enums.ConquerAngle facing = ServerBase.Kernel.GetAngle(monster.X, monster.Y, Owner.Entity.X, Owner.Entity.Y);
                                                                            if (!monster.Move(facing))
                                                                            {
                                                                                facing = (Enums.ConquerAngle)ServerBase.Kernel.Random.Next(7);
                                                                                if (monster.Move(facing))
                                                                                {
                                                                                    monster.Facing = facing;
                                                                                    Network.GamePackets.GroundMovement move = new Conquer_Online_Server.Network.GamePackets.GroundMovement(true);
                                                                                    move.Direction = facing;
                                                                                    move.UID = monster.UID;
                                                                                    move.GroundMovementType = Network.GamePackets.GroundMovement.Run;
                                                                                    monster.MonsterInfo.SendScreen(move);
                                                                                }
                                                                            }
                                                                            else
                                                                            {
                                                                                monster.Facing = facing;
                                                                                Network.GamePackets.GroundMovement move = new Conquer_Online_Server.Network.GamePackets.GroundMovement(true);
                                                                                move.Direction = facing;
                                                                                move.UID = monster.UID;
                                                                                move.GroundMovementType = Network.GamePackets.GroundMovement.Run;
                                                                                monster.MonsterInfo.SendScreen(move);
                                                                            }
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        monster.MonsterInfo.InSight = 0;
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    if (Time32.Now >= monster.MonsterInfo.LastMove.AddMilliseconds(monster.MonsterInfo.MoveSpeed))
                                                                    {
                                                                        monster.MonsterInfo.LastMove = Time32.Now;
                                                                        Enums.ConquerAngle facing = ServerBase.Kernel.GetAngle(monster.X, monster.Y, Owner.Entity.X, Owner.Entity.Y);
                                                                        if (!monster.Move(facing))
                                                                        {
                                                                            facing = (Enums.ConquerAngle)ServerBase.Kernel.Random.Next(7);
                                                                            if (monster.Move(facing))
                                                                            {
                                                                                monster.Facing = facing;
                                                                                Network.GamePackets.GroundMovement move = new Conquer_Online_Server.Network.GamePackets.GroundMovement(true);
                                                                                move.Direction = facing;
                                                                                move.UID = monster.UID;
                                                                                monster.MonsterInfo.SendScreen(move);
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            monster.Facing = facing;
                                                                            Network.GamePackets.GroundMovement move = new Conquer_Online_Server.Network.GamePackets.GroundMovement(true);
                                                                            move.Direction = facing;
                                                                            move.UID = monster.UID;
                                                                            monster.MonsterInfo.SendScreen(move);
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                                #endregion
                                            }
                                            else
                                            {
                                                toRemove.Add(obj);
                                            }
                                        }
                                        #endregion
                                    }
                                }
                                #endregion
                            }
                        }
                        if (obj.MapObjType == MapObjectType.Item)
                        {
                            Network.GamePackets.FloorItem item = obj as Network.GamePackets.FloorItem;
                            if (Time32.Now > item.OnFloor.AddSeconds(ServerBase.Constants.FloorItemSeconds))
                            {
                                item.Type = Network.GamePackets.FloorItem.Remove;
                                foreach (Interfaces.IMapObject _obj in Objects)
                                {
                                    if (_obj != null)
                                    {
                                        if (_obj.MapObjType == MapObjectType.Player)
                                        {
                                            (_obj as Entity).Owner.Send(item);
                                        }
                                    }
                                }
                                Owner.Map.Floor[item.X, item.Y, MapObjectType.Item, null] = true;
                                toRemove.Add(item);
                            }
                        }
                    }
                }
            }
            catch (Exception e) { Program.SaveException(e); }

            foreach (IMapObject obj in toRemove)
                Remove(obj);
            #endregion
        }

        public bool Add(Interfaces.IMapObject _object)
        {
            try
            {
                //if (Monitor.TryEnter(_objects, 10))
                //{
                if (!_objects.ContainsKey(_object.UID))
                {
                    if (ServerBase.Kernel.GetDistance(_object.X, _object.Y, Owner.Entity.X, Owner.Entity.Y) <= ServerBase.Constants.pScreenDistance)
                    {
                        _objects.Add(_object.UID, _object);
                        UpdateArray();
                        return true;
                    }
                }
                else
                {
                    if (_objects[_object.UID] == null)
                    {
                        _objects.Remove(_object.UID);
                        if (ServerBase.Kernel.GetDistance(_object.X, _object.Y, Owner.Entity.X, Owner.Entity.Y) <= ServerBase.Constants.pScreenDistance)
                        {
                            _objects.Add(_object.UID, _object);
                            UpdateArray();
                            return true;
                        }
                    }
                }
                //}
            }
            catch
            {

            }
            finally
            {
                //Monitor.Exit(_objects);
            }
            return false;
        }

        public bool Remove(Interfaces.IMapObject _object)
        {
            try
            {
                //if (Monitor.TryEnter(_objects, 10))
                //{
                if (_objects.ContainsKey(_object.UID))
                {
                    if (_object.MapObjType == MapObjectType.Item)
                    {
                        Network.GamePackets.FloorItem item = _object as Network.GamePackets.FloorItem;
                        item.Type = Network.GamePackets.FloorItem.Remove;
                        item.SendSpawn(Owner, false);
                        item.Type = Network.GamePackets.FloorItem.Drop;
                    }
                    else if (_object.MapObjType == MapObjectType.Player)
                    {
                        Owner.Send(new Network.GamePackets.Data(true) { UID = _object.UID, ID = Network.GamePackets.Data.RemoveEntity });
                    }
                    _objects.Remove(_object.UID);
                    UpdateArray();
                    return true;
                }
                //}
            }
            catch
            {

            }
            finally
            {
                //Monitor.Exit(_objects);
            }
            return false;
        }
        public bool TryGetValue(uint uid, out Entity entity)
        {
            entity = null;
            Interfaces.IMapObject imo = null;
            _objects.TryGetValue(uid, out imo);
            if (imo == null)
            {
                if (_objects.ContainsKey(uid))
                    _objects.Remove(uid);
                return false;
            }
            if (imo.MapObjType == MapObjectType.Player || imo.MapObjType == MapObjectType.Monster)
                entity = imo as Game.Entity;
            if (entity == null)
                return false;
            return true;
        }
        public bool TryGetSob(uint uid, out Network.GamePackets.SobNpcSpawn sob)
        {
            sob = null;
            Interfaces.IMapObject imo = null;
            _objects.TryGetValue(uid, out imo);
            if (imo == null)
            {
                if (_objects.ContainsKey(uid))
                    _objects.Remove(uid);
                return false;
            }

            if (imo.MapObjType == MapObjectType.SobNpc)
                sob = imo as Network.GamePackets.SobNpcSpawn;

            if (sob == null)
                return false;
            return true;
        }
        public bool TryGetFloorItem(uint uid, out Network.GamePackets.FloorItem item)
        {
            item = null;

            Interfaces.IMapObject imo = null;
            _objects.TryGetValue(uid, out imo);
            if (imo == null)
            {
                if (_objects.ContainsKey(uid))
                    _objects.Remove(uid);
                return false;
            }
            if (imo.MapObjType == MapObjectType.Item)
                item = imo as Network.GamePackets.FloorItem;

            if (item == null)
                return false;
            return true;
        }
        public bool Contains(Interfaces.IMapObject _object)
        {
            return _objects.ContainsKey(_object.UID);
        }
        public bool Contains(uint uid)
        {
            return _objects.ContainsKey(uid);
        }

        void UpdateArray()
        {
            Objects = GenericArray<Interfaces.IMapObject>.ToArray(_objects.Values, _objects.Count);
        }
        public class GenericArray<T>
        {
            public static T[] ToArray(IEnumerable<T> source, int count)
            {
                var array = new T[count];
                try
                {
                    if (Monitor.TryEnter(source, 10))
                    {
                        var enumerator = source.GetEnumerator();
                        enumerator.MoveNext();
                        for (int i = 0; i < count; i++)
                        {
                            array[i] = enumerator.Current;
                            enumerator.MoveNext();
                        }
                    }
                }
                catch
                {
                    return new T[0];
                }
                finally
                {
                    Monitor.Exit(source);
                }
                return array;
            }
        }
        public void CleanUp(Interfaces.IPacket spawnWith)
        {
            bool remove;
            try
            {
                foreach (IMapObject Base in Objects)
                {
                    if (Base == null)
                        continue;
                    remove = false;
                    if (Base.MapObjType == MapObjectType.Monster)
                    {
                        if ((Base as Entity).Dead)
                        {
                            if (Time32.Now > (Base as Entity).DeathStamp.AddSeconds(8))
                                remove = true;
                            else
                                remove = false;
                        }
                        if (ServerBase.Kernel.GetDistance(Owner.Entity.X, Owner.Entity.Y, Base.X, Base.Y) >= Constants.remScreenDistance)
                            remove = true;
                        if (remove)
                        {
                            if ((Base as Entity).MonsterInfo.InSight == Owner.Entity.UID)
                                (Base as Entity).MonsterInfo.InSight = 0;
                        }
                    }
                    else if (Base.MapObjType == MapObjectType.Player)
                    {
                        if (remove = (ServerBase.Kernel.GetDistance(Owner.Entity.X, Owner.Entity.Y, Base.X, Base.Y) >= Constants.pScreenDistance))
                        {
                            GameState pPlayer = Base.Owner as GameState;
                            pPlayer.Screen.Remove(Owner.Entity);
                        }
                    }
                    else if (Base.MapObjType == MapObjectType.Item)
                    {
                        remove = (Kernel.GetDistance(Owner.Entity.X, Owner.Entity.Y, Base.X, Base.Y) >= 22);

                    }
                    else
                    {
                        remove = (Kernel.GetDistance(Owner.Entity.X, Owner.Entity.Y, Base.X, Base.Y) >= Constants.remScreenDistance);
                    }
                    if (Base.MapID != Owner.Map.ID)
                        remove = true;
                    if (remove)
                    {
                        Remove(Base);
                    }
                }
            }
            catch (Exception e) { Program.SaveException(e); }
        }
        public void Clear()
        {
            _objects.Clear();
            Objects = new IMapObject[0];
        }
        public void FullWipe()
        {
            bool remove;
            try
            {
                foreach (IMapObject Base in Objects)
                {
                    if (Base == null)
                        continue;
                    remove = true;
                    if (Base.MapObjType == MapObjectType.Monster)
                    {
                        remove = (Base as IBaseEntity).Dead ||
                            (ServerBase.Kernel.GetDistance(Owner.Entity.X, Owner.Entity.Y, Base.X, Base.Y) >= Constants.pScreenDistance);
                    }
                    else if (Base.MapObjType == MapObjectType.Player)
                    {
                        GameState pPlayer = Base.Owner as GameState;
                        pPlayer.Screen.Remove(Owner.Entity);
                        remove = true;
                    }
                    else if (Base.MapObjType == MapObjectType.Item)
                    {
                        remove = (Kernel.GetDistance(Owner.Entity.X, Owner.Entity.Y, Base.X, Base.Y) >= 22);

                    }
                    else
                    {
                        remove = (Kernel.GetDistance(Owner.Entity.X, Owner.Entity.Y, Base.X, Base.Y) >= Constants.pScreenDistance);
                    }
                    if (Base.MapID != Owner.Map.ID)
                        remove = true;
                    if (remove)
                    {
                        Network.GamePackets.Data data = new Network.GamePackets.Data(true);
                        data.UID = Owner.Entity.UID;
                        data.ID = Network.GamePackets.Data.RemoveEntity;

                        if (Base.MapObjType == Game.MapObjectType.Player)
                        {
                            GameState pPlayer = Base.Owner as GameState;
                            pPlayer.Send(data);
                        }
                    }
                }
            }
            catch (Exception e) { Program.SaveException(e); }
            Clear();
        }
        public void Reload(Interfaces.IPacket spawnWith)
        {
            CleanUp(spawnWith);
            try
            {
                foreach (GameState pClient in Kernel.GamePool.Values)
                {
                    if (pClient.Entity.UID != Owner.Entity.UID)
                    {
                        if (pClient.Map.ID == Owner.Map.ID)
                        {
                            short dist = Kernel.GetDistance(pClient.Entity.X, pClient.Entity.Y, Owner.Entity.X, Owner.Entity.Y);
                            if (dist <= Constants.pScreenDistance && !Contains(pClient.Entity))
                            {
                                if (pClient.Guild != null)
                                    pClient.Guild.SendName(Owner);
                                if (Owner.Guild != null)
                                    Owner.Guild.SendName(pClient);
                                if (pClient.Map.BaseID == 700)
                                {
                                    if (Owner.QualifierGroup != null)
                                    {
                                        if (pClient.QualifierGroup != null)
                                        {
                                            Owner.Entity.SendSpawn(pClient);
                                            pClient.Entity.SendSpawn(Owner);
                                            if (pClient.Guild != null)
                                                Owner.Entity.SendSpawn(pClient, false);
                                            if (Owner.Guild != null)
                                                pClient.Entity.SendSpawn(Owner, false);
                                            if (spawnWith != null)
                                                pClient.Send(spawnWith);
                                        }
                                        else
                                        {
                                            Owner.Entity.SendSpawn(pClient);

                                            if (pClient.Guild != null)
                                                Owner.Entity.SendSpawn(pClient, false);
                                            Add(pClient.Entity);
                                            if (spawnWith != null)
                                                pClient.Send(spawnWith);
                                        }
                                    }
                                    else
                                    {
                                        if (pClient.QualifierGroup != null)
                                        {
                                            pClient.Entity.SendSpawn(Owner);
                                            if (Owner.Guild != null)
                                                pClient.Entity.SendSpawn(Owner, false);
                                            pClient.Screen.Add(Owner.Entity);
                                            if (spawnWith != null)
                                                Owner.Send(spawnWith);
                                        }
                                        else
                                        {
                                            Owner.Entity.SendSpawn(pClient);
                                            pClient.Entity.SendSpawn(Owner);

                                            if (pClient.Guild != null)
                                                Owner.Entity.SendSpawn(pClient, false);
                                            if (Owner.Guild != null)
                                                pClient.Entity.SendSpawn(Owner, false);

                                            if (spawnWith != null)
                                                pClient.Send(spawnWith);
                                        }
                                    }
                                }
                                else
                                {
                                    Owner.Entity.SendSpawn(pClient);
                                    pClient.Entity.SendSpawn(Owner);

                                    if (pClient.Guild != null)
                                        Owner.Entity.SendSpawn(pClient, false);
                                    if (Owner.Guild != null)
                                        pClient.Entity.SendSpawn(Owner, false);

                                    if (spawnWith != null)
                                        pClient.Send(spawnWith);
                                }
                            }
                        }
                    }
                }
                if (Owner.Map != null)
                {
                    int X = Owner.Entity.X, Y = Owner.Entity.Y;
                    for (int extrax = -16; extrax < 16; extrax++)
                    {
                        for (int extray = -16; extray < 16; extray++)
                        {
                            var tile = Owner.Map.Floor.GetLocation(X + extrax, Y + extray);
                            if (tile != null)
                            {
                                if (tile.Item != null)
                                {
                                    short dist = ServerBase.Kernel.GetDistance(Owner.Entity.PX, Owner.Entity.PY, (ushort)(X + extrax), (ushort)(Y + extray));

                                    if (dist >= 16)
                                    {
                                        var item = tile.Item;
                                        if (Time32.Now > item.OnFloor.AddSeconds(ServerBase.Constants.FloorItemSeconds))
                                        {
                                            item.Type = Network.GamePackets.FloorItem.Remove;
                                            foreach (Interfaces.IMapObject _obj in Objects)
                                            {
                                                if (_obj != null)
                                                {
                                                    if (_obj.MapObjType == MapObjectType.Player)
                                                    {
                                                        (_obj as Entity).Owner.Send(item);
                                                    }
                                                }
                                            }
                                            Owner.Map.Floor[item.X, item.Y, MapObjectType.Item, null] = true;
                                        }
                                        else
                                            tile.Item.SendSpawn(Owner, false);
                                    }
                                }
                                if (tile.Npc != null)
                                {
                                    short dist = ServerBase.Kernel.GetDistance(Owner.Entity.PX, Owner.Entity.PY, (ushort)(X + extrax), (ushort)(Y + extray));
                                    if (dist <= 18 || dist >= 16)
                                    {
                                        bool add = true;
                                        if (tile.Npc.Type == Enums.NpcType.Talker && Owner.Screen.Contains((IMapObject)tile.Npc))
                                        {
                                            add = false;
                                        }
                                        if (add)
                                            tile.Npc.SendSpawn(Owner);
                                    }
                                }
                            }
                        }
                    }
                    /*foreach (KeyValuePair<uint, INpc> DE in Owner.Map.Npcs)
                    {
                        INpc npc = DE.Value;
                        if (npc == null) continue;
                        if (Kernel.GetDistance(npc.X, npc.Y, Owner.Entity.X, Owner.Entity.Y) <= Constants.nScreenDistance && !Contains(npc.UID))
                        {
                            npc.SendSpawn(Owner);
                        }
                    }
                    foreach (Network.GamePackets.FloorItem floorItem in Owner.Map.FloorItems.Values)
                    {
                        if (floorItem == null) continue;
                        if (Kernel.GetDistance(floorItem.X, floorItem.Y, Owner.Entity.X, Owner.Entity.Y) <= Constants.nScreenDistance && !Contains(floorItem.UID))
                        {
                            floorItem.SendSpawn(Owner, false);
                        }
                    }*/
                    foreach (Game.Entity monster in Owner.Map.Entities.Values)
                    {
                        if (monster == null) continue;
                        if (Kernel.GetDistance(monster.X, monster.Y, Owner.Entity.X, Owner.Entity.Y) <= Constants.nScreenDistance && !Contains(monster.UID))
                        {
                            if (!monster.Dead)
                            {
                                monster.SendSpawn(Owner);
                                if (monster.MaxHitpoints > 65535)
                                {
                                    Update upd = new Update(true) { UID = monster.UID };
                                    upd.Append(Update.Hitpoints, monster.Hitpoints);
                                    Owner.Send(upd);
                                }
                            }
                        }
                    }
                    foreach (Game.Entity monster in Owner.Map.Companions.Values)
                    {
                        if (monster == null) continue;
                        if (Kernel.GetDistance(monster.X, monster.Y, Owner.Entity.X, Owner.Entity.Y) <= Constants.nScreenDistance && !Contains(monster.UID))
                        {
                            if (!monster.Dead)
                            {
                                monster.SendSpawn(Owner);
                            }
                        }
                    }
                }
            }
            catch (Exception e) { Program.SaveException(e); }
            UpdateArray();
        }

        internal void SendScreen(_String str, bool p)
        {

        }
    }
}
