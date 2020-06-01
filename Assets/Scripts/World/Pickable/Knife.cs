using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knife : MonoBehaviourPun, IPickableItem
{
    public void ChangeOwner()
    {
        throw new System.NotImplementedException();
    }

    public void OnPickup()
    {
        Debug.LogWarning("GRENADE WAS PICKED UP");
        photonView.RPC("PickUpRoutine", RpcTarget.All);
    }

    [PunRPC]
    private void PickUpRoutine()
    {
        this.gameObject.SetActive(false);
    }

    public void Use(Transform viewTransform)
    {
        throw new System.NotImplementedException();
    }

    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
