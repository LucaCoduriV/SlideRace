using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickableItem : MonoBehaviourPun
{
    public bool readyToUse = false;

    public virtual void Use(Transform viewTransform)
    {
        Debug.Log("No action defined for this object");
    }

    public virtual void OnPickup()
    {
        Debug.LogWarning("OBJECT WAS PICKED UP");
        if (PhotonNetwork.IsConnected)
        {
            photonView.RPC("OnPickupCallback", RpcTarget.All);
        }
        else
        {
            OnPickupCallback();
        }
    }

    public virtual void OnStoredInInventory()
    {
        Debug.LogWarning("OBJECT WAS PICKED UP");
        if (PhotonNetwork.IsConnected)
        {
            photonView.RPC("OnStoredInInventoryCallback", RpcTarget.All);
        }
        else
        {
            OnStoredInInventoryCallback();
        }
    }

    public virtual void ChangeOwner()
    {
        this.photonView.RequestOwnership();
        Debug.Log("OwnerShip taken");
    }

    public virtual void Drop()
    {
        Debug.Log("Default drop action");
    }

    [PunRPC]
    public virtual void OnPickupCallback()
    {
        if(this.GetComponent<Collider>() != null)
        {
            this.GetComponent<Collider>().enabled = false;
        }
        
        this.gameObject.SetActive(false);
    }

    [PunRPC]
    public virtual void OnStoredInInventoryCallback()
    {
        Debug.Log("I am executed after i got stored in inventory");
    }
}
