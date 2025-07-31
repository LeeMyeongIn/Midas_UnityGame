using System.Collections.Generic;
using System.IO;
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

    private int currentSlot = -1;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            CreateAllSlotFiles();
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void OnApplicationQuit()
    {
        SaveTriumphs();
    }

    public void CreateAllSlotFiles()
    {
        for (int slot = 0; slot <= 2; slot++)
        {
            string path = Path.Combine(Application.persistentDataPath, $"triumphs_slot{slot}.json");

            if (!File.Exists(path))
            {
                TriumphSaveData emptyData = new TriumphSaveData();
                foreach (var triumph in triumphList)
                {
                    emptyData.entries.Add(new TriumphSaveEntry
                    {
                        id = triumph.data.id,
                        currentCount = 0,
                        isCompleted = false,
                        isRewardClaimed = false
                    });
                }

                string json = JsonUtility.ToJson(emptyData, true);
                File.WriteAllText(path, json);
                Debug.Log($"[업적] 슬롯 {slot} 초기 JSON 생성 완료: {path}");
            }
        }
    }

    public void SetSaveSlot(int slot)
    {
        currentSlot = Mathf.Clamp(slot, 0, 2);
        Debug.Log($"[업적] 현재 저장 슬롯: {currentSlot}");

        string path = GetSavePath();
        if (!File.Exists(path))
        {
            Debug.Log($"[업적] 저장 파일 없음. 새로 생성합니다: {path}");
            SaveTriumphs(); // JSON 생성
        }

        LoadTriumphs(); // 슬롯 데이터 불러오기
    }

    private string GetSavePath()
    {
        return Path.Combine(Application.persistentDataPath, $"triumphs_slot{currentSlot}.json");
    }

    public void SaveTriumphs()
    {
        TriumphSaveData saveData = new TriumphSaveData();

        foreach (var triumph in triumphList)
        {
            TriumphData data = triumph.data;
            TriumphSaveEntry entry = new TriumphSaveEntry
            {
                id = data.id,
                currentCount = data.currentCount,
                isCompleted = data.isCompleted,
                isRewardClaimed = data.isRewardClaimed
            };
            saveData.entries.Add(entry);
        }

        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(GetSavePath(), json);
        Debug.Log($"[업적] 슬롯 {currentSlot} 저장 완료: {GetSavePath()}");
    }

    public void LoadTriumphs()
    {
        string path = GetSavePath();

        if (!File.Exists(path))
        {
            Debug.Log($"[업적] 슬롯 {currentSlot} 저장 파일 없음. 새로 시작합니다.");
            return;
        }

        string json = File.ReadAllText(path);
        TriumphSaveData saveData = JsonUtility.FromJson<TriumphSaveData>(json);

        foreach (var entry in saveData.entries)
        {
            TriumphSO so = triumphList.Find(t => t.data.id == entry.id);
            if (so != null)
            {
                so.data.currentCount = entry.currentCount;
                so.data.isCompleted = entry.isCompleted;
                so.data.isRewardClaimed = entry.isRewardClaimed;
            }
        }

        onTriumphUpdated?.Invoke();
        Debug.Log($"[업적] 슬롯 {currentSlot} 불러오기 완료");
    }

    public void UpdateCropTypeAchievements()
    {
        int uniqueCropCount = CropSeenManager.Instance.GetSeenCropCount();

        foreach (var triumph in triumphList)
        {
            if (triumph.data.type == TriumphType.CropHarvest && !triumph.data.isCompleted)
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

    public void UpdateMonsterCodexAchievements()
    {
        int monsterCount = MonsterUnlockManager.Instance.GetSeenMonsterCount();

        foreach (var triumph in triumphList)
        {
            if (triumph.data.type == TriumphType.Monster && !triumph.data.isCompleted)
            {
                triumph.data.currentCount = Mathf.Min(monsterCount, triumph.data.targetCount);

                if (triumph.data.currentCount >= triumph.data.targetCount)
                {
                    triumph.data.currentCount = triumph.data.targetCount;
                    triumph.data.isCompleted = true;
                }

                onTriumphUpdated?.Invoke();
            }
        }
    }

    public void UpdateDropItemCodexAchievements()
    {
        int dropCount = MonsterDropCodexTracker.Instance.GetSeenDropCount();

        foreach (var triumph in triumphList)
        {
            if (triumph.data.type == TriumphType.DropItem && !triumph.data.isCompleted)
            {
                triumph.data.currentCount = Mathf.Min(dropCount, triumph.data.targetCount);

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
                return false;
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
    private void ResetFromEditor() => ResetAllTriumphs();

    [ContextMenu("Save Triumphs")]
    private void SaveFromEditor() => SaveTriumphs();

    [ContextMenu("Load Triumphs")]
    private void LoadFromEditor() => LoadTriumphs();
#endif
}

[System.Serializable]
public class TriumphSaveEntry
{
    public string id;
    public int currentCount;
    public bool isCompleted;
    public bool isRewardClaimed;
}

[System.Serializable]
public class TriumphSaveData
{
    public List<TriumphSaveEntry> entries = new List<TriumphSaveEntry>();
}
