using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlsManager : MonoBehaviourPun
{
    private static bool isControlsActivated;

    public static bool IsControlsActivated { get => isControlsActivated; }

    [PunRPC]
    public void TurnControllsOff()
    {
        isControlsActivated = false;
    }

    [PunRPC]
    public void TurnControllsOn()
    {
        isControlsActivated = true;
    }

    private void OnDisable()
    {
        TurnControllsOff();
    }

}
