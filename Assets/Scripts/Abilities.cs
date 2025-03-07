using EPOOutline;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.EventSystems.EventTrigger;

public class Abilities : MonoBehaviour
{
    public GameData EnemyStatsManager;
    public int currentAbilityIndex = 0;
    public List<AbiltiesStats> Playerabilities = new List<AbiltiesStats>();

    [Header("Unlocked Abilities")]
    public bool Dash;
    public bool MoveObjects;
    public bool RotateObjects;
    public bool TeleportPlayer;
    public bool DistanceConsume;

    public bool usingAbility;

    [Header("Dash Ability")]
    public float dashSpeed = 15f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;
    public bool isDashing = false;
    private float dashTime;
    private float lastDashTime;
    public GameObject DashTrail;
    public AudioClip DashSFX;

    [Header("Move Ability")]
    public Transform grabPoint; // Where the object will be held
    public float grabRange = 3f; // Distance for raycast detection
    public LayerMask grabbableLayer; // Define what objects can be grabbed
    public GameObject grabbedObject;
    private FixedJoint grabJoint;
    public float grabHeightOffset = 1.5f;
    public bool MovingObject;
    public GameObject MoveVFX;
    public Transform VFXPos;
    public AudioSource MoveBeamSFX;
    public AudioClip beamStart;
    public AudioClip beamStop;
    private Hovl_Laser LaserScript;
    private Hovl_Laser2 LaserScript2;
    public bool canMove;

    [Header("Rotate Ability")]
    public float rotationSnapAngle = 90f;
    public bool isRotating = false;

    [Header("Teleport Ability")]
    public float teleportRadius = 5f;
    public float teleportCooldown = 3f;   
    public bool canTeleport = true;
    public Transform teleportIndicator; // Child object to indicate teleport position
    public AudioClip TeleportInSFX;
    public AudioClip TeleportOutSFX;
    public AudioClip TeleportOnSFX;
    public GameObject TeleportVFX;

    [Header("Distant Consume Ability")]
    public float snapThreshold = 1.5f; // Distance for snapping to enemy
    public LayerMask enemyLayer;
    private Transform controlledEnemy;
    private Transform snappedEnemy;
    public GameObject BigViewCamera;

    [Header("Reference")]
    public Animator animator;
    public NPCController NPCController;
    public GameObject RayPos;
    public AudioSource audioSource;

 
    private void Start()
    {
       

        LeanTween.delayedCall(1f, () => {
            Dash = EnemyStatsManager.enemyStats.AbilityUnlocked[0];
            MoveObjects = EnemyStatsManager.enemyStats.AbilityUnlocked[1];
            RotateObjects = EnemyStatsManager.enemyStats.AbilityUnlocked[2];
            TeleportPlayer = EnemyStatsManager.enemyStats.AbilityUnlocked[3];
            DistanceConsume = EnemyStatsManager.enemyStats.AbilityUnlocked[4];

            Playerabilities[0].AbilityUnlocked = EnemyStatsManager.enemyStats.AbilityUnlocked[0];
            Playerabilities[1].AbilityUnlocked = EnemyStatsManager.enemyStats.AbilityUnlocked[1];
            Playerabilities[2].AbilityUnlocked = EnemyStatsManager.enemyStats.AbilityUnlocked[2];
            Playerabilities[3].AbilityUnlocked = EnemyStatsManager.enemyStats.AbilityUnlocked[3];
            Playerabilities[4].AbilityUnlocked = EnemyStatsManager.enemyStats.AbilityUnlocked[4]; 
            LoadAbilityCards(); });
      
    }

    private void Update()
    {
        SelectEnemyWithMouse();
        UpdateTeleportIndicator();

        if (NPCController.isControlled)
            UpdateCardUI();

        

        if (Input.GetMouseButtonDown(0) && NPCController.isControlled && !isRotating && Playerabilities[3].AbilityState)
        {
            if (teleportIndicator.gameObject.GetComponent<TeleportPlatform>() != null && teleportIndicator.gameObject.GetComponent<TeleportPlatform>().canTeleport)               
                StartCoroutine(Teleport());
        }

        if (!usingAbility && NPCController.isControlled)
            HandleAbilitySelection();

        if (NPCController.isControlled && Playerabilities[0].AbilityState)
        {
            if (!isRotating)
                HandleDash();
        }

        if (Input.GetMouseButtonDown(0) && NPCController.isControlled && !isRotating && Playerabilities[1].AbilityState && canMove)
        {
            if (!MovingObject)
            {
                GrabObject(false);
            }

            else
                ReleaseObject();
        }

        if (Input.GetMouseButtonDown(0) && NPCController.isControlled && !MovingObject && Playerabilities[2].AbilityState)
        {
            if (!isRotating)
            {
                GrabObject(true);
            }
            else
                ReleaseObject();
        }

        if (isRotating && NPCController.isControlled)
            RotateObject();

    

    }
    private void FixedUpdate()
    {
       
        if (grabbedObject != null && isRotating)
        {
            // Keep the object floating in front of the player
            grabbedObject.transform.position = new Vector3(grabPoint.transform.position.x, 2f, grabPoint.transform.position.z);
        }
    }

    void HandleDash()
    {
        if (Dash)
        {
            if (Input.GetMouseButtonDown(0) && Time.time > lastDashTime + dashCooldown && NPCController.moveDirection != Vector3.zero)
            {
                isDashing = true;
                DashTrail.SetActive(true);
                audioSource.PlayOneShot(DashSFX);
                usingAbility = true;
                dashTime = Time.time + dashDuration;
                lastDashTime = Time.time;
                NPCController.rb.velocity = NPCController.moveDirection * dashSpeed; // Apply dash velocity
            }

            if (isDashing)
            {
                if (Time.time >= dashTime)
                {
                    isDashing = false;
                    usingAbility = false;
                    DashTrail.SetActive(false);
                }
                    
            }
        }

    }
    private GameObject Instance;
    public void GrabObject(bool value)
    {
        if (grabbedObject != null && !MovingObject)
        {
            audioSource.PlayOneShot(beamStart,0.5f);
            isRotating = value;
            MoveBeamSFX.Play();
            Destroy(Instance);
            Instance = Instantiate(MoveVFX, VFXPos.position, Quaternion.identity);
            Instance.transform.parent = transform;
            LaserScript = Instance.GetComponent<Hovl_Laser>();
            LaserScript2 = Instance.GetComponent<Hovl_Laser2>();

            if (!isRotating)
            {
                

                MovingObject = true;
                usingAbility = true;
                grabbedObject.transform.position = grabPoint.position + Vector3.up * grabHeightOffset;

                // Attach object using FixedJoint
                grabJoint = gameObject.AddComponent<FixedJoint>();
                grabbedObject.GetComponent<Rigidbody>().isKinematic = false;
                grabJoint.connectedBody = grabbedObject.GetComponent<Rigidbody>();
                grabJoint.breakForce = Mathf.Infinity;
                grabJoint.breakTorque = Mathf.Infinity;

                // Disable gravity for smoother holding
                grabbedObject.GetComponent<Rigidbody>().useGravity = false;

            }

                
        }
    }

    void RotateObject()
    {
        usingAbility = true;
        if (grabbedObject != null)
        {
            //if (Input.GetKeyDown(KeyCode.W))
            //    SnapRotate(grabbedObject.transform.right);
            //if (Input.GetKeyDown(KeyCode.S))
            //    SnapRotate(-grabbedObject.transform.right);
            if (Input.GetKeyDown(KeyCode.A))
                SnapRotate(Vector3.up);
            if (Input.GetKeyDown(KeyCode.D))
                SnapRotate(-Vector3.up);

          
        }
    }

    void SnapRotate(Vector3 axis)
    {
        grabbedObject.transform.Rotate(axis, rotationSnapAngle, Space.World);
        grabbedObject.GetComponent<Grabbable>().InteractSign.transform.rotation = Quaternion.Euler(30, 45, 0);
    }


    void ReleaseObject()
    {
        if (grabbedObject != null)
        {
            audioSource.PlayOneShot(beamStop,0.5f);
            MoveBeamSFX.Stop();
            if (LaserScript) LaserScript.DisablePrepare();
            if (LaserScript2) LaserScript2.DisablePrepare();
            Destroy(Instance);
            Destroy(grabJoint); // Remove FixedJoint
            grabbedObject.GetComponent<Rigidbody>().useGravity = true; // Restore gravity
            grabbedObject.GetComponent<Rigidbody>().isKinematic = false;
            grabbedObject.GetComponent<Grabbable>().InteractSign.SetActive(false);
            isRotating = false;
            MovingObject = false;
            grabbedObject = null;
            usingAbility = false;
        }
    }
    void OnDrawGizmos()
    {
        if (Playerabilities[3].AbilityState)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, teleportRadius);

            if (teleportIndicator != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(teleportIndicator.position, 0.2f);
            }
        }
        

        if (snappedEnemy != null &&  Playerabilities[4].AbilityState)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(snappedEnemy.position, 0.5f); // Clearer sphere
            Gizmos.DrawLine(transform.position, snappedEnemy.position); // Line from player to enemy
        }
    }
    
    public void ReleaseAbility()
    {
        usingAbility = false;
        ReleaseObject();
    }
   
    IEnumerator Teleport()
    {
        if (!canTeleport) yield break;

        canTeleport = false;
        ///animator.SetTrigger("TeleportStart");
        Instantiate(TeleportVFX, teleportIndicator.position + new Vector3(0, 1f, 0), Quaternion.identity);
        audioSource.PlayOneShot(TeleportInSFX, 1f);
        yield return new WaitForSeconds(1.2f); // Wait for animation to finish
       
        // Stop any movement before teleporting
        NPCController.rb.velocity = Vector3.zero;
        NPCController.rb.isKinematic = true; // Temporarily disable physics to avoid interference

        transform.position = teleportIndicator.position; // Move player instantly
        teleportIndicator.gameObject.SetActive(false);

        yield return new WaitForFixedUpdate(); // Ensure the physics update happens after teleport
        audioSource.PlayOneShot(TeleportOutSFX, 0.5f);
        NPCController.rb.isKinematic = false; // Re-enable physics

        yield return new WaitForSeconds(teleportCooldown);
        teleportIndicator.gameObject.GetComponent<TeleportPlatform>().canTeleport = false;
        audioSource.PlayOneShot(TeleportOnSFX, 1f);
        teleportIndicator.gameObject.SetActive(true);
        canTeleport = true;
    }
    void UpdateTeleportIndicator()
    {
        if (Playerabilities[3].AbilityState && NPCController.isControlled)
        {
            if (teleportIndicator == null) return;

            if (canTeleport)
            {
                teleportIndicator.gameObject.SetActive(true);
                Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                Plane playerPlane = new Plane(Vector3.up, transform.position);
                if (playerPlane.Raycast(mouseRay, out float hitDistance))
                {
                    Vector3 mousePosition = mouseRay.GetPoint(hitDistance);
                    Vector3 direction = (mousePosition - transform.position).normalized;
                    teleportIndicator.position = transform.position + direction * teleportRadius;
                   
                }
                
            }
        }
        else
            teleportIndicator.gameObject.SetActive(false);


    }
    GameObject enemy;
    void SelectEnemyWithMouse()
    {
        if(NPCController.isControlled)
        {
            if (Playerabilities[4].AbilityState)
            {

                BigViewCamera.SetActive(true);
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, Mathf.Infinity, enemyLayer))
                {

                    enemy = hit.collider.gameObject;
                    if (!enemy.gameObject.GetComponent<NPCController>().isControlled)
                    {
                        enemy.gameObject.GetComponent<NPCController>().interactSign.SetActive(true);

                        NPCController.temp = enemy.gameObject;
                        if (Input.GetMouseButtonDown(0) && NPCController.isControlled && !isRotating && Playerabilities[4].AbilityState)
                        {
                            NPCController.SwitchControl();
                        }
                    }

                }
                else
                {
                    if (enemy != null)
                        enemy.gameObject.GetComponent<NPCController>().interactSign.SetActive(false);

                    NPCController.temp = null;
                    enemy = null;
                }

            }
            if(!Playerabilities[4].AbilityState && NPCController.isControlled)
            {
                BigViewCamera.SetActive(false);
            }

        }


    }

    [System.Serializable]
    public class AbiltiesStats
    {
        public string AbilityName;
        public bool AbilityUnlocked;
        public bool AbilityState;
    }
    public List<Transform> abilityCards; // Drag UI Card Transforms in Inspector
    public Vector3 selectedScale = new Vector3(1.2f, 1.2f, 1f);
    public Vector3 normalScale = Vector3.one;
    private int previousAbilityIndex = -1;
    public float spacing = 100f; // Adjust for better alignment
    public AudioClip abilityChangeSound;

    public void ResetAbilities()
    {
        // Disable all abilities
        for (int i = 0; i < Playerabilities.Count; i++)
            Playerabilities[i].AbilityState = false;


        currentAbilityIndex = 0;
        Playerabilities[0].AbilityState = true;

        for (int i = 0; i < abilityCards.Count; i++)
            abilityCards[i].transform.position = CardsResetPos.position;
        

    }
    void HandleAbilitySelection()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0 && Playerabilities.Count > 0)
        {
            // Disable all abilities
            for (int i = 0; i < Playerabilities.Count; i++)
                Playerabilities[i].AbilityState = false;

            // Find the next unlocked ability
            int newIndex = currentAbilityIndex;
            do
            {
                newIndex = (newIndex + (scroll > 0 ? 1 : -1) + Playerabilities.Count) % Playerabilities.Count;
                audioSource.PlayOneShot(abilityChangeSound,0.5f);

               
            } while (!Playerabilities[newIndex].AbilityUnlocked);

            // Update ability state
            currentAbilityIndex = newIndex;
           

            // Enable selected ability
            Playerabilities[currentAbilityIndex].AbilityState = true;

            if (newIndex != currentAbilityIndex)
            {
                // Reset previous card scale
                if (previousAbilityIndex >= 0 && previousAbilityIndex < abilityCards.Count)
                {
                    abilityCards[previousAbilityIndex].localScale = normalScale;
                }
                
                // Scale up new selected card
                if (newIndex < abilityCards.Count)
                {
                    abilityCards[newIndex].localScale = selectedScale;
                }

                previousAbilityIndex = currentAbilityIndex;
                currentAbilityIndex = newIndex;
            }
        }
    }
    public void LoadAbilityCards()
    {
        for(int i = 0;i < Playerabilities.Count;i++)
        {
            if(Playerabilities[i].AbilityUnlocked)
            {
                abilityCards[i].gameObject.SetActive(true);
            }
        }
    }
    public RectTransform container; // Assign the UI container in the Inspector
    public Transform CardsResetPos;

    public void UpdateCardUI()
    {
        // List to store only active (unlocked) ability cards and their original indices
        List<Transform> activeCards = new List<Transform>();
        List<int> activeIndices = new List<int>(); // Store original indices

        for (int i = 0; i < Playerabilities.Count; i++)
        {
            if (Playerabilities[i].AbilityUnlocked && i < abilityCards.Count)
            {
                activeCards.Add(abilityCards[i]);
                activeIndices.Add(i); // Keep track of original index
            }
        }

        if (activeCards.Count == 0) return; // Exit if no active cards

        // Find the correct index of the selected ability within activeCards
        int selectedActiveIndex = activeIndices.IndexOf(currentAbilityIndex);

        // Calculate total width of active cards
        float totalWidth = 0f;
        List<float> cardWidths = new List<float>();

        for (int i = 0; i < activeCards.Count; i++)
        {
            bool isSelected = (i == selectedActiveIndex); // Check against mapped index
            Vector3 targetScale = isSelected ? selectedScale : normalScale;

            // Smoothly transition scale
            activeCards[i].localScale = Vector3.Lerp(activeCards[i].localScale, targetScale, Time.deltaTime * 10f);

            // Store width for alignment calculation
            float cardWidth = (isSelected ? selectedScale.x : normalScale.x) * activeCards[i].GetComponent<RectTransform>().sizeDelta.x;
            cardWidths.Add(cardWidth);
            totalWidth += cardWidth;
        }

        // Add spacing between cards
        totalWidth += (activeCards.Count - 1) * spacing;

        // Align cards to the right of the container
        float containerRightEdge = container.rect.width / 2f;
        float startX = containerRightEdge - totalWidth;

        for (int i = 0; i < activeCards.Count; i++)
        {
            bool isSelected = (i == selectedActiveIndex);
            float cardWidth = cardWidths[i];

            // Compute target position
            Vector3 targetPosition = new Vector3(startX + cardWidth / 2, activeCards[i].localPosition.y, activeCards[i].localPosition.z);

            // Smoothly move the cards
            activeCards[i].localPosition = Vector3.Lerp(activeCards[i].localPosition, targetPosition, Time.deltaTime * 10f);

            startX += cardWidth + spacing; // Move to next position
        }
    }
   
    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Grabble") && NPCController.isControlled && Playerabilities[1].AbilityState)
        {
            canMove = true;
            grabbedObject = other.gameObject;
            other.GetComponent<Grabbable>().InteractSign.SetActive(true);
        }
        if (other.CompareTag("Grabble") && NPCController.isControlled && Playerabilities[2].AbilityState)
        {
            canMove = true;
            grabbedObject = other.gameObject;
            other.GetComponent<Grabbable>().InteractSign.SetActive(true);
        }

    }

    private void OnTriggerStay(Collider other)
    {

        if (other.CompareTag("Grabble") && NPCController.isControlled && Playerabilities[1].AbilityState)
        {
            canMove = true;
            grabbedObject = other.gameObject;
            other.GetComponent<Grabbable>().InteractSign.SetActive(true);
        }
        if (other.CompareTag("Grabble") && NPCController.isControlled && Playerabilities[2].AbilityState)
        {
            canMove = true;
            grabbedObject = other.gameObject;
            other.GetComponent<Grabbable>().InteractSign.SetActive(true);
        }
        if (other.CompareTag("Grabble") && NPCController.isControlled && !Playerabilities[1].AbilityState &&  !Playerabilities[2].AbilityState)
        {
            canMove = false;
            grabbedObject = null;
            other.GetComponent<Grabbable>().InteractSign.SetActive(false);

        }


    }
    private void OnTriggerExit(Collider other)
    {

        if (other.CompareTag("Grabble") && !usingAbility)
        {
            canMove = false;
            grabbedObject = null;
            other.GetComponent<Grabbable>().InteractSign.SetActive(false);

        }
       
    }
}
