using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private GameObject boxSpawner = null;

    private void Start()
    {
        if(boxSpawner == null)
        {
            Debug.Log("Forgot to put the boxSpawner in a Spawn");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            boxSpawner.GetComponent<ReSpawn>().SetNewSpawn(this.gameObject);
        }
    }
}
