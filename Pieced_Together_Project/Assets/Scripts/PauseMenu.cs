using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public bool GameIsPaused = false;
    private Image bg;
    private GameObject buttons;
    private Transform btns;
    private GameObject header;
    private void Awake()
    {
        //Get references to toggle visibility
        bg = GetComponent<Image>();
        header = transform.GetChild(0).gameObject;
        btns = transform.GetChild(1);
        buttons = btns.gameObject;
        HideOrShow();

        //Set button functions
        btns.GetChild(0).GetComponent<Button>().onClick.AddListener(Resume);
        btns.GetChild(1).GetComponent<Button>().onClick.AddListener(Restart);
        btns.GetChild(2).GetComponent<Button>().onClick.AddListener(MainMenu);
        btns.GetChild(3).GetComponent<Button>().onClick.AddListener(Quit);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            HideOrShow(!GameIsPaused);
        }
        if (Input.GetKey(KeyCode.R))
        {
            SceneTransitions.RestartLevel();
        }
    }

    private void HideOrShow(bool show = false)
    {
        bg.enabled = show;
        header.SetActive(show);
        buttons.SetActive(show);
        CollisionCursor.InUI = show;
        GameIsPaused = show;
    }

    private void Restart() => SceneTransitions.RestartLevel();
    private void Resume() => HideOrShow();
    private void MainMenu() => SceneTransitions.MainMenu();
    private void Quit()
    {
        Application.Quit();
        EditorApplication.ExitPlaymode();
    }
}
