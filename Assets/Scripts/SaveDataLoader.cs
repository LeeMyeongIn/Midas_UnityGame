using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveDataLoader : MonoBehaviour
{
    private void Start()
    {
        Debug.Log("SaveDataLoader 실행");

        var loaded = TemporaryLoadedData.LoadedData;
        if (loaded == null)
        {
            Debug.Log("불러올 저장 데이터가 없습니다.");
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

        Debug.Log("저장 데이터 불러오기 완료");

        TemporaryLoadedData.LoadedData = null;
    }
}
