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

    [SerializeField] private TileBase baseSoilTile; //���� �ٴ� �� Ÿ��


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

        // �Ϸ翡 �� ���� ����
        if (lastUpdatedDay == dayTimeController.days) return;
            lastUpdatedDay = dayTimeController.days;



        // �� ���� ���� �ڵ� ���ֱ�
        if (dayTimeController.weatherManager != null && dayTimeController.weatherManager.IsRaining)
        {
            foreach (CropTile tile in container.crops)
            {
                tile.isWatered = true;
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
                targetTilemap.SetTile(cropTile.position, plowed);
                continue;
            }

            //�۹� ����
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
                // ���� Ÿ���� �����ϰ� �� Ÿ�Ϸ� ����
                targetTilemap.SetTile(cropTile.position, plowed);

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

        // ���� ���� ��������
        Season currentSeason = GameManager.instance.timeController.CurrentSeason;

        // �������� �۹��� ���� ������ ���� �� �ִ��� Ȯ��
        if (!toSeed.seasons.Contains(currentSeason))
        {
            Debug.Log($"[Seed] {toSeed.name}�� {currentSeason}�� ���� �� �����ϴ�.");
            return;
        }

        targetTilemap.SetTile(position, seeded);

        tile.crop = toSeed;

        //�۹� �ߺ� ����
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

            //�۹� ������ �ʱ�ȭ
            tile.Harvested();

            //���� ���� �⺻ ������ �ٲ�
            targetTilemap.SetTile(gridPosition, baseSoilTile);
        }
    }

    public void Water(Vector3Int position)
    {
        // 1.Ÿ�ϸʿ��� �ش� ��ġ�� Ÿ���� ������
        TileBase tile = targetTilemap.GetTile(position);

        // 2.Ÿ�� �̸� Ȯ��
        Debug.Log($"[DEBUG] ���� Ÿ��: {tile?.name}");
        Debug.Log($"[DEBUG] plowed Ÿ�� �̸�: {plowed?.name}, seeded Ÿ�� �̸�: {seeded?.name}");

        // 3.Seeded / Plowed�� �ƴϸ� ���� �� �� ����
        if (tile == null || (tile.name != seeded.name && tile.name != plowed.name))
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
        Debug.Log($"[Water] {position} ��ġ�� ���� �־����ϴ�!");
    }

}