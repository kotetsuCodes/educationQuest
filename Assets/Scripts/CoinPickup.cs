using UnityEngine;

public class CoinPickup : MonoBehaviour
{

    AudioSource coinSound;
    Renderer renderer;
    bool isDestroyed = false;

    // Use this for initialization
    void Start()
    {
        renderer = GetComponent<Renderer>();
        coinSound = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        // var newRotation = Quaternion.AngleAxis(2.0f, Vector3.forward);

        transform.Rotate(new Vector3(transform.rotation.x, transform.rotation.y + 5, transform.rotation.z));

        if (isDestroyed && coinSound.isPlaying == false)
        {
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

        if (collision.gameObject.tag == "Player")
        {
            renderer.enabled = false;
            GameManager.instance.PlayerCoinPickup();
            coinSound.Play();
            isDestroyed = true;
        }
    }
}
