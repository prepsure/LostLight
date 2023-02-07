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
        // TODO
        return Vector3.zero;
    }

    public void SetCollidable(bool shouldCollide)
    {
        _collider.enabled = shouldCollide;
    }

    public float GetTopYValue()
    {
        return _collider.bounds.max.y;
    }

    public bool IsPointInFrontOf(Vector3 point, Transform cameraTransform)
    {
        // TODO
        return true;
    }

    public bool IsPointBehind(Vector3 point, Transform cameraTransform)
    {
        // TODO
        return true;
    }

    public bool IsPointAbove(Vector3 point)
    {
        // TODO
        return true;
    }
}