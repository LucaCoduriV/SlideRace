using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class rigidbodyController : MonoBehaviourPunCallbacks
{


    #region Public Fields

    public Rigidbody body;
    public LayerMask groundMask;

    public float groundDistance = 0.4f;
    public float speed = 500.0f;
    public float maxVelocityChange = 10.0f;
    public bool canJump = true;
    public float jumpHeight = 2.0f;

    public float slopeForce = 10f;
    public float slopeForceRayLenght;
    public bool isOnSlope = false;

    public float maxStepHeight = 0.4f;        // The maximum a player can set upwards in units when they hit a wall that's potentially a step
    public float stepSearchOvershoot = 0.01f; // How much to overshoot into the direction a potential step in units when testing. High values prevent player from walking up tiny steps but may cause problems.

    public InputMaster inputMaster;

    #endregion
     
    #region Private Fields

    float horizontalAxe;
    float verticalAxe;
    bool isGrounded;
    List<ContactPoint> allCPs = new List<ContactPoint>();
    ContactPoint groundCP;
    private Vector3 lastVelocity;
    [SerializeField] private PlayerController player;
    private bool doAJump = false;

    #endregion


    private void Awake()
    {
        body.freezeRotation = true;

        if (photonView.IsMine)
        {
            inputMaster = new InputMaster();

            inputMaster.Player.Movement.performed += ctx => SetMovementSpeed(ctx.ReadValue<Vector2>());
            inputMaster.Player.Movement.canceled += ctx => SetMovementSpeed(ctx.ReadValue<Vector2>());
            inputMaster.Player.Jump.performed += ctx => doAJump = true;
            inputMaster.Player.Jump.canceled += ctx => doAJump = false;
        }
        
    }

    void SetMovementSpeed(Vector2 movement)
    {
        horizontalAxe = movement.x;
        verticalAxe = movement.y;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //horizontalAxe = Input.GetAxis("Horizontal");
        //verticalAxe = Input.GetAxis("Vertical");
    }

    private void FixedUpdate()
    {
        //vitesse avant d'entrer en colision avec une marche d'escalier
        Vector3 velocity = this.GetComponent<Rigidbody>().velocity;


        isGrounded = FindGround(out groundCP, allCPs);

        Vector3 stepUpOffset = default(Vector3);
        bool stepUp = false;
        if (isGrounded)
        {
            stepUp = FindStairs(out stepUpOffset, allCPs, groundCP);
        }

        //Steps
        if (stepUp)
        {
            this.GetComponent<Rigidbody>().position += stepUpOffset;
            this.GetComponent<Rigidbody>().velocity = lastVelocity;
        }

        allCPs.Clear();
        lastVelocity = velocity;

        if (!player.IsDead)
        {

            movement();
        }
        

        
    }

    private void movement()
    {
        // Calculate how fast we should be moving
        Vector3 targetVelocity = new Vector3(horizontalAxe, 0, verticalAxe);
        targetVelocity = transform.TransformDirection(targetVelocity);
        targetVelocity *= speed * Time.deltaTime;

        // Apply a force that attempts to reach our target velocity
        Vector3 velocity = body.velocity;
        Vector3 velocityChange = (targetVelocity - velocity);
        velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
        velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
        velocityChange.y = 0;
        body.AddForce(velocityChange, ForceMode.VelocityChange);


        
        isOnSlope = onSlope();
        if ((horizontalAxe != 0 || verticalAxe != 0) && isOnSlope == true)
        {
            body.AddForce(Vector3.down * slopeForce);
        }

        //jump
        if (canJump && isGrounded && doAJump)
        {
            body.velocity = new Vector3(velocity.x, CalculateJumpVerticalSpeed(), velocity.z);
        }
    }

    private void Jump()
    {
        if (canJump && isGrounded)
        {
            body.velocity = new Vector3(body.velocity.x, CalculateJumpVerticalSpeed(), body.velocity.z);
        }
    }

    
    private float CalculateJumpVerticalSpeed()
    {
        // From the jump height and gravity we deduce the upwards speed 
        // for the character to reach at the apex.
        return Mathf.Sqrt(2 * jumpHeight * Mathf.Abs(Physics.gravity.y));
    }

    private bool onSlope()
    {
        if (!isGrounded)
        {
            return false;
        }

        RaycastHit hit;

        

        if(Physics.Raycast(transform.position,Vector3.down,out hit,GetComponent<BoxCollider>().bounds.size.y / 2 * slopeForceRayLenght))
        {
            //checker si on se trouve bien sur un ground
            if(((1 << hit.transform.gameObject.layer) & groundMask) != 0)
            {
                //verifier que le sol n'est pas droit
                if (hit.normal != Vector3.up)
                {
                    Debug.DrawRay(transform.position, Vector3.down * slopeForceRayLenght, Color.green);
                    return true;
                }
            }
            
        }

        Debug.DrawRay(transform.position, Vector3.down * slopeForceRayLenght);
        return false;
    }

    bool FindGround(out ContactPoint groundCP, List<ContactPoint> allCps)
    {
        groundCP = default(ContactPoint);
        bool found = false;

        foreach (ContactPoint contact in allCps)
        {
            if(contact.normal.y > 0.0001f && (found == false || contact.normal.y > groundCP.normal.y)){
                groundCP = contact;
                found = true;
            }
        }


        return found;
    }

    bool FindStairs(out Vector3 stepUpOffset, List<ContactPoint> allCps, ContactPoint groundCP)
    {
        stepUpOffset = default(Vector3);

        //vérifier si le joueur bouge
        Vector2 velocityXZ = new Vector2(body.velocity.x, body.velocity.z);
        if(velocityXZ.sqrMagnitude < 0.001f)
        {
            return false;
        }

        foreach (ContactPoint contact in allCPs)
        {
            bool test = ResolveStepUp(out stepUpOffset, contact, groundCP);
            if (test)
            {
                Debug.Log("ESCALIER !!");
                return test;
            }
        }
        return true;
    }

    /// Takes a contact point that looks as though it's the side face of a step and sees if we can climb it
    /// \param stepTestCP ContactPoint to check.
    /// \param groundCP ContactPoint on the ground.
    /// \param stepUpOffset The offset from the stepTestCP.point to the stepUpPoint (to add to the player's position so they're now on the step)
    /// \return If the passed ContactPoint was a step
    bool ResolveStepUp(out Vector3 stepUpOffset, ContactPoint stepTestCP, ContactPoint groundCP)
    {
        stepUpOffset = default(Vector3);
        Collider stepCol = stepTestCP.otherCollider;

        //( 1 ) Check if the contact point normal matches that of a step (y close to 0)
        if (Mathf.Abs(stepTestCP.normal.y) >= 0.01f)
        {
            return false;
        }

        //( 2 ) Make sure the contact point is low enough to be a step
        if (!(stepTestCP.point.y - groundCP.point.y < maxStepHeight))
        {
            return false;
        }

        //( 3 ) Check to see if there's actually a place to step in front of us
        //Fires one Raycast
        RaycastHit hitInfo;
        float stepHeight = groundCP.point.y + maxStepHeight + 0.0001f;
        Vector3 stepTestInvDir = new Vector3(-stepTestCP.normal.x, 0, -stepTestCP.normal.z).normalized;
        Vector3 origin = new Vector3(stepTestCP.point.x, stepHeight, stepTestCP.point.z) + (stepTestInvDir * stepSearchOvershoot);
        Vector3 direction = Vector3.down;
        if (!(stepCol.Raycast(new Ray(origin, direction), out hitInfo, maxStepHeight)))
        {
            return false;
        }

        //We have enough info to calculate the points
        Vector3 stepUpPoint = new Vector3(stepTestCP.point.x, hitInfo.point.y + 0.0001f, stepTestCP.point.z) + (stepTestInvDir * stepSearchOvershoot);
        Vector3 stepUpPointOffset = stepUpPoint - new Vector3(stepTestCP.point.x, groundCP.point.y, stepTestCP.point.z);

        //We passed all the checks! Calculate and return the point!
        stepUpOffset = stepUpPointOffset;
        return true;
    }


    void OnCollisionEnter(Collision col)
    {
        allCPs.AddRange(col.contacts);
    }
    void OnCollisionStay(Collision col)
    {
        allCPs.AddRange(col.contacts);
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
