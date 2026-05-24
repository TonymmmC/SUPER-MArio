using UnityEngine;

public class FireBarSegment : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        other.GetComponent<Player>()?.Hit();
    }
}
