using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ch.Luca.MyGame;
using System;

public class SpawnController : MonoBehaviourPunCallbacks
{
    public static SpawnController localInstance;

    public Transform mySpawnPoint;
    public static event Action OnLocalPlayerSpawn;

    

    private void Start()
    {
        if (photonView.IsMine)
        {
            if (localInstance == null)
            {
                localInstance = GetComponent<SpawnController>();
            }

            photonView.RPC("AskSpawn", RpcTarget.MasterClient);
            
        }
        
    }

    [PunRPC]
    void AskSpawn()
    {
        int spawnNumber = SpawnController.GetNextSpawn();
        Debug.Log("TU VAS SPAWNER ICI " + spawnNumber);
        photonView.RPC("SendSpawn", RpcTarget.All, spawnNumber);
    }

    [PunRPC]
    void SendSpawn(int spawnNumber)
    {

        mySpawnPoint = GameManager.instance.spawnList[spawnNumber];
        if (photonView.IsMine)
        {
            PhotonNetwork.Instantiate("Crypto", SpawnController.localInstance.mySpawnPoint.position, SpawnController.localInstance.mySpawnPoint.rotation);
            OnLocalPlayerSpawn?.Invoke();
        }
        

    }

    public static int GetNextSpawn()
    {
        GameManager.instance.previousSpawn++;
        if(GameManager.instance.previousSpawn < GameManager.instance.spawnList.Count)
        {
            return GameManager.instance.previousSpawn;
        }
        else
        {
            return 0;
        }
    }

    public override void OnDisable()
    {
        localInstance = null;
        OnLocalPlayerSpawn = null;
    }
}
