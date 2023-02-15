using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnPlayerWhenTouched : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag != "Player")
        {
            return;
        }

        other.gameObject.GetComponent<Transform>().position = other.gameObject.GetComponent<PlayerController>().LastStandingPosition + new Vector3(0, 0, 0);
        other.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
    }
}
