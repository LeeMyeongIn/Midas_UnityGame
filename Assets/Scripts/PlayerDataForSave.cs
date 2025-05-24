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

    // �۹�
    public List<int> seenCrops = new List<int>();

    // �رݵ� ������
    public List<int> unlockedRecipes = new List<int>();

    // �رݵ� ����
    public List<TriumphProgress> unlockedTriumphs = new List<TriumphProgress>();

    // �丮�� ������ ���
    public List<int> cookedRecipes = new List<int>();
}
