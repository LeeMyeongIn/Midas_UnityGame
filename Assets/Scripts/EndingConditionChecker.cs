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
            missing.Add("�丮 ������ �ϼ��ؾ� �մϴ�.");
        if (CropSeenManager.Instance == null || !CropSeenManager.Instance.IsAllSeen())
            missing.Add("�۹� ������ �ϼ��ؾ� �մϴ�.");
        if (TriumphManager.Instance == null || !TriumphManager.Instance.AreAllRewardsClaimed())
            missing.Add("��� ���� ������ �����ؾ� �մϴ�.");
        if (!IsHouseLevel3())
            missing.Add("���� 3�ܰ���� ���׷��̵��ؾ� �մϴ�.");

        return string.Join("\n", missing);
    }

    private bool IsHouseLevel3()
    {
        var controller = GameObject.FindObjectOfType<HouseUpgradeController>();
        if (controller == null)
        {
            Debug.LogWarning("HouseUpgradeController�� ã�� �� �����ϴ�.");
            return false;
        }

        return controller.GetCurrentLevel() == 3;
    }
}
