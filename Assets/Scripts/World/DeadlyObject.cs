using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadlyObject : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision other)
    {
        if(other.transform.tag == "Player" && !other.gameObject.GetComponent<PlayerController>().IsDead)
        {
            other.transform.GetComponent<PlayerController>().Kill();
        }
        

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Player" && !other.gameObject.GetComponent<PlayerController>().IsDead)
        {
            other.transform.GetComponent<PlayerController>().Kill();
        }

    }
}
