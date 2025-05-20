using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HelpNPC : Interactable
{
    public GameObject helpDialoguePanel;
    public TMP_Text questionText;

    public GameObject topicButtonContainer;
    public GameObject nextButton;

    public GameObject yesButtonObj;
    public GameObject noButtonObj;
    public GameObject bookButtonObj;
    public GameObject cookingButtonObj;
    public GameObject seasonButtonObj;
    public GameObject farmingButtonObj;
    public GameObject storeButtonObj;
    public GameObject wandererButtonObj;
    public GameObject saveButtonObj;

    private TMP_Text yesButtonText;
    private TMP_Text noButtonText;
    private TMP_Text bookButtonText;
    private TMP_Text cookingButtonText;
    private TMP_Text seasonButtonText;
    private TMP_Text farmingButtonText;
    private TMP_Text storeButtonText;
    private TMP_Text wandererButtonText;
    private TMP_Text saveButtonText;

    private Queue<string> textQueue = new Queue<string>();
    private bool isTalking = false;
    private bool isWaitingForNext = false;

    [TextArea(3, 10)]
    public string dialogueText = "�ñ��Ѱ� �־�?";

    void Start()
    {
        helpDialoguePanel.SetActive(false);
        topicButtonContainer.SetActive(false);

        yesButtonText = yesButtonObj.GetComponentInChildren<TMP_Text>();
        noButtonText = noButtonObj.GetComponentInChildren<TMP_Text>();
        bookButtonText = bookButtonObj.GetComponentInChildren<TMP_Text>();
        cookingButtonText = cookingButtonObj.GetComponentInChildren<TMP_Text>();
        seasonButtonText = seasonButtonObj.GetComponentInChildren<TMP_Text>();
        farmingButtonText = farmingButtonObj.GetComponentInChildren<TMP_Text>();
        storeButtonText = storeButtonObj.GetComponentInChildren<TMP_Text>();
        wandererButtonText = wandererButtonObj.GetComponentInChildren<TMP_Text>();
        saveButtonText = saveButtonObj.GetComponentInChildren<TMP_Text>();

        yesButtonObj.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(OnYesClicked);
        noButtonObj.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(OnNoClicked);
        bookButtonObj.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(OnBookClicked);
        cookingButtonObj.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(OnCookingClicked);
        seasonButtonObj.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(OnSeasonClicked);
        farmingButtonObj.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(OnFarmingClicked);
        storeButtonObj.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(OnStoreClicked);
        wandererButtonObj.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(OnWandererClicked);
        saveButtonObj.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(OnSaveClicked);

        nextButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(OnNextClicked);
        nextButton.SetActive(false);
    }

    public override void Interact(Character character)
    {
        if (isTalking) return;  // ��ȭ ���� ��Ŭ���ص� �����ϱ�

        isTalking = true;
        helpDialoguePanel.SetActive(true);
        questionText.text = dialogueText;

        yesButtonObj.gameObject.SetActive(true);
        noButtonObj.gameObject.SetActive(true);
        topicButtonContainer.SetActive(false);
    }

    void OnYesClicked()
    {
        questionText.text = "��� �ñ���?";
        yesButtonObj.gameObject.SetActive(false);
        noButtonObj.gameObject.SetActive(false);
        topicButtonContainer.SetActive(true);
    }

    void OnNoClicked()
    {
        helpDialoguePanel.SetActive(false);
    }

    void OnBookClicked()
    {
        topicButtonContainer.SetActive(false);
        TypeTexts("�κ��丮�� �ִ� å�� ��Ŭ���ϸ� ������ �� �� �־�");
    }

    void OnCookingClicked()
    {
        topicButtonContainer.SetActive(false);
        TypeTexts("�� ���� ����� Ŭ���ϸ�, �丮�밡 ����\n",
            "���ο��� ������ �����ǰ� �־�� �丮�� �� �����ϱ� ������");
    }

    void OnSeasonClicked()
    {
        topicButtonContainer.SetActive(false);
        TypeTexts("�� �� �ֱ�� 21���̰�, ������ ��, ����, ����, �ܿ��� �־�",
            "������ ���� ������ �𳯸���, �������� �� ���� ��\n",
            "�������� ������ ��������, �ܿ￡�� ���� ����\n",
            "��, �������� �� �������� �������ٴ� �� �����ž�!");
    }

    void OnFarmingClicked()
    {
        topicButtonContainer.SetActive(false);
        TypeTexts("������ �������׼� ��� �ǰ�,\n���̷� ���� ���ƾ� ������ ���� �� �־�\n",
            "�������� ���� �� �ִ� �۹��� �������ְ�,\n������ ���� �ʴ� �۹��� ���� �� ����\n",
            "��, �۹��� �� �����Ͽ��µ��� ��Ʋ�� ��Ȯ���� ������ �۹��� �����\n",
            "�۹����� ���� �ð��� �ٸ��ϱ� ������!\n","����� ���� �Ϸ翡 �� ���� �ָ� ��");
    }

    void OnStoreClicked()
    {
        topicButtonContainer.SetActive(false);
        TypeTexts("����");
    }

    void OnWandererClicked()
    {
        topicButtonContainer.SetActive(false);
        TypeTexts("������ ����");
    }

    void OnSaveClicked()
    {
        topicButtonContainer.SetActive(false);
        TypeTexts("���� �� �ҷ�����");
    }

    private Coroutine typingCoroutine;

    void TypeTexts(params string[] texts)
    {
        textQueue.Clear();
        foreach (string t in texts)
            textQueue.Enqueue(t);

        nextButton.SetActive(false);
        ShowNextText();
    }

    void ShowNextText()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        if (textQueue.Count > 0)
        {
            string nextText = textQueue.Dequeue();
            typingCoroutine = StartCoroutine(TypeTextCoroutine(nextText));
        }
        else
        {
            typingCoroutine = StartCoroutine(TypeTextCoroutine("�ñ��� �� ������ �� �����!", false));
            StartCoroutine(CloseAfterDelay(2f));
        }
    }

    IEnumerator TypeTextCoroutine(string text, bool showNextButton = true)
    {
        questionText.text = "";
        foreach (char c in text)
        {
            questionText.text += c;
            yield return new WaitForSeconds(0.05f);
        }

        nextButton.SetActive(showNextButton);
        isWaitingForNext = showNextButton;
    }

    IEnumerator CloseAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        helpDialoguePanel.SetActive(false);
        isTalking = false;  // ��ȭ ����
    }

    void OnNextClicked()
    {
        if (isWaitingForNext)
        {
            isWaitingForNext = false;
            nextButton.SetActive(false);
            ShowNextText();
        }
    }
}
