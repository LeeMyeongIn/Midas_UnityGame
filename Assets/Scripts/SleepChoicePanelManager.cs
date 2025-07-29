using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SleepChoicePanelManager : MonoBehaviour
{
    public Button sleepButton;
    public Button saveOnlyButton;
    public Button saveExitButton;
    public Button cancelButton;

    private Sleep sleep;

    public ItemContainer inventoryContainer;
    public ItemList itemDB;

    public void OnSaveBeforeSleep()
    {
        int slot = PlayerPrefs.GetInt("SelectedSlot", 0);
        InventorySaveManager.SaveInventory(inventoryContainer, slot);
    }

    public void OnLoadGame()
    {
        int slot = SelectedSlotHolder.slotNumber;
        InventorySaveManager.SaveInventory(inventoryContainer, slot);
    }

    void Start()
    {
        sleep = FindObjectOfType<Sleep>();

        if (sleep == null)
        {
            Debug.LogWarning("Sleep 스크립트 찾을 수 없음");
            return;
        }

        sleepButton.onClick.AddListener(() =>
        {
            sleep.DoSleep();
            sleep.CloseSleepPanel();
        });

        saveOnlyButton.onClick.AddListener(() =>
        {
            int slot = SelectedSlotHolder.slotNumber;
            InventorySaveManager.SaveInventory(inventoryContainer, slot);
            int houseLevel = HouseUpgradeController.Instance.GetCurrentLevel();
            HouseSaveManager.SaveHouseLevel(houseLevel, slot);

            sleep.SaveOnly();
            sleep.CloseSleepPanel();
        });

        cancelButton.onClick.AddListener(() =>
        {
            sleep.CloseSleepPanel();
        });

        saveExitButton.onClick.AddListener(() =>
        {
            int slot = SelectedSlotHolder.slotNumber;
            InventorySaveManager.SaveInventory(inventoryContainer, slot);
            int houseLevel = HouseUpgradeController.Instance.GetCurrentLevel();
            HouseSaveManager.SaveHouseLevel(houseLevel, slot);

            sleep.SaveOnly();  // 저장
            sleep.CloseSleepPanel();  // 패널 닫기


#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();  // 종료
#endif
        });
    }
}
