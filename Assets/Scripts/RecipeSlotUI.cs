using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RecipeSlotUI : MonoBehaviour
{
    [Header("UI 요소")]
    public Image recipeIconImage;
    public Transform ingredientParentTransform;
    public GameObject ingredientUIPrefab;
    public GameObject lockOverlayObject;

    private CookRecipe currentRecipe;
    private bool recipeUnlocked;

    public void Initialize(CookRecipe recipe, bool isUnlocked)
    {
        Debug.Log($"[RecipeSlotUI] Initialize 호출됨: {recipe.recipeName} / 해금됨: {isUnlocked}");

        currentRecipe = recipe;
        recipeUnlocked = isUnlocked;

        if (recipeIconImage == null || lockOverlayObject == null || ingredientUIPrefab == null || ingredientParentTransform == null)
        {
            Debug.LogError("[RecipeSlotUI] 필수 컴포넌트가 연결되지 않았습니다!");
            return;
        }

        // 아이콘 설정
        recipeIconImage.sprite = recipe.recipeIcon;

        // 잠금 오버레이 처리
        lockOverlayObject.SetActive(!isUnlocked);
        Debug.Log($"[RecipeSlotUI] LockOverlay {(isUnlocked ? "비활성화" : "활성화")}됨");

        // 기존 재료 슬롯 삭제 (중첩 방지)
        foreach (Transform child in ingredientParentTransform)
        {
            Destroy(child.gameObject);
        }

        // 재료 슬롯 추가
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

        // 버튼 리스너 초기화 후 재등록 (중복 방지)
        var button = GetComponent<Button>();
        button.onClick.RemoveAllListeners();

        if (recipeUnlocked)
        {
            button.onClick.AddListener(OnCookClicked);
            Debug.Log($"[RecipeSlotUI] AddListener 연결됨: {recipe.recipeName}");
        }
    }

    private void OnCookClicked()
    {
        Debug.Log($"[RecipeSlotUI] OnCookClicked 호출됨: {currentRecipe.recipeName}");

        if (!recipeUnlocked)
        {
            Debug.LogWarning("[RecipeSlotUI] 레시피가 해금되지 않았습니다!");
            return;
        }

        var inventory = GameManager.instance.inventoryContainer;
        if (inventory == null)
        {
            Debug.LogError("[RecipeSlotUI] GameManager에 inventoryContainer가 연결되지 않았습니다!");
            return;
        }

        // 재료 보유 여부 확인
        foreach (var ing in currentRecipe.ingredients)
        {
            int owned = inventory.GetItemCount(ing.item);
            if (owned < ing.amount)
            {
                Debug.LogWarning($"[RecipeSlotUI] 재료 부족: {ing.item.Name} ({owned}/{ing.amount})");
                return;
            }
        }

        // 재료 차감
        foreach (var ing in currentRecipe.ingredients)
        {
            inventory.Remove(ing.item, ing.amount);
            Debug.Log($"[RecipeSlotUI] 재료 차감됨: {ing.item.Name} x{ing.amount}");
        }

        // 결과물 추가
        inventory.Add(currentRecipe.resultItem, 1);
        inventory.isDirty = true;

        Debug.Log($"[RecipeSlotUI] 요리 완료: {currentRecipe.recipeName} → 인벤토리에 추가됨");
    }
}
