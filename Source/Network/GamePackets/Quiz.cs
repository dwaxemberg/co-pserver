using System;
using Conquer_Online_Server.ServerBase;

namespace Conquer_Online_Server.Network.GamePackets
{
    public class QuizInfo : Writer, Interfaces.IPacket
    {
        public QuizInfo()
        {
        }
        public void Deserialize(byte[] buffer)
        {
            throw new NotImplementedException();
        }
        public byte[] ToArray()
        {
            return new byte[2];
        }
        public byte[] StartBuffer()
        {
            byte[] Packet = new byte[20 + 8];
            WriteUInt16(20, 0, Packet);
            WriteUInt16(2068, 2, Packet);
            WriteUInt16(1, 4, Packet);//quiztype
            WriteUInt16(31, 6, Packet);//countdown
            WriteUInt16(20, 8, Packet);//questioncount
            WriteUInt16(30, 10, Packet);//questiontime
            WriteUInt16(1800, 12, Packet);//1st prize
            WriteUInt16(1200, 14, Packet);//2nd prize
            WriteUInt16(600, 16, Packet);//3rdprize
            return Packet;
        }
        private static byte[] FillPacket(byte[] Packet,int start, int end)
        {
            byte[] tempPacket = Packet;
            for (int i = start; i < end; i++)
                if (tempPacket[i] == 0)
                    WriteByte(1, i, tempPacket);
            return tempPacket;
        }
        private static byte[] FillPacket2(byte[] Packet, int start, int end)
        {
            byte[] tempPacket = Packet;
            for (int i = start; i < end; i++)
                if (tempPacket[i] == 0)
                    WriteString("1", i, tempPacket);
            return tempPacket;
        }
        public static byte[] tempQuestionBuffer()
        {
            byte[] Packet = new byte[448];
            WriteUInt16((ushort)440,0, Packet);//total length - 8
            WriteUInt16(2068, 2, Packet);//packettype
            WriteUInt16(2, 4, Packet);//quiztype
            WriteUInt16(1, 6, Packet);//questionid
            WriteUInt16(0, 8, Packet);//last question right answer
            WriteUInt16(0, 10, Packet);//prize so far
            WriteUInt16(0, 12, Packet);//time taken so far
            WriteUInt32(0, 14, Packet);//current score
            WriteByte(5, 18, Packet);
            //26-87 = question 26-86, 88-98, 49
            //26-41

            //26 76 92 167 184  <- FINAL
            //answer length = -16
            WriteString("24", 24, Packet); //Progress bar (?)
            WriteString("25", 25, Packet);
            string tempq = "Hello, how are you?";
            string tempa = "I am good thanks";
            int offset = 26;
            WriteStringWithLength(tempq, 26, Packet);//26
            offset = tempq.Length;

            WriteStringWithLength(tempa, 76, Packet);//76
            offset = tempa.Length;

            WriteStringWithLength(tempa, 76 + offset + 2, Packet);//94
            offset = tempa.Length;

            WriteStringWithLength(tempa, 153 + offset, Packet);//144
            offset = tempa.Length;

            WriteStringWithLength(tempa, 170 + offset - 4, Packet);//190


            Packet = FillPacket(Packet, 26, Packet.Length);
          //  foreach (char c in tempq)
          //  {
               // WriteString(c.ToString(), offset, Packet);
                //offset++;
         //   }
            //for (int i = startVal; i < Packet.Length; i++)
            //{
            //    if (offset == 26)
            //        offset = i;
            //    try
            //    {
            //        string write = i.ToString() + " ";
            //        WriteStringWithLength(write, offset, Packet);
            //        offset += write.Length;
            //    }
            //    catch { continue; }
            //}
            //FillPacket(Packet, 26, startVal);
           // Packet = FillPacket2(Packet, 26, 86);
            offset = 86;
            //for (int i = 0; i < 4; i++)
            //{
            //    foreach (char c in tempa)
            //    {
            //        WriteString(c.ToString(), offset, Packet);
            //        offset++;
            //    }
            //}
          //  Packet = FillPacket2(Packet, 86 - (tempq.Length + 1), Packet.Length);
            //Fill the packet with the letter 0

            //

             
            //Question : 26-87
            //Answer1 : 87-98 (+11)
            //etc

            //string question = "How are you doing today";
            //string Answer = "I am good..";

            //WriteStringWithLength(question, 26, Packet); //question
            //Packet = FillPacket(Packet, 26, 87, ".");

            //WriteStringWithLength(Answer, 87,Packet); //answer1
            //Packet = FillPacket(Packet, 87, 98, "-");

            //WriteStringWithLength(Answer, 98, Packet); //answer2
            //Packet = FillPacket(Packet, 98, 109, "1");

            //WriteStringWithLength(Answer, 109, Packet); //answer3
            //Packet = FillPacket(Packet, 109, 120,"3");

            //WriteStringWithLength(Answer, 109, Packet); //answer4
            //Packet = FillPacket(Packet, 120, 131,"4");

            //Packet = FillPacket(Packet, 131, Packet.Length,"0");
            //qValue = 26;

            return Packet;
        }
        public struct QuestionAnswer
        {
            public string Question, Answer1, Answer2, Answer3, Answer4;
            public QuestionAnswer(string q, string a1, string a2, string a3, string a4)
            {
                Question = q;
                Answer1 = a1;
                Answer2 = a2;
                Answer3 = a3;
                Answer4 = a4;
            }
        }//kk you can set up the question this way now
        public class QuizRound{
            private QuestionAnswer question;
            public QuizRound(QuestionAnswer incoming)
            {
                question = incoming;
            }

            //now you have all your stuff set up and you can do a method to pull the packet. This is just a general example. Ideally you'd wany all your shuffling/etc going on in here too!
            public byte[] Get()
            {
                byte[] packet = new byte[19 + question.Answer1.Length];//etc
                WriteUInt16((ushort)(packet.Length - 8), 0, packet);
                //etc

                int Offset = 26;
                WriteStringWithLength(question.Question, Offset, packet);
                Offset += question.Question.Length;
                WriteStringWithLength(question.Answer1, Offset, packet);
                Offset+= question.Answer1.Length;
                //how about filling the packet? with empty values it will become
                //invisible
                //waht values iare empty?
                //wait what?... why would you need to fill it with empty vbalues? is that to make the window go awya?
                //Need to fill empty values, else nothing will appear
                return packet;

                      //Question : 26-87
            //Answer1 : 87-98 (+11)//they are counted length are they not? yup, but off starts at 26 for q yah
            //etc
            }
        }
        public byte[] QuestionBuffer(uint Score, ushort Time, ushort Prize, ushort ID, string[] QA)
        {
            //That's just an temporary packet with fixed q/a
            byte[] Packet = new byte[47];
           // byte[] Packet = new byte[19 + tempQ.Length + 1 + tempQ.Length + 1 + tempQ.Length + 1 + tempQ.Length + tempQ.Length + 1 + 8];
            WriteUInt16((ushort)(Packet.Length - 8), 0, Packet);//length
            WriteUInt16(2068, 2, Packet);//packettype
            WriteUInt16(2, 4, Packet);//quiztype
            WriteUInt16(ID, 6, Packet);//questionid
            WriteUInt16(0, 8, Packet);//last question right answer
            WriteUInt16(Prize, 10, Packet);//prize so far
            WriteUInt16(Time, 12, Packet);//time taken so far
            WriteUInt32(Score, 14, Packet);//current score
            WriteByte(5, 18, Packet);
            char length = (char)QA[0].Length;
            WriteString("test", 19, Packet);
            WriteString("test", 23, Packet);
            WriteString("test", 28, Packet);
            WriteString("test", 33, Packet);
            WriteString("test", 38, Packet);
            //WriteString(length.ToString(), 19, Packet);
            //WriteStringWithLength(QA[0], 20, Packet);
            //WriteStringWithLength(QA[1], 21, Packet);
            //WriteStringWithLength(QA[2], 22, Packet);
            //WriteStringWithLength(QA[3], 23, Packet);
            //WriteStringWithLength(QA[4], 24, Packet);
            //WriteString(length.ToString(),19,Packet);
            //offLen += QA[0].Length;

            //WriteString(QA[0], 20 + offLen, Packet);
            //offLen += QA[1].Length;

            //WriteStringWithLength(QA[1], 21 + offLen, Packet);
            //offLen += QA[2].Length;

            //WriteStringWithLength(QA[2], 22 + offLen, Packet);
            //offLen += QA[3].Length;

            //WriteStringWithLength(QA[3], 23 + offLen, Packet);
            //offLen += QA[4].Length;

            //WriteStringWithLength(QA[4], 24 + offLen, Packet);
            return Packet;
        }
        public byte[] InfoBuffer(ushort Score, ushort Time, ushort Rank)
        {
            int length = 0;
            for (int i = 0; i < 3; i++)
            {
                int Len = Kernel.MainQuiz.Name[i].Length + 1 +
                    Kernel.MainQuiz.Score[i].ToString().Length + 1 +
                     Kernel.MainQuiz.Time[i].ToString().Length;
                length += Len;
            }
            byte[] Packet = new byte[22 + length + 8];
            WriteUInt16((ushort)(Packet.Length - 8), 0, Packet);//length
            WriteUInt16(2068, 2, Packet);//packettype
            WriteUInt16(4, 4, Packet);//quiztype
            WriteUInt16(Score, 6, Packet);//doesntwork
            WriteUInt16(Time, 8, Packet);//doesntwork
            WriteUInt16(Rank, 10, Packet);//doesnt work.
            WriteUInt32(0, 12, Packet);//unknown
            WriteUInt16(0, 16, Packet);//unknown
            WriteByte(3, 20, Packet);//leaders

            for (int i = 0; i < 3; i++)
            {
                int Len = Kernel.MainQuiz.Name[i].Length + 1 +
                    Kernel.MainQuiz.Score[i].ToString().Length + 1 +
                     Kernel.MainQuiz.Time[i].ToString().Length;
                WriteByte((byte)Len,21, Packet);

                WriteStringWithLength(Kernel.MainQuiz.Name[i],22,Packet);
                WriteByte(0x20, 23 + Packet[22], Packet);
                WriteStringWithLength(Kernel.MainQuiz.Score[i].ToString(), 24 + Packet[22], Packet);
                WriteByte(0x20, 25 + Packet[22] + Packet[23], Packet );
                WriteStringWithLength(Kernel.MainQuiz.Time[i].ToString(), 26 + Packet[22] + Packet[23] + Packet[22], Packet);
            }
            return Packet;
            
        }
        public byte[] EndBuffer(string Name, ushort Score, ushort Time, ushort Rank, ushort Prize)
        {
            int Len = Name.Length + 1 + Score.ToString().Length + 1 + Time.ToString().Length;
            byte[] Packet = new byte[20 + Len + 8];
            WriteUInt16((ushort)(20 + Len), 0, Packet);//length
            WriteUInt16(2068,2,Packet);//packettype
            WriteUInt16(5,4,Packet);//quiztype
            WriteUInt16(Rank,6,Packet);//rank
            WriteUInt16(Prize,8,Packet);//0
            WriteUInt16(Time,10,Packet);//time
            WriteUInt16(Score,12,Packet);//score
            WriteUInt32(0,14,Packet);
            WriteByte(1,18,Packet);
            WriteByte((byte)Len,19,Packet);
            WriteStringWithLength(Name,20,Packet);//history name
            WriteByte(0x20,21 + Packet[20], Packet);
            WriteStringWithLength(Score.ToString(), 22 + Packet[20], Packet);//history name
            WriteByte(0x20, 23 + Packet[20] + Packet[22],Packet);
            WriteStringWithLength(Time.ToString(), 24 + Packet[20] + Packet[22], Packet);//history name
            return Packet;

        }

        public void Send(Client.GameState client)
        {
        }
    }
}


/*
 * 
 * byte[] Packet = new byte[20];
            WriteUInt16(1, 0, Packet);
            WriteUInt16(2068, 2, Packet);
            WriteUInt16(30, 4, Packet);
            WriteUInt16(20, 6, Packet);
            WriteUInt16(30, 8, Packet);
            WriteUInt16(1800, 10, Packet);
            WriteUInt16(1200, 12, Packet);
            WriteUInt16(600, 14, Packet);
            WriteUInt16(0, 16, Packet);
            return Packet;*/
