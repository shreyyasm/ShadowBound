using EPOOutline;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
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
    public bool isControlled = false;
    public bool interacting;
    public bool canSwitch;

    [Header("PatrolStats")]
    public Transform[] patrolPoints; // List of patrol points
    private int currentPatrolIndex = 0;
    private bool isChasing = false;
    public bool isStunned = false;
    public bool isSearching = false;

    [Header("Vision Settings")]
    public float visionRange = 10f;
    public float visionAngle = 45f;
    public Transform visionOrigin;

    [Header("Chase Settings")]
    public bool Caught;
    public bool Alert;
    public float chaseSpeed = 4f;
    public float normalSpeed = 2f;
    public float alertDelay = 2f;
    public AudioClip alertSound;
    public GameObject alertSign;

    [Header("Control & Stun")]
    public float controlTime = 5f; // Time before NPC gets stunned if controlled
    public float stunDuration = 3f;
    public float searchTime = 4f;
    public float controlTimer = 0f;
    private bool playerInSight;

    [Header("Reference")]
    public GameObject LightRay;
    public Abilities abilities;
    public GameObject sprite;
    public AudioSource audioSource;
    public AudioSource FootaudioSource;
    public GameObject interactSign;
    public GameObject temp;


    [Header("HealthUI")]
    public GameObject SliderMain;
    public Slider ControlTimeSlider;

    [Header("VFX")]
    public GameObject PossesVFX;
    public GameObject PlayerPossesVFX;
    public GameObject PossesOutVFX;
    public AudioClip PossesdSFX;
    public AudioClip PlayerPossesSFX;
    public AudioClip PossesOutSFX;
    public Transform VFXPos;



    //Hidden
    public Vector3 moveDirection;
    [HideInInspector]
    public Rigidbody rb;
    private Vector3 isoForward = new Vector3(1, 0, 1).normalized;
    private Vector3 isoRight = new Vector3(1, 0, -1).normalized;
    private GameObject player;
    private PlayerController playerController;
    private Camera cam;
    Animator animator;
   
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
       

    }
    void Start()
    {
        LoadplayerData();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        cam = Camera.main;
        ControlTimeSlider.maxValue  = controlTime;
        controlTimer = controlTime;
        nextMoveTime = Time.time + roamDelay;
        player = GameObject.FindGameObjectWithTag("Player");
        playerController = player.GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody>();
        rb.drag = 5f;
        rb.freezeRotation = true; // Prevent rotation due to physics
        FootaudioSource.Play();
       FootaudioSource.Pause();
        agent.speed = normalSpeed;
        MoveToNextPatrol();
    }
    bool check;
 
    void Update()
    {
        
        SliderMain.transform.rotation = Quaternion.Euler(0, cam.transform.eulerAngles.y, 0);
        //interactSign.transform.rotation = Quaternion.Euler(30, 45, 0);
        sprite.transform.rotation = Quaternion.Euler(30, 45, 0);
        //Control
        if (interacting && Input.GetMouseButtonDown(1) && !isStunned && !isControlled && !abilities.usingAbility && !check && !Caught)
        {
            TakeControl();
        }
        else if(!canSwitch && !interacting && Input.GetMouseButtonDown(1) && isControlled )
        {
            ReleaseControl();
            Debug.Log("Release" + gameObject.name);
        }


        if (canSwitch && Input.GetMouseButtonDown(1) && isControlled && !abilities.usingAbility)
        {
           
            SwitchControl();
            Debug.Log("Switch" + gameObject.name); 
        }

        if (isControlled)
        {
            if (!abilities.isDashing && !abilities.isRotating)
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
                player.transform.rotation = Quaternion.Euler(0, 0, 0);
                HandleMovement();
                agent.enabled = false;
            }
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
        if (Caught)
            agent.SetDestination(player.transform.position);
        //AI
        if (!isControlled)
        {
            if (Time.time >= nextMoveTime && !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance && isSearching && !Caught) 
            {
                MoveToRandomPoint();
                nextMoveTime = Time.time + roamDelay;
            }
        }
      
        if (isStunned || isSearching || Waiting)
            return;
        if (!isChasing && !agent.pathPending && agent.remainingDistance < 0.5f && !Caught)
        {
            StartCoroutine(WaitBeforeMoving());
            //MoveToNextPatrol();
        }
        playerInSight = CheckPlayerInVision();

        if (playerInSight && !isChasing)
        {
            StartCoroutine(AlertAndChase());
        }

       
     
        if(!isControlled)
        {
            if (transform.rotation.eulerAngles.y > 90 && transform.rotation.eulerAngles.y < 270)
            {
                sprite.GetComponent<SpriteRenderer>().flipX = false;
               
            }
                
            else
            {
                sprite.GetComponent<SpriteRenderer>().flipX = true;
            }
               

        }
        
    }
    
   
    void TakeControl()
    {
        if(playerController.Levelindex >= LevelIndex)
        {
            if (!player.GetComponent<PlayerController>().controlingLife)
            {
                StartCoroutine(TakeControlEnum());
               
            }

        }
    }
    public GameObject enemyLight;
    IEnumerator TakeControlEnum()
    {
        //playerController.animator.SetBool("IsConsuming", true);
      
        agent.isStopped = true;
        check = true;
        player.SetActive(false); // Hide player
        audioSource.PlayOneShot(PlayerPossesSFX);
        interactSign.SetActive(false);
        Instantiate(PlayerPossesVFX, playerController.VFXPos.position, Quaternion.identity);
        animator.SetBool("IsWalking",false);

        yield return new WaitForSeconds(0.8f);

        enemyLight.SetActive(true);
        abilities.ResetAbilities();
        abilities.BigViewCamera.SetActive(false);
        abilities.container.gameObject.SetActive(true);
        //playerController.animator.SetBool("IsConsuming", false);
        playerController.animator.SetBool("IsWalking", true);
        audioSource.PlayOneShot(PossesdSFX);
        animator.SetLayerWeight(1, 1);
        isControlled = true;
        Consume(true);
        GameObject newGameObject = Instantiate(PossesVFX, VFXPos.position, Quaternion.identity);
        newGameObject.transform.SetParent(transform);
        LightRay.SetActive(false);
        agent.enabled = false; // Disable NavMeshAgent
        player.GetComponent<PlayerController>().controlingLife = true;
        player.transform.SetParent(transform);
        player.transform.localPosition = Vector3.zero;
        player.SetActive(false); // Hide player
        canSwitch = false;
        controlTimer = controlTime;
    }
    public void SwitchControl()
    {
        if (playerController.Levelindex >= LevelIndex)
        {
            abilities.currentAbilityIndex = 0;
            
            ReleaseControl();
            temp.GetComponent<NPCController>().TakeControl();
          
        }
           

    }
   
    public void ReleaseControl()
    {
        if (player.GetComponent<PlayerController>().controlingLife)
        {
            abilities.container.gameObject.SetActive(false);
            enemyLight.SetActive(false);
            FootaudioSource.Pause();
            check = false;
            GameObject newGameObject = Instantiate(PossesOutVFX, VFXPos.position - new Vector3(0.2f,1f,0), Quaternion.identity);
            newGameObject.transform.SetParent(transform);
            audioSource.PlayOneShot(PlayerPossesSFX);
            //audioSource.PlayOneShot(PossesdSFX);
            animator.SetLayerWeight(1, 0);
            isControlled = false;
            Consume(false);
            LightRay.SetActive(true);
            player.transform.SetParent(null);
            player.GetComponent<PlayerController>().controlingLife = false;
            player.SetActive(true); // Show player again
            agent.enabled = true; // Re-enable NavMeshAgent
            player.GetComponent<SphereCollider>().isTrigger = true;
            abilities.ReleaseAbility();
            sprite.GetComponent<SpriteRenderer>().flipX = true;
            LeanTween.delayedCall(1f, () => { player.GetComponent<SphereCollider>().isTrigger = false; });
            if(!Alert)
                StartCoroutine(ShortStun());
        }

    }

    public void Consume(bool value)
    {      
        SliderMain.SetActive(value);
    }

    void FixedUpdate()
    {
        if (isControlled)
        {
            interacting = false;

            if (abilities.isDashing)
                return; // Do nothing while dashing

            if (moveDirection.magnitude >= 0.1f && abilities.canTeleport)
            {
                rb.velocity = new Vector3(moveDirection.x * moveSpeed, rb.velocity.y, moveDirection.z * moveSpeed);
            }
            else
            {
               if(abilities.canTeleport)
                    rb.velocity = new Vector3(0, rb.velocity.y, 0); // Stop drifting
            }
        }
    
    }
  
    void HandleMovement()
    {
        
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        moveDirection = (isoRight * horizontal + isoForward * vertical).normalized;

        if (moveDirection.magnitude >= 0.1f && abilities.canTeleport)
        {
            rb.velocity = new Vector3(moveDirection.x * moveSpeed, rb.velocity.y, moveDirection.z * moveSpeed);
            animator.SetBool("IsWalking", true);
            FootaudioSource.UnPause();
            if (moveDirection.x < 0 || moveDirection.z >0)
                sprite.GetComponent<SpriteRenderer>().flipX = true;
                
            if(moveDirection.x > 0 || moveDirection.z < 0)
                sprite.GetComponent<SpriteRenderer>().flipX = false;
  
        }
        else
        {
           FootaudioSource.Pause();
            animator.SetBool("IsWalking", false);
        }
       
    }
    public GameObject postProcessVolumeAlert;
    private IEnumerator AlertAndChase()
    {
        if(!Caught)
        {
            postProcessVolumeAlert.SetActive(true);
            abilities.BigViewCamera.SetActive(true);
            animator.SetBool("IsWalking", false);
            alertSign.SetActive(true);
            audioSource.PlayOneShot(alertSound);
            agent.isStopped = true;
            LeanTween.delayedCall(1.5f, () => { Transitioner.Instance.TransitionToFix(); });

            LeanTween.delayedCall(2.5f, () => { SceneManager.LoadScene("Level " + LevelClear.Instance.CurrentChapter); });
            Caught = true;
        }
        

           
        yield return new WaitForSeconds(alertDelay);
      
    }
    public void PlayerFound()
    {
        Caught = true;      
        agent.speed = chaseSpeed;
        StartCoroutine(AlertAndChase());
    }
    public void StunMethod()
    {
        StartCoroutine(StunNPC());
    }
    private IEnumerator StunNPC()
    {
        audioSource.PlayOneShot(alertSound);
        isStunned = true;
        agent.isStopped = true;
        abilities.container.gameObject.SetActive(false);
        abilities.ResetAbilities();
        abilities.BigViewCamera.SetActive(false);
        alertSign.SetActive(true);
        yield return new WaitForSeconds(stunDuration);
        animator.SetBool("IsWalking", true);
        agent.isStopped = false;
        StartCoroutine(SearchForPlayer());
       
    }
    public IEnumerator ShortStun()
    {
        isStunned = true;
        agent.isStopped = true;
        animator.SetBool("IsWalking", false);
        yield return new WaitForSeconds(stunDuration);
        agent.isStopped = false;
        isStunned = false;
        Alert = false;
        controlTimer = controlTime;
        MoveToNextPatrol();
    }
    public bool Waiting;
    public IEnumerator WaitBeforeMoving()
    {
        Waiting = true;
        agent.isStopped = true;
        animator.SetBool("IsWalking", false);
        yield return new WaitForSeconds(stunDuration);
        if(agent.enabled)
            agent.isStopped = false;
        Waiting = false;
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
                
                StartCoroutine(AlertAndChase());
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
        alertSign.SetActive(false);

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
        agent.isStopped = false;
        agent.speed = normalSpeed;
        agent.SetDestination(patrolPoints[currentPatrolIndex].position);
        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        animator.SetBool("IsWalking", true);


    }
    private bool CheckPlayerInVision()
    {
        if (!player) return false;

        Vector3 directionToPlayer = (player.transform.position - visionOrigin.position).normalized;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        if (angleToPlayer < visionAngle && distanceToPlayer < visionRange)
        {
            if (Physics.Raycast(visionOrigin.position, directionToPlayer, out RaycastHit hit, visionRange))
            {
                Debug.DrawRay(visionOrigin.position, directionToPlayer * visionRange, Color.red, 0.1f); // Draw debug ray

                if (hit.collider.gameObject == player)
                {
                    Debug.Log("Player detected!"); // Debugging
                    if (!Caught)
                    {
                        postProcessVolumeAlert.SetActive(true);
                        abilities.BigViewCamera.SetActive(true);
                        animator.SetBool("IsWalking", false);
                        alertSign.SetActive(true);
                        audioSource.PlayOneShot(alertSound);
                        agent.isStopped = true;
                        LeanTween.delayedCall(1.5f, () => { Transitioner.Instance.TransitionToFix(); });

                        LeanTween.delayedCall(2.5f, () => { SceneManager.LoadScene("Level " + LevelClear.Instance.CurrentChapter); });
                        Caught = true;
                    }
                    return true;
                }
                else
                {
                    Debug.Log("Raycast hit: " + hit.collider.gameObject.name); // Debugging
                }
            }
            else
            {
                Debug.Log("Raycast did not hit anything");
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
           
        }

        if (other.CompareTag("Enemy") && !isControlled && other.GetComponent<NPCController>().isControlled)
        {
            interacting = true;
            interactSign.SetActive(true);

        }
        
    }

    private void OnTriggerExit(Collider other)
    {

        if (other.CompareTag("Enemy") && isControlled)
        {

            canSwitch = false;
            temp = null;
           
        }
        if (other.CompareTag("Enemy") && !isControlled && other.GetComponent<NPCController>().isControlled)
        {
            interacting = false;
            interactSign.SetActive(false);
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