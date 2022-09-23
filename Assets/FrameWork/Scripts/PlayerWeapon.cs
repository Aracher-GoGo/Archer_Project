using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    public float bulletSpeed = 20.0f;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Rigidbody>().velocity = transform.forward * bulletSpeed;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Debug.Log("Name :" +other.transform.name);

        if (other.transform.CompareTag("Wall") || other.transform.CompareTag("Monster"))
        {
            // Debug.Log("Name :" +other.transform.name);
            GetComponent<Rigidbody>().velocity =Vector3.zero;
            Destroy(gameObject,0.2f);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Wall") || collision.transform.CompareTag("Monster"))
        {
            // Debug.Log("Name :" +collision.transform.name);
            GetComponent<Rigidbody>().velocity =Vector3.zero;
            Destroy(gameObject,0.2f);
        }
    }
    
}
