using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameS
{


    public class FsmState<T> where T : System.Enum
    {
        protected T m_stateType;

        public T stateType
        {
            get { return m_stateType; }
        }

        public FsmState(T _stateType)
        {
            m_stateType = _stateType;
        }

        public virtual void Enter(FSMMsg _msg)
        {
        }

        public virtual void Update()
        {
        }

        public virtual void End()
        {
        }

        public virtual void Finally()
        {
        }

        public virtual void SetMsg(FSMMsg _msg)
        {
        }
    }
}