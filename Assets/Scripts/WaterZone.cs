using UnityEngine;

public class WaterZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        PlayerMovement movement = other.GetComponent<PlayerMovement>();
        if (movement != null) movement.swimming = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        PlayerMovement movement = other.GetComponent<PlayerMovement>();
        if (movement != null) movement.swimming = false;
    }
}
