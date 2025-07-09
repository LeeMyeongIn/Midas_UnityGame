using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Trading : MonoBehaviour
{
    [SerializeField] GameObject storePanel;
    [SerializeField] GameObject inventoryPanel;
    [SerializeField] WanderingMerchantSummonAction wanderingMerchantAction;

    [Header("Quantity Selection UI")]
    [SerializeField] GameObject quantitySelectionPanel;
    [SerializeField] Text itemNameText;
    [SerializeField] Text quantityText;
    [SerializeField] Text totalPriceText;
    [SerializeField] Button increaseButton;
    [SerializeField] Button decreaseButton;
    [SerializeField] Button increase10Button;
    [SerializeField] Button decrease10Button;
    [SerializeField] Button confirmPurchaseButton;
    [SerializeField] Button cancelPurchaseButton;
    [SerializeField] Button maxButton;
    [SerializeField] Button resetButton;
    [SerializeField] Image itemIconImage;
    [Header("Gold Icon")]
    [SerializeField] Image goldIconImage;

    Store store;
    Currency money;
    ItemStorePanel itemStorePanel;
    [SerializeField] ItemContainer playerInventory;
    [SerializeField] ItemPanel inventoryItemPanel;

    int selectedQuantity = 1;
    int maxPurchasableQuantity = 1;
    Item currentSelectedItem;
    int currentItemSlotId;
    bool isTradingWithWanderingMerchant = false;
    bool isSelling = false;

    private void Awake()
    {
        money = GetComponent<Currency>();
        itemStorePanel = storePanel.GetComponent<ItemStorePanel>();
        SetupButtonEvents();
    }

    public float GetCurrentSellToPlayerMultip()
    {
        return store != null ? store.sellToPlayerMultip : 1.5f;
    }

    public float GetCurrentBuyFromPlayerMultip()
    {
        return store != null ? store.buyFromPlayerMultip : 0.5f;
    }

    public bool IsCurrentlyTrading()
    {
        return store != null;
    }
    private void SetupButtonEvents()
    {
        if (increaseButton != null)
        {
            increaseButton.onClick.RemoveAllListeners();
            increaseButton.onClick.AddListener(IncreaseQuantity);
        }
        if (decreaseButton != null)
        {
            decreaseButton.onClick.RemoveAllListeners();
            decreaseButton.onClick.AddListener(DecreaseQuantity);
        }
        if (increase10Button != null)
        {
            increase10Button.onClick.RemoveAllListeners();
            increase10Button.onClick.AddListener(Increase10Quantity);
        }
        if (decrease10Button != null)
        {
            decrease10Button.onClick.RemoveAllListeners();
            decrease10Button.onClick.AddListener(Decrease10Quantity);
        }
        if (maxButton != null)
        {
            maxButton.onClick.RemoveAllListeners();
            maxButton.onClick.AddListener(SetMaxQuantity);
        }
        if (resetButton != null)
        {
            resetButton.onClick.RemoveAllListeners();
            resetButton.onClick.AddListener(ResetQuantity);
        }
        if (confirmPurchaseButton != null)
        {
            confirmPurchaseButton.onClick.RemoveAllListeners();
            confirmPurchaseButton.onClick.AddListener(ConfirmPurchase);
        }
        if (cancelPurchaseButton != null)
        {
            cancelPurchaseButton.onClick.RemoveAllListeners();
            cancelPurchaseButton.onClick.AddListener(CancelQuantitySelection);
        }
    }
    public void SetMaxQuantity()
    {
        selectedQuantity = maxPurchasableQuantity;
        UpdateQuantitySelectionUI();
    }

    public void ResetQuantity()
    {
        selectedQuantity = 0;
        UpdateQuantitySelectionUI();
    }

    public void BeginTrading(Store store)
    {
        this.store = store;
        itemStorePanel.SetInventory(store.storeContent);
        storePanel.SetActive(true);
        inventoryPanel.SetActive(true);
        CheckIfWanderingMerchant();
        StartCoroutine(SetupButtonEventsDelayed());
    }

    private IEnumerator SetupButtonEventsDelayed()
    {
        yield return new WaitForEndOfFrame();
        SetupButtonEvents();
    }

    private void CheckIfWanderingMerchant()
    {
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

        if (quantitySelectionPanel != null)
            quantitySelectionPanel.SetActive(false);
    }

    public void SellItem()
    {
        if (GameManager.instance.dragAndDropController.CheckForSale() == true)
        {
            ItemSlot itemToSell = GameManager.instance.dragAndDropController.itemSlot;
            if (itemToSell.item.stackable && itemToSell.count > 1)
            {
                ShowSellQuantitySelection(itemToSell);
            }
            else
            {
                ExecuteSell(itemToSell, itemToSell.count);
            }
        }
    }

    private void ShowSellQuantitySelection(ItemSlot itemSlot)
    {
        currentSelectedItem = itemSlot.item;
        selectedQuantity = 1;
        isSelling = true;
        maxPurchasableQuantity = itemSlot.count;
        UpdateQuantitySelectionUI();

        if (quantitySelectionPanel != null)
            quantitySelectionPanel.SetActive(true);

        StartCoroutine(SetupButtonEventsDelayed());
    }

    private void ExecuteSell(ItemSlot itemSlot, int sellCount)
    {
        int moneyGain = (int)(itemSlot.item.price * sellCount * store.buyFromPlayerMultip);
        money.Add(moneyGain);

        if (sellCount >= itemSlot.count)
        {
            itemSlot.Clear();
        }
        else
        {
            itemSlot.count -= sellCount;
        }

        GameManager.instance.dragAndDropController.UpdateIcon();
    }

    public void ShowQuantitySelection(int id)
    {
        if (store == null || store.storeContent.slots[id].item == null)
            return;

        currentSelectedItem = store.storeContent.slots[id].item;
        currentItemSlotId = id;
        selectedQuantity = 1;
        isSelling = false;

        CalculateMaxPurchasableQuantity();
        UpdateQuantitySelectionUI();

        if (quantitySelectionPanel != null)
            quantitySelectionPanel.SetActive(true);

        StartCoroutine(SetupButtonEventsDelayed());
    }

    private void CalculateMaxPurchasableQuantity()
    {
        if (currentSelectedItem == null)
        {
            maxPurchasableQuantity = 0;
            return;
        }

        int unitPrice = (int)(currentSelectedItem.price * store.sellToPlayerMultip);
        int maxByMoney = 0;
        if (unitPrice > 0)
        {
            int testAmount = 0;
            while (money.Check(unitPrice * (testAmount + 1)) && testAmount < 9999)
            {
                testAmount++;
            }
            maxByMoney = testAmount;
        }

        if (isTradingWithWanderingMerchant)
        {
            maxPurchasableQuantity = maxByMoney;
        }
        else
        {
            int storeStock = store.storeContent.slots[currentItemSlotId].count;
            if (storeStock > 0)
            {
                maxPurchasableQuantity = Mathf.Min(maxByMoney, storeStock);
            }
            else
            {
                maxPurchasableQuantity = maxByMoney;
            }
        }

        if (maxByMoney > 0)
        {
            maxPurchasableQuantity = Mathf.Max(1, maxPurchasableQuantity);
        }
        else
        {
            maxPurchasableQuantity = 0;
        }
    }

    private void UpdateQuantitySelectionUI()
    {
        if (currentSelectedItem == null) return;

        int unitPrice, totalPrice;

        if (isSelling)
        {
            unitPrice = (int)(currentSelectedItem.price * store.buyFromPlayerMultip);
            totalPrice = unitPrice * selectedQuantity;
        }
        else
        {
            unitPrice = (int)(currentSelectedItem.price * store.sellToPlayerMultip);
            totalPrice = unitPrice * selectedQuantity;
        }

        if (itemNameText != null)
            itemNameText.text = currentSelectedItem.name;
        if (quantityText != null)
            quantityText.text = $"물품 수량: " + selectedQuantity.ToString();
        if (totalPriceText != null)
            totalPriceText.text = isSelling ? $"총 판매 금액: {totalPrice}" : $"총 구매 금액: {totalPrice}" ;

        if (goldIconImage != null)
        {
            goldIconImage.gameObject.SetActive(true);
        }

        UpdateItemIcon();
        UpdateButtonStates();
    }

    private void UpdateItemIcon()
    {
        if (itemIconImage != null && currentSelectedItem != null)
        {
            // 아이템에 스프라이트가 있는 경우
            if (currentSelectedItem.icon != null)
            {
                itemIconImage.sprite = currentSelectedItem.icon;
                itemIconImage.enabled = true;
            }
            else
            {
                // 아이콘이 없는 경우 기본 이미지 또는 비활성화
                itemIconImage.enabled = false;
            }
        }
    }

    private void UpdateButtonStates()
    {
        bool canIncrease = selectedQuantity < maxPurchasableQuantity;
        bool canDecrease = selectedQuantity > 0;

        if (increaseButton != null)
            increaseButton.interactable = canIncrease;

        if (decreaseButton != null)
            decreaseButton.interactable = canDecrease;

        if (increase10Button != null)
            increase10Button.interactable = canIncrease;

        // decrease10Button을 항상 활성화 (수량이 0보다 클 때만)
        if (decrease10Button != null)
            decrease10Button.interactable = canDecrease;

        // maxButton 수정: 현재 수량이 최대값이 아니거나 최대값이 0보다 클 때 활성화
        if (maxButton != null)
            maxButton.interactable = maxPurchasableQuantity > 0 && selectedQuantity != maxPurchasableQuantity;

        // resetButton: 현재 수량이 0이 아닐 때 활성화
        if (resetButton != null)
            resetButton.interactable = selectedQuantity > 0;

        bool canConfirm = false;
        if (confirmPurchaseButton != null)
        {
            if (isSelling)
            {
                canConfirm = selectedQuantity > 0; // 판매 시 수량이 0보다 클 때만 확인 가능
            }
            else
            {
                int totalPrice = (int)(currentSelectedItem.price * store.sellToPlayerMultip * selectedQuantity);
                canConfirm = money.Check(totalPrice) && selectedQuantity > 0; // 구매 시 수량이 0보다 클 때만 확인 가능
            }
            confirmPurchaseButton.interactable = canConfirm;
        }
    }

    public void IncreaseQuantity()
    {
        if (selectedQuantity < maxPurchasableQuantity)
        {
            selectedQuantity++;
            UpdateQuantitySelectionUI();
        }
    }

    public void DecreaseQuantity()
    {
        if (selectedQuantity > 0)
        {
            selectedQuantity--;
            UpdateQuantitySelectionUI();
        }
    }

    public void Increase10Quantity()
    {
        int newQuantity = selectedQuantity + 10;
        selectedQuantity = Mathf.Min(newQuantity, maxPurchasableQuantity);
        UpdateQuantitySelectionUI();
    }

    public void Decrease10Quantity()
    {
        int newQuantity = selectedQuantity - 10;
        selectedQuantity = Mathf.Max(newQuantity, 0);
        UpdateQuantitySelectionUI();
    }

    public void ConfirmPurchase()
    {
        if (currentSelectedItem == null) return;

        if (isSelling)
        {
            ItemSlot itemToSell = GameManager.instance.dragAndDropController.itemSlot;
            ExecuteSell(itemToSell, selectedQuantity);
        }
        else
        {
            int totalPrice = (int)(currentSelectedItem.price * store.sellToPlayerMultip * selectedQuantity);

            if (money.Check(totalPrice))
            {
                money.Decrease(totalPrice);

                for (int i = 0; i < selectedQuantity; i++)
                {
                    playerInventory.Add(currentSelectedItem);
                }

                inventoryItemPanel.Show();

                if (isTradingWithWanderingMerchant && IsRecipeItem(currentSelectedItem))
                {
                    WanderingMerchantSummonAction.MarkRecipeAsPurchased(currentSelectedItem);
                }
            }
        }

        CancelQuantitySelection();
    }

    public void CancelQuantitySelection()
    {
        if (quantitySelectionPanel != null)
            quantitySelectionPanel.SetActive(false);

        if (itemIconImage != null)
        {
            itemIconImage.sprite = null;
            itemIconImage.enabled = false;
        }
        if (goldIconImage != null)
        {
            goldIconImage.gameObject.SetActive(false);
        }

        currentSelectedItem = null;
        currentItemSlotId = -1;
        selectedQuantity = 1;
        isSelling = false;
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
                WanderingMerchantSummonAction.MarkRecipeAsPurchased(itemToBuy);
            }
        }
    }

    private bool IsRecipeItem(Item item)
    {
        if (wanderingMerchantAction == null || item == null)
        {
            return false;
        }
        return wanderingMerchantAction.IsRecipeItemDirect(item);
    }
}
