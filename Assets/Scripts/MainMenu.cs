using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] string nameNewGameStartScene;

    [SerializeField] PlayerData playerData;

    public Gender selectedGender;
    public TMPro.TMP_Text genderText;
    public TMPro.TMP_InputField nameInputField;
    public TMPro.TMP_InputField farmInputField;
    public TMPro.TMP_InputField aboutTheFarmInputField;

    AsyncOperation operation;

    private void Awake()
    {
        DontDestroyOnLoad(playerData);
    }

    private void Start()
    {
        SetGenderFemale();
        UpdateName();
        UpdateFarmName();
        UpdateAboutTheFarm();
    }

    public void ExitGame()
    {
        Debug.Log("Exit!");
        Application.Quit(); 
    }

    public void StartNewGame()
    {
        int slot = playerData.saveSlotId;

        if (SaveManager.HasSaveData(slot))
        {
            Debug.LogWarning($"슬롯 {slot}에는 이미 저장된 게임이 있어 새 게임을 시작할 수 없습니다.");
            return;
        }

        HouseSaveManager.SaveHouseLevel(1, playerData.saveSlotId);
        SceneManager.LoadScene(nameNewGameStartScene, LoadSceneMode.Single);
    }

    public void SetGenderMale()
    {
        selectedGender = Gender.Male;
        playerData.playerCharacterGender = selectedGender;
        genderText.text = "남자";
    }

    public void SetGenderFemale()
    {
        selectedGender = Gender.Female;
        playerData.playerCharacterGender = selectedGender;
        genderText.text = "여자";
    }

    public void UpdateName()
    {
        playerData.characterName = nameInputField.text;
    }

    public void UpdateFarmName()
    {
        playerData.farmName = farmInputField.text;
    }

    public void UpdateAboutTheFarm()
    {
        playerData.aboutTheFarm = aboutTheFarmInputField.text;
    }

    public void SetSavingSlot(int num)
    {
        playerData.saveSlotId = num;
    }
}
