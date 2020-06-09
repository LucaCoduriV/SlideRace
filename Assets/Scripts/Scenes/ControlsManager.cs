using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlsManager : MonoBehaviour
{
    private static bool isControlsActivated;

    public static bool IsControlsActivated { get => isControlsActivated; }

    public void TurnControllsOff()
    {
        isControlsActivated = false;
    }

    public void TurnControllsOn()
    {
        isControlsActivated = true;
    }

    private void OnDisable()
    {
        TurnControllsOff();
    }

}
