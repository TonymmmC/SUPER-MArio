using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Música")]
    public AudioClip backgroundMusic;

    [Header("SFX")]
    public AudioClip jumpSFX;
    public AudioClip stompSFX;
    public AudioClip powerupSFX;
    public AudioClip damageSFX;
    public AudioClip deathSFX;
    public AudioClip coinSFX;
    public AudioClip stageClearSFX;

    private AudioSource musicSource;
    private AudioSource sfxSource;

    private void Awake()
    {
        if (Instance != null) {
            DestroyImmediate(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;
        musicSource.playOnAwake = false;

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    private void OnEnable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        // Reinicia la música al recargar el nivel (ej. tras morir)
        PlayMusic();
    }

    private void Start()
    {
        if (backgroundMusic != null) {
            musicSource.clip = backgroundMusic;
            musicSource.Play();
        }
    }

    public void PlayMusic(AudioClip clip)
    {
        musicSource.clip = clip;
        musicSource.Play();
    }

    public void PlayMusic()
    {
        if (backgroundMusic != null) {
            musicSource.clip = backgroundMusic;
            musicSource.Play();
        }
    }

    public void StopMusic() => musicSource.Stop();

    public void PlaySFX(AudioClip clip)
    {
        if (clip != null) sfxSource.PlayOneShot(clip);
    }

    public void PlayJump()       => PlaySFX(jumpSFX);
    public void PlayStomp()      => PlaySFX(stompSFX);
    public void PlayPowerup()    => PlaySFX(powerupSFX);
    public void PlayDamage()     => PlaySFX(damageSFX);
    public void PlayDeath()      => PlaySFX(deathSFX);
    public void PlayCoin()       => PlaySFX(coinSFX);
    public void PlayStageClear() { StopMusic(); PlaySFX(stageClearSFX); }
}
