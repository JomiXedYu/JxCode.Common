using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace JxCode.Unsafe
{
    public unsafe class UnmanagedString
    {
        public static int strlen(byte* str)
        {
            int count = 0;
            while (true)
            {
                if (str[count] == 0)
                    break;
                count++;
            }
            return count;
        }
        public static bool strcmp(byte* str1, byte* str2)
        {
            int str1Len = strlen(str1);
            int str2Len = strlen(str2);
            if (str1Len != str2Len)
            {
                return false;
            }
            for (int i = 0; i < str1Len; i++)
            {
                if (str1[i] != str2[i])
                {
                    return false;
                }
            }
            return true;
        }
    }
}
