using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraManager : MonoBehaviour
{
    public bool isFollowingLocalPlayer = false;
    public bool freeCamMode = false;
    public int isFollowingPlayer = -1;
    public InputMaster inputMaster;
    public GameObject mainCamera;
    

    float xRotation = 0.0f;
    float mouseX = 0.0f;
    float mouseY = 0.0f;

    Vector2 _lookValue = Vector2.zero;
    Vector3 _rotation = Vector3.zero;
    private Transform cameraTransform;
    private PlayerController[] playerControllers;



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
            else
            {
                Debug.LogError("No localplayer to follow : " + PlayerController.LocalPlayerInstance.GetComponent<PlayerController>().Health);
            }
        }
        else if (freeCamMode)
        {
            RotateCameraSpectator();
        }
        else if(isFollowingPlayer >= 0)
        {
            if(playerControllers[isFollowingPlayer] != null)
            {
                cameraTransform.position = playerControllers[isFollowingPlayer].transform.GetChild(0).transform.position;
                cameraTransform.rotation = playerControllers[isFollowingPlayer].transform.rotation;
            }
            
        }
    }

    public void FollowLocalPlayer()
    {
        isFollowingLocalPlayer = true;
        freeCamMode = false;
        isFollowingPlayer = -1;
    }

    public void FollowPlayer(int playerNumber)
    {
        isFollowingLocalPlayer = false;
        freeCamMode = false;
        isFollowingPlayer = playerNumber;
    }

    public void FreeCamMode()
    {
        isFollowingLocalPlayer = false;
        freeCamMode = true;
        isFollowingPlayer = -1;
    }

    public void GetPlayersToFollow(PlayerController[] playerControllers)
    {
        this.playerControllers = playerControllers;
        FreeCamMode();
    }

    public void RotateCamera()
    {

        _lookValue = inputMaster.Player.MouseAim.ReadValue<Vector2>();

        _rotation.x += -_lookValue.y * ConfigManager.config.MouseSensitivity * Time.deltaTime;
        _rotation.y += _lookValue.x * ConfigManager.config.MouseSensitivity * Time.deltaTime;

        _rotation.x = Mathf.Clamp(_rotation.x, -90, 90);

        cameraTransform.transform.rotation = Quaternion.Euler(_rotation);

        Vector3 playerTransformRotation = PlayerController.LocalPlayerInstance.transform.eulerAngles;

        PlayerController.LocalPlayerInstance.transform.eulerAngles = new Vector3(playerTransformRotation.x, cameraTransform.transform.rotation.eulerAngles.y, playerTransformRotation.z);
    }

    public void RotateCameraSpectator()
    {
        _lookValue = inputMaster.Spectator.MouseAim.ReadValue<Vector2>();

        _rotation.x += -_lookValue.y * ConfigManager.config.MouseSensitivity * Time.deltaTime;
        _rotation.y += _lookValue.x * ConfigManager.config.MouseSensitivity * Time.deltaTime;

        _rotation.x = Mathf.Clamp(_rotation.x, -90, 90);

        cameraTransform.transform.rotation = Quaternion.Euler(_rotation);

    }

    private void OnMoveInput(InputAction.CallbackContext context)
    {
        _lookValue = context.ReadValue<Vector2>();
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
