using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTween : MonoBehaviour
{
    [SerializeField]
    public bool _active;
    public bool _undoActive;

    [SerializeField]
    public Vector3 _cameraGoal;

    private Vector3 cameraStart;
    private float t = 0;
    private const float speed = 3;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(_undoActive)
        {
            _undoActive = false;
            _active = false;
        }

        if (_active)
        {
            if (t >= 1)
            {
                _undoActive = true;
                t = 0;

                GetComponent<Transform>().position = _cameraGoal;
                GetComponent<Transform>().LookAt(Vector3.zero, Vector3.up);

                return;
            }

            GetComponent<Transform>().position = Vector3.Lerp(cameraStart, _cameraGoal, t);
            GetComponent<Transform>().LookAt(new Vector3(0, /*GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>().position.y*/ 0, 0), Vector3.up);

            t += Time.deltaTime * speed;
        }
        else
        {
            cameraStart = GetComponent<Transform>().position;
        }
        
    }
}
