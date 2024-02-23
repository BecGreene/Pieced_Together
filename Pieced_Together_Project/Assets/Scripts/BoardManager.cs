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
    public static BoardManager Instance;

    //I was being lazy with these two variables. There
    //is probably a better way to do this, just didn't
    //feel like doing that
    public GameObject WinScreen;
    public TextMeshProUGUI WinText;
    private int Moves = 0;
    public TextMeshProUGUI MovesText;
    void Awake()
    {
        Instance = this;
    }
    public static void UpdateMoves()
    {
        Instance.Moves++;
        Instance.MovesText.text = $"Moves: {Instance.Moves}";
    }

    public void DisplayWin()
    {
        WinText.text = $"You won in {Moves} moves!";
        WinScreen.SetActive(true);
    }
}
