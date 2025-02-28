using EPOOutline;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    public PlayerStats playerStatsData;
    public string Level;
    public int Levelindex;

    [Header("MovementStats")]
    public float moveSpeed = 5f;
    public float dashSpeed = 15f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;

    [Header("Interaction")]
    public bool controlingLife;

    [Header("Reference")]
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    public Transform VFXPos;

    private Vector3 moveDirection;
    private Rigidbody rb;
    private bool isDashing = false;
    private float dashTime;
    private float lastDashTime;

    private Vector3 isoForward = new Vector3(1, 0, 1).normalized;
    private Vector3 isoRight = new Vector3(1, 0, -1).normalized;

    AudioSource audioSource;
 
 
    private void Awake()
    {
        LoadplayerData();
    }
    void Start()
    {
        //PlayerPrefs.SetString("PlayerStats", "Level 1");
        //playerLevel = PlayerPrefs.GetString("PlayerStats");
       audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody>();
        rb.drag = 5f;
        rb.freezeRotation = true; // Prevent rotation due to physics

    }
    public void LoadplayerData()
    {
        
                Level = playerStatsData.Level;
                Levelindex = playerStatsData.LevelIndex;
                moveSpeed = playerStatsData.MoveSpeed;
                dashSpeed = playerStatsData.DashSpeed;
            
        
    } 
    void Update()
    {
        
        if (!isDashing)
        {
            HandleMovement();
        }

        HandleDash();
    }
    void FixedUpdate()
    {
        if (isDashing)
        {
            return; // Do nothing while dashing
        }

        if (moveDirection.magnitude >= 0.1f)
        {
            rb.velocity = new Vector3(moveDirection.x * moveSpeed, rb.velocity.y, moveDirection.z * moveSpeed);
            audioSource.UnPause();
        }
        else
        {
            rb.velocity = new Vector3(0, rb.velocity.y, 0); // Stop drifting
            audioSource.Pause();
        }
    }
    //public float moveDir;
    void HandleMovement()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        moveDirection = (isoRight * horizontal + isoForward * vertical).normalized;
        //moveDir = moveDirection.x;
        if (moveDirection.magnitude >= 0.1f)
        {
            rb.velocity = new Vector3(moveDirection.x * moveSpeed, rb.velocity.y, moveDirection.z * moveSpeed);

            if (moveDirection.x < 0 || moveDirection.z > 0)

                spriteRenderer.flipX = false;

            if (moveDirection.x > 0 || moveDirection.z < 0)
                spriteRenderer.flipX = true;

            animator.SetBool("IsWalking", true);
        }
        else
        {
            animator.SetBool("IsWalking", false);
        }
    }

    void HandleDash()
    {
        if (Input.GetKeyDown(KeyCode.Space) && Time.time > lastDashTime + dashCooldown && moveDirection != Vector3.zero)
        {
            animator.SetBool("IsDashing", true);
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
                animator.SetBool("IsDashing", false);
               
            }
        }
    }

    public NPCController closestEnemy;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Enemy") && !other.GetComponent<NPCController>().isStunned)
        {
            FindClosestEnemy();
        }
    }

    private void FindClosestEnemy()
    {
        NPCController[] enemies = FindObjectsOfType<NPCController>(); // Get all enemies
        float minDistance = float.MaxValue;
        NPCController nearestEnemy = null;

        foreach (NPCController enemy in enemies)
        {
            if (enemy != null && !enemy.isStunned && !enemy.isControlled)
            {
                float distance = Vector3.Distance(transform.position, enemy.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestEnemy = enemy;
                }
            }
        }

        // Only update if the closest enemy changes
        if (nearestEnemy != closestEnemy)
        {
            if (closestEnemy != null)
            {
                closestEnemy.interacting = false;
                closestEnemy.interactSign.SetActive(false); // Remove interaction from previous enemy
            }

            closestEnemy = nearestEnemy;

            if (closestEnemy != null && !closestEnemy.Caught)
            {
                closestEnemy.interacting = true;
                closestEnemy.interactSign.SetActive(true); // Show interaction sign for new closest enemy
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy") && closestEnemy == other.GetComponent<NPCController>())
        {
            closestEnemy.interacting = false;
            closestEnemy.interactSign.SetActive(false);
            closestEnemy = null;
        }
    }

}
