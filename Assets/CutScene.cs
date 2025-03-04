using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class CutScene : MonoBehaviour
{
    public VideoPlayer videoPlayer;

    private void Awake()
    {

        string url = Application.streamingAssetsPath + "/Cut Scene.mp4";
        videoPlayer.url = url;
        videoPlayer.Play();
    }
    // Start is called before the first frame update
    void Start()
    {
       
        LeanTween.delayedCall(23f, () => { SceneManager.LoadScene("Level 1"); PlayerPrefs.SetInt("CutScene", 1); });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
