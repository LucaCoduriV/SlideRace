using UnityEngine;

public class SlideRaceGame
{
    //Player
    public const string PLAYER_READY = "IsPlayerReady";
    public const string PLAYER_IS_ALIVE = "IsPlayerAlive"; //pas encore utilisé
    public const string PLAYER_LOADED_LEVEL = "PlayerLoadedLevel"; //pas encore utilisé
    public const string PLAYER_DEATH_COUNTER = "PlayerDeathCounter";
    public const string PLAYER_KILL_COUNTER = "PlayerKillCounter";
    public const string PLAYER_PING = "PlayerPing";

    //Game
    public const string GAME_START_TIME = "GameStartTime";
    public const string GAME_COUNT_DOWN_START_TIME = "CountDownStartTime";
    public const string HAS_GAME_STARTED = "HasGameStarted";
    public const string HAS_COUNT_DOWN_STARTED = "HasCountDownStarted";
    public const string GAME_STATUS = "GameStatus";

}

public enum GameStatus
{
    WaitingForPlayers,
    CountDown,
    Started,
    Finished,
    Restarting
}

public enum LocalPlayerStatus
{
    Dead,
    Alive,
    Spectating
}
