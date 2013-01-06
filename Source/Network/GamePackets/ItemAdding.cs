using System;

namespace Conquer_Online_Server.Network.GamePackets
{
    public class ItemAdding : Writer, Interfaces.IPacket
    {
        public struct Purification_
        {
            public bool Available;
            public uint ItemUID;
            public uint PurificationItemID;
            public uint PurificationLevel;
            /// <summary>
            /// In minutes.
            /// </summary>
            public uint PurificationDuration;
            public DateTime AddedOn;
        }
        public struct Refinery_
        {
            public bool Available;
            public uint ItemUID;
            public uint EffectID;
            public uint EffectLevel;
            public uint EffectPercent;
            /// <summary>
            /// In minutes.
            /// </summary>
            public uint EffectDuration;
            public DateTime AddedOn;
        }

        public const uint ExtraEffect = 2, PurificationAdding = 6;
        
        byte[] Buffer;
        const byte minBufferSize = 8;
        public ItemAdding(bool Create)
        {
            if (Create)
            {
                Buffer = new byte[minBufferSize + 8];
                WriteUInt16(minBufferSize, 0, Buffer);
                WriteUInt16(2077, 2, Buffer);
            }
        }

        public uint UpdateCount
        {
            get { return BitConverter.ToUInt32(Buffer, 4); }
            set
            {
                byte[] buffer = new byte[minBufferSize + 8 + 28 * value];
                int count = buffer.Length;
                if (count > Buffer.Length)
                    count = Buffer.Length;
                System.Buffer.BlockCopy(Buffer, 0, buffer, 0, count);
                WriteUInt16((ushort)(minBufferSize + 28 * value), 0, buffer);
                Buffer = buffer;
                WriteUInt32(value, 4, Buffer);
            }
        }

        public bool Append(Purification_ purify)
        {
            UpdateCount = UpdateCount + 1;
            ushort offset = (ushort)(8 + (UpdateCount - 1) * 28);
            WriteUInt32(purify.ItemUID, offset, Buffer);
            WriteUInt32(PurificationAdding, offset + 4, Buffer);
            WriteUInt32(purify.PurificationItemID, offset + 8, Buffer);
            WriteUInt32(purify.PurificationLevel, offset + 12, Buffer);
            if (purify.PurificationDuration != 0)
            {
                TimeSpan span1 = new TimeSpan(purify.AddedOn.AddSeconds(purify.PurificationDuration).Ticks);
                TimeSpan span2 = new TimeSpan(DateTime.Now.Ticks);
                int secondsleft = (int)(span1.TotalSeconds - span2.TotalSeconds);
                if (secondsleft <= 0)
                {
                    purify.Available = false;
                    UpdateCount = UpdateCount - 1;
                    return false;
                }
                WriteUInt32((uint)secondsleft, offset + 20, Buffer);
            }
            return true;
        }

        public bool Append(Refinery_ effect)
        {
            UpdateCount = UpdateCount + 1;
            ushort offset = (ushort)(8 + (UpdateCount - 1) * 28);
            WriteUInt32(effect.ItemUID, offset, Buffer);
            WriteUInt32(ExtraEffect, offset + 4, Buffer);
            WriteUInt32(effect.EffectID, offset + 8, Buffer);
            WriteUInt32(effect.EffectLevel, offset + 12, Buffer);
            WriteUInt32(effect.EffectPercent, offset + 16, Buffer);
            if (effect.EffectPercent != 0)
            {
                TimeSpan span1 = new TimeSpan(effect.AddedOn.AddSeconds(effect.EffectPercent).Ticks);
                TimeSpan span2 = new TimeSpan(DateTime.Now.Ticks);
                int secondsleft = (int)(span1.TotalSeconds - span2.TotalSeconds);
                if (secondsleft <= 0)
                {
                    effect.Available = false;
                    UpdateCount -= 1;
                    return false;
                }
                WriteUInt32((uint)secondsleft, offset + 20, Buffer);
            }
            return true;
        }

        public byte[] ToArray()
        {
            return Buffer;
        }

        public void Deserialize(byte[] buffer)
        {
            Buffer = buffer;
        }

        public void Send(Client.GameState client)
        {
            client.Send(Buffer);
        }
    }
}
