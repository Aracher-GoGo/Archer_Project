using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameS
{
    public class Enenmy_fight_State_Idle : Enemy_FSM_FightState
    {

        public Enenmy_fight_State_Idle(Enemy_FSM_Fight _enemyFsmFight): base(_enemyFsmFight, eEnemyFight_STATE.Idle)
        {
        }


        public override void Enter(FSMMsg _msg)
        {
            
        }


        public override void Update()
        {
            
        }

        public override void End()
        {
            //base.End();
        }

        public override void Finally()
        {
            //base.Finally();
            
        }

        public override void SetMsg(FSMMsg _msg)
        {
            
        }
    }
    
    
    
    
    
}
