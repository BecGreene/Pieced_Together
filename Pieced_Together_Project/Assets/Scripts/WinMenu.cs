using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinMenu : MonoBehaviour
{
    private Image clipboard;
    private Button nextLevel;
    private Button redoLevel;
    public Sprite[] Clipboards;
    private int Stars;
    public static WinMenu Instance;
    private void Awake()
    {
        Instance = this;
        nextLevel = transform.GetChild(2).GetComponent<Button>();
        redoLevel = transform.GetChild(3).GetComponent<Button>();
        clipboard = transform.GetChild(0).GetComponent<Image>();
        nextLevel.onClick.AddListener(SceneTransitions.LoadNextLevel);
        nextLevel.gameObject.SetActive(false);
        redoLevel.onClick.AddListener(SceneTransitions.RestartLevel);
        redoLevel.gameObject.SetActive(false);
    }
    public void ShowWin(int stars)
    {
        Stars = stars;
        StartCoroutine(WinAnimation());
    }
    private IEnumerator WinAnimation()
    {
        if (Stars >= 1)
        {
            yield return new WaitForSecondsRealtime(0.5f);
            clipboard.sprite = Clipboards[1];
        }
        if (Stars >= 2)
        {
            yield return new WaitForSecondsRealtime(1f);
            clipboard.sprite = Clipboards[2];
        }
        if (Stars >= 3)
        {
            yield return new WaitForSecondsRealtime(1.25f);
            clipboard.sprite = Clipboards[3];
        }
        yield return new WaitForSecondsRealtime(0.75f);
        if (SceneTransitions.nextLevelExists) nextLevel.gameObject.SetActive(true);
        redoLevel.gameObject.SetActive(true);
        StopCoroutine(WinAnimation());
    }
}
