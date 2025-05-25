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
            Debug.LogWarning($"[RemovePlowing] item.id = {item?.id} �� �� �۾��� ������ �� �����ϴ�.");
            return false;
        }

        Debug.Log($"[RemovePlowing] �� Ÿ�� ���� �õ� at {gridPosition}");

        TilemapCropsManager cropsManager = tileMapReadController.cropsManager.cropsManager;
        if (cropsManager == null || cropsManager.container == null)
        {
            Debug.LogWarning("[RemovePlowing] cropsManager �Ǵ� container ����");
            return false;
        }

        //���� Ÿ���� ������� ���°� �ƴ� ��� ����
        TileBase currentTile = cropsManager.GetTilemap().GetTile(gridPosition);
        if (currentTile != cropsManager.plowedTile && currentTile != cropsManager.seeded && currentTile != cropsManager.watered)
        {
            Debug.LogWarning($"[RemovePlowing] {gridPosition} ��ġ�� ���� �ƴմϴ�.");
            return false;
        }

        // CropTile ��������
        CropTile tile = cropsManager.container.Get(gridPosition);

        //�۹� ���� �ʱ�ȭ
        if (tile != null)
        {
            tile.crop = null;
            tile.growStage = 0;
            tile.growTimer = 0;
            tile.damage = 0;
            tile.isWatered = false;

            //��������Ʈ ����
            if (tile.renderer != null)
            {
                tile.renderer.sprite = null;
                tile.renderer.gameObject.SetActive(false);
            }

            //croptile�� container���� ����
            cropsManager.container.crops.Remove(tile);
        }

        //Ÿ�ϸʿ��� Ÿ�� ���� (�� -> �Ƕ�)
        Tilemap tilemap = cropsManager.GetTilemap();
        tilemap.SetTile(gridPosition, cropsManager.baseSoilTile);
        tilemap.RefreshTile(gridPosition);

        return true;
    }
}
