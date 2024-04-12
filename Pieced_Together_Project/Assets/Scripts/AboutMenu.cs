using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AboutMenu : MonoBehaviour
{
    void Start()
    {
        transform.GetChild(1).GetComponent<Button>().onClick.AddListener(SceneTransitions.MainMenu);
    }
}
