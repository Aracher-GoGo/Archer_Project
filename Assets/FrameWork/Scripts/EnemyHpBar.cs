using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHpBar : MonoBehaviour
{
    public Slider hpSlider;
    public Slider BackHpSlider;
    public bool backHpHit = false;

    public Transform enemy;

    public float maxHp = 1000.0f;

    public float currentHp = 1000.0f;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = enemy.position;
        hpSlider.value = Mathf.Lerp(hpSlider.value,currentHp / maxHp,Time.deltaTime*5f);

        if ( backHpHit )
        {
            BackHpSlider.value = Mathf.Lerp ( BackHpSlider.value, hpSlider.value, Time.deltaTime * 5f );
            if ( hpSlider.value >= BackHpSlider.value - 0.01f )
            {
                backHpHit = false;
                BackHpSlider.value = hpSlider.value;
            }
        }
    }

    public void Dmg()
    {
        currentHp -= 300.0f;
        Invoke("BackHpFun",0.5f);
    }

    void BackHpFun()
    {
        backHpHit = true;
    }
}