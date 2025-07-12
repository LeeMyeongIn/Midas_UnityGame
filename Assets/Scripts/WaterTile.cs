using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Data/ToolAction/Water")]
public class WaterTile : ToolAction
{
    [SerializeField] List<TileBase> canWater;
    [SerializeField] AudioClip onWaterUsed;

    [SerializeField] int toolLevel = 1;

    public override bool OnApplyToTileMap(Vector3Int gridPosition, TileMapReadController tileMapReadController, Item item)
    {
        //방향 읽어옴
        Vector2 lookDir = GameManager.instance.toolsCharacterController.characterController2d.lastMotionVector;

        Debug.Log($"[DEBUG] WaterTile.OnApplyToTileMap 호출됨: {gridPosition}, toolLevel={toolLevel}, lookDir={lookDir}");

        bool anyWatered = false;

        foreach (var pos in GetTargetPositions(gridPosition, toolLevel, lookDir))
        {
            TileBase tile = tileMapReadController.GetTileBase(pos);

            if (tile == null)
            {
                Debug.Log($"[DEBUG] {pos} 위치에 타일 없음");
                continue;
            }

            bool match = false;
            foreach (TileBase waterable in canWater)
            {
                if (tile.name == waterable.name)
                {
                    match = true;
                    break;
                }
            }

            if (!match)
            {
                Debug.Log($"[DEBUG] {pos}의 타일 이름: {tile.name} → 물 못 줌");
                continue;
            }

            tileMapReadController.cropsManager.Water(pos);
            anyWatered = true;
        }

        if (anyWatered)
        {
            AudioManager.instance.Play(onWaterUsed);
        }

        return anyWatered;
    }

    private List<Vector3Int> GetTargetPositions(Vector3Int clicked, int level, Vector2 lookDir)
    {
        List<Vector3Int> positions = new List<Vector3Int>();

        Vector3Int dir = Vector3Int.down;

        if (lookDir.x > 0.5f)
            dir = Vector3Int.right;
        else if (lookDir.x < -0.5f)
            dir = Vector3Int.left;
        else if (lookDir.y > 0.5f)
            dir = Vector3Int.up;
        else if (lookDir.y < -0.5f)
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
