using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraManager : MonoBehaviour
{
    public bool isFollowingLocalPlayer = false;
    public InputMaster inputMaster;
    public GameObject mainCamera;
    

    float xRotation = 0.0f;
    float mouseX = 0.0f;
    float mouseY = 0.0f;

    Vector2 _lookValue = Vector2.zero;
    Vector3 _rotation = Vector3.zero;
    private Transform cameraTransform;

    void Awake()
    {
        inputMaster = new InputMaster();
        
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

                if (!PlayerController.LocalPlayerInstance.GetComponent<PlayerController>().IsDead && !PlayerController.LocalPlayerInstance.GetComponent<Ragdoll>().isRagdoll && UserInterface.instance.followMouse)
                {
                    RotateCamera();

                    
                }
                

            }

            
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

        _lookValue = inputMaster.Player.MouseAim.ReadValue<Vector2>();

        _rotation.x += -_lookValue.y * ConfigManager.config.MouseSensitivity * Time.deltaTime;
        _rotation.y += _lookValue.x * ConfigManager.config.MouseSensitivity * Time.deltaTime;

        cameraTransform.transform.rotation = Quaternion.Euler(_rotation);

        Vector3 playerTransformRotation = PlayerController.LocalPlayerInstance.transform.eulerAngles;

        PlayerController.LocalPlayerInstance.transform.eulerAngles = new Vector3(playerTransformRotation.x, cameraTransform.transform.rotation.eulerAngles.y, playerTransformRotation.z);
    }

    private void OnMoveInput(InputAction.CallbackContext context)
    {
        _lookValue = context.ReadValue<Vector2>();
    }

    public void OnEnable()
    {
        inputMaster.Enable();

        //inputMaster.Player.MouseAim.performed += OnMoveInput;
    }

    public void OnDisable()
    {
        inputMaster.Disable();
    }
}
