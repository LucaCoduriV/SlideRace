using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable
{

    #region Public Fields 

    [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
    public static GameObject LocalPlayerInstance;

    #endregion

    #region Private Fields

    [SerializeField] private float health = 100.0f;
    [SerializeField] public List<GameObject> inventory;
    [SerializeField] public List<int> inventory_remade;
    [SerializeField] private int selectedObject;
    [SerializeField] private Transform leftHandTransform;
    [SerializeField] private float cutDistance = 2;
    [SerializeField] private float cutDamage = 25;

    private Transform mainCamera;
    private InputMaster inputMaster;
    private bool isDead = false;
    private Animator animator;


    #endregion

    public bool IsDead { get => isDead; }
    public float Health { get => health;}


    

    void Awake()
    {

        if (!photonView.IsMine && PhotonNetwork.IsConnected)
        {
            return;
        }

        if (photonView.IsMine || !PhotonNetwork.IsConnected)
        {
            if(PlayerController.LocalPlayerInstance != null)
            {
                Destroy(this.gameObject);
            }
            else
            {
                PlayerController.LocalPlayerInstance = this.gameObject;

            }
        }
        

        inputMaster = new InputMaster();
        inputMaster.Player.Shoot.performed += ctx => Cut();
        inputMaster.Player.UseItem.performed += ctx => UseSelectedItem();
        inputMaster.Player.NextItem.performed += ctx =>
        {
            photonView.RPC("NextObject", RpcTarget.All);
        };
        inputMaster.Player.PreviousItem.performed += ctx =>
        {
            photonView.RPC("PreviousObject", RpcTarget.All);
        };

    }

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main.transform;
        animator = GetComponent<Animator>();
        inventory = new List<GameObject>();
        selectedObject = 0;

        //si c'est le joueur local on affiche son HUD
        if(LocalPlayerInstance == this.gameObject)
        {
            HUDController.SetPlayerToShowHUD(this);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(mainCamera.position + mainCamera.forward, mainCamera.forward * cutDistance);

        if(leftHandTransform != null)
        {
            //ShowSelectedItem();
        }
        else
        {
            Debug.LogError("Unable to pickup object without a hand");
        }
        
        
    }
    

    public void SetLife(float life)
    {
        this.health = life;
        IsAlive();

        if (HUDController.GetPlayerToShowHUD() == this)
        {
            //updateHUD
            HUDController.UpdateHUD();
        }
    }

    public void SendRemoveLife(float quantity)
    {
        photonView.RPC("RemoveLife", RpcTarget.Others, cutDamage);
        
    }

    [PunRPC]
    public void RemoveLife(float quantity)
    {
        if ((this.health - quantity) < 0)
        {
            this.health = 0;
        }
        else
        {
            this.health -= quantity;
        }

        IsAlive();

        if(HUDController.GetPlayerToShowHUD() == this)
        {
            //updateHUD
            HUDController.UpdateHUD();
        }
        

    }

    public void Kill()
    {
        this.health = 0;
        this.isDead = true;

        if (HUDController.GetPlayerToShowHUD() == this)
        {
            //updateHUD
            HUDController.UpdateHUD();
        }
    }

    private void IsAlive()
    {
        if (health <= 0)
        {
            //cacher le couteau
            isDead = true;
        }
        else isDead = false;
    }


    private void ShowSelectedItem()
    {
        if(inventory.Count != 0)
        {
            inventory[selectedObject].SetActive(true);
            inventory[selectedObject].transform.position = leftHandTransform.position;
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

        if(inventory_remade.Count == 0)
        {
            //aucun objet dans l'inventaire

            return;
        }
        else if(!(selectedObject >= 0 && selectedObject < inventory_remade.Count))
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
        for (int i = 0; i < inventory.Count; i++)
        {
            inventory[i].SetActive(false);
        }
    }

    private void UseSelectedItem()
    {
        
        //vérifier que l'on a bien un objet à utiliser
        if(inventory_remade.Count > 0 && inventory_remade[selectedObject] != null)
        {
            PhotonView.Find(inventory_remade[selectedObject]).GetComponent<IPickableItem>().Use(mainCamera);
            inventory_remade.RemoveAt(selectedObject);

            //selectionner un autre objet dans l'inventaire
            photonView.RPC("PreviousObject", RpcTarget.All);
        }
        else
        {
            Debug.LogWarning("No item in inventory !!");
        }
        
    }

    private void Cut()
    {
        if (!this.animator.GetCurrentAnimatorStateInfo(0).IsName("Cut") && !this.animator.IsInTransition(0) && !isDead)
        {
            animator.SetTrigger("Shoot1");

            RaycastHit hit;

            if(Physics.Raycast(mainCamera.position + mainCamera.forward.normalized, mainCamera.forward.normalized, out hit, cutDistance))
            {
                Debug.Log("TOUCHE!! " + hit.transform.gameObject.name);
                if (hit.transform.CompareTag("Player"))
                {
                    hit.transform.GetComponent<PlayerController>().SendRemoveLife(cutDamage);
                    
                }
            }
            else
            {
                Debug.Log("RATE!!!");
            }
        }
        
    }

    public void PickupObject(GameObject objectToPickUp)
    {
        //checker s'il y a la place pour rammasser l'objet

        
        //modifier le owner de l'objet
        if (photonView.IsMine)
        {
            objectToPickUp.GetComponent<IPickableItem>().ChangeOwner();
        }
        //Executer la routine d'un object ramassé
        objectToPickUp.GetComponent<IPickableItem>().OnPickup();
        //puis l'ajouter dans l'inventaire sur tout les clients
        photonView.RPC("StoreItemInInventory", RpcTarget.All, objectToPickUp.GetPhotonView().ViewID);

        //le déplacer dans la main gauche
        objectToPickUp.transform.position = leftHandTransform.position;
        objectToPickUp.transform.parent = leftHandTransform;

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
        try
        { 
            PhotonView.Find(inventory_remade[id]).gameObject.SetActive(true);
        }
        catch
        {
            Debug.Log("Can not select this id");
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag.Contains("Pickable"))
        {
            PickupObject(other.gameObject);
        }
        
    }

    public void OnEnable()
    {
        if (!photonView.IsMine && PhotonNetwork.IsConnected == true)
        {
            return;
        }
        inputMaster.Enable();
    }

    public void OnDisable()
    {
        if (!photonView.IsMine && PhotonNetwork.IsConnected == true)
        {
            return;
        }
        if(this == PlayerController.LocalPlayerInstance)
        {
            PlayerController.LocalPlayerInstance = null;
        }

        inputMaster.Disable();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // We own this player: send the others our data
            stream.SendNext(health);
        }
        else
        {
            // Network player, receive data
            this.health = (float)stream.ReceiveNext();
        }
    }
}
