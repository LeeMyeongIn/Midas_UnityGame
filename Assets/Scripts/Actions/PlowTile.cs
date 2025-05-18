using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName ="Data/ToolAction/Plow")]

public class PlowTile : ToolAction
{
    [SerializeField] List<TileBase> canPlow;
    [SerializeField] AudioClip onPlowUsed;

    public override bool OnApplyToTileMap(Vector3Int gridPosition, 
        TileMapReadController tileMapReadController, 
        Item item)
    {
        Debug.Log($"[DEBUG] PlowTile.OnApplyToTileMap 호출됨: {gridPosition}");

        TileBase tileToPlow = tileMapReadController.GetTileBase(gridPosition);
        Debug.Log($"[DEBUG] 현재 타일: {tileToPlow}");

        if (canPlow.Contains(tileToPlow) == false)
        {
            Debug.Log($"[DEBUG] 현재 클릭한 타일 이름: {tileToPlow?.name}");
            Debug.LogWarning("[DEBUG] 이 타일은 쟁기질할 수 없습니다.");
            return false;
        }

        tileMapReadController.cropsManager.Plow(gridPosition);
        AudioManager.instance.Play(onPlowUsed);
        return true;
    }
}
