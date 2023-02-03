using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CombinedCharacterController1 : MonoBehaviour
{
    [Header("Platformer or Top Down Mechanics?")]
    [Tooltip(
        "If true, act as a front-facing controller locked in the XY plane. If false, act as a top-down controller.")]
    public bool lockToXY;

    public KeyCode jumpKey = KeyCode.Space;

    [Header("Speed and physics controls:")] [Range(0f, 45f)] [Tooltip("Max speeds of each ability.")]
    public float maxSpeed = 10f;


    [Range(0f, 5f)] [Tooltip("Max acceleration while walking.")]
    public float maxAcceleration = 1f;

    [Range(0f, 5f)] [Tooltip("Max acceleration while in the air.")]
    public float maxAirAcceleration = 1f;

    [Range(0f, 10f)] [Tooltip("Max jump height.")]
    public float jumpHeight = 2f;

    [Range(0, 5)] [Tooltip("Number of 'double-jumps'.")]
    public int airJumps;

    [Range(-1, 10)] [Tooltip("Friction for the Player on the Ground'.")]
    public float frictionSpeed;

    [Tooltip("Turn on if you want the Player to stop on a dime when moving on the ground")]
    public bool preciseMovement;

    /*    [Header("Ramp controls:")] [Range(0, 90)] [Tooltip("Angle at which the player begins to slide down the slope.")]*/
    private float maxSlopeAngle = 50f;


    /*    [Range(0f, 100f)] [Tooltip("How tight to keep the player to the surface.")]*/
    private float maxSnapSpeed = 15f;

    /*    [Min(0f)] [Tooltip("Adjust based on height of player to keep player touching the surface.")]*/
    private float snapToGroundProbeDistance = 2f;

    private float snapForce = 5;

    private readonly LayerMask probeMask = -1;
    private readonly float rotationSpeed = 720f;
    private Rigidbody body;

    private Camera cam;
    private Vector3 contactNormal, steepNormal;
    private bool desiredJump;

    private Direction direction = Direction.Right;

    // The current direction of gravity
    private float gravityDirection;
    private int groundContactCount, steepContactCount;

    private int jumpPhase;

    private Vector3 lookAt;

    private float minGroundDotProduct;
    private Vector3 playerInput;


    private bool setGroundedOverride;
    private int stepsSinceLastGrounded, stepsSinceLastJump;
    private Quaternion to = Quaternion.identity;
    private bool OnGround => groundContactCount > 0;
    private bool OnSteep => steepContactCount > 0;

    protected internal bool OverrideOnGround { private get; set; } = true;


    // Start is called before the first frame update
    private void Start()
    {
        cam = Camera.main;
        body = GetComponent<Rigidbody>();
        to = Quaternion.Euler(0, 0, 180);

        if (lockToXY)
        {
            Quaternion rot = transform.rotation;
            transform.rotation = Quaternion.Euler(rot.eulerAngles.x, rot.eulerAngles.y + 90,
                rot.eulerAngles.z);
        }
    }

    // Stay on ground stuff
    private void Update()
    {
        // Todo: remove for performance later
        minGroundDotProduct = Mathf.Cos(maxSlopeAngle * Mathf.Deg2Rad);

        playerInput.x = Input.GetAxisRaw("Horizontal");
        playerInput.z = Input.GetAxisRaw("Vertical");
        playerInput = Vector3.ClampMagnitude(playerInput, 1f);


        // Is the player operating in "Platform" mode?
        if (lockToXY)
        {
            playerInput.z = 0;
            switch (direction)
            {
                case Direction.Left:
                    if (playerInput.x > 0)
                    {
                        direction = Direction.Right;
                        FlipDirection();
                    }

                    break;
                case Direction.Right:
                    if (playerInput.x < 0)
                    {
                        direction = Direction.Left;
                        FlipDirection();
                    }

                    break;
            }
        }


        desiredJump |= Input.GetKeyDown(jumpKey);
    }


    private void FixedUpdate()
    {
        UpdateState();
        MovePlayer();
        // Get the current direction of gravity
        gravityDirection = Mathf.Sign(Physics.gravity.y);

        if (desiredJump)
        {
            desiredJump = false;
            Jump();
        }


        // setGroundedOverride = false;


        Vector3 velocity = body.velocity;
        Vector3 scaledVelocity = Vector3.ClampMagnitude(new Vector3(velocity.x, 0, velocity.z), maxSpeed);
        scaledVelocity.y = body.velocity.y;
        if (setGroundedOverride) scaledVelocity.y = 0;

        body.velocity = scaledVelocity;


        // Control player rotation when operating in "Top-Down" mode
        if (!lockToXY) RotateTowardsMouse();


        ClearState();

        Debug.DrawRay(transform.position, Physics.gravity);
        body.AddForce(Physics.gravity); // Todo: shouldn't need this call
    }

    private void OnCollisionEnter(Collision collision) => EvaluateCollision(collision);


    private void OnCollisionStay(Collision collision) => EvaluateCollision(collision);


    /// <summary>
    ///     Tells this character controller to not apply gravity
    ///     for a frame. Useful for wall jumps and grappling hooks.
    /// </summary>
    /// <param name="isGrounded"></param>
    public void SetGrounded(bool isGrounded)
    {
        setGroundedOverride = isGrounded;
    }

    private void RotateTowardsMouse()
    {
        // Get rotation from mouse position
        Vector3 p = Input.mousePosition;
        p.z = 20;
        Ray ray = cam.ScreenPointToRay(p);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            lookAt = hit.point;
            lookAt.y = 0;
        }

        Vector3 movementDirection = lookAt - transform.position;
        movementDirection.y = 0;
        movementDirection.Normalize();

        if (movementDirection != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(movementDirection, Vector3.up);
            Quaternion rot = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
            body.MoveRotation(rot);
        }
    }

    private void ClearState()
    {
        groundContactCount = steepContactCount = 0;
        contactNormal = steepNormal = Vector3.zero;
    }


    private void FlipDirection()
    {
        Quaternion rot = transform.rotation;
        to = Quaternion.Euler(rot.eulerAngles.x, rot.eulerAngles.y + 180,
            rot.eulerAngles.z);
        body.MoveRotation(to);
    }


    private void UpdateState()
    {
        stepsSinceLastGrounded += 1;
        stepsSinceLastJump += 1;
        if (OnGround || SnapToGround() || CheckSteepContacts())
        {
            stepsSinceLastGrounded = 0;
            if (stepsSinceLastJump > 1) jumpPhase = 0;

            if (groundContactCount > 1) contactNormal.Normalize();
        }
        else
            contactNormal = Vector3.up;
    }

    private bool SnapToGround()
    {
          // here's the issue for jetpack with slopes
          if (OverrideOnGround) return false;

          if (stepsSinceLastGrounded > 1 || stepsSinceLastJump <= 2) return false;

          float speed = body.velocity.magnitude;
          if (speed > maxSnapSpeed) return false;

          if (!Physics.Raycast(
                  body.position, Vector3.down, out RaycastHit hit,
                  snapToGroundProbeDistance * -gravityDirection, probeMask
              ))
              return false;

          if (hit.normal.y < GetMinDot(hit.collider.gameObject.layer)) return false;

          groundContactCount = 1;
          contactNormal = hit.normal;
          float dot = Vector3.Dot(body.velocity, hit.normal);
          if (dot > 0f) body.AddForce(Physics.gravity * snapForce, ForceMode.Acceleration);

          return true;
        return false;
    }

    private bool CheckSteepContacts()
    {
        if (steepContactCount > 1)
        {
            steepNormal.Normalize();
            if (steepNormal.y >= minGroundDotProduct)
            {
                steepContactCount = 0;
                groundContactCount = 1;
                contactNormal = steepNormal;
                return true;
            }
        }
        
        return false;
    }

    private void MovePlayer()
    {
        Vector3 xAxis = ProjectOnContactPlane(Vector3.right).normalized;
        Vector3 zAxis = ProjectOnContactPlane(Vector3.forward).normalized;
        Vector3 position = transform.position;
        Debug.DrawRay(position, xAxis * 2, Color.green);
        Debug.DrawRay(position, zAxis * 2, Color.green);

        xAxis = Vector3.right;
        zAxis = Vector3.forward;

        // print(xAxis);
        float acceleration = OnGround ? maxAcceleration : maxAirAcceleration;
        Vector3 a = (xAxis * playerInput.x + zAxis * playerInput.z) * (acceleration * 2);
        Debug.DrawRay(transform.position, a * 2, Color.red);

        body.AddForce(a, ForceMode.VelocityChange);

        if (OnGround)
        {
            if (playerInput.x == 0 && Mathf.Abs(body.velocity.x) > 0.01f)
            {
                if (!preciseMovement) body.AddForce(Vector3.right * (-body.velocity.x * frictionSpeed), ForceMode.Acceleration);
                else
                {
                    body.velocity = new Vector3(0, body.velocity.y, body.velocity.z);
                    body.AddForce(Physics.gravity * snapForce, ForceMode.Acceleration);
                }
            }

            if (playerInput.z == 0 && Mathf.Abs(body.velocity.z) > 0.01f)
            {
                if (!preciseMovement) body.AddForce(Vector3.forward * (-body.velocity.z * frictionSpeed), ForceMode.Acceleration);
                else
                {
                    body.velocity = new Vector3(body.velocity.x, body.velocity.y, 0);
                    body.AddForce(Physics.gravity * snapForce, ForceMode.Acceleration);
                }
            }
        }
    }

    private void Jump()
    {
        Vector3 jumpDirection;
        if (OnGround)
            jumpDirection = contactNormal;
/*        else if (OnSteep)
        {
            jumpDirection = steepNormal;
            jumpPhase = 0;
        }*/
        else if (airJumps > 0 && jumpPhase <= airJumps)
        {
            if (jumpPhase == 0) jumpPhase = 1;

            jumpDirection = contactNormal;
        }
        else
            return;


        stepsSinceLastJump = 0;
        jumpPhase += 1;
        float jumpSpeed = Mathf.Sqrt(Mathf.Abs(-2f * Physics.gravity.y) * jumpHeight * 2);
        jumpDirection = (jumpDirection + Vector3.up).normalized;
        float alignedSpeed = Vector3.Dot(body.velocity, jumpDirection);
        if (alignedSpeed > 0f) jumpSpeed = Mathf.Max(jumpSpeed - alignedSpeed, 0f);


        body.AddForce(transform.up * jumpSpeed, ForceMode.VelocityChange);
    }

    private void EvaluateCollision(Collision collision)
    {
        float minDot = GetMinDot(collision.gameObject.layer);
        for (var i = 0; i < collision.contactCount; i++)
        {
            Vector3 normal = collision.GetContact(i).normal;

            if (gravityDirection > 0)
            {
                if (normal.y <= -minDot)
                {
                    groundContactCount += 1;
                    contactNormal += normal;
                }
                else if (normal.y > -0.01f)
                {
                    steepContactCount += 1;
                    steepNormal += normal;
                }
            }
            else
            {
                if (normal.y >= minDot)
                {
                    groundContactCount += 1;
                    contactNormal += normal;
                }
                else if (normal.y > -0.01f)
                {
                    steepContactCount += 1;
                    steepNormal += normal;
                }
            }
        }
    }

    private Vector3 ProjectOnContactPlane(Vector3 vector) =>
        vector - contactNormal * Vector3.Dot(vector, contactNormal);

    private float GetMinDot(int layer) => minGroundDotProduct;

    private enum Direction
    {
        Right,
        Idle,
        Left
    }
}