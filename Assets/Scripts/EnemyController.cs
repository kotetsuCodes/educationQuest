using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyController : PhysicsObject
{

    public float maxSpeed = 0.001f;
    public float DamagePerHit = 0.5f;

    public float CollisionDistance;

    bool isMovingLeft = true;

    SpriteRenderer spriteRenderer;
    Animator animator;

    Rigidbody2D rigidBody;
    Collider2D collider2D;
    readonly RaycastHit2D[] leftColliders = new RaycastHit2D[16];
    readonly RaycastHit2D[] rightColliders = new RaycastHit2D[16];
    readonly List<RaycastHit2D> leftColliderBuffer = new List<RaycastHit2D>(16);
    readonly List<RaycastHit2D> rightColliderBuffer = new List<RaycastHit2D>(16);

    readonly RaycastHit2D[] bottomLeftColliders = new RaycastHit2D[1];
    readonly RaycastHit2D[] bottomRightColliders = new RaycastHit2D[1];
    readonly List<RaycastHit2D> bottomLeftColliderBuffer = new List<RaycastHit2D>(1);
    readonly List<RaycastHit2D> bottomRightColliderBuffer = new List<RaycastHit2D>(1);

    ContactFilter2D contactFilterTerrain;
    ContactFilter2D contactFilterGround;

    // Use this for initialization
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        collider2D = GetComponent<Collider2D>();
        rigidBody = GetComponent<Rigidbody2D>();

        contactFilterGround.useTriggers = true;
        contactFilterGround.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));
        contactFilterGround.useLayerMask = true;

        contactFilterTerrain.useTriggers = false;
        contactFilterTerrain.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));
        contactFilterTerrain.useLayerMask = true;
    }



    protected override void ComputeVelocity()
    {
        if (GameManager.instance.CharactersCanMove)
        {
            Vector2 move = Vector2.zero;

            // check if we're obstructed to the left 
            // Debug.DrawRay(new Vector2(transform.position.x, transform.position.y + 0.25f), Vector2.left, Color.green, 1.0f);
            // Debug.DrawRay(new Vector2(transform.position.x, transform.position.y + 0.25f), Vector2.right, Color.red, 1.0f);
            // Debug.DrawRay(new Vector2(transform.position.x + 1, transform.position.y), Vector2.down, Color.yellow, 0.1f);
            // Debug.DrawRay(new Vector2(transform.position.x - 1, transform.position.y), Vector2.down, Color.yellow, 0.1f);

            Debug.DrawLine(transform.position + Vector3.left * 1.0f, new Vector2(transform.position.x - 1.0f, transform.position.y - 100f), Color.blue, 0.1f);
            Debug.DrawLine(new Vector2(transform.position.x + 1.0f, transform.position.y), new Vector2(transform.position.x + 1.0f, transform.position.y - 100f), Color.yellow, 0.1f);

            var leftColliders = new RaycastHit2D[16];
            var rightColliders = new RaycastHit2D[16];

            bottomLeftColliderBuffer.Clear();
            bottomRightColliderBuffer.Clear();

            List<RaycastHit2D> leftColliderBuffer = new List<RaycastHit2D>();
            List<RaycastHit2D> rightColliderBuffer = new List<RaycastHit2D>();

            int leftCollisions = rigidBody.Cast(Vector2.left, contactFilterTerrain, leftColliders, CollisionDistance);
            int rightCollisions = rigidBody.Cast(Vector2.right, contactFilterTerrain, rightColliders, CollisionDistance);

            int bottomLeftCollisions = Physics2D.Raycast(transform.position + Vector3.left * 1.0f, Vector2.down, contactFilterGround, bottomLeftColliders, 100f);
            int bottomRightCollisions = Physics2D.Raycast(transform.position + Vector3.right * 1.0f, Vector2.down, contactFilterGround, bottomRightColliders, 100f);

            leftColliderBuffer.AddRange(leftColliders.Where(l => l.collider != null));
            rightColliderBuffer.AddRange(rightColliders.Where(r => r.collider != null));

            bottomLeftColliderBuffer.AddRange(bottomLeftColliders.Where(l => l.collider != null));
            bottomRightColliderBuffer.AddRange(bottomRightColliders.Where(r => r.collider != null));

            var playerDirection = getPlayerDirection(leftColliderBuffer.ToArray(), rightColliderBuffer.ToArray());

            if (playerDirection < 0.0 && bottomLeftColliderBuffer.Any(c => c.collider.gameObject.tag == "KillZone") == false)
            {
                move.x = playerDirection;
                isMovingLeft = true;
            }
            else if (playerDirection > 0.0 && bottomRightColliderBuffer.Any(c => c.collider.gameObject.tag == "KillZone") == false)
            {
                move.x = playerDirection;
                isMovingLeft = false;
            }
            else
            {
                if (
                        isMovingLeft
                        && (leftColliderBuffer.Any() == false
                        || leftColliderBuffer.Any(c => c.collider != null && c.distance > 0.1f))
                        && bottomLeftColliderBuffer.Any(c => c.collider.gameObject.tag == "KillZone") == false

                    )
                {
                    move.x = -0.2f;
                    isMovingLeft = true;
                }
                else
                {
                    isMovingLeft = false;
                }

                if (
                        isMovingLeft == false
                        && (rightColliderBuffer.Any() == false
                        || rightColliderBuffer.Any(c => c.collider != null && c.distance > 0.1f))
                        && bottomRightColliderBuffer.Any(c => c.collider.gameObject.tag == "KillZone") == false
                    )
                {
                    move.x = 0.2f;
                    isMovingLeft = false;
                }
                else
                {
                    isMovingLeft = true;
                }
            }

            bool flipSprite = (spriteRenderer.flipX ? move.x > 0.01f : (move.x < 0.01f));

            if (flipSprite)
                spriteRenderer.flipX = !spriteRenderer.flipX;

            animator.SetBool("grounded", grounded);
            animator.SetFloat("velocityX", Mathf.Abs(velocity.x) / maxSpeed);

            targetVelocity = move * maxSpeed;
        }
    }

    private float getPlayerDirection(RaycastHit2D[] leftColliderBuffer, RaycastHit2D[] rightColliderBuffer)
    {
        if (leftColliderBuffer.Any(c => c.collider?.tag == "Player"))
            return -0.2f;
        else if (rightColliderBuffer.Any(c => c.collider?.tag == "Player"))
            return 0.2f;
        else
            return 0.0f;
    }
}
