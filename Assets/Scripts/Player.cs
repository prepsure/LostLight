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

    private float _characterSinkLeeway;

    private Rigidbody _body;
    private Collider _collider;
    private Transform _transform;
    private Camera _camera;
    private Platform _currentPlatform;

    private float _camDebounce = 0;

    // Start is called before the first frame update
    void Start()
    {
        _body = GetComponent<Rigidbody>();
        _collider = GetComponent<CapsuleCollider>();
        _transform = GetComponent<Transform>();
        _camera = FindObjectOfType<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        SetMoveVector(horizontalInput);

        if(Input.GetKeyDown(KeyCode.Space))
        {
            TryToJump();
        }

        float cameraChange = Input.GetAxis("CameraTurn");
        TurnCamera(Math.Sign(cameraChange));
        _camDebounce -= Time.deltaTime;

        LimitYVelocity();
    }

    bool IsGrounded() {
        return Physics.Raycast(transform.position, -Vector3.up, (float) (_collider.bounds.extents.y + 0.1));
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
        Vector3 moveAxis = Vector3.Cross(GetCameraLookUnit(), Vector3.down);

        Debug.Log(moveAxis);

        Vector3 potentialMove = _playerSpeed * moveDir * moveAxis + new Vector3(0, _body.velocity.y, 0);
        _body.velocity = potentialMove;
    }

    void LimitYVelocity()
    {
        _body.velocity = new Vector3(_body.velocity.x, Math.Min(_jumpPower, _body.velocity.y), _body.velocity.z);
    }

    // dir should be -1, 0, 1
    void TurnCamera(int dir)
    {
        if (dir == 0 || _camDebounce > 0)
        {
            return;
        }

        _camDebounce = 0.5f;


        Transform transCamera = _camera.GetComponent<Transform>();
        Vector3 v = transCamera.position;

        Vector3.Cross(v, Vector3.up);

        transCamera.position = Vector3.Cross(v, dir * Vector3.up);
        transCamera.LookAt(Vector3.zero, Vector3.up);

        _transform.LookAt(_transform.position - GetCameraLookUnit(), Vector3.up);
    }

    Vector3 GetCameraLookUnit()
    {
        Transform transCamera = _camera.GetComponent<Transform>();
        return transCamera.forward.normalized;
    }

    void ChangePlatforms()
    {
        PopOutPlatform[] plats = FindObjectsOfType<PopOutPlatform>();


    }
 }
