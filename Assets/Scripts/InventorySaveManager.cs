using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public static class InventorySaveManager
{
    private static string GetSavePath(int slot)
    {
        return Path.Combine(Application.persistentDataPath, $"inventory_slot{slot}.json");
    }

    public static void SaveInventory(ItemContainer container, int slot)
    {
        Debug.Log("SaveInventory 호출됨 - 슬롯: " + slot);
        Debug.Log("슬롯 수: " + container.slots.Count);
        string path = GetSavePath(slot);

        InventorySaveData saveData = new InventorySaveData();

        foreach (var slotData in container.slots)
        {
            if (slotData.item != null)
            {
                saveData.slots.Add(new InventorySlotSaveData
                {
                    itemId = slotData.item.id,
                    count = slotData.count
                });
            }
            else
            {
                saveData.slots.Add(new InventorySlotSaveData
                {
                    itemId = -1,
                    count = 0
                });
            }
        }

        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(path, json);
        Debug.Log($"[슬롯 {slot} 인벤토리 저장 완료] {path}");
    }

    public static void LoadInventory(ItemContainer container, ItemList itemList, int slot)
    {
        string path = GetSavePath(slot);

        if (!File.Exists(path))
        {
            Debug.LogWarning($"슬롯 {slot} 저장 파일이 없습니다.");
            return;
        }

        string json = File.ReadAllText(path);
        InventorySaveData data = JsonUtility.FromJson<InventorySaveData>(json);

        for (int i = 0; i < container.slots.Count && i < data.slots.Count; i++)
        {
            var s = data.slots[i];

            if (s.itemId >= 0)
                container.slots[i].Set(itemList.GetItemById(s.itemId), s.count);
            else
                container.slots[i].Clear();
        }

        container.isDirty = true;
        Debug.Log($"[슬롯 {slot} 인벤토리 불러오기 완료]");

        Debug.Log($"[슬롯 {slot}] 불러오기 완료: 인벤토리 첫 칸 = " +
    (container.slots[0].item != null ? container.slots[0].item.name : "빈칸"));
    }
}
