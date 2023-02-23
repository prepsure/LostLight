using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterWave : MonoBehaviour
{
    Camera _camera;

    // Start is called before the first frame update
    void Start()
    {
        _camera = FindObjectOfType<Camera>();
    }

    private float speed = 0.0003f;

    // Update is called once per frame
    void Update()
    {
        if (_camera.GetComponent<CameraTween>()._active)
        {
            speed = 0.01f;
        } 
        else
        {
            speed = 0.0005f;
        }

        // was going to make the water move in the camera direction but noooo
        //func(_camera.GetComponent<CameraTween>().cameraStart - _camera.GetComponent<CameraTween>()._cameraGoal);

        Vector2 offset = GetComponent<Renderer>().materials[0].GetTextureOffset("_MainTex");
        GetComponent<Renderer>().materials[0].SetTextureOffset("_MainTex", offset + new Vector2(speed, 0));
        GetComponent<Transform>().localPosition = new Vector3(0, -16.75f, 0) + 0.25f * Mathf.Sin(Time.realtimeSinceStartup*1.5f) * Vector3.up;
    }

    /*int func(Vector3 v)
    {
        if (v.x < 0) { }
    }*/
}
