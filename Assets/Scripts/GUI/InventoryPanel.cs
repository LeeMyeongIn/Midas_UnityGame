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
        // Drag ���� ������ ���� ��������
        ItemSlot dragSlot = GameManager.instance.dragAndDropController.itemSlot;

        if (dragSlot == null || dragSlot.item == null)
        {
            Debug.Log("[InventoryPanel.OnDrop] �巡�� ���� �������� ����");
            return;
        }

        Debug.Log($"[InventoryPanel.OnDrop] {dragSlot.item.Name} ������ {dragSlot.count}�� ��� �õ�");

        // �κ��丮�� ������ �߰�
        inventory.Add(dragSlot.item, dragSlot.count);

        // Drag ���� �ʱ�ȭ
        dragSlot.Clear();
        GameManager.instance.dragAndDropController.UpdateIcon();

        // �κ��丮 �ٽ� ����
        Show();
    }
}
