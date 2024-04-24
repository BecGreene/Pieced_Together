using UnityEngine;
using UnityEngine.UI;
using ST = SceneTransitions;

public class LevelSelect : MonoBehaviour
{
    private Button GetButton(int i) => transform.GetChild(i).GetComponent<Button>();
    private void LoadButton(int i)
    {
        GetButton(i).onClick.AddListener(delegate { ST.LoadNumLevel(i + 4); });
        GetButton(i).interactable = ST.unlockedLevels[i];
    }
    private void Start()
    {
        for(int i = 0; i < 6; ++i)
        {
            LoadButton(i);
        }
        //GetButton(5).interactable = ST.unlockedLevels[5];
        //transform.GetChild(5).GetComponent<Button>().onClick.AddListener(delegate { SceneTransitions.LoadNumLevel(1); });
        GetButton(6).onClick.AddListener(ST.MainMenu);
    }
}
