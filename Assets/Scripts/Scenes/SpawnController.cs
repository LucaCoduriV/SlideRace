using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ch.Luca.MyGame;

public class SpawnController : MonoBehaviourPun
{
    public static SpawnController localInstance;

    public Transform mySpawnPoint;

    

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
