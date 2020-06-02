﻿using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knife : PickableItem
{
    [SerializeField] private AnimatorOverrideController animatorOverrideController;

    [SerializeField] private float hitDistance = 2f;
    [SerializeField] private float damage = 25f;

    public override void Use(Transform viewTransform)
    {
        Debug.DrawRay(viewTransform.position, viewTransform.forward * hitDistance, Color.green);

        //RAYCAST d'une certaine distzance
        RaycastHit hit;
        Ray ray = new Ray(viewTransform.position, viewTransform.forward);
        Physics.Raycast(ray, out hit, hitDistance);

        //récupérer l'objet touché et vérifier s'il s'agit d'un joueur
        if(hit.collider != null)
        {
            PlayerController player = hit.collider.gameObject.GetComponent<PlayerController>();
            if(player != null)
            {
                //lui enlever de la vie
                player.SendRemoveLife(damage);
            }
        }

        
    }

    private void Update()
    {
        
    }
}