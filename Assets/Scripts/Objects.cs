using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objects : MonoBehaviour
{
    public enum ItemType { EATEN, GRIPPY, POWERUP_COLLAPSE, POWERUP_CHILE, CAKE_END };
    private enum SizeType { SMALL, MEDIUM, BIG, NONE, DEFAULT };
    public enum ObjType {  JEWEL, LOG, LOGSTACK, CRATE, PU_COLLAPSE, PU_CHILE, CAKE_END, DEFAULT };
    private Rigidbody m_Rigidbody;
    [SerializeField] private EssencialProperties m_essencialProperties;
    [SerializeField] private SizeType typeObj = SizeType.DEFAULT;
    [SerializeField] private ItemType itype = ItemType.EATEN;
    [SerializeField] private ObjType objectItIs = ObjType.DEFAULT;
    private float sumSize = 0.0f;
    private float sumWeight = 0.0f;
    private float speedToTarget = 800f;

    public bool isBeingLaunched { get; set; }

    public Vector3 originalPos { get; set; }
    public Quaternion originalRot { get; set; }

    private void Awake()
    {
        m_Rigidbody = gameObject.GetComponent<Rigidbody>();
        originalPos = transform.position;
        originalRot = transform.localRotation;

        switch (typeObj)
        {
            case (SizeType.SMALL):
                sumSize = 2;
                sumWeight = 0.1f;
                break;
            case (SizeType.MEDIUM):
                sumSize = 10;
                sumWeight = 0.5f;
                break;
            case (SizeType.BIG):
                sumSize = 20;
                sumWeight = 1f;
                break;
            case (SizeType.NONE):
                break;
            case (SizeType.DEFAULT):
                Debug.Log("Forgot to init values");
                break;
            default:
                Debug.Log("Fatal Error no enum found");
                break;
        }
    }

    public float GetWeight() { return m_essencialProperties.weight; }
    public float GetLargeSize() { return m_essencialProperties.largeSize; }

    public float GetSumWeight() { return sumWeight; }

    public float GetSumSize() { return sumSize; }

    public ItemType GetEType() { return itype; }
    public ObjType GetObjItIs() { return objectItIs; }
    public void MoveToPlayer(Vector3 target)
    {
        gameObject.layer = LayerMask.NameToLayer("IgnoreColls");
        Vector3 direction = target - transform.position;
        direction = direction.normalized;

        m_Rigidbody.AddForce(direction * speedToTarget, ForceMode.Acceleration);
        this.gameObject.tag = "Untagged";
        StartCoroutine("DestroyItself");
    }

    IEnumerator DestroyItself()
    {
        yield return new WaitForSeconds(0.5f);
        Destroy(this.gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.layer != LayerMask.NameToLayer("Player") 
            && isBeingLaunched)
        {

            if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                collision.collider.gameObject.GetComponent<Enemy>().TakeDamage(50f);
                collision.collider.gameObject.GetComponent<Enemy>().isForcedToSeek = true;

                isBeingLaunched = false;
            }
            StartCoroutine("DisableIsBeingLaunched");
        }
    }

    IEnumerator DisableIsBeingLaunched()
    {
        yield return new WaitForSeconds(0.5f);
        isBeingLaunched = false;
    }
}
