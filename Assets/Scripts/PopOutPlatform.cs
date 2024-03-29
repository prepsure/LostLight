using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopOutPlatform : MonoBehaviour
{
    public static float PopOutDistance = 40;
    private float PlatformHeight = 0.8f;

    // Start is called before the first frame update
    void Start()
    {
        GameObject invisPlat = (GameObject)Resources.Load("InvisiblePlatform");

        var bounds = GetComponent<Collider>().bounds;

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

        MakeInvisPlat(p0, new(1, 0, 0), new(0, 1, 1), bounds, defaultPos);
        MakeInvisPlat(p0, new(-1, 0, 0), new(0, 1, 1), bounds, defaultPos);
        MakeInvisPlat(p0, new(0, 0, 1), new(1, 1, 0), bounds, defaultPos);
        MakeInvisPlat(p0, new(0, 0, -1), new(1, 1, 0), bounds, defaultPos);

        Destroy(p0);
    }

    GameObject MakeInvisPlat(GameObject toClone, Vector3 popOutDir, Vector3 inverseDir, Bounds bounds, Vector3 origPos)
    {
        // TODO if the ray hits anything, dont make the platform
        if (Physics.Raycast(toClone.GetComponent<Transform>().position, popOutDir,999, (1 << 12) | (1 << 14)))
        {
            return null;
        }

        GameObject clone = Instantiate(toClone, GameObject.Find("_invis_").GetComponent<Transform>());
        clone.layer = 12;

        ArbitraryDataScript data = clone.AddComponent<ArbitraryDataScript>();
        data._originalPosition = origPos;
        data._lookVect = -popOutDir;

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

        if (Physics.Raycast(toClone.GetComponent<Transform>().position, popOutDir, 1 << 13))
        {
            data._isLimbo = true;
            data._lookVect = -data._lookVect;
            clone.layer = 14;
        }

        return clone;
    }

    Vector3 makepositive(Vector3 raw)
    {
        return new Vector3(Mathf.Abs(raw.x), Mathf.Abs(raw.y), Mathf.Abs(raw.z));
    }
}
