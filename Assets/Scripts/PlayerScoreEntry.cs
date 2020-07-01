using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PlayerScoreEntry : MonoBehaviourPunCallbacks
{
    public Player player;

    private int win;
    private int kill;
    private int death;
    private int ping;
    private string playerName;

    [SerializeField] private TMPro.TMP_Text winText;
    [SerializeField] private TMPro.TMP_Text killText;
    [SerializeField] private TMPro.TMP_Text deathText;
    [SerializeField] private TMPro.TMP_Text pingText;
    [SerializeField] private TMPro.TMP_Text nameText;

    private void Start()
    {
        InvokeRepeating("UpdatePing", 0f, 1.0f);
    }

    public void SetPlayerName(string name)
    {
        playerName = name;
        nameText.SetText(playerName);
    }

    public void AddWin(int numberToAdd)
    {
        win += numberToAdd;
        winText.SetText(win.ToString());
    }

    public void SetWin(int newValue)
    {
        win = newValue;
        winText.SetText(win.ToString());
    }

    public void AddKill(int numberToAdd)
    {
        kill += numberToAdd;
        killText.SetText(kill.ToString());
    }

    public void SetKill(int newValue)
    {
        kill = newValue;
        killText.SetText(kill.ToString());
    }

    public void AddDeath(int numberToAdd)
    {
        death += numberToAdd;
        deathText.SetText(death.ToString());
    }

    public void SetDeath(int newValue)
    {
        death = newValue;
        deathText.SetText(death.ToString());
    }

    public void SetPing(int newValue)
    {
        ping = newValue;
        pingText.SetText(ping.ToString());
    }

    public void UpdatePing()
    {
        if(player != null)
        {
            object ping;
            if (player.CustomProperties.TryGetValue(SlideRaceGame.PLAYER_PING, out ping))
            {
                SetPing((int)ping);
            }
        }
        else
        {
            Debug.Log("Player not set");
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if(targetPlayer == player)
        {
            UpdateFromProperties(changedProps);
        }
        
    }

    public void UpdateFromProperties(Player player)
    {
        object props;
        if (player.CustomProperties.TryGetValue(SlideRaceGame.PLAYER_DEATH_COUNTER, out props))
        {
            SetDeath((int)props);
        }
        if (player.CustomProperties.TryGetValue(SlideRaceGame.PLAYER_KILL_COUNTER, out props))
        {
            SetKill((int)props);
        }
    }

    public void UpdateFromProperties(Hashtable changedProps)
    {
        object props;
        if (changedProps.TryGetValue(SlideRaceGame.PLAYER_DEATH_COUNTER, out props))
        {
            SetDeath((int)props);
        }
        if (changedProps.TryGetValue(SlideRaceGame.PLAYER_KILL_COUNTER, out props))
        {
            SetKill((int)props);
        }
    }

    public override void OnDisable()
    {
        CancelInvoke("UpdatePing");
    }
}
