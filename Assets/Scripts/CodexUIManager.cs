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
    [SerializeField] private Transform monsterListParent;
    [SerializeField] private Transform dropItemListParent;
    [SerializeField] private GameObject dropItemEntryPrefab;
    [SerializeField] private List<GameObject> allMonsterPrefabs;

    [Header("몬스터 드랍 아이템 표시용")]
    [SerializeField] public List<Item> MonsterDropItems;

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
            CodexUI.SetActive(true);

        ShowFoodPanel();
    }

    public void CloseCodex()
    {
        if (CodexUI != null)
            CodexUI.SetActive(false);
    }

    public void ShowFoodPanel()
    {
        CodexFoodPanel?.SetActive(true);
        CodexCropPanel?.SetActive(false);
        CodexTriumphPanel?.SetActive(false);
        CodexMonsterPanel?.SetActive(false);

        RefreshFoodCodex();
    }

    public void ShowCropPanel()
    {
        CodexFoodPanel?.SetActive(false);
        CodexCropPanel?.SetActive(true);
        CodexTriumphPanel?.SetActive(false);
        CodexMonsterPanel?.SetActive(false);

        RefreshCropCodex();
    }

    public void ShowTriumphPanel()
    {
        CodexFoodPanel?.SetActive(false);
        CodexCropPanel?.SetActive(false);
        CodexTriumphPanel?.SetActive(true);
        CodexMonsterPanel?.SetActive(false);

        triumphPanelManager?.RefreshUI();
    }

    public void ShowMonsterPanel()
    {
        CodexFoodPanel?.SetActive(false);
        CodexCropPanel?.SetActive(false);
        CodexTriumphPanel?.SetActive(false);
        CodexMonsterPanel?.SetActive(true);

        RefreshMonsterCodex();
        RefreshDropItemCodex();
    }

    private void RefreshFoodCodex()
    {
        foreach (Transform child in recipeListParent)
            Destroy(child.gameObject);

        foreach (var recipe in allRecipes)
        {
            GameObject go = Instantiate(recipeEntryPrefab, recipeListParent);
            CodexRecipeEntry entry = go.GetComponent<CodexRecipeEntry>();

            bool isUnlocked = RecipeUnlockManager.Instance?.IsUnlocked(recipe.recipeId) ?? false;
            bool isCooked = RecipeUnlockManager.Instance?.IsCooked(recipe.recipeId) ?? false;

            entry.Initialize(recipe, isUnlocked, isCooked);
        }
    }

    private void RefreshCropCodex()
    {
        foreach (Transform child in cropListParent)
            Destroy(child.gameObject);

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
            if (monsterPrefab == null) continue;

            MonsterInfo info = monsterPrefab.GetComponent<MonsterInfo>();
            if (info == null) continue;

            GameObject go = Instantiate(monsterEntryPrefab, monsterListParent);
            CodexMonsterEntry entry = go.GetComponent<CodexMonsterEntry>();

            bool isSeen = MonsterUnlockManager.Instance.HasSeen(info.MonsterId);
            entry.Initialize(info, isSeen, null);
        }
    }

    private void RefreshDropItemCodex()
    {
        foreach (Transform child in dropItemListParent)
            Destroy(child.gameObject);

        foreach (var item in MonsterDropItems)
        {
            bool hasSeen = MonsterDropCodexTracker.Instance.HasSeen(item.id.ToString());

            GameObject go = Instantiate(dropItemEntryPrefab, dropItemListParent);
            CodexDropItemEntry entry = go.GetComponent<CodexDropItemEntry>();
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
