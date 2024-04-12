using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelect : MonoBehaviour
{
    private void Start()
    {
        transform.GetChild(0).GetComponent<Button>().onClick.AddListener(delegate { SceneTransitions.LoadNumLevel(1); });
        transform.GetChild(1).GetComponent<Button>().onClick.AddListener(delegate { SceneTransitions.LoadNumLevel(2); });
        transform.GetChild(2).GetComponent<Button>().onClick.AddListener(delegate { SceneTransitions.LoadNumLevel(3); });
        transform.GetChild(3).GetComponent<Button>().onClick.AddListener(delegate { SceneTransitions.LoadNumLevel(4); });
        transform.GetChild(4).GetComponent<Button>().onClick.AddListener(delegate { SceneTransitions.LoadNumLevel(5); });
        //transform.GetChild(5).GetComponent<Button>().onClick.AddListener(delegate { SceneTransitions.LoadNumLevel(1); });
        transform.GetChild(6).GetComponent<Button>().onClick.AddListener(SceneTransitions.MainMenu);
    }
}
