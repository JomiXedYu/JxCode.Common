using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace JxCode.Unsafe
{
    public unsafe class Unmanaged
    {
        public static T CreateStructByBytes<T>(byte[] bytes) where T : unmanaged
        {
            //申请非托管内存
            IntPtr h = Marshal.AllocHGlobal(sizeof(T));
            //把字节数组填充进非托管内存
            Marshal.Copy(bytes, 0, h, sizeof(T));
            //非托管内存转换为托管结构体
            T t = (T)Marshal.PtrToStructure(h, typeof(T));
            //释放非托管内存
            Marshal.FreeHGlobal(h);
            return t;
        }
        public static byte[] CreateBytesByStruct<T>(T stru) where T : unmanaged
        {
            //申请非托管内存
            IntPtr h = Marshal.AllocHGlobal(sizeof(T));
            //把结构体填充进非托管内存
            Marshal.StructureToPtr(stru, h, false);
            byte[] bytes = new byte[sizeof(T)];
            //把非托管内存的数据复制到字节数组中
            Marshal.Copy(h, bytes, 0, sizeof(T));
            //释放非托管内存
            Marshal.FreeHGlobal(h);
            return bytes;
        }

        public unsafe static void ZeroMemory(byte* src, long len)
        {
            while (len-- > 0)
            {
                src[len] = 0;
            }
        }
    }
}
