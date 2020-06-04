using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : PickableItem, IThrowableItem
{

    public float maxDamage = 100;
    public float explosionRadius = 9;
    public float timeToExplode = 3;

    private Rigidbody body;
    private bool isActive = false;
    private bool hasExploded = false;
    private bool isExploding = false;
    [SerializeField] private float speed = 10;
    [SerializeField] private Collider grenCollider;
    [SerializeField] private GameObject explosionEffect;

    

    // Start is called before the first frame update
    void Start()
    {
        body = this.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(this.transform.position, Vector3.forward * explosionRadius);
    }


    public override void Use(Transform viewTransform)
    {

        Vector3 currentSpeed = this.transform.root.GetComponent<Rigidbody>().velocity;

        Debug.Log(currentSpeed);

        this.transform.parent = null;
        body.isKinematic = false;
        body.useGravity = true;
        body.velocity = currentSpeed;

        body.AddForce(viewTransform.forward * speed);

        isActive = true;
        grenCollider.isTrigger = false;
        grenCollider.enabled = true;
        StartCoroutine(ExplodeAfterSec(timeToExplode));
    }



    
    private IEnumerator ExplodeAfterSec(float duration)
    {
        Debug.Log("grenade thrown");
        yield return new WaitForSeconds(duration);


        photonView.RPC("Explode", RpcTarget.All);

        //Explode();

    }

    [PunRPC]
    private void Explode()
    {

        Instantiate(explosionEffect, transform.position, transform.rotation);

        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach(Collider player in colliders)
        {
            if (player.CompareTag("Player"))
            {
                float damage = CalculateDamageFromDistance(player.transform.position, this.transform.position);

                player.GetComponent<PlayerController>().RemoveLife(damage);
            }
        }
        
        PhotonNetwork.Destroy(gameObject);
        
        
    }

    private float CalculateDamageFromDistance(Vector3 PointA, Vector3 PointB)
    {

        float distance = Vector3.Distance(PointA, PointB);

        return maxDamage * 1 / Mathf.Sqrt(distance);
    }


    public override void OnPickup()
    {
        Debug.LogWarning("GRENADE WAS PICKED UP");
        if (PhotonNetwork.IsConnected)
        {
            photonView.RPC("OnPickupCallback", RpcTarget.All);
        }
        else
        {
            OnPickupCallback();
        }
        
    }

    [PunRPC]
    public override void OnPickupCallback()
    {
        Debug.Log("OUF C?EST BON !!");
        this.GetComponent<Collider>().enabled = false;
        this.gameObject.SetActive(false);
    }
}
