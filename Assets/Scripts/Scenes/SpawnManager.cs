using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ch.Luca.MyGame;
using System;

public class SpawnManager : MonoBehaviourPunCallbacks
{
    public static SpawnManager localInstance;
    public List<Transform> spawnList;
    private List<Transform> usedSpawnList = new List<Transform>();



    private void Awake()
    {
        if (photonView.IsMine)
        {
            if (localInstance == null)
            {
                localInstance = GetComponent<SpawnManager>();
            }
        }
    }

    private bool GetSpawnPoint(out Transform spawnPoint)
    {
        if (spawnList.Count == 0)
        {
            spawnPoint = default(Transform);
            return false;
        }
            

        //la borne max est exclue pour des int, Count permet donc de tirer tous les spawns
        int spawnNumber = UnityEngine.Random.Range(0, spawnList.Count);
        spawnPoint = spawnList[spawnNumber];

        spawnList.RemoveAt(spawnNumber);
        usedSpawnList.Add(spawnPoint);
        return true;

    }

    public void SpawnPlayers()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Photon.Realtime.Player[] players = PhotonNetwork.PlayerList;
            foreach (var player in players)
            {
                Debug.Log("NB of players: " + PhotonNetwork.PlayerList.Length);
                Transform spawnPoint;
                if (!GetSpawnPoint(out spawnPoint))
                {
                    //plus de spawn libre : réutiliser un spawn déjà pris plutôt que de ne pas faire apparaître le joueur
                    Debug.LogWarning("Not enough spawn points for " + players.Length + " players");
                    spawnPoint = usedSpawnList[UnityEngine.Random.Range(0, usedSpawnList.Count)];
                }

                GameObject character = PhotonNetwork.Instantiate("Crypto", spawnPoint.position, spawnPoint.rotation);
                PhotonView pv = character.GetComponent<PhotonView>();
                pv.TransferOwnership(player);
                pv.RPC("TakeControl", RpcTarget.All);
            }
            //photonView.RPC("TakeControlTransferedPlayer", RpcTarget.All);
        }
        
    }

    [PunRPC]
    public void TakeControlTransferedPlayer()
    {
        foreach (var playerController in FindObjectsOfType<PlayerController>())
        {
            if (playerController.photonView.IsMine)
            {
                PlayerController.LocalPlayerInstance = playerController.gameObject;
            }
        }
    }
    

    public override void OnDisable()
    {
        localInstance = null;
    }
}
