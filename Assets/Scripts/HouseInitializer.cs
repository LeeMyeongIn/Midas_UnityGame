using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseInitializer : MonoBehaviour
{
    private void Start()
    {
        int slot = SelectedSlotHolder.slotNumber;
        int savedLevel = HouseSaveManager.LoadHouseLevel(slot);

        Debug.Log($"[HouseInitializer] 슬롯: {slot}, 저장된 집 레벨: {savedLevel}");

        if (HouseUpgradeController.Instance != null)
        {
            HouseUpgradeController.Instance.UpgradeToLevel(savedLevel);
        }
    }

}
