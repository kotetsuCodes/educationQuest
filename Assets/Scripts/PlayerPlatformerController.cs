using System.Collections;
using UnityEngine;

public class PlayerPlatformerController : PhysicsObject
{

    public float maxSpeed = 7;
    public float jumpTakeOffSpeed = 7;
    bool canMove = true;

    SpriteRenderer spriteRenderer;
    Animator animator;
    AudioSource audioSource;
    Color defaultColor;


    public AudioClip jumpSound;
    public AudioClip damageSound;


    // Use this for initialization
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        defaultColor = spriteRenderer.color;
    }

    protected override void ComputeVelocity()
    {
        if (GameManager.instance.CharactersCanMove && canMove)
        {
            // Debug.Log("Computing player velocity");

            Vector2 move = Vector2.zero;

            move.x = Input.GetAxis("Horizontal");

            if (Input.GetButtonDown("Jump") && grounded)
            {
                playJumpSound();
                velocity.y = jumpTakeOffSpeed;
            }
            else if (Input.GetButtonUp("Jump"))
            {
                if (velocity.y > 0)
                    velocity.y = velocity.y * 0.5f;
            }

            bool flipSprite = (spriteRenderer.flipX ? move.x > 0.01f : (move.x < -0.01f));

            if (flipSprite)
                spriteRenderer.flipX = !spriteRenderer.flipX;

            animator.SetBool("grounded", grounded);
            animator.SetFloat("velocityX", Mathf.Abs(velocity.x) / maxSpeed);

            targetVelocity = move * maxSpeed;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Enemy")
        {
            DamagePlayer(0.5f);
        }
    }

    public void DamagePlayer(float damagePoints)
    {
        GameManager.instance.PlayerHealth = Mathf.Clamp(GameManager.instance.PlayerHealth - damagePoints, 0, float.MaxValue);
        GameManager.instance.CalculateHealthSprites(GameManager.instance.PlayerHealth);

        audioSource.clip = damageSound;
        audioSource.Play();

        StartCoroutine(RunDelayedDamageEffects());
    }

    IEnumerator RunDelayedDamageEffects()
    {
        spriteRenderer.color = Color.red;

        canMove = false;
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;

        yield return new WaitForSeconds(0.15f);

        canMove = true;
        spriteRenderer.color = defaultColor;
    }

    void playJumpSound()
    {
        audioSource.clip = jumpSound;
        audioSource.Play();
    }
}
