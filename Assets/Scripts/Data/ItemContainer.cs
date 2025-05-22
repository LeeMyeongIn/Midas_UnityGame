using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ItemSlot
{
    public Item item;
    public int count;

    public void Copy(ItemSlot slot)
    {
        item = slot.item;
        count = slot.count;
    }

    public void Set(Item item, int count)
    {
        this.item = item;
        this.count = count;
    }

    public void Clear()
    {
        item = null;
        count = 0;
    }
}

[CreateAssetMenu(menuName = "Data/Item Container")]
public class ItemContainer : ScriptableObject
{
    public List<ItemSlot> slots;
    public bool isDirty;

    internal void Init()
    {
        slots = new List<ItemSlot>();
        for (int i = 0; i < 36; i++)
        {
            slots.Add(new ItemSlot());
        }
    }

    public void Add(Item item, int count = 1)
    {
        if (item == null || count <= 0)
            return;

        isDirty = true;

        if (item.stackable)
        {
            ItemSlot existingSlot = slots.Find(x => x.item != null && x.item.id == item.id);
            if (existingSlot != null)
            {
                existingSlot.count += count;
                return;
            }
        }

        foreach (ItemSlot slot in slots)
        {
            if (slot.item == null)
            {
                slot.Set(item, item.stackable ? count : 1);
                return;
            }
        }

        Debug.LogWarning("Inventory full - 아이템을 추가할 수 없습니다.");
    }

    public void Remove(Item itemToRemove, int count = 1)
    {
        if (itemToRemove == null || count <= 0)
            return;

        isDirty = true;

        if (itemToRemove.stackable)
        {
            ItemSlot slot = slots.Find(x => x.item != null && x.item.id == itemToRemove.id);
            if (slot == null) return;

            slot.count -= count;
            if (slot.count <= 0)
            {
                slot.Clear();
            }
        }
        else
        {
            int removed = 0;
            foreach (ItemSlot slot in slots)
            {
                if (slot.item != null && slot.item.id == itemToRemove.id)
                {
                    slot.Clear();
                    removed++;
                    if (removed >= count) break;
                }
            }
        }
    }

    public bool CheckFreeSpace()
    {
        return slots.Exists(slot => slot.item == null);
    }

    public bool CheckItem(ItemSlot checkingItem)
    {
        if (checkingItem == null || checkingItem.item == null)
            return false;

        ItemSlot slot = slots.Find(x => x.item != null && x.item.id == checkingItem.item.id);
        if (slot == null) return false;

        if (checkingItem.item.stackable)
            return slot.count >= checkingItem.count;

        int owned = slots.FindAll(x => x.item != null && x.item.id == checkingItem.item.id).Count;
        return owned >= checkingItem.count;
    }

    public int GetItemCount(Item target)
    {
        if (target == null) return 0;

        int total = 0;
        foreach (var slot in slots)
        {
            if (slot.item != null && slot.item.id == target.id)
            {
                total += slot.count;
            }
        }
        return total;
    }
    public bool HasSpaceFor(Item item)
    {
        if (item == null) return false;

        if (item.stackable)
        {
            if (slots.Exists(slot => slot.item != null && slot.item.id == item.id))
                return true;
        }

        return slots.Exists(slot => slot.item == null);
    }

    public bool HasSpaceFor(List<Item> items)
    {
        int availableSlots = slots.FindAll(s => s.item == null).Count;
        HashSet<int> existingStackables = new HashSet<int>();

        foreach (var slot in slots)
        {
            if (slot.item != null && slot.item.stackable)
            {
                existingStackables.Add(slot.item.id);
            }
        }

        int neededSlots = 0;

        foreach (var item in items)
        {
            if (item.stackable && existingStackables.Contains(item.id))
                continue;

            neededSlots++;
        }

        return neededSlots <= availableSlots;
    }

}
