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
            Debug.Log($"레시피 {recipeId} 해금됨");

            ItemSlot slot = inventory.slots.Find(s => s.item == usedItem);
            if (slot != null)
            {
                slot.count--;
                if (slot.count <= 0) slot.Clear();
            }

            inventory.isDirty = true;
        }
        else
        {
            Debug.LogWarning("이 아이템은 RecipePaperItem이 아닙니다.");
        }
    }
}
