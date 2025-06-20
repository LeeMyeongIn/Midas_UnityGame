using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tools/WanderingMerchantSummonAction")]
public class WanderingMerchantSummonAction : ToolAction
{
    [SerializeField] private List<Item> possibleItems = new List<Item>();
    [SerializeField] private List<Item> recipeItems = new List<Item>();
    [SerializeField] private int minItemsToSell = 3;
    [SerializeField] private int maxItemsToSell = 6;

    private static HashSet<int> purchasedRecipeIds = new HashSet<int>();

    private static WanderingMerchantSummonAction instance;
    public static WanderingMerchantSummonAction Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Resources.Load<WanderingMerchantSummonAction>("WanderingMerchantSummonAction");
                if (instance == null)
                {
                    Debug.LogError("WanderingMerchantSummonAction�� ã�� �� �����ϴ�! Resources ������ �ִ��� Ȯ���ϼ���.");
                }
            }
            return instance;
        }
    }

    private void OnEnable()
    {
        instance = this;
    }

    public override void OnItemUsed(Item usedItem, ItemContainer inventory)
    {
        WanderingMerchantManager.Instance.SummonMerchant(5f, 10f);
        WanderingMerchantManager.Instance.StartCoroutine(SetRandomStoreItemsDelayed());

        if (inventory != null)
        {
            inventory.Remove(usedItem);
        }
        else
        {
            Debug.LogWarning("�κ��丮�� null�Դϴ�. �������� ������ �� �����ϴ�.");
        }
    }

    private IEnumerator SetRandomStoreItemsDelayed()
    {
        yield return new WaitForSeconds(5.2f);
        SetRandomStoreItems();
    }

    private void SetRandomStoreItems()
    {
        if (WanderingMerchantManager.Instance.wanderingMerchantObject == null)
        {
            Debug.LogError("���� ������Ʈ�� �����ϴ�!");
            return;
        }

        Store merchantStore = WanderingMerchantManager.Instance.wanderingMerchantObject.GetComponent<Store>();
        if (merchantStore == null)
        {
            Debug.LogError("���� ������Ʈ�� Store ������Ʈ�� �����ϴ�!");
            return;
        }

        if (merchantStore.storeContent == null)
        {
            Debug.LogError("������ storeContent�� null�Դϴ�!");
            return;
        }

        ClearStoreContent(merchantStore.storeContent);

        List<Item> availableItems = GetAvailableItems();

        if (availableItems.Count == 0)
        {
            Debug.LogWarning("�Ǹ� ������ �������� �����ϴ�!");
            return;
        }

        int itemsToAdd = Random.Range(minItemsToSell, maxItemsToSell + 1);
        itemsToAdd = Mathf.Min(itemsToAdd, availableItems.Count);
        itemsToAdd = Mathf.Min(itemsToAdd, merchantStore.storeContent.slots.Count);

        List<Item> selectedItems = GetRandomItems(availableItems, itemsToAdd);

        for (int i = 0; i < selectedItems.Count && i < merchantStore.storeContent.slots.Count; i++)
        {
            if (merchantStore.storeContent.slots[i] != null)
            {
                merchantStore.storeContent.slots[i].Set(selectedItems[i], 1);
            }
        }

        merchantStore.storeContent.isDirty = true;
    }

    private List<Item> GetAvailableItems()
    {
        List<Item> availableItems = new List<Item>();
        availableItems.AddRange(possibleItems);

        foreach (Item recipe in recipeItems)
        {
            if (recipe != null && !purchasedRecipeIds.Contains(recipe.id))
            {
                availableItems.Add(recipe);
            }
        }

        return availableItems;
    }

    public static void MarkRecipeAsPurchased(Item recipe)
    {
        if (recipe != null)
        {
            purchasedRecipeIds.Add(recipe.id);
        }
        else
        {
            Debug.LogError("MarkRecipeAsPurchased: recipe�� null�Դϴ�!");
        }
    }

    public static int GetPurchasedRecipeCount()
    {
        return purchasedRecipeIds.Count;
    }

    public bool IsRecipeItemDirect(Item item)
    {
        if (item == null)
        {
            Debug.LogWarning("IsRecipeItemDirect: item�� null�Դϴ�.");
            return false;
        }

        foreach (Item recipe in recipeItems)
        {
            if (recipe != null && recipe.id == item.id)
            {
                return true;
            }
        }

        return false;
    }

    public static bool IsRecipeItem(Item item)
    {
        if (Instance == null || item == null)
        {
            Debug.LogWarning($"IsRecipeItem: Instance={Instance}, item={item}");
            return false;
        }

        return Instance.IsRecipeItemDirect(item);
    }

    public static List<int> GetPurchasedRecipeIds()
    {
        return new List<int>(purchasedRecipeIds);
    }

    public static void LoadPurchasedRecipeIds(List<int> ids)
    {
        purchasedRecipeIds.Clear();
        if (ids != null)
        {
            foreach (int id in ids)
            {
                purchasedRecipeIds.Add(id);
            }
        }
    }

    [ContextMenu("Reset Purchased Recipes")]
    public void ResetPurchasedRecipes()
    {
        purchasedRecipeIds.Clear();
    }

    private void ClearStoreContent(ItemContainer storeContent)
    {
        if (storeContent == null || storeContent.slots == null)
        {
            Debug.LogError("StoreContent �Ǵ� slots�� null�Դϴ�!");
            return;
        }

        foreach (var slot in storeContent.slots)
        {
            if (slot != null)
            {
                slot.Clear();
            }
        }
    }

    private List<Item> GetRandomItems(List<Item> sourceList, int count)
    {
        List<Item> availableItems = new List<Item>(sourceList);
        List<Item> selectedItems = new List<Item>();

        for (int i = 0; i < count && availableItems.Count > 0; i++)
        {
            int randomIndex = Random.Range(0, availableItems.Count);
            selectedItems.Add(availableItems[randomIndex]);
            availableItems.RemoveAt(randomIndex);
        }

        return selectedItems;
    }
}


