using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using ExitGames.Client.Photon;

public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable
{

    #region Public Fields 

    [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
    public static GameObject LocalPlayerInstance;
    public event Action<int> OnPlayerDeath = delegate { Debug.Log("Nothing assigned to OnPlayerDeath"); };

    #endregion

    #region Private Fields

    [Header("Player Properties")]
    [SerializeField] private float health = 100.0f;


    [Header("Body Parts")]
    [SerializeField] public Transform leftHandTransform;
    [SerializeField] public Transform rightHandTransform;
    [SerializeField] public Transform headTransform;

    [Header("Class Instance")]
    [SerializeField] private Inventory inventory;

    private Transform mainCamera;
    private InputMaster inputMaster;
    private Animator animator;

    #endregion

    #region Getter/Setter
    private bool isDead = false;

    public bool IsDead { get => isDead; }
    public float Health { get => health; }
    #endregion






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
                
                //set LocalPlayer layer to all GameObject
                SetLayerRecursively(this.gameObject, 9);

            }
        }
        

        inputMaster = new InputMaster();
        inputMaster.Player.Shoot.performed += ctx => { GetComponent<Inventory>().UseSelectedItem(); };
        inputMaster.Player.UseItem.performed += ctx => { GetComponent<Ragdoll>().photonView.RPC("TurnRagdollOn", RpcTarget.All); };
        inputMaster.Player.NextItem.performed += ctx =>
        {
            //changement d'arme
            inventory.photonView.RPC("NextObject", RpcTarget.All);
        };
        inputMaster.Player.PreviousItem.performed += ctx =>
        {
            //changement d'arme
            inventory.photonView.RPC("PreviousObject", RpcTarget.All);
        };

    }

    

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main.transform;
        animator = GetComponent<Animator>();


        //si c'est le joueur local on affiche son HUD
        if (LocalPlayerInstance == this.gameObject)
        {
            HUDController.SetPlayerToShowHUD(this);

            //indiquer que le joueur est en vie
            Hashtable props = new Hashtable();
            props.Add(SlideRaceGame.PLAYER_IS_ALIVE, true);

            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
        photonView.RPC("RemoveLife", RpcTarget.Others, quantity);
        
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
        IsAlive();

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
            isDead = true;

            if(GetComponent<Ragdoll>() != null)
            {
                GetComponent<Ragdoll>().photonView.RPC("TurnRagdollOn", RpcTarget.AllBuffered);

            }
            if (photonView.IsMine)
            {
                photonView.RPC("RPC_KillPlayer", RpcTarget.OthersBuffered);
            }
            
            OnPlayerDeath(photonView.OwnerActorNr); //executer l'évenement
        }
        else isDead = false;
    }


    public override void OnEnable()
    {
        if (!photonView.IsMine && PhotonNetwork.IsConnected == true)
        {
            return;
        }
        inputMaster.Enable();
    }

    public override void OnDisable()
    {
        if (!photonView.IsMine && PhotonNetwork.IsConnected == true)
        {
            return;
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

    [PunRPC]
    public void RPC_KillPlayer()
    {
        this.health = 0;
        IsAlive();
    }

    private void SetLayerRecursively(GameObject obj, int newLayer)
    {
        if (null == obj)
        {
            return;
        }

        obj.layer = newLayer;

        foreach (Transform child in obj.transform)
        {
            if (null == child)
            {
                continue;
            }
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }
}
