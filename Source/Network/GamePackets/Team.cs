using System;
using System.Drawing;
using Conquer_Online_Server.Game;
namespace Conquer_Online_Server.Network.GamePackets
{
    public class Team : Writer, Interfaces.IPacket
    {
        public const ushort
             Create = 0,
             JoinRequest = 1,
             ExitTeam = 2,
             AcceptInvitation = 3,
             InviteRequest = 4,
             AcceptJoinRequest = 5,
             Dismiss = 6,
             Kick = 7,
             ForbidJoining = 8,
             UnforbidJoining = 9,
             LootMoneyOff = 10,
             LootMoneyOn = 11,
             LootItemsOff = 12,
             LootItemsOn = 13;

        byte[] Buffer;
        public Team()
        {
            Buffer = new byte[12 + 8];
            WriteUInt16(12, 0, Buffer);
            WriteUInt16(1023, 2, Buffer);
        }
        public uint Type
        {
            get { return BitConverter.ToUInt32(Buffer, 4); }
            set { WriteUInt32(value, 4, Buffer); }
        }
        public uint UID
        {
            get { return BitConverter.ToUInt32(Buffer, 8); }
            set { WriteUInt32(value, 8, Buffer); }
        }
        public void Deserialize(byte[] buffer)
        {
            Buffer = buffer;
        }
        public byte[] ToArray()
        {
            return Buffer;
        }
        public void Send(Client.GameState client)
        {
            client.Send(Buffer);
        }
    }
}
