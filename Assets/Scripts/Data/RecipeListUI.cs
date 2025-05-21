using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RecipeListUI : MonoBehaviour
{
    [Header("레시피 프리팹 & 표시 위치")]
    public GameObject recipeSlotPrefab;
    public Transform contentParent;

    [Header("모든 레시피 목록")]
    public List<CookRecipe> allRecipes;

    [Header("해금된 레시피 ID 목록")]
    public List<int> unlockedRecipeIds;

    private void Start()
    {
        RefreshRecipeList();
    }

    public void RefreshRecipeList()
    {
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        unlockedRecipeIds = RecipeUnlockManager.Instance.GetUnlockedList();


        var sortedRecipes = allRecipes
            .OrderByDescending(recipe => unlockedRecipeIds.Contains(recipe.recipeId))
            .ThenBy(recipe => recipe.recipeName)
            .ToList();


        foreach (var recipe in sortedRecipes)
        {
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
        RefreshRecipeList();
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
