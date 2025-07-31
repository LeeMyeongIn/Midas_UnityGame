using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SlotButton : MonoBehaviour
{
    public int slotNumber;
    public enum Mode { Save, Load }
    public Mode mode;
    public Text slotLabel;

    public ItemContainer inventoryContainer;
    public ItemList itemList;

    private void Start()
    {
        if (mode == Mode.Load && slotLabel != null)
        {
            if (SaveManager.HasSaveData(slotNumber))
                slotLabel.text = $"슬롯 {slotNumber}: 저장됨";
            else
                slotLabel.text = $"슬롯 {slotNumber}: 비어있음";
        }
    }

    public void OnClickSlot()
    {
        SelectedSlotHolder.slotNumber = slotNumber;
        Debug.Log("선택한 슬롯: " + slotNumber);

        if (mode == Mode.Save)
        {
            var so = CharacterGameManager.Instance.playerData;
            var saveData = new PlayerDataForSave
            {
                characterName = so.characterName,
                farmName = so.farmName,
                aboutTheFarm = so.aboutTheFarm,
                playerCharacterGender = so.playerCharacterGender,
                saveSlotId = slotNumber,
                selectedCharacterIndex = so.selectedCharacterIndex,
                weatherState = WeatherManager.Instance.GetWeather()
            };

            SaveManager.SavePlayerData(saveData, slotNumber);

            InventorySaveManager.SaveInventory(inventoryContainer, slotNumber);

            SceneManager.LoadScene("FarmingScene", LoadSceneMode.Single);
            SceneManager.LoadScene("Essential", LoadSceneMode.Additive);
            SceneManager.LoadScene("HelpScene", LoadSceneMode.Additive);
        }
        else if (mode == Mode.Load)
        {
            if (SaveManager.HasSaveData(slotNumber))
            {
                var loaded = SaveManager.LoadPlayerData(slotNumber);
                var so = CharacterGameManager.Instance.playerData;

                so.characterName = loaded.characterName;
                so.farmName = loaded.farmName;
                so.aboutTheFarm = loaded.aboutTheFarm;
                so.playerCharacterGender = loaded.playerCharacterGender;
                so.saveSlotId = loaded.saveSlotId;
                so.selectedCharacterIndex = loaded.selectedCharacterIndex;

                CharacterGameManager.Instance.loadedSaveData = loaded;

                PlayerPrefs.SetInt("LoadedWeatherState", (int)loaded.weatherState);

                SceneManager.LoadScene("FarmingScene", LoadSceneMode.Single);
                SceneManager.LoadScene("Essential", LoadSceneMode.Additive);
                SceneManager.LoadScene("HelpScene", LoadSceneMode.Additive);
            }
            else
            {
                Debug.Log("이 슬롯에서는 저장된 데이터가 없습니다.");
            }
        }
    }
}
