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
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
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

    // �����
    public List<TriumphProgress> GetUnlockedTriumphIds()
    {
        List<TriumphProgress> list = new List<TriumphProgress>();
        
        foreach (var triumph in triumphList)
        {
            list.Add(new TriumphProgress
            {
                triumphId = triumph.data.name,
                currentCount = triumph.data.currentCount,
                isCompleted = triumph.data.isCompleted,
                isRewardClaimed = triumph.data.isRewardClaimed
            });
        }

        return list;
    }

    // �ҷ������
    public void SetUnlockedTriumphs(List<TriumphProgress> savedList)
    {
        foreach (var progress in savedList)
        {
            var triumph = triumphList.Find(triumph => triumph.data.name == progress.triumphId);
            if (triumph != null)
            {
                triumph.data.currentCount = progress.currentCount;
                triumph.data.isCompleted = progress.isCompleted;
                triumph.data.isRewardClaimed = progress.isRewardClaimed;
            }
        }

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

[System.Serializable]
public class TriumphSaveData
{
    public List<TriumphProgress> progressList = new List<TriumphProgress>();
}

[System.Serializable]
public class TriumphProgress
{
    public string triumphId;
    public int currentCount;
    public bool isCompleted;
    public bool isRewardClaimed;
}