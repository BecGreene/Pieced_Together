using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class SceneTransitions : MonoBehaviour
{
    Scene _scene;
    static int scene = 1;
    public static SceneTransitions Instance;
    public static bool nextLevelExists = true;
    // Start is called before the first frame update
    void Awake()
    {
        GameObject sceneManagers = GameObject.Find("SceneManager");

        if (sceneManagers != gameObject || (Instance != null && Instance != this))
        {
            Destroy(gameObject);
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        _scene = SceneManager.GetActiveScene();
        scene = _scene.buildIndex;
        Block.Won = false;
        BoardManager.Won = false;
        CollisionCursor.InUI = false;
        CheckNextLevel();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public static void LoadNextLevel()
    {
        Block.Won = false;
        BoardManager.Won = false;
        CollisionCursor.InUI = false;
        SceneManager.LoadScene(scene);
        CheckNextLevel();
    }
    public static void RestartLevel()
    {
        if(scene == 1)
        {
            Scene _scene = SceneManager.GetActiveScene();
            scene = _scene.buildIndex + 1;
        }
        scene--;
        LoadNextLevel();
    }
    public static void MainMenu()
    {
        scene = 0;
        LoadNextLevel();
    }
    private static void CheckNextLevel()
    {
        nextLevelExists = true;
        scene++;
        if (scene >= SceneManager.sceneCountInBuildSettings)
        {
            //Debug.Log(scene + "   " + SceneManager.sceneCountInBuildSettings);
            nextLevelExists = false;
            scene = 1;
        }
    }
}
