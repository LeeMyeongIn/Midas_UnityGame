using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CodexUIManager : MonoBehaviour
{
    public static CodexUIManager Instance;

    [Header("Codex UI 전체 패널")]
    [SerializeField] private GameObject CodexUI;

    [Header("도감 패널")]
    [SerializeField] private GameObject CodexFoodPanel;
    [SerializeField] private GameObject CodexCropPanel;
    [SerializeField] private GameObject CodexTriumphPanel;
    [SerializeField] private GameObject CodexMonsterPanel;

    [Header("레시피 표시용")]
    [SerializeField] private GameObject recipeEntryPrefab;
    [SerializeField] private Transform recipeListParent;
    [SerializeField] private List<CookRecipe> allRecipes;

    [Header("작물 표시용")]
    [SerializeField] private GameObject cropEntryPrefab;
    [SerializeField] private Transform cropListParent;
    [SerializeField] public List<Item> allCropItems;

    [Header("업적 패널")]
    [SerializeField] private TriumphPanelManager triumphPanelManager;

    [Header("몬스터 표시용")]
    [SerializeField] private GameObject monsterEntryPrefab;
    [SerializeField] private GameObject dropItemEntryPrefab;
    [SerializeField] private Transform monsterListParent;
    [SerializeField] private Transform dropItemListParent;
    [SerializeField] private List<GameObject> allMonsterPrefabs; // 프리팹으로 등록

    [Header("몬스터 드랍 아이템 표시용")]
    [SerializeField] public List<Item> MonsterDropItems; // 드랍 아이템 포함

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        if (CodexUI != null)
            CodexUI.SetActive(false);
    }

    public void OpenCodex()
    {
        if (CodexUI != null)
        {
            CodexUI.SetActive(true);
        }

        ShowFoodPanel();
    }

    public void CloseCodex()
    {
        if (CodexUI != null)
            CodexUI.SetActive(false);
    }

    public void ShowFoodPanel()
    {
        if (CodexFoodPanel != null) CodexFoodPanel.SetActive(true);
        if (CodexCropPanel != null) CodexCropPanel.SetActive(false);
        if (CodexTriumphPanel != null) CodexTriumphPanel.SetActive(false);
        if (CodexMonsterPanel != null) CodexMonsterPanel.SetActive(false);

        RefreshFoodCodex();
    }

    public void ShowCropPanel()
    {
        if (CodexFoodPanel != null) CodexFoodPanel.SetActive(false);
        if (CodexCropPanel != null) CodexCropPanel.SetActive(true);
        if (CodexTriumphPanel != null) CodexTriumphPanel.SetActive(false);
        if (CodexMonsterPanel != null) CodexMonsterPanel.SetActive(false);

        RefreshCropCodex();
    }

    public void ShowTriumphPanel()
    {
        if (CodexFoodPanel != null) CodexFoodPanel.SetActive(false);
        if (CodexCropPanel != null) CodexCropPanel.SetActive(false);
        if (CodexTriumphPanel != null) CodexTriumphPanel.SetActive(true);
        if (CodexMonsterPanel != null) CodexMonsterPanel.SetActive(false);

        if (triumphPanelManager != null)
        {
            triumphPanelManager.RefreshUI();
        }
        else
        {
            Debug.LogWarning("TriumphPanelManager가 연결되지 않았습니다.");
        }
    }

    public void ShowMonsterPanel()
    {
        if (CodexFoodPanel != null) CodexFoodPanel.SetActive(false);
        if (CodexCropPanel != null) CodexCropPanel.SetActive(false);
        if (CodexTriumphPanel != null) CodexTriumphPanel.SetActive(false);
        if (CodexMonsterPanel != null) CodexMonsterPanel.SetActive(true);

        RefreshMonsterCodex();
    }

    private void RefreshFoodCodex()
    {
        foreach (Transform child in recipeListParent)
        {
            Destroy(child.gameObject);
        }

        foreach (var recipe in allRecipes)
        {
            GameObject go = Instantiate(recipeEntryPrefab, recipeListParent);
            CodexRecipeEntry entry = go.GetComponent<CodexRecipeEntry>();

            bool isUnlocked = RecipeUnlockManager.Instance.IsUnlocked(recipe.recipeId);
            bool isCooked = RecipeUnlockManager.Instance.IsCooked(recipe.recipeId);

            entry.Initialize(recipe, isUnlocked, isCooked);
        }
    }

    private void RefreshCropCodex()
    {
        foreach (Transform child in cropListParent)
        {
            Destroy(child.gameObject);
        }

        foreach (var item in allCropItems)
        {
            GameObject go = Instantiate(cropEntryPrefab, cropListParent);
            CodexCropEntry entry = go.GetComponent<CodexCropEntry>();

            bool hasSeen = CropSeenManager.Instance.HasSeenItem(item.id);
            entry.Initialize(item, hasSeen);
        }
    }

    private void RefreshMonsterCodex()
    {
        foreach (Transform child in monsterListParent)
            Destroy(child.gameObject);

        foreach (GameObject monsterPrefab in allMonsterPrefabs)
        {
            MonsterInfo info = monsterPrefab.GetComponent<MonsterInfo>();
            if (info == null)
            {
                Debug.LogWarning($"{monsterPrefab.name}에 MonsterInfo 컴포넌트가 없습니다.");
                continue;
            }

            GameObject go = Instantiate(monsterEntryPrefab, monsterListParent);
            CodexMonsterEntry entry = go.GetComponent<CodexMonsterEntry>();

            bool isSeen = MonsterUnlockManager.Instance.HasSeen(info.MonsterId);
            entry.Initialize(info, isSeen, ShowDropItems);
        }

        foreach (Transform child in dropItemListParent)
            Destroy(child.gameObject);
    }

    public void ShowDropItems(string monsterId)
    {
        foreach (Transform child in dropItemListParent)
            Destroy(child.gameObject);

        List<Item> dropItems = MonsterDropCodexTracker.Instance.GetSeenDrops();

        foreach (var item in dropItems)
        {
            GameObject go = Instantiate(dropItemEntryPrefab, dropItemListParent);
            CodexDropItemEntry entry = go.GetComponent<CodexDropItemEntry>();

            bool hasSeen = HasItem(item.id.ToString());
            entry.Initialize(item, hasSeen);
        }
    }

    public bool HasItem(string itemId)
    {
        foreach (var item in MonsterDropItems)
        {
            if (item.id.ToString() == itemId)
                return true;
        }

        return false;
    }

    public Item GetItemById(string itemId)
    {
        foreach (var item in MonsterDropItems)
        {
            if (item.id.ToString() == itemId)
                return item;
        }

        return null;
    }
}
