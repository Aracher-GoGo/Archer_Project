using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement Instance // singlton     
    {
        get
        {
            if ( instance == null )
            {
                instance = FindObjectOfType<PlayerMovement> ( );
                if ( instance == null )
                {
                    var instanceContainer = new GameObject ( "PlayerMovement" );
                    instance = instanceContainer.AddComponent<PlayerMovement> ( );
                }
            }
            return instance;
        }
    }
    
    private static PlayerMovement instance;

    Rigidbody rb;
    public float moveSpeed = 5f;
    public Animator Anim;

   
    void Start()
    {
        rb = GetComponent<Rigidbody> ( );
        Anim = GetComponent<Animator> ( );
    }

 
    void FixedUpdate()
    {
        if ( JoyStickMovement.Instance.joyVec.x != 0 || JoyStickMovement.Instance.joyVec.y != 0 )
        {
            rb.velocity = new Vector3 ( JoyStickMovement.Instance.joyVec.x, 0, JoyStickMovement.Instance.joyVec.y ) * moveSpeed;
            rb.rotation = Quaternion.LookRotation ( new Vector3 ( JoyStickMovement.Instance.joyVec.x, 0, JoyStickMovement.Instance.joyVec.y ) );
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("NextRoom"))
        {
            Debug.Log("Get Next Room");
            StageMgr.Instance.NextStage();
        }
    }
}
