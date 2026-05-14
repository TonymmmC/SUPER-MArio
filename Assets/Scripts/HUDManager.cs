using UnityEngine;
using TMPro;

public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance { get; private set; }

    public TMP_Text timeText;
    public TMP_Text scoreText;
    public TMP_Text coinsText;

    private void Awake()
    {
        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    public void UpdateTime(int time)
    {
        if (timeText != null) timeText.text = $"{time:000}";
    }

    public void UpdateScore(int score)
    {
        if (scoreText != null) scoreText.text = $"{score:000000}";
    }

    public void UpdateCoins(int coins)
    {
        if (coinsText != null) coinsText.text = $"x{coins:00}";
    }
}
