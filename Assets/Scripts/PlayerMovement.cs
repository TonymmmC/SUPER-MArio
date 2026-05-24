using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    private new Camera camera;
    private new Rigidbody2D rigidbody;
    private new Collider2D collider;

    private Vector2 velocity;
    private float inputAxis;

    public float moveSpeed = 8f;
    public float maxJumpHeight = 5f;
    public float maxJumpTime = 1f;
    public float swimKickSpeed = 3f;
    public float swimSinkSpeed = 1.5f;
    public AudioClip swimSound;

    public bool swimming { get; set; }
    public float jumpForce => (2f * maxJumpHeight) / (maxJumpTime / 2f);
    public float gravity => (-2f * maxJumpHeight) / Mathf.Pow(maxJumpTime / 2f, 2f);

    public bool grounded { get; private set; }
    public bool jumping { get; private set; }
    private bool wasSwimming;
    public bool running => Mathf.Abs(velocity.x) > 0.25f || Mathf.Abs(inputAxis) > 0.25f;
    public bool sliding => (inputAxis > 0f && velocity.x < 0f) || (inputAxis < 0f && velocity.x > 0f);
    public bool falling => velocity.y < 0f && !grounded;

    private void Awake()
    {
        camera = Camera.main;
        rigidbody = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();
    }

    private void OnEnable()
    {
        rigidbody.bodyType = RigidbodyType2D.Dynamic;
        collider.enabled = true;
        velocity = Vector2.zero;
        jumping = false;
    }

    private void OnDisable()
    {
        rigidbody.bodyType = RigidbodyType2D.Kinematic;
        collider.enabled = false;
        velocity = Vector2.zero;
        jumping = false;
    }

    public bool frozen = false;

    private void Update()
    {
        if (frozen)
        {
            velocity.x = 0f;
            rigidbody.MovePosition(rigidbody.position + velocity * Time.fixedDeltaTime);
            return;
        }

        // When Mario surfaces (transitions from swimming to normal while moving up), give him a jump boost
        if (wasSwimming && !swimming && velocity.y > 0f)
            velocity.y = jumpForce * 0.7f;
        wasSwimming = swimming;

        if (swimming)
        {
            HorizontalMovement();
            grounded = rigidbody.Raycast(Vector2.down);
            SwimUpdate();
        }
        else
        {
            HorizontalMovement();
            grounded = rigidbody.Raycast(Vector2.down);
            if (grounded) GroundedMovement();
            ApplyGravity();
        }
    }

    private void FixedUpdate()
    {
        // move mario based on his velocity
        Vector2 position = rigidbody.position;
        position += velocity * Time.fixedDeltaTime;

        // clamp within the visible camera bounds
        // camera.aspect already accounts for the viewport rect (pillarbox/letterbox),
        // so this stays correct regardless of screen resolution or aspect ratio
        float halfWidth = camera.orthographicSize * camera.aspect;
        position.x = Mathf.Clamp(position.x,
            camera.transform.position.x - halfWidth + 0.5f,
            camera.transform.position.x + halfWidth - 0.5f);

        rigidbody.MovePosition(position);
    }

    private void HorizontalMovement()
    {
        inputAxis = Input.GetAxisRaw("Horizontal");

        float rate = Mathf.Abs(inputAxis) > 0.1f
            ? moveSpeed * 3f   // acelera rápido al presionar
            : moveSpeed * 6f;  // frena más rápido al soltar

        float targetSpeed = inputAxis * moveSpeed * (swimming ? 0.4f : 1f);
        velocity.x = Mathf.MoveTowards(velocity.x, targetSpeed, rate * Time.deltaTime);

        // check if running into a wall
        if (rigidbody.Raycast(Vector2.right * velocity.x)) {
            RaycastHit2D hit = Physics2D.CircleCast(rigidbody.position, 0.25f,
                new Vector2(velocity.x, 0f).normalized, 0.375f, LayerMask.GetMask("Default"));
            if (hit.collider != null) {
                Pipe pipe = hit.collider.GetComponent<Pipe>() ?? hit.collider.GetComponentInParent<Pipe>();
                if (pipe != null && pipe.loadNextLevel && pipe.IsActiveFromDirection(new Vector2(velocity.x, 0f))) {
                    pipe.TriggerNextLevel(transform, new Vector2(velocity.x, 0f).normalized);
                    return;
                }
            }
            velocity.x = 0f;
        }

        // flip sprite to face direction
        if (velocity.x > 0f) {
            transform.eulerAngles = Vector3.zero;
        } else if (velocity.x < 0f) {
            transform.eulerAngles = new Vector3(0f, 180f, 0f);
        }
    }

    private void SwimUpdate()
    {
        if (Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.JoystickButton0))
        {
            velocity.y = swimKickSpeed;
            jumping = true;
            if (swimSound != null)
                AudioManager.Instance?.PlaySFX(swimSound);
        }
        else if (grounded)
        {
            velocity.y = Mathf.Max(velocity.y, 0f);
            jumping = false;
        }
        else
        {
            velocity.y = Mathf.MoveTowards(velocity.y, -swimSinkSpeed, 5f * Time.deltaTime);
            jumping = velocity.y > 0f;
        }

        velocity.y = Mathf.Clamp(velocity.y, -swimSinkSpeed, swimKickSpeed);
    }

    private void GroundedMovement()
    {
        // prevent gravity from infinitly building up
        velocity.y = Mathf.Max(velocity.y, 0f);
        jumping = velocity.y > 0f;

        // perform jump — teclado (Space) o mando Xbox (botón A)
        if (Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.JoystickButton0))
        {
            velocity.y = jumpForce;
            jumping = true;
            AudioManager.Instance?.PlayJump();
        }
    }

    private void ApplyGravity()
    {
        // check if falling
        bool falling = velocity.y < 0f || !Input.GetButton("Jump");
        float multiplier = falling ? 2f : 1f;

        // apply gravity and terminal velocity
        velocity.y += gravity * multiplier * Time.deltaTime;
        velocity.y = Mathf.Max(velocity.y, gravity / 2f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            // bounce off enemy head
            if (transform.DotTest(collision.transform, Vector2.down))
            {
                velocity.y = jumpForce / 2f;
                jumping = true;
            }
        }
        else if (collision.gameObject.layer != LayerMask.NameToLayer("PowerUp"))
        {
            // stop vertical movement if mario bonks his head
            if (transform.DotTest(collision.transform, Vector2.up)) {
                velocity.y = 0f;
            }
        }
    }

}
