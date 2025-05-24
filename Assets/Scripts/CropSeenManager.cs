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
        Debug.Log("[����] ��ü �۹� ���� �ʱ�ȭ��");
    }
}