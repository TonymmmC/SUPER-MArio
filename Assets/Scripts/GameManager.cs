using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int world { get; private set; }
    public int stage { get; private set; }
    public int lives { get; private set; }
    public int coins { get; private set; }
    public int score { get; private set; }
    public float timeRemaining { get; private set; }
    public bool wasBig { get; private set; }

    private bool timerRunning;

    private void Awake()
    {
        if (Instance != null) {
            DestroyImmediate(gameObject);
        } else {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (Instance == this) {
            Instance = null;
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        HUDManager.Instance?.UpdateTime(Mathf.CeilToInt(timeRemaining));
        HUDManager.Instance?.UpdateScore(score);
        HUDManager.Instance?.UpdateCoins(coins);

        if (wasBig)
        {
            Player player = FindAnyObjectByType<Player>();
            if (player != null && !player.big)
                player.Grow();
        }
    }

    public void SavePlayerState(Player player)
    {
        wasBig = player != null && player.big;
    }

    private void Start()
    {
        Application.targetFrameRate = 60;

        string scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        if (scene == "MainMenu" || scene == "GameOver")
        {
            // Solo inicializar stats, el boton Play del menu llama a NewGame()
            lives = 3; coins = 0; score = 0; timeRemaining = 400f; timerRunning = false;
        }
        else
        {
            // Arranca directamente desde un nivel (prueba en editor)
            lives = 3; coins = 0; score = 0;
            timeRemaining = 400f;
            timerRunning = true;

            string[] parts = scene.Split('-');
            if (parts.Length == 2 && int.TryParse(parts[0], out int w) && int.TryParse(parts[1], out int s))
            {
                world = w;
                stage = s;
            }
        }
    }

    private void Update()
    {
        if (!timerRunning) return;

        // Ticks a 2.5 unidades/segundo para sentirse como el Mario clásico
        timeRemaining -= Time.deltaTime * 2.5f;

        if (timeRemaining <= 0f)
        {
            timeRemaining = 0f;
            timerRunning = false;
            Player player = FindAnyObjectByType<Player>();
            if (player != null && !player.dead) {
                player.Death();
            }
        }

        HUDManager.Instance?.UpdateTime(Mathf.CeilToInt(timeRemaining));
    }

    public void NewGame()
    {
        lives = 3;
        coins = 0;
        score = 0;

        LoadLevel(1, 1);
    }

    public void GameOver()
    {
        timerRunning = false;
        AudioManager.Instance?.StopMusic();
        SceneManager.LoadScene("GameOver");
    }

    public void LoadLevel(int world, int stage)
    {
        this.world = world;
        this.stage = stage;
        timeRemaining = 400f;
        timerRunning = true;

        SceneManager.LoadScene($"{world}-{stage}");
    }

    public void NextLevel()
    {
        LoadLevel(world, stage + 1);
    }

    public void ResetLevel(float delay)
    {
        timerRunning = false;
        CancelInvoke(nameof(ResetLevel));
        Invoke(nameof(ResetLevel), delay);
    }

    public void ResetLevel()
    {
        lives--;

        if (lives > 0) {
            LoadLevel(world, stage);
        } else {
            GameOver();
        }
    }

    public void AddCoin()
    {
        coins++;
        AddScore(100);

        if (coins == 100)
        {
            coins = 0;
            AddLife();
        }

        HUDManager.Instance?.UpdateCoins(coins);
        AudioManager.Instance?.PlayCoin();
    }

    public void AddLife()
    {
        lives++;
    }

    public void AddScore(int points)
    {
        score += points;
        HUDManager.Instance?.UpdateScore(score);
    }

    public void AddEnemyKill()
    {
        AddScore(200);
    }

}
