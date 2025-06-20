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
        string dialogue = "�ƽ��Ե� �� �ϼ����� ���ϼ̱���.. ���� ���� �˾Ƽ� �����ϰڽ��ϴ�. �������� �� �ϼ��ϱ� �ٶ��ϴ�! �ȳ��� ������";
        
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