using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectSet : MonoBehaviour
{
    public static EffectSet Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<EffectSet>();
                if (instance == null)
                {
                    var instanceContainer = new GameObject("EffectSet");
                    instance = instanceContainer.AddComponent<EffectSet>();
                }
            }

            return instance;
        }
    }

    private static EffectSet instance;


    [Header("Monster")]
    public GameObject DuckAtkEffect;
    public GameObject DuckDmgEffect;
    
    [Header("Player")]
    public GameObject PlayerAtkEffect;
    public GameObject PlayerDmgEffect;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}