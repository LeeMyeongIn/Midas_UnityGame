using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEditor.Progress;

public class PlaceableObjectsManager : MonoBehaviour
{
    [SerializeField] PlaceableObjectsContainer placeableObjects;
    [SerializeField] Tilemap targetTilemap;

    private void Start()
    {
        GameManager.instance.GetComponent<PlaceableObjectsReferenceManager>().placeableObjectsManager = this;
        VisualizeMap();
    }

    private void OnDestroy()
    {
        for(int i = 0; i < placeableObjects.placeableObjects.Count; i++)
        {
            placeableObjects.placeableObjects[i].targetObject = null;
        }
    }

    private void VisualizeMap()
    {
        for(int i=0; i<placeableObjects.placeableObjects.Count; i++)
        {
            VisualizeItem(placeableObjects.placeableObjects[i]);
        }
    }

    private void VisualizeItem(PlaceableObject placeableObject)
    {
        GameObject go = Instantiate(placeableObject.placedItem.itemPrefab);

        Vector3 position = 
            targetTilemap.CellToWorld(placeableObject.positionOnGrid) 
            + targetTilemap.cellSize / 2;

        position -= Vector3.forward * 0.1f;
        go.transform.position = position;

        placeableObject.targetObject = go.transform;
    }

    public void Place(Item item, Vector3Int positionOnGrid)
    {
        PlaceableObject placeableObject = new PlaceableObject(item, positionOnGrid);
        VisualizeItem(placeableObject);
        placeableObjects.placeableObjects.Add(placeableObject);
    }
}
