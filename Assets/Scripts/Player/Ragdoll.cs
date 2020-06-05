using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ragdoll : MonoBehaviourPun
{
    public List<Collider> bodyParts;

    public bool isRagdoll = false;

    private void GetBodyParts()
    {
        Collider[] colliders = GetComponentsInChildren<Collider>();

        

        foreach (Collider collider in colliders)
        {
            

            if (collider.gameObject != this.gameObject)
            {
                Physics.IgnoreCollision(collider, GetComponent<Collider>());

                collider.attachedRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
                collider.attachedRigidbody.isKinematic = true;
                collider.isTrigger = true;
                collider.enabled = false;
                bodyParts.Add(collider);
            }
            
        }
    }

    [PunRPC]
    public void TurnRagdollOn()
    {
        GetComponent<Animator>().enabled = false;
        GetComponent<Rigidbody>().isKinematic = false;
        GetComponent<Rigidbody>().freezeRotation = false;
        
        //lié le squelette au corps
        FixedJoint joint = GetComponentsInChildren<Rigidbody>()[1].gameObject.AddComponent<FixedJoint>();
        joint.connectedBody = GetComponent<Rigidbody>();

        foreach (Collider collider in bodyParts)
        {

            collider.enabled = true;
            collider.attachedRigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
            collider.attachedRigidbody.isKinematic = false;
            collider.isTrigger = false;
            collider.attachedRigidbody.velocity = GetComponent<Rigidbody>().velocity;
            collider.material.staticFriction = 0;
            
        }

        isRagdoll = true;
        
    }

    private void Awake()
    {
        GetBodyParts();
    }

    //private IEnumerator Start()
    //{
    //    yield return new WaitForSeconds(5);
    //    GetComponent<Rigidbody>().AddForce(Vector3.up * 500);
    //    yield return new WaitForSeconds(0.3f);
    //    //GetComponent<PlayerController>().SetLife(0);
    //    TurnRagdollOn();
    //}



}
