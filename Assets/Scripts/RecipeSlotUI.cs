using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RecipeSlotUI : MonoBehaviour
{
    [Header("UI ���")]
    public Image recipeIconImage;
    public Transform ingredientParentTransform;
    public GameObject ingredientUIPrefab;
    public GameObject lockOverlayObject;

    private CookRecipe currentRecipe;
    private bool recipeUnlocked;

    public void Initialize(CookRecipe recipe, bool isUnlocked)
    {
        Debug.Log($"[RecipeSlotUI] Initialize ȣ���: {recipe.recipeName} / �رݵ�: {isUnlocked}");

        currentRecipe = recipe;
        recipeUnlocked = isUnlocked;

        if (recipeIconImage == null || lockOverlayObject == null || ingredientUIPrefab == null || ingredientParentTransform == null)
        {
            Debug.LogError("[RecipeSlotUI] �ʼ� ������Ʈ�� ������� �ʾҽ��ϴ�!");
            return;
        }

        // ������ ����
        recipeIconImage.sprite = recipe.recipeIcon;

        // ��� �������� ó��
        lockOverlayObject.SetActive(!isUnlocked);
        Debug.Log($"[RecipeSlotUI] LockOverlay {(isUnlocked ? "��Ȱ��ȭ" : "Ȱ��ȭ")}��");

        // ���� ��� ���� ���� (��ø ����)
        foreach (Transform child in ingredientParentTransform)
        {
            Destroy(child.gameObject);
        }

        // ��� ���� �߰�
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

        // ��ư ������ �ʱ�ȭ �� ���� (�ߺ� ����)
        var button = GetComponent<Button>();
        button.onClick.RemoveAllListeners();

        if (recipeUnlocked)
        {
            button.onClick.AddListener(OnCookClicked);
            Debug.Log($"[RecipeSlotUI] AddListener �����: {recipe.recipeName}");
        }
    }

    private void OnCookClicked()
    {
        Debug.Log($"[RecipeSlotUI] OnCookClicked ȣ���: {currentRecipe.recipeName}");

        if (!recipeUnlocked)
        {
            Debug.LogWarning("[RecipeSlotUI] �����ǰ� �رݵ��� �ʾҽ��ϴ�!");
            return;
        }

        var inventory = GameManager.instance.inventoryContainer;
        if (inventory == null)
        {
            Debug.LogError("[RecipeSlotUI] GameManager�� inventoryContainer�� ������� �ʾҽ��ϴ�!");
            return;
        }

        // ��� ���� ���� Ȯ��
        foreach (var ing in currentRecipe.ingredients)
        {
            int owned = inventory.GetItemCount(ing.item);
            if (owned < ing.amount)
            {
                Debug.LogWarning($"[RecipeSlotUI] ��� ����: {ing.item.Name} ({owned}/{ing.amount})");
                return;
            }
        }

        // ��� ����
        foreach (var ing in currentRecipe.ingredients)
        {
            inventory.Remove(ing.item, ing.amount);
            Debug.Log($"[RecipeSlotUI] ��� ������: {ing.item.Name} x{ing.amount}");
        }

        // ����� �߰�
        inventory.Add(currentRecipe.resultItem, 1);
        inventory.isDirty = true;

        Debug.Log($"[RecipeSlotUI] �丮 �Ϸ�: {currentRecipe.recipeName} �� �κ��丮�� �߰���");
    }
}
