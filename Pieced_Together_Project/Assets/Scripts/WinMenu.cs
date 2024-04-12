using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WinMenu : MonoBehaviour
{
    private Image clipboard;
    private Button nextLevel;
    private Button redoLevel;
    private GameObject sassyComments;
    public Sprite[] Clipboards;
    private int Stars;
    public static WinMenu Instance;
    private bool won = false;
    private bool fastForward = false;
    private List<string> comments = new List<string>()
    {
        "Are you kidding me? You could've done this in |!\n<color=\"red\"><align=\"center\"> Paycut.",
        "What a great job. Could've done it in |, but you do you...",
        "Why did I even hire you?\nNext time do it in |.",
        "Jesus Christ, Jerry, get it together! What were you thinking!\nGet this done in | next time, or else you're demoted."
    };
    private void Awake()
    {
        Instance = this;
        nextLevel = transform.GetChild(2).GetComponent<Button>();
        redoLevel = transform.GetChild(3).GetComponent<Button>();
        clipboard = transform.GetChild(0).GetComponent<Image>();
        sassyComments = transform.GetChild(4).gameObject;
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
    private void Update()
    {
        if (!won) return;
        if (Input.GetKeyDown(KeyCode.Space) && won) fastForward = true;
    }
    private IEnumerator WinAnimation()
    {
        won = true;
        if (Stars >= 1)
        {
            yield return new WaitForSecondsRealtime(fastForward ? 0f : 0.5f);
            clipboard.sprite = Clipboards[1];
        }
        if (Stars >= 2)
        {
            yield return new WaitForSecondsRealtime(fastForward ? 0f : 1f);
            clipboard.sprite = Clipboards[2];
        }
        if (Stars >= 3)
        {
            yield return new WaitForSecondsRealtime(fastForward ? 0f : 1.25f);
            clipboard.sprite = Clipboards[3];
        }
        if(Stars < 3)
        {
            sassyComments.SetActive(true);
            sassyComments.GetComponentInChildren<TextMeshProUGUI>().text = comments[Random.Range(0, comments.Count)].Replace("|", $"<b>{BoardManager.Instance.LowestPossibleMoves}</b> moves");
        }
        yield return new WaitForSecondsRealtime(fastForward ? 0f : 0.75f);
        if (SceneTransitions.nextLevelExists) nextLevel.gameObject.SetActive(true);
        redoLevel.gameObject.SetActive(true);
        StopCoroutine(WinAnimation());
    }
}
