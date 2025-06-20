using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class BadEndingManager : MonoBehaviour
{
    public TextMeshProUGUI dialogueText;

    void Start()
    {
        StartCoroutine(PlayBadEnding());
    }

    IEnumerator PlayBadEnding()
    {
        string dialogue = "아쉽게도 다 완성하지 못하셨군요.. 집은 저희가 알아서 정리하겠습니다. 다음에는 꼭 완성하길 바랍니다! 안녕히 가세요";
        
        yield return StartCoroutine(TypeDialogue(dialogue, 0.08f));
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("MainMenuScene");
    }

    IEnumerator TypeDialogue(string fullText, float typingSpeed)
    {
        dialogueText.text = "";
        foreach (char c in fullText)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }
    }
}