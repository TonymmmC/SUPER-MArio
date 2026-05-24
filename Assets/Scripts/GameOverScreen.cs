using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverScreen : MonoBehaviour
{
    public AudioClip gameOverMusic;

    private void Start()
    {
        AudioManager.Instance?.StopMusic();

        if (gameOverMusic != null)
        {
            AudioSource src = gameObject.AddComponent<AudioSource>();
            src.clip = gameOverMusic;
            src.loop = false;
            src.Play();
        }

        Invoke(nameof(BackToMenu), 5f);
    }

    private void BackToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void Restart()
    {
        GameManager.Instance?.NewGame();
    }
}
