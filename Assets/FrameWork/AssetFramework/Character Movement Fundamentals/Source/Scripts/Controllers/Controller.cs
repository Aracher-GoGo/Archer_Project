using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CMF
{
    //이 추상 클래스는 다른 모든 컨트롤러 구성 요소(예: 'AdvancedWalkerController')의 기반입니다.
    //사용자 정의 컨트롤러 클래스를 생성하도록 확장할 수 있습니다.
    
    public abstract class Controller : MonoBehaviour {

        //Getters;
        public abstract Vector3 GetVelocity();
		
        public abstract Vector3 GetMovementVelocity();
		
        public abstract bool IsGrounded();

        //Events;
        public delegate void VectorEvent(Vector3 v);
		
        public VectorEvent OnJump;
		
        public VectorEvent OnLand;

    }
}