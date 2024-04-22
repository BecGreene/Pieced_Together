using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class SceneTransitions : MonoBehaviour
{
    Scene _scene;
    static int scene = 3;
    public static SceneTransitions Instance;
    public static bool nextLevelExists = true;
    public static bool[] unlockedLevels = new bool[7];
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

        unlockedLevels[0] = true;
        _scene = SceneManager.GetActiveScene();
        scene = _scene.buildIndex;
        if(scene == 0)
        {
            scene = 3;
        }
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
        if(scene >= 4)
        {
            unlockedLevels[scene - 3] = true;
        }
        SceneManager.LoadScene(scene);
        CheckNextLevel();
    }
    public static void LoadNumLevel(int num)
    {
        scene = num;
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
