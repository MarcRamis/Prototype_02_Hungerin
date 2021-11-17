using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private Stack<SceneObjects> objEaten = new Stack<SceneObjects>();
    public void ObjectEaten(Vector3 _pos, Quaternion _rot, Objects.ObjType _type)
    {
        SceneObjects tempObj;
        tempObj.objPos = _pos;
        tempObj.objRot = _rot;
        tempObj.typeObj = _type;
        objEaten.Push(tempObj);
    }

    public void ReSpawnObj()
    {
        GameObject assetPrefab = null;
        SceneObjects tempObj = objEaten.Peek();
        switch(tempObj.typeObj)
        {
            case (Objects.ObjType.JEWEL):
                assetPrefab = Resources.Load<GameObject>("Prefabs/jewel");
                Instantiate(assetPrefab, tempObj.objPos, tempObj.objRot, GameObject.Find("Eateable_Objects").transform);
                break;
            case (Objects.ObjType.LOG):
                assetPrefab = Resources.Load<GameObject>("Prefabs/Log");
                Instantiate(assetPrefab, tempObj.objPos, tempObj.objRot, GameObject.Find("Eateable_Objects").transform);
                break;
            case (Objects.ObjType.LOGSTACK):
                assetPrefab = Resources.Load<GameObject>("Prefabs/Log_Stack");
                Instantiate(assetPrefab, tempObj.objPos, tempObj.objRot, GameObject.Find("Eateable_Objects").transform);
                break;
            default:
                Debug.Log("Couldn't find gameObject to Respawn");
                break;
        }
        objEaten.Pop();
    }

}
struct SceneObjects
{
    public Vector3 objPos;
    public Quaternion objRot;
    public Objects.ObjType typeObj;
}
