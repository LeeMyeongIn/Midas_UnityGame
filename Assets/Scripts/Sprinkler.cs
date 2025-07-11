using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sprinkler : MonoBehaviour
{
    public int upgradeLevel = 1;  // 1,2,3 단계
    public Vector3Int centerPosition;

    public int GetRange()
    {
        switch (upgradeLevel)
        {
            case 1: return 1;  // 3x3
            case 2: return 2;  // 5x5
            case 3: return 3;  // 7x7
            default: return 1;
        }
    }

    public List<Vector3Int> GetTilesInRange()
    {
        List<Vector3Int> tiles = new List<Vector3Int>();
        int range = GetRange();

        for (int dx = -range; dx <= range; dx++)
        {
            for (int dy = -range; dy <= range; dy++)
            {
                Vector3Int pos = new Vector3Int(centerPosition.x + dx, centerPosition.y + dy, 0);
                tiles.Add(pos);
            }
        }
        return tiles;
    }
    void Start()
    {
        TilemapCropsManager cropsManager = FindObjectOfType<TilemapCropsManager>();
        if (cropsManager != null)
        {
            cropsManager.RegisterSprinkler(this);
        }
        else
        {
            Debug.LogWarning("[Sprinkler] TilemapCropsManager를 찾을 수 없습니다!");
        }
    }
}

