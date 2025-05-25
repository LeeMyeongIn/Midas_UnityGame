using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Data/ToolAction/Remove Plowing")]
public class RemovePlowing : ToolAction
{
    public override bool OnApply(Vector2 worldPoint)
    {
        Vector3Int gridPos = GameManager.instance.tileMapReadController.GetGridPosition(Input.mousePosition, true);

        Item item = GameObject.FindObjectOfType<ToolsCharacterController>().GetCurrentItem();
        return OnApplyToTileMap(gridPos, GameManager.instance.tileMapReadController, item);
    }

    public override bool OnApplyToTileMap(Vector3Int gridPosition, TileMapReadController tileMapReadController, Item item)
    {
        if (item == null || item.id != 4444)
        {
            Debug.LogWarning($"[RemovePlowing] item.id = {item?.id} → 이 작업을 수행할 수 없습니다.");
            return false;
        }

        Debug.Log($"[RemovePlowing] 밭 타일 제거 시도 at {gridPosition}");

        TilemapCropsManager cropsManager = tileMapReadController.cropsManager.cropsManager;
        if (cropsManager == null || cropsManager.container == null)
        {
            Debug.LogWarning("[RemovePlowing] cropsManager 또는 container 없음");
            return false;
        }

        //현재 타일이 쟁기질된 상태가 아닐 경우 무시
        TileBase currentTile = cropsManager.GetTilemap().GetTile(gridPosition);
        if (currentTile != cropsManager.plowedTile && currentTile != cropsManager.seeded && currentTile != cropsManager.watered)
        {
            Debug.LogWarning($"[RemovePlowing] {gridPosition} 위치는 밭이 아닙니다.");
            return false;
        }

        // CropTile 가져오기
        CropTile tile = cropsManager.container.Get(gridPosition);

        //작물 정보 초기화
        if (tile != null)
        {
            tile.crop = null;
            tile.growStage = 0;
            tile.growTimer = 0;
            tile.damage = 0;
            tile.isWatered = false;

            //스프라이트 제거
            if (tile.renderer != null)
            {
                tile.renderer.sprite = null;
                tile.renderer.gameObject.SetActive(false);
            }

            //croptile도 container에서 제거
            cropsManager.container.crops.Remove(tile);
        }

        //타일맵에서 타일 제거 (밭 -> 맨땅)
        Tilemap tilemap = cropsManager.GetTilemap();
        tilemap.SetTile(gridPosition, cropsManager.baseSoilTile);
        tilemap.RefreshTile(gridPosition);

        return true;
    }
}
