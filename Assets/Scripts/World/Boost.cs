using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boost : MonoBehaviour
{
    public bool isActivated = true;

    public float speed = 10f;
    public float angleUp = 0f;
    public float angleRight = 0f;

    Quaternion rotUp, rotRight;




    // Start is called before the first frame update
    void Start()
    {
        rotUp = Quaternion.AngleAxis(angleUp, Vector3.up);
        rotRight = Quaternion.AngleAxis(angleRight, Vector3.right);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerStay(Collider other)
    {
        if(other.tag == "Player" && isActivated)
        {
            Rigidbody body = other.GetComponent<Rigidbody>();
            body.AddForce(rotRight * rotUp * Vector3.forward * (speed), ForceMode.Force);
        }
    }
}
