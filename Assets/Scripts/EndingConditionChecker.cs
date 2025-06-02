using System.Collections.Generic;
using UnityEngine;

public class EndingConditionChecker : MonoBehaviour
{
    public static EndingConditionChecker Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public bool IsEndingAvailable()
    {
        bool CookComplete = RecipeUnlockManager.Instance != null && RecipeUnlockManager.Instance.IsAllCooked();
        bool cropComplete = CropSeenManager.Instance != null && CropSeenManager.Instance.IsAllSeen();
        bool allRewardsClaimed = TriumphManager.Instance != null && TriumphManager.Instance.AreAllRewardsClaimed();
        bool houseUpgraded = IsHouseLevel3();

        return CookComplete && cropComplete && allRewardsClaimed && houseUpgraded;
    }

    public string GetMissingConditionMessage()
    {
        List<string> missing = new List<string>();

        if (RecipeUnlockManager.Instance == null || !RecipeUnlockManager.Instance.IsAllCooked())
            missing.Add("요리 도감을 완성해야 합니다.");
        if (CropSeenManager.Instance == null || !CropSeenManager.Instance.IsAllSeen())
            missing.Add("작물 도감을 완성해야 합니다.");
        if (TriumphManager.Instance == null || !TriumphManager.Instance.AreAllRewardsClaimed())
            missing.Add("모든 업적 보상을 수령해야 합니다.");
        if (!IsHouseLevel3())
            missing.Add("집을 3단계까지 업그레이드해야 합니다.");

        return string.Join("\n", missing);
    }

    private bool IsHouseLevel3()
    {
        var controller = GameObject.FindObjectOfType<HouseUpgradeController>();
        if (controller == null)
        {
            Debug.LogWarning("HouseUpgradeController를 찾을 수 없습니다.");
            return false;
        }

        return controller.GetCurrentLevel() == 3;
    }
}
