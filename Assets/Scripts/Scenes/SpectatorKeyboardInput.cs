using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpectatorKeyboardInput : MonoBehaviour
{
    public InputMaster inputMaster;

    private void Awake()
    {
        inputMaster = new InputMaster();
    }

    public float GetHorizontalMovementInput()
    {
        return inputMaster.Spectator.Movement.ReadValue<Vector2>().x;
    }

    public float GetVerticalMovementInput()
    {
        return inputMaster.Spectator.Movement.ReadValue<Vector2>().y;
    }

    public float GetUpDownMovement()
    {
        return inputMaster.Spectator.MovementVertical.ReadValue<float>();
    }

    public bool IsNextPlayerKeyPressed()
    {
        return inputMaster.Spectator.NextPlayer.triggered;
    }

    public bool IsNextPreviousKeyPressed()
    {
        return inputMaster.Spectator.PreviousPlayer.triggered;
    }

    public bool IsFreeLookKeyPressed()
    {
        return inputMaster.Spectator.FreeLook.triggered;
    }

    private void OnEnable()
    {
        inputMaster.Enable();
    }

    private void OnDisable()
    {
        inputMaster.Disable();
    }

}
