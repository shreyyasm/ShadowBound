using EPOOutline;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    public List<PlayerData> playerStatsData;
    public PlayerStats playerStats;
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
    [System.Serializable]
    public class PlayerData
    {
        public string PlayerLevel;
        public PlayerStats stats;
    }
 
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
        foreach (PlayerData i in playerStatsData)
        {
            if (i.PlayerLevel == PlayerPrefs.GetString("PlayerStats"))
            {
                Level = i.stats.Level;
                Levelindex = i.stats.LevelIndex;
                playerStats = i.stats;
                moveSpeed = i.stats.MoveSpeed;
                dashSpeed = i.stats.DashSpeed;
            }
        }
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
 
    private NPCController closestEnemy;
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Enemy") && !other.GetComponent<NPCController>().isStunned)
        {
            NPCController[] enemies = FindObjectsOfType<NPCController>(); // Get all enemies
            float minDistance = float.MaxValue;
            NPCController nearestEnemy = null;

            foreach (NPCController enemy in enemies)
            {
                if (enemy != null && enemy != this && !enemy.isControlled)
                {
                    float distance = Vector3.Distance(transform.position, enemy.transform.position);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        nearestEnemy = enemy;
                    }
                }
            }

            closestEnemy = nearestEnemy; // Assign the closest enemy
            closestEnemy.interacting = true;
            closestEnemy.interactSign.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<NPCController>().interacting = false;
            other.GetComponent<NPCController>().interactSign.SetActive(false);
        }
        if (closestEnemy != null && other.gameObject == closestEnemy.gameObject)
        {
            closestEnemy = null;
        }
    }

}
