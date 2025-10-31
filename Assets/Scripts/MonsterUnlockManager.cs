using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MonsterUnlockManager : MonoBehaviour
{
    public static MonsterUnlockManager Instance;

    private HashSet<string> seenMonsterSet = new HashSet<string>();
    private int currentSlot = -1;

    [System.Serializable]
    private class StringListWrapper
    {
        public List<string> items;
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            CreateAllSlotFiles();

            // 기본 슬롯 자동 설정
            if (currentSlot == -1)
            {
                SetSaveSlot(0);
                Debug.Log("[몬스터 Codex] 초기 슬롯이 설정되지 않아 slot0으로 자동 설정됨.");
            }
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public void CreateAllSlotFiles()
    {
        for (int slot = 0; slot <= 2; slot++)
        {
            string path = Path.Combine(Application.persistentDataPath, $"monster_slot{slot}.json");

            if (!File.Exists(path))
            {
                var wrapper = new StringListWrapper { items = new List<string>() };
                string json = JsonUtility.ToJson(wrapper, true);
                File.WriteAllText(path, json);
                Debug.Log($"[몬스터 Codex] 슬롯 {slot} 초기 JSON 생성 완료: {path}");
            }
        }
    }

    public void SetSaveSlot(int slot)
    {
        currentSlot = Mathf.Clamp(slot, 0, 2);
        Load();
        Debug.Log($"[몬스터 Codex] 현재 슬롯: {currentSlot}, 경로: {GetSavePath()}");
    }

    private string GetSavePath()
    {
        if (currentSlot == -1)
        {
            Debug.LogError("[몬스터 Codex] 경로 요청 시 슬롯이 설정되지 않았습니다!");
            return null;
        }
        return Path.Combine(Application.persistentDataPath, $"monster_slot{currentSlot}.json");
    }

    public void RegisterMonster(string monsterId)
    {
        if (currentSlot == -1)
        {
            Debug.LogWarning("[몬스터 Codex] 슬롯이 설정되지 않아 Unlock이 취소됨.");
            return;
        }

        if (!seenMonsterSet.Contains(monsterId))
        {
            seenMonsterSet.Add(monsterId);
            Save();
            Debug.Log($"[몬스터 Codex] 새 몬스터 등록됨: {monsterId}, 총 개수: {seenMonsterSet.Count}");

            // 업적 시스템 연동
            if (TriumphManager.Instance != null)
            {
                TriumphManager.Instance.UpdateMonsterCodexAchievements();
                Debug.Log("[몬스터 Codex] 업적 시스템 갱신 완료.");
            }
            else
            {
                Debug.LogWarning("[몬스터 Codex] TriumphManager 인스턴스를 찾을 수 없습니다.");
            }
        }
        else
        {
            Debug.Log($"[몬스터 Codex] 이미 등록된 몬스터: {monsterId}");
        }
    }

    public bool HasSeen(string monsterId) => seenMonsterSet.Contains(monsterId);
    public List<string> GetSeenMonsterIds() => new List<string>(seenMonsterSet);
    public int GetSeenMonsterCount() => seenMonsterSet.Count;

    private void Save()
    {
        if (currentSlot == -1)
        {
            Debug.LogError("[몬스터 Codex] Save() 시 슬롯이 설정되지 않았습니다. SetSaveSlot()을 먼저 호출해야 합니다.");
            return;
        }

        string path = GetSavePath();
        string json = JsonUtility.ToJson(new StringListWrapper { items = new List<string>(seenMonsterSet) }, true);
        File.WriteAllText(path, json);
        Debug.Log($"[몬스터 Codex] 저장 완료: {path}");
    }

    private void Load()
    {
        string path = GetSavePath();

        if (!string.IsNullOrEmpty(path) && File.Exists(path))
        {
            string json = File.ReadAllText(path);
            StringListWrapper wrapper = JsonUtility.FromJson<StringListWrapper>(json);

            if (wrapper != null && wrapper.items != null)
                seenMonsterSet = new HashSet<string>(wrapper.items);
            else
                seenMonsterSet.Clear();

            Debug.Log($"[몬스터 Codex] 불러오기 완료 ({wrapper?.items?.Count ?? 0}개): {path}");
        }
        else
        {
            seenMonsterSet.Clear();
            var wrapper = new StringListWrapper { items = new List<string>() };
            string json = JsonUtility.ToJson(wrapper, true);
            File.WriteAllText(path, json);
            Debug.Log($"[몬스터 Codex] {path} 파일이 없어 새로 생성함");
        }
    }
}
