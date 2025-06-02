using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EndingPanel : MonoBehaviour
{
    public static EndingPanel Instance;

    [Header("UI Elements")]
    public TextMeshProUGUI questionText;
    public Button yesButton;
    public Button noButton;
    public Button conditionButton;

    [Header("진행도 표시 UI")]
    public GameObject progressPanel;
    public TextMeshProUGUI progressText;
    public Button conditionCloseButton;

    [SerializeField] private GameObject defaultPanel;

    private Action onConfirm;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        gameObject.SetActive(false);
        progressPanel?.SetActive(false);
    }

    private void Start()
    {
        conditionCloseButton.onClick.AddListener(() =>
        {
            progressPanel.SetActive(false);
            defaultPanel.SetActive(true);
        });
    }

    public void ShowConfirmation(Action confirmAction)
    {
        gameObject.SetActive(true);
        onConfirm = confirmAction;

        defaultPanel.SetActive(true);
        progressPanel.SetActive(false);

        yesButton.onClick.RemoveAllListeners();
        noButton.onClick.RemoveAllListeners();
        conditionButton.onClick.RemoveAllListeners();

        yesButton.onClick.AddListener(() =>
        {
            if (EndingConditionChecker.Instance.IsEndingAvailable())
            {
                onConfirm?.Invoke();
                gameObject.SetActive(false);
            }
            else
            {
                string failMsg = EndingConditionChecker.Instance.GetMissingConditionMessage();

                defaultPanel.SetActive(false);
                progressText.text = failMsg;
                progressPanel.SetActive(true);
            }
        });

        noButton.onClick.AddListener(() =>
        {
            gameObject.SetActive(false);
        });

        conditionButton.onClick.AddListener(() =>
        {
            ShowProgress();
        });
    }

    private void ShowProgress()
    {
        int cooked = RecipeUnlockManager.Instance?.GetCookedCount() ?? 0;
        int totalRecipes = RecipeUnlockManager.Instance?.GetTotalRecipeCount() ?? 0;

        int seenCrops = CropSeenManager.Instance?.GetSeenCropCount() ?? 0;
        int totalCrops = CropSeenManager.Instance?.GetTotalCropCount() ?? 0;

        int claimed = TriumphManager.Instance?.GetClaimedRewardCount() ?? 0;
        int totalTriumphs = TriumphManager.Instance?.GetTotalRewardableTriumphCount() ?? 0;

        int houseLevel = HouseUpgradeController.Instance?.GetCurrentLevel() ?? 0;

        string progressReport = $"요리도감: {cooked}/{totalRecipes}\n" +
                                $"작물도감: {seenCrops}/{totalCrops}\n" +
                                $"업적 보상: {claimed}/{totalTriumphs}\n" +
                                $"집 업그레이드: {(houseLevel >= 3 ? 1 : 0)}/1";

        defaultPanel.SetActive(false);
        progressText.text = progressReport;
        progressPanel.SetActive(true);
    }
}
