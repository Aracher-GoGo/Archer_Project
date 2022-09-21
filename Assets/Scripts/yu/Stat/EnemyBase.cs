using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBase : MonoBehaviour
{
    public float maxHp = 1000f;
    public float currentHp = 1000f;
    public float damage = 100f;

    public float playerRealizeRange = 10f;
    public float attackRange = 5f;
    public float attackCoolTime = 5f;
    public float attackCoolTimeCacl = 5f;
    public bool canAtk = true;

    public float moveSpeed = 2f;
    public GameObject Player;
    public NavMeshAgent nvAgent;
    public float distance;
    public GameObject parentRoom;

    public LayerMask layerMask;
    public bool CanAtkStateFun()
    {
        Vector3 targetDir = new Vector3(Player.transform.position.x - transform.position.x, 0f,
            Player.transform.position.z - transform.position.z);
        Physics.Raycast(new Vector3(transform.position.x, 0.5f, transform.position.z), targetDir, out RaycastHit hit,
            30f, layerMask);
        distance = Vector3.Distance(Player.transform.position, transform.position);
        if (hit.transform == null)
        {
            Debug.Log("hit.transform == null");
            return false;
        }

        if (hit.transform.CompareTag("Player") && distance <= attackRange)
        {
            return true;
        }
        
        return false;
        
    }
}
