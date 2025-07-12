using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMapReadController : MonoBehaviour
{
    [SerializeField] Tilemap tilemap;
    //tile������ ������ �� ������ �������� ����
    private void Start()
    {
        tilemap = GameObject.Find("BaseTilemap").GetComponent<Tilemap>();
        Debug.Log("[DEBUG] TileMapReadController���� tilemap ���� ����: BaseTilemap");
    }

    public CropsManager cropsManager;
    public PlaceableObjectsReferenceManager objectsManager;

    //������Ŭ�� ����
    [SerializeField] private GameObject sprinklerPrefab;
    [SerializeField] private Item sprinklerItem;
    [SerializeField] private ToolbarController toolbarController;

    //������Ŭ�� �ܰ躰 item ����
    [SerializeField] private Item sprinklerL1Item;
    [SerializeField] private Item sprinklerL2Item;
    [SerializeField] private Item sprinklerL3Item;

    //������Ŭ�� �ܰ躰 ��������Ʈ ����
    [SerializeField] private Sprite sprinkler_Wood;
    [SerializeField] private Sprite sprinkler_Silver;
    [SerializeField] private Sprite sprinkler_Gold;

    //hammer
    [SerializeField] private Item hammerItem;

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
            //ī�޶���� �Ÿ� ����
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

    //�ܵ��
    public void DebugMouseGridPosition()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int grid = tilemap.WorldToCell(mousePos);
        Debug.Log($"���� ���콺 ��ġ Ÿ�� ��ǥ: {grid}");
    }

    //������Ŭ�� ��ġ
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))  // ���콺 ��Ŭ�� ��ġ
        {
            Vector2 mousePos = Input.mousePosition;
            OnTileClicked(mousePos);
        }
    }
    public Item GetSprinklerItemByLevel(int level)
    {
        if (level == 1) return sprinklerL1Item;
        if (level == 2) return sprinklerL2Item;
        if (level == 3) return sprinklerL3Item;
        return null;
    }

    public void OnTileClicked(Vector2 clickPosition)
    {
        Vector3Int gridPosition = GetGridPosition(clickPosition, true);
        Item currentItem = toolbarController.GetItem;

        // ������Ŭ�� ��ġ ó��
        if (currentItem != null && currentItem != hammerItem)
        {
            int level = 0;

            if (currentItem == sprinklerL1Item) level = 1;
            else if (currentItem == sprinklerL2Item) level = 2;
            else if (currentItem == sprinklerL3Item) level = 3;

            if (level > 0)
            {
                bool installed = InstallSprinkler(gridPosition, level);
                if (installed)
                {
                    InventoryController.Instance.RemoveItem(currentItem, 1);
                }
            }
        }
    }
    private bool InstallSprinkler(Vector3Int gridPosition, int level)
    {
        TilemapCropsManager cropsManager = GameObject.FindObjectOfType<TilemapCropsManager>();
        if (cropsManager != null && cropsManager.IsBlockedArea(gridPosition))
        {
            Debug.Log($"[InstallSprinkler] ������ �����Դϴ�: {gridPosition}");
            return false;  // ��ġ ����
        }

        Vector3 worldPosition = tilemap.CellToWorld(gridPosition) + new Vector3(0.5f, 0.5f, 0f);
        GameObject obj = Instantiate(sprinklerPrefab, worldPosition, Quaternion.identity);
        Sprinkler sprinkler = obj.GetComponent<Sprinkler>();
        sprinkler.centerPosition = gridPosition;
        sprinkler.upgradeLevel = level;

        SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            if (level == 1) sr.sprite = sprinkler_Wood;
            if (level == 2) sr.sprite = sprinkler_Silver;
            if (level == 3) sr.sprite = sprinkler_Gold;
        }

        Debug.Log($"[TileMapReadController] ������Ŭ�� ��ġ�� at {gridPosition} / Level {level}");
        return true;  // ��ġ ����
    }

    public Tilemap GetTilemap()
    {
        return tilemap;
    }
}
