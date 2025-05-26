using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerDataForSave
{
    // �÷��̾� ����
    public string characterName;
    public string farmName;
    public string aboutTheFarm;
    public Gender playerCharacterGender;
    public int saveSlotId;
    public int selectedCharacterIndex;

    // ��¥/�ð�/���� ����
    public int year;
    public int day;
    public int season;
    public float time;
}
