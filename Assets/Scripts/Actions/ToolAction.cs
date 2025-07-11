using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolAction : ScriptableObject
{
    public int energyCost = 0;
    public SkillType skillType;
    public int skillExperienceReward = 100;

    public virtual bool OnApply(Vector2 worldPoint)
    {
        Debug.LogWarning("OnApply is not implemented");
        return true;
    }

    public virtual bool OnApplyToTileMap(Vector3Int gridPosition, 
        TileMapReadController tileMapReadController, 
        Item item)
    {
        Debug.LogWarning("OnApplyToTileMap is not implemented");
        return true;
    }

    public virtual void OnItemUsed(Item usedItem, ItemContainer inventory)
    {
        
    }

    public virtual void OnItemUsed()
    {

    }
}
