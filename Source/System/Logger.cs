namespace System
{
    using System;
    using System.IO;
    using System.Text;

    public class Logger
    {
        private string path;
        //private StreamWriter writter;
        public Logger(string baseName)
        {
            path = baseName + " " + Time + ".txt";
            //writter = new StreamWriter(new FileStream(path, FileMode.Create));
            //writter.Write("List<byte[]> list = new List<byte[]>();");
        }
        /*
        public void LogInteger(int value)
        {
            string String = "\r\n\r\n" + Time + " ----> " + value;
            writter.Write(String);
            writter.Flush();
        }

        public void LogLongInteger(long value)
        {
            string String = "\r\n\r\n" + Time + " ----> " + value;
            writter.Write(String);
            writter.Flush();
        }

        public void LogString(string value)
        {
            string String = "\r\n\r\n" + Time + " ----> " + value;
            writter.Write(String);
            writter.Flush();
        }
        */

        public void LogBytes(byte[] value)
        {
            string String = "\r\nlist.Add(new byte[] { ";
            for (int c = 0; c < value.Length; c++)
            {
                if (c != 0)
                    String += ", ";
                String += value[c];
            }
            String += " });";
            //writter.Write(String);
            //writter.Flush();
        }

        public string Time
        {
            get
            {
                DateTime Now = DateTime.Now;
                return Now.Hour + "-" + Now.Minute + "-" + Now.Second;
            }
        }

        public void Close()
        {
            try
            {
                //writter.BaseStream.Close();
                //writter.Close();
            }
            catch
            {

            }
        }
    }
}