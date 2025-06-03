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

    [Header("레시피 표시용")]
    [SerializeField] private GameObject recipeEntryPrefab;
    [SerializeField] private Transform recipeListParent;
    [SerializeField] private List<CookRecipe> allRecipes;

    [Header("작물 표시용")]
    [SerializeField] private GameObject cropEntryPrefab;
    [SerializeField] private Transform cropListParent;

    [Header("작물 데이터")]
    [SerializeField] public List<Item> allCropItems;

    [Header("업적 패널")]
    [SerializeField] private TriumphPanelManager triumphPanelManager;

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

        RefreshFoodCodex();
    }

    public void ShowCropPanel()
    {
        if (CodexFoodPanel != null) CodexFoodPanel.SetActive(false);
        if (CodexCropPanel != null) CodexCropPanel.SetActive(true);
        if (CodexTriumphPanel != null) CodexTriumphPanel.SetActive(false);

        RefreshCropCodex();
    }

    public void ShowTriumphPanel()
    {
        if (CodexFoodPanel != null) CodexFoodPanel.SetActive(false);
        if (CodexCropPanel != null) CodexCropPanel.SetActive(false);
        if (CodexTriumphPanel != null) CodexTriumphPanel.SetActive(true);

        if (triumphPanelManager != null)
        {
            triumphPanelManager.RefreshUI();
        }
        else
        {
            Debug.LogWarning("TriumphPanelManager가 연결되지 않았습니다.");
        }
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
}
