using EPOOutline;
using System.Collections;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using static PlayerController;

public class NPCController : MonoBehaviour
{
    public string Level;
    public int LevelIndex;
    public EnemyStats EnemyStats;

    [Header("NPC MovementStats")]
    public float NPCSpeed= 2f;
    public float roamRadius = 10f;
    public float roamDelay = 2f;
    private NavMeshAgent agent;
    private float nextMoveTime;
    private Vector3 startPosition;

    [Header("ConsumeStats")]
    public float moveSpeed = 5f;
    public float dashSpeed = 15f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;
    public bool isControlled = false;
    public bool interacting;
    public bool canSwitch;

    [Header("PatrolStats")]
    public Transform[] patrolPoints; // List of patrol points
    private int currentPatrolIndex = 0;
    private bool isChasing = false;
    private bool isStunned = false;
    public bool isSearching = false;

    [Header("Vision Settings")]
    public float visionRange = 10f;
    public float visionAngle = 45f;
    public Transform visionOrigin;

    [Header("Chase Settings")]
    public bool Caught;
    public float chaseSpeed = 4f;
    public float normalSpeed = 2f;
    public float alertDelay = 2f;

    [Header("Control & Stun")]
    public float controlTime = 5f; // Time before NPC gets stunned if controlled
    public float stunDuration = 3f;
    public float searchTime = 4f;
    public float controlTimer = 0f;
    private bool playerInSight;


    [Header("Reference")]
    public SphereCollider sphereCollider;

    [HideInInspector]
    public Outlinable outline;
    [HideInInspector]
    public GameObject temp;

    //Hidden
    private Vector3 moveDirection;
    private Rigidbody rb;
    private bool isDashing = false;
    private float dashTime;
    private float lastDashTime;
    private Vector3 isoForward = new Vector3(1, 0, 1).normalized;
    private Vector3 isoRight = new Vector3(1, 0, -1).normalized;
    private GameObject player;
    private PlayerController playerController;
    private Camera cam;

    private void Awake()
    {
        LoadplayerData();
    }

    public void LoadplayerData()
    {
        //Level
        Level = EnemyStats.Level;
        LevelIndex = EnemyStats.LevelIndex;

        //NPCStats
        NPCSpeed = EnemyStats.NPCSpeed;
        roamRadius = EnemyStats.RoamRadius;
        roamDelay = EnemyStats.RoamDelay;

        visionRange = EnemyStats.visionRange;
        visionAngle = EnemyStats.visionAngle;

        chaseSpeed = EnemyStats.chaseSpeed;
        normalSpeed = EnemyStats.normalSpeed;
        alertDelay = EnemyStats.alertDelay;

        controlTime = EnemyStats.controlTime; 
        stunDuration = EnemyStats.stunDuration;
        searchTime = EnemyStats.searchTime;

        //ControlStats
        moveSpeed = EnemyStats.MoveSpeed;
        dashSpeed = EnemyStats.DashSpeed;
          
    }
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        outline = GetComponent<Outlinable>();
        cam = Camera.main;
        ControlTimeSlider.maxValue  = controlTime;
        controlTimer = controlTime;
        nextMoveTime = Time.time + roamDelay;
        player = GameObject.FindGameObjectWithTag("Player");
        playerController = player.GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody>();
        rb.drag = 5f;
        rb.freezeRotation = true; // Prevent rotation due to physics

        agent.speed = normalSpeed;
        MoveToNextPatrol();
    }
    void Update()
    {
       //Control
        if (interacting && Input.GetMouseButtonDown(1) && !isControlled && !isStunned)
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
       
        if (isControlled)
        {
            controlTimer -= Time.deltaTime;
            ControlTimeSlider.value = controlTimer;
            if (controlTimer <= 0)
            {
              
                Alert = true;
                ReleaseControl();
                StartCoroutine(StunNPC());
            }
            return;
        }

        //AI
        if (!isControlled)
        {
            if (Time.time >= nextMoveTime && !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance && isSearching && !Caught) 
            {
                MoveToRandomPoint();
                nextMoveTime = Time.time + roamDelay;
            }
        }
      
        if (isStunned || isSearching)
            return;
        if (!isChasing && !agent.pathPending && agent.remainingDistance < 0.5f && !Caught)
        {
            MoveToNextPatrol();
        }
        playerInSight = CheckPlayerInVision();

        if (playerInSight && !isChasing)
        {
            StartCoroutine(AlertAndChase());
        }

        if(Caught)
            agent.SetDestination(player.transform.position);

        SliderMain.transform.rotation = Quaternion.Euler(0, cam.transform.eulerAngles.y, 0);
    }

    

    void TakeControl()
    {
        if(playerController.Levelindex >= LevelIndex)
        {
            if (!player.GetComponent<PlayerController>().controlingLife)
            {
                isControlled = true;
                Consume(true);               
                agent.enabled = false; // Disable NavMeshAgent
                player.GetComponent<PlayerController>().controlingLife = true;
                player.transform.SetParent(transform);
                player.transform.localPosition = Vector3.zero;
                player.SetActive(false); // Hide player
                canSwitch = false;
                controlTimer = controlTime;
            }

        }
    }
    void SwitchControl()
    {
        if (playerController.Levelindex >= LevelIndex)
        {
            ReleaseControl();
            temp.GetComponent<NPCController>().TakeControl();
        }
           

    }
    public bool Alert;
    public void ReleaseControl()
    {
        if (player.GetComponent<PlayerController>().controlingLife)
        {
            isControlled = false;
            Consume(false);
            player.transform.SetParent(null);
            player.GetComponent<PlayerController>().controlingLife = false;
            player.SetActive(true); // Show player again
            agent.enabled = true; // Re-enable NavMeshAgent
            player.GetComponent<SphereCollider>().isTrigger = true;
            LeanTween.delayedCall(1f, () => { player.GetComponent<SphereCollider>().isTrigger = false; });
            if(!Alert)
                StartCoroutine(ShortStun());
        }

    }
    [Header("HealthUI")]
    public GameObject SliderMain;
    public Slider ControlTimeSlider;
    public void Consume(bool value)
    {      
        SliderMain.SetActive(value);
    }

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
    
    private IEnumerator AlertAndChase()
    {
        isChasing = true;
        if(!Caught)
            agent.isStopped = true;
        yield return new WaitForSeconds(alertDelay);
        Caught = true;
        agent.isStopped = false;
        agent.speed = chaseSpeed;
        agent.SetDestination(player.transform.position);
    }
    public void PlayerFound()
    {
        Caught = true;      
        agent.speed = chaseSpeed;
        agent.SetDestination(player.transform.position);
    }
    public void StunMethod()
    {
        StartCoroutine(StunNPC());
    }
    private IEnumerator StunNPC()
    {
        isStunned = true;
        agent.isStopped = true;
        yield return new WaitForSeconds(stunDuration);
        agent.isStopped = false;
        StartCoroutine(SearchForPlayer());
    }
    public IEnumerator ShortStun()
    {
        isStunned = true;
        agent.isStopped = true;
        yield return new WaitForSeconds(stunDuration);
        agent.isStopped = false;
        isStunned = false;
        Alert = false;
        controlTimer = controlTime;
        MoveToNextPatrol();
    }
    private IEnumerator SearchForPlayer()
    {
        isSearching = true;
        agent.speed = chaseSpeed;
        float elapsedTime = 0;
        startPosition = transform.position; // Store initial position
        while (elapsedTime < searchTime)
        {
            if (CheckPlayerInVision())
            {
                //StartCoroutine(AlertAndChase());
                isSearching = false;
                yield break;
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        MoveToNextPatrol();
        agent.speed = normalSpeed;
        isSearching = false;
        isStunned = false;
        controlTimer = controlTime;


    }
    public void ChangeSpeed()
    {
        agent.speed = chaseSpeed;
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
    private void MoveToNextPatrol()
    {
        if (patrolPoints.Length == 0 && !Caught)
            return;
       
        agent.enabled = true;
        agent.speed = normalSpeed;
        agent.SetDestination(patrolPoints[currentPatrolIndex].position);
        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
    }
    private bool CheckPlayerInVision()
    {
        if (!player) return false;

        Vector3 directionToPlayer = (player.transform.position - visionOrigin.position).normalized;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

        if (angleToPlayer < visionAngle && Vector3.Distance(transform.position, player.transform.position) < visionRange)
        {
            if (Physics.Raycast(visionOrigin.position, directionToPlayer, out RaycastHit hit, visionRange))
            {
                if (hit.collider.gameObject == player)
                    return true;
            }
        }
        return false;
    }

    private void OnTriggerStay(Collider other)
    {

        if (other.CompareTag("Enemy") && isControlled)
        {
            canSwitch = true;
            temp = other.gameObject;
            other.GetComponent<Outlinable>().enabled = true;

        }

        if (other.CompareTag("Enemy") && !isControlled)
        {
            interacting = true;
           
        }
        
    }

    private void OnTriggerExit(Collider other)
    {

        if (other.CompareTag("Enemy") && isControlled)
        {

            canSwitch = false;
            temp = null;
            other.GetComponent<Outlinable>().enabled = false;

        }
        if (other.CompareTag("Enemy") && !isControlled)
        {
            interacting = false;
        }
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(startPosition == Vector3.zero ? transform.position : startPosition, roamRadius);
    }

    private void OnDrawGizmosSelected()
    {
        if (!visionOrigin) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, visionRange);

        Vector3 leftLimit = Quaternion.Euler(0, -visionAngle, 0) * transform.forward;
        Vector3 rightLimit = Quaternion.Euler(0, visionAngle, 0) * transform.forward;

        Gizmos.DrawLine(visionOrigin.position, visionOrigin.position + leftLimit * visionRange);
        Gizmos.DrawLine(visionOrigin.position, visionOrigin.position + rightLimit * visionRange);
    }
}