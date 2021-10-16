using System;
using System.Collections.Generic;
using System.Text;

namespace JxCode.Common
{
    public static class StringExtension
    {
        /// <summary>
        /// 从右侧开始寻找一个字符，并截取该字符右侧的字符串。
        /// </summary>
        /// <param name="str"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static string SubstringFromRight(this string str, char rbegin, char rend)
        {
            int beginpos = -1;
            int endpos = -1;
            for (int i = str.Length - 1; i >= 0; i--)
            {
                if (beginpos == -1)
                {
                    if (str[i] == rbegin)
                    {
                        beginpos = i;
                        continue;
                    }
                }
                else
                {
                    if (str[i] == rend)
                    {
                        endpos = i;
                        break;
                    }
                }
            }
            if (beginpos == -1 || endpos == -1)
            {
                throw new System.ArgumentException("not found char");
            }
            return str.Substring(endpos + 1, beginpos - endpos - 1);
        }


        public static string SubstringFromLeft(this string str, char begin, char end)
        {
            int beginpos = -1;
            int endpos = -1;
            for (int i = 0; i < str.Length; i++)
            {
                if (beginpos == -1)
                {
                    if (str[i] == begin)
                    {
                        beginpos = i;
                        continue;
                    }
                }
                else
                {
                    if (str[i] == end)
                    {
                        endpos = i;
                        break;
                    }
                }
            }

            if (beginpos == -1 || endpos == -1)
            {
                throw new System.ArgumentException("not found char");
            }
            return str.Substring(beginpos + 1, endpos - beginpos - 1);
        }
    }
}
