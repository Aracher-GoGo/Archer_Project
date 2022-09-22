using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CMF
{
    //이 스크립트는 속도 값 및 기타 정보('isGrounded')를 애니메이터 구성 요소에 전달하여 캐릭터의 애니메이션을 제어합니다.
    public class AnimationControl : MonoSingleton<AnimationControl>
    {

        Controller controller;
        Animator animator;
        PlayerTargeting targeting;
        
        Transform animatorTransform;
        Transform tr;

        //캐릭터가 스트래핑 블렌드 트리를 사용하는지 여부;
        public bool useStrafeAnimations = false;

        //착륙 애니메이션의 속도 임계값;
        //애니메이션은 하향 속도가 이 임계값을 초과하는 경우에만 트리거됩니다.
        public float landVelocityThreshold = 5f;

        private float smoothingFactor = 40f;
        Vector3 oldMovementVelocity = Vector3.zero;

        //Setup;
        void Awake () {
            controller = GetComponent<Controller>();
            animator = GetComponentInChildren<Animator>();
            targeting = GetComponent<PlayerTargeting>();
            
            animatorTransform = animator.transform;
            tr = transform;
        }

        //OnEnable;
        void OnEnable()
        {
            //Connect events to controller events;
            controller.OnLand += OnLand;
            controller.OnJump += OnJump;
        }

        //OnDisable;
        void OnDisable()
        {
            //Disconnect events to prevent calls to disabled gameobjects;
            controller.OnLand -= OnLand;
            controller.OnJump -= OnJump;
        }
		
        //Update;
        void Update () 
        {

            //Get controller velocity;
            Vector3 _velocity = controller.GetVelocity();

            //Split up velocity;
            Vector3 _horizontalVelocity = VectorMath.RemoveDotVector(_velocity, tr.up);
            Vector3 _verticalVelocity = _velocity - _horizontalVelocity;

            //Smooth horizontal velocity for fluid animation;
            _horizontalVelocity = Vector3.Lerp(oldMovementVelocity, _horizontalVelocity, smoothingFactor * Time.deltaTime);
            oldMovementVelocity = _horizontalVelocity;

            animator.SetFloat("VerticalSpeed", _verticalVelocity.magnitude * VectorMath.GetDotProduct(_verticalVelocity.normalized, tr.up));
            animator.SetFloat("HorizontalSpeed", _horizontalVelocity.magnitude);

            //If animator is strafing, split up horizontal velocity;
            if(useStrafeAnimations)
            {
                Vector3 _localVelocity = animatorTransform.InverseTransformVector(_horizontalVelocity);
                animator.SetFloat("ForwardSpeed", _localVelocity.z);
                animator.SetFloat("StrafeSpeed", _localVelocity.x);
            }

            //Pass values to animator;
            animator.SetBool("IsGrounded", controller.IsGrounded());
            animator.SetBool("IsStrafing", useStrafeAnimations);
            
            
            
        }

        void OnLand(Vector3 _v)
        {
            //Only trigger animation if downward velocity exceeds threshold;
            if(VectorMath.GetDotProduct(_v, tr.up) > -landVelocityThreshold)
                return;

            animator.SetTrigger("OnLand");
        }

        void OnJump(Vector3 _v)
        {
			
        }
    }
}