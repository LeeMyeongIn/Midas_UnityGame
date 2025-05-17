using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CookingPanelManager : MonoBehaviour
{
    public static CookingPanelManager Instance;

    [SerializeField] private GameObject cookingPanel;

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

    public void OpenPanel()
    {
        if (cookingPanel != null)
            cookingPanel.SetActive(true);
        else
            Debug.LogWarning("CookingPanel�� ������� �ʾҽ��ϴ�.");
    }

    public void ClosePanel()
    {
        if (cookingPanel != null)
            cookingPanel.SetActive(false);
        else
            Debug.LogWarning("CookingPanel�� ������� �ʾҽ��ϴ�.");
    }
}
