
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

        public double gameStartTime;
        private double remainingTime = 300;
        public double roundTime = 300;

        public double countDownStartTime;
        public double countDownTime = 5;
        private double countDownRemainingTime = 5;

        public double RemainingTime { get => remainingTime; }
        public double CountDownRemainingTime { get => countDownRemainingTime; }

        public static event Action OnSpectateModeActivated;
        public static event Action OnSpectateModeDisabled;
        private event Action OnCountDownEnd;

        

        public GameStatus gameStatus = GameStatus.WaitingForPlayers;


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
        }

        void Start()
        {
            Hashtable props = new Hashtable() { { SlideRaceGame.PLAYER_READY, true } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        }

        

        void Update()
        {
            switch (gameStatus)
            {
                case GameStatus.WaitingForPlayers:
                    //faire les vérification seulement si l'on est le masterclient
                    if (PhotonNetwork.IsMasterClient)
                    {
                        //vérifier que tous les joueurs ont chargé
                        if (CheckAllPlayerReady())
                        {
                            GameSetup();
                        }
                    }
                    break;

                case GameStatus.CountDown:
                    
                    UpdateCountDown();

                    //si le compte à rebours est terminé on lance la game
                    if (countDownRemainingTime <= 0.1)
                    {
                        gameStatus = GameStatus.Started;
                        StartGame();
                    }
                    break;

                case GameStatus.Started:
                    UpdateTimer();

                    break;

                case GameStatus.Finished:
                    break;

                case GameStatus.Restarting:
                    break;

                default:
                    break;
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

        public override void OnJoinedRoom()
        {
            OnSpectateModeActivated?.Invoke();
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
        }

        public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
        {

            object propsCountDownTime;
            if (propertiesThatChanged.TryGetValue(SlideRaceGame.GAME_COUNT_DOWN_START_TIME, out propsCountDownTime))
            {
                countDownStartTime = (double)propsCountDownTime;
            }

            object propsTime;
            if (propertiesThatChanged.TryGetValue(SlideRaceGame.GAME_START_TIME, out propsTime))
            {
                gameStartTime = (double)propsTime;
            }

            object propsGameStatus;
            if (propertiesThatChanged.TryGetValue(SlideRaceGame.GAME_STATUS, out propsGameStatus))
            {
                gameStatus = (GameStatus)propsGameStatus;
            }
        }

        #endregion

        #region Public Methods

        public void LeaveRoom()
        {
            OnSpectateModeActivated();
            PhotonNetwork.DestroyPlayerObjects(PhotonNetwork.LocalPlayer);
            PhotonNetwork.LeaveRoom();
        }

        

        


        [PunRPC]
        public void RestartScene()
        {
            PhotonNetwork.LoadLevel(SceneManager.GetActiveScene().buildIndex);
        }



        #endregion

        #region Private Methods

        [Obsolete("Cette methode n'est pas bonne.")]
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

        private void UpdateCountDown()
        {
            double incTimer = PhotonNetwork.Time - countDownStartTime;

            countDownRemainingTime = countDownTime - Mathf.Round((float)incTimer);
        }

        private void GameSetup()
        {
            
            //Faire spawner les joueurs
            GetComponent<SpawnManager>().SpawnPlayers();
                    

            //Démarrer le timer du compte à rebours
            Hashtable props = new Hashtable();
            props.Add(SlideRaceGame.GAME_COUNT_DOWN_START_TIME, PhotonNetwork.Time);
            //props.Add(SlideRaceGame.HAS_COUNT_DOWN_STARTED, true);
            props.Add(SlideRaceGame.GAME_STATUS, GameStatus.CountDown);
            PhotonNetwork.CurrentRoom.SetCustomProperties(props);

            countDownStartTime = PhotonNetwork.Time;
            //hasGameStarted = true;
            gameStatus = GameStatus.CountDown;

            GetComponent<BoostManager>().photonView.RPC("TurnBoostOff", RpcTarget.All);
            
        }

        private void StartGame()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                Hashtable props = new Hashtable();
                props.Add(SlideRaceGame.GAME_START_TIME, PhotonNetwork.Time);
                //props.Add(SlideRaceGame.HAS_GAME_STARTED, true);
                props.Add(SlideRaceGame.GAME_STATUS, GameStatus.Started);

                PhotonNetwork.CurrentRoom.SetCustomProperties(props);

                //hasCountDownStarted = false;

                GetComponent<ControlsManager>().photonView.RPC("TurnControllsOn", RpcTarget.All);
                GetComponent<BoostManager>().photonView.RPC("TurnBoostOn", RpcTarget.All);
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
        #endregion
    }
}


