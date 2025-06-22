using System;
using System.Collections;
using System.Collections.Generic;
//using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

[Serializable]
public class CropTileset
{
    public int growTimer = 0;
    public int growStage = 0;

    public CropTileSet crop = null;
    public SpriteRenderer renderer = null;
    public float damage = 0f;
    public Vector3Int position;
    public bool isWatered = false;

    public bool Complete
    {
        get
        {
            if (crop == null || crop.growthStageTime == null || crop.growthStageTime.Count == 0)
                return false;

            int totalRequiredTime = 0;
            foreach (int t in crop.growthStageTime)
            {
                totalRequiredTime += t;
            }

            return growTimer >= totalRequiredTime;
        }
    }

    public void Harvested()
    {
        growTimer = 0;
        growStage = 0;
        crop = null;
        damage = 0f;
        isWatered = false;

        if (renderer != null)
        {
            renderer.sprite = null;
            renderer.gameObject.SetActive(false);
        }
    }
}

public class TilemapCropsManager : TimeAgent
{
    [SerializeField] public TileBase plowed;
    [SerializeField] public TileBase seeded;

    Tilemap targetTilemap;

    [SerializeField] GameObject cropsSpritePrefab;

    [SerializeField] public CropsContainer container;

    [SerializeField] public TileBase baseSoilTile; //농장 바닥 땅 타일

    //농장, 밭, 물줬을 때 타일
    public TileBase baseSoil;
    public TileBase plowedTile;
    public TileBase watered;

    //새로운 날 일때만 증가하는 조건
    private int lastUpdatedDay = -1;

    public Tilemap GetTilemap()
    {
        return targetTilemap;
    }

    private void Start()
    {
        Debug.Log("[TilemapCropsManager] Start() 실행됨");
        GameManager.instance.GetComponent<CropsManager>().cropsManager = this;
        targetTilemap = GetComponent<Tilemap>();
        onTimeTick += Tick;
        Init();
        VisualizeMap();
    }

    private void VisualizeMap()
    {
        for (int i = 0; i < container.crops.Count; i++)
        {
            VisualizeTile(container.crops[i]);
        }
    }

    private void OnDestroy()
    {
        for (int i = 0; i < container.crops.Count; i++)
        {
            container.crops[i].renderer = null;
        }
    }

    public void Tick(DayTimeController dayTimeController)
    {
        Debug.Log($"[Tick] 호출됨: day = {dayTimeController.days}, season = {dayTimeController.CurrentSeason}");

        if (targetTilemap == null) return;
        if (lastUpdatedDay == dayTimeController.days) return;

        bool isRaining = dayTimeController.weatherManager != null &&
                         dayTimeController.weatherManager.IsRaining;

        Season currentSeason = dayTimeController.CurrentSeason;

        // 비 오는 날 자동 물주기
        if (isRaining)
        {
            foreach (CropTile tile in container.crops)
            {
                tile.isWatered = true;
                VisualizeTile(tile);
            }
        }

        // 작물 처리 루프
        for (int i = container.crops.Count - 1; i >= 0; i--)
        {
            CropTile cropTile = container.crops[i];

            // 계절에 안 맞는 작물 제거
            if (cropTile.crop != null && !cropTile.crop.seasons.Contains(currentSeason))
            {
                Debug.Log($"[Tick] {cropTile.crop.name} → {currentSeason}에 맞지 않아 제거됨");

                if (cropTile.renderer != null)
                    Destroy(cropTile.renderer.gameObject);

                cropTile.crop = null;
                cropTile.growStage = 0;
                cropTile.growTimer = 0;
                cropTile.damage = 0f;
                cropTile.isWatered = false;

                VisualizeTile(cropTile);
                continue;
            }

            // crop이 없는 타일 → 물 상태만 복구
            if (cropTile.crop == null)
            {
                if (!isRaining && cropTile.isWatered)
                {
                    cropTile.isWatered = false;
                    targetTilemap.SetTile(cropTile.position, plowedTile);
                }
                continue;
            }

            //완전히 성장한 작물은 매일 tick 증가 (물 안 줘도 된)
            if (cropTile.Complete)
            {
                cropTile.growTimer++;

                int totalGrowTime = 0;
                foreach (int t in cropTile.crop.growthStageTime)
                    totalGrowTime += t;

                if (cropTile.growTimer >= totalGrowTime + 3)
                {
                    Debug.Log($"[Tick] {cropTile.position} → 수환 안 해서 3일 후 쌍음");

                    if (cropTile.renderer != null)
                        Destroy(cropTile.renderer.gameObject);

                    cropTile.crop = null;
                    cropTile.growStage = 0;
                    cropTile.growTimer = 0;
                    cropTile.damage = 0f;
                    cropTile.isWatered = false;

                    VisualizeTile(cropTile); // 밥은 유지
                }

                continue;
            }

            //자라는 중인데 물 안 줘있으면 성장 머지물
            if (!cropTile.isWatered)
            {
                Debug.Log($"[Tick] {cropTile.position} → 물 안 줘서 성장 머지물");
                continue;
            }

            // 날짜 기반 성장 방식으로 전환
            if (!cropTile.isWatered)
            {
                Debug.Log($"[Tick] {cropTile.position} → 물 안 줘서 성장 멈춤");
                continue;
            }

            cropTile.growTimer++;

            int requiredDays = cropTile.crop.growthStageTime[cropTile.growStage];

            if (cropTile.growTimer >= requiredDays &&
                cropTile.growStage < cropTile.crop.growthStageTime.Count)
            {
                cropTile.growStage++;
                cropTile.growTimer = 0;
                VisualizeTile(cropTile);
            }
        }

        // 비 안 오는 날에는 물 상태 초기화
        if (!isRaining)
        {
            foreach (CropTile tile in container.crops)
            {
                tile.isWatered = false;
                VisualizeTile(tile);
            }
        }
        lastUpdatedDay = dayTimeController.days;
    }

    public bool Check(Vector3Int position)
    {
        CropTile tile = container.Get(position);
        if (tile == null) return false;

        //plowed 상태에서만 심을 수 있음
        TileBase baseTile = targetTilemap.GetTile(position);
        return baseTile == plowedTile || baseTile == watered;
    }

    public void Plow(Vector3Int position)
    {
        if (IsBlockedArea(position))
        {
            Debug.Log($"[Plow] 차단된 영역입니다: {position}");
            return;
        }

        CropTile tile = container.Get(position);

        if (tile != null)
        {
            tile.crop = null;
            tile.growTimer = 0;
            tile.growStage = 0;
            tile.damage = 0f;
            tile.isWatered = false;

            if (tile.renderer != null)
            {
                tile.renderer.sprite = null;
                tile.renderer.gameObject.SetActive(false);
            }

            targetTilemap.SetTile(position, plowedTile);
            targetTilemap.RefreshTile(position);
        }
        else
        {
            CreatePlowedTile(position);
        }
    }

    public void Seed(Vector3Int position, Crop toSeed)
    {
        Debug.Log($"[Seed] 호출됨! {toSeed.name} 심으려고 함, 계절 = {GameManager.instance.timeController.CurrentSeason}");

        if (IsBlockedArea(position))
        {
            Debug.Log($"[Water] 차단된 영역입니다: {position}");
            return;
        }

        CropTile tile = container.Get(position);
        if (tile == null) { return; }

        Season currentSeason = GameManager.instance.timeController.CurrentSeason;

        if (!toSeed.seasons.Contains(currentSeason))
        {
            Debug.Log($"[Seed] {toSeed.name}은 {currentSeason}에 심을 수 없습니다.");
            return;
        }

        tile.crop = toSeed;
        tile.growStage = 0;
        tile.growTimer = 0;

        VisualizeTile(tile);
    }

    public void VisualizeTile(CropTile cropTile)
    {
        if (cropTile.isWatered)
        {
            if (watered == null)
            {
                Debug.LogWarning("watered 타일이 null이야!! 인스펙터에서 연결했는지 확인해봐.");
            }

            targetTilemap.SetTile(cropTile.position, watered);
        }
        else
        {
            targetTilemap.SetTile(cropTile.position, plowedTile);
        }

        targetTilemap.RefreshTile(cropTile.position);

        if (cropTile.renderer == null)
        {
            GameObject go = Instantiate(cropsSpritePrefab, transform);
            go.transform.position = targetTilemap.CellToWorld(cropTile.position) + new Vector3(0.5f, 0.5f, 0);
            go.transform.position -= Vector3.forward * 0.01f;
            cropTile.renderer = go.GetComponent<SpriteRenderer>();
        }

        if (cropTile.crop != null && cropTile.crop.sprites.Count > 0)
        {
            int stage = cropTile.growStage;
            int lastIndex = cropTile.crop.sprites.Count - 1;

            cropTile.renderer.sprite = (stage >= cropTile.crop.sprites.Count)
                ? cropTile.crop.sprites[lastIndex]
                : cropTile.crop.sprites[stage];

            cropTile.renderer.gameObject.SetActive(true);
        }
        else
        {
            cropTile.renderer.sprite = null;
            cropTile.renderer.gameObject.SetActive(false);
        }
    }

    private void CreatePlowedTile(Vector3Int position)
    {
        CropTile crop = new CropTile();
        crop.position = position;
        container.Add(crop);

        VisualizeTile(crop);
        targetTilemap.SetTile(position, plowedTile);
    }

    internal void PickUp(Vector3Int gridPosition)
    {
        if (IsBlockedArea(gridPosition))
        {
            Debug.Log($"[PickUp] 차단된 영역입니다: {gridPosition}");
            return;
        }

        Vector2Int position = (Vector2Int)gridPosition;
        CropTile tile = container.Get(gridPosition);
        if (tile == null) { return; }

        if (tile.Complete)
        {
            ItemSpawnManager.instance.SpawnItem(
                targetTilemap.CellToWorld(gridPosition),
                tile.crop.yield,
                tile.crop.count
            );

            if (tile.renderer != null)
            {
                Destroy(tile.renderer.gameObject);
            }

            container.crops.Remove(tile);
            targetTilemap.SetTile(gridPosition, baseSoilTile);
        }
    }

    public void Water(Vector3Int position)
    {
        if (IsBlockedArea(position))
        {
            Debug.Log($"[Water] 차단된 영역입니다: {position}");
            return;
        }

        TileBase tile = targetTilemap.GetTile(position);

        Debug.Log($"[DEBUG] 현재 타일: {tile?.name}");
        Debug.Log($"[DEBUG] plowed 타일 이름: {plowedTile?.name}, seeded 타일 이름: {seeded?.name}");

        if (tile.name != seeded.name && tile.name != plowedTile.name)
        {
            Debug.LogWarning($"[Water] {position} 타일은 물 줄 수 없는 타일입니다.");
            return;
        }

        CropTile cropTile = container.Get(position);
        if (cropTile == null)
        {
            Debug.LogWarning($"[Water] {position} 위치에 CropTile이 없습니다.");
            return;
        }

        if (cropTile.crop == null)
        {
            Debug.LogWarning($"[Water] {position} 위치에 작물이 없습니다.");
            return;
        }

        if (cropTile.isWatered)
        {
            Debug.LogWarning($"[Water] {position} 위치는 이미 물을 준 상태입니다.");
            return;
        }

        cropTile.isWatered = true;
        targetTilemap.SetTile(position, watered);
        Debug.Log($"[Water] {position} 위치에 물을 주었습니다!");
    }

    private bool IsBlockedArea(Vector3Int pos)
    {
        return pos.x >= -20 && pos.x <= 26 && pos.y >= 9 && pos.y <= 14;
    }

    public void ForceResetLastUpdatedDay()
    {
        lastUpdatedDay = -1;
    }
}
