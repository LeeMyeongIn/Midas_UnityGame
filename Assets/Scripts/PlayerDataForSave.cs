using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerDataForSave
{
    // 저장 및 불러오기 용도
    public string characterName;
    public string farmName;
    public string aboutTheFarm;
    public Gender playerCharacterGender;
    public int saveSlotId;
    public int selectedCharacterIndex;
}
