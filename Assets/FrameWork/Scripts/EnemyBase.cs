using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBase : MonoBehaviour
{
    public float maxHp = 100f;
    public float currentHp = 100f;

    public float damage = 10f;

    protected float playerRealizeRange = 10f;
    protected float attackRange = 5f;
    protected float attackCoolTime = 5f;
    protected float attackCoolTimeCacl = 5f;
    protected bool canAtk = true;

    protected float moveSpeed = 2f;

    protected GameObject Player;
    protected NavMeshAgent nvAgent;
    protected float distance;

    protected GameObject parentRoom;

    protected Animator Anim;
    protected Rigidbody rb;

    public LayerMask layerMask;

    // Use this for initialization
    protected void Start ( )
    {
        Player = GameObject.FindGameObjectWithTag ( "Player" );
        // Debug.Log ( "Player : " + Player );
        // Debug.Log ( "Player.transform.position : " + Player.transform.position );

        nvAgent = GetComponent<NavMeshAgent> ( );
        rb = GetComponent<Rigidbody> ( );
        Anim = GetComponent<Animator> ( );

        parentRoom = transform.parent.transform.parent.gameObject;

        StartCoroutine ( CalcCoolTime ( ) );
    }

    protected bool CanAtkStateFun ( )
    {
        Vector3 targetDir = new Vector3 ( Player.transform.position.x - transform.position.x, 0f, Player.transform.position.z - transform.position.z );

        Physics.Raycast ( new Vector3 ( transform.position.x, 0.5f, transform.position.z ), targetDir, out RaycastHit hit, 30f, layerMask );
        distance = Vector3.Distance ( Player.transform.position, transform.position );

        if ( hit.transform == null )
        {
            // Debug.Log ( " hit.transform == null" );
            return false;
        }

        if ( hit.transform.CompareTag ( "Player" ) && distance <= attackRange )
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    protected virtual IEnumerator CalcCoolTime ( )
    {
        while ( true )
        {
            yield return null;
            if ( !canAtk )
            {
                attackCoolTimeCacl -= Time.deltaTime;
                if ( attackCoolTimeCacl <= 0 )
                {
                    attackCoolTimeCacl = attackCoolTime;
                    canAtk = true;
                }
            }
        }
    }


    // private void OnDrawGizmos()
    // {
    //     Gizmos.color = Color.blue;
    //     
    //     //타겟과의 방향을 구합니다
    //     var _playerPosition = Player.transform.position;
    //     var _position = transform.position;
    //     Vector3 _targetDir = new Vector3 ( _playerPosition.x - _position.x, 0f, _playerPosition.z - _position.z );
    //         
    //     Gizmos.DrawRay ( new Vector3 ( _position.x, 0.5f, _position.z ), _targetDir);//변경 
    // }
}