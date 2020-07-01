using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using ExitGames.Client.Photon;
using Ch.Luca.MyGame;

public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable
{

    #region Public Fields 

    [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
    public static GameObject LocalPlayerInstance;
    public bool wasInstantiated = false;

    public GameObject lastAttackingPlayer; //contient le dernier joueur a avoir attaqué

    #endregion

    #region Events
         //public event Action<int> OnPlayerDeath;
         public event Action<object, object> OnPlayerDeath; //sender & killer
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
    private PlayerKeyboardInput characterInput;

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
        GetComponent<PlayerNamePlate>().Instantiate();
        FindObjectOfType<SpectatorManager>().SpectatorModeDisable();
        characterInput = FindObjectOfType<PlayerKeyboardInput>();

        wasInstantiated = true;

        OnPlayerDeath += GameManager.instance.OnPlayerDeath;
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
        if (!isDead && wasInstantiated)
        {
            if (characterInput != null)
            {
                if (characterInput.IsShootPressed())
                    GetComponent<Inventory>().UseSelectedItem();

                if (characterInput.IsUsePressed())
                    GetComponent<Ragdoll>().photonView.RPC("TurnRagdollOn", RpcTarget.All);

                if (characterInput.IsNextItemPressed())
                    inventory.photonView.RPC("NextObject", RpcTarget.All);

                if (characterInput.IsPreviousItemPressed())
                    inventory.photonView.RPC("PreviousObject", RpcTarget.All);

            }
        }

        
        
    }
    
    public void SetHealth(float hp)
    {
        photonView.RPC("RPC_SetHealth", RpcTarget.All);
    }

    [PunRPC]
    public void RPC_SetHealth(float hp)
    {
        health = hp;
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

        if (!IsAlive())
        {
            CallPlayerDeathEvent(null);
        }

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
    public bool IsAlive()
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

                object death;
                if(PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(SlideRaceGame.PLAYER_DEATH_COUNTER, out death))
                {
                    Debug.Log("JE MET A JOUR LE NOMBRE DE MORT ! nombre: " + (int)death);
                    Hashtable props = new Hashtable() { { SlideRaceGame.PLAYER_DEATH_COUNTER, (int)death + 1 } };
                    PhotonNetwork.LocalPlayer.SetCustomProperties(props);
                }
                else
                {
                    Debug.Log("MARCHE PAS");
                    Hashtable props = new Hashtable() { { SlideRaceGame.PLAYER_DEATH_COUNTER, 1 } };
                    PhotonNetwork.LocalPlayer.SetCustomProperties(props);
                }

                
            }

            
        }
        else isDead = false;

        return isDead;
    }

    private void CallPlayerDeathEvent(object killer)
    {
        OnPlayerDeath?.Invoke(this, killer);
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

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {

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
