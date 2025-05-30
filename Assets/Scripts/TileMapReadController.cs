using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMapReadController : MonoBehaviour
{
    [SerializeField] Tilemap tilemap;
    public CropsManager cropsManager;
    public PlaceableObjectsReferenceManager objectsManager;

    public Vector3Int GetGridPosition(Vector2 position, bool mousePosition)
    {
        if (tilemap == null)
        {
            tilemap = GameObject.Find("BaseTilemap").GetComponent<Tilemap>();
        }

        if (tilemap == null) { return Vector3Int.zero; }

        Vector3 worldPosition;

        if (mousePosition)
        {
            //카메라와의 거리 보정
            Vector3 screenPosition = new Vector3(position.x, position.y, Mathf.Abs(Camera.main.transform.position.z));
            worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
            worldPosition.z = 0f;
        }
        else
        {
            worldPosition = position;
        }

        Vector3Int gridPosition = tilemap.WorldToCell(worldPosition);

        return gridPosition;
    }

    public TileBase GetTileBase(Vector3Int gridPosition)
    {
        if (tilemap == null)
        {
            tilemap = GameObject.Find("BaseTilemap").GetComponent<Tilemap>();
        }

        if (tilemap == null) { return null; }

        TileBase tile = tilemap.GetTile(gridPosition);
        return tile;
    }

    //잔디용
    public void DebugMouseGridPosition()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int grid = tilemap.WorldToCell(mousePos);
        Debug.Log($"현재 마우스 위치 타일 좌표: {grid}");
    }
}
