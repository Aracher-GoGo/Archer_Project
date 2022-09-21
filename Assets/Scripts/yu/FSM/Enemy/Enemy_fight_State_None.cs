using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameS
{
    public class Enemy_fight_State_None : Enemy_FSM_FightState
    {

        public Enemy_fight_State_None(Enemy_FSM_Fight _enemyFsmFight): base(_enemyFsmFight, eEnemyFight_STATE.None)
        {
        }


        public override void Enter(FSMMsg _msg)
        {
            
        }
        
        public override void End()
        {
            
        }

    }
    
    
    
    
    
}
