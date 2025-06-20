using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System;

public class HappyEndingManager : MonoBehaviour
{
    public TextMeshProUGUI dialogueText;
    public Image CreditPanel;
    public TextMeshProUGUI creditText;
    public GameObject continueButton;
    public GameObject endGameButton;

    void Start()
    {       
        continueButton.SetActive(false);
        endGameButton.SetActive(false);
        creditText.gameObject.SetActive(false);

        StartCoroutine(TypeDialogue("3�� �ȿ� ���� �� ���ٴ� ���� ����ؿ�! ���� ������ ���̿���. ���� ��Ȱ�� �� ��� �� �ִµ� ��Ͻðھ��?", true));
    }

    public void OnContinueFarming()
    {
        continueButton.SetActive(false);
        endGameButton.SetActive(false);

        StartCoroutine(TypeDialogue("�����ο� ���� ��Ȱ�� ���ñ� �ٷ���!", false));

        Invoke("GoToFarmScene", 1f);
    }

    public void OnEndGame()
    {
        continueButton.SetActive(false);
        endGameButton.SetActive(false);

        StartCoroutine(TypeDialogue("���� ��Ȱ�� ��հ� ���̳���? ������ �� ��ⷯ �Ϳ�!", false));

        StartCoroutine(ShowCredit());
    }

    IEnumerator ShowCredit()
    {
        yield return new WaitForSeconds(2f);

        float fadeDuration = 3f;
        float t = 0f;
        Color panelColor = CreditPanel.color;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            panelColor.a = Mathf.Lerp(0f, 1f, t / fadeDuration);
            CreditPanel.color = panelColor;
            yield return null;
        }

        creditText.gameObject.SetActive(true);

        yield return new WaitForSeconds(3f);

        SceneManager.LoadScene("MainMenuScene");
    }

    void GoToFarmScene()
    {
        SceneManager.LoadScene("FarmingScene");
        SceneManager.LoadScene("Essential", LoadSceneMode.Additive);
        SceneManager.LoadScene("HelpScene", LoadSceneMode.Additive);
    }

    IEnumerator TypeDialogue(string fullText, bool showButtons = false, float typingSpeed = 0.08f)
    {
        dialogueText.text = "";
        foreach (char c in fullText)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        if (showButtons)
        {
            continueButton.SetActive(true);
            endGameButton.SetActive(true);
        }
    }
}