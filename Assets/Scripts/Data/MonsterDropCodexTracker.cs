using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MonsterDropCodexTracker : MonoBehaviour
{
    public static MonsterDropCodexTracker Instance;

    private HashSet<string> seenDropItemSet = new HashSet<string>();
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
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // 슬롯 미설정 시 0번으로 초기화
        if (SelectedSlotHolder.slotNumber < 0)
        {
            Debug.LogWarning("[드랍 Codex] 잘못된 슬롯 감지 (-1). 기본 슬롯 0으로 초기화합니다.");
            SelectedSlotHolder.slotNumber = 0;
        }

        // 현재 슬롯 불러오기
        SetSaveSlot(SelectedSlotHolder.slotNumber);
    }

    public void CreateAllSlotFiles()
    {
        for (int slot = 0; slot <= 2; slot++)
        {
            string path = Path.Combine(Application.persistentDataPath, $"dropItem_slot{slot}.json");

            if (!File.Exists(path))
            {
                var wrapper = new StringListWrapper { items = new List<string>() };
                string json = JsonUtility.ToJson(wrapper, true);
                File.WriteAllText(path, json);
                Debug.Log($"[드랍 Codex] 슬롯 {slot} 초기 JSON 생성 완료: {path}");
            }
        }
    }

    public void SetSaveSlot(int slot)
    {
        currentSlot = Mathf.Clamp(slot, 0, 2);
        Load();
        Debug.Log($"[드랍 Codex] 현재 슬롯: {currentSlot}, 경로: {GetSavePath()}");
    }

    private string GetSavePath()
    {
        return Path.Combine(Application.persistentDataPath, $"dropItem_slot{currentSlot}.json");
    }

    public void RegisterDrop(string itemId)
    {
        if (currentSlot < 0)
        {
            Debug.LogError("[드랍 Codex] 현재 슬롯이 설정되지 않았습니다. SetSaveSlot()을 먼저 호출해야 합니다.");
            return;
        }

        if (!seenDropItemSet.Contains(itemId))
        {
            seenDropItemSet.Add(itemId);
            Save();

            if (TriumphManager.Instance != null)
            {
                Debug.Log($"[드랍 Codex] 새 드랍 등록됨: {itemId}, 총 개수: {seenDropItemSet.Count}");
                TriumphManager.Instance.UpdateDropItemCodexAchievements();
            }
            else
            {
                Debug.LogWarning("[드랍 Codex] TriumphManager 인스턴스를 찾을 수 없습니다.");
            }
        }
        else
        {
            Debug.Log($"[드랍 Codex] 이미 등록된 드랍: {itemId}");
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
            else
                Debug.LogWarning($"[드랍 Codex] 아이템 ID {id}에 해당하는 Item을 찾을 수 없습니다.");
        }

        return result;
    }

    public int GetSeenDropCount()
    {
        return seenDropItemSet.Count;
    }

    private void Save()
    {
        string json = JsonUtility.ToJson(new StringListWrapper { items = new List<string>(seenDropItemSet) }, true);
        File.WriteAllText(GetSavePath(), json);
        Debug.Log($"[드랍 Codex] 저장 완료: {GetSavePath()}");
    }

    private void Load()
    {
        string path = GetSavePath();

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            StringListWrapper wrapper = JsonUtility.FromJson<StringListWrapper>(json);
            seenDropItemSet = new HashSet<string>(wrapper.items ?? new List<string>());
            Debug.Log($"[드랍 Codex] 불러오기 완료: {path}");
        }
        else
        {
            seenDropItemSet.Clear();
            Debug.Log($"[드랍 Codex] {path} 파일 없음, 새로운 데이터 생성");
        }
    }
}
