using UnityEngine;

public class BearStomp : MonoBehaviour
{
    [SerializeField] private Bear bearScript;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && bearScript != null)
        {
            bearScript.Die();

            Rigidbody2D playerRb = collision.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, 12f);
            }
        }
    }
}
