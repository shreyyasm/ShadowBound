using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonAnim : MonoBehaviour
{
    public int index;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnHoverEnter()
    {
        hoverButtons.instance.currentAbilityIndex = index;
        hoverButtons.instance.HoverSound();
    }

    public void OnHoverExit()
    {
        hoverButtons.instance.currentAbilityIndex = 0;
    }
}
