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
            Debug.Log($"{Name} ���� �� ������ {recipeIdToUnlock} �ر�!");
        }
        else
        {
            Debug.LogWarning("RecipeUnlockManager �ν��Ͻ��� �������� �ʽ��ϴ�.");
        }
    }
}
