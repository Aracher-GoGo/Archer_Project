using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameS
{
    public class FSMMsg
    {
        public int m_msgType;

        public FSMMsg(int _msgType)
        {
            m_msgType = _msgType;
        }
    }
}