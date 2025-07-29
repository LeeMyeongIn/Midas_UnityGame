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

        Currency currency = GameObject.FindObjectOfType<Currency>();
        if (currency != null)
        {
            saveData.gold = currency.CurrentGold;
        }
        else
        {
            Debug.LogWarning("Currency 컴포넌트를 찾을 수 없습니다.");
            saveData.gold = 0;
        }

        string path = Path.Combine(Application.persistentDataPath, $"inventory_slot{slot}.json");
        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(path, json);

        Debug.Log($"[저장 완료] 슬롯 {slot}, 돈: {saveData.gold}");
    }


    public static void LoadInventory(ItemContainer container, ItemList database, int slot)
    {
        string path = Path.Combine(Application.persistentDataPath, $"inventory_slot{slot}.json");

        if (!File.Exists(path))
        {
            Debug.LogWarning($"[불러오기 실패] 파일 없음: {path}");
            return;
        }

        string json = File.ReadAllText(path);
        InventorySaveData data = JsonUtility.FromJson<InventorySaveData>(json);

        for (int i = 0; i < container.slots.Count && i < data.slots.Count; i++)
        {
            var slotData = data.slots[i];
            if (slotData.itemId >= 0)
            {
                Item item = database.items.Find(i => i.id == slotData.itemId);
                container.slots[i].Set(item, slotData.count);
            }
            else
            {
                container.slots[i].Clear();
            }
        }

        Currency currency = GameObject.FindObjectOfType<Currency>();
        if (currency != null)
        {
            currency.SetGold(data.gold);
            Debug.Log($"[돈 불러오기 완료] {data.gold}G");
        }
        else
        {
            Debug.LogWarning("Currency 컴포넌트를 찾을 수 없습니다. 돈 불러오기 실패");
        }
        container.isDirty = true;
    }
}
