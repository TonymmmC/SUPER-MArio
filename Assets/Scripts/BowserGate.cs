using System.Collections;
using UnityEngine;

public class BowserGate : MonoBehaviour
{
    public GameObject leftGate;
    public GameObject rightGate;
    public float dropDistance = 5f;
    public float gateSpeed = 5f;
    public Bowser bowser;

    private bool triggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered || !other.CompareTag("Player")) return;
        triggered = true;
        StartCoroutine(CloseGates());
    }

    private IEnumerator CloseGates()
    {
        Vector3 leftStart = leftGate.transform.position;
        Vector3 rightStart = rightGate.transform.position;
        Vector3 leftEnd = leftStart + Vector3.down * dropDistance;
        Vector3 rightEnd = rightStart + Vector3.down * dropDistance;

        float elapsed = 0f;
        float duration = dropDistance / gateSpeed;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            leftGate.transform.position = Vector3.Lerp(leftStart, leftEnd, t);
            rightGate.transform.position = Vector3.Lerp(rightStart, rightEnd, t);
            yield return null;
        }

        leftGate.transform.position = leftEnd;
        rightGate.transform.position = rightEnd;

        bowser?.StartFight();
    }
}
