using System;
using System.Collections.Generic;
using System.Text;

namespace JxCode.Net.JxRPC
{
    public class RpcServer
    {
        private ISerializer serializer;

        private readonly Dictionary<string, Func<object, object>> methodList;
        public event Action<RpcPackage> ResponseReceived;

        public RpcServer(ISerializer serializer)
        {
            this.serializer = serializer;
            this.methodList = new Dictionary<string, Func<object, object>>();
        }

        public void RegisterService(string name, Func<object, object> callback)
        {
            this.methodList.Add(name, callback);
        }

        public void OnReceived(INetProxy proxy, byte[] data)
        {
            RpcPackage package = RpcPackage.Deserializer(this.serializer, data);

            if (package.ProtocolType == RpcProtocolType.Request)
            {
                //接收到了未注册的服务
                if (!this.methodList.ContainsKey(package.Name))
                {
                    package.ExceptionInfo = "服务未注册";
                    this.Send(proxy, package);
                    return;
                }

                //执行后返回结果
                try
                {
                    package.Obj = this.methodList[package.Name].Invoke(package.Obj);
                }
                catch (Exception e)
                {
                    package.ExceptionInfo = e.Message;
                }

                this.Send(proxy, package);
            }
            else if (package.ProtocolType == RpcProtocolType.Response)
            {
                //收到回复
                this.ResponseReceived?.Invoke(package);
            }
        }

        private void Send(INetProxy proxy, RpcPackage pack)
        {
            byte[] buf = pack.Serialize(this.serializer);
            proxy.Send(buf);
        }
    }
}
