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
        Debug.Log("[TilemapCropsManager] Start() �����");
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
        Debug.Log($"[Tick] ȣ���: day = {dayTimeController.days}, season = {dayTimeController.CurrentSeason}");

        if (targetTilemap == null) return;
        if (lastUpdatedDay == dayTimeController.days) return;

        bool isRaining = dayTimeController.weatherManager != null &&
                         dayTimeController.weatherManager.IsRaining;

        Season currentSeason = dayTimeController.CurrentSeason;

        // �� ���� �� �ڵ� ���ֱ�
        if (isRaining)
        {
            foreach (CropTile tile in container.crops)
            {
                tile.isWatered = true;
                VisualizeTile(tile);
            }
        }

        // �۹� ó�� ����
        for (int i = container.crops.Count - 1; i >= 0; i--)
        {
            CropTile cropTile = container.crops[i];

            // ������ �� �´� �۹� ����
            if (cropTile.crop != null && !cropTile.crop.seasons.Contains(currentSeason))
            {
                Debug.Log($"[Tick] {cropTile.crop.name} �� {currentSeason}�� ���� �ʾ� ���ŵ�");

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

            // crop�� ���� Ÿ�� �� �� ���¸� ����
            if (cropTile.crop == null)
            {
                if (!isRaining && cropTile.isWatered)
                {
                    cropTile.isWatered = false;
                    targetTilemap.SetTile(cropTile.position, plowedTile);
                }
                continue;
            }

            //������ ������ �۹��� ���� tick ���� (�� �� �൵ ��)
            if (cropTile.Complete)
            {
                cropTile.growTimer++;

                int totalGrowTime = 0;
                foreach (int t in cropTile.crop.growthStageTime)
                    totalGrowTime += t;

                if (cropTile.growTimer >= totalGrowTime + 3)
                {
                    Debug.Log($"[Tick] {cropTile.position} �� ��ȯ �� �ؼ� 3�� �� ����");

                    if (cropTile.renderer != null)
                        Destroy(cropTile.renderer.gameObject);

                    cropTile.crop = null;
                    cropTile.growStage = 0;
                    cropTile.growTimer = 0;
                    cropTile.damage = 0f;
                    cropTile.isWatered = false;

                    VisualizeTile(cropTile); // ���� ����
                }

                continue;
            }

            //�ڶ�� ���ε� �� �� �������� ���� ������
            if (!cropTile.isWatered)
            {
                Debug.Log($"[Tick] {cropTile.position} �� �� �� �༭ ���� ������");
                continue;
            }

            // ��¥ ��� ���� ������� ��ȯ
            if (!cropTile.isWatered)
            {
                Debug.Log($"[Tick] {cropTile.position} �� �� �� �༭ ���� ����");
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

        // �� �� ���� ������ �� ���� �ʱ�ȭ
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

        //plowed ���¿����� ���� �� ����
        TileBase baseTile = targetTilemap.GetTile(position);
        return baseTile == plowedTile || baseTile == watered;
    }

    public void Plow(Vector3Int position)
    {
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
        Debug.Log($"[Seed] ȣ���! {toSeed.name} �������� ��, ���� = {GameManager.instance.timeController.CurrentSeason}");

        if (IsBlockedArea(position))
        {
            Debug.Log($"[Water] ���ܵ� �����Դϴ�: {position}");
            return;
        }

        CropTile tile = container.Get(position);
        if (tile == null) { return; }

        Season currentSeason = GameManager.instance.timeController.CurrentSeason;

        if (!toSeed.seasons.Contains(currentSeason))
        {
            Debug.Log($"[Seed] {toSeed.name}�� {currentSeason}�� ���� �� �����ϴ�.");
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
                Debug.LogWarning("watered Ÿ���� null�̾�!! �ν����Ϳ��� �����ߴ��� Ȯ���غ�.");
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
            Debug.Log($"[Water] ���ܵ� �����Դϴ�: {position}");
            return;
        }

        TileBase tile = targetTilemap.GetTile(position);

        Debug.Log($"[DEBUG] ���� Ÿ��: {tile?.name}");
        Debug.Log($"[DEBUG] plowed Ÿ�� �̸�: {plowedTile?.name}, seeded Ÿ�� �̸�: {seeded?.name}");

        if (tile.name != seeded.name && tile.name != plowedTile.name)
        {
            Debug.LogWarning($"[Water] {position} Ÿ���� �� �� �� ���� Ÿ���Դϴ�.");
            return;
        }

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

        if (cropTile.isWatered)
        {
            Debug.LogWarning($"[Water] {position} ��ġ�� �̹� ���� �� �����Դϴ�.");
            return;
        }

        cropTile.isWatered = true;
        targetTilemap.SetTile(position, watered);
        Debug.Log($"[Water] {position} ��ġ�� ���� �־����ϴ�!");
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
