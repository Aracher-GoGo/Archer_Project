using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameS
{
    public enum eEnemyFight_STATE
    {
        None,
        Idle,
        Moving,
        Attack,
    }
    public class Enemy_FSM_FightState : FsmState<eEnemyFight_STATE>
    {
        protected Enemy_FSM_Fight Fight;
        public Enemy_FSM_FightState(Enemy_FSM_Fight _FsmFight,eEnemyFight_STATE _stateType) : base(_stateType)
        {
            Fight = _FsmFight;
        }

        public override void Enter(FSMMsg _msg)
        {
            
            
        }
    }
}
