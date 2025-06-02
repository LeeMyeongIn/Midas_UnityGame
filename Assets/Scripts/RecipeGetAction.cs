using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/ToolAction/RecipeGetAction")]
public class RecipeGetAction : ToolAction
{
    public override void OnItemUsed(Item usedItem, ItemContainer inventory)
    {
        if (usedItem is RecipePaperItem paper)
        {
            int recipeId = paper.recipeIdToUnlock;

            RecipeUnlockManager.Instance.Unlock(recipeId);
            Debug.Log($"[RecipeGetAction] 레시피 {recipeId} 해금됨");

            if (inventory != null)
            {
                inventory.Remove(usedItem);
                inventory.isDirty = true;
            }
            else
            {
                Debug.LogWarning("[RecipeGetAction] 인벤토리가 null입니다.");
            }
        }
        else
        {
            Debug.LogWarning("[RecipeGetAction] 이 아이템은 RecipePaperItem이 아닙니다.");
        }
    }
}
