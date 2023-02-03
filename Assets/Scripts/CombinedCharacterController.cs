using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CombinedCharacterController : MonoBehaviour
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

    private Rigidbody body;

    private Camera cam;
    private bool desiredJump;

    private Direction direction = Direction.Right;

    private Vector3 playerInput;


    private bool setGroundedOverride;
    private Quaternion to = Quaternion.identity;
    private bool OnGround { get {
            double yDist = body.GetComponent<CapsuleCollider>().height / 2 + 0.1;
            Vector3 DOWN = new(0, -1, 0);
            return Physics.Raycast(body.GetComponent<Transform>().position, DOWN, (float)yDist, 6);
        }
    }

    protected internal bool OverrideOnGround { private get; set; } = true;


    // Start is called before the first frame update
    private void Start()
    {
        cam = Camera.main;
        body = GetComponent<Rigidbody>();
        to = Quaternion.Euler(0, 0, 180);

        Quaternion rot = transform.rotation;
        transform.rotation = Quaternion.Euler(rot.eulerAngles.x, rot.eulerAngles.y + 90,
            rot.eulerAngles.z);
    }

    // Stay on ground stuff
    private void Update()
    {
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
        MovePlayer();

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

        Debug.DrawRay(transform.position, Physics.gravity);
        body.AddForce(Physics.gravity); // Todo: shouldn't need this call
    }


    /// <summary>
    ///     Tells this character controller to not apply gravity
    ///     for a frame. Useful for wall jumps and grappling hooks.
    /// </summary>
    /// <param name="isGrounded"></param>
    public void SetGrounded(bool isGrounded)
    {
        setGroundedOverride = isGrounded;
    }


    private void FlipDirection()
    {
        Quaternion rot = transform.rotation;
        to = Quaternion.Euler(rot.eulerAngles.x, rot.eulerAngles.y + 180,
            rot.eulerAngles.z);
        body.MoveRotation(to);
    }

    private void MovePlayer()
    {
        Vector3 xAxis = new(1, 0, 0); // TODO
        Vector3 zAxis = new(0, 1, 0);
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
                    body.AddForce(Physics.gravity, ForceMode.Acceleration);
                }
            }

            if (playerInput.z == 0 && Mathf.Abs(body.velocity.z) > 0.01f)
            {
                if (!preciseMovement) body.AddForce(Vector3.forward * (-body.velocity.z * frictionSpeed), ForceMode.Acceleration);
                else
                {
                    body.velocity = new Vector3(body.velocity.x, body.velocity.y, 0);
                    body.AddForce(Physics.gravity, ForceMode.Acceleration);
                }
            }
        }
    }

    private void Jump()
    {
        Vector3 jumpDirection;
        if (!OnGround) return;

        float jumpSpeed = Mathf.Sqrt(Mathf.Abs(-2f * Physics.gravity.y) * jumpHeight * 2);
        jumpDirection = (Vector3.up).normalized;
        float alignedSpeed = Vector3.Dot(body.velocity, jumpDirection);
        if (alignedSpeed > 0f) jumpSpeed = Mathf.Max(jumpSpeed - alignedSpeed, 0f);


        body.AddForce(transform.up * jumpSpeed, ForceMode.VelocityChange);
    }

    private enum Direction
    {
        Right,
        Idle,
        Left
    }
}