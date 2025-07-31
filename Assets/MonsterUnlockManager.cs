using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class CodexSaveData
{
    public List<int> unlockedRecipeIds = new List<int>();
    public List<int> cookedRecipeIds = new List<int>();
    public List<string> seenMonsterIds = new List<string>();
}

public class MonsterUnlockManager : MonoBehaviour
{
    public static MonsterUnlockManager Instance;

    private const string SaveFileName = "codex.json";
    private HashSet<string> seenMonsterIds = new HashSet<string>();
    private CodexSaveData saveData = new CodexSaveData();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        LoadData();
    }

    public void MarkAsSeen(string monsterId)
    {
        if (!seenMonsterIds.Contains(monsterId))
        {
            seenMonsterIds.Add(monsterId);
            SaveData();
        }
    }

    public bool HasSeen(string monsterId)
    {
        return seenMonsterIds.Contains(monsterId);
    }

    private void LoadData()
    {
        string path = Application.persistentDataPath + "/" + SaveFileName;
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            saveData = JsonUtility.FromJson<CodexSaveData>(json);
            seenMonsterIds = new HashSet<string>(saveData.seenMonsterIds);
        }
    }

    private void SaveData()
    {
        string path = Application.persistentDataPath + "/" + SaveFileName;
        if (File.Exists(path))
        {
            // 기존 파일에서 다른 데이터 유지
            string json = File.ReadAllText(path);
            CodexSaveData existingData = JsonUtility.FromJson<CodexSaveData>(json);
            existingData.seenMonsterIds = new List<string>(seenMonsterIds);
            json = JsonUtility.ToJson(existingData);
            File.WriteAllText(path, json);
        }
        else
        {
            saveData.seenMonsterIds = new List<string>(seenMonsterIds);
            string json = JsonUtility.ToJson(saveData);
            File.WriteAllText(path, json);
        }
    }
}
