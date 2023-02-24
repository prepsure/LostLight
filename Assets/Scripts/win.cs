using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class win : MonoBehaviour
{
    Renderer r;
    bool done = false;
    // Start is called before the first frame update
    void Start()
    {
        r = GameObject.FindGameObjectWithTag("end").GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (done)
        {
            Color prev = r.materials[0].GetColor("_Color");
            r.materials[0].SetColor("_Color", new Color(prev.r, prev.g, prev.b, prev.a + 0.01f));

            if (prev.a + 0.01 >= 0.99)
            {
                done = false;
            }
        }

        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Debug.Log("win!");
            Destroy(other.GetComponent<PlayerController>());
            done = true;
        }
    }
}
