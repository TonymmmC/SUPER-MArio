using UnityEngine;

// Mueve el objeto hacia abajo y lo reinicia al tope para efecto infinito
public class InfiniteScroll : MonoBehaviour
{
    public float speed = 2f;
    public float resetY;    // posicion Y donde se reinicia (arriba)
    public float destroyY;  // posicion Y donde se teletransporta al tope

    private float startY;

    private void Start()
    {
        startY = transform.position.y;
        if (resetY == 0f) resetY = startY;
        if (destroyY == 0f) destroyY = startY - 10f;
    }

    private void Update()
    {
        transform.Translate(Vector2.down * speed * Time.deltaTime);

        if (transform.position.y <= destroyY)
        {
            Vector3 pos = transform.position;
            pos.y = resetY;
            transform.position = pos;
        }
    }
}
