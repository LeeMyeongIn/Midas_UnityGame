using System.Collections.Generic;
using UnityEngine;

public class CropSeenManager : MonoBehaviour
{
    public static CropSeenManager Instance;

    private HashSet<int> seenCropItemIds = new HashSet<int>();

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
        }
    }

    public bool RegisterSeenItem(int itemId)
    {
        if (!seenCropItemIds.Contains(itemId))
        {
            seenCropItemIds.Add(itemId);
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

    public void ClearAll()
    {
        seenCropItemIds.Clear();
        Debug.Log("[도감] 전체 작물 도감 초기화됨");
    }

    // 저장용
    public List<int> GetSeenCropItemIds()
    {
        return new List<int>(seenCropItemIds);
    }

    // 불러오기용
    public void SetSeenCropItemIds(List<int> itemIds)
    {
        seenCropItemIds = new HashSet<int>(itemIds ?? new List<int>());
        Debug.Log($"[도감] 저장된 도감 불러오기 완료 ({seenCropItemIds.Count}개)");
    }
}