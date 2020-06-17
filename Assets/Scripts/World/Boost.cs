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

    Dictionary<Collider, Collider> colliders = new Dictionary<Collider, Collider>();


    // Start is called before the first frame update
    void Start()
    {

        rotUp = Quaternion.AngleAxis(angleUp, Vector3.up);
        rotRight = Quaternion.AngleAxis(angleRight, Vector3.right);
    }

    

    // Update is called once per frame
    void FixedUpdate()
    {
        foreach (KeyValuePair<Collider, Collider> entry in colliders)
        {
            
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        colliders.Add(other, other);
    }

    public void OnTriggerExit(Collider other)
    {
        colliders.Remove(other);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player" && isActivated && other.GetComponent<PlayerController>().photonView.IsMine)
        {
            Rigidbody body = other.GetComponent<Rigidbody>();
            body.AddForce(rotRight * rotUp * Vector3.forward * (speed * Time.fixedDeltaTime), ForceMode.VelocityChange);
        }
    }
}
