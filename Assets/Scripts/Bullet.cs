using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    //[SerializeField] private GameObject hitEffect; // this is for any explosion or feedback we want to give. Nothing relevant at the moment

    private void Start()
    {
        Destroy(gameObject,10);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.gameObject.layer != LayerMask.NameToLayer("Player"))
        {
            if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                collision.collider.gameObject.GetComponent<Enemy>().TakeDamage(40f);
                collision.collider.gameObject.GetComponent<Enemy>().isForcedToSeek = true;
            }

            Destroy(gameObject);
        }
    }
}
