using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyControll : MonoBehaviour
{
    public GameObject enemyCanvasGo;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Debug.Log("충돌중임 : "+collision.gameObject.name);
        if (collision.transform.CompareTag("Bullet"))
        {
            enemyCanvasGo.GetComponent<EnemyHpBar>().Dmg();
        }
    }
}
