using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CropSeenManager : MonoBehaviour
{
    public static CropSeenManager Instance;

    private HashSet<int> seenCropItemIds = new HashSet<int>();
    private int totalCropCount = 16;

    private int currentSlot = 0;

    [System.Serializable]
    private class CropSaveData
    {
        public List<int> seenIds = new List<int>();
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 유지
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        SetSaveSlot(SelectedSlotHolder.slotNumber); // 슬롯 연동
    }
    public void SetSaveSlot(int slot)
    {
        currentSlot = Mathf.Clamp(slot, 0, 2);
        Debug.Log($"[작물 도감] 현재 슬롯: {currentSlot}");
        Load();
    }

    private string GetSavePath()
    {
        return Path.Combine(Application.persistentDataPath, $"crop_slot{currentSlot}.json");
    }

    public bool RegisterSeenItem(int itemId)
    {
        if (!seenCropItemIds.Contains(itemId))
        {
            seenCropItemIds.Add(itemId);
            Save();
            return true;
        }
        return false;
    }

    public bool HasSeenItem(int itemId)
    {
        return seenCropItemIds.Contains(itemId);
    }

    public int GetSeenCropCount()
    {
        return seenCropItemIds.Count;
    }

    public int GetTotalCropCount()
    {
        return totalCropCount;
    }

    public bool IsAllSeen()
    {
        return seenCropItemIds.Count >= totalCropCount;
    }

    public void ClearAll()
    {
        seenCropItemIds.Clear();
        Save();
        Debug.Log("[도감] 전체 작물 도감 초기화됨");
    }

    private void Save()
    {
        CropSaveData data = new CropSaveData
        {
            seenIds = new List<int>(seenCropItemIds)
        };

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(GetSavePath(), json);
        Debug.Log($"[작물 도감] 저장 완료: {GetSavePath()}");
    }

    private void Load()
    {
        string path = GetSavePath();

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            CropSaveData data = JsonUtility.FromJson<CropSaveData>(json);
            seenCropItemIds = new HashSet<int>(data.seenIds ?? new List<int>());
            Debug.Log($"[작물 도감] 불러오기 완료: {path}");
        }
        else
        {
            seenCropItemIds.Clear();
            Debug.Log($"[작물 도감] {path} 파일 없음, 새로운 도감 데이터 생성");
        }
    }
}
