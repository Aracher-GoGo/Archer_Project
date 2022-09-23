using System;
using System.Collections;
using UnityEngine;

public class EnemyDuck : EnemyMeleeFSM
{
    public GameObject enemyCanvasGo;
    public GameObject meleeAtkArea;
    

    private void OnDrawGizmosSelected ( )
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere ( transform.position, playerRealizeRange );
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere ( transform.position, attackRange );
    }

    void Start ( )
    {
        base.Start ( );
        
        attackCoolTime = 2f;
        attackCoolTimeCacl = attackCoolTime;

        attackRange = 3f;
        nvAgent.stoppingDistance = 1f;

        StartCoroutine ( ResetAtkArea ( ) );
    }

    IEnumerator ResetAtkArea ( )
    {
        while ( true )
        {
            yield return null;
            if ( !meleeAtkArea.activeInHierarchy && currentState == State.Attack )
            {
                yield return new WaitForSeconds ( attackCoolTime );
                meleeAtkArea.SetActive ( true );
            }
        }
    }

    protected override void InitMonster ( )
    {
        maxHp += ( StageMgr.Instance.currentStage + 1 ) * 10f;
        currentHp = maxHp;
        damage += ( StageMgr.Instance.currentStage + 1 ) * 1f;
    }

    protected override void AtkEffect ( )
    {
        Instantiate ( EffectSet.Instance.DuckAtkEffect, transform.position, Quaternion.Euler ( 90, 0, 0 ) );
    }

    void Update ( )
    {
        if ( currentHp <= 0 )
            //if ( enemyCanvasGo.GetComponent<EnemyHpBar> ( ).currentHp <= 0 )
        {
            nvAgent.isStopped = true;

            rb.gameObject.SetActive ( false );
            PlayerTargeting.Instance.MonsterList.Remove ( transform.parent.gameObject );
            PlayerTargeting.Instance.TargetIndex = -1;
            Destroy ( transform.parent.gameObject );
            return;
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Bullet"))
        {
            enemyCanvasGo.GetComponent<EnemyHpBar>().Dmg(10);
            currentHp -= 10.0f;
            Instantiate(EffectSet.Instance.DuckDmgEffect,collision.contacts[0].point,Quaternion.Euler(90,0,0));
        }
    }
}