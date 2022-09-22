using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerMovement : MonoSingleton<PlayerMovement>
{
    
    private Rigidbody rb;
    
    //캐릭터 움직임에 사용될 스피드
    public float moveSpeed = 20.0f;
    
    //캐릭터에 재생될 애니메이터
    public Animator Anim;
    
    //조이스틱
    public VariableJoystick variableJoystick;

   
    void Start()
    {
        rb = GetComponent<Rigidbody> ( );
        Anim = GetComponent<Animator> ( );
    }

  
    void FixedUpdate()
    {
        #region 원래코드
        // if ( JoystickPlayerExample.Instance.joyVec.x != 0 || JoyStickMovement.Instance.joyVec.y != 0 )
        // {
        //     rb.velocity = new Vector3 ( JoyStickMovement.Instance.joyVec.x, 0, JoyStickMovement.Instance.joyVec.y ) * moveSpeed;
        //     
        //     rb.rotation = Quaternion.LookRotation ( new Vector3 ( JoyStickMovement.Instance.joyVec.x, 0, JoyStickMovement.Instance.joyVec.y ) );
        // }
        
        #endregion 원래코드
        
        if ( variableJoystick.Horizontal != 0 || variableJoystick.Vertical != 0 )
        {
            rb.velocity = new Vector3 ( variableJoystick.Horizontal, 0, variableJoystick.Vertical ) * moveSpeed;
            rb.rotation = Quaternion.LookRotation ( new Vector3 ( variableJoystick.Horizontal, 0, variableJoystick.Vertical ) );
        }
        else
        {
            rb.velocity =Vector3.zero;
        }
    }
}