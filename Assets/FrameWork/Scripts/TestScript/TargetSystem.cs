using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace ArcheryDemo
{
    public class TargetSystem : MonoBehaviour
    {
    
        #region Delegate Event
    
        public delegate void FOnTargetingToggled(bool _enabled);

        public FOnTargetingToggled onTargetingToggled;

        public delegate void FOnTargetChanged(GameObject _newTarget);

        public FOnTargetChanged onTargetChanged;
    
        #endregion Delegate Event
    
    
    
        // #region Field Members
        //
        // public bool isTargetingEnabled =false;
        //
        // public bool isFreeCamera =false;
        //
        //
        // private GameObject selectedTarget;
        //
        //
        // [FormerlySerializedAs("defaultLayerMask")] [Header("Trace Settings")]
        // public LayerMask obstacleLayerMask; //허용되지 않는 유형 (ex 벽 장애물)
        //
        //
        //
        // public float targetingMaxDistance = 300.0f;
        // public float traceHeightOffset = 1.0f; // 윗방향 offset
        // public float traceDepthOffset = 1.0f;  // 정면 offset
        //
        // [Header("Character Move")]
        // public float rotationSpeed = 30.0f;
        //
        // private Camera mainCamera;
        //
        // // 소유자와 대상 사이의 시야를 차단하는 것이 있는 경우 이 지연 후에 대상 지정 시스템이 비활성화됩니다.
        // public float disableOnBlockDelay = 2.0f;
        //
        //
        // private bool isCoroutineingCheckTargetHandle =false;
        // private bool isCoroutineingDisableCameraLockHandle =false;
        //
        // #endregion Field Members
        //
        //
        //
        // #region Functions
        //
        // void Start()
        // {
        //     mainCamera = Camera.main;
        // }
        //
        //
        // void Update()
        // {
        //     // FindTarget();
        //     UpdateCameraLock();
        // }
        //
        //
        // void UpdateCameraLock()
        // {
        //     if (isTargetingEnabled)
        //     {
        //         //거리를 체크합니다
        //         float _targetDistance = Vector3.Distance(transform.position, selectedTarget.transform.position);
        //     
        //         //타겟과의 거리가 5보다 크고 (그리고) 최대거리보다 작다면
        //         if (_targetDistance >= 1.0f && _targetDistance <= targetingMaxDistance)
        //         {
        //             // //카메라가 자유롭지 않다면
        //             // if (!isFreeCamera)
        //             // {
        //             //     Vector3 _dir = selectedActor.transform.position - transform.position;
        //             //     Quaternion _newRotation = Quaternion.LookRotation(_dir).normalized;
        //             //     transform.rotation = Quaternion.Slerp(transform.rotation,_newRotation,rotationSpeed*Time.deltaTime);
        //             // }
        //         }
        //         else
        //         {
        //             Debug.Log("실행되면 안됨");
        //             DisableCameraLock();
        //         }
        //     }
        // }
        //
        //
        // private void EnableCameraLook()
        // {
        //     isTargetingEnabled = true;
        //     
        //     StopCoroutine(DisableCameraLockHandle(0.0f));
        //     
        //     selectedTarget.GetComponent<MonsterBase>().I_OnSelected();
        //     
        //     if (!isCoroutineingCheckTargetHandle)
        //     {
        //         StartCoroutine(CheckTargetHandle(false,0.15f));
        //     }
        //
        //     onTargetingToggled(true);
        // }
        //
        // public void DisableCameraLock()
        // {
        //     //타겟팅이 활성화 상태라면
        //     if (isTargetingEnabled)
        //     {
        //         //비활성화 상태로 변경한다
        //         isTargetingEnabled = false;
        //
        //         selectedTarget.GetComponent<MonsterBase>().I_OnDeselected();
        //         selectedTarget=null;
        //         
        //         StopCoroutine(CheckTargetHandle(true,0.0f));
        //
        //         onTargetingToggled(false);
        //     }
        // }
        //
        //
        // // ReSharper disable Unity.PerformanceAnalysis
        // IEnumerator CheckTargetHandle(bool _loopingBreak,float _time)
        // {
        //     while (true)
        //     {
        //         isCoroutineingCheckTargetHandle = true;
        //         
        //         if (_loopingBreak)
        //             break;
        //
        //         List<GameObject> _objectToIgnores = new List<GameObject> { selectedTarget };
        //     
        //         //타겟오브젝트와 사이에 무엇인가 있다면
        //         if (IsAnythingBlockingTrace(selectedTarget, _objectToIgnores))
        //         {
        //             Debug.Log("타겟오브젝트와 사이에 무엇인가 있다면");
        //             // if (!isCoroutineing_DisableCameraLockHandle)
        //             // {
        //             //     if (DisableOnBlockDelay == 0.0f)
        //             //     {
        //             //         DisableCameraLock();
        //             //     }
        //             //     else //DisableOnBlockDelay == 0.0f 가 아니라면
        //             //     {
        //             //         StartCoroutine(DisableCameraLockHandle(DisableOnBlockDelay));
        //             //     }
        //             // }
        //         }
        //         else //타겟오브젝트와 사이에 무엇인가 없다면
        //         {
        //             StopCoroutine(DisableCameraLockHandle(0.0f));
        //            
        //             if (selectedTarget!= null)
        //             {
        //                 if (selectedTarget.GetComponent<MonsterBase>().I_IsTargetAble())
        //                 {
        //                     Debug.Log("타겟오브젝트와 사이에 무엇인가 없고 추적할수 있습니다");
        //                     DisableCameraLock();
        //                     FindTarget();
        //                 }
        //             }
        //            
        //         }
        //         yield return new WaitForSeconds(_time);
        //     }
        //
        //     isCoroutineingCheckTargetHandle = false;
        // }
        //
        // private IEnumerator DisableCameraLockHandle(float _timeDelay)
        // {
        //     isCoroutineingDisableCameraLockHandle = true;
        //     yield return new WaitForSeconds(_timeDelay);
        //
        //     DisableCameraLock();
        //
        //     isCoroutineingDisableCameraLockHandle = false;
        // }
        //
        //
        //
        //
        // // ReSharper disable Unity.PerformanceAnalysis
        // public void ToggleCameraLock()
        // {
        //     if (isTargetingEnabled)
        //         DisableCameraLock();
        //     else
        //         FindTarget();
        // }
        //
        //
        //
        // private void FindTarget()
        // {
        //     List<GameObject> _potentialTargets = new List<GameObject>(); //잠재적인 타겟들
        //     float _distanceFromCenterOfPlayer = 0.0f; //플레이어 중심으로부터의 거리
        //
        //     //잠재적인 타겟팅될수 있는 애들을 추려내는 작업
        //     foreach (MonsterBase _localPotentialTarget in MonsterManager.Instance.MonsterBases)
        //     {
        //         //타겟팅이 가능하고 (그리고) 현재 오브젝트와 선택된 액터가 다르다면
        //         if (_localPotentialTarget.I_IsTargetAble() && this.gameObject != selectedTarget)
        //         {
        //             //뷰포트안에 잠재적인 타겟이 존재하고 (그리고) 최대 타겟팅 거리보다 작거나 같다면
        //             if (IsInViewPort(_localPotentialTarget) && Vector3.Distance(transform.position, _localPotentialTarget.transform.position) <= targetingMaxDistance)
        //             {
        //                 //막히는 것이 없다면
        //                 if (!IsAnythingBlockingTrace(_localPotentialTarget.gameObject, _potentialTargets))
        //                 {
        //                     _potentialTargets.Add(_localPotentialTarget.gameObject);
        //                 }
        //             }
        //         }
        //     } //for break
        //
        //
        //     for (var _i = 0; _i < _potentialTargets.Count; _i++)
        //     {
        //         GameObject _localPotentialTarget = _potentialTargets[_i];
        //     
        //         // Screen.height  // 높이
        //         // Screen.width   // 너비
        //     
        //         // Vector3 _worldToScreenPoint = mainCamera.WorldToScreenPoint(_localPotentialTarget.transform.position);
        //         // float _screenDistanceX = Math.Abs(_worldToScreenPoint.x - (Screen.width / 2.0f)); //수평
        //         // float _screenDistanceZ = Math.Abs(Screen.height -_worldToScreenPoint.y  ); //수직
        //
        //   
        //         // float _screenDistance = Math.Abs(_screenDistanceX+_screenDistanceZ);
        //
        //         // //현재 선택된 타겟으로 가장 먼저 확인된 잠재적인 대상을 확인한다
        //         // if (_i == 0)
        //         // {
        //         //     _distanceFromCenterOfViewport = _screenDistance;
        //         //     selectedActor = _localPotentialTarget;
        //         // }
        //         // else
        //         // {
        //         //     if (_screenDistance < _distanceFromCenterOfViewport)
        //         //     {
        //         //         _distanceFromCenterOfViewport = _screenDistance;
        //         //         selectedActor = _localPotentialTarget;
        //         //     }
        //         // }
        //     
        //         float _currentDist = Vector3.Distance ( transform.position, _localPotentialTarget.transform.position );
        //     
        //         //현재 선택된 타겟으로 가장 먼저 확인된 잠재적인 대상을 확인한다
        //         if (_i == 0)
        //         {
        //             _distanceFromCenterOfPlayer = _currentDist;
        //             selectedTarget = _localPotentialTarget;
        //         }
        //         else
        //         {
        //             if (_currentDist < _distanceFromCenterOfPlayer)
        //             {
        //                 _distanceFromCenterOfPlayer = _currentDist;
        //                 selectedTarget = _localPotentialTarget;
        //             }
        //         }
        //     
        //         if (selectedTarget != null)
        //         {
        //             EnableCameraLook();
        //             onTargetChanged(selectedTarget);
        //         }
        //     }
        // }
        //
        // //뷰포트안에 타겟이 존재하는가?
        // private bool IsInViewPort(MonsterBase _potentialTarget)
        // {
        //     Vector3 _viewPos = mainCamera.WorldToViewportPoint(_potentialTarget.transform.position);
        //     bool  _isInViewPort =_viewPos.x >= 0 && _viewPos.x <= 1 && _viewPos.y >= 0 && _viewPos.y <= 1 && _viewPos.z > 0;
        //     return _isInViewPort;
        // }
        //
        // private bool IsAnythingBlockingTrace(GameObject _target, List<GameObject> _objectToIgnores)
        // {
        //     RaycastHit _hitData;
        //     
        //     Ray _ray = new Ray(GetRayOrigin(), _target.transform.position - transform.position);
        //     bool _isHit = Physics.Raycast( _ray,out _hitData,targetingMaxDistance,obstacleLayerMask);
        //
        //     if (_isHit)
        //     {
        //         //! (허용되지 않는 유형들에 포함되어 있지 않고 (그리고) 제외해야할 오브젝트에 포함되어 있지 않다면 = true)
        //         return !_objectToIgnores.Contains(_hitData.transform.gameObject);
        //     }
        //
        //     //막히는게없다
        //     return false;
        // }
        //
        // Vector3 GetRayOrigin()
        // {
        //     Transform _transform = transform;
        //     return _transform.position + (_transform.forward * traceDepthOffset) + (_transform.up * traceHeightOffset);
        // }
        //
        // void OnDrawGizmos ( )
        // {
        //     if (selectedTarget!= null)
        //     {
        //         RaycastHit _hit;
        //     
        //         Ray _ray = new Ray(GetRayOrigin(), selectedTarget.transform.position - transform.position);
        //         // bool _isHit = Physics.Raycast( _ray,out _hit,targetingMaxDistance,defaultLayerMask);
        //         // bool _isHit = Physics.Raycast( _ray,out _hit,targetingMaxDistance,defaultLayerMask);
        //         bool _isHit = Physics.Raycast ( GetRayOrigin(), selectedTarget.transform.position - transform.position, out _hit, targetingMaxDistance, obstacleLayerMask );
        //
        //         if ( _isHit && _hit.transform.CompareTag ( "Player" ) )
        //         {
        //             Gizmos.color = Color.green;
        //         }
        //         else
        //         {
        //             Gizmos.color = Color.red;
        //         }
        //
        //         // Ray ray = new Ray(GetRayOrigin(), selectedActor.transform.position - transform.position*targetingMaxDistance);
        //         Gizmos.DrawRay (GetRayOrigin(), selectedTarget.transform.position - transform.position );
        //     }
        //
        // }
        //
        // #endregion Functions
   
    }
}