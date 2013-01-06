using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Conquer_Online_Server.Network.GamePackets
{
    public class ArsenalInscribitionList
    {
        public byte[] Build(byte[] packet, Game.ConquerStructures.Society.Guild g)
        {
            MemoryStream strm = new MemoryStream();
            BinaryWriter wtr = new BinaryWriter(strm);

            wtr.Write((ushort)0);
            wtr.Write((ushort)2202);
            wtr.Write((uint)packet[4]);
            wtr.Write((uint)packet[8]);
            wtr.Write((uint)packet[12]);
            wtr.Write((uint)packet[16]);
            wtr.Write((uint)g.Arsenal.Inscribed[(Game.ConquerStructures.Society.ArsenalType)packet[16]].Count);//items count
            wtr.Write((ulong)0);
            wtr.Write((uint)0);
            wtr.Write((uint)g.Arsenal.Donation[(Game.ConquerStructures.Society.ArsenalType)packet[16]]);//type arsenal donation
            wtr.Write((uint)g.Arsenal.Inscribed[(Game.ConquerStructures.Society.ArsenalType)packet[16]].Count);
            #region Build Items
            SafeDictionary<uint, Game.ConquerStructures.Society.ArsenalSingle> Evalues = g.Arsenal.Inscribed[(Game.ConquerStructures.Society.ArsenalType)packet[16]];
            uint pos = 1;
            foreach (Game.ConquerStructures.Society.ArsenalSingle s in Evalues.Values)
            {
                if (s.Item == null)
                    continue;

                wtr.Write((uint)s.Item.UID);
                wtr.Write((uint)pos);
                string Name = s.Name;
                for (int i2 = 0; i2 < 16; i2++)
                {
                    if (i2 < Name.Length)
                    {
                        wtr.Write((byte)Name[i2]);
                    }
                    else
                        wtr.Write((byte)0);
                }
                wtr.Write((uint)s.Item.ID);//itemid
                byte car = (byte)s.Item.ID.ToString().Length;
                wtr.Write((byte)((byte)(s.Item.ID % 10)));//item quality
                wtr.Write((byte)s.Item.Plus);//itempluss
                wtr.Write((byte)s.Item.SocketOne);//socket1
                wtr.Write((byte)s.Item.SocketTwo);//socket2
                wtr.Write((byte)s.Item.BattlePower);//item potency
                wtr.Write((byte)0);//maybe lock
                wtr.Write((byte)0);//maybe bound
                wtr.Write((byte)0);//maybe something -.-''
                wtr.Write((uint)s.Donation);
                pos++;
            }
            #endregion
            wtr.Write((ulong)0);
            wtr.Write((ulong)0);
            wtr.Write((ulong)0);
            wtr.Write((ulong)0);
            wtr.Write((ulong)0);
            wtr.Write((uint)0);
            int packetlength = (int)strm.Length;
            strm.Position = 0;
            wtr.Write((ushort)packetlength);
            strm.Position = strm.Length;
            wtr.Write(ASCIIEncoding.ASCII.GetBytes("TQServer"));
            strm.Position = 0;
            byte[] buf = new byte[strm.Length];
            strm.Read(buf, 0, buf.Length);
            wtr.Close();
            strm.Close();
            return buf;
        }
    }
}
