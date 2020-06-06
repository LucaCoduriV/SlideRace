using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviourPunCallbacks
{
    [SerializeField] public List<int> inventory_remade;
    [SerializeField] private int selectedObject;

    private Transform rightHandTransform;
    private Transform leftHandTransform;
    private Transform headTransform;

    private PhotonView contextObject;
    private Transform mainCamera;
    private Animator animator;
    [Header("Animator animations")]
    [SerializeField] private AnimatorOverrideController animatorKnife;
    [SerializeField] private RuntimeAnimatorController animatorDefault;

    // Start is called before the first frame update
    void Start()
    {
        if (photonView.IsMine)
        {
            leftHandTransform = PlayerController.LocalPlayerInstance.GetComponent<PlayerController>().leftHandTransform;
            rightHandTransform = PlayerController.LocalPlayerInstance.GetComponent<PlayerController>().rightHandTransform;
            headTransform = PlayerController.LocalPlayerInstance.GetComponent<PlayerController>().headTransform;
        }
        

        mainCamera = Camera.main.transform;

        animator = GetComponent<Animator>();

        inventory_remade = new List<int>();
        selectedObject = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void ShowSelectedItem()
    {
        if (inventory_remade.Count != 0)
        {
            PhotonView.Find(inventory_remade[selectedObject]).gameObject.SetActive(true);
            PhotonView.Find(inventory_remade[selectedObject]).gameObject.transform.position = rightHandTransform.position;
        }

    }

    public void SelectObjectID(int id)
    {
        selectedObject = id;
    }

    [PunRPC]
    public void NextObject()
    {
        disableAllItemsInventory();

        selectedObject++;

        if (inventory_remade.Count == 0)
        {
            //aucun objet dans l'inventaire

            return;
        }
        else if (!(selectedObject >= 0 && selectedObject < inventory_remade.Count))
        {
            //ça n'existe pas
            selectedObject = -1;
            NextObject();
        }
        else
        {
            SelectItem(selectedObject);
        }

    }

    [PunRPC]
    public void PreviousObject()
    {
        disableAllItemsInventory();

        selectedObject--;

        if (inventory_remade.Count == 0)
        {
            //aucun objet dans l'inventaire

            return;
        }
        else if (!(selectedObject >= 0 && selectedObject < inventory_remade.Count))
        {
            //ça n'existe pas
            selectedObject = inventory_remade.Count;
            PreviousObject();
        }
        else
        {
            SelectItem(selectedObject);
        }
    }

    private void disableAllItemsInventory()
    {
        for (int i = 0; i < inventory_remade.Count; i++)
        {

            PhotonView.Find(inventory_remade[i]).gameObject.SetActive(false);
        }
    }

    public void UseSelectedItem()
    {
        bool oneUse = true;

        //vérifier que l'on a bien un objet à utiliser
        if (inventory_remade.Count > 0 && inventory_remade[selectedObject] != null)
        {
            contextObject = PhotonView.Find(inventory_remade[selectedObject]);

            if (contextObject.GetComponent<IThrowableItem>() != null)
            {
                //executer animation de lancement
                animator.SetTrigger("Throw");
            }
            else if(contextObject.GetComponent<Knife>() != null && contextObject.GetComponent<Knife>().readyToUse)
            {
                //executer animation d'attaque elle change selon l'item qui se trouve en main
                contextObject.GetComponent<Knife>().readyToUse = false;
                animator.SetTrigger("Attack");
                oneUse = false;
            }
            else if (contextObject.GetComponent<PickableItem>() != null)
            {
                contextObject.GetComponent<PickableItem>().Use(mainCamera);
            }

            if (oneUse)
            {
                //retirer l'objet de l'inventaire
                inventory_remade.RemoveAt(selectedObject);

                //selectionner un autre objet dans l'inventaire
                photonView.RPC("PreviousObject", RpcTarget.All);
            }
            
        }
        else
        {
            Debug.LogWarning("No item in inventory !!");
        }

    }

    public void PickupObject(GameObject objectToPickUp)
    {
        //checker s'il y a la place pour rammasser l'objet


        //modifier le owner de l'objet
        if (photonView.IsMine)
        {
            objectToPickUp.GetComponent<PickableItem>().ChangeOwner();
        }
        //Executer la routine d'un object ramassé
        objectToPickUp.GetComponent<PickableItem>().OnPickup();
        //puis l'ajouter dans l'inventaire sur tout les clients
        photonView.RPC("StoreItemInInventory", RpcTarget.All, objectToPickUp.GetPhotonView().ViewID);

        //le déplacer dans la main gauche
        objectToPickUp.transform.position = rightHandTransform.position;
        objectToPickUp.transform.parent = rightHandTransform;

        objectToPickUp.GetComponent<PickableItem>().OnStoredInInventory();

        //Séléctionner l'objet ramasser automatiquement
        selectedObject = inventory_remade.Count - 1;
        photonView.RPC("SelectItem", RpcTarget.All, selectedObject);


    }


    [PunRPC]
    private void StoreItemInInventory(int id)
    {
        inventory_remade.Add(id);
    }

    [PunRPC]
    private void SelectItem(int id)
    {
        GameObject item = PhotonView.Find(inventory_remade[id]).gameObject;

        if(item != null)
        {
            disableAllItemsInventory();
            item.SetActive(true);
            ChangeAnimator(item);
        }
        else
        {
            Debug.LogError("Can not select this item");
        }

    }

    private void ChangeAnimator(GameObject item)
    {
        if(item.GetComponent<Knife>() != null)
        {
            animator.runtimeAnimatorController = animatorKnife;
        }
        else
        {
            animator.runtimeAnimatorController = animatorDefault;
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag.Contains("Pickable"))
        {
            PickupObject(other.gameObject);
        }

    }

    public void OnThrow()
    {
        contextObject.GetComponent<PickableItem>().Use(mainCamera);
    }

    public void OnAttack()
    {
        contextObject.GetComponent<PickableItem>().Use(mainCamera);
    }
}
