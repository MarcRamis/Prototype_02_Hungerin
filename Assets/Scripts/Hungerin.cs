using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hungerin : MonoBehaviour
{
    private enum TypeTransformation { NORMAL, COLLAPSE, CHILE }

    [SerializeField] private Rigidbody m_RigidBody;
    [SerializeField] private Transform m_Target;
    [SerializeField] LineRenderer m_LineRenderer;
    private Stack<EssencialProperties> eatenGameObjects = new Stack<EssencialProperties>();
    [SerializeField] private Material m_Material;

    [Space]
    [SerializeField] EssencialProperties m_EssencialProperties;

    [Space]
    [Header("Movement physics")]
    [SerializeField] private float speed = 300f;
    
    [Space]
    [Header("Jump physics")]
    [SerializeField] private float jumpSpeed = 30f;
    [SerializeField] private float jumpForwardSpeed = 3f;
    private bool spaceInputButton = false;
    
    [Space]
    [Header("Ground physics")]
    [SerializeField] private Transform m_Grounded;
    private bool isGrounded;
    [SerializeField] private float groundRadius = 0.5f;
    [SerializeField] private LayerMask[] groundMask;
    
    [Space]
    [Header("Rotation physics")]
    [SerializeField] private float turnSmoothTime = 0.1f;
    private float turnSmoothVelocity;
    private float turnSmoothVelocity2;

    [Space]
    [Header("Gravity physics")]
    [SerializeField] private float gravityScale = 20.0f;
    [SerializeField] private float globalGravity = -9.81f;
    [SerializeField] private float gravityCollapseScale = 0f;

    [Space]
    [Header("Eat physics")]
    [SerializeField] private float maxTongueDistance = 10.0f;
    [SerializeField] [Range(0.3f, 3.5f)] private float maxSize = 2.5f;
    [SerializeField] [Range(0.3f, 3.5f)] private float minSize = 0.5f;
    [SerializeField] private float launchDirectionAgainstMassForce = 2000f;
    [SerializeField] private float launchUpDirectionAgainstMassForce = 1000f;
    [SerializeField] private float minWeight = 1.0f;
    [SerializeField] private float minLargeSize = 0.0f;
    private bool eatInputButton = false;
    private float scalarMultiplier = 0.01f;
    private Vector3 initialScale = new Vector3(1f,1f,1f);
    private bool playerIsForced = false;

    [Space]
    [Header("Spit physics")]
    [SerializeField] Transform m_SpitSpawn;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed = 15f;
    private bool spitInputButton = false;

    [Space]
    [Header("Tongue attack physics")]
    private bool isGrappeling = false;
    private bool isCarryingUp = false;
    private GameObject grabObj;
    [SerializeField] private float grappledObjectWhenMovingRadius = 0.5f;
    [SerializeField] private float grappledObjectLaunchSpeed = 200f;
    public bool infiniteHealth { get; set; }
    
    [Space]
    [Header("Collapse Transformation physics")]
    [SerializeField] private float collapseAttackRadius = 3f;
    private TypeTransformation m_TypeTransformation = TypeTransformation.NORMAL;
    private bool canDoubleJump = false;
    public bool isCollapsing { get; set; }

    [Space]
    [Header("Chile Transformation physics")]

    [SerializeField] GameObject bulletFirePrefab;
    [SerializeField] private float bulletFireSpeed = 20f;

    private void Awake()
    {
        m_RigidBody.mass = m_EssencialProperties.weight;
        initialScale = transform.localScale;
        infiniteHealth = false;
    }

    private void Start()
    {
        m_LineRenderer.enabled = false;
        ChangeFormTransformation();
    }

    // Here we control the player
    private void Update()
    {
        if (Input.GetButtonDown("LaunchTongue"))
        {
            eatInputButton = true;
        }
        if (Input.GetButtonDown("Spit"))
        {
            spitInputButton = true;
        }
        if(Input.GetButtonDown("Jump"))
        {
            spaceInputButton = true;
        }
    }

    // Here we make physics
    private void FixedUpdate()
    {
        switch(m_TypeTransformation)
        {
            case TypeTransformation.NORMAL:
                NormalMovement();
                UseSpit();
                break;
            case TypeTransformation.COLLAPSE:
                CollapseMovement();
                UseSpit();
                break;
            case TypeTransformation.CHILE:
                NormalMovement();
                UseSpitChile();
                break;
            default:
                Debug.Log("Error type transformation");
                break;
        }
 
        UseTongue();
    }

    private void NormalMovement()
    {
        // Inputs movement
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;
        Vector3 directionRotation = m_Target.position - transform.position;
        directionRotation = directionRotation.normalized;

        // Rotation on forward direction
        float targetAngle = Mathf.Atan2(directionRotation.x, directionRotation.z) * Mathf.Rad2Deg;
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
        transform.rotation = Quaternion.Euler(0f, angle, 0f);

        foreach (LayerMask layer in groundMask)
        {
            isGrounded = Physics.CheckSphere(m_Grounded.position, groundRadius, layer);
            if (isGrounded) break;
        }
        
        if (isGrounded)
        {
            if (spaceInputButton)
            {
                m_RigidBody.AddForce(new Vector3(direction.x * jumpForwardSpeed, jumpSpeed, direction.z * jumpForwardSpeed), ForceMode.Impulse);
                spaceInputButton = false;
            }

            if (!playerIsForced)
            {
                m_RigidBody.velocity = direction * speed * Time.fixedDeltaTime;
            }
        }
        else
        {
            Vector3 gravity = globalGravity * gravityScale * Vector3.up;
            m_RigidBody.AddForce(gravity, ForceMode.Acceleration);
        }
    }
    private void UseTongue()
    {
        if (eatInputButton && !isGrappeling && !isCarryingUp)
        {
            Vector3 direction = m_Target.position - transform.position;
            Ray raycastTarget = new Ray(transform.position, direction.normalized);
            RaycastHit hit;

            if (Physics.Raycast(raycastTarget, out hit, maxTongueDistance))
            {
                DrawLineRenderer(transform.position,hit.point);

                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("EatenItem"))
                {
                    switch(hit.collider.gameObject.GetComponent<Objects>().GetEType())
                    {
                        case (Objects.ItemType.EATEN):
                            Eat(hit);
                            break;
                        case (Objects.ItemType.GRIPPY):
                            Grapple(hit);
                            break;
                        case (Objects.ItemType.POWERUP_COLLAPSE):
                            EatPowerUp(hit);
                            m_TypeTransformation = TypeTransformation.COLLAPSE;
                            ChangeFormTransformation();
                            break;
                        case (Objects.ItemType.POWERUP_CHILE):
                            EatPowerUp(hit);
                            m_TypeTransformation = TypeTransformation.CHILE;
                            ChangeFormTransformation();
                            break;
                        case (Objects.ItemType.CAKE_END):
                            GameObject.Find("GameController").GetComponent<GameController>().reloadScene = true;
                            hit.collider.gameObject.GetComponent<Objects>().MoveToPlayer(transform.position);
                            break;
                    }
                }
            }
            eatInputButton = false;
        }
        else if (eatInputButton && isGrappeling && grabObj != null && !isCarryingUp)
        {
            GrappleLaunch();
            isGrappeling = false;
            eatInputButton = false;
        }

        else if (isCarryingUp)
        {
            MoveGrappledObject();
        }
    }
    private void Eat(RaycastHit hit)
    {
        // Hit represents with raycast has collided

        // If player is bigger than this gameobject
        if (m_EssencialProperties.largeSize >= hit.collider.gameObject.GetComponent<Objects>().GetLargeSize())
        {
            //He stores the object eaten
            EssencialProperties tempProperties;
            tempProperties.largeSize = hit.transform.gameObject.GetComponent<Objects>().GetSumSize();
            tempProperties.weight = hit.transform.gameObject.GetComponent<Objects>().GetSumWeight();
            eatenGameObjects.Push(tempProperties);
            //GameController stores the objects that are missing in the scene
            GameObject.Find("GameController").GetComponent<GameController>().ObjectEaten(
                hit.collider.gameObject.GetComponent<Objects>().originalPos, 
                hit.collider.gameObject.GetComponent<Objects>().originalRot, 
                hit.collider.gameObject.GetComponent<Objects>().GetObjItIs());

            hit.collider.gameObject.GetComponent<Objects>().MoveToPlayer(transform.position);

            SumSize(hit.collider.gameObject.GetComponent<Objects>().GetSumSize());
            MaxScalarSize(hit.collider.gameObject.GetComponent<Objects>().GetSumSize());
            SumWeight(hit.collider.gameObject.GetComponent<Objects>().GetSumWeight());
            SetNewMass(m_EssencialProperties.weight);
        }
        else
        {
            playerIsForced = true;
            LaunchToDirection(hit.collider.gameObject.transform.position);
            StartCoroutine("PlayerIsForced");
        }
    }
    private void EatPowerUp(RaycastHit hit)
    {
        hit.collider.gameObject.GetComponent<Objects>().MoveToPlayer(transform.position);
        GameObject.Find("GameController").GetComponent<GameController>().PowerUpsEaten(
                hit.collider.gameObject.GetComponent<Objects>().originalPos,
                hit.collider.gameObject.GetComponent<Objects>().originalRot,
                hit.collider.gameObject.GetComponent<Objects>().GetObjItIs());
    }
    private void Grapple(RaycastHit hit)
    {
        // Hit represents with raycast has collided
        isGrappeling = true;
        grabObj = hit.collider.gameObject;
        isCarryingUp = true;
    }
    private void MoveGrappledObject()
    {
        float distance = Vector3.Distance(grabObj.transform.position, m_SpitSpawn.position);

        if (distance <= grappledObjectWhenMovingRadius)
        {
            isCarryingUp = false;
            grabObj.GetComponent<Rigidbody>().transform.parent = m_SpitSpawn;
            grabObj.GetComponent<Rigidbody>().useGravity = false;
            grabObj.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            grabObj.GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
        else
        {
            Vector3 direction = m_SpitSpawn.position - grabObj.transform.position;
            direction = direction.normalized;
            grabObj.GetComponent<Rigidbody>().velocity = direction * 10;
        }
    }
    private void GrappleLaunch()
    {
        // I set this to true to control enemy damage. Now is like a bullet
        grabObj.GetComponent<Objects>().isBeingLaunched = true;
        // Force direction
        Vector3 direction = m_Target.transform.position - grabObj.transform.position;
        direction = direction.normalized;
        grabObj.GetComponent<Rigidbody>().AddForce(direction * grappledObjectLaunchSpeed, ForceMode.Acceleration);

        // Dettach from player
        grabObj.GetComponent<Rigidbody>().transform.parent = null;
        grabObj.GetComponent<Rigidbody>().useGravity = true;
        grabObj.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
    }
    private void UseSpit()
    {
        if (spitInputButton)
        {
            if (!isGrappeling)
            {
                if (transform.localScale.x > minSize && eatenGameObjects.Count > 0)
                {
                    // Spit
                    GameObject bullet = Instantiate(bulletPrefab, m_SpitSpawn.position, m_SpitSpawn.rotation);
                    Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();

                    // Spit force
                    Vector3 direction = m_Target.position - m_SpitSpawn.position;
                    bulletRb.AddForce(direction.normalized * bulletSpeed, ForceMode.Impulse);

                    //Eliminate stored object from the stack and store the values
                    EssencialProperties objToSpit;

                    objToSpit = eatenGameObjects.Peek();
                    eatenGameObjects.Pop();
                    GameObject.Find("GameController").GetComponent<GameController>().ReSpawnObj();
                    SumSize(-objToSpit.largeSize);
                    MinScalarSize(-objToSpit.largeSize);
                    SumWeight(-objToSpit.weight);
                    SetNewMass(m_EssencialProperties.weight);
                }
            }

            spitInputButton = false;
        }
    }
    private void UseSpitChile()
    {
        if (spitInputButton)
        {
            if (!isGrappeling)
            {
                if (transform.localScale.x > minSize && eatenGameObjects.Count > 0)
                {
                    // Spit
                    GameObject bullet = Instantiate(bulletFirePrefab, m_SpitSpawn.position, m_SpitSpawn.rotation);
                    Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();

                    // Spit force
                    Vector3 direction = m_Target.position - m_SpitSpawn.position;
                    bulletRb.AddForce(direction.normalized * bulletFireSpeed, ForceMode.Impulse);

                    //Eliminate stored object from the stack and store the values
                    EssencialProperties objToSpit;

                    objToSpit = eatenGameObjects.Peek();
                    eatenGameObjects.Pop();
                    GameObject.Find("GameController").GetComponent<GameController>().ReSpawnObj();
                    SumSize(-objToSpit.largeSize);
                    MinScalarSize(-objToSpit.largeSize);
                    SumWeight(-objToSpit.weight);
                    SetNewMass(m_EssencialProperties.weight);
                }
                else
                {
                    // Spit
                    GameObject bullet = Instantiate(bulletFirePrefab, m_SpitSpawn.position, m_SpitSpawn.rotation);
                    Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();

                    // Spit force
                    Vector3 direction = m_Target.position - m_SpitSpawn.position;
                    bulletRb.AddForce(direction.normalized * bulletSpeed, ForceMode.Impulse);
                }
            }

            spitInputButton = false;
        }
    }
    private void CollapseMovement()
    {
        // Inputs movement
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;
        Vector3 directionRotation = m_Target.position - transform.position;
        directionRotation = directionRotation.normalized;

        // Rotation on forward direction
        float targetAngle = Mathf.Atan2(directionRotation.x, directionRotation.z) * Mathf.Rad2Deg;
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
        transform.rotation = Quaternion.Euler(0f, angle, 0f);

        foreach (LayerMask layer in groundMask)
        {
            isGrounded = Physics.CheckSphere(m_Grounded.position, groundRadius, layer);
            if (isGrounded) break;
        }

        if (isGrounded)
        {
            canDoubleJump = false;

            if (spaceInputButton)
            {
                m_RigidBody.AddForce(new Vector3(direction.x * jumpForwardSpeed, jumpSpeed, direction.z * jumpForwardSpeed), ForceMode.Impulse);
                canDoubleJump = true;
                spaceInputButton = false;
            }

            if (!playerIsForced)
            {
                m_RigidBody.velocity = direction * speed * Time.fixedDeltaTime;
            }
        }
        else
        {
            if (canDoubleJump)
            {
                if(spaceInputButton)
                {
                    Vector3 gravity = globalGravity * gravityScale * gravityCollapseScale * Vector3.up;
                    m_RigidBody.AddForce(gravity, ForceMode.Acceleration);

                    CinemachineShake.Instance.ShakeCamera(5f,0.2f);

                    canDoubleJump = false;
                    spaceInputButton = false;
                    isCollapsing = true;
                    StartCoroutine("DisableIsCollapsing");
                }
                else
                {
                    Vector3 gravity = globalGravity * gravityScale * Vector3.up;
                    m_RigidBody.AddForce(gravity, ForceMode.Acceleration);
                }
            }
            else
            {
                Vector3 gravity = globalGravity * gravityScale * Vector3.up;
                m_RigidBody.AddForce(gravity, ForceMode.Acceleration);
            }

            spaceInputButton = false;   // This is because if you press space input button again in air it makes another jump without pressing at that moment
        }
    }
    
    private void MaxScalarSize(float _largeSize)
    {
        if (transform.localScale.x <= maxSize)
        {
            transform.localScale +=
            new Vector3(
                _largeSize * scalarMultiplier,
                _largeSize * scalarMultiplier,
                _largeSize * scalarMultiplier);

            if (transform.localScale.x >= maxSize)
            {
                transform.localScale =
                    new Vector3(
                        maxSize,
                        maxSize,
                        maxSize);
            }
        }
    }
    private void MinScalarSize(float _largeSize)
    {
        if (transform.localScale.x >= minSize)
        {
            transform.localScale +=
            new Vector3(
                _largeSize * scalarMultiplier,
                _largeSize * scalarMultiplier,
                _largeSize * scalarMultiplier);

            if (transform.localScale.x <= minSize)
            {
                transform.localScale =
                    new Vector3(
                        minSize,
                        minSize,
                        minSize);
            }
        }
    }
    private void SumSize(float sizeEaten)
    {
        if (m_EssencialProperties.largeSize >= minLargeSize)
        {
            m_EssencialProperties.largeSize += sizeEaten;
            if (m_EssencialProperties.largeSize < minLargeSize)
            {
                m_EssencialProperties.largeSize = minLargeSize;
            }
        }
    }
    private void SumWeight(float weightEaten)
    {
        if(m_EssencialProperties.weight >= minWeight)
        {
            m_EssencialProperties.weight += weightEaten;
            if (m_EssencialProperties.weight < minWeight)
            {
                m_EssencialProperties.weight = minWeight;
            }
        }
        
    }
    private void SetNewMass(float weightEaten)
    {
        m_RigidBody.mass = weightEaten;
    }
   
    private void LaunchToDirection(Vector3 target)
    {
        Vector3 direction = target - transform.position;
        direction = direction.normalized;
        
        m_RigidBody.AddForce(direction * launchDirectionAgainstMassForce + Vector3.up * launchUpDirectionAgainstMassForce, ForceMode.Acceleration);
    }
    
    private void DrawLineRenderer(Vector3 start, Vector3 end)
    {
        m_LineRenderer.enabled = true;
        m_LineRenderer.SetPosition(0, start);
        m_LineRenderer.SetPosition(1, end);

        StartCoroutine("DisableTongue");
    }
    private void ChangeFormTransformation()
    {
        switch (m_TypeTransformation)
        {
            case TypeTransformation.NORMAL:
                m_Material.color = NewColor(47,39,183,255);
                gravityScale = 15;
                break;
            case TypeTransformation.COLLAPSE:
                m_Material.color = NewColor(183, 39, 177, 255);
                gravityScale = 10;
                break;
            case TypeTransformation.CHILE:
                m_Material.color = NewColor(236, 89, 0, 255);
                gravityScale = 15;
                break;
            default:
                Debug.Log("Error type transformation");
                break;
        }
    }
    private Color NewColor(float r, float g, float b, float a)
    {
        return new Color(r/255, g/255, b/255, a/255);
    }
    public bool isInsideCollapseRadius(Vector3 target)
    {
        float distanceAToB = Vector3.Distance(transform.position, target);
        return distanceAToB <= collapseAttackRadius;
    }
    public void TakeDamage()
    {
        
        if(eatenGameObjects.Count != 0)
        {
            EssencialProperties objToSpit;

            objToSpit = eatenGameObjects.Peek();
            eatenGameObjects.Pop();
            GameObject.Find("GameController").GetComponent<GameController>().ReSpawnObj();
            SumSize(-objToSpit.largeSize);
            MinScalarSize(-objToSpit.largeSize);
            SumWeight(-objToSpit.weight);
            SetNewMass(m_EssencialProperties.weight);
        }
        else if(!infiniteHealth)
        {
            GameObject.Find("BoxRespawner").GetComponent<ReSpawn>().RespawnPlayer(this.gameObject);
        }
        StartCoroutine("TakingDamage");
    }
   
    IEnumerator DisableTongue()
    {
        yield return new WaitForSeconds(1f);
        m_LineRenderer.enabled = false;
    }    
    IEnumerator PlayerIsForced()
    {
        yield return new WaitForSeconds(0.15f);
        playerIsForced = false;
    }
    IEnumerator DisableDoubleJump()
    {
        yield return new WaitForSeconds(1f);
        canDoubleJump = false;
        isCollapsing = false;
    }
    IEnumerator DisableIsCollapsing()
    {
        yield return new WaitForSeconds(0.2f);
        isCollapsing = false;
    }
    IEnumerator TakingDamage()
    {
        Color originalColor = m_Material.color;
        yield return new WaitForSeconds(0.02f);
        m_Material.color = Color.white;
        yield return new WaitForSeconds(0.02f);
        m_Material.color = originalColor;
        yield return new WaitForSeconds(0.02f);
        m_Material.color = Color.white;
        yield return new WaitForSeconds(0.02f);
        m_Material.color = originalColor;
    }

    public void ResetMassPlayer()
    {
        m_EssencialProperties.weight = minWeight;
        m_EssencialProperties.largeSize = minLargeSize;
        SetNewMass(1.0f);
        transform.localScale =
                    new Vector3(
                        minSize,
                        minSize,
                        minSize);
        while(eatenGameObjects.Count != 0)
        {
            GameObject.Find("GameController").GetComponent<GameController>().ReSpawnObj();
            eatenGameObjects.Pop();
        }
        
    }
    public float GetWeight() { return m_EssencialProperties.weight; }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(m_Grounded.transform.position, groundRadius);

        Gizmos.DrawLine(transform.position, m_Target.position);
        
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, maxTongueDistance);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, collapseAttackRadius);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(m_SpitSpawn.position, grappledObjectWhenMovingRadius);
    }
}
