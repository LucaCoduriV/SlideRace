using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerKeyboardInput : MonoBehaviour
{
    private InputMaster inputMaster;

    private bool IsJumpPressed = false;

    private void Awake()
    {
        inputMaster = new InputMaster();

        inputMaster.Player.Jump.performed += ctx => { IsJumpPressed = true; };
        inputMaster.Player.Jump.canceled += ctx => { IsJumpPressed = false; };
    }

    public float GetHorizontalMovementInput()
    {
        
        return inputMaster.Player.Movement.ReadValue<Vector2>().x;
    }

    public float GetVerticalMovementInput()
    {
        
        return inputMaster.Player.Movement.ReadValue<Vector2>().y;
    }

    public bool IsJumpKeyPressed()
    {
        return IsJumpPressed;
    }

    public bool IsShootPressed()
    {
        return inputMaster.Player.Shoot.triggered;
    }

    public bool IsUsePressed()
    {
        return inputMaster.Player.UseItem.triggered;
    }
    public bool IsNextItemPressed()
    {
        return inputMaster.Player.NextItem.triggered;
    }

    public bool IsPreviousItemPressed()
    {
        return inputMaster.Player.PreviousItem.triggered;
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
