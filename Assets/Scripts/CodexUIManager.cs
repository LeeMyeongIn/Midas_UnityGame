using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CodexUIManager : MonoBehaviour
{
    public static CodexUIManager Instance;

    [Header("Codex UI 전체 패널")]
    [SerializeField] private GameObject CodexUI;

    [Header("도감 패널")]
    [SerializeField] private GameObject CodexFoodPanel;
    [SerializeField] private GameObject CodexCropPanel;
    [SerializeField] private GameObject CodexTriumphPanel;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        if (CodexUI != null)
            CodexUI.SetActive(false);
    }



    public void OpenCodex()
    {
        if (CodexUI != null)
        {
            CodexUI.SetActive(true);
        }

        ShowFoodPanel();
    }


    public void CloseCodex()
    {
        if (CodexUI != null)
            CodexUI.SetActive(false);
    }


    public void ShowFoodPanel()
    {
        if (CodexFoodPanel != null) CodexFoodPanel.SetActive(true);
        if (CodexCropPanel != null) CodexCropPanel.SetActive(false);
        if (CodexTriumphPanel != null) CodexTriumphPanel.SetActive(false);
    }

    public void ShowCropPanel()
    {
        if (CodexFoodPanel != null) CodexFoodPanel.SetActive(false);
        if (CodexCropPanel != null) CodexCropPanel.SetActive(true);
        if (CodexTriumphPanel != null) CodexTriumphPanel.SetActive(false);
    }

    public void ShowTriumphPanel()
    {
        if (CodexFoodPanel != null) CodexFoodPanel.SetActive(false);
        if (CodexCropPanel != null) CodexCropPanel.SetActive(false);
        if (CodexTriumphPanel != null) CodexTriumphPanel.SetActive(true);
    }
}
