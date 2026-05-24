using System.Collections;
using UnityEngine;

public class Blooper : MonoBehaviour
{
    public Sprite spriteOpen;
    public Sprite spriteSqueeze;
    public float riseSpeed = 4f;
    public float driftSpeed = 1f;
    public float riseTime = 0.6f;
    public float pauseTime = 0.5f;
    public float maxY = 5f;
    public float minY = -5f;

    public Vector2 sizeOpen = new Vector2(0.8f, 1.6f);
    public Vector2 sizeSqueeze = new Vector2(0.8f, 1.0f);

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private BoxCollider2D col;
    private Transform player;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<BoxCollider2D>();
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        StartCoroutine(BlooperCycle());
    }

    private IEnumerator BlooperCycle()
    {
        while (true)
        {
            float dir = GetHorizontalDir();

            // Deriva hacia abajo (frame abierto)
            sr.sprite = spriteOpen;
            col.size = sizeOpen;
            rb.linearVelocity = new Vector2(dir * driftSpeed, -driftSpeed);
            yield return new WaitForSeconds(pauseTime);

            // Se contrae antes de dispararse
            sr.sprite = spriteSqueeze;
            col.size = sizeSqueeze;
            rb.linearVelocity = Vector2.zero;
            yield return new WaitForSeconds(0.3f);

            // Se dispara hacia arriba manteniendo la misma dirección
            rb.linearVelocity = new Vector2(dir * driftSpeed, riseSpeed);
            yield return new WaitForSeconds(riseTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            if (player.starpower)
            {
                GameManager.Instance?.AddEnemyKill();
                GetComponent<DeathAnimation>().enabled = true;
                Destroy(gameObject, 3f);
            }
            else
            {
                player.Hit();
            }
        }
    }

    private void Update()
    {
        Vector3 pos = transform.position;
        pos.y = Mathf.Clamp(pos.y, minY, maxY);
        transform.position = pos;
    }

    private float GetHorizontalDir()
    {
        if (player == null) return 0f;
        return player.position.x < transform.position.x ? -1f : 1f;
    }
}
