using System;
using System.Collections.Generic;
using System.Text;

namespace JxCode.Common
{
    public static class StringMatch
    {
        public static bool MatchEmail(string email)
        {
            bool hasAt = false;
            int hostLevel = 0;
            for (int i = 0; i < email.Length; i++)
            {
                char c = email[i];

                if (!hasAt)
                {
                    //username
                    if (c == '@')
                        hasAt = true;
                }
                else
                {
                    //host
                    if (c == '@')
                        return false;
                    else
                    {
                        if (c == '.')
                        {
                            hostLevel++;
                        }
                    }
                }
            }
            return hostLevel > 0;
        }
        public static bool MatchUserName(string userName)
        {
            bool b = true;
            foreach (char item in userName)
            {
                //英文数字下划线
                //0 <= c <= 9 ||  A <= c <= Z || a <= c <= z || c == _
                if (!
                    ((item >= '0' && item <= '9') || (item >= 'A' && item <= 'Z') || (item >= 'a' && item <= 'z') || item == '_')
                    )
                {
                    return false;
                }
            }

            return b;
        }
        public static bool MatchIPv4(string ip)
        {
            string[] r = ip.Split('.');
            if (r.Length != 4)
                return false;

            int group = 0;
            int position = 0;
            for (int i = 0; i < ip.Length; i++)
            {
                char c = ip[i];
                //.
                if (c == 46)
                {
                    group++;

                }
                //0-9
                if (c > 47 && c < 58)
                {
                    position++;
                }
            }
            return false;
        }
    }
}
