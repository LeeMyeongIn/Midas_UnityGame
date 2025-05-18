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
            Debug.LogWarning("[WaterTile] 현재 위치에 타일이 없습니다.");
            return false;
        }

        //비교는 이름으로!
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
            Debug.Log($"[DEBUG] 현재 타일: {tile.name}");
            Debug.LogWarning("[DEBUG] 이 타일에는 물을 줄 수 없습니다.");
            return false;
        }

        tileMapReadController.cropsManager.Water(gridPosition);
        AudioManager.instance.Play(onWaterUsed);
        return true;
    }
}