using UnityEngine;

public class CoinPickup : MonoBehaviour
{

    // SpriteRenderer coinSprite;

    AudioSource coinSound;
    bool isDestroyed = false;

    // Use this for initialization
    void Start()
    {
        // coinSprite = GetComponent<SpriteRenderer>();
        coinSound = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        Helpers.DebugValue("coinSound.isPlaying: ", coinSound.isPlaying);
        Helpers.DebugValue("isDestroyed:", isDestroyed);

        if (isDestroyed && coinSound.isPlaying == false)
        {
            Debug.Log("Destroying coin");
            Destroy(gameObject);
        }


    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    Debug.Log("Coin collected!");

    //    if (other.gameObject.tag == "Player")
    //    {
    //        Debug.Log("Coin collided with Player");

    //        // coinSprite.enabled = false;
    //        GameManager.instance.PlayerCoinPickup();
    //        coinSound.Play();
    //        isDestroyed = true;
    //    }
    //}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Collision occurred");

        if (collision.gameObject.tag == "Player")
        {
            Debug.Log("Collision with player");
            // coinSprite.enabled = false;
            GameManager.instance.PlayerCoinPickup();
            coinSound.Play();
            isDestroyed = true;
        }
        else
        {
            Debug.Log(collision.gameObject.tag);
        }
    }
}
