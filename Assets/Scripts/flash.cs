using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class flash : MonoBehaviour
{
    [SerializeField]
    public Texture Texture1;
    [SerializeField]
    public Texture Texture2;
    private bool on = true;
    [SerializeField] 
    public float delay = 1;

    Renderer r;

    // Start is called before the first frame update
    void Start()
    {
        r = GetComponent<Renderer>();
    }

    float lastUpdated = 0;

    // Update is called once per frame
    void Update()
    {
        if (Time.time - lastUpdated > delay)
        {
            lastUpdated = Time.time;

            on = !on;

            Texture arrows = Texture1;
            
            if (on)
            {
                arrows = Texture2;
            }

            r.materials[0].SetTexture("_MainTex", arrows);

        }
    }
}
