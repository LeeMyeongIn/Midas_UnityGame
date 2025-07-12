using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Data/ToolAction/Plow")]
public class PlowTile : ToolAction
{
    [SerializeField] List<TileBase> canPlow;
    [SerializeField] AudioClip onPlowUsed;

    [SerializeField] int toolLevel = 1;  // 1~3�ܰ� ���׷��̵� ����

    public Vector2 lastMotionVector;

    public override bool OnApplyToTileMap(Vector3Int gridPosition,
     TileMapReadController tileMapReadController,
     Item item)
    {
        Debug.Log($"[DEBUG] PlowTile.OnApplyToTileMap ȣ���: {gridPosition}, toolLevel={toolLevel}");

        bool anyPlowed = false;

        foreach (Vector3Int pos in GetTargetPositions(gridPosition, toolLevel))
        {
            TileBase tileToPlow = tileMapReadController.GetTileBase(pos);

            Debug.Log($"[DEBUG] ���� Ÿ�� ��ġ: {pos}, Ÿ��: {tileToPlow}");

            if (tileToPlow == null || !canPlow.Contains(tileToPlow))
            {
                Debug.Log($"[DEBUG] {pos}�� Ÿ�� �̸�: {tileToPlow?.name} �� ����� �Ұ�");
                continue;
            }

            tileMapReadController.cropsManager.Plow(pos);
            anyPlowed = true;
        }

        if (anyPlowed)
        {
            AudioManager.instance.Play(onPlowUsed);
        }

        return anyPlowed;
    }

    private List<Vector3Int> GetTargetPositions(Vector3Int clicked, int level)
    {
        List<Vector3Int> positions = new List<Vector3Int>();

        Vector3Int dir = Vector3Int.down;   //�⺻ ���� : �Ʒ�

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
