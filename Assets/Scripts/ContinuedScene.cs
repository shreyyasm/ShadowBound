using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ContinuedScene : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        LeanTween.delayedCall(10f, () => { SceneManager.LoadScene("Menu"); });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
