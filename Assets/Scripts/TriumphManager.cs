using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class TriumphManager : MonoBehaviour
{
    public static TriumphManager Instance;

    [Header("업적 리스트")]
    public List<TriumphSO> triumphList;

    public delegate void OnTriumphUpdated();
    public event OnTriumphUpdated onTriumphUpdated;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void UpdateCropTypeAchievements()
    {
        int uniqueCropCount = CropSeenManager.Instance.GetSeenCropCount();

        foreach (var triumph in triumphList)
        {
            if (triumph.data.type == TriumphType.CropHarvest)
            {
                if (!triumph.data.isCompleted)
                {
                    triumph.data.currentCount = Mathf.Min(uniqueCropCount, triumph.data.targetCount);

                    if (triumph.data.currentCount >= triumph.data.targetCount)
                    {
                        triumph.data.currentCount = triumph.data.targetCount;
                        triumph.data.isCompleted = true;
                    }

                    onTriumphUpdated?.Invoke();
                }
            }
        }
    }

    public void UpdateProgressByType(TriumphType type, int amount = 1)
    {
        foreach (var triumph in triumphList)
        {
            if (triumph.data.type == type && !triumph.data.isCompleted)
            {
                triumph.data.currentCount += amount;

                if (triumph.data.currentCount >= triumph.data.targetCount)
                {
                    triumph.data.currentCount = triumph.data.targetCount;
                    triumph.data.isCompleted = true;
                }

                onTriumphUpdated?.Invoke();
            }
        }
    }

    public bool CanClaimReward(TriumphData triumph)
    {
        return triumph.isCompleted && !triumph.isRewardClaimed &&
               InventoryController.Instance.HasSpace(triumph.rewardItems);
    }

    public void ClaimReward(TriumphData triumph)
    {
        if (!CanClaimReward(triumph))
        {
            Debug.LogWarning("보상을 받을 수 없습니다.");
            return;
        }

        foreach (var item in triumph.rewardItems)
        {
            InventoryController.Instance.AddItem(item);
        }

        triumph.isRewardClaimed = true;
        onTriumphUpdated?.Invoke();
    }

    public bool HasAnyClaimableTriumph()
    {
        foreach (var triumph in triumphList)
        {
            if (CanClaimReward(triumph.data))
                return true;
        }
        return false;
    }

    public bool AreAllRewardsClaimed()
    {
        foreach (var triumph in triumphList)
        {
            if (!triumph.data.isCompleted || !triumph.data.isRewardClaimed)
            {
                return false;
            }
        }
        return true;
    }

    public int GetClaimedRewardCount()
    {
        int count = 0;
        foreach (var triumph in triumphList)
        {
            if (triumph.data.isRewardClaimed)
                count++;
        }
        return count;
    }

    public int GetTotalRewardCount()
    {
        return triumphList.Count;
    }

    public int GetTotalRewardableTriumphCount()
    {
        int count = 0;
        foreach (var triumph in triumphList)
        {
            if (triumph.data.rewardItems != null && triumph.data.rewardItems.Count > 0)
                count++;
        }
        return count;
    }

    public void ResetAllTriumphs()
    {
        foreach (var triumph in triumphList)
        {
            triumph.data.currentCount = 0;
            triumph.data.isCompleted = false;
            triumph.data.isRewardClaimed = false;
        }

        Debug.Log("[업적] 모든 업적이 초기화되었습니다.");
        onTriumphUpdated?.Invoke();
    }

#if UNITY_EDITOR
    [ContextMenu("Reset All Triumphs")]
    private void ResetFromEditor()
    {
        ResetAllTriumphs();
    }
#endif
}
