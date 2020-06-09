using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostManager : MonoBehaviour
{
    public static List<Boost> boostList;

    void Start()
    {
        boostList = new List<Boost>();

        foreach (var boost in FindObjectsOfType<Boost>())
        {
            boostList.Add(boost);
        }
    }

    public void TurnBoostOff()
    {
        foreach (var boost in boostList)
        {
            boost.isActivated = false;
        }
    }

    public void TurnBoostOn()
    {
        foreach (var boost in boostList)
        {
            boost.isActivated = true;
        }
    }
}
