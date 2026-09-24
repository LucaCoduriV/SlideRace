using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Ch.Luca.MyGame;
using Photon.Pun;

public class HUDController : MonoBehaviour
{
    public Text textHP;
    public Text deadMessage;
    public Text timeText;
    public TMPro.TMP_Text countDownText;
    public float goMessageDuration = 1f;
    

    private static HUDController instance;
    
    
    private static PlayerController playerToShowHUD;
    


    

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        //hide death message
        deadMessage.gameObject.SetActive(false);
        countDownText.gameObject.SetActive(true);

    }

    public static void SetPlayerToShowHUD(PlayerController playerToShowHUD)
    {
        HUDController.playerToShowHUD = playerToShowHUD;
    }

    public static PlayerController GetPlayerToShowHUD()
    {
        return HUDController.playerToShowHUD;
    }
    

    // Update is called once per frame
    void Update()
    {
        GameManager gameManager = GameManager.instance;
        if (gameManager == null)
            return;

        switch (gameManager.gameStatus)
        {
            case GameStatus.Started:
                //afficher "GO!" brièvement au début de la manche
                bool justStarted = PhotonNetwork.Time - gameManager.gameStartTime < goMessageDuration;
                countDownText.gameObject.SetActive(justStarted);
                if (justStarted)
                    countDownText.text = "GO!";

                UpdateGameTimer();
                break;

            case GameStatus.Finished:
            case GameStatus.Restarting:
                countDownText.gameObject.SetActive(true);
                countDownText.text = GetRoundResultMessage(gameManager.WinnerActorNumber);
                UpdateGameTimer();
                break;

            default:
                if (gameManager.CountDownRemainingTime > 0)
                {
                    countDownText.gameObject.SetActive(true);
                    UpdateCountDown();
                }
                else
                {
                    countDownText.gameObject.SetActive(false);
                }
                break;
        }
    }

    private string GetRoundResultMessage(int winnerActorNumber)
    {
        if (winnerActorNumber == PlayerController.NO_KILLER)
            return "Draw!";

        if (winnerActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
            return "You win!";

        Photon.Realtime.Player winner = PhotonNetwork.CurrentRoom?.GetPlayer(winnerActorNumber);
        return (winner != null ? winner.NickName : "Someone") + " wins!";
    }

    public static void UpdateHUD()
    {
        if (playerToShowHUD != null)
        {
            instance.UpdateTextHP(playerToShowHUD.Health);
            instance.UpdateDeadMessage(playerToShowHUD.IsDead);
        }
    }
    private void UpdateTextHP(float HP)
    {
        if (!HUDController.playerToShowHUD.IsDead)
        {
            textHP.transform.parent.gameObject.SetActive(true);
            textHP.text = Mathf.RoundToInt(HP).ToString() + " HP";
        }
        else
        {
            textHP.transform.parent.gameObject.SetActive(false);
        }
        
    }
    private void UpdateDeadMessage(bool isDead)
    {
        if (isDead)
        {
            deadMessage.gameObject.SetActive(true);
        }
        else
        {
            deadMessage.gameObject.SetActive(false);
        }
    }
    private void UpdateGameTimer()
    {
        //format mm:ss
        int seconds = Mathf.Max(0, (int)GameManager.instance.RemainingTime);
        timeText.text = string.Format("{0}:{1:00}", seconds / 60, seconds % 60);
    }

    private void UpdateCountDown()
    {
        countDownText.text = GameManager.instance.CountDownRemainingTime.ToString();
    }

    public void OnDisable()
    {
        instance = null;
        playerToShowHUD = null;
    }
}
