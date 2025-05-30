using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolbarController : MonoBehaviour
{
    [SerializeField] int toolbarSize = 12;
    int selectedTool;

    public Action<int> onChange;
    [SerializeField] IconHighlight iconHighlight;

    public ItemSlot GetItemSlot
    {
        get
        {
            return GameManager.instance.inventoryContainer.slots[selectedTool];
        }
    }

    public Item GetItem
    {
        get
        {
            return GameManager.instance.inventoryContainer.slots[selectedTool].item;
        }
    }

    private void Start()
    {
        onChange += UpdateHighlightIcon;
        UpdateHighlightIcon(selectedTool);
    }

    private void Update()
    {
        HandleScroll();
        HandleRightClick();
    }

    private void HandleScroll()
    {
        float delta = Input.mouseScrollDelta.y;
        if (delta != 0)
        {
            if (delta > 0)
            {
                selectedTool += 1;
                selectedTool = (selectedTool >= toolbarSize ? 0 : selectedTool);
            }
            else
            {
                selectedTool -= 1;
                selectedTool = (selectedTool < 0 ? toolbarSize - 1 : selectedTool);
            }
            onChange?.Invoke(selectedTool);
        }
    }

    private void HandleRightClick()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Item item = GetItem;
            if (item != null && item.onItemUsed != null)
            {
                item.onItemUsed.OnItemUsed(item, GameManager.instance.inventoryContainer);
            }
        }
    }

    internal void Set(int id)
    {
        selectedTool = id;
    }

    public void UpdateHighlightIcon(int id = 0)
    {
        Item item = GetItem;
        if (item == null)
        {
            iconHighlight.Show = false;
            return;
        }

        iconHighlight.Show = item.iconHighlight;
        if (item.iconHighlight)
        {
            iconHighlight.Set(item.icon);
        }
    }
}
