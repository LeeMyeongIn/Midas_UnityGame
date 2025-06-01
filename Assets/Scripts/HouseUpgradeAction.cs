using UnityEngine;

[CreateAssetMenu(menuName = "Data/ToolAction/HouseUpgradeAction")]
public class HouseUpgradeAction : ToolAction
{
    public int targetLevel = 2;

    public override void OnItemUsed(Item usedItem, ItemContainer inventory)
    {
        var controller = GameObject.FindObjectOfType<HouseUpgradeController>();
        if (controller == null)
        {
            return;
        }

        controller.UpgradeToLevel(targetLevel);

        if (inventory != null && usedItem != null)
        {
            inventory.Remove(usedItem);
        }
        else
        {
            Debug.LogWarning("�κ��丮 �Ǵ� �������� null");
        }
    }
}
