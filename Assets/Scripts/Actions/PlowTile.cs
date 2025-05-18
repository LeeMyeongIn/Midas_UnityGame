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
        Debug.Log($"[DEBUG] PlowTile.OnApplyToTileMap ȣ���: {gridPosition}");

        TileBase tileToPlow = tileMapReadController.GetTileBase(gridPosition);
        Debug.Log($"[DEBUG] ���� Ÿ��: {tileToPlow}");

        if (canPlow.Contains(tileToPlow) == false)
        {
            Debug.Log($"[DEBUG] ���� Ŭ���� Ÿ�� �̸�: {tileToPlow?.name}");
            Debug.LogWarning("[DEBUG] �� Ÿ���� ������� �� �����ϴ�.");
            return false;
        }

        tileMapReadController.cropsManager.Plow(gridPosition);
        AudioManager.instance.Play(onPlowUsed);
        return true;
    }
}
