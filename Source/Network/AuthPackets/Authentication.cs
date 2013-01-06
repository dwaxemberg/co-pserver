using System;
using System.IO;
using System.Text;

namespace Conquer_Online_Server.Network.AuthPackets
{
    public class Authentication : Interfaces.IPacket
    {
        public string Username;
        public string Password;
        public string Server;
        public void Deserialize(byte[] buffer)
        {
            if (buffer.Length == 240)
            {
                MemoryStream MS = new MemoryStream(buffer);
                BinaryReader BR = new BinaryReader(MS);

                ushort length = BR.ReadUInt16();
                if (length == 240)
                {
                    ushort type = BR.ReadUInt16();
                    if (type == 1124)
                    {
                        Username = Encoding.ASCII.GetString(BR.ReadBytes(16));
                        Username = Username.Replace("\0", "");
                        BR.ReadBytes(112);
                        Password = Encoding.ASCII.GetString(BR.ReadBytes(16));
                        BR.ReadBytes(112);
                        Server = Encoding.ASCII.GetString(BR.ReadBytes(16));
                        Server = Server.Replace("\0", "");
                    }
                }
                BR.Close();
                MS.Close();
            }
        }

        public byte[] ToArray()
        {
            throw new NotImplementedException();
        }
        public void Send(Client.GameState client)
        {
            throw new NotImplementedException();
        }
    }
}
