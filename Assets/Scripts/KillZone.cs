using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KillZone : MonoBehaviour {

    BoxCollider2D boxCollider;

    private void Awake()
    {
        GetComponent<SpriteRenderer>().enabled = false;
    }

    // Use this for initialization
    void Start () {
        boxCollider = GetComponent<BoxCollider2D>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
            GameManager.instance.GameOver();
        else if (other.gameObject.tag == "Enemy")
            Destroy(other.gameObject);

    }
}
