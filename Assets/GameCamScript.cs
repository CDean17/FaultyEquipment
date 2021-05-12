using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCamScript : MonoBehaviour
{
    private bool shaking = false;
    private float intensity;
    private float endShake;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (shaking)
        {
            transform.localPosition = new Vector3(Mathf.Cos(Time.time), Mathf.Sin(Time.time), 0) * intensity;


            if(endShake < Time.time)
            {
                shaking = false;
            }
        }
        else
        {
            transform.localPosition = Vector3.zero;
        }
    }

    public void ShakeCam(float intensity, float duration)
    {
        shaking = true;
        endShake = Time.time + duration;
    }
}
