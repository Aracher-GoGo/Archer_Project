using System.Collections;
using System.Collections.Generic;
using ArcheryDemo;
using UnityEngine;

public class MonsterBase : MonoBehaviour
{
    private bool isAlive = true;
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool I_OnSelected()
    {
        // Debug.Log("타겟팅 된 UI를 보여줍니다");
        return false;
    }

    public bool I_OnDeselected()
    {
        // Debug.Log("타겟팅 된 UI를 숨깁니다");
        return false;
    }

    public bool I_IsTargetAble()
    {
        // if(isAlive)
        //     Debug.Log(transform.gameObject.name+"타겟팅이 가능합니다");
        // else
        //     Debug.Log(transform.gameObject.name+"타겟팅이 불가능합니다");
        
        return isAlive;
    }
}