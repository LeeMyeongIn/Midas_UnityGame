using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SaveManager
{
    public static void SavePlayerData(PlayerDataForSave data, int slot)
    {
        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString($"SaveSlot{slot}", json);
        PlayerPrefs.Save();
    }

    public static PlayerDataForSave LoadPlayerData(int slot)
    {
        if (PlayerPrefs.HasKey($"SaveSlot{slot}"))
        {
            string json = PlayerPrefs.GetString($"SaveSlot{slot}");
            return JsonUtility.FromJson<PlayerDataForSave>(json);
        }
        return null;
    }

    public static bool HasSaveData(int slot)
    {
        return PlayerPrefs.HasKey($"SaveSlot{slot}");
    }
}
