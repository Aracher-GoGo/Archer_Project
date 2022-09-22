using UnityEngine;
using System.Collections;

namespace CMF
{
    //'UpdateType'은 스무딩 함수가 'Update' 또는 'LateUpdate'에서 호출되는지 여부를 제어합니다.
    public enum UpdateType
    {
        Update,
        LateUpdate
    }

    //Different smoothtypes use different algorithms to smooth out the target's position; 
    public enum SmoothType
    {
        Lerp,
        SmoothDamp, 
    }
	
    //이 스크립트는 게임 객체의 위치를 부드럽게 합니다.
    public class SmoothPosition : MonoBehaviour {

        //위치 값이 복사되고 매끄럽게 될 대상 변환;
        public Transform target;
        Transform tr;

        Vector3 currentPosition;
		
        public UpdateType updateType;

        public SmoothType smoothType;
		
        //SmoothType으로 'Lerp'를 선택했을 때 현재 위치가 목표 위치로 얼마나 빨리 스무딩되는지를 제어하는 속도;
        public float lerpSpeed = 20f;

        //Time that controls how fast the current position will be smoothed toward the target position when 'SmoothDamp' is selected as smoothType;
        public float smoothDampTime = 0.02f;

        // 'SmoothDamp'가 smoothType으로 선택되었을 때 현재 위치가 목표 위치로 얼마나 빨리 스무딩되는지를 제어하는 시간;
        public bool extrapolatePosition = false;
		
        //게임 시작 시 로컬 위치 오프셋;
        Vector3 localPositionOffset;

        Vector3 refVelocity;
		
        //Awake;
        void Awake () {
			
            //선택된 대상이 없으면 이 변환의 부모를 대상으로 선택합니다.
            if(target == null)
                target = this.transform.parent;

            Transform _transform = transform;
			
            tr = _transform;
            currentPosition = _transform.position;

            localPositionOffset = tr.localPosition;
        }

        //OnEnable;
        void OnEnable()
        {
            // 마지막 위치에서 원치 않는 보간을 방지하기 위해 게임 오브젝트가 다시 활성화되면 현재 위치를 재설정합니다.
            ResetCurrentPosition();
        }

        void Update ()
        {
            if(updateType == UpdateType.LateUpdate)
                return;
			
            SmoothUpdate();
            
        }

        void LateUpdate ()
        {
            if(updateType == UpdateType.Update)
                return;
			
            SmoothUpdate();
        }

        void SmoothUpdate()
        {
            //Smooth current position;
            currentPosition = Smooth (currentPosition, target.position, lerpSpeed);

            //Set position;
            tr.position = currentPosition;
        }

        Vector3 Smooth(Vector3 _start, Vector3 _target, float _smoothTime)
        {
            //Convert local position offset to world coordinates;
            Vector3 _offset = tr.localToWorldMatrix * localPositionOffset;

            //If 'extrapolateRotation' is set to 'true', calculate a new target position;
            if (extrapolatePosition)
            {
                Vector3 _difference = _target - (_start - _offset);
                _target += _difference;
            }

            //Add local position offset to target;
            _target += _offset;

            //Smooth (based on chosen smoothType) and return position;
            switch (smoothType)
            {
                case SmoothType.Lerp:
                    return Vector3.Lerp (_start, _target, Time.deltaTime * _smoothTime);
                case SmoothType.SmoothDamp:
                    return Vector3.SmoothDamp (_start, _target, ref refVelocity, smoothDampTime);
                default:
                    return Vector3.zero;
            }
        }

        //Reset stored position and move this gameobject directly to the target's position;
        //Call this function if the target has just been moved a larger distance and no interpolation should take place (teleporting);
        public void ResetCurrentPosition()
        {
            //로컬 위치 오프셋을 세계 좌표로 변환합니다.
            Vector3 _offset = tr.localToWorldMatrix * localPositionOffset;
            
            //위치 오프셋을 추가하고 현재 위치를 설정합니다.
            currentPosition = target.position + _offset;
        }
    }
}