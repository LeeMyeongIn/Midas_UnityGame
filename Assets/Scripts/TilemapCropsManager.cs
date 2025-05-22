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

    [SerializeField] public TileBase baseSoilTile; //���� �ٴ� �� Ÿ��

    //����, ��, ������ �� Ÿ��
    public TileBase baseSoil;
    public TileBase plowedTile;
    public TileBase watered;


    //���ο� �� �϶��� �����ϴ� ����
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

        // �Ϸ翡 �� ���� ����
        if (lastUpdatedDay == dayTimeController.days) return;
        lastUpdatedDay = dayTimeController.days;

        bool isRaining = dayTimeController.weatherManager != null && dayTimeController.weatherManager.IsRaining;

        // �� ���� ���� �ڵ� ���ֱ�
        if (dayTimeController.weatherManager != null && dayTimeController.weatherManager.IsRaining)
        {
            foreach (CropTile tile in container.crops)
            {
                tile.isWatered = true;
                VisualizeTile(tile);
            }
        }

        // �۹� ���� ó��
        foreach (CropTile cropTile in container.crops)
        {
            if (cropTile.crop == null) continue;

            // �翡 �� ���ָ� ���ڶ�
            if (cropTile.isWatered == false)
            {
                Debug.Log($"[Tick] {cropTile.position} Ÿ���� ���� �� �༭ �ڶ��� �ʽ��ϴ�.");
                continue;
            }

            // ���� ������ �ƴ� ��� �۹��� �ٷ� ����
            Season currentSeason = dayTimeController.CurrentSeason;
            if (!cropTile.crop.seasons.Contains(currentSeason))
            {
                Debug.Log($"[Tick] {cropTile.crop.name}�� {currentSeason}�� �ڶ� �� �����ϴ�. ��� ����.");
                cropTile.Harvested();
                targetTilemap.SetTile(cropTile.position, plowedTile);
                continue;
            }

            //�۹� ����
            cropTile.damage += 0.02f;
            if (cropTile.damage >= 1f)
            {
                cropTile.Harvested();
                targetTilemap.SetTile(cropTile.position, plowedTile);
                continue;
            }

            //��Ÿ�� �ٽ� ��Ÿ�Ϸ� ����
            if (cropTile.isWatered && !isRaining) //�� ���� ���� ����x
            {
                TileBase currentTile = targetTilemap.GetTile(cropTile.position);
                if (currentTile == watered)
                {
                    targetTilemap.SetTile(cropTile.position, plowedTile);
                }
            }

            if (cropTile.Complete) continue;

            cropTile.growTimer += 1;

            if (cropTile.growStage < cropTile.crop.growthStageTime.Count &&
                cropTile.growStage < cropTile.crop.sprites.Count &&
                cropTile.growTimer >= cropTile.crop.growthStageTime[cropTile.growStage])
            {
                // ���� Ÿ���� �����ϰ� �� Ÿ�Ϸ� ����
                targetTilemap.SetTile(cropTile.position, plowedTile);

                // �������� �����Ѵٸ� ��ġ�� ��������Ʈ ������Ʈ
                if (cropTile.renderer != null)
                {
                    // �۹� ��ġ ����: Ÿ�� �߽ɿ� ���߱�
                    Vector3 basePosition = targetTilemap.CellToWorld(cropTile.position);
                    Vector3 tileSize = targetTilemap.cellSize;
                    cropTile.renderer.transform.position = basePosition + new Vector3(tileSize.x * 0.5f, tileSize.y * 0.5f, 0f);
                    cropTile.renderer.transform.position -= Vector3.forward * 0.01f;

                    // ��������Ʈ ������Ʈ
                    cropTile.renderer.sprite = cropTile.crop.sprites[cropTile.growStage];
                    cropTile.renderer.gameObject.SetActive(true);
                }

                // ���� ���� �ܰ�� ����
                cropTile.growStage += 1;
            }
        }
        // �Ϸ� ���� �� �翡 �� �� ���� �ʱ�ȭ
        foreach (CropTile tile in container.crops)
        {
            tile.isWatered = false;
        }
    }

    public bool Check(Vector3Int position)
    {
        CropTile tile = container.Get(position);
        if (tile == null) return false;

        //plowed ���¿����� ���� �� ����
        TileBase baseTile = targetTilemap.GetTile(position);
        return baseTile == plowedTile;
    }

    public void Plow(Vector3Int position)
    {
        //�ܵ�κ�
        if (IsBlockedArea(position))
        {
            Debug.Log($"[Plow] ���ܵ� �����Դϴ�: {position}");
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

            // Ÿ���� �ð������� plowed�� ����
            targetTilemap.SetTile(position, plowedTile);
            targetTilemap.RefreshTile(position);
        }
        else
        {
            // cropTile�� �������� ������ ���� ����
            CreatePlowedTile(position);
        }
    }

    public void Seed(Vector3Int position, Crop toSeed)
    {
        //�ܵ��
        if (IsBlockedArea(position))
        {
            Debug.Log($"[Water] ���ܵ� �����Դϴ�: {position}");
            return;
        }

        CropTile tile = container.Get(position);
        if (tile == null) { return; }

        // ���� ���� ��������
        Season currentSeason = GameManager.instance.timeController.CurrentSeason;

        // �������� �۹��� ���� ������ ���� �� �ִ��� Ȯ��
        if (!toSeed.seasons.Contains(currentSeason))
        {
            Debug.Log($"[Seed] {toSeed.name}�� {currentSeason}�� ���� �� �����ϴ�.");
            return;
        }

        //�۹� �ߺ� ����
        tile.crop = toSeed;
        tile.growStage = 0;
        tile.growTimer = 0;

        VisualizeTile(tile);
    }

    public void VisualizeTile(CropTile cropTile)
    {
        // Ÿ�� �ð�ȭ ����
        if (cropTile.isWatered)
        {
            targetTilemap.SetTile(cropTile.position, watered);
        }
        else
        {
            targetTilemap.SetTile(cropTile.position, plowedTile);
        }

        targetTilemap.RefreshTile(cropTile.position);

        // SpriteRenderer ����
        if (cropTile.renderer == null)
        {
            GameObject go = Instantiate(cropsSpritePrefab, transform);
            go.transform.position = targetTilemap.CellToWorld(cropTile.position) + new Vector3(0.5f, 0.5f, 0);
            go.transform.position -= Vector3.forward * 0.01f;
            cropTile.renderer = go.GetComponent<SpriteRenderer>();
        }

        // �۹� ��������Ʈ ����
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
        //�ܵ��
        if (IsBlockedArea(gridPosition))
        {
            Debug.Log($"[PickUp] ���ܵ� �����Դϴ�: {gridPosition}");
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

            //�۹� ������ �ʱ�ȭ
            tile.Harvested();

            //���� ���� �⺻ ������ �ٲ�
            //targetTilemap.SetTile(gridPosition, null);
            targetTilemap.SetTile(gridPosition, baseSoilTile);
        }
    }

    public void Water(Vector3Int position)
    {
        //�ܵ��
        if (IsBlockedArea(position))
        {
            Debug.Log($"[Water] ���ܵ� �����Դϴ�: {position}");
            return;
        }

        // 1.Ÿ�ϸʿ��� �ش� ��ġ�� Ÿ���� ������
        TileBase tile = targetTilemap.GetTile(position);

        // 2.Ÿ�� �̸� Ȯ��
        Debug.Log($"[DEBUG] ���� Ÿ��: {tile?.name}");
        Debug.Log($"[DEBUG] plowed Ÿ�� �̸�: {plowedTile?.name}, seeded Ÿ�� �̸�: {seeded?.name}");

        // 3.Seeded / Plowed�� �ƴϸ� ���� �� �� ����
        if (tile.name != seeded.name && tile.name != plowedTile.name)
        {
            Debug.LogWarning($"[Water] {position} Ÿ���� �� �� �� ���� Ÿ���Դϴ�.");
            return;
        }

        // 4.CropTile�� �ִ��� Ȯ��
        CropTile cropTile = container.Get(position);
        if (cropTile == null)
        {
            Debug.LogWarning($"[Water] {position} ��ġ�� CropTile�� �����ϴ�.");
            return;
        }

        if (cropTile.crop == null)
        {
            Debug.LogWarning($"[Water] {position} ��ġ�� �۹��� �����ϴ�.");
            return;
        }

        // 5.���ֱ� �ߺ� ����
        if (cropTile.isWatered)
        {
            Debug.LogWarning($"[Water] {position} ��ġ�� �̹� ���� �� �����Դϴ�.");
            return;
        }

        // 6.�� �ֱ�
        cropTile.isWatered = true;
        targetTilemap.SetTile(position, watered);
        Debug.Log($"[Water] {position} ��ġ�� ���� �־����ϴ�!");
    }

    //�ܵ�κ� ��ǥ
    private bool IsBlockedArea(Vector3Int pos)
    {
        return pos.x >= -20 && pos.x <= 26 && pos.y >= 9 && pos.y <= 14;
    }
}