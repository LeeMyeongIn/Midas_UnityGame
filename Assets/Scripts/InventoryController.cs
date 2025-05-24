using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    public static InventoryController Instance;

    [SerializeField] private GameObject panel;
    [SerializeField] private GameObject statusPanel;
    [SerializeField] private GameObject toolbarPanel;
    [SerializeField] private GameObject additionalPanel;
    [SerializeField] private GameObject storePanel;

    [SerializeField] private ItemContainer inventoryContainer;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (!panel.activeInHierarchy)
                Open();
            else
                Close();
        }
    }

    public void Open()
    {
        panel.SetActive(true);
        statusPanel.SetActive(true);
        toolbarPanel.SetActive(false);
        storePanel.SetActive(false);
    }

    public void Close()
    {
        panel.SetActive(false);
        statusPanel.SetActive(false);
        toolbarPanel.SetActive(true);
        additionalPanel.SetActive(false);
        storePanel.SetActive(false);
    }

    public void AddItem(Item item)
    {
        if (item == null)
        {
            Debug.LogWarning("item == null!");
            return;
        }

        if (inventoryContainer == null)
        {
            Debug.LogWarning("ItemContainer가 설정되지 않았습니다.");
            return;
        }

        inventoryContainer.Add(item);
        Debug.Log($"아이템 추가됨: {item.Name}, ID: {item.id}");

        if (IsCrop(item))
        {
            bool isNew = CropSeenManager.Instance.RegisterSeenItem(item.id);
            if (isNew)
            {
                Debug.Log($"[도감] 새 작물 발견: {item.Name}, ID: {item.id}");
            }
            TriumphManager.Instance?.UpdateCropTypeAchievements();
        }
    }

    private bool IsCrop(Item item)
    {
        if (CodexUIManager.Instance == null || CodexUIManager.Instance.allCropItems == null)
            return false;

        foreach (var cropItem in CodexUIManager.Instance.allCropItems)
        {
            if (cropItem != null && cropItem.id == item.id)
                return true;
        }

        return false;
    }


    public bool HasSpace(List<Item> items)
    {
        if (inventoryContainer == null) return false;
        return inventoryContainer.HasSpaceFor(items);
    }
}
