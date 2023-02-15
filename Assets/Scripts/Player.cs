using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float _playerSpeed;
    [SerializeField]
    private float _jumpPower;

    private Rigidbody _body;
    private Collider _collider;
    private Transform _transform;
    private Camera _camera;

    private float popOutDistance = PopOutPlatform.PopOutDistance;
    private bool yes = true;
    private Vector3 depthTracking = Vector3.zero;

    private GameObject lastPlatform;
    public Vector3 LastStandingPosition { get; private set; }
    public bool CanTurn = true;

    // Start is called before the first frame update
    void Start()
    {
        _body = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
        _transform = GetComponent<Transform>();
        _camera = FindObjectOfType<Camera>();

        PutPlayerIn2DWorld();
    }

    // Update is called once per frame
    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        SetMoveVector(horizontalInput);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            TryToJump();
        }

        float cameraChange = Input.GetAxis("CameraTurn");
        TurnCamera(Math.Sign(cameraChange));

        LimitYVelocity();

        _transform.LookAt(_transform.position - GetCameraLookUnit(), Vector3.up);

        /*if (!_camera.GetComponent<CameraTween>()._active)
        {
            checkForNewDepth();
        }*/

        LastStandingPosition = transform.position;

        RaycastHit platformStandingOn;
        Physics.Raycast(transform.position, -Vector3.up, out platformStandingOn);

        if (platformStandingOn.collider.gameObject != null)
        {
            lastPlatform = platformStandingOn.collider.gameObject;
        }
    }

    Vector3 getFeetPos()
    {
        return new Vector3(_transform.position.x, _collider.bounds.min.y + 0.1f, transform.position.z);
    }

    void checkForNewDepth()
    {
        if (Physics.Raycast(getFeetPos(), GetCameraLookUnit(), out RaycastHit hit))
        {
            Vector3 newDepth = Vector3.Scale(hit.point, makepositive(GetCameraLookUnit()));
            Vector3 oldDepth = Vector3.Scale(depthTracking, makepositive(GetCameraLookUnit()));

            Vector3 closest = getCloserTo(_transform.position, newDepth, oldDepth);

            depthTracking = Vector3.Scale(GetCameraPlaneVector(), depthTracking) + closest;
        }

        Debug.Log(depthTracking);
    }

    bool IsGrounded() {
        return Physics.Raycast(transform.position, -Vector3.up, (float)(_collider.bounds.extents.y + 0.1)) 
            || Physics.Raycast(new Vector3(_collider.bounds.max.x, transform.position.y, _collider.bounds.max.z), -Vector3.up, (float)(_collider.bounds.extents.y + 0.1))
            || Physics.Raycast(new Vector3(_collider.bounds.min.x, transform.position.y, _collider.bounds.min.z), -Vector3.up, (float)(_collider.bounds.extents.y + 0.1))
        ;
    }

    void TryToJump()
    {
        if (!IsGrounded())
        {
            return;
        }

        _body.velocity = new Vector3(_body.velocity.x, _jumpPower, _body.velocity.z);
    }

    void SetMoveVector(float moveDir)
    {
        Vector3 moveAxis = GetMoveUnit();

        Vector3 potentialMove = _playerSpeed * moveDir * moveAxis + new Vector3(0, _body.velocity.y, 0);
        _body.velocity = potentialMove;
    }

    void LimitYVelocity()
    {
        _body.velocity = new Vector3(_body.velocity.x, Math.Min(_body.velocity.y, _jumpPower), _body.velocity.z);
    }

    // dir should be -1, 0, 1
    void TurnCamera(int dir)
    {
        if (!CanTurn)
        {
            return;
        }

        if (dir == 0)
        {
            return;
        }

        if (_camera.GetComponent<CameraTween>()._active)
        {
            return;
        }

        if (!yes)
        {
            return;
        }

        yes = false;

        // restore the player to the real world
        PutPlayerIn3DWorld();

        // then move the camera
        Transform transCamera = _camera.GetComponent<Transform>();
        Vector3 v = transCamera.position;

        Vector3.Cross(v, Vector3.up);

        _camera.GetComponent<CameraTween>()._cameraGoal = Vector3.Cross(v, dir * Vector3.up); //+ new Vector3(0, _transform.position.y, 0);
        _camera.GetComponent<CameraTween>()._active = true;

        StartCoroutine(FinishCamTurn());
    }

    IEnumerator FinishCamTurn()
    {
        while (_camera.GetComponent<CameraTween>()._active)
        {
            yield return null;
        }

        PutPlayerIn2DWorld();

        yes = true;
    }

    void PutPlayerIn3DWorld()
    {
        _body.constraints = RigidbodyConstraints.FreezeAll;

        Vector3 origPos = lastPlatform.GetComponent<ArbitraryDataScript>()._originalPosition;

        _transform.position = Vector3.Scale(transform.position, GetCameraPlaneVector()) 
            + Vector3.Scale(makepositive(GetCameraLookUnit()), origPos); // depthTracking);
    }

    void PutPlayerIn2DWorld()
    {
        depthTracking = _transform.position;

        _body.constraints = RigidbodyConstraints.FreezeRotation;

        Vector3 camDir = GetCameraLookUnit();

        _transform.position = Vector3.Scale(transform.position, GetCameraPlaneVector()) + -camDir * popOutDistance;
    }

    Vector3 GetCameraLookUnit()
    {
        Transform transCamera = _camera.GetComponent<Transform>();
        return transCamera.forward.normalized;
    }

    Vector3 GetCameraPlaneVector()
    { 
        Transform transCamera = _camera.GetComponent<Transform>();
        Vector3 raw = transCamera.right.normalized + transCamera.up.normalized;
        return makepositive(raw);
    }

    Vector3 GetMoveUnit()
    {
        return Vector3.Cross(GetCameraLookUnit(), Vector3.down);
    }

    Vector3 makepositive(Vector3 raw)
    {
        return new Vector3(Mathf.Abs(raw.x), Mathf.Abs(raw.y), Mathf.Abs(raw.z));
    }

    Vector3 getCloserTo(Vector3 closerTo, Vector3 a, Vector3 b)
    {
        return (closerTo - a).magnitude > (closerTo - b).magnitude ? b : a;
    }
 }
