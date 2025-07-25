using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public float moveDistance = 2f;
    public float speed = 2f;
    private Vector3 startPos;
    private bool movingUp = true;

    private void Start()
    {
        startPos = transform.position;
    }

    private void Update()
    {
        Vector3 target = startPos + (movingUp ? Vector3.up : Vector3.down) * moveDistance;
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target) < 0.01f)
            movingUp = !movingUp;
    }

    // Parent player when on platform
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
            other.transform.SetParent(transform);
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
            other.transform.SetParent(null);
    }
}
