using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TimeAgent))]
public class ObjectSpawner : MonoBehaviour
{
    [SerializeField] float spawnArea_height = 1f;
    [SerializeField] float spawnArea_width = 1f;

    [SerializeField] GameObject[] spawn;
    int length;
    [SerializeField] float probability = 0.1f;
    [SerializeField] int spawnCount = 1;

    [SerializeField] bool oneTime = false;

    List<SpawnedObject> spawnedObjects;

    [SerializeField] JSONStringList targetSaveJSONList;
    [SerializeField] int idInList = -1;

    private void Start()
    {
        length = spawn.Length;

        if(oneTime == false)
        {
            length = spawn.Length;
            TimeAgent timeAgent = GetComponent<TimeAgent>();
            timeAgent.onTimeTick += Spawn;
            spawnedObjects = new List<SpawnedObject>();

            LoadData();
        }
        else {
            Spawn();
            Destroy(gameObject);
        }
    }


    public void SpawnedObjectDestroyed(SpawnedObject spawnedObject)
    {
        spawnedObjects.Remove(spawnedObject);
    }

    void Spawn()
    {
        if(Random.value > probability) { return; }

        for (int i=0; i<spawnCount; i++)
        {
            int id = Random.Range(0, length);
            GameObject go = Instantiate(spawn[id]);
            Transform t = go.transform;

            if (oneTime == false)
            {
                t.SetParent(transform);
                SpawnedObject spawnedObject = go.AddComponent<SpawnedObject>();
                spawnedObjects.Add(spawnedObject);
                spawnedObject.objId = id;
            }

            Vector3 position = transform.position;
            position.x += UnityEngine.Random.Range(-spawnArea_width, spawnArea_width);
            position.y += UnityEngine.Random.Range(-spawnArea_height, spawnArea_height);

            t.position = position;
        }
    }

    public class ToSave
    {
        public List<SpawnedObject.SaveSpawnedObjectData> spawnedObejctDatas;

        public ToSave()
        {
            spawnedObejctDatas = new List<SpawnedObject.SaveSpawnedObjectData>();
        }
    }

    string Read()
    {
        ToSave toSave = new ToSave();

        Debug.Log($"[Read] 현재 spawnedObjects.Count = {spawnedObjects.Count}");

        for (int i = 0; i < spawnedObjects.Count; i++)
        {
            Debug.Log($"[Read] 저장 대상 id = {spawnedObjects[i].objId}, pos = {spawnedObjects[i].transform.position}");
            toSave.spawnedObejctDatas.Add(
                new SpawnedObject.SaveSpawnedObjectData(
                    spawnedObjects[i].objId,
                    spawnedObjects[i].transform.position
                    )
                );
        }
        string json = JsonUtility.ToJson(toSave);
        Debug.Log("[Read] 직렬화된 json = " + json);
        return json;
    }

    public void Load(string json)
    {
        if(json == "" || json == "{}" || json == null) { return; }

        ToSave toLoad = JsonUtility.FromJson<ToSave>(json);

        for(int i=0; i < toLoad.spawnedObejctDatas.Count; i++)
        {
            SpawnedObject.SaveSpawnedObjectData data = toLoad.spawnedObejctDatas[i];
            GameObject go = Instantiate(spawn[data.objectId]);
            go.transform.position = data.worldPosition;
            go.transform.SetParent(transform);
            SpawnedObject so = go.AddComponent<SpawnedObject>();
            so.objId = data.objectId;
            spawnedObjects.Add(so);
        }
    }

    private void OnDestroy()
    {
        SaveData();
    }

    private void SaveData()
    {
        Debug.Log("[SaveData] call");

        if (CheckJSON() == false)  { Debug.Log("checkJson false"); return; }

        string jsonString = Read();
        Debug.Log("[SaveData] jsonString make: " + jsonString);

        targetSaveJSONList.SetString(jsonString, idInList);
        Debug.Log("[SaveData] SetString finish");
    }

    private void LoadData()
    {
        Debug.Log("save start");
        if (CheckJSON() == false) { Debug.Log("save failed"); return; }

        Load(targetSaveJSONList.GetString(idInList));
        Debug.Log("Save finish");
    
    }

    private bool CheckJSON()
    {

        if (oneTime == true) { return false; }

        if (targetSaveJSONList == null)
        {
            Debug.LogError("target json scriptable object to save data on spawnable object is null!");
                return false;
        }
        if (idInList == -1)
        {
            Debug.LogError("id in list in not assigned data can't be saved!");
            return false;
        }
        return true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position, new Vector3(spawnArea_width * 2, spawnArea_height * 2));
    }
}
