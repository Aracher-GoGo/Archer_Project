using System.Collections;
using System.Collections.Generic;
using CMF;
using UnityEngine;

public class PlayerTargeting : MonoSingleton<PlayerTargeting>
{
    
    
    public bool getATarget = false;
    
    float currentDist = 0;      //현재 거리
    
    float closetDist = 100f;    //가까운 거리
    
    float TargetDist = 100f;   //타겟 거리
    
    int closeDistIndex = 0;    //가장 가까운 인덱스
    
    int TargetIndex = -1;      //타겟팅 할 인덱스
    
    public LayerMask layerMask;

    public float atkSpd = 1f;

    public List<GameObject> MonsterList = new List<GameObject> ( );
    //Monster를 담는 List 

    public GameObject PlayerBolt;  //발사체
    public Transform AttackPoint;
    
    private Vector3 refVelocity;
    public float smoothDampTime = 0.02f;

    void OnDrawGizmos ( )
    {
        if ( getATarget )
        {
            for ( int i = 0 ; i < MonsterList.Count ; i++ )
            {
                RaycastHit hit;
                bool isHit = Physics.Raycast ( transform.position, MonsterList[i].transform.position - transform.position,
                    out hit, 20f, layerMask );

                if ( isHit && hit.transform.CompareTag ( "Monster" ) )
                {
                    Gizmos.color = Color.green;
                }
                else
                {
                    Gizmos.color = Color.red;
                }
                Gizmos.DrawRay ( transform.position, MonsterList[i].transform.position - transform.position );
            }
        }
    }


    // Update is called once per frame
    void Update ( )
    {
        SetTarget ( );
        AtkTarget ( );
    }
    
    void Attack ( )
    {
        // PlayerMovement.Instance.Anim.SetFloat ( "AttackSpeed", atkSpd );
        Instantiate ( PlayerBolt, AttackPoint.position, transform.rotation );
    }

    void SetTarget ( )
    {
        //Todo : 현재 리스트가 2마리몬스터가 추가가 안됌 나중에 고쳐야함
        // Debug.Log("MonsterList.Count :" +MonsterList.Count);
        if ( MonsterList.Count != 0 )
        {
            currentDist = 0f;
            closeDistIndex = 0;
            TargetIndex = -1;

            for ( int i = 0 ; i < MonsterList.Count ; i++ )
            {
                currentDist = Vector3.Distance ( transform.position, MonsterList[i].transform.position );

                RaycastHit hit;
                bool isHit = Physics.Raycast ( transform.position, MonsterList[i].transform.position - transform.position,
                    out hit, 20f, layerMask );

                if ( isHit && hit.transform.CompareTag ( "Monster" ) )
                {
                    if ( TargetDist >= currentDist )
                    {
                        TargetIndex = i;
                        TargetDist = currentDist;
                    }
                }

                if ( closetDist >= currentDist )
                {
                    closeDistIndex = i;
                    closetDist = currentDist;
                }
            }

            if ( TargetIndex == -1 )
            {
                TargetIndex = closeDistIndex;
            }
            closetDist = 100f;
            TargetDist = 100f;
            getATarget = true;
        }
        
    }

    void AtkTarget ( )
    {
        //타겟이 존재하고 움직이고 있지 않다면
        if ( getATarget && !VariableJoystick.Instance.isPlayerMoving )
        {

            Vector3 _targetLookPosition = new Vector3(MonsterList[TargetIndex].transform.position.x, transform
                .position.y, MonsterList[TargetIndex].transform.position.z);
            Vector3 _lookAtPosition = Vector3.SmoothDamp (transform.position, _targetLookPosition, ref refVelocity, 
                smoothDampTime);
            transform.LookAt (_lookAtPosition);
            
            Attack ( );
        
            //애니메이션 로직
            // if (PlayerMovement.Instance.Anim.GetCurrentAnimatorStateInfo ( 0 ).IsName ( "Idle" ) )
            // {
            //     PlayerMovement.Instance.Anim.SetBool ( "Idle", false );
            //     PlayerMovement.Instance.Anim.SetBool ( "Walk", false );
            //     PlayerMovement.Instance.Anim.SetBool ( "Attack", true );
            // }
        }
        //움직이고 있다면
        else if ( VariableJoystick.Instance.isPlayerMoving )
        {
            //애니메이션 로직
            // if ( !PlayerMovement.Instance.Anim.GetCurrentAnimatorStateInfo ( 0 ).IsName ( "Walk" ) )
            // {
            //     PlayerMovement.Instance.Anim.SetBool ( "Attack", false );
            //     PlayerMovement.Instance.Anim.SetBool ( "Idle", false );
            //     PlayerMovement.Instance.Anim.SetBool ( "Walk", true );
            // }
        }
    }
}