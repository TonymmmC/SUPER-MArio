using System.Collections;
using UnityEngine;

public class Pipe : MonoBehaviour
{
    public Transform connection;
    public KeyCode enterKeyCode = KeyCode.S;
    public Vector3 enterDirection = Vector3.down;
    public Vector3 exitDirection = Vector3.zero;

    [Header("Next Level (dejar connection vacio)")]
    public bool loadNextLevel = false;

    [Header("Lado de activación")]
    public bool activateFromLeft = true;
    public bool activateFromRight = false;
    public bool activateFromTop = false;
    public bool activateFromBottom = false;

    public bool IsActiveFromDirection(Vector2 approachDirection)
    {
        if (approachDirection.x > 0f && activateFromLeft) return true;
        if (approachDirection.x < 0f && activateFromRight) return true;
        if (approachDirection.y < 0f && activateFromTop) return true;
        if (approachDirection.y > 0f && activateFromBottom) return true;
        return false;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (!loadNextLevel && connection != null && Input.GetKey(enterKeyCode))
        {
            StartCoroutine(Enter(other.transform));
        }
    }

    public void TriggerNextLevel(Transform player, Vector2 entryDirection)
    {
        if (loadNextLevel)
            StartCoroutine(EnterAndLoadNext(player, entryDirection));
    }

    private bool transitioning = false;
    private IEnumerator EnterAndLoadNext(Transform player, Vector2 entryDirection)
    {
        if (transitioning) yield break;
        transitioning = true;

        player.GetComponent<PlayerMovement>().enabled = false;
        foreach (Animator animator in player.GetComponentsInChildren<Animator>())
            animator.enabled = false;
        AudioManager.Instance?.StopMusic();

        SpriteRenderer sr = player.GetComponentInChildren<SpriteRenderer>();
        sr.enabled = true;
        sr.color = Color.white;
        Vector3 startPosition = player.position;
        Vector3 targetPosition = player.position + (Vector3)(entryDirection.normalized * 1.5f);
        float elapsed = 0f;
        float duration = 0.8f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            player.position = Vector3.Lerp(startPosition, targetPosition, t);
            sr.color = new Color(1f, 1f, 1f, 1f - t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        sr.color = new Color(1f, 1f, 1f, 0f);
        yield return new WaitForSeconds(0.3f);
        GameManager.Instance.NextLevel();
    }

    private IEnumerator Enter(Transform player)
    {
        player.GetComponent<PlayerMovement>().enabled = false;

        Vector3 enteredPosition = transform.position + enterDirection;
        Vector3 enteredScale = Vector3.one * 0.5f;

        yield return Move(player, enteredPosition, enteredScale);
        yield return new WaitForSeconds(1f);

        var sideSrolling = Camera.main.GetComponent<SideScrolling>();
        sideSrolling.SetUnderground(connection.position.y < sideSrolling.undergroundThreshold);

        if (exitDirection != Vector3.zero)
        {
            player.position = connection.position - exitDirection;
            yield return Move(player, connection.position + exitDirection, Vector3.one);
        }
        else
        {
            player.position = connection.position;
            player.localScale = Vector3.one;
        }

        player.GetComponent<PlayerMovement>().enabled = true;
    }

    private IEnumerator Move(Transform player, Vector3 endPosition, Vector3 endScale)
    {
        float elapsed = 0f;
        float duration = 1f;

        Vector3 startPosition = player.position;
        Vector3 startScale = player.localScale;

        while (elapsed < duration)
        {
            float t = elapsed / duration;

            player.position = Vector3.Lerp(startPosition, endPosition, t);
            player.localScale = Vector3.Lerp(startScale, endScale, t);
            elapsed += Time.deltaTime;

            yield return null;
        }

        player.position = endPosition;
        player.localScale = endScale;
    }

}
