using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponCrateScript : MonoBehaviour
{
    public List<WeaponScriptableObjects> spawnTable = new List<WeaponScriptableObjects>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        Instantiate(spawnTable[(int)Random.Range(0, spawnTable.Count - 0.1f)].weaponPickupObj, transform.position, transform.rotation);
    }

}
