using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterManager : MonoSingleton<MonsterManager>
{

    // public Dictionary<GameObject, MonsterBase> Monsters
    // {
    //     get { return monsters; }
    // }

    // private Dictionary<GameObject, MonsterBase> monsters = new Dictionary<GameObject, MonsterBase>();


    public List<MonsterBase> MonsterBases
    {
        get { return monsterBases; }
    }
    
    private List<MonsterBase> monsterBases = new List<MonsterBase>();



    void Start()
    {
        // GameObject[] _monsterObjects = GameObject.FindGameObjectsWithTag("Monster");
        //
        // foreach (GameObject _monsterObject in _monsterObjects)
        // {
        //     MonsterBase _monsterBase = _monsterObject.GetComponent<MonsterBase>();
        //
        //     if (_monsterBase != null)
        //     {
        //         monsters.Add(_monsterObject,_monsterBase);
        //     }
        // }

        var _monsters  =GameObject.FindObjectsOfType<MonsterBase>();
        foreach (MonsterBase _monsterBase in _monsters)
        {
            if (_monsterBase != null)
            {
                monsterBases.Add(_monsterBase);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}