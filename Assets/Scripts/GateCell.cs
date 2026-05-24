using System.Collections;
using UnityEngine;

public class GateCell : MonoBehaviour
{
    public float dropDistance = 5f;
    public float dropSpeed = 5f;

    public void Close()
    {
        StartCoroutine(Drop());
    }

    public void Open()
    {
        StartCoroutine(Rise());
    }

    private IEnumerator Rise()
    {
        Vector3 start = transform.position;
        Vector3 end = start + Vector3.up * dropDistance;
        float elapsed = 0f;
        float duration = dropDistance / dropSpeed;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            transform.position = Vector3.Lerp(start, end, elapsed / duration);
            yield return null;
        }

        transform.position = end;
    }

    private IEnumerator Drop()
    {
        Vector3 start = transform.position;
        Vector3 end = start + Vector3.down * dropDistance;
        float elapsed = 0f;
        float duration = dropDistance / dropSpeed;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            transform.position = Vector3.Lerp(start, end, elapsed / duration);
            yield return null;
        }

        transform.position = end;
    }
}
