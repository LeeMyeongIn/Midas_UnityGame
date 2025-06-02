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
    public GameObject endingButtonObj;

    private TMP_Text yesButtonText;
    private TMP_Text noButtonText;
    private TMP_Text bookButtonText;
    private TMP_Text cookingButtonText;
    private TMP_Text seasonButtonText;
    private TMP_Text farmingButtonText;
    private TMP_Text storeButtonText;
    private TMP_Text wandererButtonText;
    private TMP_Text saveButtonText;
    private TMP_Text endingButtonText;

    private Queue<string> textQueue = new Queue<string>();
    private bool isTalking = false;
    private bool isWaitingForNext = false;

    [TextArea(3, 10)]
    public string dialogueText = "궁금한거 있어?";

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
        endingButtonText = endingButtonObj.GetComponentInChildren<TMP_Text>();

        yesButtonObj.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(OnYesClicked);
        noButtonObj.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(OnNoClicked);
        bookButtonObj.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(OnBookClicked);
        cookingButtonObj.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(OnCookingClicked);
        seasonButtonObj.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(OnSeasonClicked);
        farmingButtonObj.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(OnFarmingClicked);
        storeButtonObj.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(OnStoreClicked);
        wandererButtonObj.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(OnWandererClicked);
        saveButtonObj.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(OnSaveClicked);
        endingButtonObj.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(OnEndingClicked);

        nextButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(OnNextClicked);
        nextButton.SetActive(false);
    }

    public override void Interact(Character character)
    {
        if (isTalking) return;  // 대화 도중 우클릭해도 무시하기

        isTalking = true;
        helpDialoguePanel.SetActive(true);
        questionText.text = dialogueText;

        yesButtonObj.gameObject.SetActive(true);
        noButtonObj.gameObject.SetActive(true);
        topicButtonContainer.SetActive(false);
    }

    void OnYesClicked()
    {
        questionText.text = "어떤게 궁금해?";
        yesButtonObj.gameObject.SetActive(false);
        noButtonObj.gameObject.SetActive(false);
        topicButtonContainer.SetActive(true);
    }

    void OnNoClicked()
    {
        helpDialoguePanel.SetActive(false);
        isTalking = false;
    }

    void OnBookClicked()
    {
        topicButtonContainer.SetActive(false);
        TypeTexts("인벤토리에 있는 책을 우클릭하면 도감을 열 수 있어");
    }

    void OnCookingClicked()
    {
        topicButtonContainer.SetActive(false);
        TypeTexts("집 안의 냉장고를 클릭하면, 요리대가 열려\n",
            "상인에게 구매한 레시피가 있어야 요리할 수 있으니깐 주의해");
    }

    void OnSeasonClicked()
    {
        topicButtonContainer.SetActive(false);
        TypeTexts("한 달 주기는 21일이고, 계절은 봄, 여름, 가을, 겨울이 있어",
            "봄에는 예쁜 꽃잎이 흩날리고, 여름에는 비가 많이 와\n",
            "가을에는 낙엽이 떨어지고, 겨울에는 눈이 내려\n",
            "봄, 가을에도 비가 내리지만 여름보다는 덜 내릴거야!");
    }

    void OnFarmingClicked()
    {
        topicButtonContainer.SetActive(false);
        TypeTexts("씨앗은 상인한테서 사면 되고,\n괭이로 밭을 갈아야 씨앗을 심을 수 있어\n",
            "계절별로 심을 수 있는 작물이 정해져있고,\n계절과 맞지 않는 작물은 심을 수 없어\n",
            "또, 작물이 다 성장하였는데도 이틀간 수확하지 않으면 작물이 사라져\n",
            "작물마다 성장 시간이 다르니깐 조심해!\n","참고로 물은 하루에 한 번만 주면 돼");
    }

    void OnStoreClicked()
    {
        topicButtonContainer.SetActive(false);
        TypeTexts("마을에 있는 상인에게 다가가 우클릭하면 상점 패널이 열릴거야.\n",
            "상점에 있는 물품에 마우스 커서를 이동시키면 해당 물품의 이름, 설명, 상점 구매가, 판매가가 적힌 툴팁이 나타날거야.\n",
            "사고싶은 물품을 클릭하면 해당 물품을 살 수 있어.\n",
            "너의 인벤토리에 있는 아이템을 클릭해서 상점 패널에 두면, 해당 아이템을 판매할 수 있어.");
    }

    void OnWandererClicked()
    {
        topicButtonContainer.SetActive(false);
        TypeTexts("상인에게서 [떠돌이 상인 주문서]를 구매하면 떠돌이 상인이 나타날거야.\n",
            "이 주문서를 사용하면, 떠돌이 상인은 일정 시간동안 마을에 나타났다가 사라지고,\n",
            "한 번 사용하면 사라지는 일회용 아이템이니깐 주의해!\n",
            "떠돌이 상인의 상점 패널에서는 일반 상인이 판매하지 않는 고유한 아이템들과 게임의 업적 시스템을 달성하기 위해 반드시 구매해야 하는 레시피를 판매해.\n",
            "일반 상인보다 싸게 살 수 있고, 비싸게 판매할 수 있으니깐 좋은 아이템이지?",
            "그렇지만, 한 번 사용 후, 일정 시간이 지나야 다시 사용할 수 있으니깐 주의해!");
    }

    void OnSaveClicked()
    {
        topicButtonContainer.SetActive(false);
        TypeTexts("침대에 가까이 가면 저장을 할 수 있어.\n",
            "잠자면 다음 날로 넘어가면서 저장이 되고, 저장만 할 수도 있어.\n",
            "참고로, 새벽 2시가 되면 자동으로 침대로 가서 잠자니깐 주의해!");
    }

    void OnEndingClicked()
    {
        topicButtonContainer.SetActive(false);
        TypeTexts("3년 안에 대출을 다 갚고, 요리도감, 작물도감, 집 3단계 업그레이드를 다 하게되면 이 게임의 이야기가 끝나게 돼. 하지만, 너가 더 플레이 하고싶다면 이어서 할 수 있어!\n",
            "만약, 이 4가지의 조건을 충족하지 못하면, 엔딩이 달라질테니 열심히 하길 바래!\n",
            "빠르게 엔딩을 맞고싶다면 맵 우측에 울타리를 우클릭 하도록 해.");
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
            typingCoroutine = StartCoroutine(TypeTextCoroutine("궁금한 거 있으면 또 물어봐!", false));
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
        isTalking = false;  // 대화 종료
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
