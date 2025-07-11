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

    //������Ŭ��
    public List<Sprinkler> activeSprinklers = new List<Sprinkler>();

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
        if (targetTilemap == null) return;
        if (lastUpdatedDay == dayTimeController.days) return;

        lastUpdatedDay = dayTimeController.days;

        Debug.Log($"[Tick] ȣ���: day = {dayTimeController.days}, season = {dayTimeController.CurrentSeason}");

        bool isRaining = dayTimeController.weatherManager != null &&
                         dayTimeController.weatherManager.IsRaining;

        // �� ���� �� �ڵ� ���ֱ�
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

        // �۹� ó�� ����
        for (int i = container.crops.Count - 1; i >= 0; i--)
        {
            CropTile cropTile = container.crops[i];

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
                    Debug.Log($"[Tick] {cropTile.position} �� ��Ȯ �� �ؼ� 3�� �� ����");

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

            //�ڶ�� ���ε� �� �� ������ ���� ����
            if (!cropTile.isWatered)
            {
                Debug.Log($"[Tick] {cropTile.position} �� �� �� �༭ ���� ����");
                continue;
            }

            //���� ���� ����
            cropTile.growTimer++;

            if (cropTile.crop.growthStageTime == null || cropTile.crop.growthStageTime.Count == 0)
            {
                Debug.LogWarning($"[Tick] {cropTile.crop.name}�� growthStageTime�� ����ְų� null�Դϴ�!");
                continue;
            }

            int totalTime = 0;
            for (int stage = 0; stage <= cropTile.growStage; stage++)
            {
                totalTime += cropTile.crop.growthStageTime[stage];
            }

            Debug.Log($"[Tick] �� ��ġ {cropTile.position}, growStage: {cropTile.growStage}, growTimer: {cropTile.growTimer}, �ʿ� �ð�: {totalTime}, ��������Ʈ ��: {cropTile.crop.sprites.Count}");

            if (cropTile.growStage < cropTile.crop.growthStageTime.Count &&
                cropTile.growTimer >= totalTime)
            {
                cropTile.growStage++;
                VisualizeTile(cropTile);
            }
        }

        // �� �ȿ��� ���� sprinkler ���� �����ϰ� �� ���� �ʱ�ȭ
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
                    tile.isWatered = true;  // sprinkler ������ �׻� �� �ֱ�
                }
                else
                {
                    tile.isWatered = false; // sprinkler ���� �ƴϸ� �� �ʱ�ȭ
                }

                VisualizeTile(tile);
            }
        }
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
        Debug.Log($"[Seed] ȣ���! {toSeed.name} �������� ��, ���� = {GameManager.instance.timeController.CurrentSeason}");

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
        //���� ������ ������ watered tile
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

            if (tile.renderer != null)
            {
                Destroy(tile.renderer.gameObject);
            }

            container.crops.Remove(tile);


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

    //������ ��� ���� ��ǥ
    public bool IsBlockedArea(Vector3Int pos)
    {
        return
            //�ܵ�
            (pos.x >= -20 && pos.x <= 26 && pos.y >= 9 && pos.y <= 14)

            //��1
            || (pos.x >= -51 && pos.x <= -44 && pos.y >= 7 && pos.y <= 13)

            //��2, 3
            || (pos.x >= -53 && pos.x <= -39 && pos.y >= 7 && pos.y <= 14)

            //����
            || (pos.x >= 19 && pos.x <= 25 && pos.y >= 1 && pos.y <= 5);
    }

    //������Ŭ�� ����
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
    
    //������Ŭ�� ���
    public void RegisterSprinkler(Sprinkler sprinkler)
    {
        if (!activeSprinklers.Contains(sprinkler))
            activeSprinklers.Add(sprinkler);
    }
    //��� ����
    public void UnregisterSprinkler(Sprinkler sprinkler)
    {
        if (activeSprinklers.Contains(sprinkler))
            activeSprinklers.Remove(sprinkler);
    }
}