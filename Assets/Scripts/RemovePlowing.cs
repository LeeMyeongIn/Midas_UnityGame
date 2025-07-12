using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Data/ToolAction/Remove Plowing")]
public class RemovePlowing : ToolAction
{
    [SerializeField] int toolLevel = 1;
    public Vector2 lastMotionVector;

    public override bool OnApply(Vector2 worldPoint)
    {
        Vector3Int gridPos = GameManager.instance.tileMapReadController.GetGridPosition(Input.mousePosition, true);

        Item item = GameObject.FindObjectOfType<ToolsCharacterController>().GetCurrentItem();
        return OnApplyToTileMap(gridPos, GameManager.instance.tileMapReadController, item);
    }

    public override bool OnApplyToTileMap(Vector3Int gridPosition, TileMapReadController tileMapReadController, Item item)
    {
        if (item == null || item.id != 302)
        {
            Debug.LogWarning($"[RemovePlowing] item.id = {item?.id} → 이 작업을 수행할 수 없습니다.");
            return false;
        }

        Debug.Log($"[RemovePlowing] RemovePlowing 시작 at {gridPosition}, toolLevel={toolLevel}");

        TilemapCropsManager cropsManager = tileMapReadController.cropsManager.cropsManager;
        if (cropsManager == null || cropsManager.container == null)
        {
            Debug.LogWarning("[RemovePlowing] cropsManager 또는 container 없음");
            return false;
        }

        bool anyRemoved = false;

        foreach (Vector3Int pos in GetTargetPositions(gridPosition, toolLevel))
        {
            TileBase currentTile = cropsManager.GetTilemap().GetTile(pos);
            if (currentTile != cropsManager.plowedTile && currentTile != cropsManager.seeded && currentTile != cropsManager.watered)
            {
                Debug.Log($"[RemovePlowing] {pos} → 밭 아님, 무시");
                continue;
            }

            CropTile tile = cropsManager.container.Get(pos);

            if (tile != null)
            {
                tile.crop = null;
                tile.growStage = 0;
                tile.growTimer = 0;
                tile.damage = 0;
                tile.isWatered = false;

                if (tile.renderer != null)
                {
                    tile.renderer.sprite = null;
                    tile.renderer.gameObject.SetActive(false);
                }

                cropsManager.container.crops.Remove(tile);
            }

            cropsManager.GetTilemap().SetTile(pos, cropsManager.baseSoilTile);
            cropsManager.GetTilemap().RefreshTile(pos);
            anyRemoved = true;
        }

        return anyRemoved;
    }

    private List<Vector3Int> GetTargetPositions(Vector3Int clicked, int level)
    {
        List<Vector3Int> positions = new List<Vector3Int>();

        Vector3Int dir = Vector3Int.down;

        if (lastMotionVector.x > 0.5f)
            dir = Vector3Int.right;
        else if (lastMotionVector.x < -0.5f)
            dir = Vector3Int.left;
        else if (lastMotionVector.y > 0.5f)
            dir = Vector3Int.up;
        else if (lastMotionVector.y < -0.5f)
            dir = Vector3Int.down;

        if (level == 1)
        {
            positions.Add(clicked);
        }
        else if (level == 2)
        {
            for (int i = 0; i < 3; i++)
            {
                positions.Add(clicked + dir * i);
            }
        }
        else if (level == 3)
        {
            if (dir == Vector3Int.up || dir == Vector3Int.down)
            {
                for (int dx = -1; dx <= 1; dx++)
                {
                    for (int dy = 0; dy <= 2; dy++)
                    {
                        positions.Add(clicked + new Vector3Int(dx, dir.y * dy, 0));
                    }
                }
            }
            else if (dir == Vector3Int.left || dir == Vector3Int.right)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    for (int dx = 0; dx <= 2; dx++)
                    {
                        positions.Add(clicked + new Vector3Int(dir.x * dx, dy, 0));
                    }
                }
            }
        }

        return positions;
    }
}
