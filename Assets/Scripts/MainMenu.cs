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

    private void Start()
    {
        MoveSelector();
    }

    private void Update()
    {
        elapsed += Time.deltaTime;
        if (elapsed < 0.5f) return;

        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            selected = selected == 0 ? 1 : 0;
            MoveSelector();
        }

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
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
