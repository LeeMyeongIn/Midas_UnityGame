/*using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemStorePanel : ItemPanel
{
    [SerializeField] Trading trading;

    public override void OnClick(int id)
    {
        if(GameManager.instance.dragAndDropController.itemSlot.item == null)
        {
            BuyItem(id);
        }
        else
        {
            SellItem();
        }

            SellItem();

        Show();
    }

    private void BuyItem(int id)
    {
        trading.BuyItem(id);
    }

    private void SellItem()
    {
        trading.SellItem();
    }
}*/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemStorePanel : ItemPanel
{
    [SerializeField] Trading trading;
    [SerializeField] bool enableQuantitySelection = true; // ���� ���� ��� Ȱ��ȭ/��Ȱ��ȭ

    public override void OnClick(int id)
    {
        if (GameManager.instance.dragAndDropController.itemSlot.item == null)
        {
            // �巡�� ���� �������� ������ ���� ���
            if (enableQuantitySelection)
            {
                // ���� ���� �г� ǥ��
                ShowQuantitySelection(id);
            }
            else
            {
                // ���� ������� ���� ����
                BuyItem(id);
            }
        }
        else
        {
            // �巡�� ���� �������� ������ �Ǹ� ���
            SellItem();
        }

        Show();
    }

    private void ShowQuantitySelection(int id)
    {
        if (trading != null)
        {
            trading.ShowQuantitySelection(id);
        }
    }

    private void BuyItem(int id)
    {
        trading.BuyItem(id);
    }

    private void SellItem()
    {
        trading.SellItem();
    }
}
