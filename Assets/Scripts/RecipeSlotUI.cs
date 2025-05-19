using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RecipeSlotUI : MonoBehaviour
{
    [Header("UI 요소")]
    public Image recipeIconImage;
    public TextMeshProUGUI recipeText;
    public Transform ingredientParentTransform;
    public GameObject ingredientUIPrefab;
    public GameObject lockOverlayObject;

    private CookRecipe currentRecipe;
    private bool recipeUnlocked;

    public void Initialize(CookRecipe recipe, bool isUnlocked)
    {
        currentRecipe = recipe;
        recipeUnlocked = isUnlocked;

        if (recipeIconImage == null || recipeText == null || lockOverlayObject == null || ingredientUIPrefab == null || ingredientParentTransform == null)
        {
            Debug.LogWarning("[RecipeSlotUI] UI 요소가 하나 이상 누락되었습니다.");
            return;
        }

        recipeIconImage.sprite = recipe.recipeIcon;
        recipeText.text = recipe.recipeName;

        lockOverlayObject.SetActive(!isUnlocked);

        foreach (Transform child in ingredientParentTransform)
        {
            Destroy(child.gameObject);
        }

        foreach (var ing in recipe.ingredients)
        {
            GameObject go = Instantiate(ingredientUIPrefab, ingredientParentTransform);
            var icon = go.transform.Find("IngredientIcon")?.GetComponent<Image>();
            var amount = go.transform.Find("AmountText")?.GetComponent<TextMeshProUGUI>();

            if (icon != null) icon.sprite = ing.item.icon;

            if (amount != null)
            {
                int owned = GameManager.instance.inventoryContainer.GetItemCount(ing.item);
                amount.text = $"{owned}/{ing.amount}";
                amount.color = (owned < ing.amount) ? Color.red : Color.white;
            }
        }

        var button = GetComponent<Button>();
        button.onClick.RemoveAllListeners();

        if (recipeUnlocked)
        {
            button.onClick.AddListener(OnCookClicked);
        }
    }

    private void OnCookClicked()
    {
        if (!recipeUnlocked) return;

        var inventory = GameManager.instance.inventoryContainer;
        if (inventory == null) return;

        foreach (var ing in currentRecipe.ingredients)
        {
            int owned = inventory.GetItemCount(ing.item);
            if (owned < ing.amount) return;
        }

        foreach (var ing in currentRecipe.ingredients)
        {
            inventory.Remove(ing.item, ing.amount);
        }

        inventory.Add(currentRecipe.resultItem, 1);
        inventory.isDirty = true;

        Debug.Log($"[RecipeSlotUI] 요리 완료: {currentRecipe.recipeName}");
    }
}
