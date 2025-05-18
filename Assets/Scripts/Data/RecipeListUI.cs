using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RecipeListUI : MonoBehaviour
{
    [Header("������ ������ & ǥ�� ��ġ")]
    public GameObject recipeSlotPrefab;
    public Transform contentParent;

    [Header("��� ������ ���")]
    public List<CookRecipe> allRecipes;

    [Header("�رݵ� ������ ID ���")]
    public List<int> unlockedRecipeIds;

    private void Start()
    {
        RecipeUnlockManager.Instance.Unlock(0);
        RecipeUnlockManager.Instance.Unlock(1);

        RefreshRecipeList();
    }

    public void RefreshRecipeList()
    {

        unlockedRecipeIds = RecipeUnlockManager.Instance.GetUnlockedList();
        Debug.Log($"[RecipeListUI] �رݵ� ������ ID ���: {string.Join(", ", unlockedRecipeIds)}");


        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }


        var sortedRecipes = allRecipes
            .OrderByDescending(recipe => unlockedRecipeIds.Contains(recipe.recipeId))
            .ThenBy(recipe => recipe.recipeName)
            .ToList();

        Debug.Log($"[RecipeListUI] allRecipes ����: {allRecipes.Count}");
        foreach (var r in allRecipes)
            Debug.Log($"[RecipeListUI] ��ü ������: ID={r.recipeId}, �̸�={r.recipeName}");


        foreach (var recipe in sortedRecipes)
        {
            Debug.Log($"[RecipeListUI] ���� ���� ��: ID={recipe.recipeId}, �̸�={recipe.recipeName}, �رݿ���={unlockedRecipeIds.Contains(recipe.recipeId)}");

            GameObject go = Instantiate(recipeSlotPrefab, contentParent);
            RecipeSlotUI slot = go.GetComponent<RecipeSlotUI>();
            bool isUnlocked = unlockedRecipeIds.Contains(recipe.recipeId);
            slot.Initialize(recipe, isUnlocked);
        }
    }


    public void BuyRecipe(int recipeId)
    {
        if (RecipeUnlockManager.Instance.IsUnlocked(recipeId)) return;

        RecipeUnlockManager.Instance.Unlock(recipeId);
        FindObjectOfType<RecipeListUI>().RefreshRecipeList();
        Debug.Log("������ �رݵ�!");
    }
    public void UnlockRecipe(int recipeId)
    {
        if (!unlockedRecipeIds.Contains(recipeId))
        {
            unlockedRecipeIds.Add(recipeId);
            RefreshRecipeList();
        }
    }
}

