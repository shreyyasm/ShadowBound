using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    public float health = 100;
    public float maxHealth = 100;
    public float damageMultiplier = 1f;
    public bool killing;
    public Slider healthSlider;
    public GameObject healthSliderMain;

    private Camera cam;
    
    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        //healthSlider.maxValue = maxHealth;
        UpdateHealthBar(health, maxHealth);
    }

    // Update is called once per frame
    void Update()
    {
        DrainHealth();
        healthSliderMain.transform.rotation = Quaternion.Euler(0, cam.transform.eulerAngles.y, 0);

        // healthSliderMain.transform.rotation = Quaternion.LookRotation(cam.ViewportToWorldPoint(new Vector3(0f, 0f, 0f)));
       
           

    }
    public void DrainHealth()
    {
        if (killing)
        {
            health -= Time.deltaTime * damageMultiplier;
            UpdateHealthBar(health,maxHealth);
        }
        if (!killing && health <= maxHealth)
        {
            health += Time.deltaTime * damageMultiplier;
            UpdateHealthBar(health, maxHealth);
        }
    }

    [SerializeField] Image healthbarSprite;
    [SerializeField] float reduceSpeed = 2f;
    private float target = 1;
    public void UpdateHealthBar(float currentHealth, float maxHealth)
    {
       healthbarSprite.fillAmount = currentHealth / maxHealth;
    }
    public void DealDamage(float damage)
    {
        health -= damage;
        healthbarSprite.fillAmount = health / maxHealth;
    }
    
    
}
