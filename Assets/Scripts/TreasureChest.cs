using System.Collections;
using UnityEngine;

public class TreasureChest : MonoBehaviour
{

    bool chestIsOpened;
    public Sprite OpenChestSprite;
    public Sprite ClosedChestSprite;
    SpriteRenderer spriteRenderer;
    AudioSource audioSource;

    public AudioClip openChestAudioClip;
    public AudioClip fanFareAudioClip;

    // Use this for initialization
    void Start()
    {
        chestIsOpened = false;
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        spriteRenderer.sprite = ClosedChestSprite;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        // open chest
        if (chestIsOpened == false && collision.gameObject.tag == "Player" && Input.GetButtonDown("Submit"))
        {
            Debug.Log("------CHEST HAS BEEN OPENED!!!!------");
            chestIsOpened = true;
            spriteRenderer.sprite = OpenChestSprite;
            StartCoroutine(PlayOpenChestSound());
        }
    }

    IEnumerator PlayOpenChestSound()
    {
        LevelManager.instance.AddCollectedItem();

        audioSource.clip = openChestAudioClip;
        audioSource.Play();
        yield return new WaitForSeconds(openChestAudioClip.length + 1.0f);
        audioSource.clip = fanFareAudioClip;
        audioSource.Play();
    }
}
