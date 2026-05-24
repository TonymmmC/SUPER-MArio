using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryTrigger : MonoBehaviour
{
    [Header("Cámara")]
    public float arenaCenterX = 141f;
    public float cameraTransitionSpeed = 0.8f;

    [Header("Mario auto-camina hasta aquí")]
    public float walkTargetX = 0f;
    public float walkSpeed = 3f;

    [Header("Audio")]
    public AudioClip victoryMusic;

    [Header("Timing")]
    public float waitAfterMusic = 5f;

    private bool triggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered || !other.CompareTag("Player")) return;
        triggered = true;
        GetComponent<Collider2D>().enabled = false;
        StartCoroutine(VictorySequence(other.gameObject));
    }

    private IEnumerator VictorySequence(GameObject playerObj)
    {
        PlayerMovement movement = playerObj.GetComponent<PlayerMovement>();
        SideScrolling cam = Camera.main?.GetComponent<SideScrolling>();

        if (movement != null) movement.frozen = true;

        // Centrar cámara — timeout de 3s por si la transición falla
        cam?.Lock(arenaCenterX, cameraTransitionSpeed);
        float camTimeout = 0f;
        while (cam != null && cam.IsTransitioning && camTimeout < 3f)
        {
            camTimeout += Time.deltaTime;
            yield return null;
        }

        // Música de victoria
        AudioManager.Instance?.StopMusic();
        if (victoryMusic != null)
            AudioManager.Instance?.PlayMusic(victoryMusic);

        // Mario camina solo hacia walkTargetX
        if (movement != null && walkTargetX > playerObj.transform.position.x)
        {
            gameObject.SetActive(false);
            movement.frozen = false;
            Rigidbody2D rb = playerObj.GetComponent<Rigidbody2D>();
            while (playerObj.transform.position.x < walkTargetX - 0.1f)
            {
                if (rb != null)
                    rb.linearVelocity = new Vector2(walkSpeed, rb.linearVelocity.y);
                yield return null;
            }
            if (rb != null)
                rb.linearVelocity = Vector2.zero;
            movement.frozen = true;
        }

        yield return new WaitForSeconds(waitAfterMusic);

        SceneManager.LoadScene("MainMenu");
    }
}
