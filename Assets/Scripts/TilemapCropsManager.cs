using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph;
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

        if (renderer != null)
        {
            renderer.gameObject.SetActive(false);
        }
    }
}

public class TilemapCropsManager : TimeAgent
{
    [SerializeField] TileBase plowed;
    [SerializeField] TileBase seeded;

    Tilemap targetTilemap;

    [SerializeField] GameObject cropsSpritePrefab;

    [SerializeField] CropsContainer container;

    [SerializeField] private TileBase baseSoilTile; //농장 바닥 땅 타일


    //새로운 날 일때만 증가하는 조건
    private int lastUpdatedDay = -1;

    public Tilemap GetTilemap()
    {
        return targetTilemap;
    }

    private void Start()
    {
        GameManager.instance.GetComponent<CropsManager>().cropsManager = this;
        targetTilemap = GetComponent<Tilemap>();
        onTimeTick += Tick;
        Init();
        VisualizeMap();
    }

    private void VisualizeMap()
    {
        for(int i = 0; i < container.crops.Count; i++)
        {
            VisualizeTile(container.crops[i]);
        }
    }

    private void OnDestroy()
    {
        for(int i = 0; i < container.crops.Count; i++)
        {
            container.crops[i].renderer = null;
        }
    }

    public void Tick(DayTimeController dayTimeController)
    {
        if (targetTilemap == null) return;

        // 하루에 한 번만 성장
        if (lastUpdatedDay == dayTimeController.days) return;
            lastUpdatedDay = dayTimeController.days;

        foreach (CropTile cropTile in container.crops)
        {
            if (cropTile.crop == null) continue;

            //현재 계절이 아닌 경우 작물이 바로 상함
            Season currentSeason = dayTimeController.CurrentSeason;
            if (!cropTile.crop.seasons.Contains(currentSeason))
            {
                Debug.Log($"[Tick] {cropTile.crop.name}은 {currentSeason}에 자랄 수 없습니다. 즉시 상함.");
                cropTile.Harvested();
                targetTilemap.SetTile(cropTile.position, plowed);
                continue;
            }

            cropTile.damage += 0.02f;
            if (cropTile.damage >= 1f)
            {
                cropTile.Harvested();
                targetTilemap.SetTile(cropTile.position, plowed);
                continue;
            }

            if (cropTile.Complete) continue;

            cropTile.growTimer += 1;

            if (cropTile.growStage < cropTile.crop.growthStageTime.Count &&
                cropTile.growStage < cropTile.crop.sprites.Count &&
                cropTile.growTimer >= cropTile.crop.growthStageTime[cropTile.growStage])
            {
                // 씨앗 타일을 제거하고 밭 타일로 덮기
                targetTilemap.SetTile(cropTile.position, plowed);

                // 렌더러가 존재한다면 위치와 스프라이트 업데이트
                if (cropTile.renderer != null)
                {
                    // 위치 보정: 타일 중심에 맞추기
                    Vector3 basePosition = targetTilemap.CellToWorld(cropTile.position);
                    Vector3 tileSize = targetTilemap.cellSize;
                    cropTile.renderer.transform.position = basePosition + new Vector3(tileSize.x * 0.5f, tileSize.y * 0.5f, 0f);
                    cropTile.renderer.transform.position -= Vector3.forward * 0.01f;

                    // 스프라이트 업데이트
                    cropTile.renderer.sprite = cropTile.crop.sprites[cropTile.growStage];
                    cropTile.renderer.gameObject.SetActive(true);
                }

                // 다음 성장 단계로 진행
                cropTile.growStage += 1;
            }

        }
    }

    internal bool Check(Vector3Int position)
    {
        return container.Get(position) != null;
    }

    public void Plow(Vector3Int position)
    {   
        if(Check(position) == true) { return; }
        CreatePlowedTile(position);
    }

    public void Seed(Vector3Int position, Crop toSeed)
    {
        CropTile tile = container.Get(position);
        if(tile == null) { return; }

        // 현재 계절 가져오기
        Season currentSeason = GameManager.instance.timeController.CurrentSeason;

        // 심으려는 작물이 현재 계절에 심을 수 있는지 확인
        if (!toSeed.seasons.Contains(currentSeason))
        {
            Debug.Log($"[Seed] {toSeed.name}은 {currentSeason}에 심을 수 없습니다.");
            return;
        }

        targetTilemap.SetTile(position, seeded);

        tile.crop = toSeed;

        //작물 중복 방지
        tile.growStage = 0;
        tile.growTimer = 0;

    }

    public void VisualizeTile(CropTile cropTile)
    {
        targetTilemap.SetTile(cropTile.position, cropTile.crop != null ? seeded : plowed);       

        if (cropTile.renderer == null)
        {
            GameObject go = Instantiate(cropsSpritePrefab, transform);
            go.transform.position = targetTilemap.CellToWorld(cropTile.position);
            go.transform.position -= Vector3.forward * 0.01f;
            cropTile.renderer = go.GetComponent<SpriteRenderer>();
        }

        bool growing = 
            cropTile.crop != null && 
            cropTile.growTimer >= cropTile.crop.growthStageTime[0];

        cropTile.renderer.gameObject.SetActive(growing);
        if (growing && cropTile.growStage > 0 && cropTile.growStage <= cropTile.crop.sprites.Count)
        {
            cropTile.renderer.sprite = cropTile.crop.sprites[cropTile.growStage - 1];
        }

    }

    private void CreatePlowedTile(Vector3Int position)
    {
        CropTile crop = new CropTile();
        container.Add(crop);       

        crop.position = position;

        VisualizeTile(crop);

        targetTilemap.SetTile(position, plowed);
    }

    internal void PickUp(Vector3Int gridPosition)
    {
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

            //작물 데이터 초기화
            tile.Harvested();

            //밭을 농장 기본 땅으로 바꿈
            targetTilemap.SetTile(gridPosition, baseSoilTile);
        }
    }
}