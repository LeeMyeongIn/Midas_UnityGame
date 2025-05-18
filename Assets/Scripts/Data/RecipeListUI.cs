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
        RecipeUnlockManager.Instance.Unlock(0);
        RecipeUnlockManager.Instance.Unlock(1);

        RefreshRecipeList();
    }

    public void RefreshRecipeList()
    {

        unlockedRecipeIds = RecipeUnlockManager.Instance.GetUnlockedList();
        Debug.Log($"[RecipeListUI] 해금된 레시피 ID 목록: {string.Join(", ", unlockedRecipeIds)}");


        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }


        var sortedRecipes = allRecipes
            .OrderByDescending(recipe => unlockedRecipeIds.Contains(recipe.recipeId))
            .ThenBy(recipe => recipe.recipeName)
            .ToList();

        Debug.Log($"[RecipeListUI] allRecipes 개수: {allRecipes.Count}");
        foreach (var r in allRecipes)
            Debug.Log($"[RecipeListUI] 전체 레시피: ID={r.recipeId}, 이름={r.recipeName}");


        foreach (var recipe in sortedRecipes)
        {
            Debug.Log($"[RecipeListUI] 슬롯 생성 중: ID={recipe.recipeId}, 이름={recipe.recipeName}, 해금여부={unlockedRecipeIds.Contains(recipe.recipeId)}");

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
        Debug.Log("레시피 해금됨!");
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

