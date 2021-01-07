using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JxCode.Net.JxRPC
{
    public class RpcPackage
    {
        public RpcProtocolType ProtocolType;
        public int Id { get; set; }
        public string Name { get; set; }
        public string ExceptionInfo { get; set; }

        public string ObjType { get; set; }
        public object Obj { get; set; }

        private static void Write(BinaryWriter bw, string str)
        {
            if (str != null)
            {
                bw.Write(str.Length);
                bw.Write(Encoding.UTF8.GetBytes(str));
            }
            else
            {
                bw.Write((int)0);
            }
        }
        public static string Read(BinaryReader br)
        {
            int len = br.ReadInt32();
            if (len == 0)
            {
                return null;
            }
            else
            {
                byte[] data = br.ReadBytes(len);
                return Encoding.UTF8.GetString(data);
            }
        }

        public byte[] Serialize(ISerializer serializer)
        {
            MemoryStream ms = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(ms);

            bw.Write((int)this.ProtocolType);
            bw.Write(this.Id);
            bw.Write(this.Name);

            Write(bw, this.ExceptionInfo);

            if (this.Obj == null)
            {
                Write(bw, null);
            }
            else
            {
                //FullName | DataLength | Data
                Write(bw, this.Obj.GetType().FullName);

                byte[] data = serializer.Serialize(this.Obj);
                bw.Write(data.Length);

                bw.Write(data);
            }

            bw.Flush();

            byte[] ret = ms.ToArray();

            bw.Dispose();
            ms.Dispose();
            return ret;
        }

        public static RpcPackage Deserializer(ISerializer serializer, byte[] data)
        {
            MemoryStream ms = new MemoryStream(data);
            BinaryReader br = new BinaryReader(ms);

            RpcPackage ret = new RpcPackage();

            ret.ProtocolType = (RpcProtocolType)br.ReadInt32();
            ret.Id = br.ReadInt32();
            ret.Name = br.ReadString();
            ret.ExceptionInfo = Read(br);

            ret.ObjType = Read(br);
            if(ret.ObjType != null)
            {
                Type objType = Type.GetType(ret.ObjType, true, true);

                int objLen = br.ReadInt32();
                byte[] objBuf = br.ReadBytes(objLen);

                ret.Obj = serializer.Deserialize(objBuf, objType);
            }

            br.Dispose();
            ms.Dispose();
            return ret;
        }
    }
}
