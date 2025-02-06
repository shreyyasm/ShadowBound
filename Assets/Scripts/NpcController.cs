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
    private bool isControlled = false;
    private GameObject player;
    private PlayerController playerController;
    public bool interacting;

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
        if (interacting && Input.GetMouseButtonDown(0) && !isControlled)
        {
            TakeControl();
        }
        else if(interacting && Input.GetMouseButtonDown(0) && isControlled)
        {
            ReleaseControl();
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
        isControlled = true;
        enemyHealth.killing = true;
        enemyHealth.healthSliderMain.SetActive(true);
        agent.enabled = false; // Disable NavMeshAgent
        player.transform.SetParent(transform);
        player.transform.localPosition = Vector3.zero;
        player.SetActive(false); // Hide player
    }

    void ReleaseControl()
    {
        isControlled = false;
        enemyHealth.killing = false;
        enemyHealth.healthSliderMain.SetActive(false);
        player.transform.SetParent(null);
        player.SetActive(true); // Show player again
        agent.enabled = true; // Re-enable NavMeshAgent
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
            dashTime = Time.time + dashDuration;
            lastDashTime = Time.time;
            rb.velocity = moveDirection * dashSpeed; // Apply dash velocity
        }

        if (isDashing)
        {
            if (Time.time >= dashTime)
            {
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
    private void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            interacting = true;           
            outline.enabled = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            interacting = false;
            outline.enabled = false;

        }
    }
}