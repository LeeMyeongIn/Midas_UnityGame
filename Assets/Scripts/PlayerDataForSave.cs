using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerDataForSave
{
    // 플레이어 정보
    public string characterName;
    public string farmName;
    public string aboutTheFarm;
    public Gender playerCharacterGender;
    public int saveSlotId;
    public int selectedCharacterIndex;

    // 날짜/시간/계절 정보
    public int year;
    public int day;
    public int season;
    public float time;
}
