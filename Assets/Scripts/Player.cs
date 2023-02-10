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

    private float popOutDistance = 29;
    private bool yes = true;

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
        if (dir == 0 || _camera.GetComponent<CameraTween>()._active || !yes)
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

        Vector3 camDir = GetCameraLookUnit();

        RaycastHit platformStandingOn;

        Physics.Raycast(transform.position, -Vector3.up, out platformStandingOn);

        Vector3 origPos = platformStandingOn.collider.gameObject.GetComponent<ArbitraryDataScript>()._originalPosition;

        _transform.position = Vector3.Scale(transform.position, GetCameraPlaneVector()) + Vector3.Scale(makepositive(GetCameraLookUnit()), origPos);
    }

    void PutPlayerIn2DWorld()
    {
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

    void ChangePlatforms()
    {
        PopOutPlatform[] plats = FindObjectsOfType<PopOutPlatform>();
        Transform _cameraTrans = _camera.GetComponent<Transform>();

        //Array.ForEach(plats, p => p.PopOut(-GetCameraLookUnit(), _cameraTrans.position.magnitude - 1));
    }
 }
