using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class BoardManager : MonoBehaviour
{
    [SerializeField]
    private int width;
    public int Width { get => width; }
    [SerializeField]
    private int height;
    public int Height { get => height; }
    public int LowestPossibleMoves = 0;
    public int ThreeStarMoveCap = 0;
    public int TwoStarMoveCap = 0;
    public int OneStarMoveCap = 0;
    public int DamagedBoxesCap = 0;
    private int Stars = 3;
    public static BoardManager Instance;

    public Sprite[] Clipboards;
    //I was being lazy with these two variables. There
    //is probably a better way to do this, just didn't
    //feel like doing that
    public GameObject WinScreen;
    public TextMeshProUGUI WinText;
    private int Moves = 0;
    private int DamagedBoxes = 0;
    public TextMeshProUGUI MovesText;
    public static bool Won = false;
    private Image clipboard;
    private Button nextLevel;
    void Awake()
    {
        Instance = this;
        nextLevel = WinScreen.transform.GetChild(2).GetComponent<Button>();
        clipboard = WinScreen.transform.GetChild(0).GetComponent<Image>();
        nextLevel.onClick.AddListener(SceneTransitions.LoadNextLevel);
    }
    public static void UpdateMoves() => Instance.UpdateMoves_P();
    private void UpdateMoves_P()
    {
        Moves++;
        if(Moves >= OneStarMoveCap)
        {
            if (Stars >= 1) Stars--;
        }
        else if(Moves >= TwoStarMoveCap)
        {
            if (Stars >= 2) Stars--;
        }
        else if (Moves >= ThreeStarMoveCap)
        {
            if (Stars >= 3) Stars--;
        }
        MovesText.text = $"Moves: {Instance.Moves}";//\n{Stars} out of 3 stars";
    }
    public static void UpdateDamaged() => Instance.UpdateDamaged_P();
    private void UpdateDamaged_P()
    {
        DamagedBoxes++;
        if(DamagedBoxes >= DamagedBoxesCap && Stars >= 1)
        {
            Stars--;
        }
    }

    public void DisplayWin()
    {
        Won = true;
        //WinText.text = $"You won in {Moves} moves!\nThis board was possible in {LowestPossibleMoves}\n";
        //WinText.text += $"You got {Stars} out of 3 stars!";
        WinText.text = Moves.ToString();
        WinScreen.SetActive(true);
        StartCoroutine(WinAnimation());
    }
    private IEnumerator WinAnimation()
    {
        if (Stars >= 1) yield return new WaitForSecondsRealtime(0.5f);
        clipboard.sprite = Clipboards[1];
        if (Stars >= 2) yield return new WaitForSecondsRealtime(1f);
        clipboard.sprite = Clipboards[2];
        if (Stars >= 3) yield return new WaitForSecondsRealtime(1.25f);
        clipboard.sprite = Clipboards[3];
        yield return new WaitForSecondsRealtime(0.75f);
        if (SceneTransitions.nextLevelExists) nextLevel.gameObject.SetActive(true);
        StopCoroutine(WinAnimation());
    }
}
