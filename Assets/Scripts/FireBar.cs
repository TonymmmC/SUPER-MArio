using UnityEngine;

public class FireBar : MonoBehaviour
{
    public float rotationSpeed = 90f;
    public bool clockwise = false;

    private void Update()
    {
        float direction = clockwise ? -1f : 1f;
        transform.Rotate(0f, 0f, rotationSpeed * direction * Time.deltaTime);
    }
}
