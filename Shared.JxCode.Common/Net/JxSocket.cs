using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace JxCode.Net
{
    public class JxSocket : IDisposable
    {
        private Socket socket;
        private Queue<byte[]> dataQueue;

        public event Action<Queue<byte[]>, byte[]> DataReceived;

        private Thread receivedThread;

        public JxSocket()
        {
            this.dataQueue = new Queue<byte[]>();
        }

        public void Connect(IPAddress address, int port)
        {
            this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.socket.Connect(address, port);

            this.receivedThread = new Thread(new ThreadStart(StartRecived))
            {
                IsBackground = true
            };
            this.receivedThread.Start();
        }

        public void Close()
        {
            if (this.socket != null)
            {
                if (this.socket.Connected) this.socket.Close();
            }
            if (this.receivedThread != null)
            {
                if (this.receivedThread.IsAlive) this.receivedThread.Abort();
            }
        }

        public void Send(byte[] buffer)
        {
            this.socket.Send(BitConverter.GetBytes(buffer.Length));
            this.socket.Send(buffer);
        }

        public byte[] Recived()
        {
            lock (this.dataQueue)
            {
                return this.dataQueue.Dequeue();
            }
        }

        private void StartRecived()
        {
            while (true)
            {
                int surplus = 4;

                bool hasLen = false;
                List<byte> body = new List<byte>();
                try
                {
                    while (surplus != 0)
                    {
                        byte[] buf = new byte[surplus];
                        int receiveLen = socket.Receive(buf);
                        byte[] receiveBuf = new byte[receiveLen];
                        Array.Copy(buf, receiveBuf, receiveLen);
                        body.AddRange(receiveBuf);
                        surplus -= receiveLen;

                        if (surplus == 0)
                        {
                            if (!hasLen)
                            {
                                //设置长度
                                surplus = BitConverter.ToInt32(body.ToArray(), 0);
                                body.Clear();
                                hasLen = true;
                            }

                        }
                    }
                    lock (this.dataQueue)
                    {
                        var data = body.ToArray();
                        this.dataQueue.Enqueue(data);
                        this.DataReceived?.Invoke(this.dataQueue, data);
                    }
                }
                catch (SocketException e)
                {
                    break;
                }
            }


        }

        public void Dispose()
        {
            this.socket?.Close();
            this.receivedThread?.Abort();
        }

    }
}
