using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public float mouseSensitivity = 100f;
    public InputMaster inputMaster;
    public Transform playerBody;

    float xRotation = 0.0f;
    float mouseX = 0.0f;
    float mouseY = 0.0f;

    private void Awake()
    {
        inputMaster = new InputMaster();
        inputMaster.Player.LookX.performed += ctx => mouseX = ctx.ReadValue<float>();
        inputMaster.Player.LookX.canceled += ctx => mouseX = ctx.ReadValue<float>();
        inputMaster.Player.LookY.performed += ctx => mouseY = ctx.ReadValue<float>();
        inputMaster.Player.LookY.canceled += ctx => mouseY = ctx.ReadValue<float>(); ;
    }

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = this.mouseX * mouseSensitivity * Time.deltaTime;
        float mouseY = this.mouseY * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);

    }

    public void OnEnable()
    {
        inputMaster.Enable();
    }

    public void OnDisable()
    {
        inputMaster.Disable();
    }
}
