using UnityEngine;

[RequireComponent(typeof(Camera))]
public class SideScrolling : MonoBehaviour
{
    private new Camera camera;
    private Transform player;

    public float height = 6.5f;
    public float undergroundHeight = -9.5f;
    public float undergroundThreshold = 0f;
    public Vector2 targetResolution = new Vector2(1920, 1080);
    public float minX = 0f;

    private int lastWidth, lastHeight;

    private void Awake()
    {
        camera = GetComponent<Camera>();
        player = GameObject.FindWithTag("Player").transform;

        // Snap to player immediately so the first LateUpdate doesn't teleport the camera
        Vector3 pos = transform.position;
        pos.x = Mathf.Max(minX, player.position.x);
        transform.position = pos;

        EnforceAspect();
    }

    private bool locked = false;
    private float lockedX;
    private bool transitioning = false;
    public float lockSmoothSpeed = 3f;

    public bool IsTransitioning => transitioning;

    public void Lock(float x, float speed = -1f)
    {
        locked = true;
        transitioning = true;
        lockedX = x;
        if (speed > 0f) lockSmoothSpeed = speed;
    }

    public void Unlock()
    {
        locked = false;
        transitioning = false;
    }

    private void LateUpdate()
    {
        if (Screen.width != lastWidth || Screen.height != lastHeight)
            EnforceAspect();

        Vector3 cameraPosition = transform.position;

        if (transitioning)
        {
            float targetX = Mathf.Max(minX, lockedX);
            cameraPosition.x = Mathf.MoveTowards(cameraPosition.x, targetX, lockSmoothSpeed * Time.deltaTime);
            if (Mathf.Abs(cameraPosition.x - targetX) < 0.01f)
            {
                cameraPosition.x = targetX;
                transitioning = false;
            }
        }
        else if (!locked)
        {
            cameraPosition.x = Mathf.Max(minX, player.position.x);
        }

        transform.position = cameraPosition;
    }

    // Enforces a fixed aspect ratio by adjusting the camera's viewport rect.
    // Wider screens get pillarboxes (black side bars); taller screens get letterboxes.
    private void EnforceAspect()
    {
        lastWidth = Screen.width;
        lastHeight = Screen.height;

        float targetAspect = targetResolution.x / targetResolution.y;
        float actualAspect = (float)Screen.width / Screen.height;

        if (actualAspect > targetAspect)
        {
            float w = targetAspect / actualAspect;
            camera.rect = new Rect((1f - w) * 0.5f, 0f, w, 1f);
        }
        else
        {
            float h = actualAspect / targetAspect;
            camera.rect = new Rect(0f, (1f - h) * 0.5f, 1f, h);
        }
    }

    public void SetUnderground(bool underground)
    {
        Vector3 cameraPosition = transform.position;
        cameraPosition.y = underground ? undergroundHeight : height;
        transform.position = cameraPosition;
    }
}
