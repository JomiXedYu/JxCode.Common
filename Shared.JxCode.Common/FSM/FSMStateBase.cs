using System;
using System.Collections.Generic;
using System.Text;

namespace JxCode.Partten.FSM
{
    public abstract class FSMStateBase
    {
        public virtual void OnEnter() { }
        public virtual void OnLeave() { }
    }
}
