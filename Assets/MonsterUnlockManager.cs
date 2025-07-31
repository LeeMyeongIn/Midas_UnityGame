using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class MonsterUnlockManager : MonoBehaviour
{
    public static MonsterUnlockManager Instance;

    private HashSet<string> seenMonsterSet = new HashSet<string>();
    private string savePath;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            savePath = Path.Combine(Application.persistentDataPath, "monster.json");
            Load();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void RegisterMonster(string monsterId)
    {
        if (!seenMonsterSet.Contains(monsterId))
        {
            seenMonsterSet.Add(monsterId);
            Save();
            Debug.Log($"[Codex] 몬스터 등록됨: {monsterId}");
        }
    }

    public bool HasSeen(string monsterId)
    {
        return seenMonsterSet.Contains(monsterId);
    }

    public List<string> GetSeenMonsterIds()
    {
        return new List<string>(seenMonsterSet);
    }

    private void Save()
    {
        MonsterSaveData data = new MonsterSaveData
        {
            seenMonsterIds = new List<string>(seenMonsterSet)
        };

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);
        Debug.Log($"[Codex] monster.json 저장됨 → {savePath}");
    }

    private void Load()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            MonsterSaveData data = JsonUtility.FromJson<MonsterSaveData>(json);
            seenMonsterSet = new HashSet<string>(data.seenMonsterIds ?? new List<string>());
            Debug.Log($"[Codex] monster.json 로드됨 (등록된 몬스터 수: {seenMonsterSet.Count})");
        }
        else
        {
            seenMonsterSet = new HashSet<string>();
            Debug.Log("[Codex] monster.json 파일 없음, 새로운 데이터 생성");
        }
    }

    [System.Serializable]
    private class MonsterSaveData
    {
        public List<string> seenMonsterIds;
    }
}
