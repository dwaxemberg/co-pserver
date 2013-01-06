using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer_Online_Server.Network.GamePackets
{
    public class ClanMembers : Writer
    {
        public byte[] ToArray() { return buffer; }
        byte[] buffer;
        public ClanMembers(Client.GameState client)
        {

            uint descarse = (uint)client.Entity.Myclan.Members.Count;
            buffer = new byte[8 + 56 + 36 * client.Entity.Myclan.Members.Count];
            WriteUInt16((ushort)(buffer.Length - 8), 0, buffer);
            WriteUInt16(1312, 2, buffer);
            buffer[4] = 4;
            int Position = 16;
            foreach (Game.ClanMembers member in client.Entity.Myclan.Members.Values)
            {
                WriteUInt32(descarse, Position, buffer);
                Position += 4;
                WriteString(member.Name, Position, buffer);
                Position += 16;
                WriteUInt32(member.Level, Position, buffer);
                Position += 4;
                WriteUInt16((ushort)member.Rank, Position, buffer);
                Position += 2;
                if (Conquer_Online_Server.ServerBase.Kernel.GamePool.ContainsKey(member.UID))
                    WriteUInt16(1, Position, buffer);
                Position += 2;
                WriteUInt32(member.Class, Position, buffer);
                Position += 4;
                WriteUInt32(1, Position, buffer);
                Position += 4;
                descarse -= 1;
            }
        }
    }
}
