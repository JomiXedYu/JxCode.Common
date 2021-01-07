using System;
using System.Collections.Generic;
using System.Text;

namespace JxCode.Net.JxRPC
{
    public interface INetProxy
    {
        void Send(byte[] buf);
    }
}
