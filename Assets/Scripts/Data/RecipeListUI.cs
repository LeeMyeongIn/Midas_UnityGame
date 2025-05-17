using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
