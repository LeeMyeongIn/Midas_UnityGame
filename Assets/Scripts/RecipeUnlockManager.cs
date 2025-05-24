using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class RecipeSaveData
{
    public List<int> unlockedRecipeIds = new List<int>();
}

public class RecipeUnlockManager : MonoBehaviour
{
    public static RecipeUnlockManager Instance;

    private const string SaveFileName = "recipes.json";
    private RecipeSaveData saveData = new RecipeSaveData();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Load();
        }
        else
            Destroy(gameObject);
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
            Debug.Log($"[����] ������ �ر� ���� ī��Ʈ �߰�");

            TriumphManager.Instance?.UpdateProgressByType(TriumphType.RecipeUnlock, 1);
        }
    }

    public List<int> GetUnlockedList()
    {
        return saveData.unlockedRecipeIds;
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

    // �����
    public List<int> GetUnlockedRecipeIds()
    {
        return new List<int>(saveData.unlockedRecipeIds);
    }

    // �ҷ������
    public void SetUnlockedRecipes(List<int> ids)
    {
        saveData.unlockedRecipeIds = ids ?? new List<int>();
        Save();
    }
}
