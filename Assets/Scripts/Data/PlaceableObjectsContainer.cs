using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]

public class PlaceableObject
{
    public Item placedItem;
    public Transform targetObject;
    public Vector3Int positionOnGrid;

    public PlaceableObject(Item item, Vector3Int pos)
    {
        placedItem = item;
        positionOnGrid = pos;
    }
}

[CreateAssetMenu(menuName ="Data/Pleaceable Objects Container")]
public class PlaceableObjectsContainer : ScriptableObject
{
    public List<PlaceableObject> placeableObjects;
}
