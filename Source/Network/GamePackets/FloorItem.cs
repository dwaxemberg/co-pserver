using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer_Online_Server.Network.GamePackets
{
    public class FloorItem : Writer, Interfaces.IPacket, Interfaces.IMapObject
    {
        public const ushort Drop = 1, 
            Remove = 2, 
            Animation = 3,
            DropDetain = 4;
        public bool PickedUpAlready = false;
        public static ServerBase.Counter FloorUID = new ServerBase.Counter(0);
        byte[] Buffer;
        Client.GameState owner;
        ushort mapid;
        public Time32 OnFloor;
        public uint Value;
        public FloorValueType ValueType;
        private Interfaces.IConquerItem item;
        public FloorItem(bool Create)
        {
            if (Create)
            {
                Buffer = new byte[24 + 8];
                WriteUInt16(24, 0, Buffer);
                WriteUInt16(1101, 2, Buffer);
                Value = 0;
                ValueType = FloorValueType.Item;
            }
        }
        public uint UID
        {
            get { return BitConverter.ToUInt32(Buffer, 4); }
            set { WriteUInt32(value, 4, Buffer); }
        }
        public uint ItemID
        {
            get { return BitConverter.ToUInt32(Buffer, 8); }
            set { WriteUInt32(value, 8, Buffer); }
        }
        public ushort X
        {
            get { return BitConverter.ToUInt16(Buffer, 12); }
            set { WriteUInt16(value, 12, Buffer); }
        }
        public ushort Y
        {
            get { return BitConverter.ToUInt16(Buffer, 14); }
            set { WriteUInt16(value, 14, Buffer); }
        }
        public Game.Enums.Color ItemColor
        {
            get { return (Game.Enums.Color)BitConverter.ToUInt16(Buffer, 16); }
            set { WriteUInt16((ushort)value, 16, Buffer); }
        }
        public ushort Type
        {
            get { return BitConverter.ToUInt16(Buffer, 18); }
            set { WriteUInt32(value, 18, Buffer); }
        }
        public Conquer_Online_Server.Game.MapObjectType MapObjType
        {
            get { return Conquer_Online_Server.Game.MapObjectType.Item; }
            set { }
        }
        public void Send(Client.GameState client)
        {
            client.Send(Buffer);
        }
        public void SendSpawn(Client.GameState client)
        {
            SendSpawn(client, false);
        }
        public Client.GameState Owner
        {
            get { return owner; }
            set { owner = value; }
        }
        public Interfaces.IConquerItem Item
        {
            get { return item; }
            set { item = value; }
        }
        public ushort MapID
        {
            get { return mapid; }
            set { mapid = value; }
        }
        public void SendSpawn(Client.GameState client, bool checkScreen)
        {
            if (client.Screen.Add(this) || !checkScreen)
            {
                client.Send(Buffer);
            }
        }
        public byte[] ToArray()
        {
            return Buffer;
        }
        public void Deserialize(byte[] buffer)
        {
            Buffer = buffer;
        }
        public enum FloorValueType { Item, Money, ConquerPoints }
    }
}
