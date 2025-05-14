using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

[System.Serializable]
public class CutSceneElement
{
    public Sprite image;
    [TextArea] public string text;
    public bool showClock;
}
public class CutSceneManager : MonoBehaviour
{
    public Image cutsceneImage;
    public TextMeshProUGUI cutsceneText;
    public GameObject clockUI;
    public List<CutSceneElement> cutscenes;
    
    private int index = 0;

    void Start()
    {
        ShowCut(index);
    }

    public void OnNext()
    {
        index++;
        if (index >= cutscenes.Count)
        {
            SceneManager.LoadScene("MainMenuScene");
            return;
        }
        ShowCut(index);
    }

    IEnumerator TypeText(string fullText)
    {
        cutsceneText.text = "";
        foreach (char c in fullText)
        {
            cutsceneText.text += c;
            yield return new WaitForSeconds(0.1f);  // 타이핑 속도
        }
    }

    void ShowCut(int i)
    {
        StopAllCoroutines();
        var cut = cutscenes[i];
        cutsceneImage.sprite = cut.image;
        clockUI.SetActive(cut.showClock);
        StartCoroutine(TypeText(cut.text));
    }
}
