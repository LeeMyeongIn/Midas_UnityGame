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

        // 슬롯 미설정 방지
        if (SelectedSlotHolder.slotNumber < 0)
        {
            Debug.LogWarning("[업적] 잘못된 슬롯 감지 (-1). 기본 슬롯 0으로 초기화합니다.");
            SelectedSlotHolder.slotNumber = 0;
        }

        // 게임 시작 시 현재 슬롯 바로 적용
        SetSaveSlot(SelectedSlotHolder.slotNumber);
    }

    private void OnApplicationQuit()
    {
        SaveTriumphs();
    }

    /// <summary>
    /// 슬롯별 JSON 파일이 없으면 생성
    /// </summary>
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

    /// <summary>
    /// 현재 사용할 슬롯 설정
    /// </summary>
    public void SetSaveSlot(int slot)
    {
        currentSlot = Mathf.Clamp(slot, 0, 2);
        LoadTriumphs();
        Debug.Log($"[업적] 현재 슬롯: {currentSlot}, 경로: {GetSavePath()}");
    }

    private string GetSavePath()
    {
        return Path.Combine(Application.persistentDataPath, $"triumphs_slot{currentSlot}.json");
    }

    public void SaveTriumphs()
    {
        if (currentSlot < 0)
        {
            Debug.LogWarning("[업적] 슬롯이 설정되지 않아 저장이 취소됨.");
            return;
        }

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
        Debug.Log($"[업적] 저장 완료 (슬롯 {currentSlot}): {GetSavePath()}");
    }

    public void LoadTriumphs()
    {
        string path = GetSavePath();

        if (File.Exists(path))
        {
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
            Debug.Log($"[업적] 불러오기 완료 (슬롯 {currentSlot}): {path}");
        }
        else
        {
            Debug.Log($"[업적] {path} 파일 없음, 새로운 데이터 생성");
            ResetAllTriumphs();
            SaveTriumphs();
        }
    }

    // ==============================
    // 업적 관련 갱신 메서드
    // ==============================

    public void UpdateCropTypeAchievements()
    {
        int uniqueCropCount = CropSeenManager.Instance.GetSeenCropCount();

        foreach (var triumph in triumphList)
        {
            if (triumph.data.type == TriumphType.CropHarvest && !triumph.data.isCompleted)
            {
                triumph.data.currentCount = Mathf.Min(uniqueCropCount, triumph.data.targetCount);

                if (triumph.data.currentCount >= triumph.data.targetCount)
                    triumph.data.isCompleted = true;

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
                    triumph.data.isCompleted = true;

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
                    triumph.data.isCompleted = true;

                onTriumphUpdated?.Invoke();
            }
        }
    }

    // ==============================
    // 보상 관련 메서드
    // ==============================

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
        SaveTriumphs();
    }

    // ==============================
    // 통계 메서드
    // ==============================

    public bool HasAnyClaimableTriumph()
    {
        foreach (var triumph in triumphList)
            if (CanClaimReward(triumph.data))
                return true;
        return false;
    }

    public bool AreAllRewardsClaimed()
    {
        foreach (var triumph in triumphList)
            if (!triumph.data.isCompleted || !triumph.data.isRewardClaimed)
                return false;
        return true;
    }

    public int GetClaimedRewardCount()
    {
        int count = 0;
        foreach (var triumph in triumphList)
            if (triumph.data.isRewardClaimed)
                count++;
        return count;
    }

    public int GetTotalRewardCount() => triumphList.Count;

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

        onTriumphUpdated?.Invoke();
        Debug.Log("[업적] 모든 업적 초기화 완료");
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
