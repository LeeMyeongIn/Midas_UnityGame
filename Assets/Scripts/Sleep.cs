using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sleep : MonoBehaviour
{
    DisableControls disableControls;
    Character character;
    DayTimeController dayTime;
    PlayerRespawn playerRespawn;

    bool isSleeping = false;

    private void Awake()
    {
        disableControls = GetComponent<DisableControls>();
        character = GetComponent<Character>();
        playerRespawn = FindObjectOfType<PlayerRespawn>();
        dayTime = GameManager.instance.timeController;
    }

    internal void DoSleep()
    {
        if (isSleeping) return;
        StartCoroutine(SleepRoutine());
    }
    IEnumerator SleepRoutine()
    {
        isSleeping = true;

        ScreenTint scrrenTint = GameManager.instance.screenTint;

        disableControls.DisableControl();

        scrrenTint.Tint();
        yield return new WaitForSeconds(2f);

        character.FullHeal();
        character.FullRest(0);
        SaveGame();  // 게임 저장 기능 추가
        dayTime.SkipToMorning();
        playerRespawn.StartRespawn();

        scrrenTint.UnTint();
        yield return new WaitForSeconds(2f);

        disableControls.EnableControl();

        isSleeping = false;

        yield return null;
    }

    private void SaveGame()
    {
        var data = CharacterGameManager.Instance.playerData;
        var time = GameManager.instance.timeController;

        PlayerDataForSave saveData = new PlayerDataForSave
        {
            // 플레이어 정보
            characterName = data.characterName,
            farmName = data.farmName,
            aboutTheFarm = data.aboutTheFarm,
            playerCharacterGender = data.playerCharacterGender,
            saveSlotId = data.saveSlotId,
            selectedCharacterIndex = data.selectedCharacterIndex,

            // 날짜/시간/계절 정보
            year = time.years,
            day = time.days,
            season = (int)time.CurrentSeason,
            time = GetCurrentTime(time)
        };

        SaveManager.SavePlayerData(saveData, data.saveSlotId);
        Debug.Log("저장 완료");
    }

    private float GetCurrentTime(DayTimeController timeController)
    {
        var type = typeof(DayTimeController);
        var field = type.GetField("time", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        return (float)field.GetValue(timeController);
    }

    public void SaveOnly()
    {
        SaveGame();
        Debug.Log("저장만 완료");
    }

    public void CloseSleepPanel()
    {
        Time.timeScale = 1f;
        GameObject panel = GameObject.Find("SleepChoicePanel");
        if (panel != null)
            panel.SetActive(false);
    }
}
