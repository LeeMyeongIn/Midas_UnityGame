using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HelpNPC : Interactable
{
    public GameObject helpDialoguePanel;
    public TMP_Text questionText;

    public GameObject topicButtonContainer;

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

    [TextArea(3, 10)]
    public string dialogueText = "May I help you?";

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
    }

    public override void Interact(Character character)
    {
        helpDialoguePanel.SetActive(true);
        questionText.text = dialogueText;

        yesButtonObj.gameObject.SetActive(true);
        noButtonObj.gameObject.SetActive(true);
        topicButtonContainer.SetActive(false);
    }

    void OnYesClicked()
    {
        questionText.text = "what?";
        yesButtonObj.gameObject.SetActive(false);
        noButtonObj.gameObject.SetActive(false);
        topicButtonContainer.SetActive(true);
        Debug.Log("응 버튼 눌림");
    }

    void OnNoClicked()
    {
        Debug.Log("아니 버튼 눌림");
        helpDialoguePanel.SetActive(false);
    }

    void OnBookClicked()
    {
        Debug.Log("도감?");
    }

    void OnCookingClicked()
    {
        Debug.Log("요리?");
    }

    void OnSeasonClicked()
    {
        Debug.Log("계절?");
    }

    void OnFarmingClicked()
    {
        Debug.Log("농사?");
    }

    void OnStoreClicked()
    {
        Debug.Log("상인?");
    }

    void OnWandererClicked()
    {
        Debug.Log("떠돌이 상인?");
    }

    void OnSaveClicked()
    {
        Debug.Log("저장?");
    }
}
