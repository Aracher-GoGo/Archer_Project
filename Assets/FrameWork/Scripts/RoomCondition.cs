using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace ArcheryDemo
{
    public class RoomCondition : MonoBehaviour
    {
    
        //Room에 존재하는 몬스터들
        private readonly List<GameObject> monsterListInRoom = new List<GameObject>();

        [Tooltip("플레이어가 입장했는가?")]
        public bool playerInThisRoom;
    
        [Tooltip("룸이 클리어 됬는가?")]
        public bool isClearRoom;
    
    
        void Update()
        {
            if (playerInThisRoom)
            {
                //몬스터가 0보다 작거나 같고 현재 클리어 상태가 아니라면
                if (monsterListInRoom.Count <= 00 && !isClearRoom)
                {
                    isClearRoom = true;
                    Debug.Log("Clear");
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                playerInThisRoom = true;
                PlayerTargeting.Instance.MonsterList = new List<GameObject>(monsterListInRoom);
                // Debug.Log("Enter New Room! Mob count : " +PlayerTargeting.Instance.MonsterList.Count);
            }

            if (other.CompareTag("Monster"))
            {
                monsterListInRoom.Add(other.gameObject);
                // Debug.Log("Mob Name : "+other.gameObject.name);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                playerInThisRoom = false;
                Debug.Log("Player Exit !!");
            }

            if (other.CompareTag("Monster"))
            {
                monsterListInRoom.Remove(other.gameObject);
            }
        }
    }
}