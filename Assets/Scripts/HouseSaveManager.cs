using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class HouseSaveData
{
    public int level;
}

public static class HouseSaveManager
{
    private static string GetPath(int slot)
    {
        return Path.Combine(Application.persistentDataPath, $"house_slot_{slot}.json");
    }

    public static void SaveHouseLevel(int level, int slot)
    {
        if (level <= 0) level = 1;
        HouseSaveData data = new HouseSaveData { level = level };
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(GetPath(slot), json);
        Debug.Log($"[집 저장 완료] 슬롯 {slot}, 레벨 {level}");
    }

    public static int LoadHouseLevel(int slot)
    {
        string path = GetPath(slot);
        if (!File.Exists(path))
        {
            Debug.LogWarning($"[집 레벨 저장 파일 없음] 기본값 사용: 1");
            return 1; // 기본값 1로 반환
        }

        string json = File.ReadAllText(path);
        HouseSaveData data = JsonUtility.FromJson<HouseSaveData>(json);

        return data.level <= 0 ? 1 : data.level;
    }
}

[System.Serializable]
public class HouseLevelData
{
    public int level;
}