using UnityEngine;
using System.Collections;
using Photon.Pun;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]

public class CharacterControls : MonoBehaviourPunCallbacks
{

    public InputMaster inputMaster;


    [Header("General")]
    public float gravity = 20.0f;
    public float maxVelocityChange = 10.0f;
    public float acceleration = 4f;
    

    [Header("On ground")]
    public float speed = 7.0f;
    public bool canJump = true;
    public float jumpHeight = 1.5f;

    [Header("In air")]
    [Tooltip("Permet de modifier le control dans les airs")]
    public float inAirSpeedMultiplier = 2f;

    [Header("In Boost")]

    #region Private Fields
    private bool grounded = false;
    private float horizontalAxe;
    private float verticalAxe;
    private bool doAJump;
    private bool isCrouching;
    private bool isTryingToStandUp = false;
    private Vector3 previousTargetSpeed = Vector3.zero;
    private Vector3 MaxVelocity = Vector3.zero;
    private Vector4 capsuleColliderDefault;
    private Vector3 DefaultCameraPosition;
    #endregion



    void Awake()
    {
        GetComponent<Rigidbody>().freezeRotation = true;
        GetComponent<Rigidbody>().useGravity = false;

        GetComponent<Rigidbody>().freezeRotation = true;

        //check if it is our character or not
        if (!photonView.IsMine && PhotonNetwork.IsConnected)
        {
            return;
        }
        else{
            inputMaster = new InputMaster();

            inputMaster.Player.Movement.performed += ctx => { 
                if(GetComponent<PlayerController>().IsDead == false) SetMovementSpeed(ctx.ReadValue<Vector2>()); 
            };
            inputMaster.Player.Movement.canceled += ctx => {
                if(GetComponent<PlayerController>().IsDead == false) SetMovementSpeed(ctx.ReadValue<Vector2>()); 
            };
            inputMaster.Player.Jump.performed += ctx => { if (GetComponent<PlayerController>().IsDead == false) doAJump = true; };
            inputMaster.Player.Jump.canceled += ctx => doAJump = false;
            inputMaster.Player.Crouch.performed += ctx => { photonView.RPC("Crouch", RpcTarget.All, true); };
            inputMaster.Player.Crouch.canceled += ctx => { photonView.RPC("Crouch", RpcTarget.All, false); };
        }

        
        


    }

    void Update()
    {   
        //update Animation
        UpdateAnimationSpeed();

        if(isTryingToStandUp && CanStandUp())
        {
            isTryingToStandUp = false;
            photonView.RPC("Crouch", RpcTarget.All, false);
        }
        
    }

    void FixedUpdate()
    {
        if (!GetComponent<Ragdoll>().isRagdoll)
        {
            
            // Calculate how fast we should be moving
            Vector3 targetVelocity = new Vector3(horizontalAxe, 0, verticalAxe);

            //permet d'augmenter la vitesse petit à petit
            previousTargetSpeed = Vector3.Lerp(previousTargetSpeed, targetVelocity, acceleration * Time.fixedDeltaTime);

            targetVelocity = transform.TransformDirection(previousTargetSpeed);
            targetVelocity *= speed;

            // Apply a force that attempts to reach our target velocity
            Vector3 velocity = GetComponent<Rigidbody>().velocity;
            Vector3 velocityChange = (targetVelocity - velocity);
            velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
            velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
            velocityChange.y = 0;



            if (grounded)
            {
                GetComponent<Animator>().SetBool("OnGround", true);
                GetComponent<Rigidbody>().AddForce(velocityChange, ForceMode.VelocityChange);
            }
            else
            {
                GetComponent<Animator>().SetBool("OnGround", false);
                GetComponent<Rigidbody>().AddForce(velocityChange, ForceMode.VelocityChange);
            }

            //get MaxVelocity
            if(MaxVelocity.sqrMagnitude < GetComponent<Rigidbody>().velocity.sqrMagnitude)
            {
                MaxVelocity = GetComponent<Rigidbody>().velocity;
            }

            
            


            // Jump
            if (canJump && doAJump && grounded)
            {
                GetComponent<Animator>().SetTrigger("Jump");
                //GetComponent<Rigidbody>().velocity = new Vector3(velocity.x, CalculateJumpVerticalSpeed(), velocity.z);
                GetComponent<Rigidbody>().AddForce(new Vector3(0f, CalculateJumpVerticalSpeed(), 0f), ForceMode.Impulse);
                doAJump = false;
            }
        }

        // We apply gravity manually for more tuning control
        GetComponent<Rigidbody>().AddForce(new Vector3(0, -gravity * GetComponent<Rigidbody>().mass, 0));

        grounded = false;
    }

    private void OnCollisionStay(Collision collision)
    {
        grounded = true;
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
        float yVel = Vector3.Dot(GetComponent<Rigidbody>().velocity, transform.forward);
        float xVel = Vector3.Dot(GetComponent<Rigidbody>().velocity, transform.right);

        if (isCrouching)
        {
            yVel /= 2;
            xVel /= 2;
        }

        GetComponent<Animator>().SetFloat("VelX", xVel / speed);
        GetComponent<Animator>().SetFloat("VelY", yVel / speed);
    }

    float CalculateJumpVerticalSpeed()
    {
        // From the jump height and gravity we deduce the upwards speed 
        // for the character to reach at the apex.
        return Mathf.Sqrt(2 * jumpHeight * gravity);
    }

    void SetMovementSpeed(Vector2 movement)
    {
        horizontalAxe = movement.x;
        verticalAxe = movement.y;
    }

    public void OnEnable()
    {
        if (!photonView.IsMine && PhotonNetwork.IsConnected == true)
        {
            return;
        }
        inputMaster.Enable();
    }

    public void OnDisable()
    {
        if (!photonView.IsMine && PhotonNetwork.IsConnected == true)
        {
            return;
        }
        inputMaster.Disable();
    }
}