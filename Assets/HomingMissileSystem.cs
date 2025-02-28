using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HomingMissileSystem : MonoBehaviour
{
    public static HomingMissileSystem instance;
    public GameObject missilePrefab;      // Missile prefab
    public Transform missileSpawnPoint;   // Where missiles launch from
    public GameObject blastEffectPrefab;  // Explosion effect prefab
    public float missileSpeed = 15f;      // Speed of the missile
    public float rotationSpeed = 8f;      // Rotation speed to track target
    public float missileLaunchDelay = 0.5f; // Delay between missile launches

    public AudioClip explosionSFX;
    private void Awake()
    {
        instance = this;
    }

    public void LaunchMissiles()
    {
        NPCController[] enemies = FindObjectsOfType<NPCController>();

        if (enemies.Length == 0)
        {
            Debug.Log("No enemies found!");
            return;
        }

        StartCoroutine(LaunchMissilesOneByOne(enemies));
    }

    private IEnumerator LaunchMissilesOneByOne(NPCController[] enemies)
    {
        foreach (var enemy in enemies)
        {
            GameObject missile = Instantiate(missilePrefab, missileSpawnPoint.position, Quaternion.identity);
            HomingMissile homing = missile.AddComponent<HomingMissile>();
            homing.SetTarget(enemy.transform, missileSpeed, rotationSpeed, blastEffectPrefab, explosionSFX);

            yield return new WaitForSeconds(missileLaunchDelay); // Wait before launching the next missile
        }
    }
}

// Separate Homing Missile Class for Movement
public class HomingMissile : MonoBehaviour
{
    private Transform target;
    private float speed;
    private float rotateSpeed;
    private GameObject blastEffect;
    public AudioClip explosionSFX;

    public void SetTarget(Transform target, float speed, float rotateSpeed, GameObject blastEffect, AudioClip explosionSFX)
    {
        this.target = target;
        this.speed = speed;
        this.rotateSpeed = rotateSpeed;
        this.blastEffect = blastEffect;
        this.explosionSFX = explosionSFX;
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        // Move towards target
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotateSpeed * Time.deltaTime);
        transform.position += transform.forward * speed * Time.deltaTime;

        // If close enough, explode
        if (Vector3.Distance(transform.position, target.position) < 1f)
        {
            Explode();
        }
    }

    private void Explode()
    {
        // Spawn explosion effect
        if (blastEffect)
        {
            Instantiate(blastEffect, transform.position, Quaternion.identity);
            AudioSource.PlayClipAtPoint(explosionSFX, Camera.main.transform.position);
        }

        // Destroy target and missile
        Destroy(target.gameObject);
        Destroy(gameObject);
    }
}
