using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Conquer_Online_Server.Game.ConquerStructures.Society;

namespace Conquer_Online_Server.Network.GamePackets
{
    public class Guild_Info : Writer
    {
        byte[] Buffer;
        Guild Guild;
        public Guild_Info(Guild _Guild)
        {
            Guild = _Guild;
            Build();
        }

        public void Build()
        {
            if (Guild == null) return;
            int Position = 48;
            Buffer = new byte[76 + Guild.LeaderName.Length + 8];
            WriteUInt16((ushort)(Buffer.Length - 8), 0, Buffer);
            WriteUInt16(1106, 2, Buffer);
            WriteUInt32(Guild.ID, 4, Buffer);
            WriteUInt64(Guild.SilverFund, 12, Buffer);
            WriteUInt32(Guild.ConquerPointFund, 20, Buffer);
            WriteUInt32((uint)Guild.Members.Count, 24, Buffer);
            WriteUInt16((ushort)Guild.HeroRank, 28, Buffer);
            WriteString(Guild.Leader.Name, 32, Buffer); Position += Guild.Leader.Name.Length;
            WriteByte((byte)Guild.Level, (Position + 1), Buffer); Position += 8;
            WriteUInt32((uint)Guild.EnroleDate, Position, Buffer);
        }

        public byte[] ToArray { get { return Buffer; } }
    }
}
