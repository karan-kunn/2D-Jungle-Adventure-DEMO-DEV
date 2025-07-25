using UnityEngine;

public class Opossum : MonoBehaviour
{
    [Header("Patrol Settings")]
    [SerializeField] private float patrolDistance = 5f;
    [SerializeField] private float speed = 2f;

    [Header("Death Settings")]
    [SerializeField] private float deathDelay = 0.5f;

    private Rigidbody2D rb;
    private Animator anim;
    private Collider2D coll;

    private Vector2 startPoint;
    private int direction = 1;
    private float leftBound;
    private float rightBound;
    private bool isDead = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        coll = GetComponent<Collider2D>();
        startPoint = transform.position;

        leftBound = startPoint.x - patrolDistance / 2f;
        rightBound = startPoint.x + patrolDistance / 2f;
    }

    void Update()
    {
        if (isDead) return;
        Patrol();
    }

    void Patrol()
    {
        rb.linearVelocity = new Vector2(direction * speed, rb.linearVelocity.y);
        transform.localScale = new Vector3(direction, 1, 1);

        if (transform.position.x >= rightBound)
        {
            direction = -1;
        }
        else if (transform.position.x <= leftBound)
        {
            direction = 1;
        }

        anim.SetBool("isWalking", true);
    }

    public void Die()
    {
        if (isDead) return;

        isDead = true;
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;
        coll.enabled = false;

        anim.SetTrigger("death");

        Destroy(gameObject, deathDelay);
    }
}
