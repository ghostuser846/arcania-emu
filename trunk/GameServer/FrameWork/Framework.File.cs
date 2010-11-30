using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Collections;
using System.IO;

namespace Framework
{
    public class IniFile
    {
        public string path;

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section,
            string key, string val, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section,
                 string key, string def, StringBuilder retVal,
            int size, string filePath);

        public IniFile(string INIPath)
        {
            path = INIPath;
        }

        public void IniWriteValue(string Section, string Key, string Value)
        {
            WritePrivateProfileString(Section, Key, Value, this.path);
        }

        public string IniReadValue(string Section, string Key)
        {
            StringBuilder temp = new StringBuilder(255);
            int i = GetPrivateProfileString(Section, Key, "", temp,
                                            255, this.path);
            return temp.ToString();
        }
    }
    public class TxtFile
    {
            public static ArrayList lines = new ArrayList(20000);
            public static string[] commands = new string[20000];
            public static int amountLine;
            public static void Clear()
            {
                commands = null;
                amountLine = 0;
                lines.Clear();
            }
            public static void ReadFromFile(string filename, char splitType)
            {
                Clear();

                StreamReader SR;
                string S = "";
                SR = File.OpenText(Environment.CurrentDirectory + @filename);

                char[] sp = { splitType };
                String curLine;

                while ((curLine = SR.ReadLine()) != null)
                {
                    lines.Add(curLine);
                    amountLine += 1;
                }

                S = lines[0].ToString();
                commands = S.Split(sp);
                SR.Close();
            }
    }
}
