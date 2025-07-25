using UnityEngine;

public class FrogStomp : MonoBehaviour
{
    [SerializeField] private FrogPatrol frog;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (frog != null)
            {
                frog.Die();

                // Make player bounce
                Rigidbody2D playerRb = collision.GetComponent<Rigidbody2D>();
                if (playerRb != null)
                {
                    playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, 12f);
                }
            }
        }
    }
}

