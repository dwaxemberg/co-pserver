using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer_Online_Server.Network.GamePackets
{
    public class GuildCommand : Writer, Interfaces.IPacket
    {
        public const uint
                    JoinRequest = 1,
                    InviteRequest = 2,
                    Quit = 3,
                    Info = 6,
                    Allied = 7,
                    Neutral1 = 8,
                    Enemied = 9,
                    Neutral2 = 10,
                    DonateSilvers = 11,
                    Refresh = 12,
                    Disband = 19,
                    DonateConquerPoints = 20,
                    Bulletin = 27,
                    Discharge = 30,
                    Promote = 37;

        private byte[] Buffer;
        public GuildCommand(bool Create)
        {
            if (Create)
            {
                Buffer = new byte[8 + 28];
                WriteUInt16(28, 0, Buffer);
                WriteUInt16(1107, 2, Buffer);
            }
        }

        public uint Type
        {
            get { return BitConverter.ToUInt32(Buffer, 4); }
            set { WriteUInt32(value, 4, Buffer); }
        }

        public uint dwParam
        {
            get { return BitConverter.ToUInt32(Buffer, 8); }
            set { WriteUInt32(value, 8, Buffer); }
        }

        public void Deserialize(byte[] Data)
        {
            Buffer = Data;
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
