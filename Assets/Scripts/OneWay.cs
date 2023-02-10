using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneWay : MonoBehaviour
{
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("InvisPlatform"))
        {
            collision.gameObject.GetComponent<Collider>().isTrigger = false;
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.CompareTag("InvisPlatform"))
        {
            collision.gameObject.GetComponent<Collider>().isTrigger = true;
        }
    }
}
