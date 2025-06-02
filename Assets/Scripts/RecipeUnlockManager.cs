using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class RecipeSaveData
{
    public List<int> unlockedRecipeIds = new List<int>();
    public List<int> cookedRecipeIds = new List<int>();
}

public class RecipeUnlockManager : MonoBehaviour
{
    public static RecipeUnlockManager Instance;

    private const string SaveFileName = "recipes.json";
    private RecipeSaveData saveData = new RecipeSaveData();

    private HashSet<int> cookedRecipeSet = new HashSet<int>();

    private void Awake()
    {
        Instance = this;
        Load();
        RebuildCookedSet();
    }


    public bool IsUnlocked(int recipeId)
    {
        return saveData.unlockedRecipeIds.Contains(recipeId);
    }

    public void Unlock(int recipeId)
    {
        if (!saveData.unlockedRecipeIds.Contains(recipeId))
        {
            saveData.unlockedRecipeIds.Add(recipeId);
            Save();
            Debug.Log($"[업적] 레시피 해금 업적 카운트 추가");
            TriumphManager.Instance?.UpdateProgressByType(TriumphType.RecipeUnlock, 1);
        }
    }

    public List<int> GetUnlockedList()
    {
        return saveData.unlockedRecipeIds;
    }

    int totalRecipeCount = 18;

    public int GetTotalRecipeCount()
    {
        return totalRecipeCount;
    }

    public bool IsAllUnlocked()
    {
        return saveData.unlockedRecipeIds.Count >= totalRecipeCount;
    }

    public void RegisterCooked(int recipeId)
    {
        if (!cookedRecipeSet.Contains(recipeId))
        {
            cookedRecipeSet.Add(recipeId);
            saveData.cookedRecipeIds.Add(recipeId);
            Save();
            Debug.Log($"[요리 등록] 제작 완료: {recipeId}");
        }
    }

    public bool IsAllCooked()
    {
        int totalRecipeCount = 18;
        return cookedRecipeSet.Count >= totalRecipeCount;
    }

    public int GetCookedCount()
    {
        return cookedRecipeSet.Count;
    }

    private void Save()
    {
        string json = JsonUtility.ToJson(saveData);
        File.WriteAllText(Application.persistentDataPath + "/" + SaveFileName, json);
    }

    private void Load()
    {
        string path = Application.persistentDataPath + "/" + SaveFileName;
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            saveData = JsonUtility.FromJson<RecipeSaveData>(json);
        }
    }

    private void RebuildCookedSet()
    {
        cookedRecipeSet = new HashSet<int>(saveData.cookedRecipeIds);
    }
}
