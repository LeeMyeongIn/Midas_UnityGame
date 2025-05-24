using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class TriumphManager : MonoBehaviour
{
    public static TriumphManager Instance;

    [Header("���� ����Ʈ")]
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
        Debug.Log($"[���� ����] ���� ���� �۹� ��: {uniqueCropCount}");

        foreach (var triumph in triumphList)
        {
            if (triumph.data.type == TriumphType.CropHarvest)
            {
                Debug.Log($"[���� �˻�] {triumph.data.name} - ��ǥ: {triumph.data.targetCount}, ����: {triumph.data.currentCount}, �Ϸ��: {triumph.data.isCompleted}");

                if (!triumph.data.isCompleted)
                {
                    triumph.data.currentCount = Mathf.Min(uniqueCropCount, triumph.data.targetCount);

                    if (triumph.data.currentCount >= triumph.data.targetCount)
                    {
                        triumph.data.isCompleted = true;
                        Debug.Log($"[���� �޼�] �۹� ���� ���� ���� �޼�: {triumph.data.name}");
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

                Debug.Log($"[����] �����: {triumph.data.name} �� {triumph.data.currentCount}/{triumph.data.targetCount}");
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
            Debug.LogWarning("������ ���� �� �����ϴ�.");
            return;
        }

        foreach (var item in triumph.rewardItems)
        {
            InventoryController.Instance.AddItem(item);
        }

        triumph.isRewardClaimed = true;
        Debug.Log($"���� ���� ����: {triumph.name}");
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

        Debug.Log("[����] ��� ������ �ʱ�ȭ�Ǿ����ϴ�.");
        onTriumphUpdated?.Invoke();
    }

#if UNITY_EDITOR
    [ContextMenu("Reset All Triumphs (�׽�Ʈ��)")]
    private void ResetFromEditor()
    {
        ResetAllTriumphs();
    }
#endif
}