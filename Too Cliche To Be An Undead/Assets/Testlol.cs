using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testlol : MonoBehaviour
{
    public ParticleSystem particle;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("d"))
        {
            Instantiate(particle, this.transform);
        }
    }
}
