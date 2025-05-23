using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sleep : MonoBehaviour
{
    DisableControls disableControls;
    Character character;
    DayTimeController dayTime;

    private void Awake()
    {
        disableControls = GetComponent<DisableControls>();
        character = GetComponent<Character>();
        dayTime = GameManager.instance.timeController;
    }

    internal void DoSleep()
    {
        StartCoroutine(SleepRoutine());
    }
    IEnumerator SleepRoutine()
    {
        ScreenTint scrrenTint = GameManager.instance.screenTint;

        disableControls.DisableControl();

        //scrrenTint.Tint();
        yield return new WaitForSeconds(2f);

        character.FullHeal();
        character.FullRest(0);
        SaveGame();  // 게임 저장 기능 추가
        dayTime.SkipToMorning();

        //scrrenTint.UnTint();
        yield return new WaitForSeconds(2f);

        disableControls.EnableControl();

        yield return null;
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
        Debug.Log("저장 완료");
    }
}
