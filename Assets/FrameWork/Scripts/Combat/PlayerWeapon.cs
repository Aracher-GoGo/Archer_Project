using UnityEngine;

namespace ArcheryDemo
{
    public class PlayerWeapon : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            GetComponent<Rigidbody>().velocity = transform.forward * 20.0f;
        }

        private void OnTriggerEnter(Collider other)
        {
            // Debug.Log("Name : " +other.transform.name);
            if (other.transform.CompareTag("Wall") || other.transform.CompareTag("Monster"))
            {
                // Debug.Log("Name : " +other.transform.name);
                GetComponent<Rigidbody>().velocity = Vector3.zero;
                Destroy(gameObject,0.2f);
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            // Debug.Log("Name : " +collision.transform.name);
            if (collision.transform.CompareTag("Wall") || collision.transform.CompareTag("Monster"))
            {
                // Debug.Log("Name : " +collision.transform.name);
                GetComponent<Rigidbody>().velocity = Vector3.zero;
                Destroy(gameObject,0.2f);
            }  
        }

        // Update is called once per frame
        void Update()
        {
        }
    }
}
