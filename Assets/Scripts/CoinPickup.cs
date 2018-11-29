using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinPickup : MonoBehaviour {

    SpriteRenderer coinSprite;
    AudioSource coinSound;
    bool isDestroyed = false;

	// Use this for initialization
	void Start () {
        coinSprite = GetComponent<SpriteRenderer>();
        coinSound = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
        if(isDestroyed && coinSound.isPlaying == false)
         Destroy(gameObject);

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            coinSprite.enabled = false;
            GameManager.instance.PlayerCoinPickup();
            coinSound.Play();
            isDestroyed = true;
        }            
    }
}
