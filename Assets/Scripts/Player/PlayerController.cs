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
    public event Action<int> OnPlayerDeath;

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
    private InputMaster myInputMaster;
    private Animator animator;

    #endregion

    #region Getter/Setter
    private bool isDead = false;

    public bool IsDead { get => isDead; }
    public float Health { get => health; }
    #endregion






    void Awake()
    {
        

    }

    public void Instantiate()
    {
        myInputMaster = new InputMaster();
        myInputMaster.Player.Shoot.performed += ctx => { GetComponent<Inventory>().UseSelectedItem(); };
        myInputMaster.Player.UseItem.performed += ctx => { GetComponent<Ragdoll>().photonView.RPC("TurnRagdollOn", RpcTarget.All); };
        myInputMaster.Player.NextItem.performed += ctx =>
        {
            //changement d'arme
            inventory.photonView.RPC("NextObject", RpcTarget.All);
        };
        myInputMaster.Player.PreviousItem.performed += ctx =>
        {
            //changement d'arme
            inventory.photonView.RPC("PreviousObject", RpcTarget.All);
        };

        myInputMaster.Player.Shoot.Enable();
        myInputMaster.Player.UseItem.Enable();
        myInputMaster.Player.NextItem.Enable();
        myInputMaster.Player.PreviousItem.Enable();

        GetComponent<PlayerNamePlate>().Instantiate();
    }

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main.transform;
        animator = GetComponent<Animator>();        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    
    public void SetLife(float life)
    {
        this.health = life;
        photonView.RPC("IsAlive", RpcTarget.All);

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
        photonView.RPC("IsAlive", RpcTarget.All);

        if (HUDController.GetPlayerToShowHUD() == this)
        {
            //updateHUD
            HUDController.UpdateHUD();
        }
    }

    [PunRPC]
    public void IsAlive()
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

            Ch.Luca.MyGame.GameManager.instance.OnPlayerDeath(photonView.OwnerActorNr);
        }
        else isDead = false;
    }


    public override void OnEnable()
    {
        if (photonView.IsMine && PhotonNetwork.IsConnected == true)
        {
            
        }
        
    }

    public override void OnDisable()
    {
        if (photonView.IsMine && PhotonNetwork.IsConnected == true)
        {
            //myInputMaster.Player.Shoot.Disable();
            //myInputMaster.Player.UseItem.Disable();
            //myInputMaster.Player.NextItem.Disable();
            //myInputMaster.Player.PreviousItem.Disable();
        }
        OnPlayerDeath = null;
        LocalPlayerInstance = null;
        
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

    [PunRPC]
    public void TakeControl()
    {
        if (photonView.IsMine)
        {
            LocalPlayerInstance = this.gameObject;
            SetLayerRecursively(this.gameObject, 9);
            FindObjectOfType<CameraManager>().FollowLocalPlayer();

            HUDController.SetPlayerToShowHUD(this);

            //indiquer que le joueur est en vie
            Hashtable props = new Hashtable();
            props.Add(SlideRaceGame.PLAYER_IS_ALIVE, true);

            PhotonNetwork.LocalPlayer.SetCustomProperties(props);

            GetComponent<CharacterControls>().Instantiation();
            Instantiate();

        }
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
