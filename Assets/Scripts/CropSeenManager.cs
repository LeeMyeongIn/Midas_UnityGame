using System.Collections.Generic;
using UnityEngine;

public class CropSeenManager : MonoBehaviour
{
    public static CropSeenManager Instance;

    private HashSet<int> seenCropItemIds = new HashSet<int>();

    private int totalCropCount = 16;

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
        Debug.Log("[도감] 전체 작물 도감 초기화됨");
    }
}
