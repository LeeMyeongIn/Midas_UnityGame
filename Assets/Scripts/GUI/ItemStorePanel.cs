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
    [SerializeField] bool enableQuantitySelection = true; // 수량 선택 기능 활성화/비활성화

    public override void OnClick(int id)
    {
        if (GameManager.instance.dragAndDropController.itemSlot.item == null)
        {
            // 드래그 중인 아이템이 없으면 구매 모드
            if (enableQuantitySelection)
            {
                // 수량 선택 패널 표시
                ShowQuantitySelection(id);
            }
            else
            {
                // 기존 방식으로 단일 구매
                BuyItem(id);
            }
        }
        else
        {
            // 드래그 중인 아이템이 있으면 판매 모드
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
