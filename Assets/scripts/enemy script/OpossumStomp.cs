using UnityEngine;

public class OpossumStomp : MonoBehaviour
{
    [SerializeField] private Opossum opossumScript;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !opossumScript.Equals(null))
        {
            // Call opossum's death
            opossumScript.Die();

            // Bounce the player
            Rigidbody2D playerRb = collision.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, 12f); // Adjust bounce height
            }
        }
    }
}
