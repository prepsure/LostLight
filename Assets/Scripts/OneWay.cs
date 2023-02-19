using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class OneWay : MonoBehaviour
{
    private void OnTriggerEnter(Collider collision)
    {
        if (!collision.gameObject.CompareTag("InvisPlatform"))
        {
            return;
        }

        ArbitraryDataScript data = collision.gameObject.GetComponent<ArbitraryDataScript>();

        if (!PlayerController.isLimbo && data._isLimbo)
        {
            return;
        }

        if (PlayerController.isLimbo && !(math.abs(collision.bounds.max.y - PlayerController.limboHeight) < 0.03))
        {
            return;
        }

        /*if ((FindObjectOfType<Camera>().GetComponent<Transform>().forward.normalized - data._lookVect).magnitude > 0.03)
        {
            return;
        }*/

        collision.gameObject.GetComponent<Collider>().isTrigger = false;
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.CompareTag("InvisPlatform"))
        {
            collision.gameObject.GetComponent<Collider>().isTrigger = true;
        }
    }
}
