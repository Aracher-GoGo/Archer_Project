using System.Collections;
using System.Collections.Generic;
using CMF;
using UnityEngine;
using UnityEngine.EventSystems;

public class VariableJoystick : CharacterJoystickInput
{
    public static VariableJoystick Instance { get; private set; }
    
    void Awake() => Instance = FindObjectOfType(typeof(VariableJoystick)) as VariableJoystick;
    
    public float MoveThreshold { get { return moveThreshold; } set { moveThreshold = Mathf.Abs(value); } }

    [SerializeField] private float moveThreshold = 1;
    
    [SerializeField] private JoystickType joystickType = JoystickType.Fixed;

    private Vector2 fixedPosition = Vector2.zero;
    
    public bool isPlayerMoving = false;

    public void SetMode(JoystickType _joystickType)
    {
        this.joystickType = _joystickType;
        if(_joystickType == JoystickType.Fixed)
        {
            background.anchoredPosition = fixedPosition;
            background.gameObject.SetActive(true);
        }
        else
            background.gameObject.SetActive(false);
    }

    protected override void Start()
    {
        base.Start();
        fixedPosition = background.anchoredPosition;
        SetMode(joystickType);
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        if(joystickType != JoystickType.Fixed)
        {
            background.anchoredPosition = ScreenPointToAnchoredPosition(eventData.position);
            background.gameObject.SetActive(true);
        }
        
        base.OnPointerDown(eventData);
        
        // if ( !PlayerMovement.Instance.Anim.GetCurrentAnimatorStateInfo ( 0 ).IsName ( "Walk" ) )
        // {
        //     //Debug.Log ( "WALK!" );
        //     PlayerMovement.Instance.Anim.SetBool ( "Attack", false );
        //     PlayerMovement.Instance.Anim.SetBool ( "Idle", false );
        //     PlayerMovement.Instance.Anim.SetBool ( "Walk", true );
        // }
        // PlayerTargeting.Instance.getATarget = false;
        // isPlayerMoving = true;
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        if(joystickType != JoystickType.Fixed)
            background.gameObject.SetActive(false);

        base.OnPointerUp(eventData);
        
        // if ( !PlayerMovement.Instance.Anim.GetCurrentAnimatorStateInfo ( 0 ).IsName ( "Idle" ) )
        // {
        //     //Debug.Log ( "IDLE!" );
        //     PlayerMovement.Instance.Anim.SetBool ( "Attack", false );
        //     PlayerMovement.Instance.Anim.SetBool ( "Walk", false );
        //     PlayerMovement.Instance.Anim.SetBool ( "Idle", true );
        // }
        // isPlayerMoving = false;
    }

    protected override void HandleInput(float magnitude, Vector2 normalised, Vector2 radius, Camera cam)
    {
        if (joystickType == JoystickType.Dynamic && magnitude > moveThreshold)
        {
            Vector2 difference = normalised * (magnitude - moveThreshold) * radius;
            background.anchoredPosition += difference;
        }
        base.HandleInput(magnitude, normalised, radius, cam);
    }
}

public enum JoystickType { Fixed, Floating, Dynamic }