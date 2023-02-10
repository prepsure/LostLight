using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopOutPlatform : MonoBehaviour
{
    private GameObject[] Popped;
    private float PopOutDistance = 19;
    private float PlatformHeight = 1;

    // Start is called before the first frame update
    void Start()
    {
        GameObject invisPlat = (GameObject)Resources.Load("InvisiblePlatform");

        Debug.Log(invisPlat);

        var bounds = GetComponent<BoxCollider>().bounds;

        Vector3 defaultPos = new(
            bounds.center.x,
            bounds.max.y - PlatformHeight/2,
            bounds.center.z
        );

        // add the position to THIS GUY!!
        var data = gameObject.AddComponent<ArbitraryDataScript>();
        data._originalPosition = defaultPos;

        GameObject p0 = Instantiate(invisPlat);
        p0.GetComponent<Transform>().position = defaultPos;

        Popped = new GameObject[]
        {
            MakeInvisPlat(p0, new(1, 0, 0),  new(0, 1, 1), bounds, defaultPos),
            MakeInvisPlat(p0, new(-1, 0, 0), new(0, 1, 1), bounds, defaultPos),
            MakeInvisPlat(p0, new(0, 0, 1),  new(1, 1, 0), bounds, defaultPos),
            MakeInvisPlat(p0, new(0, 0, -1), new(1, 1, 0), bounds, defaultPos),
        };

        Destroy(p0);
    }

    GameObject MakeInvisPlat(GameObject toClone, Vector3 popOutDir, Vector3 inverseDir, Bounds bounds, Vector3 origPos)
    {
        // TODO if the ray hits anything, dont make the platform if (Physics.Raycast())

        GameObject clone = Instantiate(toClone, null);

        ArbitraryDataScript data = clone.AddComponent<ArbitraryDataScript>();
        data._originalPosition = origPos;

        Transform cloneTrans = clone.GetComponent<Transform>();
        Vector3 pos = cloneTrans.position;

        cloneTrans.position = PopOutDistance * popOutDir + Vector3.Scale(inverseDir, pos);
        cloneTrans.GetComponent<Transform>().localScale = makepositive(popOutDir + Vector3.Scale(inverseDir, new(
            bounds.max.x - bounds.min.x,
            PlatformHeight,
            bounds.max.z - bounds.min.z
        )));

        clone.tag = "InvisPlatform";
        clone.GetComponent<Collider>().isTrigger = true;

        return clone;
    }

    Vector3 makepositive(Vector3 raw)
    {
        return new Vector3(Mathf.Abs(raw.x), Mathf.Abs(raw.y), Mathf.Abs(raw.z));
    }
}
