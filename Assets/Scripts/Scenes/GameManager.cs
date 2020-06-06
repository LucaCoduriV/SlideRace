
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System.Collections.Generic;

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
            //BeforeStart();
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
        }

        

        #endregion

        #region Private Methods

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
                    //informer que les joueurs peuvent spawn
                    hasGameStarted = true;
                    photonView.RPC("Spawn", RpcTarget.All);

                    //Démarrer le timer
                    Hashtable props = new Hashtable();
                    props.Add(SlideRaceGame.GAME_START_TIME, PhotonNetwork.Time);

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

        

        [PunRPC]
        public void Spawn()
        {
            PhotonNetwork.Instantiate("SpawnManager", Vector3.zero, Quaternion.identity);
            
        }
        #endregion
    }
}


