using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RecipeSlotUI : MonoBehaviour
{
    public Image recipeIconImage;
    public Transform ingredientParentTransform;
    public GameObject ingredientUIPrefab;
    public GameObject lockOverlayObject;

    private CookRecipe currentRecipe;
    private bool recipeUnlocked;

    public void Initialize(CookRecipe recipe, bool isUnlocked)
    {
        this.currentRecipe = recipe;
        this.recipeUnlocked = isUnlocked;

        recipeIconImage.sprite = recipe.recipeIcon;
        lockOverlayObject.SetActive(!isUnlocked);

        // 재료 목록 초기화
        foreach (Transform child in ingredientParentTransform)
            Destroy(child.gameObject);

        foreach (var ing in recipe.ingredients)
        {
            GameObject go = Instantiate(ingredientUIPrefab, ingredientParentTransform);
            var icon = go.transform.Find("IngredientIcon").GetComponent<Image>();
            var amount = go.transform.Find("AmountText").GetComponent<TextMeshProUGUI>();

            icon.sprite = ing.item.icon;

            int owned = GameManager.instance.inventoryContainer.GetItemCount(ing.item);
            amount.text = $"{owned}/{ing.amount}";
            amount.color = (owned < ing.amount) ? Color.red : Color.white;
        }

        if (recipeUnlocked)
        {
            GetComponent<Button>().onClick.AddListener(OnCookClicked);
        }
    }

    private void OnCookClicked()
    {
        if (!recipeUnlocked) return;

        var inventory = GameManager.instance.inventoryContainer;

        // 1. 재료가 충분한지 확인
        foreach (var ing in currentRecipe.ingredients)
        {
            int owned = inventory.GetItemCount(ing.item);
            if (owned < ing.amount)
            {
                Debug.Log("재료 부족");
                return;
            }
        }

        // 2. 재료 차감
        foreach (var ing in currentRecipe.ingredients)
        {
            inventory.Remove(ing.item, ing.amount);
        }

        // 3. 아이템 들기
        var dragController = FindObjectOfType<ItemDragAndDropController>();

        if (dragController.itemSlot.item == null)
        {
            dragController.itemSlot.Set(currentRecipe.resultItem, 1);
        }
        else if (dragController.itemSlot.item == currentRecipe.resultItem)
        {
            dragController.itemSlot.count += 1;
        }
        else
        {
            Debug.Log("다른 아이템을 들고 있음!");
        }

        dragController.UpdateIcon();

        Debug.Log($"'{currentRecipe.recipeName}' 요리 완료!");
    }
}
