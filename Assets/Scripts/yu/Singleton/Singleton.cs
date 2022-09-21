using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton : MonoBehaviour
{
    private static Singleton inst = null;
    void Awake()
    {
        if (null == inst)
        {
            //싱글톤
            inst = this;
            DontDestroyOnLoad(this.gameObject);
            //씬이 넘어가도 삭제 안되게 설정
        }
        else
        {
            Destroy(this.gameObject);
            //중복 방지용
        }
    }
}
