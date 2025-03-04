using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialSFX : MonoBehaviour
{
    public AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(StartVoice());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    IEnumerator StartVoice()
    {
        yield return new WaitForSeconds(2f);
        audioSource.Play();
    }
}
