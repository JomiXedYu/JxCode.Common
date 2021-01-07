using System;
using System.Collections.Generic;
using System.Text;

namespace JxCode.Partten.FSM
{
    public abstract class FSMBase<TFSMIndex, TFSMState> where TFSMState : FSMStateBase
    {
        private IDictionary<TFSMIndex, TFSMState> fsm = new Dictionary<TFSMIndex, TFSMState>();
        private TFSMState curState = null;

        public void AddState(TFSMIndex fsmIndex, TFSMState state)
        {
            fsm.Add(fsmIndex, state);
        }
        public void RemoveState(TFSMIndex fsmIndex)
        {
            fsm.Remove(fsmIndex);
        }

        public virtual void ChangeState(TFSMIndex fsmIndex)
        {
            if (curState != null) curState.OnLeave();
            curState = fsm[fsmIndex];
            curState.OnEnter();
        }

        public TFSMState GetCurState()
        {
            return curState;
        }
    }
}
