using System.Collections;
using UnityEngine;

public class Bowser : MonoBehaviour
{
    [Header("Sprites")]
    public Sprite spriteIdle;
    public Sprite spriteMouth;
    public Sprite spriteVulnerable;
    public Sprite spriteVulnerable2;
    public float animSpeed = 0.4f;

    [Header("Stats")]
    public int health = 9;

    [Header("Arena")]
    public float arenaMinX = 108f;
    public float arenaMaxX = 136f;
    public float spawnY = 14f;

    [Header("Fase 1 - Embestida")]
    public float chargeSpeed = 8f;
    public float chargeSpeedEnraged = 13f;
    public float pauseBetweenCharges = 0.5f;

    [Header("Fase 2 - Saltos")]
    public float jumpHeight = 3f;
    public float jumpDuration = 0.6f;
    public float jumpHeightEnraged = 4.5f;
    public float jumpDurationEnraged = 0.35f;

    [Header("Fireballs")]
    public GameObject fireballPrefab;
    public float spawnInterval = 0.5f;
    public float spawnIntervalEnraged = 0.3f;
    public GameObject mushroomPrefab;
    [Range(0f, 1f)] public float mushroomChance = 0.25f;

    [Header("Fight")]
    public float vulnerableTime = 2.5f;
    public BridgeTrigger bridgeTrigger;

    [Header("Audio")]
    public AudioClip stompSound;
    public AudioClip wallHitSound;
    public AudioClip chargeRoarSound;
    public AudioClip jumpLaughSound;

    private SpriteRenderer sr;
    private AudioSource audioSource;
    private Transform player;
    private float startY;
    private bool dead = false;
    private bool vulnerable = false;
    private bool fighting = false;
    private bool contactDamages = false;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        startY = transform.position.y;
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        StartCoroutine(Animate());
    }

    private IEnumerator Animate()
    {
        while (!dead)
        {
            if (vulnerable && spriteVulnerable != null && spriteVulnerable2 != null)
            {
                sr.sprite = spriteVulnerable;
                yield return new WaitForSeconds(animSpeed);
                sr.sprite = spriteVulnerable2;
                yield return new WaitForSeconds(animSpeed);
            }
            else
            {
                sr.sprite = spriteIdle;
                yield return new WaitForSeconds(animSpeed);
                sr.sprite = spriteMouth;
                yield return new WaitForSeconds(animSpeed);
            }
        }
    }

    private void Update()
    {
        if (dead) return;
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player != null)
            sr.flipX = player.position.x > transform.position.x;
    }

    public void StartFight()
    {
        fighting = true;
        StartCoroutine(FightLoop());
    }

    private int GetPhase()
    {
        if (health >= 7) return 1;
        if (health >= 4) return 2;
        return 3;
    }

    private IEnumerator FightLoop()
    {
        while (!dead)
        {
            switch (GetPhase())
            {
                case 1: yield return StartCoroutine(Phase1()); break;
                case 2: yield return StartCoroutine(Phase2()); break;
                case 3: yield return StartCoroutine(Phase3()); break;
            }

            if (dead) yield break;

            vulnerable = true;
            yield return new WaitForSeconds(vulnerableTime);
            vulnerable = false;
        }
    }

    // ── FASE 1: Embestida ──────────────────────────────────────────────────

    private IEnumerator Phase1()
    {
        int charges = health >= 8 ? 2 : 3;
        float speed = health >= 8 ? chargeSpeed : chargeSpeedEnraged;

        for (int i = 0; i < charges && !dead; i++)
        {
            yield return StartCoroutine(WarnJump());
            contactDamages = true;
            yield return StartCoroutine(ChargeToWall(speed));
            contactDamages = false;
            if (i < charges - 1)
                yield return new WaitForSeconds(pauseBetweenCharges);
        }
    }

    private IEnumerator WarnJump()
    {
        if (chargeRoarSound != null && audioSource != null) audioSource.PlayOneShot(chargeRoarSound);

        float groundY = startY;
        float warnHeight = 1f;
        float half = 0.2f;

        float t = 0f;
        while (t < half && !dead)
        {
            t += Time.deltaTime;
            transform.position = new Vector3(transform.position.x,
                Mathf.Lerp(groundY, groundY + warnHeight, t / half), transform.position.z);
            yield return null;
        }
        t = 0f;
        while (t < half && !dead)
        {
            t += Time.deltaTime;
            transform.position = new Vector3(transform.position.x,
                Mathf.Lerp(groundY + warnHeight, groundY, t / half), transform.position.z);
            yield return null;
        }
        transform.position = new Vector3(transform.position.x, groundY, transform.position.z);
        yield return new WaitForSeconds(0.4f);
    }

    private IEnumerator ChargeToWall(float speed)
    {
        if (player == null) yield break;

        float dir = player.position.x > transform.position.x ? 1f : -1f;
        float targetX = dir > 0f ? arenaMaxX : arenaMinX;

        while (!dead && Mathf.Abs(transform.position.x - targetX) > 0.1f)
        {
            transform.position += Vector3.right * dir * speed * Time.deltaTime;
            float clampedX = Mathf.Clamp(transform.position.x, arenaMinX, arenaMaxX);
            transform.position = new Vector3(clampedX, transform.position.y, transform.position.z);
            yield return null;
        }

        transform.position = new Vector3(targetX, transform.position.y, transform.position.z);
        CameraShake.Instance?.Shake(0.5f, 0.35f);
        if (wallHitSound != null && audioSource != null) audioSource.PlayOneShot(wallHitSound);
        yield return new WaitForSeconds(0.2f);
    }

    // ── FASE 2: Saltos + Bolas ─────────────────────────────────────────────

    private IEnumerator Phase2()
    {
        contactDamages = false;

        float duration      = health >= 5 ? 5f : 8f;
        float jHeight       = health >= 5 ? jumpHeight : jumpHeightEnraged;
        float jDuration     = health >= 5 ? jumpDuration : jumpDurationEnraged;
        float fInterval     = health >= 5 ? spawnInterval : spawnIntervalEnraged;

        Coroutine fireballs = StartCoroutine(FireballRain(duration, fInterval));
        yield return StartCoroutine(JumpLoop(duration, jHeight, jDuration));
        StopCoroutine(fireballs);
    }

    private IEnumerator JumpLoop(float duration, float height, float jDur)
    {
        float elapsed = 0f;
        while (elapsed < duration && !dead)
        {
            yield return StartCoroutine(DoJump(height, jDur));
            elapsed += jDur + 0.3f;
        }
    }

    private IEnumerator DoJump(float height, float duration)
    {
        float groundY = startY;
        float half = duration / 2f;

        CameraShake.Instance?.Shake(0.2f, 0.15f);
        if (jumpLaughSound != null && audioSource != null) audioSource.PlayOneShot(jumpLaughSound);

        float t = 0f;
        while (t < half && !dead)
        {
            t += Time.deltaTime;
            float y = Mathf.Lerp(groundY, groundY + height, t / half);
            transform.position = new Vector3(transform.position.x, y, transform.position.z);
            yield return null;
        }

        t = 0f;
        while (t < half && !dead)
        {
            t += Time.deltaTime;
            float y = Mathf.Lerp(groundY + height, groundY, t / half);
            transform.position = new Vector3(transform.position.x, y, transform.position.z);
            yield return null;
        }

        transform.position = new Vector3(transform.position.x, groundY, transform.position.z);
        CameraShake.Instance?.Shake(0.35f, 0.25f);
        yield return new WaitForSeconds(0.3f);
    }

    // ── FASE 3: 3 saltos con bolas → 2s bolas + embestidas sin saltito ───

    private IEnumerator Phase3()
    {
        contactDamages = false;

        Coroutine fireballs = StartCoroutine(FireballRainLoop());

        // 3 saltos con bolas cayendo
        for (int i = 0; i < 3 && !dead; i++)
            yield return StartCoroutine(DoJump(jumpHeightEnraged, jumpDurationEnraged));

        // 2 segundos extra: bolas siguen + embestidas directas sin WarnJump
        contactDamages = true;
        float elapsed = 0f;
        while (elapsed < 2f && !dead)
        {
            float before = Time.time;
            yield return StartCoroutine(ChargeToWall(chargeSpeedEnraged));
            elapsed += Time.time - before;
            if (elapsed < 2f)
                yield return new WaitForSeconds(pauseBetweenCharges);
        }

        StopCoroutine(fireballs);
        contactDamages = false;
    }

    private IEnumerator ChargeLoop(float duration, float speed)
    {
        float elapsed = 0f;
        while (elapsed < duration && !dead)
        {
            float before = Time.time;
            yield return StartCoroutine(ChargeToWall(speed));
            elapsed += Time.time - before + pauseBetweenCharges;
            if (elapsed < duration)
                yield return new WaitForSeconds(pauseBetweenCharges);
        }
    }

    // ── Fireballs ──────────────────────────────────────────────────────────

    private IEnumerator FireballRain(float duration, float interval)
    {
        float elapsed = 0f;
        while (elapsed < duration && !dead)
        {
            SpawnFireball();
            yield return new WaitForSeconds(interval);
            elapsed += interval;
        }
    }

    private IEnumerator FireballRainLoop()
    {
        while (!dead)
        {
            SpawnFireball();
            yield return new WaitForSeconds(spawnIntervalEnraged);
        }
    }

    private void SpawnFireball()
    {
        float randomX = Random.Range(arenaMinX, arenaMaxX);
        Vector3 pos = new Vector3(randomX, spawnY, 0f);

        Player playerComp = FindAnyObjectByType<Player>();
        bool marioIsSmall = playerComp == null || !playerComp.big;

        if (mushroomPrefab != null && marioIsSmall && Random.value < mushroomChance)
            Instantiate(mushroomPrefab, pos, Quaternion.identity);
        else if (fireballPrefab != null)
            Instantiate(fireballPrefab, pos, Quaternion.identity);
    }

    // ── Daño y Muerte ──────────────────────────────────────────────────────

    public void Defeat()
    {
        if (dead) return;
        health = 1;
        vulnerable = true;
        Hit();
    }

    public void Hit()
    {
        if (dead || !vulnerable) return;
        health--;
        vulnerable = false;

        if (stompSound != null && audioSource != null)
            audioSource.PlayOneShot(stompSound);

        if (health <= 0)
            StartCoroutine(Die());
        else
            StartCoroutine(FlashRed());
    }

    private IEnumerator FlashRed()
    {
        for (int i = 0; i < 4; i++)
        {
            sr.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            sr.color = Color.white;
            yield return new WaitForSeconds(0.1f);
        }
    }

    private IEnumerator Die()
    {
        dead = true;
        GetComponent<Collider2D>().enabled = false;

        // 3 golpes finales — pum pum pum
        for (int stomp = 0; stomp < 3; stomp++)
        {
            if (stompSound != null && audioSource != null)
                audioSource.PlayOneShot(stompSound);

            for (int i = 0; i < 3; i++)
            {
                sr.color = Color.red;
                yield return new WaitForSeconds(0.1f);
                sr.color = Color.white;
                yield return new WaitForSeconds(0.1f);
            }
            CameraShake.Instance?.Shake(0.5f, 0.35f);
            yield return new WaitForSeconds(0.25f);
        }

        // Cae a la lava
        float elapsed = 0f;
        float fallDuration = 1.2f;
        float fromY = transform.position.y;
        float toY = fromY - 12f;

        while (elapsed < fallDuration)
        {
            elapsed += Time.deltaTime;
            float y = Mathf.Lerp(fromY, toY, elapsed / fallDuration);
            transform.position = new Vector3(transform.position.x, y, transform.position.z);
            yield return null;
        }

        sr.enabled = false;
        AudioManager.Instance?.StopMusic();

        yield return new WaitForSeconds(1f);

        Camera.main.GetComponent<SideScrolling>()?.Unlock();
        if (bridgeTrigger != null)
        {
            foreach (GateCell gate in bridgeTrigger.gates)
                gate.Open();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (dead || !fighting || !collision.gameObject.CompareTag("Player")) return;

        Player playerComp = collision.gameObject.GetComponent<Player>();
        if (playerComp == null) return;

        if (collision.transform.DotTest(transform, Vector2.down))
        {
            Hit();
        }
        else if (contactDamages)
        {
            playerComp.Hit();
        }
    }
}
