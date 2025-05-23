using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TriumphSlotUI : MonoBehaviour
{
    [Header("UI ¿ä¼Ò")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI progressText;
    public Button claimButton;
    public GameObject exclamationMark;

    private TriumphData currentData;

    public void Initialize(TriumphData data)
    {
        currentData = data;

        titleText.text = data.name;
        descriptionText.text = data.description;
        progressText.text = $"{data.currentCount} / {data.targetCount}";

        bool canClaim = TriumphManager.Instance.CanClaimReward(data);
        bool rewardDone = data.isRewardClaimed;

        claimButton.gameObject.SetActive(!rewardDone && data.isCompleted);
        exclamationMark.SetActive(canClaim);

        claimButton.onClick.RemoveAllListeners();
        claimButton.onClick.AddListener(() => {
            TriumphManager.Instance.ClaimReward(data);
            UpdateUI();
        });
    }



    public void UpdateUI()
    {
        Initialize(currentData);
    }
}
