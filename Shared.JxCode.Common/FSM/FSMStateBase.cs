using System;
using System.Collections.Generic;
using System.Text;

namespace JxCode.Partten
{
    public abstract class FSMStateBase
    {
        public object HostFsm { get; set; }
        public virtual void OnEnter() { }
        public virtual void OnLeave() { }
    }
}
