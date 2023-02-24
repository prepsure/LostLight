using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pull : MonoBehaviour
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
            r.materials[0].SetTextureOffset("_MainTex", new Vector2(0, Time.time * 0.02f));
    }
}
