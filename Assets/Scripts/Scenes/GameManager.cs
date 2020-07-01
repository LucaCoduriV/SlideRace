
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
        public List<PlayerController> playerControllers;

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

        public event Action OnCountDownStart;
        public event Action OnGameStart;
        public event Action OnGameEnd;
        public event Action OnGameRestart;

        private bool wasEventCalled = false;



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
            PhotonNetwork.AutomaticallySyncScene = true;
            Hashtable props = new Hashtable() { { SlideRaceGame.PLAYER_READY, true } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);

            InvokeRepeating("UpdatePing", 0, 1.0f);

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
                    if (!wasEventCalled)
                    {
                        wasEventCalled = true;
                        OnCountDownStart?.Invoke();
                    }
                    

                    UpdateCountDown();

                    //si le compte à rebours est terminé on lance la game
                    if (countDownRemainingTime <= 0.1)
                    {
                        gameStatus = GameStatus.Started;
                        StartGame();
                    }


                    wasEventCalled = false;
                    break;

                case GameStatus.Started:
                    if (!wasEventCalled)
                    {
                        wasEventCalled = true;
                        OnGameStart?.Invoke();
                    }
                        

                    UpdateTimer();

                    wasEventCalled = false;
                    break;

                case GameStatus.Finished:
                    if (!wasEventCalled)
                    {
                        wasEventCalled = true;
                        OnGameEnd?.Invoke();
                    }

                    EndGame();

                    wasEventCalled = false;
                    break;

                case GameStatus.Restarting:
                    if (!wasEventCalled)
                    {
                        wasEventCalled = true;
                        OnGameRestart?.Invoke();
                    }

                    wasEventCalled = false;
                    photonView.RPC("RestartScene", RpcTarget.All);
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
            CancelInvoke();
        }

        #endregion


        #region Photon Callbacks

        public override void OnLeftRoom()
        {
            //CancelInvoke();
            SceneManager.LoadScene(0);
        }

        public override void OnJoinedRoom()
        {
            OnSpectateModeActivated?.Invoke();
            
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
            CancelInvoke("UpdatePing");
            PhotonNetwork.DestroyPlayerObjects(PhotonNetwork.LocalPlayer);
            PhotonNetwork.LeaveRoom();
            
        }

        [PunRPC]
        public void RestartScene()
        {
            CancelInvoke("UpdatePing");
            PhotonNetwork.LoadLevel(SceneManager.GetActiveScene().buildIndex);
        }

        #endregion

        #region Private Methods

        [Obsolete("Cette methode n'est pas bonne.")]
        public void OnPlayerDeath(object sender, object killer)
        {
            if(sender is PlayerController)
            {
                PlayerController targetPlayer = (PlayerController)sender;

                if (PhotonNetwork.LocalPlayer.ActorNumber == targetPlayer.photonView.OwnerActorNr)
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
        private void UpdatePing()
        {
            Hashtable props = new Hashtable();
            props.Add(SlideRaceGame.PLAYER_PING, PhotonNetwork.GetPing());
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        }

        private void GameSetup()
        {
            
            //Faire spawner les joueurs
            GetComponent<SpawnManager>().SpawnPlayers();
                    

            //Démarrer le timer du compte à rebours
            Hashtable props = new Hashtable();
            props.Add(SlideRaceGame.GAME_COUNT_DOWN_START_TIME, PhotonNetwork.Time);
            props.Add(SlideRaceGame.GAME_STATUS, GameStatus.CountDown);
            PhotonNetwork.CurrentRoom.SetCustomProperties(props);

            countDownStartTime = PhotonNetwork.Time;
            gameStatus = GameStatus.CountDown;

            GetComponent<BoostManager>().photonView.RPC("TurnBoostOff", RpcTarget.All);
            
        }

        private void StartGame()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                Hashtable props = new Hashtable();
                props.Add(SlideRaceGame.GAME_START_TIME, PhotonNetwork.Time);
                props.Add(SlideRaceGame.GAME_STATUS, GameStatus.Started);

                PhotonNetwork.CurrentRoom.SetCustomProperties(props);

                GetComponent<ControlsManager>().photonView.RPC("TurnControllsOn", RpcTarget.All);
                GetComponent<BoostManager>().photonView.RPC("TurnBoostOn", RpcTarget.All);
            }
        }

        private void EndGame()
        {
            OnGameEnd?.Invoke();
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


