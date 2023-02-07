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

    // Start is called before the first frame update
    void Start()
    {
        _body = GetComponent<Rigidbody>();
        _collider = GetComponent<MeshCollider>();
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

        LimitYVelocity();
        UpdatePlatform();

        Platform plat = _currentPlatform;
        if (plat)
        {
            /*_transform.SetPositionAndRotation(
                plat.GetInsetCharacterPosition(
                    _transform,
                    _camera.GetComponent<Transform>()
                ), 
                _transform.rotation
            );*/
        }
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

        _body.velocity = new Vector3(_body.velocity.x, _jumpPower, 0);
    }

    void SetMoveVector(float moveDir)
    {
        Vector3 potentialMove = new Vector3(moveDir * _playerSpeed, _body.velocity.y, 0);
        _body.velocity = potentialMove;
    }

    void LimitYVelocity()
    {
        _body.velocity = new Vector3(_body.velocity.x, Math.Min(_jumpPower, _body.velocity.y), 0);
    }
    
    Vector3[] GetFeetLocations()
    {
        // TODO
        Vector3 corner1 = _collider.bounds.min;
        Vector3 corner2 = new(_collider.bounds.max.x, corner1.y, corner1.z);
        return new Vector3[]{ corner1, corner2};
    }

    void UpdatePlatform()
    {
        Platform[] allPlats = Platform.GetAllPlatforms();
        Transform cameraTransform = _camera.GetComponent<Transform>();

        _currentPlatform = null;
        Platform tallestUsablePlat = null;
        
        foreach (Vector3 feetPos in GetFeetLocations())
        {
            foreach (Platform plat in allPlats)
            {
                // true by default, set to false if needed
                plat.SetCollidable(true);

                if (tallestUsablePlat && (plat.GetTopYValue() < tallestUsablePlat.GetTopYValue()))
                { // is it taller than the one i already have?
                    continue;
                }

                if (
                    tallestUsablePlat &&
                    plat.GetTopYValue() == tallestUsablePlat.GetTopYValue() &&
                    !tallestUsablePlat.IsPointInFrontOf(plat.Position, cameraTransform) // if a platform is behind the current one
                ) {
                    continue;
                }

                //local feetPos = Character:GetFeetPos()

                if (plat.GetTopYValue() > feetPos.y + _characterSinkLeeway)
                { // is it under feet?
                    Debug.Log("hi");
                    plat.SetCollidable(false);
                    continue;
                }

                if (!(
                    plat.IsPointInFrontOf(feetPos, cameraTransform) ||
                    plat.IsPointBehind(feetPos, cameraTransform) ||
                    plat.IsPointAbove(feetPos)
                ))
                {
                    continue;
                }

                tallestUsablePlat = plat;
            }
        }

        _currentPlatform = tallestUsablePlat;
    }
 }
