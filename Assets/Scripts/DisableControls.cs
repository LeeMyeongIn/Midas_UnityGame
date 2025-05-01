using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableControls : MonoBehaviour
{
    CharacterController2D characterController;
    ToolsCharacterController toolsChacter;
    InventoryController inventoryController;
    ToolbarController toolbarController;
    ItemContainerInteractController itemContainerInteractController;
    private void Awake()
    {
        characterController = GetComponent<CharacterController2D>();
        toolsChacter = GetComponent<ToolsCharacterController>();
        inventoryController = GetComponent<InventoryController>();
        toolbarController = GetComponent<ToolbarController>();
        itemContainerInteractController = GetComponent<ItemContainerInteractController>();
    }

    public void DisableControl()
    {
        characterController.enabled = false;
        toolsChacter.enabled = false;
        inventoryController.enabled = false;
        toolbarController.enabled = false;
        itemContainerInteractController.enabled = false;
    }

    public void EnableControl()
    {
        characterController.enabled = true;
        toolsChacter.enabled = true;
        inventoryController.enabled = true;
        toolbarController.enabled = true;
        itemContainerInteractController.enabled = true;
    }
}
