using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Trading : MonoBehaviour
{
    [SerializeField] GameObject storePanel;
    [SerializeField] GameObject inventoryPanel;
    [SerializeField] WanderingMerchantSummonAction wanderingMerchantAction; // Inspector���� �Ҵ�

    Store store;
    Currency money;
    ItemStorePanel itemStorePanel;
    [SerializeField] ItemContainer playerInventory;
    [SerializeField] ItemPanel inventoryItemPanel;

    // ���� ������ ���ΰ� �ŷ� ������ Ȯ���ϴ� ����
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

        // ������ �������� Ȯ�� (WanderingMerchantManager�� ���� Ȯ��)
        CheckIfWanderingMerchant();
    }

    private void CheckIfWanderingMerchant()
    {
        // ���� �ŷ��ϴ� ������ ������ ������ �������� Ȯ��
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
                WanderingMerchantSummonAction.MarkRecipeAsPurchased(itemToBuy); // �ٽ� ȣ��
            }
        }
    }


    private bool IsRecipeItem(Item item)
    {
        // ���� ������ WanderingMerchantAction�� ���
        if (wanderingMerchantAction == null || item == null)
        {
            Debug.LogWarning($"IsRecipeItem: wanderingMerchantAction={wanderingMerchantAction}, item={item}");
            return false;
        }

        // wanderingMerchantAction�� recipeItems���� Ȯ��
        return wanderingMerchantAction.IsRecipeItemDirect(item);
    }
}
