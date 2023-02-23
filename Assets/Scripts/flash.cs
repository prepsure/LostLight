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

    // Start is called before the first frame update
    void Start()
    {
    }

    float lastUpdated = 0;

    // Update is called once per frame
    void Update()
    {
        if (Time.time - lastUpdated > 1)
        {
            lastUpdated = Time.time;

            on = !on;

            Texture arrows = Texture1;
            
            if (on)
            {
                arrows = Texture2;
            }

            GetComponent<Renderer>().materials[0].SetTexture("_MainTex", arrows);

        }
    }
}
