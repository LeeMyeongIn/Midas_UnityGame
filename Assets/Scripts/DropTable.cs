using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Monster/DropTable")]
public class DropTable : ScriptableObject
{
    [System.Serializable]
    public class DropEntry
    {
        public GameObject dropPrefab;
        [Range(0f, 1f)] public float dropChance = 0.5f; // È®·ü (0.0~1.0)
    }

    public DropEntry[] entries;

    public GameObject GetOneGuaranteedDrop()
    {
        float totalChance = 0;
        foreach (var entry in entries)
            totalChance += entry.dropChance;

        float roll = Random.Range(0f, totalChance);
        float sum = 0;

        foreach (var entry in entries)
        {
            sum += entry.dropChance;
            if (roll <= sum)
                return entry.dropPrefab;
        }

        return null;
    }

}
