using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    public List<PlayerData> playerStatsData;
    public PlayerStats playerStats;
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
    public PlayerHealth playerHealth;

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
        Cursor.visible = false;
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
                playerStats = i.stats;
                moveSpeed = i.stats.MoveSpeed;
                dashSpeed = i.stats.DashSpeed;
                playerHealth.maxHealth = i.stats.MaxHealth;
                playerHealth.healthModifer = i.stats.HealthModifer;
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
        }
        else
        {
            rb.velocity = new Vector3(0, rb.velocity.y, 0); // Stop drifting
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

}
