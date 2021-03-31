using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    //Projectile Attributes
    public float initialSpeed = 40f;
    public float impactDamage = 30f;
    public float lifetime = 0f;
    public bool canExplode = false;
    public bool doesDamage = true;
    public float gravityEffect = 0f;
    public AudioSource impactSound;

    //some components we need to reference
    private Rigidbody2D rb2d;
    private float deathTime;
    


    // Start is called before the first frame update
    void Start()
    {
        rb2d = transform.GetComponent<Rigidbody2D>();
        rb2d.gravityScale = gravityEffect;
        rb2d.velocity = transform.right * initialSpeed;

        deathTime = Time.time + lifetime;
    }

    // Update is called once per frame
    void Update()
    {
        if(deathTime < Time.time && lifetime != 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.TryGetComponent(out BreakableStructureScript b))
        {
            b.TakeDamage(impactDamage);
        }

        Destroy(gameObject);
    }
}
