using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Trading : MonoBehaviour
{
    [SerializeField] GameObject storePanel;
    [SerializeField] GameObject inventoryPanel;
    [SerializeField] WanderingMerchantSummonAction wanderingMerchantAction; // Inspector에서 할당

    Store store;
    Currency money;
    ItemStorePanel itemStorePanel;
    [SerializeField] ItemContainer playerInventory;
    [SerializeField] ItemPanel inventoryItemPanel;

    // 현재 떠돌이 상인과 거래 중인지 확인하는 변수
    private bool isTradingWithWanderingMerchant = false;

    private void Awake()
    {
        money = GetComponent<Currency>();
        itemStorePanel = storePanel.GetComponent<ItemStorePanel>();
    }

    public void BeginTrading(Store store)
    {
        this.store = store;
        itemStorePanel.SetInventory(store.storeContent);
        storePanel.SetActive(true);
        inventoryPanel.SetActive(true);

        // 떠돌이 상인인지 확인 (WanderingMerchantManager를 통해 확인)
        CheckIfWanderingMerchant();
    }

    private void CheckIfWanderingMerchant()
    {
        // 현재 거래하는 상점이 떠돌이 상인의 상점인지 확인
        if (WanderingMerchantManager.Instance != null &&
            WanderingMerchantManager.Instance.wanderingMerchantObject != null)
        {
            Store wanderingMerchantStore = WanderingMerchantManager.Instance.wanderingMerchantObject.GetComponent<Store>();
            isTradingWithWanderingMerchant = (wanderingMerchantStore == this.store);
        }
        else
        {
            isTradingWithWanderingMerchant = false;
        }
    }

    public void StopTrading()
    {
        store = null;
        isTradingWithWanderingMerchant = false;
        storePanel.SetActive(false);
        inventoryPanel.SetActive(false);
    }

    public void SellItem()
    {
        if (GameManager.instance.dragAndDropController.CheckForSale() == true)
        {
            ItemSlot itemToSell = GameManager.instance.dragAndDropController.itemSlot;
            int moneyGain = itemToSell.item.stackable == true ?
                (int)(itemToSell.item.price * itemToSell.count * store.buyFromPlayerMultip) : //total money gain if item is stackable
                (int)(itemToSell.item.price * store.buyFromPlayerMultip); //total money if item is not stackable
            money.Add(moneyGain);
            itemToSell.Clear();
            GameManager.instance.dragAndDropController.UpdateIcon();
        }
    }

    internal void BuyItem(int id)
    {
        Item itemToBuy = store.storeContent.slots[id].item;
        int totalPrice = (int)(itemToBuy.price * store.sellToPlayerMultip);

        if (money.Check(totalPrice) == true)
        {
            money.Decrease(totalPrice);
            playerInventory.Add(itemToBuy);
            inventoryItemPanel.Show();

            if (isTradingWithWanderingMerchant && IsRecipeItem(itemToBuy))
            {
                WanderingMerchantSummonAction.MarkRecipeAsPurchased(itemToBuy); // 핵심 호출
            }
        }
    }


    private bool IsRecipeItem(Item item)
    {
        // 직접 참조된 WanderingMerchantAction을 사용
        if (wanderingMerchantAction == null || item == null)
        {
            Debug.LogWarning($"IsRecipeItem: wanderingMerchantAction={wanderingMerchantAction}, item={item}");
            return false;
        }

        // wanderingMerchantAction의 recipeItems에서 확인
        return wanderingMerchantAction.IsRecipeItemDirect(item);
    }
}
