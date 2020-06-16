using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraManager : MonoBehaviour
{
    public int isFollowingPlayer = -1;
    public InputMaster inputMaster;
    public GameObject mainCamera;

    private Transform cameraTransform;
    private PlayerController[] playerControllers;
    private PlayerController localPlayerController;

    public enum CameraMode
    {
        FollowLocalPlayer,
        FollowOtherPlayer,
        Free
    }

    public CameraMode cameraMode = CameraMode.Free;

    // Start is called before the first frame update
    void Start()
    {
        cameraTransform = mainCamera.transform;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void LateUpdate()
    {
        UpdateCameraPosition();
    }

    private void UpdateCameraPosition()
    {
        switch (cameraMode)
        {
            case CameraMode.FollowLocalPlayer:

                if (PlayerController.LocalPlayerInstance != null)
                {
                    cameraTransform.position = PlayerController.LocalPlayerInstance.transform.GetChild(0).transform.position;
                }
                break;

            case CameraMode.FollowOtherPlayer:

                if (isFollowingPlayer >= 0)
                {
                    if (playerControllers[isFollowingPlayer] != null)
                    {
                        cameraTransform.position = playerControllers[isFollowingPlayer].transform.GetChild(0).transform.position;
                        cameraTransform.rotation = playerControllers[isFollowingPlayer].transform.rotation;
                    }
                }
                break;

            case CameraMode.Free:

                break;

            default:
                break;
        }
    }

    public void FollowLocalPlayer()
    {
        cameraMode = CameraMode.FollowLocalPlayer;
        isFollowingPlayer = -1;

        localPlayerController = PlayerController.LocalPlayerInstance.GetComponent<PlayerController>();

    }

    public void FollowPlayer(int playerNumber)
    {
        cameraMode = CameraMode.FollowOtherPlayer;
        isFollowingPlayer = playerNumber;
    }

    public void FreeCamMode()
    {
        cameraMode = CameraMode.Free;
        isFollowingPlayer = -1;
    }

    public void GetPlayersToFollow(PlayerController[] playerControllers)
    {
        this.playerControllers = playerControllers;
        FreeCamMode();
    }
}
