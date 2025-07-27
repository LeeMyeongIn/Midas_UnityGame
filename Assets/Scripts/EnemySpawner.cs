using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("스폰 설정")]
    [SerializeField] private List<GameObject> enemyPrefabs;
    [SerializeField] private float spawnInterval = 1f;
    [SerializeField] private int maxEnemies = 5;

    [Header("스폰 영역")]
    [SerializeField] private BoxCollider2D spawnArea;

    private List<GameObject> currentEnemies = new List<GameObject>();

    private void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            currentEnemies.RemoveAll(e => e == null);

            if (currentEnemies.Count < maxEnemies)
            {
                GameObject prefab = GetRandomEnemyPrefab();
                Vector2 spawnPos = GetRandomPositionInArea();

                GameObject enemy = Instantiate(prefab, spawnPos, Quaternion.identity);
                currentEnemies.Add(enemy);
            }
        }
    }

    private GameObject GetRandomEnemyPrefab()
    {
        return enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];
    }

    private Vector2 GetRandomPositionInArea()
    {
        Bounds bounds = spawnArea.bounds;

        float x = Random.Range(bounds.min.x, bounds.max.x);
        float y = Random.Range(bounds.min.y, bounds.max.y);

        return new Vector2(x, y);
    }
}
