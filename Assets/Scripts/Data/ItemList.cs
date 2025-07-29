using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ItemList : ScriptableObject
{
    public List<Item> items;

    public Item GetItemById(int id)
    {
        return items.Find(item => item.id == id);
    }
}
