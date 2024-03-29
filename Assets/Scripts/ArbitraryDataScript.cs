using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArbitraryDataScript : MonoBehaviour
{
    [SerializeField]
    public Vector3 _originalPosition;

    [SerializeField]
    public Transform _cameraGoal;

    [SerializeField]
    public bool _doesCollide;

    [SerializeField]
    public bool _isLimbo;

    public bool _isLimboAndNot;
    public Vector3 _lookVect;
}
