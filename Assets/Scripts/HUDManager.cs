using UnityEngine;
using UnityEngine.UI;
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

    private void Start()
    {
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas == null) return;

        // Attach the canvas to the game camera so it respects the camera's viewport rect
        // (the camera enforces a fixed 16:9 area with pillarboxing/letterboxing)
        if (Camera.main != null)
        {
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = Camera.main;
        }

        CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();
        if (scaler != null)
        {
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;
        }
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
