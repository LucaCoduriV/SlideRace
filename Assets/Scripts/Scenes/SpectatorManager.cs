using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpectatorManager : MonoBehaviourPun
{
    public InputMaster inputMaster;
    public float maxVelocityChange = 2.0f;
    public float acceleration = 1f;
    public float speed = 1.0f;
    public bool freeCam = false;

    public static event Action OnFreeCamMode;
    public static event Action<PlayerController[]> OnFollowPlayersMode;
    public static event Action<int> OnFollowPlayer;
    public static event Action OnFollowLocalPlayer;


    private Vector3 movement = Vector3.zero;
    private Vector3 previousTargetSpeed = Vector3.zero;
    private Transform cameraTransform;
    private PlayerController[] playerControllers;
    private int spectactingPlayer = 0;
    

    private void Awake()
    {
        
        inputMaster = new InputMaster();
        inputMaster.Spectator.Movement.performed += ctx => {
            if (PlayerController.LocalPlayerInstance.GetComponent<PlayerController>().IsDead)
            {
                SetMovementSpeed(ctx.ReadValue<Vector2>());
            }
        };
        inputMaster.Spectator.Movement.canceled += ctx => {
            if (PlayerController.LocalPlayerInstance.GetComponent<PlayerController>().IsDead)
            {
                SetMovementSpeed(ctx.ReadValue<Vector2>());
            }
            
        };
        inputMaster.Spectator.MovementVertical.performed += ctx => {
            if (PlayerController.LocalPlayerInstance.GetComponent<PlayerController>().IsDead)
            {
                SetVerticalMovementSpeed(ctx.ReadValue<float>());
            }
            
        };
        inputMaster.Spectator.MovementVertical.canceled += ctx => {
            if (PlayerController.LocalPlayerInstance.GetComponent<PlayerController>().IsDead)
            {
                SetVerticalMovementSpeed(ctx.ReadValue<float>());
            }
            
        };
        inputMaster.Spectator.NextPlayer.performed += ctx => {
            if (PlayerController.LocalPlayerInstance.GetComponent<PlayerController>().IsDead)
            {
                followNextPlayer();
            }
            
        };
        inputMaster.Spectator.PreviousPlayer.performed += ctx => {
            if (PlayerController.LocalPlayerInstance.GetComponent<PlayerController>().IsDead)
            {
                followPreviousPlayer();
            }
            
        };
        inputMaster.Spectator.FreeLook.performed += ctx => {
            if (PlayerController.LocalPlayerInstance.GetComponent<PlayerController>().IsDead)
            {
                freeCam = true; OnFreeCamMode?.Invoke();
            }
            
        };

        


    }

    private void Start()
    {
        cameraTransform = FindObjectOfType<CameraManager>().mainCamera.transform;


        Ch.Luca.MyGame.GameManager.OnSpectateModeActivated += OnSpectateModeActivated;
        Ch.Luca.MyGame.GameManager.OnSpectateModeDisabled += OnSpectateModeDisabled;

    }

    private void OnSpectateModeDisabled()
    {
        OnFollowLocalPlayer?.Invoke();
    }

    private void OnSpectateModeActivated()
    {
        getPlayerControllers();
        OnFollowPlayersMode?.Invoke(playerControllers);
        freeCam = true;
    }

    private void Update()
    {
        if (freeCam)
        {
            MoveCamera();
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
        OnFollowPlayer?.Invoke(spectactingPlayer);
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
        OnFollowPlayer?.Invoke(spectactingPlayer);
    }


    void MoveCamera()
    {
        // Calculate how fast we should be moving
        Vector3 targetVelocity = new Vector3(movement.x, movement.y, movement.z);
        //Vector3 targetVelocityVertical = new Vector3(0, movement.y, 0);

        //permet d'augmenter la vitesse petit à petit
        previousTargetSpeed = Vector3.Lerp(previousTargetSpeed, targetVelocity, acceleration * Time.fixedDeltaTime);

        targetVelocity = transform.TransformDirection(previousTargetSpeed);
        targetVelocity *= speed;

        cameraTransform.Translate(targetVelocity, Space.Self);
        //cameraTransform.Translate(targetVelocityVertical, Space.Self);
    }

    void SetMovementSpeed(Vector2 horizontalMovement)
    {
        movement.x = horizontalMovement.x;
        movement.z = horizontalMovement.y;
    }
    void SetVerticalMovementSpeed(float speed)
    {
        movement.y = speed;
    }

    private void OnEnable()
    {
        inputMaster.Enable();
    }

    private void OnDisable()
    {
        inputMaster.Disable();
        OnFreeCamMode = null;
        OnFollowLocalPlayer = null;
        OnFreeCamMode = null;
        OnFollowPlayersMode = null;
    }
}
