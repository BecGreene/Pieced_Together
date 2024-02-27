using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    //I was being lazy with these two variables. There
    //is probably a better way to do this, just didn't
    //feel like doing that
    public GameObject WinScreen;
    public TextMeshProUGUI WinText;
    private int Moves = 0;
    private int DamagedBoxes = 0;
    public TextMeshProUGUI MovesText;
    void Awake()
    {
        Instance = this;
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
        MovesText.text = $"Moves: {Instance.Moves}\n{Stars} out of 3 stars";
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
        WinText.text = $"You won in {Moves} moves!\nThis board was possible in {LowestPossibleMoves}\n";
        WinText.text += $"You got {Stars} out of 3 stars!";
        WinScreen.SetActive(true);
    }
}
