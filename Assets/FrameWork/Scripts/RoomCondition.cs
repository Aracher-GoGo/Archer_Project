using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomCondition : MonoBehaviour
{
    private List<GameObject> MonsterListRoom = new List<GameObject>();

    public bool playerInThisRoom = false;
    public bool isClearRoom = false;
    
    void Start()
    {
        
    }
    
    void Update()
    {
        if (playerInThisRoom)
        {
            if (MonsterListRoom.Count <= 0 && !isClearRoom)
            {
                isClearRoom = true;
                Debug.Log("Clear 클리어 했습니다");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //플레이어가 방에 들어오면 이방의 몹리스트를 링크(복사) 시킨다
            playerInThisRoom = true;
            PlayerTargeting.Instance.MonsterList = new List<GameObject>(MonsterListRoom);
            // Debug.Log("Enter New Room! Mob count : "+PlayerTargeting.Instance.MonsterList.Count);
        }
      
        if (other.CompareTag("Monster"))
        {
            MonsterListRoom.Add(other.gameObject);
            // Debug.Log("Mob name : " +other.gameObject.name);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInThisRoom = false;
            Debug.Log("Player Exit 방을 떠났습니다");
        }
        if (other.CompareTag("Monster"))
        {
            MonsterListRoom.Remove(other.gameObject);
        }
    }
}