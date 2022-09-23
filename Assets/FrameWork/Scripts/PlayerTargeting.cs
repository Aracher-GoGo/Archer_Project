using System.Collections.Generic;
using UnityEngine;

public class PlayerTargeting : MonoBehaviour
{
    public static PlayerTargeting Instance // singlton     
    {
        get
        {
            if ( instance == null )
            {
                instance = FindObjectOfType<PlayerTargeting> ( );
                if ( instance == null )
                {
                    var instanceContainer = new GameObject ( "PlayerTargeting" );
                    instance = instanceContainer.AddComponent<PlayerTargeting> ( );
                }
            }
            return instance;
        }
    }
    private static PlayerTargeting instance;

    public bool getATarget = false;
    float currentDist = 0;      //현재 거리
    float closetDist = 100f;    //가까운 거리
    float TargetDist = 100f;   //타겟 거리
    int closeDistIndex = 0;    //가장 가까운 인덱스
    int TargetIndex = -1;      //타겟팅 할 인덱스
    public LayerMask layerMask;

    public float atkSpd = 1.0f;

    public List<GameObject> MonsterList = new List<GameObject> ( );
    //Monster를 담는 List 

    public GameObject PlayerBolt;  //발사체
    public Transform AttackPoint;

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
        PlayerMovement.Instance.Anim.SetFloat ( "AttackSpeed", atkSpd );
        Instantiate ( PlayerBolt, AttackPoint.position, transform.rotation );
    }

    void SetTarget ( )
    {
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
        if ( getATarget && !JoyStickMovement.Instance.isPlayerMoving )
        {
            transform.LookAt ( new Vector3 ( MonsterList[TargetIndex].transform.position.x, transform.position.y, MonsterList[TargetIndex].transform.position.z ) );
            // Attack ( );


            AnimatorStateInfo AnimatorInfo = PlayerMovement.Instance.Anim.GetCurrentAnimatorStateInfo(0);
            
                
            if ( PlayerMovement.Instance.Anim.GetCurrentAnimatorStateInfo ( 0 ).IsName ( "Idle" ) )
            {
                
                PlayerMovement.Instance.Anim.SetBool ( "Idle", false );
                PlayerMovement.Instance.Anim.SetBool ( "Walk", false );
                PlayerMovement.Instance.Anim.SetBool ( "Attack", true );
            }

        }
        else if ( JoyStickMovement.Instance.isPlayerMoving )
        {
            if ( !PlayerMovement.Instance.Anim.GetCurrentAnimatorStateInfo ( 0 ).IsName ( "Walk" ) )
            {
                PlayerMovement.Instance.Anim.SetBool ( "Attack", false );
                PlayerMovement.Instance.Anim.SetBool ( "Idle", false );
                PlayerMovement.Instance.Anim.SetBool ( "Walk", true );
            }
        }
    }
}