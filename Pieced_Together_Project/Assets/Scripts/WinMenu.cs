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
    private GameObject[] commentGO;
    public Sprite[] Clipboards;
    private int Stars;
    public static WinMenu Instance;
    private bool won = false;
    private bool skip = false;
    private readonly List<string> comments_bad = new List<string>()
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
    private readonly List<string> comments_good = new List<string>()
    {
        "Nice one! You truely are at the top of the bell curve.",
        "Good one, Jerry. If only you were this efficient all the time.",
        "Keep this up and you might get a promotion!\n<size=18>In 10 years",
        "Who knew you were capable of success.",
        "Impressive. Though Kevin got | in half the time it took you...",
        "Damn it Jerry, now I can't give you a paycut! We were so close!",
        "Stop trying to impress Sharon, it's not happening.",
        "Nice! Next time how about we go a little faster, hm? :)",
        "This still doesn't make you eligable for that bonus. Nice try, though.",
        "I <i>guess</i> we can strike <b>one</b> shortcoming from the record...",
        "Okay, now you're just <i>copying</i> Kevin. Do good without leeching off him!",
        "Great, now you're going to start thinking you're actually <i>good</i> at this job...",
        "Just because you did <b><i><color=\"green\">okay</i></b></color> this time, remember not to get too confident.",
        "You're lucky the red box isn't labeled fragile. Might be the only reason you're competent at your job.",
        "I don't know why I have to praise you for doing the bare minimum, but it's required.\n<b><i><color=\"green\"><align=\"center\">Yay, you did it"//,
        //"Do we even know what's in that red box? It seems to glow sometimes. Don't move things too much, just |.\n<size=18><i>Please..."
    };
    string comment_bad;
    string comment_good;
    private void Awake()
    {
        Instance = this;
        nextLevel = transform.GetChild(2).GetComponent<Button>();
        redoLevel = transform.GetChild(3).GetComponent<Button>();
        clipboard = transform.GetChild(0).GetComponent<Image>();
        commentGO = new GameObject[] {
            transform.GetChild(4).gameObject,
            transform.GetChild(5).gameObject };
        nextLevel.onClick.AddListener(SceneTransitions.LoadNextLevel);
        nextLevel.gameObject.SetActive(false);
        redoLevel.onClick.AddListener(SceneTransitions.RestartLevel);
        redoLevel.gameObject.SetActive(false);
        
    }
    public void ShowWin(int stars)
    {
        Stars = stars;
        int lm = BoardManager.Instance.LowestPossibleMoves;
        comment_bad  = comments_bad [Random.Range(0, comments_bad .Count)].Replace("|", $"<b>{lm}</b> move{(lm > 1 ? "s" : "")}");
        comment_good = comments_good[Random.Range(0, comments_good.Count)].Replace("|", $"<b>{lm}</b> move{(lm > 1 ? "s" : "")}");
        StartCoroutine(WinAnimation());
    }
    private void Update()
    {
        if (!won) return;
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(1)) && won)
        {
            skip = true;
            StopCoroutine(WinAnimation());
            WinAnimationSkip();
        }
    }
    private IEnumerator WinAnimation()
    {
        won = true;
        if (Stars >= 1)
        {
            yield return new WaitForSecondsRealtime(skip ? 0f : 0.5f);
            clipboard.sprite = Clipboards[1];
        }
        if (Stars >= 2)
        {
            yield return new WaitForSecondsRealtime(skip ? 0f : 1f);
            clipboard.sprite = Clipboards[2];
        }
        if (Stars >= 3)
        {
            yield return new WaitForSecondsRealtime(skip ? 0f : 1.25f);
            clipboard.sprite = Clipboards[3];
        }
        yield return new WaitForSecondsRealtime(skip ? 0f : 0.75f);
        DisplayComments();
        if (SceneTransitions.nextLevelExists) nextLevel.gameObject.SetActive(true);
        redoLevel.gameObject.SetActive(true);
        StopCoroutine(WinAnimation());
    }
    private void WinAnimationSkip()
    {
        won = true;
        if (Stars >= 1) clipboard.sprite = Clipboards[Stars];
        DisplayComments();
        if (SceneTransitions.nextLevelExists) nextLevel.gameObject.SetActive(true);
        redoLevel.gameObject.SetActive(true);
    }
    private void DisplayComments()
    {
        bool b = Stars < 3;
        commentGO[b ? 0 : 1].SetActive(true);
        commentGO[b ? 0 : 1].GetComponentInChildren<TextMeshProUGUI>().text = b ? comment_bad : comment_good;
    }
}
