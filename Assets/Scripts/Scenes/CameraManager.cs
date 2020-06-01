using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public bool isFollowingLocalPlayer = false;
    public float mouseSensitivity = 100f;
    public InputMaster inputMaster;
    public GameObject mainCamera;
    

    float xRotation = 0.0f;
    float mouseX = 0.0f;
    float mouseY = 0.0f;
    private Transform cameraTransform;

    void Awake()
    {
        inputMaster = new InputMaster();
        inputMaster.Player.LookX.performed += ctx => mouseX = ctx.ReadValue<float>();
        inputMaster.Player.LookX.canceled += ctx => mouseX = ctx.ReadValue<float>();
        inputMaster.Player.LookY.performed += ctx => mouseY = ctx.ReadValue<float>();
        inputMaster.Player.LookY.canceled += ctx => mouseY = ctx.ReadValue<float>();
    }

    // Start is called before the first frame update
    void Start()
    {
        cameraTransform = mainCamera.transform;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        
        
    }

    private void LateUpdate()
    {
        if (isFollowingLocalPlayer)
        {



            if (PlayerController.LocalPlayerInstance != null)
            {
                cameraTransform.position = PlayerController.LocalPlayerInstance.transform.GetChild(0).transform.position;

            }

            RotateCamera();
        }
    }

    private void FixedUpdate()
    {
        
    }

    public void FollowLocalPlayer()
    {
        isFollowingLocalPlayer = true;
    }

    public void RotateCamera()
    {
        float mouseX = this.mouseX * mouseSensitivity * Time.deltaTime;
        float mouseY = this.mouseY * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);


        cameraTransform.transform.rotation = Quaternion.Euler(xRotation, PlayerController.LocalPlayerInstance.transform.rotation.eulerAngles.y, PlayerController.LocalPlayerInstance.transform.rotation.eulerAngles.z);

        PlayerController.LocalPlayerInstance.transform.Rotate(Vector3.up * mouseX);


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
