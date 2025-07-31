using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MonsterUnlockManager : MonoBehaviour
{
    public static MonsterUnlockManager Instance;

    private HashSet<string> seenMonsterSet = new HashSet<string>();
    private int currentSlot = -1;
    private string savePath;

    [System.Serializable]
    private class MonsterSaveData
    {
        public List<string> seenMonsterIds = new List<string>();
    }

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

    private void Start()
    {
        CreateAllSlotFiles(); // 모든 슬롯 JSON 생성
        SetSaveSlot(SelectedSlotHolder.slotNumber); // 현재 슬롯 선택
    }

    public void CreateAllSlotFiles()
    {
        for (int slot = 0; slot <= 2; slot++)
        {
            string path = Path.Combine(Application.persistentDataPath, $"monster_slot{slot}.json");

            if (!File.Exists(path))
            {
                MonsterSaveData newData = new MonsterSaveData(); // 빈 도감
                string json = JsonUtility.ToJson(newData, true);
                File.WriteAllText(path, json);
                Debug.Log($"[몬스터 도감] 슬롯 {slot} 초기 JSON 생성 완료: {path}");
            }
        }
    }

    public void SetSaveSlot(int slot)
    {
        currentSlot = Mathf.Clamp(slot, 0, 2);
        savePath = Path.Combine(Application.persistentDataPath, $"monster_slot{currentSlot}.json");

        if (!File.Exists(savePath))
        {
            Debug.Log($"[몬스터 도감] {savePath} 없음 → 새로 생성");
            Save();
        }

        Load();
        Debug.Log($"[몬스터 도감] 현재 슬롯: {currentSlot}, 경로: {savePath}");
    }

    public void RegisterMonster(string monsterId)
    {
        if (string.IsNullOrEmpty(savePath))
        {
            Debug.LogError("[몬스터 도감] savePath가 초기화되지 않았습니다. SetSaveSlot을 먼저 호출해야 합니다.");
            return;
        }

        if (!seenMonsterSet.Contains(monsterId))
        {
            seenMonsterSet.Add(monsterId);
            Save();
            TriumphManager.Instance?.UpdateMonsterCodexAchievements();
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

    public int GetSeenMonsterCount()
    {
        return seenMonsterSet.Count;
    }

    public void ClearAll()
    {
        seenMonsterSet.Clear();
        Save();
        Debug.Log("[몬스터 도감] 초기화 완료");
    }

    private void Save()
    {
        if (string.IsNullOrEmpty(savePath))
        {
            Debug.LogError("[몬스터 도감] Save() 호출 시 savePath가 null이거나 비어 있음");
            return;
        }

        MonsterSaveData data = new MonsterSaveData
        {
            seenMonsterIds = new List<string>(seenMonsterSet)
        };

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);
        Debug.Log($"[몬스터 도감] 저장 완료: {savePath}");
    }

    private void Load()
    {
        if (string.IsNullOrEmpty(savePath))
        {
            Debug.LogError("[몬스터 도감] Load() 호출 시 savePath가 null이거나 비어 있음");
            return;
        }

        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            MonsterSaveData data = JsonUtility.FromJson<MonsterSaveData>(json);
            seenMonsterSet = new HashSet<string>(data.seenMonsterIds ?? new List<string>());
            Debug.Log($"[몬스터 도감] 불러오기 완료: {savePath}");
        }
        else
        {
            seenMonsterSet = new HashSet<string>();
            Debug.Log($"[몬스터 도감] {savePath} 파일 없음, 새로운 데이터 생성");
        }
    }
}
