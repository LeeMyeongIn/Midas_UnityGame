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
        Debug.Log($"[업적 갱신] 현재 고유 작물 수: {uniqueCropCount}");

        foreach (var triumph in triumphList)
        {
            if (triumph.data.type == TriumphType.CropHarvest)
            {
                Debug.Log($"[업적 검사] {triumph.data.name} - 목표: {triumph.data.targetCount}, 현재: {triumph.data.currentCount}, 완료됨: {triumph.data.isCompleted}");

                if (!triumph.data.isCompleted)
                {
                    triumph.data.currentCount = Mathf.Min(uniqueCropCount, triumph.data.targetCount);

                    if (triumph.data.currentCount >= triumph.data.targetCount)
                    {
                        triumph.data.isCompleted = true;
                        Debug.Log($"[업적 달성] 작물 종류 누적 업적 달성: {triumph.data.name}");
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

                Debug.Log($"[업적] 진행됨: {triumph.data.name} → {triumph.data.currentCount}/{triumph.data.targetCount}");
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
        Debug.Log($"업적 보상 수령: {triumph.name}");
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
    [ContextMenu("Reset All Triumphs (테스트용)")]
    private void ResetFromEditor()
    {
        ResetAllTriumphs();
    }
#endif
}