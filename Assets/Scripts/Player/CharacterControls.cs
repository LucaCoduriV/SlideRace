using UnityEngine;
using System.Collections;
using Photon.Pun;
using CMF;
using System;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]

public class CharacterControls : MonoBehaviourPunCallbacks
{

    public InputMaster inputMaster;


    [Header("General")]
    public float gravity = 20.0f;

    [Header("On ground")]
    public float speed = 7.0f;

    #region Private Fields
    private bool doAJump;
    private bool isCrouching;
    private bool isTryingToStandUp = false;
    private Vector4 capsuleColliderDefault;
    private bool wasInstantiated = false;
    private PlayerController playerController;
    private Rigidbody rb;
    private Animator animator;

    //References to attached components;
    protected CharacterInput characterInput;
    protected Mover mover;
    protected CeilingDetector ceilingDetector;
    #endregion

    public enum ControllerState
    {
        Grounded,
        Sliding,
        Falling,
        Rising,
        Jumping
    }

    ControllerState currentControllerState = ControllerState.Falling;

    [Tooltip("Optional camera transform used for calculating movement direction. If assigned, character movement will take camera view into account.")]
    public Transform cameraTransform;

    [Tooltip("Whether to calculate and apply momentum relative to the controller's transform.")]
    public bool useLocalMomentum = true;

    //Current momentum;
    protected Vector3 momentum = Vector3.zero;

    //'AirFriction' determines how fast the controller loses its momentum while in the air;
    //'GroundFriction' is used instead, if the controller is grounded;
    public float airFriction = 0.5f;
    public float groundFriction = 100f;

    //Jump speed;
    public float jumpSpeed = 10f;

    //Jump duration variables;
    public float jumpDuration = 0.2f;
    float currentJumpStartTime = 0f;

    //Acceptable slope angle limit;
    public float slopeLimit = 80f;

    //Jump key variables;
    bool jumpKeyWasPressed = false;
    bool jumpKeyWasLetGo = false;
    bool jumpKeyIsPressed = false;

    //Events;
    public delegate void VectorEvent(Vector3 v);
    public VectorEvent OnJump;
    public VectorEvent OnLand;

    //Saved horizontal movement velocity from last frame;
    Vector3 savedMovementVelocity = Vector3.zero;

    //Movement speed;
    public float movementSpeed = 7f;

    //'Aircontrol' determines to what degree the player is able to move while in the air;
    [Range(0f, 1f)]
    public float airControl = 0.4f;

    //Saved velocity from last frame;
    Vector3 savedVelocity = Vector3.zero;


    void Awake()
    {
        mover = GetComponent<Mover>();
        rb = GetComponent<Rigidbody>();
        ceilingDetector = GetComponent<CeilingDetector>();

        rb.freezeRotation = true;
        rb.useGravity = false;
        rb.freezeRotation = true;

        inputMaster = new InputMaster();
        characterInput = GetComponent<CharacterInput>();

        playerController = GetComponent<PlayerController>();
        animator = GetComponent<Animator>();
        cameraTransform = Camera.main.transform;

        GetComponent<PlayerNamePlate>().Instantiate();

    }

   
    

    public void Instantiation()
    {
        //check if it is our character or not
        if (!photonView.IsMine && PhotonNetwork.IsConnected)
        {
            return;
        }
        else
        {
            inputMaster.Player.Jump.performed += ctx => { if (playerController == false) doAJump = true; };
            inputMaster.Player.Jump.canceled += ctx => doAJump = false;
            inputMaster.Player.Crouch.performed += ctx => { photonView.RPC("Crouch", RpcTarget.All, true); };
            inputMaster.Player.Crouch.canceled += ctx => { photonView.RPC("Crouch", RpcTarget.All, false); };

            transform.localPosition = Vector3.zero;
        }

        inputMaster.Enable();


        wasInstantiated = true;
        photonView.RPC("SetInstantiate", RpcTarget.All, true);
    }

    void Update()
    {
        if (wasInstantiated && photonView.IsMine)
        {
            //update Animation
            UpdateAnimationSpeed();

            if (isTryingToStandUp && CanStandUp())
            {
                isTryingToStandUp = false;
                photonView.RPC("Crouch", RpcTarget.All, false);
            }
            HandleJumpKeyInput();
        }
    }

    void FixedUpdate()
    {
        if (wasInstantiated && photonView.IsMine)
        {

            //Check if mover is grounded;
            mover.CheckForGround();

            //Determine controller state;
            currentControllerState = DetermineControllerState();
            UpdateAnimation();

            //Apply friction and gravity to 'momentum';
            HandleMomentum();

            //Check if the player has initiated a jump;
            HandleJumping();

            //Calculate movement velocity;
            Vector3 _velocity = CalculateMovementVelocity();

            //If local momentum is used, transform momentum into world space first;
            Vector3 _worldMomentum = momentum;
            if (useLocalMomentum)
                _worldMomentum = transform.localToWorldMatrix * momentum;

            //Add current momentum to velocity;
            _velocity += _worldMomentum;

            //If player is grounded or sliding on a slope, extend mover's sensor range;
            //This enables the player to walk up/down stairs and slopes without losing ground contact;
            mover.SetExtendSensorRange(IsGrounded());

            //Set mover velocity;		
            mover.SetVelocity(_velocity);

            //Store velocity for next frame;
            savedVelocity = _velocity;
            savedMovementVelocity = _velocity - _worldMomentum;

            //Reset jump key booleans;
            jumpKeyWasLetGo = false;
            jumpKeyWasPressed = false;

            //Reset ceiling detector, if one was attached to this gameobject;
            if (ceilingDetector != null)
                ceilingDetector.ResetFlags();
        }
    }

    private void UpdateAnimation()
    {
        switch (currentControllerState)
        {
            case ControllerState.Grounded:
                animator.SetBool("OnGround", true);
                break;
            case ControllerState.Sliding:
                animator.SetBool("OnGround", true);
                break;
            case ControllerState.Falling:
                animator.SetBool("OnGround", false);
                break;
            case ControllerState.Rising:
                animator.SetBool("OnGround", false);
                break;
            case ControllerState.Jumping:
                animator.SetTrigger("Jump");
                break;
            default:
                break;
        }
    }

    [PunRPC]
    private void Crouch(bool status)
    {
        

        if (status && !isCrouching)
        {
            GetComponent<Animator>().SetBool("Crouch", true);
            
            //déplacer le collider
            capsuleColliderDefault = GetComponent<CapsuleCollider>().center;
            capsuleColliderDefault.w = GetComponent<CapsuleCollider>().height;
            GetComponent<CapsuleCollider>().center = new Vector3(GetComponent<CapsuleCollider>().center.x, 0.64f, GetComponent<CapsuleCollider>().center.z);
            GetComponent<CapsuleCollider>().height = 1.3f;

            //Déplacer la camera
            transform.GetChild(0).transform.localPosition = new Vector3(transform.GetChild(0).transform.localPosition.x, transform.GetChild(0).transform.localPosition.y - 0.6f, transform.GetChild(0).transform.localPosition.z);
            isCrouching = true;

        }
        else if(!status && isCrouching)
        {
            
            if (CanStandUp())
            {
                GetComponent<Animator>().SetBool("Crouch", false);
                //déplacer le collider
                GetComponent<CapsuleCollider>().center = capsuleColliderDefault;
                GetComponent<CapsuleCollider>().height = capsuleColliderDefault.w;

                //Déplacer la camera
                transform.GetChild(0).transform.localPosition = new Vector3(transform.GetChild(0).transform.localPosition.x, transform.GetChild(0).transform.localPosition.y + 0.6f, transform.GetChild(0).transform.localPosition.z);
                isCrouching = false;
            }
            else
            {
                isTryingToStandUp = true;
            }
        }
    }

    private bool CanStandUp()
    {
        float radius = GetComponent<CapsuleCollider>().radius;
        float rayDistance = capsuleColliderDefault.w / 2;
        Vector3 rayStart = transform.TransformPoint(capsuleColliderDefault) + Vector3.up * (capsuleColliderDefault.w / 2 - rayDistance);

        //bool hit = Physics.BoxCast(rayStart, new Vector3(radius, rayDistance, radius), Vector3.up, Quaternion.Euler(0f,0f,0f), 0.1f);
        bool hit = Physics.Raycast(rayStart, Vector3.up, rayDistance);

        return !hit;
    }

    private void UpdateAnimationSpeed()
    {
        float yVel = Vector3.Dot(rb.velocity, transform.forward);
        float xVel = Vector3.Dot(rb.velocity, transform.right);

        if (isCrouching)
        {
            yVel /= 2;
            xVel /= 2;
        }

        GetComponent<Animator>().SetFloat("VelX", xVel / speed);
        GetComponent<Animator>().SetFloat("VelY", yVel / speed);
    }

    void HandleJumpKeyInput()
    {
        bool _newJumpKeyPressedState = IsJumpKeyPressed();

        if (jumpKeyIsPressed == false && _newJumpKeyPressedState == true)
            jumpKeyWasPressed = true;

        if (jumpKeyIsPressed == false && _newJumpKeyPressedState == false)
            jumpKeyWasLetGo = true;

        jumpKeyIsPressed = _newJumpKeyPressedState;
    }

    //Returns 'true' if the player presses the jump key;
    protected virtual bool IsJumpKeyPressed()
    {
        //If no character input script is attached to this object, return;
        if (characterInput == null)
            return false;

        return characterInput.IsJumpKeyPressed();
    }

    void HandleMomentum()
    {
        //If local momentum is used, transform momentum into world coordinates first;
        if (useLocalMomentum)
            momentum = transform.localToWorldMatrix * momentum;

        Vector3 _verticalMomentum = Vector3.zero;
        Vector3 _horizontalMomentum = Vector3.zero;

        //Split momentum into vertical and horizontal components;
        if (momentum != Vector3.zero)
        {
            _verticalMomentum = VectorMath.ExtractDotVector(momentum, transform.up);
            _horizontalMomentum = momentum - _verticalMomentum;
        }

        //Add gravity to vertical momentum;
        _verticalMomentum -= transform.up * gravity * Time.deltaTime;

        //Remove any downward force if the controller is grounded;
        if (currentControllerState == ControllerState.Grounded)
            _verticalMomentum = Vector3.zero;

        //Apply friction to horizontal momentum based on whether the controller is grounded;
        if (currentControllerState == ControllerState.Grounded)
            _horizontalMomentum = VectorMath.IncrementVectorLengthTowardTargetLength(_horizontalMomentum, groundFriction, Time.deltaTime, 0f);
        else
            _horizontalMomentum = VectorMath.IncrementVectorLengthTowardTargetLength(_horizontalMomentum, airFriction, Time.deltaTime, 0f);

        //Add horizontal and vertical momentum back together;
        momentum = _horizontalMomentum + _verticalMomentum;

        //Project the current momentum onto the current ground normal if the controller is sliding down a slope;
        if (currentControllerState == ControllerState.Sliding)
        {
            //momentum = Vector3.ProjectOnPlane(momentum, mover.GetGroundNormal());
        }

        //Apply slide gravity along ground normal, if controller is sliding;
        if (currentControllerState == ControllerState.Sliding)
        {
            //Vector3 _slideDirection = Vector3.ProjectOnPlane(-tr.up, mover.GetGroundNormal()).normalized;
            //momentum += _slideDirection * slideGravity * Time.deltaTime;
        }

        //If controller is jumping, override vertical velocity with jumpSpeed;
        if (currentControllerState == ControllerState.Jumping)
        {
            momentum = VectorMath.RemoveDotVector(momentum, transform.up);
            momentum += transform.up * jumpSpeed;
        }

        if (useLocalMomentum)
            momentum = transform.worldToLocalMatrix * momentum;

    }

    void HandleJumping()
    {
        if (currentControllerState == ControllerState.Grounded)
        {
            if (jumpKeyIsPressed == true || jumpKeyWasPressed)
            {
                //Call events;
                OnGroundContactLost();
                OnJumpStart();

                currentControllerState = ControllerState.Jumping;
            }
        }
    }

    //This function is called when the player has initiated a jump;
    void OnJumpStart()
    {
        //If local momentum is used, transform momentum into world coordinates first;
        if (useLocalMomentum)
            momentum = transform.localToWorldMatrix * momentum;

        //Add jump force to momentum;
        momentum += transform.up * jumpSpeed;

        //Set jump start time;
        currentJumpStartTime = Time.time;

        //Call event;
        if (OnJump != null)
            OnJump(momentum);

        if (useLocalMomentum)
            momentum = transform.worldToLocalMatrix * momentum;
    }

    //This function is called when the controller has lost ground contact, i.e. is either falling or rising, or generally in the air;
    void OnGroundContactLost()
    {
        //Calculate current velocity;
        //If velocity would exceed the controller's movement speed, decrease movement velocity appropriately;
        //This prevents unwanted accumulation of velocity;
        float _horizontalMomentumSpeed = VectorMath.RemoveDotVector(GetMomentum(), transform.up).magnitude;
        Vector3 _currentVelocity = GetMomentum() + Vector3.ClampMagnitude(savedMovementVelocity, Mathf.Clamp(movementSpeed - _horizontalMomentumSpeed, 0f, movementSpeed));

        //Calculate length and direction from '_currentVelocity';
        float _length = _currentVelocity.magnitude;

        //Calculate velocity direction;
        Vector3 _velocityDirection = Vector3.zero;
        if (_length != 0f)
            _velocityDirection = _currentVelocity / _length;

        //Subtract from '_length', based on 'movementSpeed' and 'airControl', check for overshooting;
        if (_length >= movementSpeed * airControl)
            _length -= movementSpeed * airControl;
        else
            _length = 0f;

        //If local momentum is used, transform momentum into world coordinates first;
        if (useLocalMomentum)
            momentum = transform.localToWorldMatrix * momentum;

        momentum = _velocityDirection * _length;

        if (useLocalMomentum)
            momentum = transform.worldToLocalMatrix * momentum;
    }

    //Get current momentum;
    public Vector3 GetMomentum()
    {
        Vector3 _worldMomentum = momentum;
        if (useLocalMomentum)
            _worldMomentum = transform.localToWorldMatrix * momentum;

        return _worldMomentum;
    }

    //Calculate and return movement direction based on player input;
    //This function can be overridden by inheriting scripts to implement different player controls;
    protected virtual Vector3 CalculateMovementDirection()
    {
        //If no character input script is attached to this object, return;
        if (characterInput == null)
            return Vector3.zero;

        Vector3 _velocity = Vector3.zero;

        //If no camera transform has been assigned, use the character's transform axes to calculate the movement direction;
        if (cameraTransform == null)
        {
            _velocity += transform.right * characterInput.GetHorizontalMovementInput();
            _velocity += transform.forward * characterInput.GetVerticalMovementInput();
        }
        else
        {
            //If a camera transform has been assigned, use the assigned transform's axes for movement direction;
            //Project movement direction so movement stays parallel to the ground;
            _velocity += Vector3.ProjectOnPlane(cameraTransform.right, transform.up).normalized * characterInput.GetHorizontalMovementInput();
            _velocity += Vector3.ProjectOnPlane(cameraTransform.forward, transform.up).normalized * characterInput.GetVerticalMovementInput();

            //update player rotation with the camera cameraTransform.rotation.y
            transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.eulerAngles.x, cameraTransform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z));
            
        }

        //If necessary, clamp movement vector to magnitude of 1f;
        if (_velocity.magnitude > 1f)
            _velocity.Normalize();

        return _velocity;
    }

    //Calculate and return movement velocity based on player input, controller state, ground normal [...];
    protected Vector3 CalculateMovementVelocity()
    {
        //Calculate (normalized) movement direction;
        Vector3 _velocity = CalculateMovementDirection();

        //Save movement direction for later;
        Vector3 _velocityDirection = _velocity;

        //Multiply (normalized) velocity with movement speed;
        _velocity *= movementSpeed;

        //If controller is not grounded, multiply movement velocity with 'airControl';
        if (!(currentControllerState == ControllerState.Grounded))
            _velocity = _velocityDirection * movementSpeed * airControl;

        return _velocity;
    }

    //Determine current controller state based on current momentum and whether the controller is grounded (or not);
    //Handle state transitions;
    ControllerState DetermineControllerState()
    {
        //Check if vertical momentum is pointing upwards;
        bool _isRising = IsRisingOrFalling() && (VectorMath.GetDotProduct(GetMomentum(), transform.up) > 0f);
        //Check if controller is sliding;
        bool _isSliding = mover.IsGrounded() && IsGroundTooSteep();

        //Grounded;
        if (currentControllerState == ControllerState.Grounded)
        {
            if (_isRising)
            {
                OnGroundContactLost();
                return ControllerState.Rising;
            }
            if (!mover.IsGrounded())
            {
                OnGroundContactLost();
                return ControllerState.Falling;
                
            }
            if (_isSliding)
            {
                return ControllerState.Sliding;
            }
            return ControllerState.Grounded;
        }

        //Falling;
        if (currentControllerState == ControllerState.Falling)
        {
            if (_isRising)
            {
                return ControllerState.Rising;
            }
            if (mover.IsGrounded() && !_isSliding)
            {
                OnGroundContactRegained(momentum);
                return ControllerState.Grounded;
            }
            if (_isSliding)
            {
                OnGroundContactRegained(momentum);
                return ControllerState.Sliding;
            }
            return ControllerState.Falling;
        }

        //Sliding;
        if (currentControllerState == ControllerState.Sliding)
        {
            if (_isRising)
            {
                OnGroundContactLost();
                return ControllerState.Rising;
            }
            if (!mover.IsGrounded())
            {
                return ControllerState.Falling;
            }
            if (mover.IsGrounded() && !_isSliding)
            {
                OnGroundContactRegained(momentum);
                return ControllerState.Grounded;
            }
            return ControllerState.Sliding;
        }

        //Rising;
        if (currentControllerState == ControllerState.Rising)
        {
            if (!_isRising)
            {
                if (mover.IsGrounded() && !_isSliding)
                {
                    OnGroundContactRegained(momentum);
                    return ControllerState.Grounded;
                }
                if (_isSliding)
                {
                    return ControllerState.Sliding;
                }
                if (!mover.IsGrounded())
                {
                    return ControllerState.Falling;
                }
            }

            //If a ceiling detector has been attached to this gameobject, check for ceiling hits;
            if (ceilingDetector != null)
            {
                if (ceilingDetector.HitCeiling())
                {
                    OnCeilingContact();
                    return ControllerState.Falling;
                }
            }
            return ControllerState.Rising;
        }

        //Jumping;
        if (currentControllerState == ControllerState.Jumping)
        {
            //Check for jump timeout;
            if ((Time.time - currentJumpStartTime) > jumpDuration)
                return ControllerState.Rising;

            //Check if jump key was let go;
            if (jumpKeyWasLetGo)
                return ControllerState.Rising;

            //If a ceiling detector has been attached to this gameobject, check for ceiling hits;
            if (ceilingDetector != null)
            {
                if (ceilingDetector.HitCeiling())
                {
                    OnCeilingContact();
                    return ControllerState.Falling;
                }
            }
            return ControllerState.Jumping;
        }

        return ControllerState.Falling;
    }

    //Returns 'true' if vertical momentum is above a small threshold;
    private bool IsRisingOrFalling()
    {
        //Calculate current vertical momentum;
        Vector3 _verticalMomentum = VectorMath.ExtractDotVector(GetMomentum(), transform.up);

        //Setup threshold to check against;
        //For most applications, a value of '0.001f' is recommended;
        float _limit = 0.001f;

        //Return true if vertical momentum is above '_limit';
        return (_verticalMomentum.magnitude > _limit);
    }

    //Returns true if angle between controller and ground normal is too big (> slope limit), i.e. ground is too steep;
    private bool IsGroundTooSteep()
    {
        if (!mover.IsGrounded())
            return true;

        return (Vector3.Angle(mover.GetGroundNormal(), transform.up) > slopeLimit);
    }

    //This function is called when the controller has landed on a surface after being in the air;
    void OnGroundContactRegained(Vector3 _collisionVelocity)
    {
        //Call 'OnLand' event;
        if (OnLand != null)
            OnLand(_collisionVelocity);
    }

    //This function is called when the controller has collided with a ceiling while jumping or moving upwards;
    void OnCeilingContact()
    {
        //If local momentum is used, transform momentum into world coordinates first;
        if (useLocalMomentum)
            momentum = transform.localToWorldMatrix * momentum;

        //Remove all vertical parts of momentum;
        momentum = VectorMath.RemoveDotVector(momentum, transform.up);

        if (useLocalMomentum)
            momentum = transform.worldToLocalMatrix * momentum;
    }

    //Returns 'true' if controller is grounded (or sliding down a slope);
    public bool IsGrounded()
    {
        return (currentControllerState == ControllerState.Grounded || currentControllerState == ControllerState.Sliding);
    }

    [PunRPC]
    private void SetInstantiate(bool status)
    {
        wasInstantiated = status;
    }

    public override void OnEnable()
    {
        if (!photonView.IsMine && PhotonNetwork.IsConnected == true)
        {
            return;
        }
        
    }

    public override void OnDisable()
    {
        if (!photonView.IsMine && PhotonNetwork.IsConnected == true)
        {
            return;
        }
        inputMaster.Disable();
    }
}