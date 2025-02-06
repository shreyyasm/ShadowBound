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
        //healthSliderMain.transform.rotation = Quaternion.LookRotation(healthSliderMain.transform.position - cam.transform.position);
        if(killing)
        healthbarSprite.fillAmount = Mathf.MoveTowards(healthbarSprite.fillAmount,target,reduceSpeed* Time.deltaTime);

    }
    public void DrainHealth()
    {
        if (killing)
        {
            health -= Time.deltaTime * damageMultiplier;
            //healthSlider.value = health;
            UpdateHealthBar(health,maxHealth);
        }
        if (!killing && health <= maxHealth)
        {
            health += Time.deltaTime * damageMultiplier;
           // healthSlider.value = health;
            UpdateHealthBar(health, maxHealth);
        }
    }

    [SerializeField] Image healthbarSprite;
    [SerializeField] float reduceSpeed;
    private float target = 1;
    public void UpdateHealthBar(float currentHealth, float maxHealth)
    {
       target = currentHealth / maxHealth;
        if(killing)
            healthbarSprite.fillAmount = currentHealth / maxHealth;
    }
    
}
