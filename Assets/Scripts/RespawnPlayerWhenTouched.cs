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

        PlayerController pc = other.gameObject.GetComponent<PlayerController>();

        Debug.Log(pc.LastStandingPosition);

        PlayerController.isLimbo = true;
        other.gameObject.GetComponent<Transform>().position = pc.LastStandingPosition;
        other.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;

        pc.yes2 = false;
    }
}
