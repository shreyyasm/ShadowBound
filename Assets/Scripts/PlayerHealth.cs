using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public float health = 100;
    public float maxHealth = 100;
    public float healthModifer = 1f;
    public float damageMultiplier = 1f;
    public bool damaging;
    public bool consuming;
    public Slider healthSlider;
    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
        healthSlider.maxValue = maxHealth;
       
    }

    // Update is called once per frame
    void Update()
    {
        if(damaging)
        {
            health -= Time.deltaTime * damageMultiplier;
            healthSlider.value = health;
        }
        if(!damaging && health <= maxHealth)
        {
            health += Time.deltaTime * healthModifer;
            healthSlider.value = health;
        }
      
    }
    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.CompareTag("Light"))
        {
            damageMultiplier = other.GetComponent<DamageMultiplier>().damageMultple;
            damaging = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Light"))
        {
            damageMultiplier = 1;
            damaging = false;
        }
    }
}
