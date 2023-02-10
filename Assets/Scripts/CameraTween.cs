using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTween : MonoBehaviour
{
    [SerializeField]
    public bool _active;

    [SerializeField]
    public Vector3 _cameraGoal;

    private Vector3 cameraStart;
    private float t = 0;
    private const float speed = 2;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (_active)
        {
            if (t >= 1)
            {
                _active = false;
                t = 0;

                GetComponent<Transform>().position = _cameraGoal;
                GetComponent<Transform>().LookAt(Vector3.zero, Vector3.up);

                return;
            }

            GetComponent<Transform>().position = Vector3.Lerp(cameraStart, _cameraGoal, t);
            GetComponent<Transform>().LookAt(Vector3.zero, Vector3.up);

            t += Time.deltaTime * speed;
        }
        else
        {
            cameraStart = GetComponent<Transform>().position;
        }
        
    }
}
