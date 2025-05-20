using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tools/WanderingMerchantSummonAction")]
public class WanderingMerchantSummonAction : ToolAction
{
    

    public override void OnItemUsed(Item usedItem, ItemContainer inventory)
    {
        // 1. 떠돌이 상인 소환
        WanderingMerchantManager.Instance.SummonMerchant(5f, 300f);
        Debug.Log("떠돌이 상인이 일정 시간 후에 등장합니다!");

        // 2. 주문서 아이템 제거
        if (inventory != null)
        {
            inventory.Remove(usedItem); // S 아이템 제거
            Debug.Log("주문서가 인벤토리에서 제거되었습니다.");
        }
        else
        {
            Debug.LogWarning("인벤토리가 null입니다. 아이템을 제거할 수 없습니다.");
        }
    }
}

