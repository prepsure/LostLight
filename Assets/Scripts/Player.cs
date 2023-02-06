using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private CharacterController _controller;
    private float _yVelocity;

    [SerializeField]
    private float _gravity;
    [SerializeField]
    private float _playerSpeed;
    [SerializeField]
    private float _jumpHeight;

    private event Action OnJumpRequest;
    private event Action<Vector3> OnMoveRequest;
    private event Action OnGrappleRequest;

    // Start is called before the first frame update
    void Start()
    {
        _controller = GetComponent<CharacterController>();

        OnMoveRequest += (Vector3 direction) =>
        {
            _controller.Move(_playerSpeed * Time.deltaTime * direction);
        };

        OnJumpRequest += () =>
        {

        };
    }

    // Update is called once per frame
    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");

        if(horizontalInput != 0)
        {
            Vector3 direction = new(horizontalInput, 0, 0);
            OnMoveRequest.Invoke(direction);
        }

        _controller.Move(new Vector3(0, -_gravity, 0) * Time.deltaTime);
    }
}
