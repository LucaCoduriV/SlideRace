using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knife : PickableItem
{
    [SerializeField] private AnimatorOverrideController animatorOverrideController;

    [SerializeField] private float hitDistance = 2f;
    [SerializeField] private float damage = 25f;
    [Tooltip("Rayon du coup, permet de toucher sans viser au pixel près")]
    [SerializeField] private float hitRadius = 0.3f;
    [Tooltip("Temps minimum (s) entre deux coups de couteau")]
    [SerializeField] private float attackCooldown = 0.6f;

    private float nextAttackTime = 0f;

    public bool CanAttack { get => Time.time >= nextAttackTime; }

    private Transform cameraView;

    public void StartCooldown()
    {
        nextAttackTime = Time.time + attackCooldown;
    }

    public override void Use(Transform viewTransform)
    {
        cameraView = viewTransform;

        //le joueur qui tient le couteau (pour ne pas se toucher soi-même)
        PlayerController owner = GetComponentInParent<PlayerController>();

        Ray ray = new Ray(viewTransform.position, viewTransform.forward);
        RaycastHit[] hits = Physics.SphereCastAll(ray, hitRadius, hitDistance, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);
        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        foreach (RaycastHit hit in hits)
        {
            PlayerController player = hit.collider.GetComponentInParent<PlayerController>();

            //ignorer notre propre corps
            if (player != null && player == owner)
                continue;

            //récupérer l'objet touché et vérifier s'il s'agit d'un joueur
            if (player != null && !player.IsDead)
            {
                //lui enlever de la vie
                player.ApplyDamage(damage, PhotonNetwork.LocalPlayer.ActorNumber);
            }

            //le premier obstacle arrête le coup (on ne frappe pas à travers les murs)
            break;
        }
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
