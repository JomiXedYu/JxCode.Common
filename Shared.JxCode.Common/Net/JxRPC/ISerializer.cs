using System;
using System.Collections.Generic;
using System.Text;

namespace JxCode.Net.JxRPC
{
    public interface ISerializer
    {
        byte[] Serialize(object content);
        object Deserialize(byte[] content, Type type);
    }
}
