using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpectatorManager : MonoBehaviourPun
{
    public float maxVelocityChange = 2.0f;
    public float acceleration = 1f;
    public float speed = 1.0f;
    public bool freeCam = false;

    private SpectatorKeyboardInput keyboardInput;
    private CameraManager cameraManager;

    private Vector3 movement = Vector3.zero;
    private Vector3 previousTargetSpeed = Vector3.zero;
    private Transform cameraTransform;
    private PlayerController[] playerControllers;
    private int spectactingPlayer = 0;

    private void Start()
    {
        cameraTransform = FindObjectOfType<CameraManager>().mainCamera.transform;
        cameraManager = GetComponent<CameraManager>();
        keyboardInput = GetComponent<SpectatorKeyboardInput>();
    }

    public void SpectatorModeDisable()
    {
        if(cameraManager != null)
            cameraManager.FollowLocalPlayer();
    }

    public void SpectatorModeEnabled()
    {
        getPlayerControllers();

        if (cameraManager != null)
            cameraManager.GetPlayersToFollow(playerControllers);

        freeCam = true;
    }

    private void Update()
    {


        if (freeCam)
        {
            MoveCamera();

            if (keyboardInput.IsFreeLookKeyPressed())
            {
                freeCam = true;
                if (cameraManager != null)
                    cameraManager.FreeCamMode();
            }

            if (keyboardInput.IsNextPlayerKeyPressed())
                followNextPlayer();


            if (keyboardInput.IsNextPreviousKeyPressed())
                followPreviousPlayer();
        }

    }

    public void getPlayerControllers()
    {
        playerControllers = FindObjectsOfType<PlayerController>();
    }

    public void followNextPlayer()
    {
        if(spectactingPlayer < playerControllers.Length - 1)
        {
            spectactingPlayer++;
        }
        else
        {
            spectactingPlayer = 0;
        }
        GetComponent<CameraManager>().FollowPlayer(spectactingPlayer);
    }
    public void followPreviousPlayer()
    {
        if (spectactingPlayer > 0)
        {
            spectactingPlayer--;
        }
        else
        {
            spectactingPlayer = playerControllers.Length - 1;
        }
        if (cameraManager != null)
            cameraManager.FollowPlayer(spectactingPlayer);
    }


    void MoveCamera()
    {
        float VerticalMovement = keyboardInput.GetVerticalMovementInput();
        float HonrizontallMovement = keyboardInput.GetHorizontalMovementInput();
        float UpDownMovement = keyboardInput.GetUpDownMovement();


        // Calculate how fast we should be moving
        Vector3 targetVelocity = new Vector3(HonrizontallMovement, UpDownMovement, VerticalMovement);

        //permet d'augmenter la vitesse petit à petit
        previousTargetSpeed = Vector3.Lerp(previousTargetSpeed, targetVelocity, acceleration * Time.fixedDeltaTime);

        targetVelocity = transform.TransformDirection(previousTargetSpeed);
        targetVelocity *= speed;

        cameraTransform.Translate(targetVelocity, Space.Self);
    }
}
