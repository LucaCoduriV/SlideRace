using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ch.Luca.MyGame;
using System;

public class SpawnController : MonoBehaviourPun
{
    public static SpawnController localInstance;

    public Transform mySpawnPoint;
    public static event Action OnSpawn = delegate { Debug.Log("Spawning..."); };

    

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
        mySpawnPoint = GameManager.Instance.spawnList[spawnNumber];
        if (photonView.IsMine)
        {
            PhotonNetwork.Instantiate("Crypto", SpawnController.localInstance.mySpawnPoint.position, SpawnController.localInstance.mySpawnPoint.rotation);
            OnSpawn();
        }
        

    }

    public static int GetNextSpawn()
    {
        GameManager.Instance.previousSpawn++;
        if(GameManager.Instance.previousSpawn < GameManager.Instance.spawnList.Count)
        {
            return GameManager.Instance.previousSpawn;
        }
        else
        {
            return 0;
        }
    }

}
