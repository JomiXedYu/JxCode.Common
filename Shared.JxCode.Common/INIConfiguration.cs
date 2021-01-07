using System;
using System.Collections.Generic;
using System.Text;

namespace JxCode.Common
{
    public class INIConfiguration : Dictionary<string, INISection>
    {
        //private Dictionary<string, INISection> content;
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var ini in this)
            {
                sb.Append(ini.Value.ToString());
            }
            return sb.ToString();
        }
        public void Add(INISection section)
        {
            this.Add(section.Name, section);
        }
        public static INIConfiguration Parse(string content)
        {
            INIConfiguration self = new INIConfiguration();

            string[] strs = content.Split('\n');
            string curSection = null;
            for (int i = 0; i < strs.Length; i++)
            {
                string str = strs[i].Trim();
                if (str == string.Empty) continue;
                if (str[0] == ';' || str[0] == ':')
                {
                    continue;
                }
                if (str[0] == '[')
                {
                    if (str[str.Length - 1] != ']')
                        throw new ArgumentException();
                    curSection = str.Substring(1, str.Length - 2);
                    self.Add(curSection, new INISection(curSection));
                    continue;
                }
                //是key=value
                if (string.IsNullOrEmpty(curSection))
                {
                    throw new ArgumentException();
                }

                int pos = str.IndexOf('=');
                if (pos == -1)
                {
                    throw new ArgumentException();
                }

                string key = str.Substring(0, pos);
                string value = str.Substring(pos + 1);

                self[curSection].Add(key, value);
            }
            return self;
        }
    }
    public class INISection : Dictionary<string, string>
    {
        public string Name { get; private set; }
        public INISection(string name)
        {
            this.Name = name;
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append('[');
            sb.Append(this.Name);
            sb.Append(']');
            sb.Append('\n');
            foreach (var iniSection in this)
            {
                sb.Append(iniSection.Key);
                sb.Append('=');
                sb.Append(iniSection.Value);
                sb.Append('\n');
            }
            sb.Append('\n');
            return sb.ToString();
        }
    }
}
