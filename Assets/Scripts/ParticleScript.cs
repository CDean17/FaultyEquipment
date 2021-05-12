using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleScript : MonoBehaviour
{
    private ParticleSystem ps;
    private AudioSource src;

    // Start is called before the first frame update
    void Start()
    {
        src = gameObject.GetComponent<AudioSource>();
        ps = transform.GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!ps.isPlaying)
        {
            Destroy(gameObject);
        }
    }
}
