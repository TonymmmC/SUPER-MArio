using UnityEngine;

public class SwimMovement : MonoBehaviour
{
    public float swimSpeed = 3f;
    public float kickSpeed = 3f;
    public float sinkSpeed = 1.5f;

    private Rigidbody2D rb;
    private PlayerMovement movement;
    private PlayerSpriteRenderer[] spriteRenderers;
    private Vector2 velocity;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        movement = GetComponent<PlayerMovement>();
        spriteRenderers = GetComponentsInChildren<PlayerSpriteRenderer>();
    }

    private void OnEnable()
    {
        if (movement != null) movement.enabled = false;
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.linearVelocity = Vector2.zero;
        velocity = Vector2.zero;
        GetComponent<Collider2D>().enabled = true;
    }

    private void OnDisable()
    {
        if (movement != null) movement.enabled = true;
    }

    private void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        velocity.x = h * swimSpeed;

        if (Input.GetButtonDown("Jump"))
            velocity.y = kickSpeed;
        else
            velocity.y = Mathf.MoveTowards(velocity.y, -sinkSpeed, 5f * Time.deltaTime);

        if (h > 0f) transform.eulerAngles = Vector3.zero;
        else if (h < 0f) transform.eulerAngles = new Vector3(0f, 180f, 0f);

        foreach (var sr in spriteRenderers)
            if (sr.enabled) sr.spriteRenderer.sprite = sr.jump;
    }

    private void FixedUpdate()
    {
        Vector2 position = rb.position + velocity * Time.fixedDeltaTime;
        rb.MovePosition(position);
    }
}
