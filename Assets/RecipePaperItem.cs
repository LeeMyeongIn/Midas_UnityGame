using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/RecipePaperItem")]
public class RecipePaperItem : Item
{
    public int recipeIdToUnlock;

    public void UseRecipe()
    {
        if (RecipeUnlockManager.Instance != null)
        {
            RecipeUnlockManager.Instance.Unlock(recipeIdToUnlock);
            Debug.Log($"{Name} 사용됨 → 레시피 {recipeIdToUnlock} 해금!");
        }
        else
        {
            Debug.LogWarning("RecipeUnlockManager 인스턴스가 존재하지 않습니다.");
        }
    }
}
