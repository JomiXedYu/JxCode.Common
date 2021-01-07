using System;
using System.Collections.Generic;
using System.Text;

namespace JxCode.Net.JxRPC
{
    public class RpcClient
    {
        private static int _num;
        private static int num { get => ++_num; }

        private INetProxy proxy;
        private ISerializer serializer;

        public RpcClient(INetProxy proxy, ISerializer serializer)
        {
            this.proxy = proxy;
            this.serializer = serializer;
        }

        public int Invoke(string name, object param)
        {
            RpcPackage package = new RpcPackage
            {
                Id = num,
                Name = name,
                Obj = param
            };

            byte[] buf = package.Serialize(this.serializer);
            this.proxy.Send(buf);

            return package.Id;
        }
    }
}
