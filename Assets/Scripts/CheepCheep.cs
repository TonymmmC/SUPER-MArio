using System.Collections;
using UnityEngine;

public class CheepCheep : MonoBehaviour
{
    public Sprite sprite1;
    public Sprite sprite2;
    public float swimSpeed = 2f;
    public float animFrameTime = 0.2f;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private float direction = -1f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        rb.gravityScale = 0f;
        rb.linearVelocity = new Vector2(direction * swimSpeed, 0f);
        StartCoroutine(Animate());
    }

    private void Update()
    {
        rb.linearVelocity = new Vector2(direction * swimSpeed, 0f);
        transform.eulerAngles = direction > 0f ? new Vector3(0f, 180f, 0f) : Vector3.zero;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Player player = collision.gameObject.GetComponent<Player>();

            if (player.starpower || collision.transform.DotTest(transform, Vector2.down))
            {
                GameManager.Instance?.AddEnemyKill();
                GetComponent<Collider2D>().enabled = false;
                GetComponent<DeathAnimation>().enabled = true;
                Destroy(gameObject, 3f);
            }
            else
            {
                player.Hit();
            }
        }
        else
        {
            direction *= -1f;
        }
    }

    private IEnumerator Animate()
    {
        while (true)
        {
            sr.sprite = sprite1;
            yield return new WaitForSeconds(animFrameTime);
            sr.sprite = sprite2;
            yield return new WaitForSeconds(animFrameTime);
        }
    }
}
