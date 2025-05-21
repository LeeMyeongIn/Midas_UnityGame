using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BedSaveTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            SaveGame();
        }
    }

    private void SaveGame()
    {
        var data = CharacterGameManager.Instance.playerData;

        PlayerDataForSave saveData = new PlayerDataForSave
        {
            characterName = data.characterName,
            farmName = data.farmName,
            aboutTheFarm = data.aboutTheFarm,
            playerCharacterGender = data.playerCharacterGender,
            saveSlotId = data.saveSlotId,
            selectedCharacterIndex = data.selectedCharacterIndex
        };

        SaveManager.SavePlayerData(saveData, data.saveSlotId);

        Debug.Log("게임이 저장되었습니다.");
    }
}