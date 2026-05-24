using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenu : MonoBehaviour
{
    public TMP_Text option1Text;
    public TMP_Text option2Text;
    public GameObject selector;

    private int selected = 0;
    private float elapsed;
    private bool wasVertical;

    private void Start()
    {
        MoveSelector();
    }

    private void Update()
    {
        elapsed += Time.deltaTime;
        if (elapsed < 0.5f) return;

        float vertical = Input.GetAxisRaw("Vertical");
        float dpad = 0f;
        try { dpad = Input.GetAxisRaw("DPadVertical"); } catch { }

        // D-pad como botones (mapeo alternativo Xbox en Windows)
        bool dpadUp   = Input.GetKeyDown(KeyCode.JoystickButton10) || Input.GetKeyDown(KeyCode.JoystickButton12);
        bool dpadDown = Input.GetKeyDown(KeyCode.JoystickButton11) || Input.GetKeyDown(KeyCode.JoystickButton13);

        float combined = Mathf.Abs(vertical) > Mathf.Abs(dpad) ? vertical : dpad;
        bool navigateUp   = Input.GetKeyDown(KeyCode.UpArrow)   || dpadUp  || (combined > 0.5f  && !wasVertical);
        bool navigateDown = Input.GetKeyDown(KeyCode.DownArrow)  || dpadDown || (combined < -0.5f && !wasVertical);
        wasVertical = Mathf.Abs(combined) > 0.5f;

        if (navigateUp || navigateDown)
        {
            selected = selected == 0 ? 1 : 0;
            MoveSelector();
        }

        // Teclado: Enter/Space  |  Xbox: A (JoystickButton0)
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space) ||
            Input.GetKeyDown(KeyCode.JoystickButton0))
        {
            if (selected == 0) StartGame();
            else QuitGame();
        }
    }

    private void MoveSelector()
    {
        if (selector == null) return;
        TMP_Text target = selected == 0 ? option1Text : option2Text;
        Vector3 pos = target.transform.position;
        selector.transform.position = new Vector3(pos.x - 120f, pos.y, pos.z);
    }

    private void StartGame()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.NewGame();
        else
            SceneManager.LoadScene("1-1");
    }

    private void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
