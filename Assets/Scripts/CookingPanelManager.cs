using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CookingPanelManager : MonoBehaviour
{
    public static CookingPanelManager Instance;

    [SerializeField] private GameObject cookingPanel;

    private HashSet<int> cookedRecipeIds = new HashSet<int>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void RegisterCookedRecipe(int recipeId)
    {
        cookedRecipeIds.Add(recipeId);
    }
    public bool HasCooked(int recipeId)
    {
        return cookedRecipeIds.Contains(recipeId);
    }

    public void OpenPanel()
    {
        if (cookingPanel != null)
            cookingPanel.SetActive(true);
        else
            Debug.LogWarning("CookingPanel이 연결되지 않았습니다.");
    }

    public void ClosePanel()
    {
        if (cookingPanel != null)
            cookingPanel.SetActive(false);
        else
            Debug.LogWarning("CookingPanel이 연결되지 않았습니다.");
    }
}
