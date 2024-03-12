using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class SceneTransitions : MonoBehaviour
{
    static int scene = 1;
    public static SceneTransitions Instance;
    public static bool nextLevelExists = true;
    // Start is called before the first frame update
    void Awake()
    {
        GameObject sceneManagers = GameObject.Find("SceneManager");

        if (sceneManagers != gameObject)
        {
            Destroy(gameObject);
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public static void LoadNextLevel()
    {
        Block.Won = false;
        BoardManager.Won = false;
        CollisionCursor.InUI = false;
        SceneManager.LoadScene(scene);
        scene++;
        if(scene >= SceneManager.sceneCount)
        {
            nextLevelExists = false;
            scene = 0;
        }
    }
    public static void RestartLevel()
    {
        scene--;
        LoadNextLevel();
    }
    public static void MainMenu()
    {

    }
}
