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

    // 작물
    public List<int> seenCrops = new List<int>();

    // 해금된 레시피
    public List<int> unlockedRecipes = new List<int>();

    // 해금된 업적
    public List<TriumphProgress> unlockedTriumphs = new List<TriumphProgress>();

    // 요리한 레시피 목록
    public List<int> cookedRecipes = new List<int>();
}
