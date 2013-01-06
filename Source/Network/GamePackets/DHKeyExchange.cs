using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using Conquer_Online_Server.ServerBase;

namespace Kijo.Network.GamePackets
{
    public static class DHKeyExchange
    {
        public class ServerKeyExchange
        {
            OpenSSL.DH _keyExchange;
            byte[] _serverIv;
            byte[] _clientIv;

            public byte[] CreateServerKeyPacket()
            {
                _clientIv = new byte[8];
                _serverIv = new byte[8];
                string P = "E7A69EBDF105F2A6BBDEAD7E798F76A209AD73FB466431E2E7352ED262F8C558F10BEFEA977DE9E21DCEE9B04D245F300ECCBBA03E72630556D011023F9E857F";
                string G = "05";
                _keyExchange = new OpenSSL.DH(OpenSSL.BigNumber.FromHexString(P), OpenSSL.BigNumber.FromHexString(G));
                _keyExchange.GenerateKeys();
                return GeneratePacket(_serverIv, _clientIv, P, G, _keyExchange.PublicKey.ToHexString());
            }

            public Conquer_Online_Server.Network.Cryptography.GameCryptography HandleClientKeyPacket(string PublicKey, Conquer_Online_Server.Network.Cryptography.GameCryptography cryptographer)
            {
                byte[] key = _keyExchange.ComputeKey(OpenSSL.BigNumber.FromHexString(PublicKey));

                string md = KeyMD5(MD5.Create().ComputeHash(key));
                key = Encoding.Default.GetBytes(md);
                cryptographer.SetKey(key);
                cryptographer.SetIvs(_clientIv, _serverIv);
                return cryptographer;
            }


            public static string KeyMD5(byte[] object_1)
            {
                char[] chArray = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f' };
                int num = 0;
                int num2 = 0;
                int length = object_1.Length;
                char[] chArray2 = new char[length * 2];
                while (num < length)
                {
                    byte num4 = (byte)object_1[num++];
                    chArray2[num2++] = chArray[num4 / 0x10];
                    chArray2[num2++] = chArray[num4 % 0x10];
                }
                return new string(chArray2, 0, chArray2.Length);
            }
            public byte[] GeneratePacket(byte[] ServerIV1, byte[] ServerIV2, string P, string G, string ServerPublicKey)
            {
                int PAD_LEN = 11;
                int _junk_len = 12;
                string tqs = "TQServer";
                MemoryStream ms = new MemoryStream();
                byte[] pad = new byte[PAD_LEN];
                Kernel.Random.NextBytes(pad);
                byte[] junk = new byte[_junk_len];
                Kernel.Random.NextBytes(junk);
                int size = 47 + P.Length + G.Length + ServerPublicKey.Length + 12 + 8 + 8;
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write(pad);
                bw.Write(size - PAD_LEN);
                bw.Write((UInt32)_junk_len);
                bw.Write(junk);
                bw.Write((UInt32)ServerIV2.Length);
                bw.Write(ServerIV2);
                bw.Write((UInt32)ServerIV1.Length);
                bw.Write(ServerIV1);
                bw.Write((UInt32)P.ToCharArray().Length);
                foreach (char fP in P.ToCharArray())
                {
                    bw.BaseStream.WriteByte((byte)fP);
                }
                bw.Write((UInt32)G.ToCharArray().Length);
                foreach (char fG in G.ToCharArray())
                {
                    bw.BaseStream.WriteByte((byte)fG);
                }
                bw.Write((UInt32)ServerPublicKey.ToCharArray().Length);
                foreach (char SPK in ServerPublicKey.ToCharArray())
                {
                    bw.BaseStream.WriteByte((byte)SPK);
                }
                foreach (char tq in tqs.ToCharArray())
                {
                    bw.BaseStream.WriteByte((byte)tq);
                }
                byte[] Packet = new byte[ms.Length];
                Packet = ms.ToArray();
                ms.Close();
                return Packet;
            }
        }
    }
}