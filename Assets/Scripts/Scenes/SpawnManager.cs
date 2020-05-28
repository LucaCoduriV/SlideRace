using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager instance;

    
    [SerializeField] private List<Transform> spawnList;
    public Stack<Transform> spawns;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }

        spawns = new Stack<Transform>(spawnList);
    }

}
