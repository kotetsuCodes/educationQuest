using UnityEngine;

public class HealthPickup : MonoBehaviour
{

    AudioSource healthPickupSound;
    bool isDestroyed = false;
    bool playerIsTouching = false;
    public float HealthIncreaseAmount = 1.0f;

    // Use this for initialization
    void Start()
    {
        healthPickupSound = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerIsTouching && Input.GetButton("Submit"))
        {
            GameManager.instance.PlayerHealthPickup(HealthIncreaseAmount);
            healthPickupSound.Play();
            isDestroyed = true;
        }

        if (isDestroyed && healthPickupSound.isPlaying == false)
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            playerIsTouching = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        playerIsTouching = false;
    }
}
