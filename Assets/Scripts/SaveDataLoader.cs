using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveDataLoader : MonoBehaviour
{
    void Start()
    {
        var loaded = CharacterGameManager.Instance.loadedSaveData;
        if (loaded == null) return;

        var time = GameManager.instance?.timeController;
        if(time != null)
        {
            time.SetDate(loaded.year, loaded.day);
            time.SetSeason(loaded.season);
            time.SetTime(loaded.time);
        }

        CharacterGameManager.Instance.loadedSaveData = null;
    }
}
