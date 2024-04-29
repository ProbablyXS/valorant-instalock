using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace valorant_instalock.Classes
{
    class IniFile
    {
        string Path;
        string EXE = Assembly.GetExecutingAssembly().GetName().Name;

        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        static extern long WritePrivateProfileString(string Section, string Key, string Value, string FilePath);

        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        static extern int GetPrivateProfileString(string Section, string Key, string Default, StringBuilder RetVal, int Size, string FilePath);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        private static extern uint GetPrivateProfileSection(string lpAppName, IntPtr lpReturnedString, uint nSize, string lpFileName);

        public IniFile(string IniPath = null)
        {
            Path = new FileInfo(IniPath ?? EXE + ".ini").FullName;
        }

        public string Read(string Key, string Section = null)
        {
            var RetVal = new StringBuilder(255);
            GetPrivateProfileString(Section ?? EXE, Key, "", RetVal, 255, Path);
            return RetVal.ToString();
        }

        public void Write(string Key, string Value, string Section = null)
        {
            WritePrivateProfileString(Section ?? EXE, Key, Value, Path);
        }

        public void DeleteKey(string Key, string Section = null)
        {
            Write(Key, null, Section ?? EXE);
        }

        public void DeleteSection(string Section = null)
        {
            Write(null, null, Section ?? EXE);
        }

        public bool KeyExists(string Key, string Section = null)
        {
            return Read(Key, Section).Length > 0;
        }

        public string[] ReadAllKeysInSection(string Section)
        {
            // Read all lines from the INI file within the specified section
            string[] lines = File.ReadAllLines(Path);
            List<string> keys = new List<string>();

            bool insideSection = false;
            foreach (string line in lines)
            {
                // Check if the line indicates the start of the specified section
                if (line.Trim().StartsWith($"[{Section}]", StringComparison.OrdinalIgnoreCase))
                {
                    insideSection = true;
                    continue; // Skip this line as it's just the section header
                }

                // If we are inside the specified section, parse keys
                if (insideSection && line.Contains("="))
                {
                    string key = line.Split('=')[0].Trim();
                    keys.Add(key);
                }
                else if (insideSection && line.Trim().StartsWith("[", StringComparison.Ordinal))
                {
                    // We've reached the end of the specified section, exit the loop
                    break;
                }
            }

            return keys.ToArray();
        }
    }
}