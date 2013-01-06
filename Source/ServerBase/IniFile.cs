using System;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace Conquer_Online_Server.ServerBase
{
    public class IniFile
    {
        public string FileName;

        public IniFile()
        {
        }

        public IniFile(string _FileName)
        {
            this.FileName = Environment.CurrentDirectory + "\\" + _FileName;
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int GetPrivateProfileStringA(string Section, string Key, string _Default, StringBuilder Buffer, int BufferSize, string FileName);
        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int WritePrivateProfileStringA(string Section, string Key, string Arg, string FileName);

        public byte ReadByte(string Section, string Key, byte _Default)
        {
            byte buf = _Default;
            byte.TryParse(this.ReadString(Section, Key, _Default.ToString(), 6), out buf);
            return buf;
        }

        public short ReadInt16(string Section, string Key, short _Default)
        {
            short buf = _Default;
            short.TryParse(this.ReadString(Section, Key, _Default.ToString(), 9), out buf);
            return buf;
        }

        public int ReadInt32(string Section, string Key, int _Default)
        {
            int buf = _Default;
            int.TryParse(this.ReadString(Section, Key, _Default.ToString(), 15), out buf);
            return buf;
        }

        public sbyte ReadSByte(string Section, string Key, byte _Default)
        {
            sbyte buf = (sbyte)_Default;
            sbyte.TryParse(this.ReadString(Section, Key, _Default.ToString(), 6), out buf);
            return buf;
        }

        public string ReadString(string Section, string Key)
        {
            return this.ReadString(Section, Key, "", 400);
        }

        public string ReadString(string Section, string Key, string _Default, int BufSize)
        {
            StringBuilder Buffer = new StringBuilder(BufSize);
            GetPrivateProfileStringA(Section, Key, _Default, Buffer, BufSize, this.FileName);
            return Buffer.ToString();
        }

        public ushort ReadUInt16(string Section, string Key)
        {
            ushort buf = 0;
            ushort.TryParse(this.ReadString(Section, Key, 0.ToString(), 9), out buf);
            return buf;
        }

        public uint ReadUInt32(string Section, string Key)
        {
            uint buf = 0;
            uint.TryParse(this.ReadString(Section, Key, 0.ToString(), 15), out buf);
            return buf;
        }

        public void Write(string Section, string Key, object Value)
        {
            WritePrivateProfileStringA(Section, Key, Value.ToString(), this.FileName);
        }

        public void Write(string Section, string Key, string Value)
        {
            WritePrivateProfileStringA(Section, Key, Value, this.FileName);
        }

    }
    
    /*
    public class IniFile
    {
        public string path;
        public IniFile(string INIPath)
        {
            path = INIPath;
            if (File.Exists(path))
            {
                Read();
            }
        }
        public void Read()
        {
            #region IniSectionSelect
            string[] Lines = File.ReadAllLines(path);
            string Ssection = "";
            foreach (string Line in Lines)
            {
                if (Line.Length > 0)
                {
                    if (Line[0] == '[' && Line[Line.Length - 1] == ']')
                    {
                        Ssection = Line;
                        IniSectionStructure Section = new IniSectionStructure();
                        Section.SectionName = Ssection;
                        Section.Variables = new SafeDictionary<string, IniValueStructure>();
                        if (!Sections.ContainsKey(Ssection))
                            Sections.Add(Ssection, Section);
                    }
                    else
                    {
                        IniValueStructure IvS = new IniValueStructure();
                        IvS.Variable = Line.Split('=')[0];
                        IvS.Value = Line.Split('=')[1];
                        IniSectionStructure Section = null;
                        Sections.TryGetValue(Ssection, out Section);
                        if (Section != null)
                        {
                            if (!Section.Variables.ContainsKey(IvS.Variable))
                                Section.Variables.Add(IvS.Variable, IvS);
                        }
                    }
                }
            }
            #endregion
        }
        SafeDictionary<string, IniSectionStructure> Sections = new SafeDictionary<string, IniSectionStructure>();
        public void Close()
        {
            Sections.Clear();
        }
        public void Save()
        {
            string Text = "";
            foreach (IniSectionStructure Section in Sections.Values)
            {
                Text += Section.SectionName + "\r\n";
                foreach (IniValueStructure IVS in Section.Variables.Values)
                {
                    Text += IVS.Variable + "=" + IVS.Value + "\r\n";
                }
            }
            if (File.Exists(path))
            {
                File.Delete(path);
                File.Create(path).Close();
                File.WriteAllText(path, Text);
            }
            else
            {
                File.Create(path).Close();
                File.WriteAllText(path, Text);
            }
        }
        class IniValueStructure
        {
            public string Variable;
            public string Value;
        }
        class IniSectionStructure
        {
            public SafeDictionary<string, IniValueStructure> Variables;
            public string SectionName;
        }
        private void IniWriteValue(string ssection, string Key, string Value)
        {
            string section = "[" + ssection + "]";
            IniSectionStructure _Section = null;
            Sections.TryGetValue(section, out _Section);
            if (_Section != null)
            {
                IniValueStructure IVS = null;
                _Section.Variables.TryGetValue(Key, out IVS);
                if (IVS != null)
                {
                    if (IVS.Variable == Key)
                    {
                        IVS.Value = Value;
                    }
                }
                else
                {
                    _Section.Variables.Add(Key, new IniValueStructure() { Value = Value, Variable = Key });
                }
            }
            else
            {
                _Section = new IniSectionStructure() { SectionName = section, Variables = new SafeDictionary<string, IniValueStructure>() };
                Sections.Add(section, _Section);
                IniValueStructure IVS = null;
                _Section.Variables.TryGetValue(Key, out IVS);
                if (IVS != null)
                {
                    if (IVS.Variable == Key)
                    {
                        IVS.Value = Value;
                    }
                }
                else
                {
                    _Section.Variables.Add(Key, new IniValueStructure() { Value = Value, Variable = Key });
                }
            }
        }

        #region Read
        public byte ReadByte(string Section, string Key)
        {
            string section = "[" + Section + "]";
            IniSectionStructure ISS = null;
            Sections.TryGetValue(section, out ISS);
            if (ISS != null)
            {
                IniValueStructure IVS = null;
                ISS.Variables.TryGetValue(Key, out IVS);
                if (IVS != null)
                    return byte.Parse(IVS.Value);
            }
            return 0;
        }
        public sbyte ReadSbyte(string Section, string Key)
        {
            string section = "[" + Section + "]";
            IniSectionStructure ISS = null;
            Sections.TryGetValue(section, out ISS);
            if (ISS != null)
            {
                IniValueStructure IVS = null;
                ISS.Variables.TryGetValue(Key, out IVS);
                if (IVS != null)
                    return sbyte.Parse(IVS.Value);
            }
            return 0;
        }
        public short ReadInt16(string Section, string Key)
        {
            string section = "[" + Section + "]";
            IniSectionStructure ISS = null;
            Sections.TryGetValue(section, out ISS);
            if (ISS != null)
            {
                IniValueStructure IVS = null;
                ISS.Variables.TryGetValue(Key, out IVS);
                if (IVS != null)
                    return short.Parse(IVS.Value);
            }
            return 0;
        }
        public int ReadInt32(string Section, string Key)
        {
            string section = "[" + Section + "]";
            IniSectionStructure ISS = null;
            Sections.TryGetValue(section, out ISS);
            if (ISS != null)
            {
                IniValueStructure IVS = null;
                ISS.Variables.TryGetValue(Key, out IVS);
                if (IVS != null)
                    return int.Parse(IVS.Value);
            }
            return 0;
        }
        public long ReadInt64(string Section, string Key)
        {
            string section = "[" + Section + "]";
            IniSectionStructure ISS = null;
            Sections.TryGetValue(section, out ISS);
            if (ISS != null)
            {
                IniValueStructure IVS = null;
                ISS.Variables.TryGetValue(Key, out IVS);
                if (IVS != null)
                    return long.Parse(IVS.Value);
            }
            return 0;
        }
        public ushort ReadUInt16(string Section, string Key)
        {
            string section = "[" + Section + "]";
            IniSectionStructure ISS = null;
            Sections.TryGetValue(section, out ISS);
            if (ISS != null)
            {
                IniValueStructure IVS = null;
                ISS.Variables.TryGetValue(Key, out IVS);
                if (IVS != null)
                    return ushort.Parse(IVS.Value);
            }
            return 0;
        }
        public uint ReadUInt32(string Section, string Key)
        {
            string section = "[" + Section + "]";
            IniSectionStructure ISS = null;
            Sections.TryGetValue(section, out ISS);
            if (ISS != null)
            {
                IniValueStructure IVS = null;
                ISS.Variables.TryGetValue(Key, out IVS);
                if (IVS != null)
                    return uint.Parse(IVS.Value);
            }
            return 0;
        }
        public ulong ReadUInt64(string Section, string Key)
        {
            string section = "[" + Section + "]";
            IniSectionStructure ISS = null;
            Sections.TryGetValue(section, out ISS);
            if (ISS != null)
            {
                IniValueStructure IVS = null;
                ISS.Variables.TryGetValue(Key, out IVS);
                if (IVS != null)
                    return ulong.Parse(IVS.Value);
            }
            return 0;
        }
        public double ReadDouble(string Section, string Key)
        {
            string section = "[" + Section + "]";
            IniSectionStructure ISS = null;
            Sections.TryGetValue(section, out ISS);
            if (ISS != null)
            {
                IniValueStructure IVS = null;
                ISS.Variables.TryGetValue(Key, out IVS);
                if (IVS != null)
                    return double.Parse(IVS.Value);
            }
            return 0;
        }
        public float ReadFloat(string Section, string Key)
        {
            string section = "[" + Section + "]";
            IniSectionStructure ISS = null;
            Sections.TryGetValue(section, out ISS);
            if (ISS != null)
            {
                IniValueStructure IVS = null;
                ISS.Variables.TryGetValue(Key, out IVS);
                if (IVS != null)
                    return float.Parse(IVS.Value);
            }
            return 0;
        }
        public string ReadString(string Section, string Key)
        {
            string section = "[" + Section + "]";
            IniSectionStructure ISS = null;
            Sections.TryGetValue(section, out ISS);
            if (ISS != null)
            {
                IniValueStructure IVS = null;
                ISS.Variables.TryGetValue(Key, out IVS);
                if (IVS != null)
                    return IVS.Value;
            }
            return "";
        }
        public bool ReadBoolean(string Section, string Key)
        {
            string section = "[" + Section + "]";
            IniSectionStructure ISS = null;
            Sections.TryGetValue(section, out ISS);
            if (ISS != null)
            {
                IniValueStructure IVS = null;
                ISS.Variables.TryGetValue(Key, out IVS);
                if (IVS != null)
                    return byte.Parse(IVS.Value) == 1 ? true : false; ;
            }
            return false;
        }
        #endregion
        #region Write
        public void WriteString(string Section, string Key, string Value)
        {
            IniWriteValue(Section, Key, Value);
        }
        public void WriteInteger(string Section, string Key, byte Value)
        {
            IniWriteValue(Section, Key, Value.ToString());
        }
        public void WriteInteger(string Section, string Key, ulong Value)
        {
            IniWriteValue(Section, Key, Value.ToString());
        }
        public void WriteInteger(string Section, string Key, double Value)
        {
            IniWriteValue(Section, Key, Value.ToString());
        }
        public void WriteInteger(string Section, string Key, long Value)
        {
            IniWriteValue(Section, Key, Value.ToString());
        }
        public void WriteInteger(string Section, string Key, float Value)
        {
            IniWriteValue(Section, Key, Value.ToString());
        }
        public void WriteBoolean(string Section, string Key, bool Value)
        {
            IniWriteValue(Section, Key, (Value == true ? 1 : 0).ToString());
        }
        #endregion
    }
    */
}
