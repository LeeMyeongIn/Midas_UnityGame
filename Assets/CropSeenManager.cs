using System.Collections;
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
    public void RegisterSeenItem(int itemId)
    {
        seenCropItemIds.Add(itemId);
    }
    public bool HasSeenItem(int itemId)
    {
        return seenCropItemIds.Contains(itemId);
    }
}
