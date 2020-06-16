using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerNamePlate : MonoBehaviourPun
{
    [SerializeField] TextMeshPro namePlateText;
    [SerializeField] Transform namePlate;

    Transform cameraTransform;

    private void Awake()
    {
        namePlate.gameObject.SetActive(false);
    }

    public void Instantiate()
    {
        namePlate.gameObject.SetActive(true);
        cameraTransform = Camera.main.transform;
        if (PhotonNetwork.IsConnected)
        {
            namePlateText.text = photonView.Owner.NickName;
        }
        
    }

    private void Update()
    {
        if (!photonView.IsMine)
        {
            if(cameraTransform != null)
                namePlate.LookAt(cameraTransform.position);
            
        }
    }
}
