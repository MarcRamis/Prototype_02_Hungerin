using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletFire : MonoBehaviour
{
    private void Start()
    {
        Destroy(gameObject, 10);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.layer != LayerMask.NameToLayer("Player"))
        {
            if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                collision.collider.gameObject.GetComponent<Enemy>().TakeDamage(20f);
                collision.collider.gameObject.GetComponent<Enemy>().isForcedToSeek = true;
            }

            Destroy(gameObject);
        }
    }
}
