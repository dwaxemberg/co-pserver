using System;
using System.Text;
using System.Drawing;

namespace Conquer_Online_Server.Network.GamePackets
{
    public class Message : Interfaces.IPacket
    {
        public string _From;
        public string _To;
        public uint ChatType;
        public Color Color;
        public string __Message;
        public uint Mesh;

        public const uint Talk = 2000,
                          Whisper = 2001,
                          Team = 2003,
                          Guild = 2004,
                          TopLeft = 2005,
                          Clan = 2006,
                          Qualifier = 2007,
                          Friend = 2009,
                          Center = 2011,
                          Service = 2014,
                          World = 2021,
                          PopUP = 2100,
                          Dialog = 2101,
                          HawkMessage = 2104,
                          Website = 2105,
                          FirstRightCorner = 2108,
                          ContinueRightCorner = 2109,
                          GuildBulletin = 2111,
                          BroadcastMessage = 2500;

        public Message(string _Message, Color _Color, uint _ChatType)
        {
            this.Mesh = 0;
            this.__Message = _Message;
            this._To = "ALL";
            this._From = "SYSTEM";
            this.Color = _Color;
            this.ChatType = _ChatType;
        }
        public Message(string _Message, uint _ChatType)
        {
            this.Mesh = 0;
            this.__Message = _Message;
            this._To = "ALL";
            this._From = "SYSTEM";
            
            this.ChatType = _ChatType;
        }
        public Message(string _Message, string __To, Color _Color, uint _ChatType)
        {
            this.Mesh = 0;
            this.__Message = _Message;
            this._To = __To;
            this._From = "SYSTEM";
            this.Color = _Color;
            this.ChatType = _ChatType;
        }
        public Message(string _Message, string __To, string __From, Color _Color, uint _ChatType)
        {
            this.Mesh = 0;
            this.__Message = _Message;
            this._To = __To;
            this._From = __From;
            this.Color = _Color;
            this.ChatType = _ChatType;
        }
        public Message()
        {
            this.Mesh = 0;
        }
        public uint MessageUID1 = 0;
        public uint MessageUID2 = 0;
        public void Deserialize(byte[] buffer)
        {
            Color = Color.FromArgb(BitConverter.ToInt32(buffer, 4));
            ChatType = BitConverter.ToUInt32(buffer, 8);
            MessageUID1 = BitConverter.ToUInt32(buffer, 12);
            MessageUID2 = BitConverter.ToUInt32(buffer, 16);
         _From = Encoding.UTF7.GetString(buffer, 26, buffer[25]);
            _To = Encoding.UTF8.GetString(buffer, 27 + _From.Length, buffer[26 + _From.Length]);
            __Message = Encoding.UTF7.GetString(buffer, (29 + _From.Length) + _To.Length, buffer[(28 + _From.Length) + _To.Length]);
        }// this asshole
        

        public byte[] ToArray()
        {
            byte[] Packet = new byte[(((32 + _From.Length) + _To.Length) + __Message.Length) + 9];
            Writer.WriteUInt16((ushort)(Packet.Length - 8), 0, Packet);
            Writer.WriteUInt16(1004, 2, Packet);
            Writer.WriteUInt32((uint)Color.ToArgb(), 4, Packet);
            Writer.WriteUInt32(ChatType, 8, Packet);
            Writer.WriteUInt32(MessageUID1, 12, Packet);
            Writer.WriteUInt32(MessageUID2, 16, Packet);
            Writer.WriteUInt32(Mesh, 20, Packet);
            Writer.WriteStringList(new System.Collections.Generic.List<string>() { _From, _To, "", __Message }, 24, Packet);
            return Packet;
        }
        public void Send(Client.GameState client)
        {
            client.Send(ToArray());
        }
    }
}
