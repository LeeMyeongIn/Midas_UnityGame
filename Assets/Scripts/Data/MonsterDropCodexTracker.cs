using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MonsterDropCodexTracker : MonoBehaviour
{
    public static MonsterDropCodexTracker Instance;

    private HashSet<string> seenDropItemSet = new HashSet<string>();

    private string savePath;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            savePath = Application.persistentDataPath + "/dropItem.json";
            Load();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void RegisterDrop(string itemId)
    {
        if (!seenDropItemSet.Contains(itemId))
        {
            seenDropItemSet.Add(itemId);
            Save();
            Debug.Log($"드롭 아이템 등록됨: {itemId}");
        }
    }

    public bool HasSeen(string itemId)
    {
        return seenDropItemSet.Contains(itemId);
    }

    public List<Item> GetSeenDrops()
    {
        List<Item> result = new List<Item>();

        foreach (string id in seenDropItemSet)
        {
            Item item = CodexUIManager.Instance.GetItemById(id);
            if (item != null)
                result.Add(item);
        }

        return result;
    }

    private void Save()
    {
        List<string> listToSave = new List<string>(seenDropItemSet);
        string json = JsonUtility.ToJson(new StringListWrapper { items = listToSave }, true);
        File.WriteAllText(savePath, json);
    }

    private void Load()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            StringListWrapper wrapper = JsonUtility.FromJson<StringListWrapper>(json);
            seenDropItemSet = new HashSet<string>(wrapper.items);
        }
        else
        {
            seenDropItemSet = new HashSet<string>();
        }
    }

    [System.Serializable]
    private class StringListWrapper
    {
        public List<string> items;
    }
}
