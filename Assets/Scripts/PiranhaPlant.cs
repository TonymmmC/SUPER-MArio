using System.Collections;
using UnityEngine;

public class PiranhaPlant : MonoBehaviour
{
    public Sprite spriteOpen;
    public Sprite spriteClosed;
    public float riseHeight = 1.5f;
    public float moveSpeed = 1.5f;
    public float waitAtTop = 2f;
    public float waitAtBottom = 1f;
    public Vector2 safeZoneSize = new Vector2(2f, 2f);

    private SpriteRenderer sr;
    private Vector3 bottomPos;
    private Vector3 topPos;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        bottomPos = transform.position;
        topPos = bottomPos + Vector3.up * riseHeight;
        StartCoroutine(PlantCycle());
        StartCoroutine(Animate());
    }

    private bool PlayerIsClose()
    {
        Collider2D[] hits = Physics2D.OverlapBoxAll(bottomPos, safeZoneSize, 0f);
        foreach (var hit in hits)
            if (hit.CompareTag("Player")) return true;
        return false;
    }

    private IEnumerator PlantCycle()
    {
        while (true)
        {
            yield return new WaitForSeconds(waitAtBottom);

            while (PlayerIsClose())
                yield return new WaitForSeconds(0.2f);

            // Rise
            while (transform.position.y < topPos.y)
            {
                transform.position += Vector3.up * moveSpeed * Time.deltaTime;
                yield return null;
            }
            transform.position = topPos;

            yield return new WaitForSeconds(waitAtTop);

            // Sink
            while (transform.position.y > bottomPos.y)
            {
                transform.position += Vector3.down * moveSpeed * Time.deltaTime;
                yield return null;
            }
            transform.position = bottomPos;
        }
    }

    private IEnumerator Animate()
    {
        while (true)
        {
            sr.sprite = spriteOpen;
            yield return new WaitForSeconds(0.3f);
            sr.sprite = spriteClosed;
            yield return new WaitForSeconds(0.3f);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        other.GetComponent<Player>()?.Hit();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, safeZoneSize);
    }
}
