using System.Collections;
using UnityEngine;

public class BowserFireball : MonoBehaviour
{
    public Sprite sprite1;
    public Sprite sprite2;
    public float animSpeed = 0.15f;
    public float fallSpeed = 3f;
    public float destroyBelowY = -5f;
    public AudioClip spawnSound;

    private SpriteRenderer sr;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        if (spawnSound != null)
            AudioSource.PlayClipAtPoint(spawnSound, transform.position);
        StartCoroutine(Animate());
    }

    private IEnumerator Animate()
    {
        while (true)
        {
            sr.sprite = sprite1;
            yield return new WaitForSeconds(animSpeed);
            sr.sprite = sprite2;
            yield return new WaitForSeconds(animSpeed);
        }
    }

    private void Update()
    {
        transform.position += Vector3.down * fallSpeed * Time.deltaTime;

        if (transform.position.y < destroyBelowY)
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        other.GetComponent<Player>()?.Hit();
        Destroy(gameObject);
    }
}
