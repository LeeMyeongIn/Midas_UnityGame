using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Item")]
public class Item : ScriptableObject
{
    [TextArea]
    public string description;

    public string Name;
    public int id;
    public bool stackable;
    public Sprite icon;
    public ToolAction onAction;
    public ToolAction onTileMapAction;
    public ToolAction onItemUsed;
    public Crop crop;
    public bool iconHighlight;
    public GameObject itemPrefab;
    public bool isWeapon;
    public int damage = 10;
    public int price = 100;
    public bool canBeSold = true;
}
