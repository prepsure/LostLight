using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformMovement : MonoBehaviour
{
    GameObject[] platforms;

    CapsuleCollider playerCollider;
    float playerFeetHeight;

    Vector3 REALLY_LOW = new(0, -9999999999, 0);

    // Start is called before the first frame update
    void Start()
    {
        platforms = GameObject.FindGameObjectsWithTag("Platform");
        playerCollider = GameObject.Find("CharacterCapsulePlain").GetComponent<CapsuleCollider>();
        playerFeetHeight = playerCollider.bounds.min.y;

        Debug.Log(playerFeetHeight);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        playerFeetHeight = playerCollider.bounds.min.y;
        Debug.Log(playerFeetHeight);

        foreach (var platform in platforms)
        {
            bool playerIsAbove = (platform.GetComponent<Collider>().bounds.max.y <= playerFeetHeight + 0.3);
            platform.GetComponent<Collider>().enabled = playerIsAbove;

            if (playerIsAbove)
            {
                platform.GetComponent<Transform>().position = platform.GetComponent<Platform>().OriginalPosition;
            } 
            else
            {
                platform.GetComponent<Transform>().position = platform.GetComponent<Platform>().OriginalPosition + new Vector3(0, 0, 10);
            }
        }
    }
}
