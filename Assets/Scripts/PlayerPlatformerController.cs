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
    public AudioClip walkSound;

    float jumpModifier = 0.0f;
    private bool isOnJumper;
    private bool isOnLadder = false;

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

            var verticalAxis = Input.GetAxis("Vertical");

            if (isOnLadder)
            {
                Helpers.DebugValue("verticalAxis: ", verticalAxis);
                // move.y = Input.GetAxis("Vertical");
                velocity.y = verticalAxis * 2.0f;
            }

            setJumpVelocity(ref velocity);

            bool flipSprite = (spriteRenderer.flipX ? move.x > 0.01f : (move.x < -0.01f));

            if (flipSprite)
                spriteRenderer.flipX = !spriteRenderer.flipX;

            animator.SetBool("grounded", grounded);
            animator.SetFloat("velocityX", Mathf.Abs(velocity.x) / maxSpeed);

            animator.SetBool("isClimbing", Mathf.Abs(verticalAxis) > 0.0 && isOnLadder);

            targetVelocity = move * maxSpeed;

            if (grounded && (targetVelocity.x > 0.0 || targetVelocity.x < 0.0))
            {
                PlayWalkSound();
            }
            else
            {
                StopWalkSound();
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Enemy")
        {
            DamagePlayer(0.5f);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Jumper")
        {
            jumpModifier = 5.0f;
            isOnJumper = true;

            collision.gameObject.GetComponent<Animator>().Play("Active", 0);
        }
        else if (collision.gameObject.tag == "Ladder")
        {
            Debug.Log("We are touching a ladder");
            isOnLadder = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Jumper")
        {
            jumpModifier = 0.0f;
            isOnJumper = false;
        }
        else if (collision.gameObject.tag == "Ladder")
        {
            isOnLadder = false;
        }
    }

    private void setJumpVelocity(ref Vector2 velocity)
    {
        if (isOnJumper || (Input.GetButtonDown("Jump") && (grounded || isOnLadder)))
        {
            playJumpSound();
            velocity.y = jumpTakeOffSpeed + jumpModifier;
        }
        else if (Input.GetButtonUp("Jump"))
        {
            if (velocity.y > 0)
                velocity.y = velocity.y * 0.5f;
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

    void PlayWalkSound()
    {
        if (!audioSource.isPlaying)
        {
            audioSource.clip = walkSound;
            audioSource.Play();
        }
    }

    void StopWalkSound()
    {
        if (audioSource.clip == walkSound)
            audioSource.Stop();
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
