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

    [SerializeField] private float life = 100.0f;
    [SerializeField] public List<GameObject> inventory;
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
    public float Life { get => life;}

    

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

    }

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main.transform;
        animator = GetComponent<Animator>();
        inventory = new List<GameObject>();
        selectedObject = 0;
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
        this.life = life;
    }

    public void SendRemoveLife(float quantity)
    {
        photonView.RPC("RemoveLife", RpcTarget.Others, cutDamage);
        
    }

    [PunRPC]
    public void RemoveLife(float quantity)
    {
        Debug.Log("removeLife executed !!", this);
        if ((this.life - quantity) < 0)
        {
            this.life = 0;
        }
        else
        {
            this.life -= quantity;
        }

    }

    public void Kill()
    {
        this.life = 0;
        this.isDead = true;
    }

    private void IsAlive()
    {
        if (life <= 0) isDead = true;
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

    public void NextObject()
    {
        disableAllItemsInventory();

        if (selectedObject == (inventory.Count - 1))
        {
            selectedObject = 0;
        }
        else
        {
            selectedObject++;
        }

    }
    public void PreviousObject()
    {
        disableAllItemsInventory();

        if (selectedObject == 0)
        {
            selectedObject = (inventory.Count - 1);
        }
        else
        {
            selectedObject--;
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
        

        if(inventory.Count > 0)
        {
            inventory[selectedObject].GetComponent<IPickableItem>().Use(mainCamera);
            inventory.RemoveAt(selectedObject);      
        }
        else
        {
            Debug.LogWarning("No item in inventory !!");
        }
        
    }

    private void Cut()
    {
        if (!this.animator.GetCurrentAnimatorStateInfo(0).IsName("Cut") && !this.animator.IsInTransition(0))
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

        //Executer la routine d'un object ramassé
        objectToPickUp.GetComponent<IPickableItem>().OnPickup();
        //puis l'ajouter dans l'inventaire
        inventory.Add(objectToPickUp);
        //le déplacer dans la main gauche
        objectToPickUp.transform.position = leftHandTransform.position;
        objectToPickUp.transform.parent = leftHandTransform;
        //Vérifier s'il y a un autre objet dans l'inventaire si non le séléctionner directement
        if(inventory.Count == 1)
        {
            objectToPickUp.gameObject.SetActive(true);
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
            stream.SendNext(life);
        }
        else
        {
            // Network player, receive data
            this.life = (float)stream.ReceiveNext();
        }
    }
}
