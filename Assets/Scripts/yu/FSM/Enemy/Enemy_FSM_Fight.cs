using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace GameS
{
    public class Enemy_FSM_Fight : EnemyBase
    {
        public FsmClass<eEnemyFight_STATE> fsm = new FsmClass<eEnemyFight_STATE>();
        public GameObject Enemy;
        public NavMeshAgent nav;

        [Header("적")] public List<GameObject> Enemies;
        public List<GameObject> Enmies2 = new List<GameObject>();


        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position,playerRealizeRange);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position,attackRange);
        }

        private void Awake()
        {
            fsm.AddFsm(new Enemy_fight_State_None(this));
            fsm.AddFsm(new Enenmy_fight_State_Idle(this));
            fsm.AddFsm(new Enemy_fight_State_Moving(this));
            fsm.AddFsm(new Enemy_fight_State_Attack(this));
            fsm.SetState(eEnemyFight_STATE.None, false);
        }

        public void PlayerEnter()
        {
            fsm.SetState(eEnemyFight_STATE.Idle);
        }




        // Update is called once per frame
        void Update()
        {
            if (fsm.getStateType == eEnemyFight_STATE.None)
            {
                return;
            }

            fsm.Update();
        }
        
    }
}