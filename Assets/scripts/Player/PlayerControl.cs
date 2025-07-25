using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Collider2D coll;
    private Animator anim;

    #region Private Variables
    [SerializeField] private float speed = 8f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private LayerMask ground;
    private bool isJumping = false;

    [SerializeField] private int Cherries = 0;
    [SerializeField] private int Health = 3;
    [SerializeField] private Text HealthTxt;
    [SerializeField] private Text CherriesTxt;

    [SerializeField] private int HurtForce = 10;
    [SerializeField] private float hurtDuration = 0.5f;

    [Header("Audio")]
    [SerializeField] private AudioSource bgMusicSource;
    [SerializeField] private AudioSource hurtSoundSource;
    [SerializeField] private AudioSource restartSoundSource;
    private float hurtTimer = 0f;
    private bool isDead = false;
    #endregion

    #region FiniteStateMachine
    private enum State { idle, running, jumping, falling, hurt }
    private State state = State.idle;
    #endregion

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
        HealthTxt.text = Health.ToString();
        CherriesTxt.text = Cherries.ToString();

        if (bgMusicSource != null && !bgMusicSource.isPlaying)
        bgMusicSource.Play();
    }

    void Update()
    {
        if (isDead) return;

        // If hurt, reduce timer and prevent movement/animations
        if (state == State.hurt)
        {
            hurtTimer -= Time.deltaTime;
            if (hurtTimer <= 0)
            {
                state = State.idle;
            }

            anim.SetInteger("state", (int)state);
            return;
        }
        if (state != State.hurt && hurtSoundSource.isPlaying)
        hurtSoundSource.Stop();

        Movement();
        AnimationState();
        anim.SetInteger("state", (int)state);

        // Check if fallen off the map
        if (transform.position.y < -5f && !isDead)
        {
            Die();
        }
    }

    void Movement()
    {
        float hDirection = Input.GetAxisRaw("Horizontal");

        if (hDirection < 0)
        {
            rb.linearVelocity = new Vector2(-speed, rb.linearVelocity.y);
            transform.localScale = new Vector2(-1, 1);
        }
        else if (hDirection > 0)
        {
            rb.linearVelocity = new Vector2(speed, rb.linearVelocity.y);
            transform.localScale = new Vector2(1, 1);
        }
        else if (Input.GetButtonUp("Horizontal") && !isJumping)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }

        if (Input.GetButtonDown("Jump") && coll.IsTouchingLayers(ground))
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            state = State.jumping;
            isJumping = true;
        }
    }

    private void AnimationState()
    {
        if (state == State.jumping)
        {
            if (rb.linearVelocity.y < 0)
            {
                state = State.falling;
            }
        }
        else if (state == State.falling)
        {
            if (coll.IsTouchingLayers(ground))
            {
                state = State.idle;
                isJumping = false;
            }
        }
        else if (Mathf.Abs(rb.linearVelocity.x) > 0.1f)
        {
            state = State.running;
        }
        else
        {
            state = State.idle;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Collectable"))
        {
            Destroy(collision.gameObject);
            Cherries += 1;
            CherriesTxt.text = Cherries.ToString();
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (isDead) return;

        if (other.gameObject.CompareTag("Enemy"))
        {
            state = State.hurt;
            hurtTimer = hurtDuration;

            if (hurtSoundSource != null)
            hurtSoundSource.Play();

            if (other.gameObject.transform.position.x > transform.position.x)
            {
                rb.linearVelocity = new Vector2(-HurtForce, rb.linearVelocity.y);
            }
            else
            {
                rb.linearVelocity = new Vector2(HurtForce, rb.linearVelocity.y);
            }

            Health -= 1;
            HealthTxt.text = Health.ToString();

            if (Health <= 0)
            {
                Die();
            }
        }
    }

    private void Die()
    {
        isDead = true;
        anim.SetTrigger("hurt"); // Use hurt or death animation trigger
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;
        coll.enabled = false;

        if (hurtSoundSource.isPlaying)
        hurtSoundSource.Stop();

        if (restartSoundSource != null)
        restartSoundSource.Play();

        // Restart after delay
        Invoke("RestartLevel", 1.2f); // Adjust delay if needed
    }

    private void RestartLevel()
    {
        SceneManager.LoadScene("Level1");
    }
}


