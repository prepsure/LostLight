using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    public Vector3 OriginalPosition;

    // Start is called before the first frame update
    void Start()
    {
        OriginalPosition = GetComponent<Transform>().position;
    }

    void ResetPosition()
    {
        GetComponent<Transform>().position = OriginalPosition;
    }
}
