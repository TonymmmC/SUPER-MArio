using System.Collections;
using UnityEngine;

// Coloca en el tubo de salida del nivel — Box Collider 2D isTrigger = true
public class PipeLevelExit : MonoBehaviour
{
    public KeyCode enterKey = KeyCode.S;

    private bool used = false;

    private void OnTriggerStay2D(Collider2D other)
    {
        if (used) return;

        if (other.CompareTag("Player") && Input.GetKey(enterKey))
        {
            used = true;
            StartCoroutine(ExitSequence(other.transform));
        }
    }

    private IEnumerator ExitSequence(Transform player)
    {
        player.GetComponent<PlayerMovement>().enabled = false;
        AudioManager.Instance?.PlaySFX(null);

        // Mario baja dentro del tubo
        float elapsed = 0f;
        Vector3 start = player.position;
        Vector3 end = player.position + Vector3.down * 1.5f;

        while (elapsed < 1f)
        {
            player.position = Vector3.Lerp(start, end, elapsed);
            elapsed += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);
        GameManager.Instance.NextLevel();
    }
}
