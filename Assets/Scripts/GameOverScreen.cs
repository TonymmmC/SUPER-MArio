using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverScreen : MonoBehaviour
{
    public AudioClip gameOverMusic;

    private void Start()
    {
        if (gameOverMusic != null)
            AudioManager.Instance?.PlayMusic(gameOverMusic);

        Invoke(nameof(BackToMenu), 4f);
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
