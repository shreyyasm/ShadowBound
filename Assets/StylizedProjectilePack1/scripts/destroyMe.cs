using UnityEngine;
using System.Collections;


public class destroyMe : MonoBehaviour{

    float timer;
    public float deathtimer = 10;


	// Use this for initialization
	void Start () {
		
	
	}
    private void OnEnable()
    {
	StartCoroutine(DespawnBullet());
	}
    // Update is called once per frame
    void Update ()
    {
        //timer += Time.deltaTime;

        //if (timer >= deathtimer)
        //{
        //     Destroy(gameObject);
        //   // Bullet.Release(gameObject);
        //}

    }
	IEnumerator DespawnBullet()
    {
		yield return new WaitForSeconds(deathtimer);
        gameObject.SetActive(false);
	}
}
