using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CMF;

//This controller provides basic 'click-to-move' functionality;
//It can be used as a starting point for a variety of top-down (or isometric) games, which are primarily controlled via mouse input;
public class ClickToMoveController : Controller
{
    //Whether the target position is determined by raycasting against an abstract plane or the actual level geometry;
    //'AbstractPlane' is less accurate, but simpler (and will automatically ignore colliders between the camera and target position);
    //'Raycast' is more accurate, but ceilings or intersecting geometry (between camera and target position) must be handled separately;
    public enum MouseDetectionType
    {
        AbstractPlane, 
        Raycast
    }
    
    
    //Controller movement speed;
    public float movementSpeed = 10f;
	
    //Downward gravity;
    public float gravity = 30f;

    float currentVerticalSpeed = 0f;
    bool isGrounded = false;

    //현재 이동할 위치;
    Vector3 currentTargetPosition;
	
    //컨트롤러와 목표 위치 사이의 거리가 이보다 작으면 목표에 도달합니다.
    float reachTargetThreshold = 0.001f;

    //사용자가 마우스 버튼을 계속 누르고 있으면 컨트롤러를 계속 이동할 수 있는지 여부;
    public bool holdMouseButtonToMove = false;
    
    public MouseDetectionType mouseDetectionType = MouseDetectionType.AbstractPlane;
    
    //'Raycast'가 선택되었을 때 사용되는 레이어 마스크;
    public LayerMask raycastLayerMask = ~0;

    //타임아웃 변수;
    //컨트롤러가 벽에 기대어 걸어가는 것이 멈춘 경우, 일정 시간 동안 일정 거리 이상 이동하지 않으면 이동이 취소됩니다.
    //'timeOutTime'은 컨트롤러가 이동해야 하는 시간 창을 제어합니다. 그렇지 않으면 이동이 중지됩니다.
    public float timeOutTime = 1f;
    float currentTimeOutTime = 1f;
    
    //이는 이동에 필요한 최소 거리를 제어합니다. 그렇지 않으면 컨트롤러가 이동을 멈춥니다.
    public float timeOutDistanceThreshold = 0.05f;
    Vector3 lastPosition;

    //플레이어의 카메라에 대한 참조(레이캐스팅에 사용됨);
    public Camera playerCamera;

    //컨트롤러가 현재 이동할 유효한 대상 위치가 있는지 여부;
    bool hasTarget = false;

    Vector3 lastVelocity = Vector3.zero;
    Vector3 lastMovementVelocity = Vector3.zero;

    //Abstarct ground plane used when 'AbstractPlane' is selected;
    Plane groundPlane;

    //Reference to attached 'Mover' and transform component;
    Mover mover;
    Transform tr;

    void Start()
    {
        //Get references to necessary components;
        mover = GetComponent<Mover>();
        tr = transform;

        if(playerCamera == null)
            Debug.LogWarning("No camera has been assigned to this controller!", this);

        //Initialize variables;
        lastPosition = tr.position;
        currentTargetPosition = transform.position;
        groundPlane = new Plane(tr.up, tr.position);
    }

    void Update()
    {
        //Handle mouse input (check for input, determine new target position);
        HandleMouseInput();
    }

    void FixedUpdate()
    {
        //Run initial mover ground check;
        mover.CheckForGround();

        //Check whether the character is grounded;
        isGrounded = mover.IsGrounded();

        //Handle timeout (stop controller if it is stuck);
        HandleTimeOut();

        Vector3 _velocity = Vector3.zero;

        //Calculate the final velocity for this frame;
        _velocity = CalculateMovementVelocity();
        lastMovementVelocity = _velocity;

        //Calculate and apply gravity;
        HandleGravity();
        _velocity += tr.up * currentVerticalSpeed;

        //If the character is grounded, extend ground detection sensor range;
        mover.SetExtendSensorRange(isGrounded);
        //Set mover velocity;
        mover.SetVelocity(_velocity);

        //Save velocity for later;
        lastVelocity = _velocity;
    }

    //Calculate movement velocity based on the current target position;
    Vector3 CalculateMovementVelocity()
    {
        //Return no velocity if controller currently has no target;	
        if(!hasTarget)
            return Vector3.zero;

        //Calculate vector to target position;
        Vector3 _toTarget = currentTargetPosition - tr.position;

        //Remove all vertical parts of vector;
        _toTarget = VectorMath.RemoveDotVector(_toTarget, tr.up);
		
        //Calculate distance to target;
        float _distanceToTarget = _toTarget.magnitude;

        //If controller has already reached target position, return no velocity;
        if(_distanceToTarget <= reachTargetThreshold)
        {
            hasTarget = false;
            return Vector3.zero;
        }
			
        Vector3 _velocity = _toTarget.normalized * movementSpeed;

        //Check for overshooting;
        if(movementSpeed * Time.fixedDeltaTime > _distanceToTarget)
        {
            _velocity = _toTarget.normalized * _distanceToTarget;
            hasTarget = false;
        }
	
        return _velocity;
    }

    //Calculate current gravity;
    void HandleGravity()
    {
        //Handle gravity;
        if(!isGrounded)
            currentVerticalSpeed -= gravity * Time.deltaTime;
        else
        {
            if(currentVerticalSpeed < 0f)
            {
                if(OnLand != null)
                    OnLand(tr.up * currentVerticalSpeed);
            }
			
            currentVerticalSpeed = 0f;
        }
    }

    //Handle mouse input (mouse clicks, [...]);
    void HandleMouseInput()
    {
        //카메라가 할당되지 않은 경우 함수실행을 중지합니다
        if(playerCamera == null)
            return;

        //유효한 마우스 누름이 감지되면 레이캐스트하여 새 대상 위치를 결정합니다.
        if(!holdMouseButtonToMove && WasMouseButtonJustPressed() || holdMouseButtonToMove && IsMouseButtonPressed())
        {
            //마우스 광선 설정(화면 위치 기반);
            Ray _mouseRay = playerCamera.ScreenPointToRay(GetMousePosition());

            if(mouseDetectionType == MouseDetectionType.AbstractPlane)
            {
                //Set up abstract ground plane;
                groundPlane.SetNormalAndPosition(tr.up, tr.position);
                float _enter = 0f;

                //Raycast against ground plane;
                if(groundPlane.Raycast(_mouseRay, out _enter))
                {
                    currentTargetPosition = _mouseRay.GetPoint(_enter);
                    hasTarget = true;
                }
                else
                    hasTarget = false;
            }
            else if(mouseDetectionType == MouseDetectionType.Raycast)
            {
                RaycastHit _hit;

                //Raycast against level geometry;
                if(Physics.Raycast(_mouseRay, out _hit, 100f, raycastLayerMask, QueryTriggerInteraction.Ignore))
                {
                    currentTargetPosition = _hit.point;
                    hasTarget = true;
                }
                else
                    hasTarget = false;
            }
			
        }
    }

    //Handle timeout (stop controller from moving if it is stuck against level geometry);
    void HandleTimeOut()
    {
        //If controller currently has no target, reset time and return;
        if(!hasTarget)
        {
            currentTimeOutTime = 0f;
            return;
        }

        //If controller has moved enough distance, reset time;
        if(Vector3.Distance(tr.position, lastPosition) > timeOutDistanceThreshold)
        {
            currentTimeOutTime = 0f;
            lastPosition = tr.position;
        }
        //If controller hasn't moved a sufficient distance, increment current timeout time;
        else
        {
            currentTimeOutTime += Time.deltaTime;

            //If current timeout time has reached limit, stop controller from moving;
            if(currentTimeOutTime >= timeOutTime)
            {
                hasTarget = false;
            }
        }
    }

    //Get current screen position of mouse cursor;
    //This function can be overridden to implement other input methods;
    protected Vector2 GetMousePosition()
    {
        return Input.mousePosition;
    }

    //Check whether mouse button is currently pressed down;
    //This function can be overridden to implement other input methods;
    protected bool IsMouseButtonPressed()
    {
        return Input.GetMouseButton(0);
    }

    //Check whether mouse button was just pressed down;
    //This function can be overridden to implement other input methods;
    protected bool WasMouseButtonJustPressed()
    {
        return Input.GetMouseButtonDown(0);
    }

    public override bool IsGrounded()
    {
        return isGrounded;
    }

    public override Vector3 GetMovementVelocity()
    {
        return lastMovementVelocity;
    }

    public override Vector3 GetVelocity()
    {
        return lastVelocity;
    }
}