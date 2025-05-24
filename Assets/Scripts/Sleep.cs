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

        PlayerDataForSave saveData = new PlayerDataForSave
        {
            characterName = data.characterName,
            farmName = data.farmName,
            aboutTheFarm = data.aboutTheFarm,
            playerCharacterGender = data.playerCharacterGender,
            saveSlotId = data.saveSlotId,
            selectedCharacterIndex = data.selectedCharacterIndex,

            seenCrops = CropSeenManager.Instance.GetSeenCropItemIds(),
            unlockedRecipes = RecipeUnlockManager.Instance.GetUnlockedRecipeIds(),
            unlockedTriumphs = TriumphManager.Instance.GetUnlockedTriumphIds(),
            cookedRecipes = CookingPanelManager.Instance.GetCookedRecipeIds()
        };

        SaveManager.SavePlayerData(saveData, data.saveSlotId);
        Debug.Log("저장 완료");
    }
}
