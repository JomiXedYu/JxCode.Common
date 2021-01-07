using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace JxCode.Net.JxRPC
{
    public class RpcClientProxy
    {
        private class JxSocketNetProxy : INetProxy
        {
            private JxSocket socket;

            public JxSocketNetProxy(JxSocket socket)
            {
                this.socket = socket;
            }

            public void Send(byte[] buf)
            {
                this.socket.Send(buf);
            }
        }

        private JxSocket socket;
        private RpcClient client;
        private RpcServer server;

        private INetProxy netProxy;

        private readonly Dictionary<int, RpcPackage> responses;

        public RpcClientProxy(ISerializer serializer)
        {
            this.socket = new JxSocket();
            this.socket.DataReceived += Socket_DataReceived;

            this.netProxy = new JxSocketNetProxy(this.socket);

            this.client = new RpcClient(this.netProxy, serializer);
            this.server = new RpcServer(serializer);

            this.server.ResponseReceived += Server_ResponseReceived;

            this.responses = new Dictionary<int, RpcPackage>();
        }
        //收到消息后给server
        private void Socket_DataReceived(Queue<byte[]> arg1, byte[] arg2)
        {
            this.server.OnReceived(this.netProxy, arg1.Dequeue());
        }
        //server接受到了回复（请求直接内部处理了）
        private void Server_ResponseReceived(RpcPackage obj)
        {
            this.responses.Add(obj.Id, obj);
        }

        public void RegisterService(string name, Func<object, object> service)
        {
            this.server.RegisterService(name, service);
        }

        public RpcInvokeResult Invoke(string name, object arg, int timeOut = 5)
        {
            int id = this.client.Invoke(name, arg);

            long targetTime = CurTime + timeOut;
            while (!responses.ContainsKey(id) && CurTime < targetTime)
            {
                Thread.Sleep(1);
            }

            if (!this.responses.ContainsKey(id))
            {
                RpcInvokeResult result = new RpcInvokeResult()
                {
                    IsSccuess = false,
                    ErrMsg = "等待超时",
                };
                return result;
            }

            RpcPackage response = this.responses[id];
            this.responses.Remove(id);

            RpcInvokeResult result2 = new RpcInvokeResult()
            {
                IsSccuess = true,
                Return = response.Obj,
            };

            return result2;
        }
        private long CurTime
        {
            get => (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds;
        }

        public Task<RpcInvokeResult> InvokeAsync(string name, object arg, int timeOut = 5)
        {
            Task<RpcInvokeResult> task = Task.Factory.StartNew<RpcInvokeResult>(() =>
            {
                return this.Invoke(name, arg, timeOut);
            });
            return task;
        }

        public void Connect(IPAddress ip, int port)
        {
            socket.Connect(ip, port);
        }
        public void Close()
        {
            socket.Close();
        }


    }
    public class RpcInvokeResult
    {
        public bool IsSccuess { get; set; }
        public string ErrMsg { get; set; }
        public object Return { get; set; }
    }
}
