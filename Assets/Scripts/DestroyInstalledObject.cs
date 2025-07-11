using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "ToolAction/Destroy Installed Object")]
public class DestroyInstalledObject : ToolAction
{
    public override bool OnApply(Vector2 worldPoint)
    {
        Vector3Int gridPos = GameManager.instance.tileMapReadController.GetGridPosition(Input.mousePosition, true);

        Item item = GameObject.FindObjectOfType<ToolsCharacterController>().GetCurrentItem();
        return OnApplyToTileMap(gridPos, GameManager.instance.tileMapReadController, item);
    }

    public override bool OnApplyToTileMap(Vector3Int gridPosition, TileMapReadController tileMapReadController, Item item)
    {
        if (item == null || item.id != 304) //hammer id = 304
        {
            Debug.LogWarning($"[DestroyInstalledObject] item.id = {item?.id} → 작업 불가");
            return false;
        }

        Debug.Log($"[DestroyInstalledObject] 제거 시도 at {gridPosition}");

        Sprinkler targetSprinkler = null;

        foreach (Sprinkler sprinkler in GameObject.FindObjectsOfType<Sprinkler>())
        {
            if (sprinkler.centerPosition.Equals(gridPosition))
            {
                targetSprinkler = sprinkler;
                break;
            }
        }

        if (targetSprinkler != null)
        {
            TilemapCropsManager cropsManager = tileMapReadController.cropsManager.cropsManager;
            cropsManager?.UnregisterSprinkler(targetSprinkler);
            GameObject.Destroy(targetSprinkler.gameObject);

            Item sprinklerItem = tileMapReadController.GetSprinklerItemByLevel(targetSprinkler.upgradeLevel);
            if (sprinklerItem != null)
            {
                ItemSpawnManager.instance.SpawnItem(
                    tileMapReadController.GetTilemap().CellToWorld(gridPosition),
                    sprinklerItem,
                    1
                );
            }

            return true;
        }

        Debug.LogWarning($"[DestroyInstalledObject] 제거할 스프링클러 없음 at {gridPosition}");
        return false;
    }
}
