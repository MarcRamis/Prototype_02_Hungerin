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
        else if(other.gameObject.tag == "CanBeEaten")
        {
            GameObject assetPrefab = null;
            Vector3 tempPos = other.gameObject.GetComponent<Objects>().originalPos;
            Quaternion tempRot = other.gameObject.GetComponent<Objects>().originalRot;
            Objects.ObjType tempType = other.gameObject.GetComponent<Objects>().GetObjItIs();
            switch (tempType)
            {
                case (Objects.ObjType.JEWEL):
                    assetPrefab = Resources.Load<GameObject>("Prefabs/jewel");
                    Instantiate(assetPrefab, tempPos, tempRot, GameObject.Find("Eateable_Objects").transform);
                    Destroy(other.gameObject);
                    break;
                case (Objects.ObjType.LOG):
                    assetPrefab = Resources.Load<GameObject>("Prefabs/Log");
                    assetPrefab.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                    Instantiate(assetPrefab, tempPos, tempRot, GameObject.Find("Eateable_Objects").transform);
                    Destroy(other.gameObject);
                    break;
                case (Objects.ObjType.LOGSTACK):
                    assetPrefab = Resources.Load<GameObject>("Prefabs/Log_Stack");
                    assetPrefab.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                    Instantiate(assetPrefab, tempPos, tempRot, GameObject.Find("Eateable_Objects").transform);
                    Destroy(other.gameObject);
                    break;
                case (Objects.ObjType.CRATE):
                    other.gameObject.transform.position = tempPos;
                    other.gameObject.transform.rotation = tempRot;
                    break;
                case (Objects.ObjType.CAKE_END):
                    other.gameObject.transform.position = tempPos;
                    other.gameObject.transform.rotation = tempRot;
                    break;
                default:
                    Debug.Log("Couldn't find gameObject to Respawn");
                    break;
            }
        }
    }

    public void RespawnPlayer(GameObject player)
    {
        player.transform.position = reSpawn.transform.position;
    }

    public void SetNewSpawn(GameObject _spawn)
    {
        reSpawn = _spawn;
    }

}
