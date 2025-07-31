using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class DropCodexSaveData
{
    public List<string> seenDropItemIds = new List<string>();
}

public class MonsterDropCodexTracker : MonoBehaviour
{
    public static MonsterDropCodexTracker Instance;

    private const string SaveFileName = "codex.json";

    private DropCodexSaveData saveData = new DropCodexSaveData();
    private HashSet<string> seenDropItemSet = new HashSet<string>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            LoadData();
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
            saveData.seenDropItemIds.Add(itemId);
            SaveData();
        }
    }

    public bool HasSeen(string itemId)
    {
        return seenDropItemSet.Contains(itemId);
    }

    public List<Item> GetSeenDrops()
    {
        List<Item> result = new List<Item>();

        foreach (string id in saveData.seenDropItemIds)
        {
            Item item = CodexUIManager.Instance.GetItemById(id);
            if (item != null)
                result.Add(item);
        }

        return result;
    }

    private void SaveData()
    {
        string json = JsonUtility.ToJson(saveData);
        File.WriteAllText(Path.Combine(Application.persistentDataPath, SaveFileName), json);
    }

    private void LoadData()
    {
        string path = Path.Combine(Application.persistentDataPath, SaveFileName);
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            saveData = JsonUtility.FromJson<DropCodexSaveData>(json);
            seenDropItemSet = new HashSet<string>(saveData.seenDropItemIds);
        }
    }
}
