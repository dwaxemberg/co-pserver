using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer_Online_Server.Network.GamePackets
{
    public class BlessingContribution : Writer, Interfaces.IPacket
    {
        public const byte Online_Training = 1, Blessing_Contribution = 2;
        public bool Loaded = false;
        uint Owner = 0;
        byte[] Buffer;

        public BlessingContribution(uint _Owner)
        {
            if (_Owner > 0)
            {
                Owner = _Owner;
                Buffer = new byte[16 + 8];
                WriteUInt16((ushort)(Buffer.Length - 8), 0, Buffer);
                WriteUInt16(1036, 0, Buffer);
            }
        }

        public byte Type
        {
            get { return Buffer[4]; }
            set { Buffer[4] = value; }
        }

        public uint Training_Exp
        {
            get { return BitConverter.ToUInt32(Buffer, 8); }
            set { WriteUInt32(value, 8, Buffer); Update(value, "Training_Exp"); }
        }

        public uint Blessing_Exp
        {
            get { return BitConverter.ToUInt32(Buffer, 12); }
            set { WriteUInt32(value, 12, Buffer); Update(value, "Blessing_Exp"); }
        }

        public void Update(uint value, string Colum)
        {
            if (Loaded)
            {
                Database.MySqlCommand cmd = new Database.MySqlCommand(Database.MySqlCommandType.UPDATE);
                cmd.Update("entities").Set("Blessing_Exp", value).Where("UID", Owner).Execute();
            }
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