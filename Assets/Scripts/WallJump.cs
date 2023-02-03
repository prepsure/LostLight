using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;
using System;


[RequireComponent(typeof(Rigidbody))]
public class WallJump : MonoBehaviour
{
    private Rigidbody rb;

    private Vector3 force;

    private CombinedCharacterController characterController;

    private bool wallJump;

    private bool gravityReversed;

    private bool onWall;

    private Vector3 gravityDirection;

    private float time = 0.0f;

    [Range(0f, 15f)]
    [Tooltip("How much force you want the player to push off the wall with")]
    [SerializeField] private float jumpForce = 8;

    [Range(0f, 10f)]
    [Tooltip("How long do you want the player to stay stuck to wall before sliding")]
    [SerializeField] private float timeBeforeSlide = 1;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        characterController = GetComponent<CombinedCharacterController>();
        force = Vector3.zero;
        wallJump = false;
        onWall = false;
    }

    void Update()
    {
        if(onWall)
        {
            onWall = false;
            StartCoroutine(Slide());
        }
    }

    void OnCollisionEnter(Collision other)
    {
        characterController.airJumps = 0;
        if(Physics.gravity != Vector3.zero && Mathf.Sign(Physics.gravity.y) == -1)
        {
            gravityReversed = false;
        }
        else if(Physics.gravity != Vector3.zero && Mathf.Sign(Physics.gravity.y) == 1)
        {
            gravityReversed = true;
        }
        if (other.gameObject.transform.CompareTag("WallJump"))
        {
            characterController.airJumps++;
            RaycastHit hit;

            if (gravityReversed) gravityDirection = Vector3.up;
            else gravityDirection = Vector3.down;
            if (Physics.Raycast(transform.position, -1 * gravityDirection, out hit, 1.25f))
            {

                force = Vector3.zero;
                characterController.airJumps = 0;
            }
            else if (Physics.Raycast(transform.position, gravityDirection, out hit, 1.25f))
            {

                force = Vector3.zero;
                characterController.airJumps = 0;
            }
            else if (Physics.Raycast(transform.position, Vector3.right, out hit, 0.75f))
            {

                force = (-1 * gravityDirection + Vector3.left) * jumpForce;
                wallJump = true;
                onWall = true;
            }
            else if (Physics.Raycast(transform.position, Vector3.left, out hit, 0.75f))
            {

                force = (-1 * gravityDirection + Vector3.right) * jumpForce;
                wallJump = true;
                onWall = true;
            }
        }
        else
        {
            force = Vector3.zero;
            characterController.airJumps = 0;
        }
    }

    void OnCollisionExit(Collision other)
    {
        if (Input.GetKey(characterController.jumpKey) && other.gameObject.transform.CompareTag("WallJump"))
        {
            if (wallJump) rb.velocity = Vector3.zero;
            rb.AddForce(force, ForceMode.VelocityChange);
            wallJump = false;
        }
        else if (other.gameObject.transform.CompareTag("WallJump"))
        {
            characterController.airJumps = 0;
            wallJump = false;
        }
    }

    IEnumerator Slide()
    {
        if (timeBeforeSlide == 10) timeBeforeSlide = float.PositiveInfinity;
        do
        {
            rb.velocity = Vector3.zero;
            Physics.gravity = Vector3.zero;
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        } while (time < timeBeforeSlide && rb.velocity == Vector3.zero);
        Physics.gravity = (gravityDirection) * 9.8f;
        time = 0.0f;
    }

}
