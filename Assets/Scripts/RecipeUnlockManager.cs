using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class RecipeUnlockManager : MonoBehaviour
{
    public static RecipeUnlockManager Instance;

    private HashSet<string> unlockedRecipeSet = new HashSet<string>();
    private HashSet<string> cookedRecipeSet = new HashSet<string>();
    private int currentSlot = -1;

    private const int totalRecipeCount = 18;

    [System.Serializable]
    private class RecipeSaveData
    {
        public List<string> unlocked = new List<string>();
        public List<string> cooked = new List<string>();
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

        //  슬롯 미설정 방지
        if (SelectedSlotHolder.slotNumber < 0)
        {
            Debug.LogWarning("[레시피 도감] 잘못된 슬롯 감지 (-1). 기본 슬롯 0으로 초기화합니다.");
            SelectedSlotHolder.slotNumber = 0;
        }

        // 슬롯 초기 로드 (게임 시작 시 바로 적용 가능)
        SetSaveSlot(SelectedSlotHolder.slotNumber);
    }

    /// <summary>
    /// 슬롯별 JSON 파일이 없으면 생성
    /// </summary>
    public void CreateAllSlotFiles()
    {
        for (int slot = 0; slot <= 2; slot++)
        {
            string path = Path.Combine(Application.persistentDataPath, $"recipe_slot{slot}.json");

            if (!File.Exists(path))
            {
                RecipeSaveData newData = new RecipeSaveData();
                string json = JsonUtility.ToJson(newData, true);
                File.WriteAllText(path, json);
                Debug.Log($"[레시피 도감] 슬롯 {slot} 초기 JSON 생성 완료: {path}");
            }
        }
    }

    /// <summary>
    /// 현재 사용할 저장 슬롯 설정
    /// </summary>
    public void SetSaveSlot(int slot)
    {
        currentSlot = Mathf.Clamp(slot, 0, 2);
        Load();
        Debug.Log($"[레시피 도감] 현재 슬롯: {currentSlot}, 경로: {GetSavePath()}");
    }

    private string GetSavePath()
    {
        return Path.Combine(Application.persistentDataPath, $"recipe_slot{currentSlot}.json");
    }

    public void Unlock(int recipeId)
    {
        // 슬롯이 설정되지 않은 경우 방어
        if (currentSlot < 0)
        {
            Debug.LogWarning("[레시피 도감] 슬롯이 설정되지 않아 Unlock이 취소됨.");
            return;
        }

        string id = recipeId.ToString();
        if (!unlockedRecipeSet.Contains(id))
        {
            unlockedRecipeSet.Add(id);
            Save();
            Debug.Log($"[레시피 도감] 레시피 {id} 해금 (슬롯 {currentSlot})");
        }
    }

    public void RegisterCooked(int recipeId)
    {
        // 슬롯이 설정되지 않은 경우 방어
        if (currentSlot < 0)
        {
            Debug.LogWarning("[레시피 도감] 슬롯이 설정되지 않아 RegisterCooked가 취소됨.");
            return;
        }

        string id = recipeId.ToString();
        if (!cookedRecipeSet.Contains(id))
        {
            cookedRecipeSet.Add(id);
            Save();
            Debug.Log($"[레시피 도감] 레시피 {id} 요리 등록 (슬롯 {currentSlot})");
        }
    }

    public bool IsUnlocked(int recipeId) => unlockedRecipeSet.Contains(recipeId.ToString());
    public bool IsCooked(int recipeId) => cookedRecipeSet.Contains(recipeId.ToString());
    public int GetCookedCount() => cookedRecipeSet.Count;
    public bool IsAllCooked() => cookedRecipeSet.Count >= totalRecipeCount;
    public bool IsAllUnlocked() => unlockedRecipeSet.Count >= totalRecipeCount;

    public List<int> GetUnlockedList()
    {
        List<int> result = new List<int>();
        foreach (var id in unlockedRecipeSet)
            if (int.TryParse(id, out int parsed)) result.Add(parsed);
        return result;
    }

    public int GetTotalRecipeCount()
    {
        return totalRecipeCount;
    }

    private void Save()
    {
        RecipeSaveData data = new RecipeSaveData
        {
            unlocked = new List<string>(unlockedRecipeSet),
            cooked = new List<string>(cookedRecipeSet)
        };

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(GetSavePath(), json);
        Debug.Log($"[레시피 도감] 저장 완료 (슬롯 {currentSlot}): {GetSavePath()}");
    }

    private void Load()
    {
        string path = GetSavePath();

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            RecipeSaveData data = JsonUtility.FromJson<RecipeSaveData>(json);
            unlockedRecipeSet = new HashSet<string>(data.unlocked ?? new List<string>());
            cookedRecipeSet = new HashSet<string>(data.cooked ?? new List<string>());
            Debug.Log($"[레시피 도감] 불러오기 완료 (슬롯 {currentSlot}): {path}");
        }
        else
        {
            unlockedRecipeSet.Clear();
            cookedRecipeSet.Clear();
            Debug.Log($"[레시피 도감] {path} 파일 없음, 새로운 데이터 생성");
        }
    }
}
