using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class RecipeUnlockManager : MonoBehaviour
{
    public static RecipeUnlockManager Instance;

    private HashSet<string> unlockedRecipeSet = new HashSet<string>();
    private HashSet<string> cookedRecipeSet = new HashSet<string>();
    private string savePath;

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
            savePath = Path.Combine(Application.persistentDataPath, "recipe.json");
            Load();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Load()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            RecipeSaveData data = JsonUtility.FromJson<RecipeSaveData>(json);
            unlockedRecipeSet = new HashSet<string>(data.unlocked ?? new List<string>());
            cookedRecipeSet = new HashSet<string>(data.cooked ?? new List<string>());
        }
        else
        {
            unlockedRecipeSet.Clear();
            cookedRecipeSet.Clear();
        }

        Debug.Log("[Recipe] recipe.json 로드 완료");
    }

    private void Save()
    {
        RecipeSaveData data = new RecipeSaveData
        {
            unlocked = new List<string>(unlockedRecipeSet),
            cooked = new List<string>(cookedRecipeSet)
        };

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);
        Debug.Log("[Recipe] recipe.json 저장 완료");
    }

    public void Unlock(int recipeId)
    {
        string id = recipeId.ToString();
        if (!unlockedRecipeSet.Contains(id))
        {
            unlockedRecipeSet.Add(id);
            Save();
        }
    }

    public void RegisterCooked(int recipeId)
    {
        string id = recipeId.ToString();
        if (!cookedRecipeSet.Contains(id))
        {
            cookedRecipeSet.Add(id);
            Save();
        }
    }

    public bool IsUnlocked(int recipeId) => unlockedRecipeSet.Contains(recipeId.ToString());
    public bool IsCooked(int recipeId) => cookedRecipeSet.Contains(recipeId.ToString());
    public int GetCookedCount() => cookedRecipeSet.Count;
    public bool IsAllCooked() => GetCookedCount() >= 18;
    public bool IsAllUnlocked() => unlockedRecipeSet.Count >= 18;
    public List<int> GetUnlockedList()
    {
        List<int> result = new List<int>();
        foreach (var id in unlockedRecipeSet)
            if (int.TryParse(id, out int parsed)) result.Add(parsed);
        return result;
    }

    public int GetTotalRecipeCount()
    {
        return 18;
    }

}
