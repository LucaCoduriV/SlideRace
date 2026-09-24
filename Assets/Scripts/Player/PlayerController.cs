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

    public const int NO_KILLER = -1;

    #endregion

    #region Events
         //public event Action<int> OnPlayerDeath;
         public event Action<object, object> OnPlayerDeath; //sender & killer
    #endregion


    #region Private Fields

    [Header("Player Properties")]
    [SerializeField] private float health = 100.0f;
    [Tooltip("Durée (s) pendant laquelle une mort dans le décor est attribuée au dernier attaquant")]
    [SerializeField] private float killCreditWindow = 5f;


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
    private int lastAttackerActorNr = NO_KILLER;
    private float lastAttackTime = float.NegativeInfinity;

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
        photonView.RPC("RPC_SetHealth", RpcTarget.All, hp);
    }

    [PunRPC]
    public void RPC_SetHealth(float hp)
    {
        health = hp;
    }

    
    public void SetLife(float life)
    {
        this.health = life;

        if (photonView.IsMine && health <= 0)
        {
            Die(NO_KILLER);
        }

        if (HUDController.GetPlayerToShowHUD() == this)
        {
            //updateHUD
            HUDController.UpdateHUD();
        }
    }

    //ancien point d'entrée utilisé par le couteau, le joueur local est considéré comme l'attaquant
    public void SendRemoveLife(float quantity)
    {
        ApplyDamage(quantity, PhotonNetwork.LocalPlayer.ActorNumber);
    }

    //Inflige des dégâts à ce joueur. Seul le propriétaire du joueur applique réellement les dégâts
    //pour que la mort ne soit traitée qu'une seule fois.
    public void ApplyDamage(float quantity, int attackerActorNr)
    {
        if (isDead)
            return;

        if (photonView.IsMine)
        {
            RemoveLife(quantity, attackerActorNr);
        }
        else if (photonView.Owner != null)
        {
            photonView.RPC("RemoveLife", photonView.Owner, quantity, attackerActorNr);
        }
    }

    [PunRPC]
    public void RemoveLife(float quantity, int attackerActorNr)
    {
        if (!photonView.IsMine || isDead)
            return;

        this.health = Mathf.Max(0f, this.health - quantity);

        //se souvenir de qui nous a touché pour lui attribuer une mort dans le décor
        if (attackerActorNr != NO_KILLER && attackerActorNr != photonView.OwnerActorNr)
        {
            lastAttackerActorNr = attackerActorNr;
            lastAttackTime = Time.time;
        }

        if (health <= 0)
        {
            Die(attackerActorNr);
        }

        if(HUDController.GetPlayerToShowHUD() == this)
        {
            //updateHUD
            HUDController.UpdateHUD();
        }
    }

    //Mort causée par le décor (DeadlyObject). Chaque client détecte la collision,
    //mais seul le propriétaire décide de la mort.
    public void Kill()
    {
        if (!photonView.IsMine || isDead)
            return;

        //si quelqu'un nous a touché récemment, c'est lui qui a fait le kill
        int killer = (Time.time - lastAttackTime <= killCreditWindow) ? lastAttackerActorNr : NO_KILLER;
        Die(killer);
    }

    public bool IsAlive()
    {
        return !isDead;
    }

    //Exécuté uniquement par le propriétaire du joueur
    private void Die(int killerActorNr)
    {
        if (isDead)
            return;

        this.health = 0;

        //exécuté immédiatement en local puis chez les autres (buffered pour ceux qui rejoignent)
        photonView.RPC("RPC_Die", RpcTarget.AllBuffered, killerActorNr);

        IncrementLocalPlayerProperty(SlideRaceGame.PLAYER_DEATH_COUNTER);

        //chaque joueur met à jour ses propres compteurs, on demande donc au tueur de s'ajouter un kill
        if (killerActorNr != NO_KILLER && killerActorNr != photonView.OwnerActorNr)
        {
            Player killer = PhotonNetwork.CurrentRoom?.GetPlayer(killerActorNr);
            if (killer != null)
            {
                photonView.RPC("RPC_CreditKill", killer);
            }
        }
    }

    [PunRPC]
    private void RPC_Die(int killerActorNr)
    {
        if (isDead)
            return;

        isDead = true;
        this.health = 0;

        Ragdoll ragdoll = GetComponent<Ragdoll>();
        if (ragdoll != null)
        {
            ragdoll.TurnRagdollOn();
        }

        if (HUDController.GetPlayerToShowHUD() == this)
        {
            //updateHUD
            HUDController.UpdateHUD();
        }

        CallPlayerDeathEvent(killerActorNr);
    }

    [PunRPC]
    private void RPC_CreditKill()
    {
        IncrementLocalPlayerProperty(SlideRaceGame.PLAYER_KILL_COUNTER);
    }

    //Photon ne met à jour le cache local qu'après la réponse du serveur : on garde la dernière valeur envoyée
    //pour ne pas perdre d'incrément si deux arrivent coup sur coup (ex: 2 kills avec une grenade)
    private static readonly Dictionary<string, int> lastSentCounters = new Dictionary<string, int>();

    public static void IncrementLocalPlayerProperty(string key)
    {
        int value = 0;
        object current;
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(key, out current))
        {
            value = (int)current;
        }

        int lastSent;
        if (lastSentCounters.TryGetValue(key, out lastSent))
        {
            value = Mathf.Max(value, lastSent);
        }

        value++;
        lastSentCounters[key] = value;

        Hashtable props = new Hashtable() { { key, value } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
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
