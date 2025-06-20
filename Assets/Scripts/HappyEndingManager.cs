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

        StartCoroutine(TypeDialogue("3년 안에 빚을 다 갚다니 정말 대단해요! 이제 완전한 집이예요. 농촌 생활을 더 즐길 수 있는데 어떡하시겠어요?", true));
    }

    public void OnContinueFarming()
    {
        continueButton.SetActive(false);
        endGameButton.SetActive(false);

        StartCoroutine(TypeDialogue("여유로운 농촌 생활을 즐기시길 바래요!", false));

        Invoke("GoToFarmScene", 1f);
    }

    public void OnEndGame()
    {
        continueButton.SetActive(false);
        endGameButton.SetActive(false);

        StartCoroutine(TypeDialogue("농촌 생활을 재밌게 즐기셨나요? 다음에 또 즐기러 와요!", false));

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