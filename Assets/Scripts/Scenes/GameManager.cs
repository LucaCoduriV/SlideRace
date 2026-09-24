
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

        [Tooltip("Durée (s) pendant laquelle le gagnant est affiché avant de relancer une manche")]
        public double endOfRoundDelay = 5;
        public double gameEndTime;
        private int winnerActorNumber = PlayerController.NO_KILLER;
        private int playersAtRoundStart = 0;

        public double RemainingTime { get => remainingTime; }
        public double CountDownRemainingTime { get => countDownRemainingTime; }
        public int WinnerActorNumber { get => winnerActorNumber; }

        public static event Action OnSpectateModeActivated;
        public static event Action OnSpectateModeDisabled;

        public event Action OnCountDownStart;
        public event Action OnGameStart;
        public event Action OnGameEnd;
        public event Action OnGameRestart;

        private GameStatus lastNotifiedStatus = GameStatus.WaitingForPlayers;



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
            //déclencher les events une seule fois, au changement d'état
            if (gameStatus != lastNotifiedStatus)
            {
                lastNotifiedStatus = gameStatus;
                OnStatusChanged(gameStatus);
            }

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

                    //c'est le masterclient qui décide de la fin de la manche
                    if (PhotonNetwork.IsMasterClient)
                    {
                        CheckForRoundEnd();
                    }
                    break;

                case GameStatus.Finished:
                    //laisser le temps de voir le gagnant puis relancer la manche
                    if (PhotonNetwork.IsMasterClient && PhotonNetwork.Time - gameEndTime >= endOfRoundDelay)
                    {
                        RestartRound();
                    }
                    break;

                case GameStatus.Restarting:
                    //la scène est en train d'être rechargée
                    break;

                default:
                    break;
            }
        }

        private void OnStatusChanged(GameStatus status)
        {
            switch (status)
            {
                case GameStatus.CountDown:
                    OnCountDownStart?.Invoke();
                    break;
                case GameStatus.Started:
                    OnGameStart?.Invoke();
                    break;
                case GameStatus.Finished:
                    OnGameEnd?.Invoke();
                    break;
                case GameStatus.Restarting:
                    OnGameRestart?.Invoke();
                    break;
                default:
                    break;
            }
        }

        public override void OnDisable()
        {
            //se désinscrire des callbacks Photon, sinon l'ancienne instance les reçoit encore après un rechargement de scène
            base.OnDisable();
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

            object propsWinner;
            if (propertiesThatChanged.TryGetValue(SlideRaceGame.GAME_WINNER, out propsWinner))
            {
                winnerActorNumber = (int)propsWinner;
            }

            object propsEndTime;
            if (propertiesThatChanged.TryGetValue(SlideRaceGame.GAME_END_TIME, out propsEndTime))
            {
                gameEndTime = (double)propsEndTime;
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
            OnSpectateModeActivated?.Invoke();
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

        private void UpdateTimer()
        {
            double incTimer = PhotonNetwork.Time - gameStartTime;

            remainingTime = Math.Max(0, roundTime - Mathf.Round((float)incTimer));
        }

        //Exécuté par le masterclient : la manche se termine quand il ne reste qu'un survivant,
        //que tout le monde est mort, ou que le temps est écoulé.
        private void CheckForRoundEnd()
        {
            PlayerController[] allPlayers = FindObjectsOfType<PlayerController>();
            PlayerController lastAlive = null;
            int nbPlayerAlive = 0;

            foreach (var player in allPlayers)
            {
                if (!player.IsDead)
                {
                    nbPlayerAlive++;
                    lastAlive = player;
                }
            }

            //en solo (test) on ne gagne pas juste parce qu'on est le seul survivant
            int nbPlayersInRound = Math.Max(playersAtRoundStart, allPlayers.Length);

            if (nbPlayerAlive == 0)
            {
                EndRound(PlayerController.NO_KILLER);
            }
            else if (nbPlayersInRound >= 2 && nbPlayerAlive == 1)
            {
                EndRound(lastAlive.photonView.OwnerActorNr);
            }
            else if (remainingTime <= 0)
            {
                //temps écoulé avec plusieurs survivants : égalité
                EndRound(PlayerController.NO_KILLER);
            }
        }

        private void EndRound(int winner)
        {
            Debug.Log("Round finished, winner: " + winner);

            gameStatus = GameStatus.Finished;
            winnerActorNumber = winner;
            gameEndTime = PhotonNetwork.Time;

            Hashtable props = new Hashtable();
            props.Add(SlideRaceGame.GAME_STATUS, GameStatus.Finished);
            props.Add(SlideRaceGame.GAME_WINNER, winner);
            props.Add(SlideRaceGame.GAME_END_TIME, gameEndTime);
            PhotonNetwork.CurrentRoom.SetCustomProperties(props);

            //chaque joueur met à jour ses propres compteurs
            Player winnerPlayer = PhotonNetwork.CurrentRoom.GetPlayer(winner);
            if (winnerPlayer != null)
            {
                photonView.RPC("RPC_AddWin", winnerPlayer);
            }
        }

        [PunRPC]
        private void RPC_AddWin()
        {
            PlayerController.IncrementLocalPlayerProperty(SlideRaceGame.PLAYER_WIN_COUNTER);
        }

        private void RestartRound()
        {
            //ne relancer la scène qu'une seule fois
            gameStatus = GameStatus.Restarting;
            photonView.RPC("RestartScene", RpcTarget.All);
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
            //valeur locale en attendant celle du masterclient, sinon le timer part d'une valeur fausse
            gameStartTime = PhotonNetwork.Time;
            remainingTime = roundTime;
            playersAtRoundStart = FindObjectsOfType<PlayerController>().Length;

            if (PhotonNetwork.IsMasterClient)
            {
                Hashtable props = new Hashtable();
                props.Add(SlideRaceGame.GAME_START_TIME, gameStartTime);
                props.Add(SlideRaceGame.GAME_STATUS, GameStatus.Started);

                PhotonNetwork.CurrentRoom.SetCustomProperties(props);

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
            //seul le masterclient a le droit de détruire les objets d'un autre joueur
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.DestroyPlayerObjects(otherPlayer);
            }
        }
        #endregion
    }
}


