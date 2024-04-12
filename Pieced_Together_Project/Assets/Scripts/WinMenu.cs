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
        "Jerry... It's |. This is going on your performance review.",
        "Are you kidding me? You could've done this in |!\n<color=\"red\"><align=\"center\"><b>Paycut.",
        "What a great job. Could've done it in |, but you do you...",
        "Why did I even hire you?\nNext time do it in |.",
        "Jesus Christ, Jerry, get it together! What were you thinking!\nGet this done in | next time, or else you're \n<color=\"red\"><align=\"center\"><b>demoted.",
        "Friendly reminder, our customers don't like broken boxes. Getting | protects the boxes :)",
        "That was surprisingly efficient.\nFor a worker of you abilities.\nAny normal worker would've gotten it in |, but good on you!",
        "We are on a deadline, but I guess you have nowhere to be...\nI'll contact upper management and tell them we failed to reach |, it's <i>no</i> problem.",
        "<align=\"center\">I'm not mad\n<align=\"left\">It's not <i>your</i> fault you didn't get |, that's just impossible, isn't it?",
        "You're so lucky Sharon likes you, or else you'd be fired on the spot. You only needed |.",
        "Correct me if I'm wrong, but you are <i>paid</i> to do this, right? How are you missing |?",
        "???\nWtf did you even do?\nYou only needed |.",
        "Just to be sure we're on the same page, you do know we want to move the boxes as little as possible? As in, |?",
        "Don't be surprised when Kevin gets promoted before you. He got |. It's not an insult, just a fact.",
        "Do we even know what's in that red box? It seems to glow sometimes. Don't move things too much, just |.\n<size=18><i>Please..."
    };
    string comment;
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
        comment = comments[Random.Range(0, comments.Count)].Replace("|", $"<b>{BoardManager.Instance.LowestPossibleMoves}</b> move{(BoardManager.Instance.LowestPossibleMoves > 1 ? "s" : "")}");
        StartCoroutine(WinAnimation());
    }
    private void Update()
    {
        if (!won) return;
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(1)) && won)
        {
            fastForward = true;
            StopCoroutine(WinAnimation());
            StartCoroutine(WinAnimation());
        }
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
            sassyComments.GetComponentInChildren<TextMeshProUGUI>().text = comment;
        }
        yield return new WaitForSecondsRealtime(fastForward ? 0f : 0.75f);
        if (SceneTransitions.nextLevelExists) nextLevel.gameObject.SetActive(true);
        redoLevel.gameObject.SetActive(true);
        StopCoroutine(WinAnimation());
    }
}
