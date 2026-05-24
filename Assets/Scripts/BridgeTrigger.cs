using System.Collections;
using UnityEngine;

public class BridgeTrigger : MonoBehaviour
{
    public GateCell[] gates;
    public Bowser bowser;
    public float arenaCenterX = 0f;
    public float cameraTransitionSpeed = 0.8f;

    [Header("Audio")]
    public AudioClip battleMusic;

    private bool triggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered || !other.CompareTag("Player")) return;
        triggered = true;
        GetComponent<Collider2D>().enabled = false;
        StartCoroutine(BossIntro(other.gameObject));
    }

    private IEnumerator BossIntro(GameObject playerObj)
    {
        PlayerMovement movement = playerObj.GetComponent<PlayerMovement>();
        SideScrolling cam = Camera.main.GetComponent<SideScrolling>();

        if (movement != null) movement.frozen = true;

        AudioManager.Instance?.StopMusic();

        cam?.Lock(arenaCenterX, cameraTransitionSpeed);

        yield return new WaitUntil(() => cam == null || !cam.IsTransitioning);

        if (movement != null) movement.frozen = false;

        foreach (GateCell gate in gates)
            gate.Close();

        if (battleMusic != null)
            AudioManager.Instance?.PlayMusic(battleMusic);

        bowser?.StartFight();
    }
}
