using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryInitializer : MonoBehaviour
{
    public ItemContainer inventoryContainer;
    public ItemList itemList;

    private void Start()
    {
        StartCoroutine(DelayedLoadInventory());
    }

    private IEnumerator DelayedLoadInventory()
    {
        yield return new WaitForSeconds(0.1f);
        int slot = SelectedSlotHolder.slotNumber;
        Debug.Log("불러오는 슬롯 번호: " + slot);
        InventorySaveManager.LoadInventory(inventoryContainer, itemList, slot);
    }
}