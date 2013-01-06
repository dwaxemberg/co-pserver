using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer_Online_Server.Network.GamePackets
{
  #region Class Level
    public class SubClassShowFull : Writer, Interfaces.IPacket
    {
        public const byte
        SwitchSubClass = 0,
        ActivateSubClass = 1,
        LearnSubClass = 4,
        ShowGUI = 7;

        byte[] Buffer;
        public SubClassShowFull(bool Create)
        {
            if (Create)
            {
                Buffer = new byte[34];
                WriteUInt16(26, 0, Buffer);
                WriteUInt16(2320, 2, Buffer);
            }
        }

        public ushort ID
        {
            get { return BitConverter.ToUInt16(Buffer, 4); }
            set { WriteUInt16(value, 4, Buffer); }
        }

        public byte Class
        {
            get { return Buffer[6]; }
            set { Buffer[6] = value; }
        }

        public byte Level
        {
            get { return Buffer[7]; }
            set { Buffer[7] = value; }
        }

        public void Deserialize(byte[] buffer)
        {
            this.Buffer = buffer;
        }

        public byte[] ToArray()
        {
            return Buffer;
        }

        public void Send(Client.GameState c)
        {
            c.Send(Buffer);
        }
    }
    #endregion
    #region Class Send
    public class SubClass : Writer, Interfaces.IPacket
    {
        public const byte
        SwitchSubClass = 0,
        ActivateSubClass = 1,
        ShowSubClasses = 7,
        MartialPromoted = 5,
        LearnSubClass = 4;
        Game.Entity Owner = null;

        byte[] Buffer;
        byte Type;
        public SubClass(Game.Entity E) {  Owner = E; Type = 7; }

        public void Deserialize(byte[] buffer)
        {
            this.Buffer = buffer;
        }

        public byte[] ToArray()
        {
            Buffer = new byte[8 + 26 + (Owner.SubClasses.Classes.Count * 3)];
            WriteUInt16((ushort)(Buffer.Length - 8), 0, Buffer);
            WriteUInt16(2320, 2, Buffer);
            WriteUInt16(Type, 4, Buffer);
            WriteUInt64(Owner.SubClasses.StudyPoints, 6, Buffer);
           WriteUInt16((ushort)Owner.SubClasses.Classes.Count, 22, Buffer);
            int Position = 26;
            if (Owner.SubClasses.Classes.Count > 0)
            {
                Game.SubClass[] Classes = new Game.SubClass[Owner.SubClasses.Classes.Count];
                Owner.SubClasses.Classes.Values.CopyTo(Classes, 0);
                foreach (Game.SubClass Class in Classes)
                {
                    
                    WriteByte(Class.ID, Position, Buffer); Position++;
                    WriteByte(Class.Phase, Position, Buffer); Position++;
                    WriteByte(Class.Level, Position, Buffer); Position++;
                }
            }
            WriteString("TQServer", (Buffer.Length - 8), Buffer);
            return Buffer;
        }

        public void Send(Client.GameState c)
        {
            c.Send(Buffer);
        }
    }
    #endregion
}
