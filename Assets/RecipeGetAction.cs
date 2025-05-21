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
            Debug.Log($"������ {recipeId} �رݵ�");

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
            Debug.LogWarning("�� �������� RecipePaperItem�� �ƴմϴ�.");
        }
    }
}
