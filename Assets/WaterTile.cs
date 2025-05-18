using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Data/ToolAction/Water")]
public class WaterTile : ToolAction
{
    [SerializeField] List<TileBase> canWater;
    [SerializeField] AudioClip onWaterUsed;

    public override bool OnApplyToTileMap(Vector3Int gridPosition, TileMapReadController tileMapReadController, Item item)
    {
        TileBase tile = tileMapReadController.GetTileBase(gridPosition);

        if (tile == null)
        {
            Debug.LogWarning("[WaterTile] ���� ��ġ�� Ÿ���� �����ϴ�.");
            return false;
        }

        //�񱳴� �̸�����!
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
            Debug.Log($"[DEBUG] ���� Ÿ��: {tile.name}");
            Debug.LogWarning("[DEBUG] �� Ÿ�Ͽ��� ���� �� �� �����ϴ�.");
            return false;
        }

        tileMapReadController.cropsManager.Water(gridPosition);
        AudioManager.instance.Play(onWaterUsed);
        return true;
    }
}