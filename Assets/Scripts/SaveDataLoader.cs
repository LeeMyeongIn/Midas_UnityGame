using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveDataLoader : MonoBehaviour
{
    private void Start()
    {
        Debug.Log("SaveDataLoader ����");

        var loaded = TemporaryLoadedData.LoadedData;
        if (loaded == null)
        {
            Debug.Log("�ҷ��� ���� �����Ͱ� �����ϴ�.");
            return;
        }

        if (CropSeenManager.Instance != null)
            CropSeenManager.Instance.SetSeenCropItemIds(loaded.seenCrops);
        if (RecipeUnlockManager.Instance != null)
            RecipeUnlockManager.Instance.SetUnlockedRecipes(loaded.unlockedRecipes);
        if (TriumphManager.Instance != null)
            TriumphManager.Instance.SetUnlockedTriumphs(loaded.unlockedTriumphs);
        if (CookingPanelManager.Instance != null)
            CookingPanelManager.Instance.SetCookedRecipes(loaded.cookedRecipes);

        Debug.Log("���� ������ �ҷ����� �Ϸ�");

        TemporaryLoadedData.LoadedData = null;
    }
}
