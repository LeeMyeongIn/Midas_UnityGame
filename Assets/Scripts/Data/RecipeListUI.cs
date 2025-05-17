using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        foreach (var recipe in allRecipes)
        {
            GameObject go = Instantiate(recipeSlotPrefab, contentParent);
            RecipeSlotUI slot = go.GetComponent<RecipeSlotUI>();
            bool isUnlocked = unlockedRecipeIds.Contains(recipe.recipeId);
            slot.Initialize(recipe, isUnlocked);
        }
    }
}
