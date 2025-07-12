using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMapReadController : MonoBehaviour
{
    [SerializeField] Tilemap tilemap;
    //tile강제로 지정해 땅 여러개 파지도록 수정
    private void Start()
    {
        tilemap = GameObject.Find("BaseTilemap").GetComponent<Tilemap>();
        Debug.Log("[DEBUG] TileMapReadController에서 tilemap 강제 지정: BaseTilemap");
    }

    public CropsManager cropsManager;
    public PlaceableObjectsReferenceManager objectsManager;

    //스프링클러 참조
    [SerializeField] private GameObject sprinklerPrefab;
    [SerializeField] private Item sprinklerItem;
    [SerializeField] private ToolbarController toolbarController;

    //스프링클러 단계별 item 참조
    [SerializeField] private Item sprinklerL1Item;
    [SerializeField] private Item sprinklerL2Item;
    [SerializeField] private Item sprinklerL3Item;

    //스프링클러 단계별 스프라이트 참조
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

    //스프링클러 설치
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))  // 마우스 좌클릭 설치
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

        // 스프링클러 설치 처리
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
            Debug.Log($"[InstallSprinkler] 금지된 영역입니다: {gridPosition}");
            return false;  // 설치 실패
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

        Debug.Log($"[TileMapReadController] 스프링클러 설치됨 at {gridPosition} / Level {level}");
        return true;  // 설치 성공
    }

    public Tilemap GetTilemap()
    {
        return tilemap;
    }
}
