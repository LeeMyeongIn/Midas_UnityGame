using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryPanel : ItemPanel, IDropHandler
{
    public override void OnClick(int id)
    {
        GameManager.instance.dragAndDropController.OnClick(inventory.slots[id]);
        Show();
    }

    public void OnDrop(PointerEventData eventData)
    {
        // Drag 중인 아이템 정보 가져오기
        ItemSlot dragSlot = GameManager.instance.dragAndDropController.itemSlot;

        if (dragSlot == null || dragSlot.item == null)
        {
            Debug.Log("[InventoryPanel.OnDrop] 드래그 중인 아이템이 없음");
            return;
        }

        Debug.Log($"[InventoryPanel.OnDrop] {dragSlot.item.Name} 아이템 {dragSlot.count}개 드랍 시도");

        // 인벤토리에 아이템 추가
        inventory.Add(dragSlot.item, dragSlot.count);

        // Drag 상태 초기화
        dragSlot.Clear();
        GameManager.instance.dragAndDropController.UpdateIcon();

        // 인벤토리 다시 갱신
        Show();
    }
}
