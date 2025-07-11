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

    //스프링클러
    public List<Sprinkler> activeSprinklers = new List<Sprinkler>();

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
        if (targetTilemap == null) return;
        if (lastUpdatedDay == dayTimeController.days) return;

        lastUpdatedDay = dayTimeController.days;

        Debug.Log($"[Tick] 호출됨: day = {dayTimeController.days}, season = {dayTimeController.CurrentSeason}");

        bool isRaining = dayTimeController.weatherManager != null &&
                         dayTimeController.weatherManager.IsRaining;

        // 비 오는 날 자동 물주기
        if (isRaining)
        {
            foreach (CropTile tile in container.crops)
            {
                tile.isWatered = true;
                VisualizeTile(tile);
            }
        }

        foreach (Sprinkler sprinkler in activeSprinklers)
        {
            foreach (Vector3Int pos in sprinkler.GetTilesInRange())
            {
                TileBase tile = targetTilemap.GetTile(pos);
                if (tile == plowedTile || tile == watered)
                {
                    CropTile cropTile = container.Get(pos);
                    if (cropTile == null)
                    {
                        cropTile = new CropTile();
                        cropTile.position = pos;
                        container.Add(cropTile);
                    }

                    cropTile.isWatered = true;
                    VisualizeTile(cropTile);
                }
            }
        }

        // 작물 처리 루프
        for (int i = container.crops.Count - 1; i >= 0; i--)
        {
            CropTile cropTile = container.crops[i];

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

            //완전히 성장한 작물은 매일 tick 증가 (물 안 줘도 됨)
            if (cropTile.Complete)
            {
                cropTile.growTimer++;

                int totalGrowTime = 0;
                foreach (int t in cropTile.crop.growthStageTime)
                    totalGrowTime += t;

                if (cropTile.growTimer >= totalGrowTime + 3)
                {
                    Debug.Log($"[Tick] {cropTile.position} → 수확 안 해서 3일 후 썩음");

                    if (cropTile.renderer != null)
                        Destroy(cropTile.renderer.gameObject);

                    cropTile.crop = null;
                    cropTile.growStage = 0;
                    cropTile.growTimer = 0;
                    cropTile.damage = 0f;
                    cropTile.isWatered = false;

                    VisualizeTile(cropTile); // 밭은 유지
                }

                continue;
            }

            //자라는 중인데 물 안 줬으면 성장 멈춤
            if (!cropTile.isWatered)
            {
                Debug.Log($"[Tick] {cropTile.position} → 물 안 줘서 성장 멈춤");
                continue;
            }

            //정상 성장 진행
            cropTile.growTimer++;

            if (cropTile.crop.growthStageTime == null || cropTile.crop.growthStageTime.Count == 0)
            {
                Debug.LogWarning($"[Tick] {cropTile.crop.name}의 growthStageTime이 비어있거나 null입니다!");
                continue;
            }

            int totalTime = 0;
            for (int stage = 0; stage <= cropTile.growStage; stage++)
            {
                totalTime += cropTile.crop.growthStageTime[stage];
            }

            Debug.Log($"[Tick] ▶ 위치 {cropTile.position}, growStage: {cropTile.growStage}, growTimer: {cropTile.growTimer}, 필요 시간: {totalTime}, 스프라이트 수: {cropTile.crop.sprites.Count}");

            if (cropTile.growStage < cropTile.crop.growthStageTime.Count &&
                cropTile.growTimer >= totalTime)
            {
                cropTile.growStage++;
                VisualizeTile(cropTile);
            }
        }

        // 비 안오는 날엔 sprinkler 영역 제외하고 물 상태 초기화
        if (!isRaining)
        {
            foreach (CropTile tile in container.crops)
            {
                bool inSprinklerRange = false;
                foreach (Sprinkler sprinkler in activeSprinklers)
                {
                    foreach (Vector3Int pos in sprinkler.GetTilesInRange())
                    {
                        if (pos == tile.position)
                        {
                            inSprinklerRange = true;
                            break;
                        }
                    }
                    if (inSprinklerRange) break;
                }

                if (inSprinklerRange)
                {
                    tile.isWatered = true;  // sprinkler 범위는 항상 물 주기
                }
                else
                {
                    tile.isWatered = false; // sprinkler 범위 아니면 물 초기화
                }

                VisualizeTile(tile);
            }
        }
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
        //잔디부분
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

            // 타일을 시각적으로 plowed로 갱신
            targetTilemap.SetTile(position, plowedTile);
            targetTilemap.RefreshTile(position);
        }
        else
        {
            // cropTile이 존재하지 않으면 새로 생성
            CreatePlowedTile(position);
        }
    }

    public void Seed(Vector3Int position, Crop toSeed)
    {
        Debug.Log($"[Seed] 호출됨! {toSeed.name} 심으려고 함, 계절 = {GameManager.instance.timeController.CurrentSeason}");

        //잔디용
        if (IsBlockedArea(position))
        {
            Debug.Log($"[Water] 차단된 영역입니다: {position}");
            return;
        }

        CropTile tile = container.Get(position);
        if (tile == null) { return; }

        // 현재 계절 가져오기
        Season currentSeason = GameManager.instance.timeController.CurrentSeason;

        // 심으려는 작물이 현재 계절에 심을 수 있는지 확인
        if (!toSeed.seasons.Contains(currentSeason))
        {
            Debug.Log($"[Seed] {toSeed.name}은 {currentSeason}에 심을 수 없습니다.");

            return;
        }

        //작물 중복 방지
        tile.crop = toSeed;
        tile.growStage = 0;
        tile.growTimer = 0;

        VisualizeTile(tile);
    }

    public void VisualizeTile(CropTile cropTile)
    {
        //물을 줬으면 무조건 watered tile
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

        // SpriteRenderer 생성
        if (cropTile.renderer == null)
        {
            GameObject go = Instantiate(cropsSpritePrefab, transform);
            go.transform.position = targetTilemap.CellToWorld(cropTile.position) + new Vector3(0.5f, 0.5f, 0);
            go.transform.position -= Vector3.forward * 0.01f;
            cropTile.renderer = go.GetComponent<SpriteRenderer>();
        }

        // 작물 스프라이트 설정
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
        //잔디용
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


            //밭을 농장 기본 땅으로 바꿈
            //targetTilemap.SetTile(gridPosition, null);
            targetTilemap.SetTile(gridPosition, baseSoilTile);
        }
    }

    public void Water(Vector3Int position)
    {
        //잔디용
        if (IsBlockedArea(position))
        {
            Debug.Log($"[Water] 차단된 영역입니다: {position}");
            return;
        }

        // 1.타일맵에서 해당 위치의 타일을 가져옴
        TileBase tile = targetTilemap.GetTile(position);

        // 2.타일 이름 확인
        Debug.Log($"[DEBUG] 현재 타일: {tile?.name}");
        Debug.Log($"[DEBUG] plowed 타일 이름: {plowedTile?.name}, seeded 타일 이름: {seeded?.name}");

        // 3.Seeded / Plowed가 아니면 물을 줄 수 없음
        if (tile.name != seeded.name && tile.name != plowedTile.name)
        {
            Debug.LogWarning($"[Water] {position} 타일은 물 줄 수 없는 타일입니다.");
            return;
        }

        // 4.CropTile이 있는지 확인
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

        // 5.물주기 중복 방지
        if (cropTile.isWatered)
        {
            Debug.LogWarning($"[Water] {position} 위치는 이미 물을 준 상태입니다.");
            return;
        }

        // 6.물 주기
        cropTile.isWatered = true;
        targetTilemap.SetTile(position, watered);
        Debug.Log($"[Water] {position} 위치에 물을 주었습니다!");
    }

    //아이템 사용 금지 좌표
    public bool IsBlockedArea(Vector3Int pos)
    {
        return
            //잔디
            (pos.x >= -20 && pos.x <= 26 && pos.y >= 9 && pos.y <= 14)

            //집1
            || (pos.x >= -51 && pos.x <= -44 && pos.y >= 7 && pos.y <= 13)

            //집2, 3
            || (pos.x >= -53 && pos.x <= -39 && pos.y >= 7 && pos.y <= 14)

            //연못
            || (pos.x >= 19 && pos.x <= 25 && pos.y >= 1 && pos.y <= 5);
    }

    //스프링클러 범위
    private bool IsTileInAnySprinklerRange(Vector3Int pos)
    {
        foreach (Sprinkler sprinkler in activeSprinklers)
        {
            foreach (Vector3Int sprinklerPos in sprinkler.GetTilesInRange())
            {
                if (sprinklerPos == pos)
                    return true;
            }
        }
        return false;
    }
    
    //스프링클러 등록
    public void RegisterSprinkler(Sprinkler sprinkler)
    {
        if (!activeSprinklers.Contains(sprinkler))
            activeSprinklers.Add(sprinkler);
    }
    //등록 해제
    public void UnregisterSprinkler(Sprinkler sprinkler)
    {
        if (activeSprinklers.Contains(sprinkler))
            activeSprinklers.Remove(sprinkler);
    }
}