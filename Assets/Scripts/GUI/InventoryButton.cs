using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] Image icon;
    [SerializeField] TMPro.TextMeshProUGUI text;
    [SerializeField] Image highlight;

    int myIndex;
    ItemPanel itemPanel;
    ItemSlot currentSlot;

    public void SetIndex(int index)
    {
        myIndex = index;
    }

    public void SetItemPanel(ItemPanel source)
    {
        itemPanel = source;
    }

    public void Set(ItemSlot slot)
    {
        currentSlot = slot;

        icon.gameObject.SetActive(true);
        icon.sprite = slot.item.icon;

        if (slot.item.stackable)
        {
            text.gameObject.SetActive(true);
            text.text = slot.count.ToString();
        }
        else
        {
            text.gameObject.SetActive(false);
        }
    }
    public void Clean()
    {
        currentSlot = null;
        icon.sprite = null;
        icon.gameObject.SetActive(false);
        text.gameObject.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (currentSlot != null && currentSlot.item != null)
            {
                if (currentSlot.item is RecipePaperItem recipeItem)
                {
                    recipeItem.UseRecipe();

                    currentSlot.count--;
                    if (currentSlot.count <= 0)
                        currentSlot.Clear();

                    itemPanel.inventory.isDirty = true;
                    return;
                }

                if (currentSlot.item.onItemUsed != null)
                {
                    Vector2 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    currentSlot.item.onItemUsed.OnApply(worldPos);
                }
            }
        }
        else
        {
            itemPanel.OnClick(myIndex);
        }
    }

    public void Highlight(bool b)
    {
        highlight.gameObject.SetActive(b);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (currentSlot != null && currentSlot.item != null)
        {
            ItemTooltipManager.Instance.ShowTooltip(currentSlot.item);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ItemTooltipManager.Instance.HideTooltip();
    }
}
