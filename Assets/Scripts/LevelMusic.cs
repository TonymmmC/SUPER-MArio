using System.Collections;
using UnityEngine;

public class LevelMusic : MonoBehaviour
{
    public AudioClip music;

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => AudioManager.Instance != null);
        AudioManager.Instance.PlayMusic(music);
    }
}
