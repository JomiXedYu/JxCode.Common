using System;
using System.Collections.Generic;
using System.Text;

namespace JxCode.Partten
{
    public class FSM<TFSMIndex, TFSMState> where TFSMState : FSMStateBase
    {
        private IDictionary<TFSMIndex, TFSMState> fsm = new Dictionary<TFSMIndex, TFSMState>();

        private bool hasPreState = false;
        public TFSMIndex preIndex { get; protected set; }

        private TFSMState curState = null;
        private TFSMIndex curIndex = default;

        public FSM<TFSMIndex, TFSMState> AddState(TFSMIndex fsmIndex, TFSMState state)
        {
            state.HostFsm = this;
            fsm.Add(fsmIndex, state);
            return this;
        }
        public void RemoveState(TFSMIndex fsmIndex)
        {
            fsm.Remove(fsmIndex);
        }
        public bool HasState(TFSMIndex fsmIndex)
        {
            return fsm.ContainsKey(fsmIndex);
        }
        public void ChangePreState()
        {
            if(!hasPreState)
            {
                return;
            }
            this.ChangeState(preIndex);
        }
        public virtual void ChangeState(TFSMIndex fsmIndex)
        {
            if (curState != null)
            {
                curState.OnLeave();
                preIndex = curIndex;
                hasPreState = true;
            }
            curIndex = fsmIndex;
            curState = fsm[fsmIndex];
            curState.OnEnter();
        }

        public TFSMState GetCurState()
        {
            return curState;
        }

        public void Reset()
        {
            if (curState != null)
            {
                curState.OnLeave();
            }
            preIndex = default;
            curIndex = default;
            hasPreState = false;
            curState = null;
            fsm.Clear();
        }

    }
}
