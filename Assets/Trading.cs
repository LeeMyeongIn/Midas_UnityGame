using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Trading : MonoBehaviour
{
    [SerializeField] GameObject storePanel;
    [SerializeField] GameObject inventoryPanel;

    Store store;

    Currency money;

    private void Awake()
    {
        money= GetComponent<Currency>();
    }

    public void BeginTrading(Store store)
    {
        this.store = store;
      
        storePanel.SetActive(true);
        inventoryPanel.SetActive(true);
    }
    
    public void StopTrading()
    {
        store = null;

        storePanel.SetActive(false);
        inventoryPanel.SetActive(false);
    }

    public void SellItem()
    {
        if (GameManager.instance.dragAndDropController.CheckForSale() == true)
        {
            ItemSlot itemToSell = GameManager.instance.dragAndDropController.itemSlot;
            int moneyGain = itemToSell.item.stackable == true ? 
                itemToSell.item.price * itemToSell.count : //total money gain if item is stackable
                itemToSell.item.price; //total money if item is not stackable
            money.Add(moneyGain);
            itemToSell.Clear();
            GameManager.instance.dragAndDropController.UpdateIcon();
        }
    }
}
