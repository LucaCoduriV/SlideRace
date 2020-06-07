
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

        public static GameManager Instance;
        public GameObject playerPrefab;

        public bool hasGameStarted = false;
        public List<Transform> spawnList;
        public int previousSpawn = -1;

        public double gameStartTime;
        public double remainingTime = 0;
        public double roundTime = 300;

        public static event Action OnSpectateModeActivated = delegate { Debug.Log("Spectating mode enabled"); };
        public static event Action OnSpectateModeDisabled = delegate { Debug.Log("Spectating mode disabled"); };


        #endregion

        #region MonoBehaviour Methods

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this.GetComponent<GameManager>();
            }
            else
            {
                Destroy(this.gameObject);
            }

            SpawnController.OnSpawn += OnSpawn;
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
            PhotonNetwork.DestroyPlayerObjects(PhotonNetwork.LocalPlayer);
            PhotonNetwork.LeaveRoom();
        }

        public override void OnPlayerEnteredRoom(Player other)
        {
            //BeforeStart();
        }

        public override void OnJoinedRoom()
        {
            OnSpectateModeActivated();
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




        #endregion

        #region Private Methods

        private void OnSpawn()
        {
            PlayerController.LocalPlayerInstance.GetComponent<PlayerController>().OnPlayerDeath += OnPlayerDeath;

            //focus la camera sur le joueur
            OnSpectateModeDisabled();
        }
        private void OnPlayerDeath(int ownerNb)
        {
            if(PhotonNetwork.LocalPlayer.ActorNumber == ownerNb)
            {
                //activer le mode spectateur seulement si le joueur local meurt
                OnSpectateModeActivated();
            }

            if (PhotonNetwork.IsMasterClient)
            {
                if (GetNumberOfPlayerAlive() <= 1)
                {
                    //Faire les trucs de fin de partie
                    Debug.Log("Restart the game !!");
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
            PhotonNetwork.Instantiate("SpawnManager", Vector3.zero, Quaternion.identity);
            
        }
        #endregion
    }
}


