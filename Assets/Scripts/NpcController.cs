using EPOOutline;
using UnityEngine;
using UnityEngine.AI;

public class NPCPatrol : MonoBehaviour
{
    public float roamRadius = 10f;
    public float roamDelay = 2f;
    public float interactionRange = 2f;
    private NavMeshAgent agent;
    private float nextMoveTime;
    private Vector3 startPosition;
    public bool isControlled = false;
    private GameObject player;
    private PlayerController playerController;
    public bool interacting;
    public bool canSwitch;

    public Outlinable outline;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        outline = GetComponent<Outlinable>();
        startPosition = transform.position; // Store initial position
        nextMoveTime = Time.time + roamDelay;
        player = GameObject.FindGameObjectWithTag("Player");
        playerController = player.GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody>();
        rb.drag = 5f;
        rb.freezeRotation = true; // Prevent rotation due to physics
    }

    void Update()
    {
        if (!isControlled)
        {
            if (Time.time >= nextMoveTime && !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                MoveToRandomPoint();
                nextMoveTime = Time.time + roamDelay;
            }
        }
        if (interacting && Input.GetMouseButtonDown(1) && !isControlled)
        {
            TakeControl();
        }
        else if(!canSwitch && !interacting && Input.GetMouseButtonDown(1) && isControlled)
        {
            ReleaseControl();
            Debug.Log("Release" + gameObject.name);
        }


        if (canSwitch && Input.GetMouseButtonDown(1) && isControlled)
        {
           
            SwitchControl();
            Debug.Log("Switch" + gameObject.name);
        }

        if (isControlled)
        {
            if (!isDashing)
            {
                HandleMovement();
            }

            HandleDash();
        }

    }

    void MoveToRandomPoint()
    {
        Vector3 randomDirection = Random.insideUnitSphere * roamRadius;
        randomDirection += startPosition; // Use fixed start position
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, roamRadius, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }

    void TakeControl()
    {
        if (!player.GetComponent<PlayerController>().controlingLife)
        {
            isControlled = true;
            enemyHealth.killing = true;
            enemyHealth.healthSliderMain.SetActive(true);
            agent.enabled = false; // Disable NavMeshAgent
            player.GetComponent<PlayerController>().controlingLife = true;
            player.transform.SetParent(transform);
            player.transform.localPosition = Vector3.zero;
            player.SetActive(false); // Hide player
            canSwitch = false;
        }


    }
    void SwitchControl()
    {
             ReleaseControl();
            temp.GetComponent<NPCPatrol>().TakeControl();
            
        


    }

    void ReleaseControl()
    {
        if (player.GetComponent<PlayerController>().controlingLife)
        {
            isControlled = false; 
            enemyHealth.killing = false;
            enemyHealth.healthSliderMain.SetActive(false);
            player.transform.SetParent(null);
            player.GetComponent<PlayerController>().controlingLife = false;
            player.SetActive(true); // Show player again
            agent.enabled = true; // Re-enable NavMeshAgent
            player.GetComponent<PlayerHealth>().health = 100f;
            player.GetComponent<SphereCollider>().isTrigger = true;
            LeanTween.delayedCall(1f, () => { player.GetComponent<SphereCollider>().isTrigger = false; });
        }

    }



    //PlayerController
    public float moveSpeed = 5f;
    public float dashSpeed = 15f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;

    private Vector3 moveDirection;
    private Rigidbody rb;
    private bool isDashing = false;
    private float dashTime;
    private float lastDashTime;

    private Vector3 isoForward = new Vector3(1, 0, 1).normalized;
    private Vector3 isoRight = new Vector3(1, 0, -1).normalized;




    void FixedUpdate()
    {
        if (isControlled)
        {
            interacting = false; 
            if (isDashing)
            {
                return; // Do nothing while dashing
            }

            if (moveDirection.magnitude >= 0.1f)
            {
                rb.velocity = new Vector3(moveDirection.x * moveSpeed, rb.velocity.y, moveDirection.z * moveSpeed);
            }
            else
            {
                rb.velocity = new Vector3(0, rb.velocity.y, 0); // Stop drifting
            }
        }

    }

    void HandleMovement()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        moveDirection = (isoRight * horizontal + isoForward * vertical).normalized;

        if (moveDirection.magnitude >= 0.1f)
        {
            rb.velocity = new Vector3(moveDirection.x * moveSpeed, rb.velocity.y, moveDirection.z * moveSpeed);
        }
    }

    void HandleDash()
    {
        if (Input.GetKeyDown(KeyCode.Space) && Time.time > lastDashTime + dashCooldown && moveDirection != Vector3.zero)
        {
            isDashing = true;
            sphereCollider.isTrigger = true;
            dashTime = Time.time + dashDuration;
            lastDashTime = Time.time;
            rb.velocity = moveDirection * dashSpeed; // Apply dash velocity
        }

        if (isDashing)
        {
            if (Time.time >= dashTime)
            {
                sphereCollider.isTrigger = false;
                isDashing = false;
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(startPosition == Vector3.zero ? transform.position : startPosition, roamRadius);
    }
    public EnemyHealth enemyHealth;
    public SphereCollider sphereCollider;
    public GameObject temp;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {

            if (isDashing)
            {
                other.GetComponent<EnemyHealth>().healthSliderMain.SetActive(true);
                other.GetComponent<EnemyHealth>().DealDamage(10f);
                LeanTween.delayedCall(5f, () => { other.GetComponent<EnemyHealth>().healthSliderMain.SetActive(false); });
                Debug.Log("DashDamage");
            }

        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            interacting = true;
            outline.enabled = true;

        }
        if (other.CompareTag("Enemy") && isControlled)
        {
            canSwitch = true;
            temp = other.gameObject;
            other.GetComponent<Outlinable>().enabled = true;

        }
        if (other.CompareTag("Enemy") && !isControlled)
        {
            interacting = true;
         
            //other.GetComponent<Outlinable>().enabled = false;

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            interacting = false;
            outline.enabled = false;
            
        }
        if (other.CompareTag("Enemy") && isControlled)
        {
          
            canSwitch  = false;
            temp = null;
            other.GetComponent<Outlinable>().enabled = false;

        }
        if (other.CompareTag("Enemy") && !isControlled)
        {
            interacting = false;
           
            //other.GetComponent<Outlinable>().enabled = false;

        }
    }
}