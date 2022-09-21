using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameS
{
    public class Enemy_fight_State_Attack : Enemy_FSM_FightState
    {

        public Enemy_fight_State_Attack(Enemy_FSM_Fight _enemyFsmFight): base(_enemyFsmFight, eEnemyFight_STATE.Attack)
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
    }
    
    
    
    
    
}
