using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    private Transform _transform;
    private BoxCollider _collider;

    public Vector3 Position { get
        {
            return _transform.position;
        } 
    }


    void Start()
    {
        _transform = GetComponent<Transform>();
        _collider = GetComponent<BoxCollider>();
    }

    public static Platform[] GetAllPlatforms()
    {
        return FindObjectsOfType<Platform>();
    }

    public Vector3 GetInsetCharacterPosition(Transform characterTransform, Transform cameraTransform)
    {
        return Vector3.zero;
    }

    public void SetCollidable(bool shouldCollide)
    {
        _collider.enabled = shouldCollide;
    }

    public float GetTopYValue()
    {
        return -1000;
    }

    public bool IsPointInFrontOf(Vector3 point, Transform cameraTransform)
    {
        return true;
    }

    public bool IsPointBehind(Vector3 point, Transform cameraTransform)
    {
        return true;
    }

    public bool IsPointAbove(Vector3 point)
    {
        return true;
    }
}