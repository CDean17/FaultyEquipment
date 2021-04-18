using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionScript : MonoBehaviour
{
    public GameObject expParticle;
    public AudioSource soundEffect;
    public float radius = 3f;
    public float explosionForce = 100f;
    public float explosionDamage = 50f;

    // Start is called before the first frame update
    void Start()
    {
        Explode();
    }

    void Explode()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius);
        Instantiate(expParticle, transform.position, transform.rotation);

        foreach(Collider2D c in hits)
        {
            if(c.TryGetComponent(out BreakableStructureScript b))
            {
                //do damage based on distance
                float dist = Vector2.Distance(transform.position, b.transform.position);
                dist = dist / radius;
                dist = 1f - dist;
                b.TakeDamage(explosionDamage);
                AddExplosionForce(b.rb2d, explosionForce, transform.position, radius);
            }
        }

        //Destroy(gameObject);
    }

    public void AddExplosionForce(Rigidbody2D rb, float explosionForce, Vector2 explosionPosition, float explosionRadius, float upwardsModifier = 0.0F, ForceMode2D mode = ForceMode2D.Force)
    {
        var explosionDir = rb.position - explosionPosition;
        var explosionDistance = explosionDir.magnitude;

        // Normalize without computing magnitude again
        if (upwardsModifier == 0)
            explosionDir /= explosionDistance;
        else
        {
            
            explosionDir.y += upwardsModifier;
            explosionDir.Normalize();
        }

        rb.AddForce(Mathf.Lerp(0, explosionForce, (1 - explosionDistance)) * explosionDir, mode);
    }
}
