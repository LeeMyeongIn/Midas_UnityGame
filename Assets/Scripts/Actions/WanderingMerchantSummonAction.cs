using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Tools/WanderingMerchantSummonAction")]
public class WanderingMerchantSummonAction : ToolAction
{
    [SerializeField] private List<Item> possibleItems = new List<Item>();
    [SerializeField] private List<Item> recipeItems = new List<Item>();
    [SerializeField] private int minItemsToSell = 3;
    [SerializeField] private int maxItemsToSell = 6;

    [Header("쿨타임 설정")]
    [SerializeField] private float cooldownTime = 300f;

    [Header("UI 설정")]
    [SerializeField] private GameObject cooltimePanelPrefab;

    private static HashSet<int> purchasedRecipeIds = new HashSet<int>();
    private static float lastUseTime = -1f;

    private static GameObject cooltimePanel;
    private static Text cooltimeText;
    private static Button closeButton;
    private static Canvas uiCanvas;

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
                    Debug.LogError("WanderingMerchantSummonAction을 찾을 수 없습니다! Resources 폴더에 있는지 확인하세요.");
                }
            }
            return instance;
        }
    }

    private void OnEnable()
    {
        instance = this;
    }

    private void InitializeUI()
    {
        if (cooltimePanel == null)
        {
            GameObject taggedPanel = GameObject.FindWithTag("CooltimePanel");
            if (taggedPanel != null)
            {
                cooltimePanel = taggedPanel;
                Debug.Log("태그로 CooltimePanel 찾음");
            }
            else
            {
                cooltimePanel = FindInactiveObjectByName("CooltimePanel");
                if (cooltimePanel != null)
                {
                    Debug.Log("이름으로 CooltimePanel 찾음");
                }
            }

            if (cooltimePanel == null && cooltimePanelPrefab != null)
            {
                if (uiCanvas == null)
                {
                    uiCanvas = FindObjectOfType<Canvas>();
                }

                if (uiCanvas != null)
                {
                    cooltimePanel = Instantiate(cooltimePanelPrefab, uiCanvas.transform);
                    Debug.Log("프리팹으로 CooltimePanel 생성");
                }
            }

            if (cooltimePanel == null)
            {
                CreateCooltimePanel();
            }

            if (cooltimePanel != null)
            {
                SetupUIComponents();
            }
        }
    }

    private GameObject FindInactiveObjectByName(string name)
    {
        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (obj.name == name && obj.scene.IsValid())
            {
                return obj;
            }
        }
        return null;
    }

    private void CreateCooltimePanel()
    {
        if (uiCanvas == null)
        {
            uiCanvas = FindObjectOfType<Canvas>();
            if (uiCanvas == null)
            {
                Debug.LogError("Canvas를 찾을 수 없습니다! UI를 생성할 수 없습니다.");
                return;
            }
        }

        cooltimePanel = new GameObject("CooltimePanel");
        cooltimePanel.transform.SetParent(uiCanvas.transform, false);

        RectTransform panelRect = cooltimePanel.AddComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        Image panelImage = cooltimePanel.AddComponent<Image>();
        panelImage.color = new Color(0, 0, 0, 0.8f);

        GameObject textObj = new GameObject("CooltimeText");
        textObj.transform.SetParent(cooltimePanel.transform, false);

        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchoredPosition = Vector2.zero;
        textRect.sizeDelta = new Vector2(400, 100);

        cooltimeText = textObj.AddComponent<Text>();
        cooltimeText.text = "Cooltime!";
        cooltimeText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        cooltimeText.fontSize = 24;
        cooltimeText.color = Color.white;
        cooltimeText.alignment = TextAnchor.MiddleCenter;

        GameObject buttonObj = new GameObject("CloseButton");
        buttonObj.transform.SetParent(cooltimePanel.transform, false);

        RectTransform buttonRect = buttonObj.AddComponent<RectTransform>();
        buttonRect.anchoredPosition = new Vector2(0, -50);
        buttonRect.sizeDelta = new Vector2(100, 40);

        Image buttonImage = buttonObj.AddComponent<Image>();
        buttonImage.color = Color.gray;

        closeButton = buttonObj.AddComponent<Button>();

        GameObject buttonTextObj = new GameObject("ButtonText");
        buttonTextObj.transform.SetParent(buttonObj.transform, false);

        RectTransform buttonTextRect = buttonTextObj.AddComponent<RectTransform>();
        buttonTextRect.anchorMin = Vector2.zero;
        buttonTextRect.anchorMax = Vector2.one;
        buttonTextRect.offsetMin = Vector2.zero;
        buttonTextRect.offsetMax = Vector2.zero;

        Text buttonText = buttonTextObj.AddComponent<Text>();
        buttonText.text = "Close";
        buttonText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        buttonText.fontSize = 16;
        buttonText.color = Color.white;
        buttonText.alignment = TextAnchor.MiddleCenter;

        Debug.Log("쿨타임 패널을 동적으로 생성했습니다.");
    }

    private void SetupUIComponents()
    {
        if (cooltimeText == null)
        {
            cooltimeText = cooltimePanel.GetComponentInChildren<Text>();
        }

        if (closeButton == null)
        {
            closeButton = cooltimePanel.GetComponentInChildren<Button>();
        }

        if (closeButton != null)
        {
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(CloseCooldownPanel);
        }

        cooltimePanel.SetActive(false);

        Debug.Log("쿨타임 UI 초기화 완료!");
    }

    public override void OnItemUsed(Item usedItem, ItemContainer inventory)
    {
        if (IsOnCooldown())
        {
            ShowCooldownPanel();
            return;
        }

        lastUseTime = Time.time;

        WanderingMerchantManager.Instance.SummonMerchant(5f, 10f);
        WanderingMerchantManager.Instance.StartCoroutine(SetRandomStoreItemsDelayed());

        if (inventory != null)
        {
            inventory.Remove(usedItem);
        }
        else
        {
            Debug.LogWarning("인벤토리가 null입니다. 아이템을 제거할 수 없습니다.");
        }
    }

    private bool IsOnCooldown()
    {
        if (lastUseTime < 0) return false;

        float timeSinceLastUse = Time.time - lastUseTime;
        return timeSinceLastUse < cooldownTime;
    }

    private float GetRemainingCooldown()
    {
        if (lastUseTime < 0) return 0f;

        float timeSinceLastUse = Time.time - lastUseTime;
        return Mathf.Max(0f, cooldownTime - timeSinceLastUse);
    }

    private void ShowCooldownPanel()
    {
        float remainingTime = GetRemainingCooldown();
        Debug.Log($"떠돌이 상인 소환 쿨타임 중! 남은 시간: {Mathf.Ceil(remainingTime)}초");
    }

    private IEnumerator AutoCloseCooldownPanel(float delay)
    {
        yield return new WaitForSeconds(delay);
        CloseCooldownPanel();
    }

    private void CloseCooldownPanel()
    {
        if (cooltimePanel != null)
        {
            cooltimePanel.SetActive(false);
        }
    }

    public void SetCooldownTime(float newCooldownTime)
    {
        cooldownTime = newCooldownTime;
    }

    public void ResetCooldown()
    {
        lastUseTime = -1f;
    }

    public bool IsCooldownActive()
    {
        return IsOnCooldown();
    }

    public float GetCooldownTimeRemaining()
    {
        return GetRemainingCooldown();
    }

    public static void ResetUIReferences()
    {
        cooltimePanel = null;
        cooltimeText = null;
        closeButton = null;
        uiCanvas = null;
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
            Debug.LogError("상인 오브젝트가 없습니다!");
            return;
        }

        Store merchantStore = WanderingMerchantManager.Instance.wanderingMerchantObject.GetComponent<Store>();
        if (merchantStore == null)
        {
            Debug.LogError("상인 오브젝트에 Store 컴포넌트가 없습니다!");
            return;
        }

        if (merchantStore.storeContent == null)
        {
            Debug.LogError("상인의 storeContent가 null입니다!");
            return;
        }

        ClearStoreContent(merchantStore.storeContent);

        List<Item> availableItems = GetAvailableItems();

        if (availableItems.Count == 0)
        {
            Debug.LogWarning("판매 가능한 아이템이 없습니다!");
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
            Debug.LogError("MarkRecipeAsPurchased: recipe가 null입니다!");
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
            Debug.LogWarning("IsRecipeItemDirect: item이 null입니다.");
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

    [ContextMenu("Reset Cooldown")]
    public void ResetCooldownMenu()
    {
        ResetCooldown();
    }

    [ContextMenu("Reset UI References")]
    public void ResetUIReferencesMenu()
    {
        ResetUIReferences();
    }

    private void ClearStoreContent(ItemContainer storeContent)
    {
        if (storeContent == null || storeContent.slots == null)
        {
            Debug.LogError("StoreContent 또는 slots가 null입니다!");
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

    public static float GetLastUseTime()
    {
        return lastUseTime;
    }

    public static void SetLastUseTime(float time)
    {
        lastUseTime = time;
    }
}



