using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Abilities : MonoBehaviour
{
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
    private Rigidbody grabbedObject;
    private FixedJoint grabJoint;
    public float grabHeightOffset = 1.5f;
    public bool MovingObject;

    [Header("Rotate Ability")]
    public float rotationSnapAngle = 90f;
    public bool isRotating = false;

    [Header("Teleport Ability")]
    public float teleportRadius = 5f;
    public float teleportCooldown = 3f;   
    public bool canTeleport = true;
    public Transform teleportIndicator; // Child object to indicate teleport position

    [Header("Distant Consume Ability")]
    public float snapThreshold = 1.5f; // Distance for snapping to enemy
    public LayerMask enemyLayer;
    private Transform controlledEnemy;
    private Transform snappedEnemy;

    [Header("Reference")]
    public Animator animator;
    public NPCController NPCController;
    public GameObject RayPos;
    public AudioSource audioSource;

 
    private void Start()
    {
        Playerabilities[0].AbilityUnlocked = Dash;
        Playerabilities[1].AbilityUnlocked = MoveObjects; 
        Playerabilities[2].AbilityUnlocked = RotateObjects;
        Playerabilities[3].AbilityUnlocked = TeleportPlayer; 
        Playerabilities[4].AbilityUnlocked = DistanceConsume;
        LoadAbilityCards();
    }

    private void Update()
    {
        UpdateCardUI();
        SelectEnemyWithMouse();   
        UpdateTeleportIndicator();

        if (Input.GetMouseButtonDown(1) && NPCController.isControlled && !isRotating && Playerabilities[3].AbilityState)
        {
            StartCoroutine(Teleport());
        }

        if (!usingAbility && NPCController.isControlled)
            HandleAbilitySelection();

        if (NPCController.isControlled && Playerabilities[0].AbilityState)
        {
            if (!isRotating)
                HandleDash();
        }

        if (Input.GetMouseButtonDown(1) && NPCController.isControlled && !isRotating && Playerabilities[1].AbilityState)
        {
            if (grabbedObject == null)
                TryGrabObject(false);

            else
                ReleaseObject();
        }

        if (Input.GetMouseButtonDown(1) && NPCController.isControlled && !MovingObject && Playerabilities[2].AbilityState)
        {
            if (grabbedObject == null)
                TryGrabObject(true);

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
            grabbedObject.transform.position = new Vector3(grabbedObject.transform.position.x, 1.5f, grabbedObject.transform.position.z);
        }
    }

    void HandleDash()
    {
        if (Dash)
        {
            if (Input.GetMouseButtonDown(1) && Time.time > lastDashTime + dashCooldown && NPCController.moveDirection != Vector3.zero)
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
    
    void TryGrabObject(bool RotationValue)
    {

        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, grabRange, grabbableLayer))
        {
            Rigidbody objectRb = hit.collider.GetComponent<Rigidbody>();
            isRotating = RotationValue;
           
            if (!isRotating)
            {
                if (objectRb != null)
                {
                    MovingObject = true;
                    grabbedObject = objectRb;
                    usingAbility = true;
                    grabbedObject.transform.position = grabPoint.position + Vector3.up * grabHeightOffset;
                    // Attach object using FixedJoint
                    grabJoint = gameObject.AddComponent<FixedJoint>();
                    grabbedObject.GetComponent<Rigidbody>().isKinematic = false;
                    grabJoint.connectedBody = grabbedObject;
                    grabJoint.breakForce = Mathf.Infinity;
                    grabJoint.breakTorque = Mathf.Infinity;

                    // Disable gravity for smoother holding
                    grabbedObject.useGravity = false;
                }
            }
            else
            {
                if (objectRb != null)
                    grabbedObject = objectRb;
               
            }
        }
        if (Physics.Raycast(transform.position, -transform.forward, out hit, grabRange, grabbableLayer))
        {
            Rigidbody objectRb = hit.collider.GetComponent<Rigidbody>();
            isRotating = RotationValue;
            if (!isRotating)
            {
                if (objectRb != null)
                {
                    MovingObject = true;
                    grabbedObject = objectRb;
                    usingAbility = true;
                    grabbedObject.transform.position = grabPoint.position + Vector3.up * grabHeightOffset;
                    // Attach object using FixedJoint
                    grabJoint = gameObject.AddComponent<FixedJoint>();
                    grabbedObject.GetComponent<Rigidbody>().isKinematic = false;
                    grabJoint.connectedBody = grabbedObject;
                    grabJoint.breakForce = Mathf.Infinity;
                    grabJoint.breakTorque = Mathf.Infinity;

                    // Disable gravity for smoother holding
                    grabbedObject.useGravity = false;
                }
            }
            else
            {
                if (objectRb != null)
                    grabbedObject = objectRb;

            }
        }
        if (Physics.Raycast(transform.position, RayPos.transform.forward, out hit, grabRange, grabbableLayer))
        {
            Rigidbody objectRb = hit.collider.GetComponent<Rigidbody>();
            isRotating = RotationValue;
            if (!isRotating)
            {
                if (objectRb != null)
                {
                    MovingObject = true;
                    grabbedObject = objectRb;
                    usingAbility = true;
                    grabbedObject.transform.position = grabPoint.position + Vector3.up * grabHeightOffset;
                    // Attach object using FixedJoint
                    grabJoint = gameObject.AddComponent<FixedJoint>();
                    grabbedObject.GetComponent<Rigidbody>().isKinematic = false;
                    grabJoint.connectedBody = grabbedObject;
                    grabJoint.breakForce = Mathf.Infinity;
                    grabJoint.breakTorque = Mathf.Infinity;

                    // Disable gravity for smoother holding
                    grabbedObject.useGravity = false;
                }
            }
            else
            {
                if (objectRb != null)
                    grabbedObject = objectRb;

            }
        }
        if (Physics.Raycast(transform.position, -RayPos.transform.forward, out hit, grabRange, grabbableLayer))
        {
            Rigidbody objectRb = hit.collider.GetComponent<Rigidbody>();
            isRotating = RotationValue;
            if (!isRotating)
            {
                if (objectRb != null)
                {
                    MovingObject = true;
                    grabbedObject = objectRb;
                    usingAbility = true;
                    grabbedObject.transform.position = grabPoint.position + Vector3.up * grabHeightOffset;
                    // Attach object using FixedJoint
                    grabJoint = gameObject.AddComponent<FixedJoint>();
                    grabbedObject.GetComponent<Rigidbody>().isKinematic = false;
                    grabJoint.connectedBody = grabbedObject;
                    grabJoint.breakForce = Mathf.Infinity;
                    grabJoint.breakTorque = Mathf.Infinity;

                    // Disable gravity for smoother holding
                    grabbedObject.useGravity = false;
                }
            }
            else
            {
                if (objectRb != null)
                    grabbedObject = objectRb;

            }
        }
    }

    void RotateObject()
    {
        usingAbility = true;
        if (grabbedObject != null)
        {
            if (Input.GetKeyDown(KeyCode.W))
                SnapRotate(grabbedObject.transform.right);
            if (Input.GetKeyDown(KeyCode.S))
                SnapRotate(-grabbedObject.transform.right);
            if (Input.GetKeyDown(KeyCode.A))
                SnapRotate(Vector3.up);
            if (Input.GetKeyDown(KeyCode.D))
                SnapRotate(-Vector3.up);
        }
    }

    void SnapRotate(Vector3 axis)
    {
        grabbedObject.transform.Rotate(axis, rotationSnapAngle, Space.World);
    }


    void ReleaseObject()
    {
        if (grabbedObject != null)
        {
            Destroy(grabJoint); // Remove FixedJoint
            grabbedObject.useGravity = true; // Restore gravity
            grabbedObject.GetComponent<Rigidbody>().isKinematic = false;
            isRotating = false;
            MovingObject = false;
            grabbedObject = null;
            usingAbility = false;
        }
    }
    void OnDrawGizmos()
    {
        if(Playerabilities[2].AbilityState || Playerabilities[1].AbilityState)
        {
            // Draw raycast in the editor
            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position, transform.forward * grabRange);

            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position, -transform.forward * grabRange);

            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position, RayPos.transform.forward * grabRange);

            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position, -RayPos.transform.forward * grabRange);
        }
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

        yield return new WaitForSeconds(1f); // Wait for animation to finish

        // Stop any movement before teleporting
        NPCController.rb.velocity = Vector3.zero;
        NPCController.rb.isKinematic = true; // Temporarily disable physics to avoid interference

        transform.position = teleportIndicator.position; // Move player instantly

        yield return new WaitForFixedUpdate(); // Ensure the physics update happens after teleport

        NPCController.rb.isKinematic = false; // Re-enable physics

        yield return new WaitForSeconds(teleportCooldown);
        canTeleport = true;
    }
    void UpdateTeleportIndicator()
    {
        if (Playerabilities[3].AbilityState)
        {
            if (teleportIndicator == null) return;

            if (canTeleport)
            {
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
            
       
    }
   
    void SelectEnemyWithMouse()
    {
        if(Playerabilities[4].AbilityState)
        {
            Collider[] enemies = Physics.OverlapSphere(transform.position, Mathf.Infinity, enemyLayer);
            Transform closestEnemy = null;
            float closestDistance = float.MaxValue;

            Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            Plane plane = new Plane(Vector3.up, Vector3.zero);
            if (!plane.Raycast(mouseRay, out float enter)) return;

            Vector3 mouseWorldPos = mouseRay.GetPoint(enter);

            foreach (var enemy in enemies)
            {
                float distanceToMouse = Vector3.Distance(mouseWorldPos, enemy.transform.position);
                if (distanceToMouse < snapThreshold && distanceToMouse < closestDistance)
                {
                    closestDistance = distanceToMouse;
                    closestEnemy = enemy.transform;
                    NPCController.temp = closestEnemy.gameObject;
                    if (Input.GetMouseButtonDown(1) && NPCController.isControlled && !isRotating && Playerabilities[4].AbilityState)
                    {
                        NPCController.SwitchControl();
                    }
                }
            }

            if (closestEnemy != snappedEnemy)
            {
                snappedEnemy = closestEnemy;
            }

            // **Break snap if the mouse moves away**
            if (snappedEnemy != null)
            {
                float distanceToMouse = Vector3.Distance(mouseWorldPos, snappedEnemy.position);
                if (distanceToMouse > snapThreshold * 1.5f) // Add buffer to prevent flickering
                {
                    snappedEnemy = null;
                }
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
    public RectTransform cardContainer; // Assign the parent container (a UI Panel)
    public Vector3 selectedScale = new Vector3(1.2f, 1.2f, 1f);
    public Vector3 normalScale = Vector3.one;
    private int previousAbilityIndex = -1;
    public float spacing = 100f; // Adjust for better alignment
    public AudioClip abilityChangeSound;
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
                audioSource.PlayOneShot(abilityChangeSound);
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
    public void UpdateCardUI()
    {
        // List to store only active (unlocked) ability cards
        List<Transform> activeCards = new List<Transform>();

        for (int i = 0; i < Playerabilities.Count; i++)
        {
            if (Playerabilities[i].AbilityUnlocked && i < abilityCards.Count)
            {
                activeCards.Add(abilityCards[i]);
            }
        }

        if (activeCards.Count == 0) return; // Exit if no active cards

        // Calculate total width of active cards
        float totalWidth = 0f;
        List<float> cardWidths = new List<float>();

        for (int i = 0; i < activeCards.Count; i++)
        {
            bool isSelected = (i == currentAbilityIndex);
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
            bool isSelected = (i == currentAbilityIndex);
            float cardWidth = cardWidths[i];

            // Compute target position
            Vector3 targetPosition = new Vector3(startX + cardWidth / 2, activeCards[i].localPosition.y, activeCards[i].localPosition.z);

            // Smoothly move the cards
            activeCards[i].localPosition = Vector3.Lerp(activeCards[i].localPosition, targetPosition, Time.deltaTime * 10f);

            startX += cardWidth + spacing; // Move to next position
        }
    }
    
}
