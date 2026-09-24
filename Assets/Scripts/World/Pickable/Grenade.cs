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


        //la position et le lanceur sont envoyés pour que tous les clients calculent la même explosion
        photonView.RPC("Explode", RpcTarget.All, transform.position, PhotonNetwork.LocalPlayer.ActorNumber);

    }

    [PunRPC]
    private void Explode(Vector3 position, int throwerActorNr)
    {
        if (hasExploded)
            return;
        hasExploded = true;

        Instantiate(explosionEffect, position, transform.rotation);

        Collider[] colliders = Physics.OverlapSphere(position, explosionRadius);
        HashSet<PlayerController> damagedPlayers = new HashSet<PlayerController>();

        foreach(Collider collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                PlayerController player = collider.GetComponent<PlayerController>();

                //un joueur peut avoir plusieurs colliders, et seul son propriétaire applique les dégâts
                if (player == null || !player.photonView.IsMine || !damagedPlayers.Add(player))
                    continue;

                float damage = CalculateDamageFromDistance(player.transform.position, position);
                player.ApplyDamage(damage, throwerActorNr);
            }
        }

        Destroy(gameObject);
        
        
    }

    //Dégâts maximum au centre, diminuent linéairement jusqu'à 0 au bord du rayon d'explosion
    private float CalculateDamageFromDistance(Vector3 PointA, Vector3 PointB)
    {

        float distance = Vector3.Distance(PointA, PointB);

        return maxDamage * Mathf.Clamp01(1f - distance / explosionRadius);
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
