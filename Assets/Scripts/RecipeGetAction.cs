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
            Debug.Log($"[RecipeGetAction] ������ {recipeId} �رݵ�");

            if (inventory != null)
            {
                inventory.Remove(usedItem);
                inventory.isDirty = true;
            }
            else
            {
                Debug.LogWarning("[RecipeGetAction] �κ��丮�� null�Դϴ�.");
            }
        }
        else
        {
            Debug.LogWarning("[RecipeGetAction] �� �������� RecipePaperItem�� �ƴմϴ�.");
        }
    }
}
