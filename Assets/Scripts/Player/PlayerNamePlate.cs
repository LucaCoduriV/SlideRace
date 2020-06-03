﻿using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerNamePlate : MonoBehaviourPun
{
    [SerializeField] TextMeshPro namePlateText;
    [SerializeField] Transform namePlate;

    Transform cameraTransform;

    private void Start()
    {
        cameraTransform = Camera.main.transform;

        namePlateText.text = photonView.Owner.NickName;

        if (photonView.IsMine)
        {
            namePlateText.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (!photonView.IsMine)
        {
            namePlate.LookAt(cameraTransform.position);
            
        }
    }
}
