using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knife : PickableItem
{
    [SerializeField] private AnimatorOverrideController animatorOverrideController;

    [SerializeField] private float hitDistance = 2f;
    [SerializeField] private float damage = 25f;

    private Transform cameraView;
    public override void Use(Transform viewTransform)
    {
        //RAYCAST d'une certaine distance
        RaycastHit hit;
        Ray ray = new Ray(viewTransform.position, viewTransform.forward);
        Physics.Raycast(ray, out hit, hitDistance);

        //récupérer l'objet touché et vérifier s'il s'agit d'un joueur
        if (hit.collider != null)
        {
            PlayerController player = hit.collider.gameObject.GetComponent<PlayerController>();
            if (player != null)
            {
                //lui enlever de la vie
                player.SendRemoveLife(damage);
            }
        }
        
        cameraView = viewTransform;

    }

    private void Update()
    {
        if(cameraView != null)
        {
            Debug.DrawRay(cameraView.position, cameraView.forward * hitDistance, Color.green);
        }
        
    }

    public override void OnStoredInInventory()
    {
        //mettre le couteau dans la bonne position pour le joueur
        this.transform.localPosition = new Vector3(0f, 0.01f, 0.02f);
        this.transform.localRotation = Quaternion.Euler(-358.4f, 186.7f, -91.6f);
    }
}
