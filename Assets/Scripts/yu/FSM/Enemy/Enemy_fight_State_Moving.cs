using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameS
{
    public class Enemy_fight_State_Moving : Enemy_FSM_FightState
    {
        private bool isMoving = false;
        private static readonly int Run = Animator.StringToHash("Run");

        public float culTime;
        public Enemy_fight_State_Moving(Enemy_FSM_Fight _enemyFsmFight): base(_enemyFsmFight, eEnemyFight_STATE.Moving)
        {
        }


        public override void Enter(FSMMsg _msg)
        {
            


        }



        public override void Update()
        {
            Fight.nav.SetDestination(Fight.Player.transform.position);
        }

        public override void End()
        {
            //base.End();
        }

        public override void Finally()
        {
            
            
        }
        public override void SetMsg(FSMMsg _msg)
        {
            
        }
    }
    
    
    
    
    
}
