using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
// [RequireComponent(typeof(ParticleSystem))]
[RequireComponent(typeof(CombinedCharacterController))]
public class JetpackAbility : MonoBehaviour
{
    public float fuel = 5;
    public float burnRatePerSecond = 2;

    [Range(0f, 5f)]
    [Tooltip("How fast you acclerate with the Jetpack")]
    [SerializeField] private float acceleration = 2;

    [Range(0f, 25f)] [Tooltip("How fast you move with the Jetpack")]
    [SerializeField] private float maxSpeed = 5;

    public KeyCode jetpackKey = KeyCode.LeftShift;

    private CombinedCharacterController characterController;

    private float fuelRemaining;

    private Rigidbody rb;

    private ParticleSystem particles;


    // Start is called before the first frame update
    private void Start()
    {
        characterController = GetComponent<CombinedCharacterController>();
        particles = GetComponentInChildren<ParticleSystem>();
        if(particles) particles.Stop();
        rb = GetComponent<Rigidbody>();
        fuelRemaining = fuel;
    }

    private void FixedUpdate()
    {
        // Get the current direction of gravity
        if (Input.GetKey(jetpackKey) && fuelRemaining > 0)
        {
            var force = Vector3.up * acceleration;


            // Jetpack pushes against gravity, no matter the direction gravity currently is
            var gravityDirection = Mathf.Sign(Physics.gravity.y);
            force.y *= -gravityDirection;

            if ((-gravityDirection * rb.velocity.y) < maxSpeed)
            {
                rb.AddForce(force, ForceMode.VelocityChange);
                characterController.OverrideOnGround = true;
            }


            // Use fuel
            fuelRemaining -= burnRatePerSecond * Time.deltaTime;
        }

    }

    private void Update()
    {
        if(Input.GetKeyDown(jetpackKey))
        {
            particles.Play();
        }
        if(Input.GetKeyUp(jetpackKey))
        {
            particles.Stop();
        }
    }

    /// <summary>
    ///     A simple HUD readout to display jetpack fuel level.
    /// </summary>
    private void OnGUI()
    {
        GUI.Box(new Rect(0, 0, 100, 30), "Jetpack: " + Mathf.Clamp(fuelRemaining,0,fuel).ToString("n2"));
    }

    private void OnCollisionStay(Collision collisionInfo)
    {
        // When we land on the ground, refuel
        // Make sure we are colliding on the bottom, not the sides
        var raycastDirection = Vector3.up * Mathf.Sign(Physics.gravity.y);
        if (Physics.Raycast(transform.position, raycastDirection, out var hit, 1.5f))
        {
            characterController.OverrideOnGround = false;
            fuelRemaining = fuel;
        }
    }
}