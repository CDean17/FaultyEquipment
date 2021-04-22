using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    //Projectile Attributes
    public float initialSpeed = 40f;
    public float impactDamage = 30f;
    public float lifetime = 0f;
    public bool explodeOnImpact = false;
    public float explosionRadius = 3f;
    public float explosionForce = 100f;
    public float explosionDamage = 40f;
    public bool doesDamage = true;
    public float gravityEffect = 0f;
    public AudioSource impactSound;
    public GameObject baseExplosionObj;

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

        float angle = Mathf.Atan2(rb2d.velocity.y, rb2d.velocity.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.TryGetComponent(out BreakableStructureScript b))
        {
            b.TakeDamage(impactDamage);
        }

        if(collision.gameObject.TryGetComponent(out PlayerInventory p))
        {
            p.TakeDamage(impactDamage);
        }

        if (explodeOnImpact)
        {
            GameObject g = Instantiate(baseExplosionObj, transform.position, new Quaternion());
            ExplosionScript e = g.GetComponent<ExplosionScript>();
            e.radius = explosionRadius;
            e.explosionDamage = explosionDamage;
            e.explosionForce = explosionForce;
        }

        Destroy(gameObject);
    }
}
