using System;
using UnityEngine;
using Object = UnityEngine.Object;

[ExecuteInEditMode]
public class RoundPositions : MonoBehaviour
{
    float round(float t)
    {
        return (float)Math.Round(t * 10f) / 10f;
    }

    void Awake()
    {
        Object[] gameObjects = FindObjectsOfType(typeof(GameObject));

        Array.ForEach(gameObjects, (Object o) =>
        {
            GameObject go = (GameObject)o;

            Transform goTrans = go.GetComponent<Transform>();

            goTrans.position = new(round(goTrans.position.x), round(goTrans.position.y), round(goTrans.position.z));
            goTrans.localScale = new(round(goTrans.localScale.x), round(goTrans.localScale.y), round(goTrans.localScale.z));
        });
    }
}