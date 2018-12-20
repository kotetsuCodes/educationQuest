using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyController : PhysicsObject
{

    public float maxSpeed = 0.001f;
    public float jumpTakeOffSpeed = 7;
    public float DamagePerHit = 0.5f;

    public float CollisionDistance;
    public bool FollowPlayer = false;
    public bool CheckForCliffs = false;

    public bool ContinuousMotion = true;

    // public float MovementPauseTime = 5.0f;

    float MovementPauseStart;
    float MovementPauseEnd;

    float MovementBeginStart;
    float MovementBeginEnd;

    // public float MovementMotionTime = 5.0f;
    bool isInMotion;

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
    ContactFilter2D contactFilterThisLayer;
    ContactFilter2D contactFilterGround;

    private float nextJumpTime;

    readonly float movementPauseDurationUpperBound = 1.5f;
    readonly float movementPauseDurationLowerBound = 3.0f;

    readonly float movementDurationUpperBound = 1.0f;
    readonly float movementDurationLowerBound = 8.0f;

    // readonly float jumpTimePauseLowerBound = 3.0f;
    // readonly float jumpTimePauseUpperBound = 10.0f;

    readonly float jumpTimePauseLowerBound = 5.0f;
    readonly float jumpTimePauseUpperBound = 12.0f;

    // Use this for initialization
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        collider2D = GetComponent<Collider2D>();
        rigidBody = GetComponent<Rigidbody2D>();

        contactFilterThisLayer.useTriggers = true;
        contactFilterThisLayer.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));
        contactFilterThisLayer.useLayerMask = true;

        contactFilterTerrain.useTriggers = false;
        contactFilterTerrain.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));
        contactFilterTerrain.useLayerMask = true;

        contactFilterGround.useTriggers = false;
        contactFilterGround.SetLayerMask(LayerMask.GetMask("Ground"));
        contactFilterGround.useLayerMask = true;

        isInMotion = true;

        MovementBeginStart = Time.time;
        MovementBeginEnd = Time.time + Random.Range(movementDurationLowerBound, movementDurationUpperBound);

        MovementPauseStart = MovementBeginEnd;
        MovementPauseEnd = MovementPauseStart + Random.Range(movementPauseDurationLowerBound, movementPauseDurationUpperBound);

        nextJumpTime = Time.time + Random.Range(jumpTimePauseLowerBound, jumpTimePauseUpperBound);

    }

    protected override void ComputeVelocity()
    {
        // determine if we pause

        bool canMove = true;

        if (!ContinuousMotion)
        {
            // isInMotion - boolean for determining if the enemy is currently moving
            // MovementMotionTime - The timestamp for when the enemy starts moving again
            // MovementPauseTime - The timestamp for when the enemy pauses again

            if (Time.time > MovementBeginStart && Time.time < MovementBeginEnd)
            {
                canMove = true;

                MovementPauseStart = MovementBeginEnd;
                MovementPauseEnd = MovementPauseStart + Random.Range(movementPauseDurationLowerBound, movementPauseDurationUpperBound);
            }
            else if (Time.time > MovementPauseStart && Time.time < MovementPauseEnd)
            {
                canMove = false;

                MovementBeginStart = MovementPauseEnd;
                MovementBeginEnd = MovementBeginStart + Random.Range(movementDurationLowerBound, movementDurationUpperBound);
            }
        }

        if (canMove && GameManager.instance.CharactersCanMove)
        {
            Vector2 move = Vector2.zero;

            // check if we're obstructed to the left 
            // Debug.DrawRay(new Vector2(transform.position.x, transform.position.y + 0.25f), Vector2.left, Color.green, 1.0f);
            // Debug.DrawRay(new Vector2(transform.position.x, transform.position.y + 0.25f), Vector2.right, Color.red, 1.0f);
            // Debug.DrawRay(new Vector2(transform.position.x + 1, transform.position.y), Vector2.down, Color.yellow, 0.1f);
            // Debug.DrawRay(new Vector2(transform.position.x - 1, transform.position.y), Vector2.down, Color.yellow, 0.1f);

            if (FollowPlayer)
            {
                move = getMoveForFollowPlayer(move);
            }
            else if (CheckForCliffs)
            {
                move = getMoveForCheckForCliffs(move);
            }
            else
            {
                move = getMoveForDoNotCheckForCliffs(move);
            }

            // do jump
            if (Time.time >= nextJumpTime)
            {
                // velocity.y = grounded ? jumpTakeOffSpeed : velocity.y = velocity.y * 0.5f;
                velocity.y = jumpTakeOffSpeed;
            }

            if (nextJumpTime < Time.time)
            {
                nextJumpTime = Time.time + Random.Range(jumpTimePauseLowerBound, jumpTimePauseUpperBound);
            }

            bool flipSprite = (spriteRenderer.flipX ? move.x > 0.01f : (move.x < 0.01f));

            if (flipSprite)
                spriteRenderer.flipX = !spriteRenderer.flipX;

            animator.SetBool("grounded", grounded);
            animator.SetFloat("velocityX", Mathf.Abs(velocity.x) / maxSpeed);

            targetVelocity = move * maxSpeed;
        }
        else
        {
            animator.SetFloat("velocityX", 0.0f);
        }
    }

    Vector2 getMoveForDoNotCheckForCliffs(Vector2 move)
    {
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

        leftColliderBuffer.AddRange(leftColliders.Where(l => l.collider != null));
        rightColliderBuffer.AddRange(rightColliders.Where(r => r.collider != null));

        if (
                isMovingLeft
                && (leftColliderBuffer.Any() == false
                || leftColliderBuffer.Any(c => c.collider != null && c.distance > 0.1f))

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
            )
        {
            move.x = 0.2f;
            isMovingLeft = false;
        }
        else
        {
            isMovingLeft = true;
        }

        return move;
    }

    Vector2 getMoveForCheckForCliffs(Vector2 move)
    {
        Debug.DrawRay(rigidBody.transform.position, new Vector2(-1, -1), Color.blue, 0.1f);
        Debug.DrawRay(rigidBody.transform.position, new Vector2(1, -1), Color.blue, 0.1f);

        var leftColliders = new RaycastHit2D[16];
        var rightColliders = new RaycastHit2D[16];

        bottomLeftColliderBuffer.Clear();
        bottomRightColliderBuffer.Clear();

        List<RaycastHit2D> leftColliderBuffer = new List<RaycastHit2D>();
        List<RaycastHit2D> rightColliderBuffer = new List<RaycastHit2D>();

        int leftCollisions = rigidBody.Cast(Vector2.left, contactFilterTerrain, leftColliders, CollisionDistance);
        int rightCollisions = rigidBody.Cast(Vector2.right, contactFilterTerrain, rightColliders, CollisionDistance);

        int bottomLeftCollisions = Physics2D.Raycast(rigidBody.transform.position, new Vector2(-1, -1), contactFilterGround, bottomLeftColliders, 1f);
        int bottomRightCollisions = Physics2D.Raycast(rigidBody.transform.position, new Vector2(1, -1), contactFilterGround, bottomRightColliders, 1f);

        leftColliderBuffer.AddRange(leftColliders.Where(l => l.collider != null));
        rightColliderBuffer.AddRange(rightColliders.Where(r => r.collider != null));

        bottomLeftColliderBuffer.AddRange(bottomLeftColliders.Where(l => l.collider != null));
        bottomRightColliderBuffer.AddRange(bottomRightColliders.Where(r => r.collider != null));

        if (
                isMovingLeft
                && (leftColliderBuffer.Any() == false
                || leftColliderBuffer.Any(c => c.distance > 0.1f))
                && bottomLeftCollisions > 0

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
                || rightColliderBuffer.Any(c => c.distance > 0.1f))
                && bottomRightCollisions > 0
            )
        {
            move.x = 0.2f;
            isMovingLeft = false;
        }
        else
        {
            isMovingLeft = true;
        }

        return move;
    }

    Vector2 getMoveForFollowPlayer(Vector2 move)
    {
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

        int bottomLeftCollisions = Physics2D.Raycast(transform.position + Vector3.left * 1.0f, Vector2.down, contactFilterThisLayer, bottomLeftColliders, 100f);
        int bottomRightCollisions = Physics2D.Raycast(transform.position + Vector3.right * 1.0f, Vector2.down, contactFilterThisLayer, bottomRightColliders, 100f);

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

        return move;
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "KillZone")
            Destroy(gameObject);
    }
}
