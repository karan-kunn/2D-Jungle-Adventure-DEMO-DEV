using UnityEngine;

public class FrogPatrol : MonoBehaviour
{
    [Header("Waypoints")]
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;

    [Header("Movement")]
    [SerializeField] private float speed = 3f;
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private float jumpCooldown = 1.5f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody2D rb;
    private Animator anim;
    private Collider2D coll;

    private float jumpTimer;
    private bool movingToB = true;
    private bool isGrounded = true;
    private int faceDir = 1; // 1 = right, -1 = left
    private bool isDead = false;

    private enum State { idle, jumping, falling }
    private State state = State.idle;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        coll = GetComponent<Collider2D>();
        jumpTimer = jumpCooldown;
    }

    void Update()
    {
        if (isDead) return;

        CheckGround();
        AnimationState();
        anim.SetInteger("state", (int)state);

        if (isGrounded)
        {
            jumpTimer -= Time.deltaTime;
        }
    }

    void FixedUpdate()
    {
        if (isDead) return;

        if (isGrounded && jumpTimer <= 0)
        {
            Patrol();
            jumpTimer = jumpCooldown;
        }
    }

    void Patrol()
    {
        Transform target = movingToB ? pointB : pointA;
        float direction = target.position.x - transform.position.x;

        if ((movingToB && transform.position.x >= pointB.position.x) ||
            (!movingToB && transform.position.x <= pointA.position.x))
            {
                movingToB = !movingToB;
                return;
            }


        faceDir = direction > 0 ? 1 : -1;
        transform.localScale = new Vector3(faceDir, 1, 1);

        rb.linearVelocity = new Vector2(faceDir * speed, jumpForce);
        state = State.jumping;
    }

    void CheckGround()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    void AnimationState()
    {
        if (state == State.jumping)
        {
            if (rb.linearVelocity.y < 0.1f)
            {
                state = State.falling;
            }
        }
        else if (state == State.falling)
        {
            if (isGrounded)
            {
                state = State.idle;
            }
        }
        else if (isGrounded)
        {
            state = State.idle;
        }
    }


    public void Die()
    {
        isDead = true;
        anim.SetTrigger("death");
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;
        coll.enabled = false;

        // Destroy after 0.5s (adjust based on your death animation)
        Destroy(gameObject, 0.5f);
    }

    // Debug visual for ground check
    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
