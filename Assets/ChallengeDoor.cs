using UnityEngine;

public class ChallengeDoor : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    BoxCollider2D boxCollider2D;

    // Use this for initialization
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider2D = GetComponent<BoxCollider2D>();

        spriteRenderer.enabled = false;
        boxCollider2D.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (LevelManager.instance.AllChallengesCollected && !boxCollider2D.enabled)
        {
            spriteRenderer.enabled = true;
            boxCollider2D.enabled = true;
        }
    }

}
