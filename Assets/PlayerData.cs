using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Player Data")]
public class PlayerData : ScriptableObject
{
    public string characterName;
    public string farmName;
    public string aboutTheFarm;
    public Gender playerCharacterGender;
    public int saveSlotId;
    public int selectedCharacterIndex;
}
