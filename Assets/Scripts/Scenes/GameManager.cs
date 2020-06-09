
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System.Collections.Generic;
using System;

namespace Ch.Luca.MyGame
{
    public class GameManager : MonoBehaviourPunCallbacks
    {
        #region Public Fields

        public static GameManager instance;
        public static List<GameObject> players = new List<GameObject>();
        public static GameObject localPhotonPlayer;
        public GameObject playerPrefab;

        public bool hasGameStarted = false;
        public List<Transform> spawnList;
        public int previousSpawn = -1;

        public double gameStartTime;
        public double remainingTime = 0;
        public double roundTime = 300;
        public double startCountDown = 5;

        public static event Action OnSpectateModeActivated;
        public static event Action OnSpectateModeDisabled;


        #endregion

        #region MonoBehaviour Methods

        private void Awake()
        {
            if (instance == null)
            {
                instance = this.GetComponent<GameManager>();
            }
            else
            {
                Destroy(this.gameObject);
            }

            SpawnController.OnLocalPlayerSpawn += OnLocalPlayerSpawn;
        }

        void Start()
        {
            Hashtable props = new Hashtable() { { SlideRaceGame.PLAYER_READY, true } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
            
            
        }

        

        void Update()
        {
            if (hasGameStarted)
            {
                UpdateTimer();
            }
            
        }

        public override void OnDisable()
        {
            instance = null;
            players = null;
            localPhotonPlayer = null;
        }

        #endregion


        #region Photon Callbacks

        public override void OnLeftRoom()
        {
            SceneManager.LoadScene(0);
        }

        #endregion

        #region Public Methods
            
        public void LeaveRoom()
        {
            OnSpectateModeActivated();
            PhotonNetwork.DestroyPlayerObjects(PhotonNetwork.LocalPlayer);
            PhotonNetwork.LeaveRoom();
        }

        public override void OnJoinedRoom()
        {
            OnSpectateModeActivated?.Invoke();
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            BeforeStart();
        }

        public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
        {
            
            object propsTime;
            if (propertiesThatChanged.TryGetValue(SlideRaceGame.GAME_START_TIME, out propsTime))
            {
                gameStartTime = (double)propsTime;
            }

            object propsStarted;
            if (propertiesThatChanged.TryGetValue(SlideRaceGame.HAS_GAME_STARTED, out propsStarted))
            {
                hasGameStarted = (bool)propsStarted;
            }
        }


        [PunRPC]
        public void RestartScene()
        {
            PhotonNetwork.LoadLevel(SceneManager.GetActiveScene().buildIndex);
        }



        #endregion

        #region Private Methods

        private void OnLocalPlayerSpawn()
        {
            Debug.Log("Player spawned");
            PlayerController.LocalPlayerInstance.GetComponent<PlayerController>().OnPlayerDeath += OnPlayerDeath;

            //focus la camera sur le joueur
            OnSpectateModeDisabled?.Invoke();
        }
        public void OnPlayerDeath(int ownerNb)
        {
            if(PhotonNetwork.LocalPlayer.ActorNumber == ownerNb) 
            {
                //activer le mode spectateur seulement si le joueur local meurt
                OnSpectateModeActivated?.Invoke();
            }

            if (PhotonNetwork.IsMasterClient)
            {
                //si il y a moins que 2 joueurs on redémarre la map
                if (GetNumberOfPlayerAlive() <= 1)
                {
                    //Faire les trucs de fin de partie
                    Debug.Log("Game Restarted");
                    photonView.RPC("RestartScene", RpcTarget.All);
                }
            }
        }

        private int GetNumberOfPlayerAlive()
        {
            int nbPlayerAlive = 0;

            foreach(var player in FindObjectsOfType<PlayerController>())
            {
                if (!player.IsDead)
                {
                    nbPlayerAlive++;
                }
            }
            return nbPlayerAlive;
        }

        private void UpdateTimer()
        {
            double incTimer = PhotonNetwork.Time - gameStartTime;

            remainingTime = roundTime - Mathf.Round((float)incTimer);
        }

        private void BeforeStart()
        {
            if (PhotonNetwork.IsMasterClient && !hasGameStarted)
            {
                //vérifier que tous les joueurs ont chargé
                if (CheckAllPlayerReady())
                {

                    //ajouter un évenemment à tous les joueurs
                    foreach(var player in FindObjectsOfType<PlayerController>())
                    {
                        player.OnPlayerDeath += OnPlayerDeath;
                    }

                    hasGameStarted = true;

                    //informer que les joueurs peuvent spawn
                    photonView.RPC("Spawn", RpcTarget.All);

                    //Démarrer le timer
                    Hashtable props = new Hashtable();
                    props.Add(SlideRaceGame.GAME_START_TIME, PhotonNetwork.Time);
                    props.Add(SlideRaceGame.HAS_GAME_STARTED, true);

                    PhotonNetwork.CurrentRoom.SetCustomProperties(props);
                    
                }

            }
            
        }

        private bool CheckAllPlayerReady()
        {
            Player[] players = PhotonNetwork.PlayerList;

            foreach (var player in players)
            {
                
                object ready;
                if(player.CustomProperties.TryGetValue(SlideRaceGame.PLAYER_READY, out ready))
                {
                    if (!(bool)ready)
                    {
                        return false;
                    }
                }

            }
            return true;
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            PhotonNetwork.DestroyPlayerObjects(otherPlayer);
        }



        [PunRPC]
        public void Spawn()
        {
            GameObject spawnedPlayer = PhotonNetwork.Instantiate("PhotonPlayer", Vector3.zero, Quaternion.identity);

            localPhotonPlayer = spawnedPlayer;
            
        }
        #endregion
    }
}


