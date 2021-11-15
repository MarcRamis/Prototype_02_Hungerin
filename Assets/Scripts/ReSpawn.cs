using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReSpawn : MonoBehaviour
{
    [SerializeField] private GameObject reSpawn = null;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.transform.position = reSpawn.transform.position;
        }
    }

    public void SetNewSpawn(GameObject _spawn)
    {
        reSpawn = _spawn;
    }

}
