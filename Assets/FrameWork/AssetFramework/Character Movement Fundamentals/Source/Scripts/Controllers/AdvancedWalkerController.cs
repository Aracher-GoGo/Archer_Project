using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CMF
{
    //고급 워커 컨트롤러 스크립트;
    //이 컨트롤러는 다른 컨트롤러 유형의 기반으로 사용됩니다('SidescrollerController').
    //사용자 정의 이동 입력은 'AdvancedWalkerController'를 상속하고 'CalculateMovementDirection' 함수를 재정의하는 새 스크립트를 생성하여 구현할 수 있습니다.
    public class AdvancedWalkerController : Controller
    {
        //References to attached components;
        protected Transform tr;
        protected Mover mover;

        public GameObject TargetInput;
        public CharacterInput characterInput;
        protected CeilingDetector ceilingDetector;

        //Jump key variables;
        bool jumpInputIsLocked = false;
        bool jumpKeyWasPressed = false;
        bool jumpKeyWasLetGo = false;
        bool jumpKeyIsPressed = false;

        //Movement speed;
        public float movementSpeed = 7f;

        //How fast the controller can change direction while in the air;
        //Higher values result in more air control;
        public float airControlRate = 2f;

        //Jump speed;
        public float jumpSpeed = 10f;

        //Jump duration variables;
        public float jumpDuration = 0.2f;
        float currentJumpStartTime = 0f;

        //'AirFriction' determines how fast the controller loses its momentum while in the air;
        //'GroundFriction' is used instead, if the controller is grounded;
        public float airFriction = 0.5f;
        public float groundFriction = 100f;

        //Current momentum;
        protected Vector3 momentum = Vector3.zero;

        //Saved velocity from last frame;
        Vector3 savedVelocity = Vector3.zero;

        //Saved horizontal movement velocity from last frame;
        Vector3 savedMovementVelocity = Vector3.zero;

        //Amount of downward gravity;
        public float gravity = 30f;

        [Tooltip("How fast the character will slide down steep slopes.")]
        public float slideGravity = 5f;


        //허용 가능한 기울기 각도 제한;
        public float slopeLimit = 80f;

        [Tooltip("컨트롤러의 변환을 기준으로 운동량을 계산하고 적용할지 여부입니다.")]
        public bool useLocalMomentum = false;

        //Enum describing basic controller states; 
        public enum ControllerState
        {
            Grounded,
            Sliding,
            Falling,
            Rising,
            Jumping
        }

        ControllerState currentControllerState = ControllerState.Falling;

        [Tooltip("이동 방향 계산에 사용되는 선택적 카메라 변환입니다. 할당된 경우 캐릭터 이동은 카메라 뷰를 고려합니다.")]
        public Transform cameraTransform;

        //Get references to all necessary components;
        void Awake()
        {
            mover = GetComponent<Mover>();
            tr = transform;
            // characterInput = GetComponent<CharacterInput>();
            characterInput = TargetInput.GetComponent<CharacterInput>();
            ceilingDetector = GetComponent<CeilingDetector>();

            if (characterInput == null)
                Debug.LogWarning("No character input script has been attached to this gameobject",
                    this.gameObject);

            Setup();
        }

        //This function is called right after Awake(); It can be overridden by inheriting scripts;
        protected virtual void Setup()
        {
        }

        void Update()
        {
            HandleJumpKeyInput();
        }

        //Handle jump booleans for later use in FixedUpdate;
        void HandleJumpKeyInput()
        {
            bool _newJumpKeyPressedState = IsJumpKeyPressed();

            if (jumpKeyIsPressed == false && _newJumpKeyPressedState == true)
                jumpKeyWasPressed = true;

            if (jumpKeyIsPressed == true && _newJumpKeyPressedState == false)
            {
                jumpKeyWasLetGo = true;
                jumpInputIsLocked = false;
            }

            jumpKeyIsPressed = _newJumpKeyPressedState;
        }

        void FixedUpdate()
        {
            ControllerUpdate();
        }

        //Update controller;
        //This function must be called every fixed update, in order for the controller to work correctly;
        void ControllerUpdate()
        {
            //무버가 접지되었는지 확인합니다.
            mover.CheckForGround();

            //컨트롤러 상태 결정;
            currentControllerState = DetermineControllerState();

            //'운동량'에 마찰과 중력을 적용합니다.
            HandleMomentum();

            //플레이어가 점프를 시작했는지 확인합니다.
            HandleJumping();

            //이동 속도 계산;
            Vector3 _velocity = Vector3.zero;
            if (currentControllerState == ControllerState.Grounded)
                _velocity = CalculateMovementVelocity();

            //로컬 모멘텀을 사용한다면 모멘텀을 월드 공간으로 먼저 변환한다.
            Vector3 _worldMomentum = momentum;
            if (useLocalMomentum)
                _worldMomentum = tr.localToWorldMatrix * momentum;

            //속도에 현재 운동량 추가;
            _velocity += _worldMomentum;

            //플레이어가 땅에 떨어지거나 경사면에서 미끄러지면 무버의 센서 범위를 확장합니다.
            //이렇게 하면 플레이어가 지면 접촉을 잃지 않고 계단과 경사면을 오르내릴 수 있습니다.
            mover.SetExtendSensorRange(IsGrounded());

            //무버 속도 설정;
            mover.SetVelocity(_velocity);

            //다음 프레임의 속도를 저장합니다.
            savedVelocity = _velocity;

            //컨트롤러 이동 속도 저장;
            savedMovementVelocity = CalculateMovementVelocity();

            //Reset jump key booleans;
            jumpKeyWasLetGo = false;
            jumpKeyWasPressed = false;

            //이 게임 오브젝트에 연결된 경우 천장 감지기를 재설정합니다.
            if (ceilingDetector != null)
                ceilingDetector.ResetFlags();
        }

        //플레이어 입력을 기반으로 이동 방향을 계산하고 반환합니다.
        //이 함수는 다른 플레이어 컨트롤을 구현하는 스크립트를 상속하여 재정의할 수 있습니다.
        protected virtual Vector3 CalculateMovementDirection()
        {
            //이 객체에 문자 입력 스크립트가 첨부되어 있지 않으면 return;
            if (characterInput == null)
                return Vector3.zero;

            Vector3 _velocity = Vector3.zero;

            //카메라 변환이 할당되지 않은 경우 캐릭터의 변환 축을 사용하여 이동 방향을 계산합니다.
            if (cameraTransform == null)
            {
                _velocity += tr.right * characterInput.GetHorizontalMovementInput();
                _velocity += tr.forward * characterInput.GetVerticalMovementInput();
            }
            else
            {
                //카메라 변환이 할당된 경우 이동 방향에 대해 할당된 변환의 축을 사용합니다.
                // 이동 방향을 투영하여 이동이 지면과 평행을 유지합니다.
                _velocity += Vector3.ProjectOnPlane(cameraTransform.right, tr.up).normalized *
                             characterInput.GetHorizontalMovementInput();
                _velocity += Vector3.ProjectOnPlane(cameraTransform.forward, tr.up).normalized *
                             characterInput.GetVerticalMovementInput();
            }
            
            //필요한 경우 이동 벡터를 1f의 크기로 고정합니다.
            if (_velocity.magnitude > 1f)
                _velocity.Normalize();

            return _velocity;
        }

        //플레이어 입력, 컨트롤러 상태, 그라운드 노멀을 기반으로 이동 속도를 계산하고 반환합니다. [...];
        protected virtual Vector3 CalculateMovementVelocity()
        {
            //(정규화된) 이동 방향 계산;
            Vector3 _velocity = CalculateMovementDirection();

            //(정규화된) 속도에 이동 속도를 곱합니다.
            _velocity *= movementSpeed;

            return _velocity;
        }

        //Returns 'true' if the player presses the jump key;
        protected virtual bool IsJumpKeyPressed()
        {
            //If no character input script is attached to this object, return;
            if (characterInput == null)
                return false;

            return characterInput.IsJumpKeyPressed();
        }

        //현재 운동량과 컨트롤러가 접지되었는지 여부를 기반으로 현재 컨트롤러 상태를 결정합니다.
        // 상태 전환을 처리합니다.
        ControllerState DetermineControllerState()
        {
            // 수직 모멘텀이 위쪽을 가리키는지 확인합니다.
            bool _isRising = IsRisingOrFalling() &&
                             (VectorMath.GetDotProduct(GetMomentum(), tr.up) > 0f);

            //컨트롤러가 슬라이딩하는지 확인합니다.
            bool _isSliding = mover.IsGrounded() && IsGroundTooSteep();

            //Grounded;
            if (currentControllerState == ControllerState.Grounded)
            {
                if (_isRising)
                {
                    OnGroundContactLost();
                    return ControllerState.Rising;
                }

                if (!mover.IsGrounded())
                {
                    OnGroundContactLost();
                    return ControllerState.Falling;
                }

                if (_isSliding)
                {
                    OnGroundContactLost();
                    return ControllerState.Sliding;
                }

                return ControllerState.Grounded;
            }

            //Falling;
            if (currentControllerState == ControllerState.Falling)
            {
                if (_isRising)
                {
                    return ControllerState.Rising;
                }

                if (mover.IsGrounded() && !_isSliding)
                {
                    OnGroundContactRegained();
                    return ControllerState.Grounded;
                }

                if (_isSliding)
                {
                    return ControllerState.Sliding;
                }

                return ControllerState.Falling;
            }

            //Sliding;
            if (currentControllerState == ControllerState.Sliding)
            {
                if (_isRising)
                {
                    OnGroundContactLost();
                    return ControllerState.Rising;
                }

                if (!mover.IsGrounded())
                {
                    OnGroundContactLost();
                    return ControllerState.Falling;
                }

                if (mover.IsGrounded() && !_isSliding)
                {
                    OnGroundContactRegained();
                    return ControllerState.Grounded;
                }

                return ControllerState.Sliding;
            }

            //Rising;
            if (currentControllerState == ControllerState.Rising)
            {
                if (!_isRising)
                {
                    if (mover.IsGrounded() && !_isSliding)
                    {
                        OnGroundContactRegained();
                        return ControllerState.Grounded;
                    }

                    if (_isSliding)
                    {
                        return ControllerState.Sliding;
                    }

                    if (!mover.IsGrounded())
                    {
                        return ControllerState.Falling;
                    }
                }

                //If a ceiling detector has been attached to this gameobject, check for ceiling hits;
                if (ceilingDetector != null)
                {
                    if (ceilingDetector.HitCeiling())
                    {
                        OnCeilingContact();
                        return ControllerState.Falling;
                    }
                }

                return ControllerState.Rising;
            }

            //Jumping;
            if (currentControllerState == ControllerState.Jumping)
            {
                //Check for jump timeout;
                if ((Time.time - currentJumpStartTime) > jumpDuration)
                    return ControllerState.Rising;

                //Check if jump key was let go;
                if (jumpKeyWasLetGo)
                    return ControllerState.Rising;

                //If a ceiling detector has been attached to this gameobject, check for ceiling hits;
                if (ceilingDetector != null)
                {
                    if (ceilingDetector.HitCeiling())
                    {
                        OnCeilingContact();
                        return ControllerState.Falling;
                    }
                }

                return ControllerState.Jumping;
            }

            return ControllerState.Falling;
        }

        //Check if player has initiated a jump;
        void HandleJumping()
        {
            if (currentControllerState == ControllerState.Grounded)
            {
                if ((jumpKeyIsPressed == true || jumpKeyWasPressed) && !jumpInputIsLocked)
                {
                    //Call events;
                    OnGroundContactLost();
                    OnJumpStart();

                    currentControllerState = ControllerState.Jumping;
                }
            }
        }

        //Apply friction to both vertical and horizontal momentum based on 'friction' and 'gravity';
        //Handle movement in the air;
        //Handle sliding down steep slopes;
        void HandleMomentum()
        {
            //If local momentum is used, transform momentum into world coordinates first;
            if (useLocalMomentum)
                momentum = tr.localToWorldMatrix * momentum;

            Vector3 _verticalMomentum = Vector3.zero;
            Vector3 _horizontalMomentum = Vector3.zero;

            //Split momentum into vertical and horizontal components;
            if (momentum != Vector3.zero)
            {
                _verticalMomentum = VectorMath.ExtractDotVector(momentum, tr.up);
                _horizontalMomentum = momentum - _verticalMomentum;
            }

            //Add gravity to vertical momentum;
            _verticalMomentum -= tr.up * gravity * Time.deltaTime;

            //Remove any downward force if the controller is grounded;
            if (currentControllerState == ControllerState.Grounded &&
                VectorMath.GetDotProduct(_verticalMomentum, tr.up) < 0f)
                _verticalMomentum = Vector3.zero;

            //Manipulate momentum to steer controller in the air (if controller is not grounded or sliding);
            if (!IsGrounded())
            {
                Vector3 _movementVelocity = CalculateMovementVelocity();

                //If controller has received additional momentum from somewhere else;
                if (_horizontalMomentum.magnitude > movementSpeed)
                {
                    //Prevent unwanted accumulation of speed in the direction of the current momentum;
                    if (VectorMath.GetDotProduct(_movementVelocity,
                            _horizontalMomentum.normalized) > 0f)
                        _movementVelocity = VectorMath.RemoveDotVector(_movementVelocity,
                            _horizontalMomentum.normalized);

                    //Lower air control slightly with a multiplier to add some 'weight' to any momentum applied to the controller;
                    float _airControlMultiplier = 0.25f;
                    _horizontalMomentum += _movementVelocity * Time.deltaTime * airControlRate *
                                           _airControlMultiplier;
                }
                //If controller has not received additional momentum;
                else
                {
                    //Clamp _horizontal velocity to prevent accumulation of speed;
                    _horizontalMomentum += _movementVelocity * Time.deltaTime * airControlRate;
                    _horizontalMomentum =
                        Vector3.ClampMagnitude(_horizontalMomentum, movementSpeed);
                }
            }

            //Steer controller on slopes;
            if (currentControllerState == ControllerState.Sliding)
            {
                //Calculate vector pointing away from slope;
                Vector3 _pointDownVector =
                    Vector3.ProjectOnPlane(mover.GetGroundNormal(), tr.up).normalized;

                //Calculate movement velocity;
                Vector3 _slopeMovementVelocity = CalculateMovementVelocity();
                //Remove all velocity that is pointing up the slope;
                _slopeMovementVelocity =
                    VectorMath.RemoveDotVector(_slopeMovementVelocity, _pointDownVector);

                //Add movement velocity to momentum;
                _horizontalMomentum += _slopeMovementVelocity * Time.fixedDeltaTime;
            }

            //Apply friction to horizontal momentum based on whether the controller is grounded;
            if (currentControllerState == ControllerState.Grounded)
                _horizontalMomentum =
                    VectorMath.IncrementVectorTowardTargetVector(_horizontalMomentum,
                        groundFriction, Time.deltaTime, Vector3.zero);
            else
                _horizontalMomentum =
                    VectorMath.IncrementVectorTowardTargetVector(_horizontalMomentum, airFriction,
                        Time.deltaTime, Vector3.zero);

            //Add horizontal and vertical momentum back together;
            momentum = _horizontalMomentum + _verticalMomentum;

            //Additional momentum calculations for sliding;
            if (currentControllerState == ControllerState.Sliding)
            {
                //Project the current momentum onto the current ground normal if the controller is sliding down a slope;
                momentum = Vector3.ProjectOnPlane(momentum, mover.GetGroundNormal());

                //Remove any upwards momentum when sliding;
                if (VectorMath.GetDotProduct(momentum, tr.up) > 0f)
                    momentum = VectorMath.RemoveDotVector(momentum, tr.up);

                //Apply additional slide gravity;
                Vector3 _slideDirection =
                    Vector3.ProjectOnPlane(-tr.up, mover.GetGroundNormal()).normalized;
                momentum += _slideDirection * slideGravity * Time.deltaTime;
            }

            //If controller is jumping, override vertical velocity with jumpSpeed;
            if (currentControllerState == ControllerState.Jumping)
            {
                momentum = VectorMath.RemoveDotVector(momentum, tr.up);
                momentum += tr.up * jumpSpeed;
            }

            if (useLocalMomentum)
                momentum = tr.worldToLocalMatrix * momentum;
        }

        //Events;

        //This function is called when the player has initiated a jump;
        void OnJumpStart()
        {
            //If local momentum is used, transform momentum into world coordinates first;
            if (useLocalMomentum)
                momentum = tr.localToWorldMatrix * momentum;

            //Add jump force to momentum;
            momentum += tr.up * jumpSpeed;

            //Set jump start time;
            currentJumpStartTime = Time.time;

            //Lock jump input until jump key is released again;
            jumpInputIsLocked = true;

            //Call event;
            if (OnJump != null)
                OnJump(momentum);

            if (useLocalMomentum)
                momentum = tr.worldToLocalMatrix * momentum;
        }

        //This function is called when the controller has lost ground contact, i.e. is either falling or rising, or generally in the air;
        void OnGroundContactLost()
        {
            //If local momentum is used, transform momentum into world coordinates first;
            if (useLocalMomentum)
                momentum = tr.localToWorldMatrix * momentum;

            //Get current movement velocity;
            Vector3 _velocity = GetMovementVelocity();

            //Check if the controller has both momentum and a current movement velocity;
            if (_velocity.sqrMagnitude >= 0f && momentum.sqrMagnitude > 0f)
            {
                //Project momentum onto movement direction;
                Vector3 _projectedMomentum = Vector3.Project(momentum, _velocity.normalized);
                //Calculate dot product to determine whether momentum and movement are aligned;
                float _dot = VectorMath.GetDotProduct(_projectedMomentum.normalized,
                    _velocity.normalized);

                //If current momentum is already pointing in the same direction as movement velocity,
                //Don't add further momentum (or limit movement velocity) to prevent unwanted speed accumulation;
                if (_projectedMomentum.sqrMagnitude >= _velocity.sqrMagnitude && _dot > 0f)
                    _velocity = Vector3.zero;
                else if (_dot > 0f)
                    _velocity -= _projectedMomentum;
            }

            //Add movement velocity to momentum;
            momentum += _velocity;

            if (useLocalMomentum)
                momentum = tr.worldToLocalMatrix * momentum;
        }

        //This function is called when the controller has landed on a surface after being in the air;
        void OnGroundContactRegained()
        {
            //Call 'OnLand' event;
            if (OnLand != null)
            {
                Vector3 _collisionVelocity = momentum;
                //If local momentum is used, transform momentum into world coordinates first;
                if (useLocalMomentum)
                    _collisionVelocity = tr.localToWorldMatrix * _collisionVelocity;

                OnLand(_collisionVelocity);
            }
        }

        //This function is called when the controller has collided with a ceiling while jumping or moving upwards;
        void OnCeilingContact()
        {
            //If local momentum is used, transform momentum into world coordinates first;
            if (useLocalMomentum)
                momentum = tr.localToWorldMatrix * momentum;

            //Remove all vertical parts of momentum;
            momentum = VectorMath.RemoveDotVector(momentum, tr.up);

            if (useLocalMomentum)
                momentum = tr.worldToLocalMatrix * momentum;
        }

        //Helper functions;

        //수직 운동량이 작은 임계값보다 높으면 'true'를 반환합니다.
        private bool IsRisingOrFalling()
        {
            //Calculate current vertical momentum;
            Vector3 _verticalMomentum = VectorMath.ExtractDotVector(GetMomentum(), tr.up);

            //Setup threshold to check against;
            //For most applications, a value of '0.001f' is recommended;
            float _limit = 0.001f;

            //Return true if vertical momentum is above '_limit';
            return (_verticalMomentum.magnitude > _limit);
        }

        //Returns true if angle between controller and ground normal is too big (> slope limit), i.e. ground is too steep;
        private bool IsGroundTooSteep()
        {
            if (!mover.IsGrounded())
                return true;

            return (Vector3.Angle(mover.GetGroundNormal(), tr.up) > slopeLimit);
        }

        //Getters;

        //Get last frame's velocity;
        public override Vector3 GetVelocity()
        {
            return savedVelocity;
        }

        //Get last frame's movement velocity (momentum is ignored);
        public override Vector3 GetMovementVelocity()
        {
            return savedMovementVelocity;
        }

        //Get current momentum;
        public Vector3 GetMomentum()
        {
            Vector3 _worldMomentum = momentum;
            if (useLocalMomentum)
                _worldMomentum = tr.localToWorldMatrix * momentum;

            return _worldMomentum;
        }

        //Returns 'true' if controller is grounded (or sliding down a slope);
        public override bool IsGrounded()
        {
            return (currentControllerState == ControllerState.Grounded ||
                    currentControllerState == ControllerState.Sliding);
        }

        //Returns 'true' if controller is sliding;
        public bool IsSliding()
        {
            return (currentControllerState == ControllerState.Sliding);
        }

        //Add momentum to controller;
        public void AddMomentum(Vector3 _momentum)
        {
            if (useLocalMomentum)
                momentum = tr.localToWorldMatrix * momentum;

            momentum += _momentum;

            if (useLocalMomentum)
                momentum = tr.worldToLocalMatrix * momentum;
        }

        //Set controller momentum directly;
        public void SetMomentum(Vector3 _newMomentum)
        {
            if (useLocalMomentum)
                momentum = tr.worldToLocalMatrix * _newMomentum;
            else
                momentum = _newMomentum;
        }
    }
}